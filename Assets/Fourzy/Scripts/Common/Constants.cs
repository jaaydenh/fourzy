//modded @vadym udod
//add scene names constants

using System.Collections.Generic;

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
        public const int running_challenges_count = 100;
        public const int complete_challenges_count = 50;

        public const string GAMEPLAY_SCENE_NAME = "gamePlayNew";
        public const string MAIN_MENU_SCENE_NAME = "tabbedUINew";
        public const string LOGO_SCENE_NAME = "logo";

        public const string KEY_APP_VERSION = "app_version";
        public const string KEY_DAILY_PUZZLE = "daily_puzzle";
    }
}
