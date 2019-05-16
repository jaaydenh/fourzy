using System;

namespace FourzyGameModel.Model
{
    public class GameActionTokenMovement : GameAction
    {
        public GameActionType Type { get { return GameActionType.TRANSITION; } }

        public GameActionTiming Timing { get { return GameActionTiming.BEFORE_MOVE; } }

        BoardLocation Start { get; set; }
        BoardLocation End { get; set; }

        TransitionType Reason { get; set; }
        IToken Token { get; set; }

        public GameActionTokenMovement(IToken Token, TransitionType Reason, BoardLocation Start, BoardLocation End)
        {
            this.Token= Token;
            this.Reason = Reason;
            this.Start= Start;
            this.End = End;
        }

        public string Print()
        {
            throw new NotImplementedException();
        }
    }
}
