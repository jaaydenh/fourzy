using System;

namespace FourzyGameModel.Model
{
    public class GameActionPush : GameAction
    {
        public GameActionType Type { get { return GameActionType.PUSH; } }

        public GameActionTiming Timing { get { return GameActionTiming.MOVE; } }

        public MovingPiece InitiatingPiece { get; set; }
        public MovingPiece PushedPiece { get; set; }
        BoardLocation Location { get; set; }

        public GameActionPush(MovingPiece Initiator, MovingPiece PushedPiece, BoardLocation Location)
        {
            this.InitiatingPiece = Initiator;
            this.PushedPiece = PushedPiece;
            this.Location = Location;
        }

        public string Print()
        {
            throw new NotImplementedException();
        }
    }
}
