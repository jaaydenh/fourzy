//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Menu;
using Fourzy._Updates.UI.Menu.Screens;
using FourzyGameModel.Model;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class MainMenuAreaWidget : WidgetBase
    {
        [SerializeField]
        private TMP_Text areaLabel;
        [SerializeField]
        private TMP_Text progressionValue;
        [SerializeField]
        private Image areaImage;
        [SerializeField]
        private Slider progressionSlider;
        [SerializeField]
        private ValueTween sliderValueTween;
        [SerializeField]
        private Image rewardParent;
        [SerializeField]
        private GameObject noReward;
        [SerializeField]
        private ProgressionRewardAnimatorWidget rewardAnimation;

        private bool isPlayFabInitialized = false;
        private bool updateRewardOnOpen;
        private int cachedValue = 0;

        private AreaProgression progressionData = null;
        private AreaProgressionEntry previousReward;
        private AreaProgressionEntry nextReward;
        private BundleItem rewardItem;
        private int gamesPlayed = 0;
        private Area currentArea;

        protected void Start()
        {
            UserManager.onAreaProgression += OnAreaProgression;
            UserManager.onPlayfabValuesLoaded += OnPlayfabValueLoaded;
        }

        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                menuScreen.menuController
                    .GetOrAddScreen<AreaProgressionRewardScreen>()
                    .ShowReward(TokenType.ARROW);
            }
        }

        protected void OnDestroy()
        {
            UserManager.onAreaProgression -= OnAreaProgression;
            UserManager.onPlayfabValuesLoaded -= OnPlayfabValueLoaded;
        }

        public override void _Update()
        {
            base._Update();

            //this called ecverytime playScreen is opened
            //so using code below i can pass unlocked rewards
            //one by one even if we skipped thought them
            if (cachedValue != 0)
            {
                //cached rewards are set when we add value to progression, but this screen isn't visible

                //check if there is next reward for current amount of gamesPlayed (not same as cached value)
                AreaProgressionEntry _next = progressionData.GetNext(gamesPlayed);

                if (_next != null && cachedValue >= _next.gamesNumber)
                {
                    //if there is next reward and our cached value is enough to unlock it, 
                    //pass _next.gamesNumber into OnAreaProgression to trigger reward manually
                    OnAreaProgression(currentArea, _next.gamesNumber);
                    return;
                }
                else
                {
                    //otherwise just progress to current value
                    OnAreaProgression(currentArea, cachedValue);
                    cachedValue = 0;
                }
            }
            else if (updateRewardOnOpen)
            {
                updateRewardOnOpen = false;

                StartRoutine("delayed_update", Constants.AREA_PROGRESSION_BEFORE_DELAY, () =>
                {
                    UpdateSlider(true);
                    UpdateBundleReward();
                    UpdateLabel();
                });
            }
        }

        public void OnRewardTap()
        {
            if (rewardItem == null) return;

            switch (rewardItem.ItemClass)
            {
                case Constants.PLAYFAB_TOKEN_CLASS:
                    PersistantMenuController.Instance
                        .GetOrAddScreen<TokenPrompt>()
                        .Prompt(GameContentManager.Instance.GetTokenData((TokenType)Enum.Parse(typeof(TokenType), rewardItem.ItemId)));

                    break;

                case Constants.PLAYFAB_GAMEPIECE_CLASS:
                    //nothing

                    break;
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            AreasDataHolder.GameArea data = GameContentManager.Instance.currentArea;

            areaLabel.text = LocalizationManager.Value(data.name);
            areaImage.sprite = data.square;
            currentArea = data.areaID;
        }

        private void OnAreaProgression(Area area, int value)
        {
            if (!isPlayFabInitialized || area != currentArea) return;

            if (!visible)
            {
                cachedValue = value;
                return;
            }

            gamesPlayed = value;

            AreaProgressionEntry _next = nextReward;
            previousReward = progressionData.GetCurrent(gamesPlayed);
            nextReward = progressionData.GetNext(gamesPlayed);

            CancelRoutine("delayed_update");
            if (_next != null && _next != nextReward)
            {
                StartRoutine("reward_routine", UnlockRewardRoutine());
            }
            else
            {
                StartRoutine("delayed_update", Constants.AREA_PROGRESSION_BEFORE_DELAY, () =>
                {
                    UpdateSlider(true);
                    UpdateBundleReward();
                    UpdateLabel();
                });
            }
        }

        private void OnPlayfabValueLoaded()
        {
            //ini slider
            if (!isPlayFabInitialized && UserManager.Instance.IsPlayfabValueLoaded(PlayfabValuesLoaded.TITLE_DATA_RECEIVED, PlayfabValuesLoaded.PLAYER_STATS_RECEIVED, PlayfabValuesLoaded.BUNDLES_INFO_RECEIVED))
            {
                switch (currentArea)
                {
                    case Area.TRAINING_GARDEN:
                        progressionData = InternalSettings.Current.TRAINING_GARDEN;

                        break;

                    case Area.ENCHANTED_FOREST:
                        progressionData = InternalSettings.Current.ENCHANTED_FOREST;

                        break;

                    case Area.SANDY_ISLAND:
                        progressionData = InternalSettings.Current.SANDY_ISLAND;

                        break;

                    case Area.ICE_PALACE:
                        progressionData = InternalSettings.Current.ICE_PALACE;

                        break;
                }

                isPlayFabInitialized = true;
                OnAreaProgression(currentArea, UserManager.Instance.GetAreaProgression(currentArea));
            }
        }

        private void UpdateFromCurrentValues()
        {
        }

        private void UpdateLabel()
        {
            if (nextReward == null)
            {
                progressionValue.text = "-/-";
            }
            else
            {
                if (previousReward == null)
                {
                    progressionValue.text = $"{gamesPlayed}/{nextReward.gamesNumber}";
                }
                else
                {
                    progressionValue.text = $"{gamesPlayed - previousReward.gamesNumber}/{nextReward.gamesNumber - previousReward.gamesNumber}";
                }
            }
        }

        private void UpdateSlider(bool animate)
        {
            float to;

            if (nextReward == null)
            {
                progressionSlider.value = 0f;

                return;
            }
            else
            {
                to = previousReward == null ?
                    (float)gamesPlayed / nextReward.gamesNumber :
                    (float)(gamesPlayed - previousReward.gamesNumber) / (nextReward.gamesNumber - previousReward.gamesNumber);
            }

            UpdateSlider(animate, to);
        }

        private void UpdateSlider(bool animate, float value)
        {
            if (animate)
            {
                sliderValueTween.from = progressionSlider.value;
                sliderValueTween.to = value;

                sliderValueTween.PlayForward(true);
            }
            else
            {
                progressionSlider.value = value;
            }
        }

        private void UpdateBundleReward()
        {
            noReward.SetActive(nextReward == null);

            if (nextReward == null)
            {
                rewardAnimation.RemoveCurrentReward();
                return;
            }

            BundleItem _current = GameContentManager.Instance.allBundlesInfo[nextReward.id].GetFirstItem();
            if (rewardItem != _current)
            {
                rewardItem = GameContentManager.Instance.allBundlesInfo[nextReward.id].GetFirstItem();
                rewardAnimation.SetReward(rewardItem);
            }
            else if (rewardAnimation)
            {
                rewardAnimation.AnimateProgress();
            }
        }

        private IEnumerator UnlockRewardRoutine()
        {
            AreaProgressionRewardScreen _rewardScreen = menuScreen.menuController.GetOrAddScreen<AreaProgressionRewardScreen>();
            //this screen is transparent at first
            //and will also block inputs
            _rewardScreen.ShowReward((TokenType)Enum.Parse(typeof(TokenType), rewardItem.ItemId));

            progressionValue.text = "";

            yield return new WaitForSeconds(Constants.AREA_PROGRESSION_BEFORE_DELAY);

            rewardAnimation.Animate(Constants.AREA_PROGRESSION_OPEN_SCREEN_DELAY);
            UpdateSlider(true, 1f);

            //animate reward
            yield return new WaitForSeconds(Constants.AREA_PROGRESSION_OPEN_SCREEN_DELAY);

            _rewardScreen.ActualOpen();

            updateRewardOnOpen = true;
        }
    }
}