//@vadym udod

using Fourzy._Updates;
using FourzyGameModel.Model;

namespace Fourzy
{
    public static class Constants
    {
        public const float DEMO_IDLE_TIME = 45f;
        public const float DEMO_HIGHLIGHT_POSSIBLE_MOVES_TIME = 8f;

        public const int DEFAULT_PLACEMENT_STYLE_TOUCH = (int)GameManager.PlacementStyle.EDGE_TAP;
        public const int DEFAULT_PLACEMENT_STYLE_POINTER = (int)GameManager.PlacementStyle.EDGE_TAP;
        public const Area DEFAULT_AREA = Area.TRAINING_GARDEN;
        public static Area[] UNLOCKED_AREAS = { Area.TRAINING_GARDEN };
        public const int DEFAULT_STANDALONE_CPU_DIFFICULTY = 1; //0-3
        public static BotGameSettings BOT_SETTINGS = new BotGameSettings()
        {
            botMatchAfter = new float[] { 10f, 15f },
            turnDelayTime = new float[] { 1.2f, 2.5f },
            rematchAcceptTime = new float[] { 3f, 6f },
            maxRematchTimes = new int[] { 2, 4 },
        };
        public static TokenType[] DEFAULT_UNLOCKED_TOKENS = { };
        public const int PORTAL_POINTS = 100;
        public const int RARE_PORTAL_POINTS = 10;
        public const float EXTRA_DELAY_BETWEEN_TURNS = 0.1f;

        public const float AREA_PROGRESSION_BEFORE_DELAY = 1f;
        public const float AREA_PROGRESSION_OPEN_SCREEN_DELAY = 2f;

        public const float GAMEPIECE_AFTER_SPAWN_DELAY = .6f;
        public const float BASE_MOVE_SPEED = 8f;
        public const float MOVE_SPEED_CAP = 16f;
        public const int REALTIME_COUNTDOWN_SECONDS = 4;
        public const int MIN_COMPLEXCITY_SCORE = 1;
        public const int MAX_COMPLEXCITY_SCORE = 20000;

        /// <summary>
        /// This value is only used when bot match wait time is < 0f, so it will time out
        /// </summary>
        public const float REALTIME_OPPONENT_WAIT_TIME = 30f;
        public const float LOBBY_GAME_LOAD_DELAY = 2.2f;
        public const float PHOTON_CONNECTION_WAIT_TIME = 5f;
        public const float TIME_UNTIL_MATCHED_WITH_BOT = 30f;

        //gauntlet
        public const int GAUNTLET_DEFAULT_MOVES_COUNT = 60;
        public const int BASE_GAUNTLET_MOVES_COST = 2;
        public const int GAUNTLET_RECHARGE_AMOUNT = 10;

        public const int TIMER_SECTIONS = 5;
        public const int INITIAL_TIMER_SECTIONS = 3;
        public const int CIRCULAR_TIMER_SECONDS = 8;
        public const int RESET_TIMER_SECTIONS = 2;

        // Adds additional time to timer after X turns
        public const int ADD_TIMER_BAR_EVERY_X_TURN = 0;
        public const int BARS_TO_ADD = 1;
        public const bool LOSE_ON_EMPTY_TIMER = true;

        public const int RUNNING_CHALLENGES_COUNT = 100;
        public const int COMPLETE_CHALLENGES_COUNT = 50;
        public const int GAMES_BEFORE_RATING_USED = 5;

        public const string GAME_MODE_FAST_PUZZLES = "unlock_fast_puzzles_mode";
        public const string GAME_MODE_GAUNTLET_GAME = "unlock_gauntlet_mode";

        public const string GAMEPLAY_SCENE_NAME = "gamePlayNew";
        public const string MAIN_MENU_P_SCENE_NAME = "portraitMainMenu";
        public const string MAIN_MENU_L_SCENE_NAME = "landscapeMainMenu";
        public const string LOGO_SCENE_NAME = "logo";
        public const string MAIN_MENU_CANVAS_NAME = "MainMenuCanvas";
        public const string GAMEPLAY_MENU_CANVAS_NAME = "GameSceneCanvas";

