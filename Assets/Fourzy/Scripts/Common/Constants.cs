//@vadym udod

using FourzyGameModel.Model;

namespace Fourzy
{
    public static class Constants
    {
        public const float DEMO_IDLE_TIME = 45f;
        public const float DEMO_HIGHLIGHT_POSSIBLE_MOVES_TIME = 8f;

        public const int DEFAULT_PLACEMENT_STYLE = (int)GameManager.PlacementStyle.SWIPE_STYLE_2;
        public const Area DEFAULT_AREA = Area.TRAINING_GARDEN;
        public const int DEFAULT_UNLOCKED_THEMES = 30;
        public const int PORTAL_POINTS = 100;
        public const int RARE_PORTAL_POINTS = 10;

        public const float BASE_MOVE_SPEED = 8f;
        public const float MOVE_SPEED_CAP = 16f;
        public const int REALTIME_COUNTDOWN_SECONDS = 6;
        public const int GAUNTLET_DEFAULT_MOVES_COUNT = 5;

        public const int TIMER_SECTIONS = 5;
        public const int CIRCULAR_TIMER_VALUE = 8;
        public const int INITIAL_TIMER_TIME = 3;
        public const int AI_TURN_TIMER_RESET_VALUE = 2;
        public const int ADD_TIMER_BAR_EVERY_X_TURN = 2;
        public const int BARS_TO_ADD = 1;

        public const int RUNNING_CHALLENGES_COUNT = 100;
        public const int COMPLETE_CHALLENGES_COUNT = 50;

        public const string GAME_MODE_FAST_PUZZLES = "unlock_fast_puzzles_mode";
        public const string GAME_MODE_GAUNTLET_GAME = "unlock_gauntlet_mode";

        public const string GAMEPLAY_SCENE_NAME = "gamePlayNew";
        public const string MAIN_MENU_SCENE_NAME = "tabbedUINew";
        public const string LOGO_SCENE_NAME = "logo";
        public const string MAIN_MENU_CANVAS_NAME = "MainMenuCanvas";
        public const string GAMEPLAY_MENU_CANVAS_NAME = "GameSceneCanvas";

        public const string PUZZLE_PACKS_ROOT_FOLDER = "PuzzlePacks";
        public const string PUZZLES_ROOT_FOLDER = "PuzzlePool";
        public const string LOCALIZATION_FOLDER = "Localization";

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
