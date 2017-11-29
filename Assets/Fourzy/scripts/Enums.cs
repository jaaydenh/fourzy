﻿namespace Fourzy
{
    //                  0      1         2           3           4            5       6        7      8          9    10                  11                 12      13
    public enum Token { EMPTY, UP_ARROW, DOWN_ARROW, LEFT_ARROW, RIGHT_ARROW, STICKY, BLOCKER, GHOST, ICE_SHEET, PIT, NINETY_RIGHT_ARROW, NINETY_LEFT_ARROW, BUMPER, COIN };
    public enum Piece { EMPTY, BLUE, RED };
    public enum Player { NONE, ONE, TWO, ALL };
    public enum Direction { UP, DOWN, LEFT, RIGHT, NONE, REVERSE };
    public enum PieceAnimStates { NONE, DROPPING };
    public enum ChallengeType { NONE, STANDARD, TOURNAMENT };
    public enum ChallengeState { NONE, RUNNING, ISSUED, COMPLETE };
    public enum GameType { RANDOM, PASSANDPLAY, FRIEND, LEADERBOARD, PUZZLE, AI };
    public enum AIPlayerSkill { LEVEL1, LEVEL2 };
}
