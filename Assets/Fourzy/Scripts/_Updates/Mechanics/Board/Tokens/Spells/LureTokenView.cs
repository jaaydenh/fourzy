//@vadym udod

using FourzyGameModel.Model;
using System.Collections;
using UnityEngine;

namespace Fourzy._Updates.Mechanics.Board
{
    public class LureTokenView : TokenSpell
    {
        public LureToken token { get; private set; }

        public override TokenView SetData(IToken tokenData = null)
        {
            token = tokenData as LureToken;

            if (token.Eaten) _Destroy();

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

            Hide(.4f);

            yield return new WaitForSeconds(.4f);

            _Destroy();
        }

        public override float OnGameAction(GameAction action)
        {
            GameActionTokenTransition _transition = action as GameActionTokenTransition;

            if (_transition == null || _transition.Reason != TransitionType.EAT_LURE) return 0f;

            StartCoroutine(OnActivated());

            return 0f;
        }
    }
}