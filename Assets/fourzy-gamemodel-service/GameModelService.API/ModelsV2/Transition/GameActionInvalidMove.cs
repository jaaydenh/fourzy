using System;

namespace FourzyGameModel.Model
{
    public class GameActionInvalidMove : GameAction
    {
        public GameActionType Type { get { return GameActionType.INVALID; } }

        public GameActionTiming Timing { get { return GameActionTiming.AFTER_MOVE; } }

        SimpleMove Move { get; set; }
        string Message { get; set; }
        InvalidTurnType Reason {get; set; }

        public GameActionInvalidMove(SimpleMove Move, string Message, InvalidTurnType Reason)
        {
            this.Move = Move;
            this.Message= Message;
            this.Reason = Reason;
        }

        public string Print()
        {
            throw new NotImplementedException();
        }
    }
}
