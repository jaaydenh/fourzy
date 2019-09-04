using System;

namespace FourzyGameModel.Model
{
    public class GameActionTokenMovement : GameAction
    {
        public GameActionType Type { get { return GameActionType.TRANSITION; } }

        public GameActionTiming Timing { get { return GameActionTiming.BEFORE_MOVE; } }

        public BoardLocation Start { get; set; }
        public BoardLocation End { get; set; }

        public TransitionType Reason { get; set; }
        public IToken Token { get; set; }

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
