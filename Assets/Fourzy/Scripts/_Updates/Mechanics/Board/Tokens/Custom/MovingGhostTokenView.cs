//@vadym udod

using FourzyGameModel.Model;

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

            countdown.hideOnEmpty = frequency == 1;
            countdown.SetValue(currentCountdownValue);

            return base.SetData(tokenData);
        }

        public override void OnBeforeMoveAction(bool startTurn, params BoardLocation[] actionsMoves)
        {
            if (!startTurn) currentCountdownValue = 0;

            base.OnBeforeMoveAction(startTurn, actionsMoves);
        }

        public override void OnAfterTurn(bool startTurn)
        {
            base.OnAfterTurn(startTurn);

            if (!startTurn) countdown.SetValue(frequency - currentCountdownValue++ - 1);
        }
    }
}