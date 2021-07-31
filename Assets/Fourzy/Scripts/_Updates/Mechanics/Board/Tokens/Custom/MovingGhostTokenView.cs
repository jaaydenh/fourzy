//@vadym udod

using FourzyGameModel.Model;
using UnityEngine;

namespace Fourzy._Updates.Mechanics.Board
{
    public class MovingGhostTokenView : TokenView
    {
        public MovingGhostToken token => Token as MovingGhostToken;
        public override TokenView SetData(IToken tokenData = null)
        {
            base.SetData(tokenData);

            if (tokenData.Type != TokenType.MOVING_GHOST)
            {
                return this;
            }

            frequency = token.Frequency;

            SetValue(currentCountdownValue = token.Countdown);

            return this;
        }

        public override void OnAfterTurn(bool startTurn)
        {
            base.OnAfterTurn(startTurn);

            if (startTurn)
            {
                return;
            }

            if (currentCountdownValue + 1 >= frequency)
            {
                currentCountdownValue = 0;
            }
            else
            {
                currentCountdownValue++;
            }

            SetValue(currentCountdownValue);
        }

        private void SetValue(int value)
        {
            if (frequency != 1)
            {
                countdown.SetValue(Mathf.Clamp(frequency - value, 1, frequency));
            }
        }
    }
}