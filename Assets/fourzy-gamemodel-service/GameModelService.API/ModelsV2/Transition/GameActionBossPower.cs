using System;

namespace FourzyGameModel.Model
{
    public class GameActionBossPower : GameAction
    {
        public GameActionType Type { get { return GameActionType.BOSS_POWER; } }
        public GameActionTiming Timing { get { return GameActionTiming.BEFORE_MOVE; } }
        public IBossPower Power { get; set; }

        public GameActionBossPower(IBossPower Power)
        {
            this.Power = Power;
        }

        public string Print()
        {
            throw new NotImplementedException();
        }
    }
}
