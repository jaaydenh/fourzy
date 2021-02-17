using System;
using System.Collections.Generic;

namespace FourzyGameModel.Model
{
    public class GameActionExplosion : GameAction
    {
        public GameActionType Type { get { return GameActionType.EFFECT; } }

        public GameActionTiming Timing { get { return GameActionTiming.AFTER_MOVE; } }

        public BoardLocation Center { get; set; }
        public List<BoardLocation> Explosion {get; set;}

        public GameActionExplosion(BoardLocation Center, List<BoardLocation> Explosion)
        {
            this.Center = Center;
            this.Explosion = Explosion;
        }

        public string Print()
        {
            throw new NotImplementedException();
        }
    }
}
