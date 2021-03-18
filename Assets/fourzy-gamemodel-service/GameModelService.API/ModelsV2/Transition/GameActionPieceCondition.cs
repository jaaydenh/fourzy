using System;

namespace FourzyGameModel.Model
{
    public class GameActionPieceCondition : GameAction
    {
        public GameActionType Type { get { return GameActionType.TRANSITION; } }

        public GameActionTiming Timing { get { return GameActionTiming.MOVE; } }

        public PieceConditionType Condition { get; set; }

        BoardLocation Location { get; set; }
        Piece Piece { get; set; }

        public GameActionPieceCondition(Piece Piece, BoardLocation Location, PieceConditionType Condition)
        {
            this.Location = Location;
            this.Piece = Piece;
            this.Condition = Condition;
        }

        public string Print()
        {
            throw new NotImplementedException();
        }
    }
}
