namespace Fourzy
{
    public enum PlayerEnum { NONE, ONE, TWO, ALL, EMPTY };
    public enum ChallengeType { NONE, STANDARD, TOURNAMENT };
    public enum ChallengeState { NONE, RUNNING, ISSUED, COMPLETE };
    public enum GameType { NONE, TURN_BASED, PASSANDPLAY, FRIEND, LEADERBOARD, PUZZLE, AI, REALTIME, ONBOARDING, DEMO};
    public enum AIPlayerSkill { LEVEL1, LEVEL2, LEVEL3 };

    public enum CurrencyType
    {
        COINS = 0,
        XP = 1,
        TICKETS = 2,
        MAGIC = 3,
        GEMS = 4,
        PORTALS = 5,
        RARE_PORTALS = 6,
        PORTAL_POINTS = 7,
        RARE_PORTAL_POINTS = 8,
        GAME_PIECE = 9,
    }
}
