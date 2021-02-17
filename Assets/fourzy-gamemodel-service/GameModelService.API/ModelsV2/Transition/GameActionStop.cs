using System;

namespace FourzyGameModel.Model
{
    public class GameActionStop : GameAction
    {
        public GameActionType Type { get { return GameActionType.STOP; } }

        public GameActionTiming Timing { get { return GameActionTiming.MOVE; } }

        MovingPiece Piece { get; set; }
        public StopType Reason { get; set; }
        public BoardLocation End { get; set; }

        public GameActionStop(MovingPiece Piece, BoardLocation End, StopType Reason)
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
