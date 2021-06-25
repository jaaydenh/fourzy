//@vadym udod

using FourzyGameModel.Model;
using System.Collections;
using UnityEngine;

namespace Fourzy._Updates.Mechanics.Board
{
    public class SnowTokenView : TokenView
    {
        public override IEnumerator OnActivated()
        {
            base.OnActivate();

            active = false;
            Hide(.3f);

            yield break;
        }

        public override float OnGameAction(GameAction action)
        {
            GameActionTokenTransition _transition = action as GameActionTokenTransition;

            if (_transition == null || _transition.Reason != TransitionType.SNOW_ICE)
            {
                return 0f;
            }

            StartCoroutine(Transition(_transition));

            return 0f;
        }

        private IEnumerator Transition(GameActionTokenTransition _transition)
        {
            gameboard
                .SpawnToken(_transition.Location, _transition.After)
                .Show(.3f);

            yield return StartCoroutine(OnActivated());
            yield return new WaitForSeconds(.3f);

            _Destroy();
        }
    }
}
