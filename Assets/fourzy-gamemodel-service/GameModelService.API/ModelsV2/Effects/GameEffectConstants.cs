using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public static class GameEffectConstants
    {
        public const int DefaultFrequency = 3;
    }

    public enum GameEffectTiming { START_OF_TURN, END_OF_TURN, PASSIVE, TRIGGERED }
    public enum GameEffectType { LIFE,VOID,SHINE,RAIN,ROTATING_SIDE_WALL,SIDESPIKE,MOVINGWALL,DETECT_WIN, DETECT_LOSS }
}
