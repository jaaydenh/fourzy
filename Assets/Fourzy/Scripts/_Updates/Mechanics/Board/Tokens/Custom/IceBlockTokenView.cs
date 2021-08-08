//@vadym udod

using FourzyGameModel.Model;
using System.Collections;
using UnityEngine;

namespace Fourzy._Updates.Mechanics.Board
{
    public class IceBlockTokenView : TokenView
    {
        private const int indexBaseLayer = 0;

        private Animator animator;
        private int h_IceBlockBreak = Animator.StringToHash("IceBlockBreak");

        public IceBlockToken token => Token as IceBlockToken;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            animator = GetComponentInChildren<Animator>();
        }

        public override TokenView SetData(IToken tokenData = null)
        {
            base.SetData(tokenData);

            if (token.Broken)
            {
                _Destroy();
            }

            return this;
        }

        public override void OnActivate()
        {
            StartCoroutine(OnActivated());
        }

        public override IEnumerator OnActivated()
        {
            base.OnActivate();
            active = false;

            animator.Play(h_IceBlockBreak);

            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(indexBaseLayer).length);
        }

        public override float OnGameAction(GameAction action)
        {
            GameActionTokenTransition _transition = action as GameActionTokenTransition;

            if (_transition == null || _transition.Reason != TransitionType.BLOCK_ICE) return 0f;

            StartCoroutine(Transition(_transition));

            return 0f;
        }

        private IEnumerator Transition(GameActionTokenTransition _transition)
        {
            TokenView newToken = gameboard.SpawnToken(_transition.Location, _transition.After);
            newToken.SetAlpha(0f);

            yield return StartCoroutine(OnActivated());

            newToken.Show(.3f);
            _Destroy();
        }
    }
}
