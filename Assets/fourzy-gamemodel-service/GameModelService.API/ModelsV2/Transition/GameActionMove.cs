using System;

namespace FourzyGameModel.Model
{
    public class GameActionMove : GameAction
    {
        public GameActionType Type { get { return GameActionType.MOVE_PIECE; } }
        public GameActionTiming Timing { get { return GameActionTiming.MOVE; } }

        public Direction Direction
        {
            get
            {
                if (End.Row - Start.Row < 0)
                    return Direction.UP;
                else if (End.Row - Start.Row > 0)
                    return Direction.DOWN;
                else if (End.Column - Start.Column > 0)
                    return Direction.RIGHT;
                else if (End.Column - Start.Column < 0)
                    return Direction.LEFT;

                return Direction.NONE;
            }
        }

        public MovingPiece Piece { get; set; }
        public BoardLocation Start { get; set; }
        public BoardLocation End { get; set; }

        public float Distance
        {
            get
            {
                return Math.Abs((End - Start).Row) + Math.Abs((End - Start).Column);
            }
        }
        public BoardLocation Next
        {
            get
            {
                return End.Neighbor(Direction);
            }
        }

        public GameActionMove(MovingPiece Piece, BoardLocation Start, BoardLocation End)
        {
            this.Piece = new MovingPiece(Piece);
            this.Start = Start;
            this.End = End;
        }

        public string Print()
        {
            return "";
        }
    }
}
