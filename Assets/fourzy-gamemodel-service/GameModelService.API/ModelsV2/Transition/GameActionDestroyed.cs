using System;

namespace FourzyGameModel.Model
{
    public class GameActionDestroyed : GameAction
    {
        public GameActionType Type { get { return GameActionType.DESTROY; } }

        public GameActionTiming Timing { get { return GameActionTiming.AFTER_MOVE; } }

        Piece Piece { get; set; }
        public DestroyType Reason { get; set; }
        public BoardLocation End { get; set; }

        public GameActionDestroyed(Piece Piece, BoardLocation End, DestroyType Reason)
        {
            this.Piece = Piece;
            this.End = End;
            this.Reason = Reason;
        }

        public string Print()
        {
            throw new NotImplementedException();
        }
    }
}
