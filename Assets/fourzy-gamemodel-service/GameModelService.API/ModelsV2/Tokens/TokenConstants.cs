using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FourzyGameModel.Model
{
    public static class TokenConstants
    {
        public const char Up = 'U';
        public const char Down = 'D';
        public const char Left = 'L';
        public const char Right = 'R';

        public static List<Direction> GetDirections()
        {
            return new List<Direction>() { Direction.UP, Direction.DOWN, Direction.LEFT, Direction.RIGHT }; 
        }

        //Arrow Token Characters
        public const char UpArrowTokenChararacter = '\u2191';
        public const char DownArrowTokenChararacter = '\u2193';
        public const char LeftArrowTokenChararacter = '\u2190';
        public const char RightArrowTokenChararacter = '\u2192';

        public const char Arrow = 'A';
        public const char ArrowActivate = 'a';
        public const char ArrowOnce = '1';
        public const char Blocker = 'B';
        public const char Bounce = '&';
        public const char CircleBomb = '0';
        public const char Cold = 'g';
        public const char CrossBomb = '-';
        public const char Fallaway = 'v';
        public const char Fire = 'F';
        public const char Flowers = 'f';
        public const char Fountain = 'Y';
        public const char FourWayArrow = '+';
        public const char Fruit = '.';
        public const char FruitTree = 'T';
        public const char Ghost = 'G';
        public const char GlassBlock = 'O';
        public const char Gold = '$';
        public const char Guard = '!';
        public const char Grass = ',';
        public const char Hex = 'H';
        public const char HiddenBomb = 'h';
        public const char Ice = 'i';
        public const char IceBlock = 'I';
        public const char Lava = 'l';
        public const char LeftTurn = ')';
        public const char Lure = 'K';
        public const char MoveBlocker = 'M';
        public const char MovingCloud = 'C';
        public const char MovingGhost = 'o';
        public const char MovingSun = 'N';
        public const char Mud = 'm';
        public const char Pit = 'X';
        public const char PopUpBlocker = 'U';
        public const char Portal = '@';
        public const char Puddle = 'p';
        public const char RandomFourWay = ':';
        public const char RandomLeftRight = '_';
        public const char RandomUpDown = '|';
        public const char RightTurn = '(';
        public const char RotatingArrow = '%';
        public const char Sand = 's';
        public const char Shark = '7';
        public const char Snow = '*';
        public const char Snowman = '8';
        public const char Spider = '9';
        public const char SpiderWeb = 'W';
        public const char SpringyBlocker = '`';
        public const char Sticky = 'S';
        public const char Switch = ']';
        public const char ToggleArrow = '^';
        public const char TrapDoor = '/';
        public const char ThinIce = '2';
        public const char Quicksand = 'q';
        public const char Volcano = 'V';
        public const char Wall = '#';
        public const char Water = '~';
        public const char Wind = '=';
        public const char Wisp = 'Z';
        public const char Zone = 'z';

        public const int WispMovePercentage = 50;

        public const int DefaultMagicArrowGain = 10;
        public const int DefaultVoidArrowLoss = 5;
        public const int PopUpBlockerFrequency = 3;

        public const char Clockwise = '+';
        public const char CounterClockwise = '-';

        public const int COMPLEXITY_SIMPLE = 1;
        public const int COMPLEXITY_BASIC = 2;
        public const int COMPLEXITY_NORMAL = 3;
        public const int COMPLEXITY_MEDIUM = 4;
        public const int COMPLEXITY_HARD = 5;
        public const int COMPLEXITY_CHALLENGE = 10;


        public static Direction GetOrientation(char orientation)
        {
            switch (orientation)
            {
                case TokenConstants.Up:
                    return Direction.UP;
                case TokenConstants.Down:
                    return Direction.DOWN;
                case TokenConstants.Left:
                    return Direction.LEFT;
                case TokenConstants.Right:
                    return Direction.RIGHT;
            }
            return Direction.NONE;
        }

        public static Direction IdentifyDirection(string DirectionString)
        {
            switch (DirectionString.ToCharArray(0,1)[0])
            {
                case TokenConstants.Up:
                    return Direction.UP;
                case TokenConstants.Down:
                    return Direction.DOWN;
                case TokenConstants.Left:
                    return Direction.LEFT;
                case TokenConstants.Right:
                    return Direction.RIGHT;
            }
            return Direction.NONE;
        }

        public static string DirectionString(Direction Direction)
        {
            switch (Direction)
            {
                case Direction.UP:
                    return TokenConstants.Up.ToString();
                case Direction.DOWN:
                    return TokenConstants.Down.ToString();
                case Direction.LEFT:
                    return TokenConstants.Left.ToString();
                case Direction.RIGHT:
                    return TokenConstants.Right.ToString();
            }
            return "";
        }

        public static string NotateRotation(Rotation RotateDirection)
        {
            switch (RotateDirection)
            {
                case Rotation.CLOCKWISE:
                    return TokenConstants.Clockwise.ToString();
                case Rotation.COUNTER_CLOCKWISE:
                    return TokenConstants.CounterClockwise.ToString();
            }
            return "";
        }

        public static Rotation GetRotation(char RotationChar)
        {
            switch (RotationChar)
            {
                case TokenConstants.Clockwise:
                    return Rotation.CLOCKWISE;
                case TokenConstants.CounterClockwise:
                    return Rotation.COUNTER_CLOCKWISE;
            }
            return Rotation.CLOCKWISE;
        }

        public static MoveMethod IdentifyMoveMethod(string MoveString)
        {
            switch (MoveString.ToCharArray(0, 1)[0])
            {
                case 'C':
                    return MoveMethod.CLOCKWISE;
                case 'R':
                    return MoveMethod.COUNTERCLOCKWISE;
                case 'H':
                    return MoveMethod.HORIZONTAL_PACE;
                case 'V':
                    return MoveMethod.VERTICAL_PACE;
            }
            return MoveMethod.NONE;
        }

        public static string MoveString(MoveMethod Move)
        {
            switch (Move)
            {
                case MoveMethod.CLOCKWISE:
                    return "C";
                case MoveMethod.COUNTERCLOCKWISE:
                    return "R";
                case MoveMethod.HORIZONTAL_PACE:
                    return "H";
                case MoveMethod.VERTICAL_PACE:
                    return "V";
            }
            return "";
        }

        public static TokenColor GetColor(string Color)
        {
            switch (Color)
            {
                case "R":
                    return TokenColor.RED;
                case "B":
                    return TokenColor.BLUE;
                case "G":
                    return TokenColor.GREEN;
                case "Y":
                    return TokenColor.YELLOW;

            }
            return TokenColor.NONE;
        }

        public static string NotateColor(TokenColor Color)
        {
            switch (Color)
            {
                case TokenColor.RED:
                    return "R";
                case TokenColor.BLUE:
                    return "B";
                case TokenColor.GREEN:
                    return "G";
                case TokenColor.YELLOW:
                    return "Y";
            }
            return "";
        }

    }

    public enum MoveMethod { HORIZONTAL_PACE, VERTICAL_PACE, CLOCKWISE, COUNTERCLOCKWISE, NONE, WRAPAROUND };

}
