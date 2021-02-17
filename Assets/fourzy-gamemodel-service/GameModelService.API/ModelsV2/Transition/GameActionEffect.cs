using System;

namespace FourzyGameModel.Model
{
    public class GameActionEffect : GameAction
    {
        public GameActionType Type { get { return GameActionType.EFFECT; } }

        public GameActionTiming Timing { get { return GameActionTiming.MOVE; } }

        BoardLocation Location { get; set; }
        TransitionType Effect { get; set; }

        public GameActionEffect(BoardLocation Location, TransitionType Effect )
        {
            this.Location = Location;
            this.Effect = Effect;
        }

        public string Print()
        {
            throw new NotImplementedException();
        }
    }
}
