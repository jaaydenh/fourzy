//modded @vadym udod
//add scene names constants

namespace Fourzy
{
    public static class Constants
    {
        public const int numRows = 8;
        public const int numColumns = 8;
        public const int numPiecesToWin = 4;
        public const float moveSpeed = 8f;
        public const int randomGeneratedBoardPercentage = 50;
        public const int playerMoveTimer_InitialTime = 30000;
        public const int playerMoveTimer_AdditionalTime = 12000;

        public const string GAMEPLAY_SCENE_NAME = "gamePlayNew";
        public const string MAIN_MENU_SCENE_NAME = "tabbedUI";
    }
}
