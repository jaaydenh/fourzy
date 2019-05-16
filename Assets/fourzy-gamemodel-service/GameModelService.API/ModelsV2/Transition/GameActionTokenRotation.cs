using System;

namespace FourzyGameModel.Model
{
    public class GameActionTokenRotation : GameAction
    {
        public GameActionType Type { get { return GameActionType.TRANSITION; } }

        public GameActionTiming Timing { get { return GameActionTiming.BEFORE_MOVE; } }

        Direction StartOrientation { get; set; }
        Direction EndOrientation { get; set; }
        Rotation RotationDirection { get; set; }

        TransitionType Reason { get; set; }
        IToken Token { get; set; }

        public GameActionTokenRotation(IToken Token, TransitionType Reason, Rotation RotationDirection, Direction StartOrientation, Direction EndOrientation)
        {
            this.Token = Token;
            this.Reason = Reason;
            this.RotationDirection = RotationDirection;
            this.StartOrientation = StartOrientation;
            this.EndOrientation = EndOrientation;
        }

        public string Print()
        {
            throw new NotImplementedException();
        }
    }
}
