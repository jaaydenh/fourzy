//@vadym udod

using Fourzy._Updates.Prefabs;
using Fourzy._Updates.UI.Menu.Widgets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class RewardScreen : MenuScreen
    {
        public RewardScreenPortalWidget portal;
        public RectTransform rewardsParent;

        public Vector3 moveRewardsTo = new Vector3(0f, -400f, 0f);

        private List<Reward> rewardsToDisplay;

        private WidgetBase currentReward;
        private int currentRewardIndex;

        protected override void Awake()
        {
            base.Awake(); rewardsToDisplay = new List<Reward>();
        }

        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
                OpenScreen(new Reward[] {
                    new Reward()
                    {
                        Type = Reward.RewardType.Coins,
                        NumberOfCoins = 10
                    },
                    new Reward()
                    {
                        Type = Reward.RewardType.CollectedGamePiece,
                        CollectedGamePiece = new GamePieceData()
                    }
                });
        }

        public void OpenScreen(ICollection<Reward> rewards)
        {
            rewardsToDisplay.Clear();
            rewardsToDisplay.AddRange(rewards);
            currentRewardIndex = 0;

            //open this screen using menu controller
            menuController.OpenScreen(this);
        }

        public override void Open()
        {
            base.Open();

            StopRoutine("screenOpen", false);
            StartRoutine("screenOpen", ScreenOpenRoutine(), () =>
            {
                ShowNextReward();
            }, () =>
            {
                portal.StopOpenAnimation();

                StartRoutine("startRewardsListing", StartRewardsListing(), null, () =>
                {
                    ShowNextReward();
                });
            });
        }

        public void SkipCurrent()
        {
            //cancel routine if active
            if (CancelRoutine("screenOpen"))
                return;

            if (CancelRoutine("startRewardsListing"))
                return;

            if (CancelRoutine("closeRoutine"))
                return;

            ShowNextReward();
        }

        private void ShowNextReward()
        {
            //skip current one
            CancelRoutine("rewardDisplay");

            if (rewardsToDisplay.Count > currentRewardIndex)
            {
                StartRoutine("rewardDisplay", ShowRewardRoutine(rewardsToDisplay[currentRewardIndex]), () =>
                {
                    ShowNextReward();
                }, () =>
                {
                    HideCurrentReward();
                });

                currentRewardIndex++;
            }
            else
                StartRoutine("closeRoutine", CloseRoutine(), null, () =>
                {
                    menuController.CloseCurrentScreen(false);
                });
        }

        private void HideCurrentReward()
        {
            if (currentReward)
            {
                currentReward.ScaleTo(Vector3.zero, .4f);
                currentReward.Hide(.3f);
            }
        }

        private IEnumerator ScreenOpenRoutine()
        {
            portal.ResetAnimation();

            yield return new WaitForSeconds(1.2f);

            //move portal up
            portal.OpenAnimation();

            yield return new WaitForSeconds(portal.positionTween.playbackTime + 1f);
        }

        private IEnumerator StartRewardsListing()
        {
            yield return new WaitForSeconds(.5f);

            ShowNextReward();
        }

        private IEnumerator ShowRewardRoutine(Reward reward)
        {
            //show&animate reward widget
            switch (reward.Type)
            {
                case Reward.RewardType.Coins:
                    currentReward = PrefabsManager.GetPrefab<WidgetBase>(PrefabType.COINS_WIDGET_SMALL, rewardsParent);
                    ((CoinsWidgetSmall)currentReward).SetValue(reward.NumberOfCoins);
                    break;
                case Reward.RewardType.CollectedGamePiece:
                    currentReward = PrefabsManager.GetPrefab<WidgetBase>(PrefabType.GAME_PIECE_SMALL, rewardsParent);
                    ((GamePieceWidgetSmall)currentReward).SetData(reward.CollectedGamePiece);
                    break;
            }

            if (currentReward)
            {
                currentReward.transform.localPosition = Vector3.zero;
                currentReward.ScaleTo(Vector3.zero, Vector3.one, .4f);

                yield return new WaitForSeconds(.2f);

                currentReward.MoveTo(moveRewardsTo, 1f);
            }

            yield return new WaitForSeconds(1.5f);

            HideCurrentReward();

            yield return new WaitForSeconds(.5f);
        }

        private IEnumerator CloseRoutine()
        {
            yield return new WaitForSeconds(3f);

            //close screen
            menuController.CloseCurrentScreen(false);
        }
    }
}