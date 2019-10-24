//modded @vadym udod
//add scene names constants

using System.Collections.Generic;

namespace Fourzy
{
    public static class Constants
    {
        public const float DEMO_IDLE_TIME = 45f;
        public const float DEMO_HIGHLIGHT_POSSIBLE_MOVES_TIME = 8f;

        public const int PORTAL_POINTS = 100;
        public const int RARE_PORTAL_POINTS = 10;

        public const float BASE_MOVE_SPEED = 8f;
        public const float MOVE_SPEED_CAP = 16f;
        public const int REALTIME_COUNTDOWN_SECONDS = 6;

        public const int timerSections = 5;
        public const int circularTimerValue = 8;
        public const int initialTimerValue = 3;
        public const int aiTurnTimerResetValue = 2;
        public const int addTimerBarEveryXTurn = 2;
        public const int barsToAdd = 1;
        
        public const int running_challenges_count = 100;
        public const int complete_challenges_count = 50;

        public const string GAME_MODE_FAST_PUZZLES = "unlock_fast_puzzles_mode";
        public const string GAME_MODE_GAUNTLET_GAME = "unlock_gauntlet_mode";

        public const string GAMEPLAY_SCENE_NAME = "gamePlayNew";
        public const string MAIN_MENU_SCENE_NAME = "tabbedUINew";
        public const string LOGO_SCENE_NAME = "logo";
        public const string MAIN_MENU_CANVAS_NAME = "MainMenuCanvas";
        public const string GAMEPLAY_MENU_CANVAS_NAME = "GameSceneCanvas";

        public const string PUZZLE_PACKS_ROOT_FOLDER = "PuzzlePacks";
        public const string PUZZLES_ROOT_FOLDER = "PuzzlePool";

        #region Remote Settings

        public const string KEY_APP_VERSION = "app_version";
        public const string KEY_DAILY_PUZZLE = "daily_puzzle";
        public const string KEY_EXTRA_FEATURES = "extra_features";
        public const string KEY_STORE_STATE = "store";

        //rewards
        public const string KEY_REWARDS_TURNBASED = "rewards_turnbased";
        public const string KEY_REWARDS_PASSPLAY = "rewards_screen_passplay";
        public const string KEY_REWARDS_PUZZLEPLAY = "rewards_screen_puzzleplay";
        public const string KEY_REWARDS_REALTIME = "rewards_screen_realtime";

        #endregion

        //photon
        public const string PLAYER_1_READY = "p1ready";
        public const string PLAYER_2_READY = "p2ready";
        public const string EPOCH_KEY = "mc_epoch";

        //events codes
        public const byte GAME_DATA = 0;
        public const byte TAKE_TURN = 1;

    }
}
