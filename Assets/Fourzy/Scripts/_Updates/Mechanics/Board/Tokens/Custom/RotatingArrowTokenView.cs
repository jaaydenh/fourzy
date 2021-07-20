﻿//@vadym udod

using Fourzy._Updates.Audio;
using FourzyGameModel.Model;

namespace Fourzy._Updates.Mechanics.Board
{
    public class RotatingArrowTokenView : ArrowTokenView
    {
        public override TokenView SetData(IToken tokenData = null)
        {
            if (tokenData.Type != TokenType.ROTATING_ARROW)
            {
                return base.SetData(tokenData);
            }

            RotatingArrowToken rotatingArrow = tokenData as RotatingArrowToken;
            frequency = rotatingArrow.Frequency;
            currentCountdownValue = rotatingArrow.CountDown;

            SetValue(currentCountdownValue);

            return base.SetData(tokenData);
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

        public override void OnBitEnter(BoardBit other)
        {
            base.OnBitEnter(other);

            AudioHolder.instance.PlaySelfSfxOneShot(onGamePieceEnter, volume, other.speedMltp);
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