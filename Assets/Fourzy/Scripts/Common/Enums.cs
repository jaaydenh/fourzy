namespace Fourzy
{
    public enum PlayerEnum { NONE, ONE, TWO, ALL, EMPTY };
    public enum ChallengeType { NONE, STANDARD, TOURNAMENT };
    public enum ChallengeState { NONE, RUNNING, ISSUED, COMPLETE };
    public enum GameType
    {
        NONE,
        TURN_BASED,
        PASSANDPLAY,
        FRIEND,
        LEADERBOARD,
        PUZZLE,
        AI,
        REALTIME,
        ONBOARDING,
        PRESENTATION,
        TRY_TOKEN,
    };

    public enum GameMode { NONE, PUZZLE_FAST, PUZZLE_PACK, AI_PACK, BOSS_AI_PACK, GAUNTLET, VERSUS }
    public enum AIPlayerSkill { LEVEL1, LEVEL2, LEVEL3 };

    public enum CurrencyType
    {
        COINS = 0,
        XP = 1,
        TICKETS = 2,
        MAGIC = 3,
        GEMS = 4,
        PORTAL_POINTS = 7,
        RARE_PORTAL_POINTS = 8,
        HINTS = 9,
        NONE = 10,
    }

    public enum RewardType
    {
        COINS = 0,
        XP = 1,
        TICKETS = 2,
        MAGIC = 3,
        GEMS = 4,
        PACK_COMPLETE = 5,
        PORTAL_POINTS = 6,
        RARE_PORTAL_POINTS = 7,
        GAME_PIECE = 8,
        OPEN_PORTAL = 9,
        OPEN_RARE_PORTAL = 10,
        HINTS = 11,

        CUSTOM = 99,
    }
}
