//@vadym udod

using FourzyGameModel.Model;

namespace Fourzy._Updates.Mechanics.Board
{
    public class DarknessTokenView : TokenSpell
    {
        public DarknessToken token => Token as DarknessToken;

        public override void SetData(ISpell spell, IMove spellMove)
        {
            base.SetData(spell, spellMove);

            countdown.SetValue(currentCountdownValue = (spell as DarknessSpell).Duration - 1);
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