        //playfab inventory keys
        public const string PLAYFAB_GAMEPIECE_CLASS = "gamepiece";
        public const string PLAYFAB_TOKEN_CLASS = "token";
        public const string PLAYFAB_BUNDLE_CLASS = "progressionReward";
        public const string PLAYFAB_SPELL_TAG = "spell";
        public const string HINTS_CURRENCY_KEY = "HI";
        public const string COINS_CURRENCY_KEY = "CO";
        public const string GEMS_CURRENCY_KEY = "GE";
        //

        public const string DEFAULT_GAME_PIECE = "cyclops";
        public const int MAX_PLAYER_LEVEL = 32;
        public const int MIN_PLAYER_LEVEL = 1;

        public const string BOARDS_ROOT = "GameBoards";
        public const string FAST_PUZZLES_FOLDER = BOARDS_ROOT + "/FastPuzzles";
        public const string REALTIME_BOT_BOARDS_FOLDER = BOARDS_ROOT + "/FTUE_ProgressionBoards";
        public const string PUZZLE_PACKS_FOLDER = BOARDS_ROOT + "/PuzzlePacks";
        public const string PASS_AND_PLAY_BOARDS_FOLDER = BOARDS_ROOT + "/PassAndPlayBoards";
        public const string INSTRUCTION_BOARDS_FOLDER  = BOARDS_ROOT + "/InstructionBoards";
        public const string TRY_IT_BOARDS_FOLDER = BOARDS_ROOT + "/TryItBoards";
        public const string MISC_BOARDS_FOLDER = BOARDS_ROOT + "/MiscBoards";

        public const string AREAS_PROGRESSION_FOLDER = "AreasProgression";
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
        public static readonly float REALTIME_GAME_VALID_AFTER_X_SECONDS = 10f;
        public static readonly string REALTIME_RATING_KEY = "r";
        public static readonly string REALTIME_WINS_KEY = "w";
        public static readonly string REALTIME_LOSES_KEY = "l";
        public static readonly string REALTIME_DRAWS_KEY = "d";
        public static readonly string REALTIME_DEFAULT_GAMEPIECE_KEY = "1";

        public static readonly string REALTIME_ROOM_PLAYER_1_READY = "p1ready";
        public static readonly string REALTIME_ROOM_PLAYER_2_READY = "p2ready";
        public static readonly string REALTIME_ROOM_PLAYER_1_REMATCH = "p1rematch";
        public static readonly string REALTIME_ROOM_PLAYER_2_REMATCH = "p2rematch";

        public static readonly string REALTIME_ROOM_AREA = "area";
        public static readonly string REALTIME_ROOM_GAMEPIECE_KEY = "p";
        public static readonly string REALTIME_MID_KEY = "mid";
        public static readonly string REALTIME_ROOM_TYPE_KEY = "type";
        public static readonly string REALTIME_ROOM_PASSWORD = "pword";
        public static readonly string REALTIME_ROOM_TIMER_KEY = "timer";
        public static readonly string REALTIME_ROOM_MAGIC_KEY = "magic";
        public static readonly string REALTIME_ROOM_RATING_KEY = "rating";
        public static readonly string REALTIME_ROOM_GAMES_TOTAL_KEY = "gamesTotal";
        public static readonly int REALTIME_ROOM_PASSWORD_LENGTH = 5;

        //events codes
        public const byte GAME_DATA = 0;
        public const byte TAKE_TURN = 1;
        public const byte REMATCH_REQUEST = 2;
        public const byte RATING_GAME_DATA = 3;
        public const byte RATING_GAME_OTHER_LOST = 4;

        public const string CreateGameEndpoint = "http://fourzyfunctions.azurewebsites.net/api/CreateGame";
    }
}
