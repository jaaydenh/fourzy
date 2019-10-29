﻿namespace FourzyGameModel.Model
{
    public enum Direction { UP, DOWN, LEFT, RIGHT, NONE, TELEPORT, RANDOM }
    public enum WinDirection { VERTICAL, HORIZONTAL, DIAGONAL_NW_SE, DIAGONAL_NE_SW}
    public enum LineDirection { VERTICAL, HORIZONTAL, DIAGONAL_NW_SE, DIAGONAL_NE_SW, NONE}
    public enum DiagonalType { HIGHRIGHT, HIGHLEFT, NONE }
    public enum CompassDirection { NE, N, NW, W, E, SE, S, SW, NONE, RANDOM }
    public enum MoveType { SIMPLE, SPELL, TOKEN, BOSSPOWER,
        PASS
    }
    public enum PieceType { PLAYER, CHAMPION, BOULDER, ICE_BOULDER }
    public enum GameActionType { INVALID, ADD_PIECE, MOVE_PIECE, BUMP, PUSH, ADD_TOKEN, GAME_END, COLLECT, DESTROY, SPELL, STOP, TRANSITION, EFFECT, REMOVE_TOKEN, BOSS_POWER, PASS    }
    public enum GameActionTiming { BEFORE_MOVE, MOVE, AFTER_MOVE}
    public enum InvalidTurnType { BLOCKED, WRONGPLAYER, BADPLAYERID}
    public enum GameEndType { WIN, DRAW, NOPIECES}
    public enum TokenType { NONE, BLOCKER, STICKY, ARROW, 
        FOURWAY_ARROW, RIGHT_TURN, LEFT_TURN, ROTATING_ARROW,
        FRUIT, FRUIT_TREE,
        WATER, GHOST, PIT,  MOVING_GHOST,
        ICE, ICE_BLOCK, SNOW, SNOWMAN,
        SAND, CIRCLE_BOMB, CROSS_BOMB, QUICKSAND,
        TRAPDOOR,
        WISP,
        WIND, PORTAL, LURE, GOLD,  GUARD, TOGGLE_ROTATE, TOGGLE_FLIP, POPUP_BLOCKER, 
        COLD, FIRE, CURRENT, HEX, GRASS, SHARK, WEB, SPIDER, FLOWERS, WALL, ZONE, SWITCH, 
        RANDOM_FOURWAY, RANDOM_UPDOWN, RANDOM_LEFTRIGHT, 
        PUDDLE, GLASS_BLOCK, FOUNTAIN,
        MOVING_CLOUD, MOVING_SUN, MUD, VOLCANO, LAVA, 
        ARROW_ONCE, FALLAWAY_FLOOR, THIN_ICE, ARROW_ACTIVATE, SPRINGY_BLOCKER, MOVE_BLOCKER, BOUNCE,
        HIDDEN_BOMB
    }
    public enum Corner { UPLEFT, UPRIGHT, DOWNLEFT, DOWNRIGHT, NONE}
    public enum StopType { BUMP, FRICTION, MOMENTUM,
        INERTIA,
        TOKEN,
        WALL
    }
    public enum DestroyType { FALLING, QUICKSAND, PORTAL, BOMB, CRUSHED_BY_BLOCKER, EATEN_BY_SHARK, BURNING}
    public enum TransitionType {BURNING, BLOCK_ICE, SNOW_ICE, ICE_MELT, WATER_EVAPORATE, FRUIT_SQUASH, FRUIT_TREE, SNOWMAN_SMASH, SNOW_MELT, WATER_FREEZE, EAT_LURE, PORTAL_IMPLODES, GUARD_MARCH, GUARD_ABOUTFACE, SPELL_FADE, SHARK_SWIM, SHARK_EAT, SHARK_SPIN,
        FALLAWAY,
        SPLASH,
        EVAPORATE,
        ROTATE_ARROW,
        GHOST_TURN,
        GHOST_MOVE,
        BLOCKER_LOWER,
        BLOCKER_RAISE,
        TELEPORT_FROM,
        TELEPORT_TO,
        BOSS_POWER,
        SPIDER_HUNT,
        SPIDER_SQUISH,
        SPIDER_WEAVE,
        DROP_FRUIT,
        DROP_FRUIT_SQUISHED,
        GHOST_WRAPAROUND,
        GAME_EFFECT
    }
    public enum PortalColor { RED, YELLOW, GREEN, BLUE}
    public enum TokenColor { RED, YELLOW, GREEN, BLUE,
        NONE
    }
    //public enum SwitchColor { RED, YELLOW, GREEN, BLUE}
    public enum Rotation { CLOCKWISE, COUNTER_CLOCKWISE,
        NONE
    }
    public enum ElementType { FIRE, HEAT, COLD, WATER,
        EXPLOSION
    }
    public enum TokenClassification { TERRAIN, INSTRUCTION, SPELL, ITEM, CREATURE, EFFECT, BLOCKER}
    public enum PuzzleGoalType { WIN, SURVIVE, COVER}
    public enum PuzzleHintType { CLUE, MOVE}
    public enum PlayerType { HUMAN, AI, BOSS }
    public enum GameType { STANDARD, AI, BOSS}
}

