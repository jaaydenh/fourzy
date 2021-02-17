using System;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class GameActionPuzzleStatus : GameAction
    {
        public GameActionType Type { get { return GameActionType.GAME_END; } }

        public GameActionTiming Timing { get { return GameActionTiming.AFTER_MOVE; } }

        public PuzzleStatus Status { get; set; }
        public PuzzleEvent Event { get; set; }
         
        public GameActionPuzzleStatus(PuzzleStatus Status, PuzzleEvent Event)
        {
            this.Status = Status;
            this.Event = Event;
        }

        public string Print()
        {
            throw new NotImplementedException();
        }
    }
}
