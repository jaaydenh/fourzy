using System;

namespace FourzyGameModel.Model
{
    public class GameActionTokenRemove: GameAction
    {
        public GameActionType Type { get { return GameActionType.REMOVE_TOKEN; } }

        public GameActionTiming Timing { get { return GameActionTiming.BEFORE_MOVE; } }

        public BoardLocation Location { get; set; }
        public TransitionType Reason { get; set; }
        public IToken Before { get; set; }

        public GameActionTokenRemove(BoardLocation Location, TransitionType Reason, IToken Before)
        {
            this.Location = Location;
            this.Reason = Reason;
            this.Before = Before;
        }

        public string Print()
        {
            throw new NotImplementedException();
        }
    }
}
