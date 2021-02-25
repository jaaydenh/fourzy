//@vadym udod

using Fourzy._Updates.Mechanics.Rewards;
using Fourzy._Updates.Tools;
using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Widgets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class PortalScreen : MenuScreen
    {
        public GameObject rewardHolderPrefab;
        public RectTransform portalTransform;
        public AlphaTween tapToContinue;

        private List<RewardsManager.Reward> rewards;
        private RewardsManager.PortalType portalType;

        private int lastRewardIndex;
        private RewardsScreenWidget currentWidget;
        private List<RewardsScreenWidget> rewardWidgets = new List<RewardsScreenWidget>();

        private bool wasStopped;

        public void SetData(RewardsManager.PortalType portalType)
        {
            this.portalType = portalType;
            lastRewardIndex = 0;
            wasStopped = false;

            tapToContinue.StopTween(true);
            //remove old ones
            foreach (RewardsScreenWidget widget in rewardWidgets) Destroy(widget.transform.parent.gameObject);
            rewardWidgets = new List<RewardsScreenWidget>();

            //get rewards
            //rewards = RewardsManager.FakePortalRewards();
            rewards = RewardsManager.GetPortalRewards();

            //assign rewards
            rewards.AssignRewards();

            DisplayRewards();

            //open screen
            menuController.OpenScreen(this);
        }

        public override void OnBack()
        {
            //if last one was canceled too, ignore inputs for a bit
            IgnoreInputs();

            CancelRoutine("rewardRoutine");

            if (IsRoutineActive("ignoreInputs")) return;

            if (lastRewardIndex == rewards.Count)
            {
                base.OnBack();
                menuController.CloseCurrentScreen();
            }
        }

        private void DisplayRewards()
        {
            //cancel routine if needed
            StartRoutine("displayRewards", DisplayRewardsRoutine(), null, () =>
            {
                CancelRoutine("rewardRoutine");

                //spawn the rest
                for (int index = lastRewardIndex + 1; index < rewards.Count + 1; index++) rewardWidgets.Add(AddWidget(rewards[index - 1]));

                lastRewardIndex = rewards.Count;
            });
        }

        private RewardsScreenWidget AddWidget(RewardsManager.Reward reward)
        {
            //get container for reward
            GameObject rewardholder = Instantiate(rewardHolderPrefab, rewardHolderPrefab.transform.parent);
            rewardholder.SetActive(true);

            RewardsScreenWidget rewardWidget = 
                Instantiate(GameContentManager.GetPrefab<RewardsScreenWidget>(reward.rewardType.AsPrefabType()), rewardholder.transform);
            rewardWidget.SetData(reward);
            rewardWidget.ResetAnchors();

            return rewardWidget;
        }

        private void IgnoreInputs()
        {
            //if last one was canceled too, ignore inputs for a bit
            if (lastRewardIndex == rewards.Count && !wasStopped)
            {
                wasStopped = true;
                StartRoutine("ignoreInputs", .6f, () => tapToContinue.PlayForward(true));
            }
        }

        private void CancelRewardWidgetAnimation(RewardsScreenWidget widget)
        {
            widget.ScaleTo(Vector3.one, 0f);
            widget.Show(0f);

            BezierPositionTween bezier = widget.GetComponent<BezierPositionTween>();

            if (bezier)
            {
                bezier.StopTween(true);
            }
        }

        private IEnumerator DisplayRewardsRoutine()
        {
            foreach (RewardsManager.Reward reward in rewards)
            {
                lastRewardIndex++;

                RewardsScreenWidget widget = AddWidget(reward);
                rewardWidgets.Add(widget);

                yield return StartRoutine("rewardRoutine", RewardRoutine(widget), null, () => CancelRewardWidgetAnimation(widget));
            }

            IgnoreInputs();
        }

        private IEnumerator RewardRoutine(RewardsScreenWidget widget)
        {
            widget.SetAlpha(0f);

            //force update layout
            LayoutRebuilder.ForceRebuildLayoutImmediate(rewardHolderPrefab.transform.parent.GetComponent<RectTransform>());

            yield return null;

            BezierPositionTween bezier = widget.gameObject.AddComponent<BezierPositionTween>();
            bezier.curve = Utils.CreateEaseInEaseOutCurve();
            bezier.from = (portalTransform.position - widget.transform.parent.position) / menuController.transform.lossyScale.x;
            bezier.to = Vector2.zero;
            bezier.control = bezier.from + Random.Range(45f, 135f).VectorFromAngle() * 350f;
            bezier.playbackTime = 1f;
            bezier.PlayForward(true);

            widget.Show(.5f);

            yield return new WaitForSeconds(1.3f);
        }
    }
}