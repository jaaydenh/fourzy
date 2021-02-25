//@vadym udod

using ByteSheep.Events;
using Fourzy._Updates.Audio;
using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.Rewards;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Menu;
using Fourzy._Updates.UI.Menu.Screens;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class RewardsScreenWidget : WidgetBase
    {
        public AudioTypes onShow;
        public float volume = 1f;
        public bool swapLabels = false;
        public RectTransform content;

        public string format = "{0}";
        public GameObject bgImage;
        public GameObject checkedImaged;
        public GameObject icon;
        public TMP_Text nameLabel;
        public TMP_Text valueLabel;

        public AdvancedEvent onChecked;

        private bool _checked = false;
        private Animator animator;
        private string rewardCoroutineID;

        public RewardsManager.Reward reward { get; private set; }
        public ClientPuzzleData puzzleData { get; private set; }

        public override void _Update()
        {
            base._Update();

            if (puzzleData != null && puzzleData.pack != null)
                SetChecked(puzzleData.pack.puzzlesComplete.Contains(puzzleData));
        }

        public virtual RewardsScreenWidget SetData(ClientPuzzleData puzzleData, RewardsManager.Reward data)
        {
            this.puzzleData = puzzleData;

            return SetData(data);
        }

        public virtual RewardsScreenWidget SetData(RewardsManager.Reward data)
        {
            reward = data;

            //assign icon
            switch (data.rewardType)
            {
                case RewardType.GAME_PIECE:
                    //spawn gamepiece
                    GamePieceWidgetSmall widgetSmall = GameContentManager.InstantiatePrefab<GamePieceWidgetSmall>(GameContentManager.PrefabType.GAME_PIECE_SMALL, content);
                    widgetSmall.SetData(GameContentManager.Instance.piecesDataHolder.GetGamePieceData(reward.asGamePieceReward.gamePieceID));

                    widgetSmall.ResetAnchors();
                    widgetSmall.transform.localScale = Vector3.one * .55f;

                    break;
            }

            return SetData(data.rewardType, data.name, data.ToString());
        }

        public virtual RewardsScreenWidget SetData(RewardType type, string name, string value)
        {
            string nameText = swapLabels ? string.Format(format, value) : name;
            string valueText = swapLabels ? name : string.Format(format, value);

            if (!string.IsNullOrEmpty(nameText) && nameLabel) nameLabel.text = nameText;
            if (!string.IsNullOrEmpty(valueText) && valueLabel) valueLabel.text = valueText;

            return this;
        }

        public RewardsScreenWidget SetBGState(bool state)
        {
            bgImage.SetActive(state);

            return this;
        }

        public RewardsScreenWidget SetChecked(bool state)
        {
            switch (reward.rewardType)
            {
                case RewardType.OPEN_PORTAL:
                case RewardType.OPEN_RARE_PORTAL:
                    if (!state || (state && PlayerPrefsWrapper.GetRewardRewarded(puzzleData.GetRewardID(reward))))
                        _Checked(state);

                    break;

                default:
                    _Checked(state);

                    break;
            }

            return this;
        }

        public void AnimateReward()
        {
            if (reward == null) return;

            switch (reward.rewardType)
            {
                case RewardType.HINTS:
                    animator.SetTrigger("bounce");
                    rewardCoroutineID = PersistantOverlayScreen.instance.AnimateReward(false, reward.rewardType, reward.quantity, Camera.main.WorldToViewportPoint(transform.position));

                    break;
            }
        }

        public void CanceRewardlAnimation()
        {
            if (string.IsNullOrEmpty(rewardCoroutineID)) return;

            PersistantOverlayScreen.instance.CancelAnimationReward(rewardCoroutineID);
            rewardCoroutineID = "";
        }

        public RewardsScreenWidget PlayAudio(AudioTypes audio)
        {
            AudioHolder.instance.PlaySelfSfxOneShotTracked(audio, volume);

            return this;
        }

        public RewardsScreenWidget PlayAudio()
        {
            AudioHolder.instance.PlaySelfSfxOneShotTracked(onShow, volume);

            return this;
        }

        public RewardsScreenWidget SetNameFontSize(float value)
        {
            nameLabel.fontSize = value;

            return this;
        }

        public RewardsScreenWidget SetValueFontSize(float value)
        {
            valueLabel.fontSize = value;

            return this;
        }

        public void OnTap()
        {
            switch (reward.rewardType)
            {
                case RewardType.OPEN_PORTAL:
                    if (puzzleData == null || puzzleData.pack == null)
                        PersistantMenuController.Instance.GetOrAddScreen<PortalScreen>().SetData(RewardsManager.PortalType.SIMPLE);
                    else
                    {
                        //check if puzzle was complete and reward wasnt assigned yet
                        if (puzzleData.pack.puzzlesComplete.Contains(puzzleData) && !PlayerPrefsWrapper.GetRewardRewarded(puzzleData.GetRewardID(reward)))
                        {
                            PersistantMenuController.Instance.GetOrAddScreen<PortalScreen>().SetData(RewardsManager.PortalType.SIMPLE);
                            PlayerPrefsWrapper.SetRewardRewarded(puzzleData.GetRewardID(reward), true);

                            SetChecked(true);
                        }
                    }

                    break;

                case RewardType.OPEN_RARE_PORTAL:
                    if (puzzleData == null || puzzleData.pack == null)
                        PersistantMenuController.Instance.GetOrAddScreen<PortalScreen>().SetData(RewardsManager.PortalType.RARE);
                    else
                    {
                        //check if puzzle was complete and reward wasnt assigned yet
                        if (puzzleData.pack.puzzlesComplete.Contains(puzzleData) && !PlayerPrefsWrapper.GetRewardRewarded(puzzleData.GetRewardID(reward)))
                        {
                            PersistantMenuController.Instance.GetOrAddScreen<PortalScreen>().SetData(RewardsManager.PortalType.RARE);
                            PlayerPrefsWrapper.SetRewardRewarded(puzzleData.GetRewardID(reward), true);

                            SetChecked(true);
                        }
                    }

                    break;
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            animator = GetComponent<Animator>();
        }

        private void _Checked(bool state)
        {
            if (state != _checked)
            {
                checkedImaged.SetActive(state);

                if (state) onChecked.Invoke();
                _checked = state;
            }
        }
    }
}