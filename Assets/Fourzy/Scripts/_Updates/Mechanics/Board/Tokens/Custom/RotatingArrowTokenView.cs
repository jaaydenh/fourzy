//@vadym udod

using FourzyGameModel.Model;

namespace Fourzy._Updates.Mechanics.Board
{
    public class RotatingArrowTokenView : ArrowTokenView
    {
        public override TokenView SetData(IToken tokenData = null)
        {
            if (tokenData.Type != TokenType.ROTATING_ARROW) return base.SetData(tokenData);

            RotatingArrowToken rotatingArrow = tokenData as RotatingArrowToken;
            frequency = rotatingArrow.Frequency;
            currentCountdownValue = rotatingArrow.CountDown;

            countdown.hideOnEmpty = frequency == 1;
            countdown.SetValue(currentCountdownValue);

            return base.SetData(tokenData);
        }

        public override float RotateTo(Direction from, Direction to, Rotation rotation, float time = 0, bool startTurn = false)
        {
            if (!startTurn) currentCountdownValue = 0;

            return base.RotateTo(from, to, rotation, time, startTurn);
        }

        public override void OnAfterTurn(bool startTurn)
        {
            base.OnAfterTurn(startTurn);

            if (!startTurn) countdown.SetValue(frequency - currentCountdownValue++ - 1);
        }
    }
}