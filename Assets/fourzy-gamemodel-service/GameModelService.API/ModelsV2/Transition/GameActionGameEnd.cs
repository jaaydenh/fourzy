using System;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class GameActionGameEnd : GameAction
    {
        public GameActionType Type { get { return GameActionType.GAME_END; } }

        public GameActionTiming Timing { get { return GameActionTiming.AFTER_MOVE; } }

        public GameEndType GameEndType { get; set; }
        public List<BoardLocation> WinningLocations { get; set; }
        public int WinnerId { get; set; }

        public GameActionGameEnd(GameEndType Type, int WinnerId, List<BoardLocation> WinningLocations)
        {
            this.GameEndType = Type;
            this.WinnerId = WinnerId;
            this.WinningLocations = new List<BoardLocation>();
            if (WinningLocations!=null)
            {
                foreach (BoardLocation loc in WinningLocations)
                {
                    this.WinningLocations.Add(loc);
                }
            }
        }

        public string Print()
        {
            throw new NotImplementedException();
        }
    }
}
