using System;

namespace FourzyGameModel.Model
{
    public class GameActionGameEffect : GameAction
    {
        public GameActionType Type { get { return GameActionType.GAME_EFFECT; } }
        public GameActionTiming Timing { get; set; }
        public IGameEffect Effect { get; set; }

        public GameActionGameEffect(IGameEffect GameEffect)
        {
            this.Effect = GameEffect;
            switch (GameEffect.Timing)
            {
                case GameEffectTiming.END_OF_TURN:
                    this.Timing = GameActionTiming.AFTER_MOVE;
                    break;
                case GameEffectTiming.START_OF_TURN:
                    this.Timing = GameActionTiming.BEFORE_MOVE;
                    break;
                      
            }
        }

        public string Print()
        {
            throw new NotImplementedException();
        }
    }
}
