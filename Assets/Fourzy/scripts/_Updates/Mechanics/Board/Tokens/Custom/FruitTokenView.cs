//@vadym udod

using System.Collections;
using UnityEngine;

namespace Fourzy._Updates.Mechanics.Board
{
    public class FruitTokenView : TokenView
    {
        private const int indexBaseLayer = 0;

        private Animator animator;
        private int h_FruitIntoSticky = Animator.StringToHash("FruitIntoSticky");

        protected override void OnInitialized()
        {
            base.OnInitialized();

            animator = GetComponentInChildren<Animator>();
        }

        public override void OnActivate()
        {
            StartCoroutine(OnActivated());
        }

        public override IEnumerator OnActivated()
        {
            base.OnActivate();
            active = false;

            animator.Play(h_FruitIntoSticky);

            yield return new WaitForEndOfFrame();

            float length = animator.GetCurrentAnimatorStateInfo(indexBaseLayer).length;

            StartRoutine("fade", length, () => Hide(.4f));
            StartRoutine("destroy", length + .4f, () => _Destroy());

            yield return new WaitForSeconds(length);
        }
    }
}
