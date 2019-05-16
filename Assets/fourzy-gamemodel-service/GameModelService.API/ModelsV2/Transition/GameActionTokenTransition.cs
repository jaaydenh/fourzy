using System;

namespace FourzyGameModel.Model
{
    public class GameActionTokenTransition : GameAction
    {
        public GameActionType Type { get { return GameActionType.TRANSITION; } }

        public GameActionTiming Timing { get { return GameActionTiming.MOVE; } }

        public BoardLocation Location { get; set; }
        public TransitionType Reason { get; set; }
        public IToken Before { get; set; }
        public IToken After { get; set; }

        public GameActionTokenTransition(BoardLocation Location, TransitionType Reason, IToken Before, IToken After)
        {
            this.Location = Location;
            this.Reason = Reason;
            this.Before = Before;
            this.After = After;
        }

        public string Print()
        {
            throw new NotImplementedException();
        }
    }
}
