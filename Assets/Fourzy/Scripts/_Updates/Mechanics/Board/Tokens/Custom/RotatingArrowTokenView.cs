//@vadym udod

using Fourzy._Updates.Audio;
using FourzyGameModel.Model;

namespace Fourzy._Updates.Mechanics.Board
{
    public class RotatingArrowTokenView : ArrowTokenView
    {
        public RotatingArrowToken token => Token as RotatingArrowToken;

        public override TokenView SetData(IToken tokenData = null)
        {
            base.SetData(tokenData);

            if (tokenData.Type != TokenType.ROTATING_ARROW)
            {
                return this;
            }

            frequency = token.Frequency;

            SetValue(currentCountdownValue = token.CountDown);

            return this;
        }

        public override void OnAfterTurn(bool startTurn)
        {
            base.OnAfterTurn(startTurn);

            if (currentCountdownValue - 1 <= 0)
            {
                currentCountdownValue = frequency;
            }
            else
            {
                currentCountdownValue--;
            }

            SetValue(currentCountdownValue);
        }

        private void SetValue(int value)
        {
            if (frequency != 1)
            {
                countdown.SetValue(value);
            }
        }
    }
}