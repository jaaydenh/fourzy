﻿namespace Fourzy
{
    //                  0      1         2           3           4            5       6        7      8          9    10                  11                 12      13    14     15          16   17
    public enum Token { EMPTY, UP_ARROW, DOWN_ARROW, LEFT_ARROW, RIGHT_ARROW, STICKY, BLOCKER, GHOST, ICE_SHEET, PIT, NINETY_RIGHT_ARROW, NINETY_LEFT_ARROW, BUMPER, COIN, FRUIT, FRUIT_TREE, WEB, SPIDER };
    public enum Piece { EMPTY, BLUE, RED };
    public enum PlayerEnum { NONE, ONE, TWO, ALL, EMPTY};
    public enum Direction { UP, DOWN, LEFT, RIGHT, NONE, REVERSE };
    public enum PieceAnimState { NONE, FALLING, MOVING, ASLEEP };
    public enum ChallengeType { NONE, STANDARD, TOURNAMENT };
    public enum ChallengeState { NONE, RUNNING, ISSUED, COMPLETE };
    public enum GameType { NONE, RANDOM, PASSANDPLAY, FRIEND, LEADERBOARD, PUZZLE, AI };
    public enum AIPlayerSkill { LEVEL1, LEVEL2, LEVEL3 };
    public enum Screens { NONE, GAME, GAMES_LIST, SOCIAL, LEADERBOARD, CREATE_GAME, GAME_OPTIONS };
}