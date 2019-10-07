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
        PuzzleAI,
        SimpleAI,
        PassBot,
        PositionBot,
        BossAI,
        BeginnerAI,
        EasyAI,
        BetterAI,
        ScoreBot,
        UpBot,
        RotatorBot,
        HorizontalBot,
        VerticalBot,
        WaitBot,
        ExtenderBotAI,
        DownAI,
        PanicBot,
        RightAI,
        LeftAI,
        DoctorBot,
        BlindBot,
        BadBot
    }

    public enum AIDifficulty { Pushover, Easy, Medium, Hard, Doctor}

}
