namespace Fourzy
{
    public enum Token {EMPTY, UP_ARROW, DOWN_ARROW, LEFT_ARROW, RIGHT_ARROW, STICKY, BLOCKER, GHOST, ICE_SHEET, PIT, NINETY_RIGHT_ARROW, NINETY_LEFT_ARROW, BUMPER};
    public enum Piece {EMPTY, BLUE, RED};
    public enum Player {NONE, ONE, TWO, ALL};
    public enum Direction {UP, DOWN, LEFT, RIGHT, NONE, REVERSE};
    public enum PieceAnimStates {NONE, DROPPING};
}
