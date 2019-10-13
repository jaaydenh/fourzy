using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace FourzyGameModel.Model
{
    public static class AIConstants
    {
        public const int DefaultPositionWeight = 10;
        public const int DefaultFourWeight = 10;
        public const int DefaultFiveWeight = 25;
        //public const int ContiguousBonus = 10;

    }

    public enum AIProfile { Player,
        PuzzleAI,       // 1
        SimpleAI,       // 2
        PassBot,        // 3
        PositionBot,    // 4
        BossAI,         // 5
        BeginnerAI,     // 6
        EasyAI,         // 7
        BetterAI,       // 8
        ScoreBot,       // 9
        UpBot,          // 10
        RotatorBot,     // 11
        HorizontalBot,  // 12
        VerticalBot,    // 13
        WaitBot,        // 14
        ExtenderBotAI,  // 15
        DownAI,         // 16
        PanicBot,       // 17
        RightAI,        // 18
        LeftAI,         // 19
        DoctorBot,      // 20
        BlindBot,       // 21
        BadBot,         // 22
        SmartBot,       // 23
        AggressiveAI    // 24
    }

    public enum AIDifficulty { Pushover, Easy, Medium, Hard, Doctor}

}
