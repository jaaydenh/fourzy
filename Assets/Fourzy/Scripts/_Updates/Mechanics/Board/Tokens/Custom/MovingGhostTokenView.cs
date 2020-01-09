﻿//@vadym udod

using Fourzy._Updates.Tools;
using FourzyGameModel.Model;
using UnityEngine;

namespace Fourzy._Updates.Mechanics.Board
{
    public class MovingGhostTokenView : TokenView
    {
        public override TokenView SetData(IToken tokenData = null)
        {
            if (tokenData.Type != TokenType.MOVING_GHOST) return base.SetData(tokenData);

            MovingGhostToken movingGhost = tokenData as MovingGhostToken;
            frequency = movingGhost.Frequency;
            currentCountdownValue = movingGhost.Countdown;

            SetValue(currentCountdownValue);

            return base.SetData(tokenData);
        }

        public override void OnAfterTurn(bool startTurn)
        {
            base.OnAfterTurn(startTurn);

            if (startTurn) return;

            if (currentCountdownValue + 1 >= frequency) currentCountdownValue = 0;
            else currentCountdownValue++;
            SetValue(currentCountdownValue);
        }

        public override TokenView UpdateGraphics()
        {
            countdown.targetText.UpdateTMP_Text();

            return base.UpdateGraphics();
        }

        private void SetValue(int value)
        {
            if (frequency != 1) countdown.SetValue(Mathf.Clamp(frequency - value, 1, frequency));
        }
    }
}