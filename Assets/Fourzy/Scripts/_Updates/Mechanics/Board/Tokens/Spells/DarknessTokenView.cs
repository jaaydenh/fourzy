//@vadym udod

using FourzyGameModel.Model;

namespace Fourzy._Updates.Mechanics.Board
{
    public class DarknessTokenView : TokenSpell
    {
        public override void SetData(ISpell spell, IMove spellMove)
        {
            if (spell.SpellId != SpellId.DARKNESS) return;

            base.SetData(spell, spellMove);

            currentCountdownValue = (spell as DarknessSpell).Duration - 1;

            countdown.SetValue(currentCountdownValue);
        }

        public override TokenView SetData(IToken tokenData = null)
        {
            if (tokenData == null || tokenData.Type != TokenType.DARKNESS) return base.SetData(tokenData);

            currentCountdownValue = (tokenData as DarknessToken).Countdown - 1;
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