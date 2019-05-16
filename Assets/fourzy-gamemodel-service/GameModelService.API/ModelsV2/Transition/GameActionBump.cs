using System;

namespace FourzyGameModel.Model
{
    public class GameActionBump : GameAction
    {
        public GameActionType Type {get { return GameActionType.BUMP;  } }

        public GameActionTiming Timing { get { return GameActionTiming.MOVE; } }

        MovingPiece Piece { get; set; }
        BoardLocation Start { get; set; }
        BoardLocation End { get; set; }

        public string Print()
        {
            throw new NotImplementedException();
        }
    }
}
