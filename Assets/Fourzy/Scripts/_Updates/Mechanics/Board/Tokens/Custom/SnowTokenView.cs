//@vadym udod

using FourzyGameModel.Model;
using System.Collections;

namespace Fourzy._Updates.Mechanics.Board
{
    public class SnowTokenView : TokenView
    {
        public override IEnumerator OnActivated()
        {
            base.OnActivate();

            //despawn this token
            Hide(.3f);
            _Destroy(.4f);

            BoardLocation _location = location;
            //spawn ice token
            gameboard.SpawnToken<TokenView>(_location.Row, _location.Column, TokenType.ICE, false).Show(.3f);

            yield break;
        }

        public override float OnGameAction(params GameAction[] actions)
        {
            GameActionTokenTransition _transition = actions[0] as GameActionTokenTransition;

            if (_transition == null || _transition.Reason != TransitionType.SNOW_ICE) return 0f;

            StartCoroutine(OnActivated());

            return 0f;
        }
    }
}
