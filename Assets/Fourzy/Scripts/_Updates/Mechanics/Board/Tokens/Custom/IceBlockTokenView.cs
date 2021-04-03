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

        public IceBlockToken token { get; private set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            animator = GetComponentInChildren<Animator>();
        }

        public override TokenView SetData(IToken tokenData = null)
        {
            token = tokenData as IceBlockToken;

            if (token.Broken)
                _Destroy();

            return base.SetData(tokenData);
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

            float length = animator.GetCurrentAnimatorStateInfo(indexBaseLayer).length;
            StartRoutine("destroy", length, () => _Destroy());

            yield return new WaitForSeconds(length);
        }

        public override float OnGameAction(params GameAction[] actions)
        {
            GameActionTokenTransition _transition = actions[0] as GameActionTokenTransition;

            if (_transition == null || _transition.Reason != TransitionType.BLOCK_ICE) return 0f;

            StartCoroutine(Transition(_transition));

            return 0f;
        }

        private IEnumerator Transition(GameActionTokenTransition _transition)
        {
            TokenView newToken = gameboard.SpawnToken<TokenView>(_transition.Location.Row, _transition.Location.Column, _transition.After.Type, true);
            newToken.SetAlpha(0f);

            yield return StartCoroutine(OnActivated());

            newToken.Show(.3f);
        }
    }
}