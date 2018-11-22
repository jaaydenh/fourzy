//@vadym udod

using Fourzy._Updates.Audio;
using Fourzy._Updates.Mechanics.Vfx;
using Fourzy._Updates.Tools;
using Fourzy._Updates.UI.Widgets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class RewardScreen : MenuScreen
    {
        public RewardScreenPortalWidget portal;
        public Vector3 moveRewardsTo = new Vector3(0f, -400f, 0f);
        public RewardHolderWidget rewardHolderPrefab;
        public RectTransform rewardsParent;

        private List<Reward> rewardsToDisplay;

        private RewardHolderWidget currentRewardHolder;
        private Vfx starsVfx;
        private int currentRewardIndex;

        protected override void Awake()
        {
            base.Awake();

            rewardsToDisplay = new List<Reward>();
        }

        //for testing
        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
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
            StartRoutine("screenOpen", 1.5f, () =>
            {
                ShowNextReward();
            }, () =>
            {
                SkipToNext();
            });
        }

        public override void Close()
        {
            base.Close();

            StopAllRoutines(false);
        }

        public void SkipCurrent()
        {
            //cancel routine if active
            if (CancelRoutine("screenOpen"))
                return;

            if (CancelRoutine("closeRoutine"))
                return;

            if (CancelRoutine("showReward"))
                return;

            if (CancelRoutine("hideReward"))
            {
                SkipToNext();
                return;
            }

            ShowNextReward();
        }

        private void ShowNextReward()
        {
            if (rewardsToDisplay.Count > currentRewardIndex)
            {
                StartRoutine("showReward", ShowRewardRoutine(rewardsToDisplay[currentRewardIndex]), () =>
                {
                    StartRoutine("hideReward", 1.5f, () =>
                    {
                        HideCurrentReward();
                        ShowNextReward();
                    });

                    //animate reward
                    //currentRewardHolder.squishTween.PlayForward(true);
                }, () =>
                {
                    //if "show reward" was cacelled, just place it at the end of path with no animation
                    currentRewardHolder.CancelAnimation();

                    StartRoutine("hideReward", 1.5f, () =>
                    {
                        HideCurrentReward();
                        ShowNextReward();
                    });
                });

                currentRewardIndex++;

                //play spawn sound
                AudioHolder.instance.PlaySelfSfxOneShotTracked(Serialized.AudioTypes.REWARD_SPAWN, .9f);
            }
            else
                StartRoutine("closeRoutine", 2f, () =>
                {
                    menuController.CloseCurrentScreen(false);
                }, () =>
                {
                    HideCurrentReward();
                    menuController.CloseCurrentScreen(false);
                });
        }

        private void SkipToNext()
        {
            ShowNextReward();
            CancelRoutine("showReward");
        }

        private void HideCurrentReward()
        {
            if (currentRewardHolder)
            {
                currentRewardHolder.ScaleTo(Vector3.zero, .4f);
                currentRewardHolder.Hide(.3f);
            }
        }

        private IEnumerator ShowRewardRoutine(Reward reward)
        {
            currentRewardHolder = Instantiate(rewardHolderPrefab, portal.transform);
            currentRewardHolder.transform.localPosition = Vector2.zero;
            currentRewardHolder.transform.localScale = Vector2.one;

            currentRewardHolder.SetRewardWidget(reward);
            currentRewardHolder.BezierMoveTo(moveRewardsTo, Random.Range(45f, 135f).VectorFromAngle() * 600f, .9f);

            yield return new WaitForSeconds(.9f);
        }
    }
}