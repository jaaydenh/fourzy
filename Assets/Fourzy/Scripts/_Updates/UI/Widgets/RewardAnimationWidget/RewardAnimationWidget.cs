//@vadym udod

using Fourzy._Updates.Mechanics._Vfx;
using Fourzy._Updates.Tools;
using System.Collections;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class RewardAnimationWidget : WidgetBase
    {
        public RectTransform widgetParent;

        private Animator animator;

        public RewardAnimationWidget SetData(RewardType type, int amount)
        {
            Instantiate(GameContentManager.GetPrefab<RewardsScreenWidget>(type.AsPrefabType()), widgetParent).SetData(type, "", amount + "");
            alphaTween.DoParse();

            return this;
        }

        public RewardAnimationWidget PlayAnimationDelayed(float delay)
        {
            StartCoroutine(DelayedAnimationRoutine(delay));

            return this;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            animator = GetComponent<Animator>();
        }

        private IEnumerator DelayedAnimationRoutine(float delay)
        {
            yield return new WaitForSeconds(.8f);
            animator.SetTrigger("bounceHide");
        }
    }
}