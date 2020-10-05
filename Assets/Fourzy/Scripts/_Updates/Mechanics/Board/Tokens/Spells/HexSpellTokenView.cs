//@vadym udod

using FourzyGameModel.Model;
using UnityEngine;

namespace Fourzy._Updates.Mechanics.Board
{
    public class HexSpellTokenView : TokenSpell
    {
        public override Color outlineColor => Color.red;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            ShowOutline(true);
        }

        public override void SetData(ISpell spell)
        {
            if (spell.SpellId != SpellId.HEX) return;

            base.SetData(spell);

            currentCountdownValue = (spell as HexSpell).Duration - 1;

            countdown.SetValue(currentCountdownValue);
        }

        public override TokenView SetData(IToken tokenData = null)
        {
            if (tokenData == null || tokenData.Type != TokenType.HEX) return base.SetData(tokenData);

            currentCountdownValue = (tokenData as HexSpellToken).Countdown - 1;
            countdown.SetValue(currentCountdownValue);

            return base.SetData(tokenData);
        }

        public override void OnAfterTurn(bool startTurn)
        {
            base.OnAfterTurn(startTurn);

            /*if (!startTurn) */
            countdown.SetValue(currentCountdownValue--);
        }
    }
}
