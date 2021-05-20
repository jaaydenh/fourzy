//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Menu;
using Fourzy._Updates.UI.Menu.Screens;
using FourzyGameModel.Model;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class MainMenuAreaWidget : WidgetBase
    {
        public TMP_Text areaLabel;
        public TMP_Text progressionValue;
        public Image areaImage;
        public Slider progressionSlider;
        public ValueTween sliderValueTween;
        public Image rewardParent;
        public GameObject noNextReward;

        private bool isPlayFabInitialized = false;
        private GameObject currentRewardItem;
        private int cachedValue = -1;

        private AreaProgression progressionData = null;
        private AreaProgressionEntry previousReward;
        private AreaProgressionEntry nextReward;
        private BundleItem rewardItem;
        private int currentValue = -1;
        private Area currentArea;

        protected void Start()
        {
            UserManager.onAreaProgression += OnAreaProgression;
            UserManager.onPlayfabValuesLoaded += OnPlayfabValueLoaded;
        }

        protected void OnDestroy()
        {
            UserManager.onAreaProgression -= OnAreaProgression;
            UserManager.onPlayfabValuesLoaded -= OnPlayfabValueLoaded;
        }

        public override void _Update()
        {
            base._Update();

            if (cachedValue != -1)
            {
                OnAreaProgression(currentArea, cachedValue);

                cachedValue = -1;
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
            if (!isPlayFabInitialized || area != currentArea || currentValue == value) return;

            if (!visible)
            {
                cachedValue = value;

                return;
            }

            currentValue = value;

            previousReward = progressionData.GetCurrent(currentValue);
            nextReward = progressionData.GetNext(currentValue);

            UpdateLabel(value);
            UpdateSlider(true);
            UpdateBundleReward();
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

        private void UpdateLabel(int gamesPlayed)
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
            float from = progressionSlider.value;
            float to;

            if (nextReward == null)
            {
                progressionSlider.value = 0f;

                return;
            }
            else
            {
                if (previousReward == null)
                {
                    progressionSlider.maxValue = nextReward.gamesNumber;
                    to = currentValue;
                }
                else
                {
                    progressionSlider.maxValue = nextReward.gamesNumber - previousReward.gamesNumber;
                    to = currentValue - previousReward.gamesNumber;
                }
            }

            if (animate)
            {
                sliderValueTween.from = from;
                sliderValueTween.to = to;

                sliderValueTween.PlayForward(true);
            }
            else
            {
                progressionSlider.value = to;
            }
        }

        private void UpdateBundleReward()
        {
            if (currentRewardItem)
            {
                Destroy(currentRewardItem);
            }
            rewardParent.color = Color.clear;

            if (nextReward == null)
            {
                noNextReward.SetActive(true);

                return;
            }
            else
            {
                noNextReward.SetActive(false);
            }

            rewardItem = GameContentManager.Instance.allBundlesInfo[nextReward.id].GetFirstItem();

            switch (rewardItem.ItemClass)
            {
                case Constants.PLAYFAB_GAMEPIECE_CLASS:
                    GamePieceView _gamepiece = Instantiate(
                        GameContentManager.Instance.piecesDataHolder.GetGamePieceData(rewardItem.ItemId).player1Prefab,
                        rewardParent.rectTransform);
                    _gamepiece.StartBlinking();

                    currentRewardItem = _gamepiece.gameObject;
                    rewardParent.color = Color.clear;

                    break;

                case Constants.PLAYFAB_TOKEN_CLASS:
                    rewardParent.sprite = GameContentManager.Instance.GetTokenData((TokenType)Enum.Parse(typeof(TokenType), rewardItem.ItemId)).GetTokenSprite();
                    rewardParent.color = Color.white;

                    break;
            }
        }
    }
}