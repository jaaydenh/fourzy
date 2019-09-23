using System;

namespace FourzyGameModel.Model
{
    public class GameActionPass : GameAction
    {
        public GameActionType Type { get { return GameActionType.STOP; } }

        public GameActionTiming Timing { get { return GameActionTiming.MOVE; } }

        Player Player { get; set; }

        public GameActionPass(Player Player)
        {
            this.Player = Player;
        }

        public string Print()
        {
            throw new NotImplementedException();
        }
    }
}
