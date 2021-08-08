//@vadym udod

using FourzyGameModel.Model;
using UnityEngine;

namespace Fourzy._Updates.Mechanics.Board
{
    public class HexSpellTokenView : TokenSpell
    {
        public HexSpellToken token => Token as HexSpellToken;

        public override Color outlineColor => Color.red;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            AnimateOutline(0f, 1f, 1f, .0015f);
        }

        public override void SetData(ISpell spell, IMove spellMove)
        {
            base.SetData(spell, spellMove);

            countdown.SetValue(currentCountdownValue = (spell as HexSpell).Duration - 1);
        }

        public override TokenView SetData(IToken tokenData = null)
        {
            base.SetData(tokenData);

            countdown.SetValue(currentCountdownValue = token.Countdown - 1);

            return this;
        }

        public override void OnAfterTurn(bool startTurn)
        {
            base.OnAfterTurn(startTurn);

            /*if (!startTurn) */
            countdown.SetValue(currentCountdownValue--);
        }
    }
}
