//@vadym udod

using Firebase.Analytics;
using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics;
using Fourzy._Updates.Tools;
using FourzyGameModel.Model;
using GameAnalyticsSDK;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy
{
    public class AnalyticsManager : RoutinesBase
    {
        public const string GAME_ID_KEY = "game_id";
        public const string ENTRY_POINT_KEY = "entry_point";
        public const string GAME_TYPE_KEY = "game_type";
        public const string GAME_RESULT_KEY = "game_result";
        public const string ALL_TURNS_COUNT_KEY = "all_turns_count";
        public const string PLAYER_TURNS_COUNT_KEY = "player_turns_count";
        public const string PUZZLE_TURNS_LIMIT_KEY = "puzzle_turns_limit";
        public const string LEVEL_INDEX_KEY = "level_index";
        public const string EVENT_ID_KEY = "event_id";
        public const string GAUNTLET_MEMBERS_LEFT_KEY = "members_left";
        public const string GAUNTLET_ID_KEY = "gauntlet_id";
        public const string SESSION_ID_KEY = "session_id";
        public const string MAGIC_LEFT_KEY = "magic_left";
        public const string GAME_AI_PROFILE_KEY = "ai_profile";
        public const string GAME_BOSS_TYPE_KEY = "boss_type";
        public const string BOARD_JSON_KEY = "board_json";
        public const string BOSS_MOVES_COUNT = "boss_moves_count";
        public const string HINT_AVAILABLE_KEY = "hint_available";
        public const string HINT_NUMBER_KEY = "hint_turn_number";
        public const string HINT_STORE_ITEMS_KEY = "hint_store_items";
        public const string STORE_ITEM_KEY = "store_item_id";
        public const string AREA_KEY = "area";
        public const string USER_ID_KEY = "user_id";
        public const string OPPONENT_ID_KEY = "opponent_id";
        public const string PACK_ID_KEY = "pack_id";
        public const string GAMEPIECE_SELECT_KEY = "gamepiece_select";
        public const string SETTINGS_VALUE_NAME_KEY = "value_name";
        public const string SETTINGS_NEW_VALUE_KEY = "new_value";
        public const string SETTINGS_OLD_VALUE_KEY = "old_value";
        public const string HINT_USED_KEY = "hint_used";
        public const string TUTORIAL_NAME_KEY = "name";
        public const string TUTORIAL_STAGE_KEY = "key";
        public const string PUZZLE_SESSION_ID = "puzzle_session_id";
        public const string BOARD_ID = "board_id";
        public const string PUZZLE_ID = "puzzle_id";

        public static AnalyticsManager Instance
        {
            get
            {
                if (instance == null)
                    Initialize(true);

                return instance;
            }
        }

        public static bool DEBUG = false;

        /// <summary>
        /// To check if event is enabled
        /// </summary>
        public static Dictionary<AnalyticsGameEvents, bool> gameEventsSwitch = new Dictionary<AnalyticsGameEvents, bool>()
        {
            [AnalyticsGameEvents.VERSUS_GAME_START] = true,
            [AnalyticsGameEvents.VERSUS_GAME_END] = true,
            [AnalyticsGameEvents.RANDOM_PUZZLE_START] = true,
            [AnalyticsGameEvents.RANDOM_PUZZLE_END] = true,
            [AnalyticsGameEvents.GAUNTLET_LEVEL_START] = true,
            [AnalyticsGameEvents.GAUNTLET_LEVEL_END] = true,
            [AnalyticsGameEvents.AI_LEVEL_START] = true,
            [AnalyticsGameEvents.AI_LEVEL_END] = true,
            [AnalyticsGameEvents.BOSS_AI_LEVEL_START] = true,
            [AnalyticsGameEvents.BOSS_AI_LEVEL_END] = true,
            [AnalyticsGameEvents.PUZZLE_LEVEL_START] = true,
            [AnalyticsGameEvents.PUZZLE_LEVEL_END] = true,
            [AnalyticsGameEvents.PUZZLE_LEVEL_HINT_BUTTON_PRESS] = true,
            [AnalyticsGameEvents.PUZZLE_HINT_STORE_HINT_PURCHASE] = true,
            [AnalyticsGameEvents.EVENT_OPENED] = true,
            [AnalyticsGameEvents.EVENT_COMPLETED] = true,
            [AnalyticsGameEvents.SELECT_GAMEPIECE] = true,
        };

        public static Dictionary<AnalyticsEvents, bool> miscEventsSwitch = new Dictionary<AnalyticsEvents, bool>()
        {
            [AnalyticsEvents.SETTINGS_CHANGE] = true,
            [AnalyticsEvents.UI] = true,
            [AnalyticsEvents.TUTORIALS] = true,
            [AnalyticsEvents.ERROR] = true,
        };

        private static AnalyticsManager instance;

        public enum AnalyticsProvider
        {
            FIREBASE = 1,
            PLAYFAB = 2,
            GAME_ANALYTICS = 4,

            ALL = FIREBASE | PLAYFAB | GAME_ANALYTICS,
        }

        public enum AnalyticsGameEvents
        {
            NONE,
            VERSUS_GAME_START,
            VERSUS_GAME_END,
            RANDOM_PUZZLE_START,
            RANDOM_PUZZLE_END,
            GAUNTLET_LEVEL_START,
            GAUNTLET_LEVEL_END,
            AI_LEVEL_START,
            AI_LEVEL_END,
            BOSS_AI_LEVEL_START,
            BOSS_AI_LEVEL_END,
            PUZZLE_LEVEL_START,
            PUZZLE_LEVEL_END,
            PUZZLE_LEVEL_HINT_BUTTON_PRESS,
            PUZZLE_HINT_STORE_HINT_PURCHASE,
            EVENT_OPENED,
            EVENT_COMPLETED,
            SELECT_GAMEPIECE,
        }

        public enum AnalyticsEvents
        {

            /// <summary>
            /// Params: value_name, new_value, old_value
            /// </summary>
            SETTINGS_CHANGE = 0,

            /// <summary>
            /// Params: name, stage
            /// </summary>
            TUTORIALS = 1,

            UI = 2,

            ERROR = 20,
        }

        public enum AnalyticsUIButtons
        {
            none,

            create_game,
            pass_and_play,
            leaderboard_play,
            turn_play,
            puzzle_play,
            change_name,
        }

        public enum AnalyticsSettingsKeys
        {
            CHANGE_NAME,
        }

        public enum AnalyticsErrorType
        {
            puzzle_play,
            pass_play,
            turn_based,
            realtime,
            onboarding,
            main_menu,
            gameplay_scene,
            settings,
            challenge_manager,
            create_turn_base_game,
            create_realtime_game,

            unidentified,
        }

        public enum GameResultType
        {
            None,
            Win,
            Lose,
            Draw,
            NoPossibleMoves,
            Reset,
            Abandoned,
        }

        protected override void Awake()
        {
            base.Awake();

            GameAnalytics.Initialize();
        }

        public static void Initialize(bool _DEBUG = false)
        {
            if (instance != null) return;

            GameObject go = new GameObject("AnalyticsManager");
            go.transform.SetParent(null);
            instance = go.AddComponent<AnalyticsManager>();

            DEBUG = _DEBUG;

            DontDestroyOnLoad(go);
        }

        //public void Identify(string userID)
        //{
        //    FirebaseAnalytics.SetUserId(userID);

        //    if (DEBUG) Debug.Log("User identity: " + userID);
        //}

        //public void LogCreateGame(
        //    GameType gameType, Area area,
        //    string userID = "",
        //    string opponentID = "",
        //    AnalyticsProvider provider = AnalyticsProvider.ALL)
        //{
        //    if (!NetworkPass()) return;
        //    if (!gameEventsSwitch[AnalyticsGameEvents.GAME_CREATE]) return;

        //    Dictionary<string, object> @params = new Dictionary<string, object>();

        //    @params.Add(GAME_TYPE_KEY, gameType);
        //    @params.Add(AREA_KEY, area.ToString().ToLower());

        //    if (lastLoggedButtonConsumable != AnalyticsUIButtons.none) @params.Add(ENTRY_POINT_KEY, lastLoggedButtonConsumable);

        //    if (!string.IsNullOrEmpty(userID)) @params.Add(USER_ID_KEY, userID);
        //    if (!string.IsNullOrEmpty(opponentID)) @params.Add(OPPONENT_ID_KEY, opponentID);

        //    foreach (Enum value in Enum.GetValues(provider.GetType()))
        //    {
        //        if (provider.HasFlag(value))
        //        {
        //            switch (value)
        //            {
        //                case AnalyticsProvider.FIREBASE:
        //                    LogFirebaseEvent(AnalyticsGameEvents.GAME_CREATE.ToString().ToLower(), @params);

        //                    break;

        //                case AnalyticsProvider.PLAYFAB:
        //                    LogPlayFabPlayerEvent(AnalyticsGameEvents.GAME_CREATE.ToString().ToLower(), @params);

        //                    break;

        //                case AnalyticsProvider.GAME_ANALYTICS:
        //                    string values = $"{AnalyticsGameEvents.GAME_CREATE.ToString().ToLower()}:{area.ToString().ToLower()}";

        //                    if (lastLoggedButtonConsumable != AnalyticsUIButtons.none) values += $":{lastLoggedButtonConsumable.ToString().ToLower()}";
        //                    if (!string.IsNullOrEmpty(userID)) values += $":{userID}";
        //                    if (!string.IsNullOrEmpty(opponentID)) values += $":{opponentID}";

        //                    LogGameAnalyticsDesignEvent(values);

        //                    break;
        //            }
        //        }
        //    }

        //    lastLoggedButton = AnalyticsUIButtons.none;
        //}

        public void LogGame(
            AnalyticsGameEvents gameEventType,
            IClientFourzy game,
            AnalyticsProvider provider = AnalyticsProvider.ALL,
            params KeyValuePair<string, object>[] extraParams)
        {
            if (!NetworkPass()) return;
            if (!gameEventsSwitch[gameEventType] || gameEventType == AnalyticsGameEvents.NONE) return;

            Dictionary<string, object> @params = new Dictionary<string, object>();

            foreach (KeyValuePair<string, object> p in extraParams) @params.Add(p.Key, p.Value);

            @params.Add(GAME_ID_KEY, game.GameID);
            @params.Add(AREA_KEY, game._Area);

            switch (gameEventType)
            {
                case AnalyticsGameEvents.VERSUS_GAME_END:
                    @params.Add(ALL_TURNS_COUNT_KEY, game._allTurnRecord.Count);

                    break;

                case AnalyticsGameEvents.RANDOM_PUZZLE_START:
                    @params.Add(SESSION_ID_KEY, GameManager.Instance.sessionID);

                    break;

                case AnalyticsGameEvents.RANDOM_PUZZLE_END:
                    @params.Add(SESSION_ID_KEY, GameManager.Instance.sessionID);
                    @params.Add(PLAYER_TURNS_COUNT_KEY, game._playerTurnRecord.Count);
                    @params.Add(PUZZLE_TURNS_LIMIT_KEY, game.puzzleData.MoveLimit);

                    break;

                case AnalyticsGameEvents.GAUNTLET_LEVEL_START:
                    @params.Add(GAUNTLET_ID_KEY, game.puzzleData.pack.packID);
                    @params.Add(LEVEL_INDEX_KEY, game.puzzleData.puzzleIndex);
                    @params.Add(GAUNTLET_MEMBERS_LEFT_KEY, game.puzzleData.gauntletStatus.FourzyCount);
                    @params.Add(MAGIC_LEFT_KEY, game.me.Magic);
                    @params.Add(GAME_AI_PROFILE_KEY, game.puzzleData.aiProfile);
                    @params.Add(BOARD_JSON_KEY, JsonConvert.SerializeObject(game.puzzleData.gameBoardDefinition));

                    break;

                case AnalyticsGameEvents.GAUNTLET_LEVEL_END:
                    @params.Add(GAUNTLET_ID_KEY, game.puzzleData.pack.packID);
                    @params.Add(LEVEL_INDEX_KEY, game.puzzleData.puzzleIndex);
                    @params.Add(GAUNTLET_MEMBERS_LEFT_KEY, game.puzzleData.gauntletStatus.FourzyCount);
                    @params.Add(MAGIC_LEFT_KEY, game.me.Magic);
                    @params.Add(GAME_AI_PROFILE_KEY, game.puzzleData.aiProfile);
                    @params.Add(BOARD_JSON_KEY, JsonConvert.SerializeObject(game.puzzleData.gameBoardDefinition));
                    @params.Add(PLAYER_TURNS_COUNT_KEY, game._playerTurnRecord.Count);

                    break;

                case AnalyticsGameEvents.AI_LEVEL_START:
                    @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);
                    @params.Add(LEVEL_INDEX_KEY, game.puzzleData.puzzleIndex);
                    @params.Add(GAME_AI_PROFILE_KEY, game.puzzleData.aiProfile);

                    break;

                case AnalyticsGameEvents.AI_LEVEL_END:
                    @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);
                    @params.Add(LEVEL_INDEX_KEY, game.puzzleData.puzzleIndex);
                    @params.Add(GAME_AI_PROFILE_KEY, game.puzzleData.aiProfile);
                    @params.Add(PLAYER_TURNS_COUNT_KEY, game._playerTurnRecord.Count);

                    break;

                case AnalyticsGameEvents.BOSS_AI_LEVEL_START:
                    @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);
                    @params.Add(LEVEL_INDEX_KEY, game.puzzleData.puzzleIndex);
                    @params.Add(GAME_AI_PROFILE_KEY, game.puzzleData.aiProfile);
                    @params.Add(GAME_BOSS_TYPE_KEY, game.puzzleData.aiBoss);

                    break;

                case AnalyticsGameEvents.BOSS_AI_LEVEL_END:
                    @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);
                    @params.Add(LEVEL_INDEX_KEY, game.puzzleData.puzzleIndex);
                    @params.Add(GAME_AI_PROFILE_KEY, game.puzzleData.aiProfile);
                    @params.Add(GAME_BOSS_TYPE_KEY, game.puzzleData.aiBoss);
                    @params.Add(PLAYER_TURNS_COUNT_KEY, game._playerTurnRecord.Count);
                    @params.Add(BOSS_MOVES_COUNT, game.BossMoves);

                    break;

                case AnalyticsGameEvents.PUZZLE_LEVEL_START:
                    @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);
                    @params.Add(LEVEL_INDEX_KEY, game.puzzleData.puzzleIndex);

                    break;

                case AnalyticsGameEvents.PUZZLE_LEVEL_END:
                    @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);
                    @params.Add(LEVEL_INDEX_KEY, game.puzzleData.puzzleIndex);
                    @params.Add(PLAYER_TURNS_COUNT_KEY, game._playerTurnRecord.Count);
                    @params.Add(PUZZLE_TURNS_LIMIT_KEY, game.puzzleData.MoveLimit);

                    break;

                case AnalyticsGameEvents.PUZZLE_LEVEL_HINT_BUTTON_PRESS:
                    //try add eventid
                    if (game.puzzleData.pack) @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);

                    @params.Add(LEVEL_INDEX_KEY, game.puzzleData.puzzleIndex);
                    @params.Add(HINT_AVAILABLE_KEY, UserManager.Instance.hints > 0);
                    @params.Add(PLAYER_TURNS_COUNT_KEY, game._playerTurnRecord.Count);
                    @params.Add(HINT_NUMBER_KEY, PlayerPrefsWrapper.GetPuzzleHintProgress(game.GameID));
                    
                    break;

                case AnalyticsGameEvents.PUZZLE_HINT_STORE_HINT_PURCHASE:
                    //try add eventid
                    if (game.puzzleData.pack) @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);

                    @params.Add(LEVEL_INDEX_KEY, game.puzzleData.puzzleIndex);
                    @params.Add(PLAYER_TURNS_COUNT_KEY, game._playerTurnRecord.Count);
                    @params.Add(HINT_NUMBER_KEY, PlayerPrefsWrapper.GetPuzzleHintProgress(game.GameID));

                    break;
            }

            foreach (Enum value in Enum.GetValues(provider.GetType()))
            {
                if (provider.HasFlag(value))
                {
                    switch (value)
                    {
                        case AnalyticsProvider.FIREBASE:
                            LogFirebaseEvent(gameEventType.ToString().ToLower(), @params);

                            break;

                        case AnalyticsProvider.PLAYFAB:
                            LogPlayFabPlayerEvent(gameEventType.ToString().ToLower(), @params);

                            break;

                        //case AnalyticsProvider.GAME_ANALYTICS:
                        //    string values = $"{gameEventType.ToString().ToLower()}";

                        //    values += $":{game._Type.ToString().ToLower()}";

                        //    values += $":{game._Area.ToString().ToLower()}";

                        //    if (game.puzzleData && game.puzzleData.pack)
                        //    {
                        //        values += $":pack_id-{game.puzzleData.pack.packID.Truncate(7)}";
                        //    }

                        //    LogGameAnalyticsDesignEvent(values);

                        //    break;
                    }
                }
            }
        }
        public void LogEvent(
            AnalyticsGameEvents gameEventType,
            AnalyticsProvider provider = AnalyticsProvider.ALL,
            params KeyValuePair<string, object>[] extraParams)
        {
            if (!NetworkPass()) return;
            if (!gameEventsSwitch[gameEventType] || gameEventType == AnalyticsGameEvents.NONE) return;

            Dictionary<string, object> @params = new Dictionary<string, object>();

            foreach (KeyValuePair<string, object> p in extraParams) @params.Add(p.Key, p.Value);

            

            foreach (Enum value in Enum.GetValues(provider.GetType()))
            {
                if (provider.HasFlag(value))
                {
                    switch (value)
                    {
                        case AnalyticsProvider.FIREBASE:
                            LogFirebaseEvent(gameEventType.ToString().ToLower(), @params);

                            break;

                        case AnalyticsProvider.PLAYFAB:
                            LogPlayFabPlayerEvent(gameEventType.ToString().ToLower(), @params);

                            break;
                    }
                }
            }
        }

        public void LogSettingsChange(AnalyticsSettingsKeys settingsKey, string newValue, string oldValue, AnalyticsProvider provider = AnalyticsProvider.ALL)
        {
            //not used

            //if (!NetworkPass()) return;
            //if (!miscEventsSwitch[AnalyticsEvents.SETTINGS_CHANGE]) return;

            //Dictionary<string, object> @params = new Dictionary<string, object>();

            //@params.Add(SETTINGS_VALUE_NAME_KEY, settingsKey);
            //@params.Add(SETTINGS_NEW_VALUE_KEY, newValue);
            //@params.Add(SETTINGS_OLD_VALUE_KEY, oldValue);

            //foreach (Enum value in Enum.GetValues(provider.GetType()))
            //{
            //    if (provider.HasFlag(value))
            //    {
            //        switch (value)
            //        {
            //            case AnalyticsProvider.FIREBASE:
            //                LogFirebaseEvent(AnalyticsEvents.SETTINGS_CHANGE.ToString().ToLower(), @params);

            //                break;

            //            case AnalyticsProvider.PLAYFAB:
            //                LogPlayFabPlayerEvent(AnalyticsEvents.SETTINGS_CHANGE.ToString().ToLower(), @params);

            //                break;

            //            case AnalyticsProvider.GAME_ANALYTICS:
            //                string values = $"{settingsKey.ToString().ToLower()}:{oldValue}:{newValue}";

            //                LogGameAnalyticsDesignEvent(values);

            //                break;
            //        }
            //    }
            //}
        }

        public void LogError(string errorString, AnalyticsErrorType analyticsError = AnalyticsErrorType.unidentified, AnalyticsProvider provider = AnalyticsProvider.ALL)
        {
            //not used

            //if (!NetworkPass()) return;
            //if (!miscEventsSwitch[AnalyticsEvents.ERROR]) return;

            //Dictionary<string, object> @params = new Dictionary<string, object>();

            //@params.Add(analyticsError.ToString().ToLower(), errorString);

            //foreach (Enum value in Enum.GetValues(provider.GetType()))
            //{
            //    if (provider.HasFlag(value))
            //    {
            //        switch (value)
            //        {
            //            case AnalyticsProvider.FIREBASE:
            //                LogFirebaseEvent(AnalyticsEvents.ERROR.ToString().ToLower(), @params);

            //                break;

            //            case AnalyticsProvider.PLAYFAB:
            //                LogPlayFabPlayerEvent(AnalyticsEvents.SETTINGS_CHANGE.ToString().ToLower(), @params);

            //                break;

            //            case AnalyticsProvider.GAME_ANALYTICS:
            //                string values = $"{analyticsError.ToString().ToLower()}:{errorString.Truncate(15)}";

            //                GAErrorSeverity severity = GAErrorSeverity.Undefined;

            //                switch (analyticsError)
            //                {
            //                    case AnalyticsErrorType.realtime:
            //                    case AnalyticsErrorType.turn_based:
            //                    case AnalyticsErrorType.puzzle_play:
            //                    case AnalyticsErrorType.pass_play:
            //                    case AnalyticsErrorType.challenge_manager:
            //                    case AnalyticsErrorType.create_realtime_game:
            //                    case AnalyticsErrorType.create_turn_base_game:
            //                        severity = GAErrorSeverity.Critical;

            //                        break;

            //                    case AnalyticsErrorType.gameplay_scene:
            //                    case AnalyticsErrorType.main_menu:
            //                        severity = GAErrorSeverity.Error;

            //                        break;

            //                    case AnalyticsErrorType.onboarding:
            //                    case AnalyticsErrorType.settings:
            //                        severity = GAErrorSeverity.Warning;

            //                        break;
            //                }

            //                LogGameAnalyticsErrorEvent(severity, errorString);

            //                break;
            //        }
            //    }
            //}
        }

        public void LogTutorialEvent(string name, string stage, AnalyticsProvider provider = AnalyticsProvider.ALL)
        {
            if (!NetworkPass()) return;
            if (!miscEventsSwitch[AnalyticsEvents.TUTORIALS]) return;

            Dictionary<string, object> _params = new Dictionary<string, object>();

            _params.Add(TUTORIAL_NAME_KEY, name);
            _params.Add(TUTORIAL_STAGE_KEY, stage);

            foreach (Enum value in Enum.GetValues(provider.GetType()))
                if (provider.HasFlag(value))
                {
                    switch (value)
                    {
                        case AnalyticsProvider.FIREBASE:
                            LogFirebaseEvent(AnalyticsEvents.TUTORIALS.ToString().ToLower(), _params);

                            break;

                        case AnalyticsProvider.PLAYFAB:
                            LogPlayFabPlayerEvent(AnalyticsEvents.TUTORIALS.ToString().ToLower(), _params);

                            break;

                        case AnalyticsProvider.GAME_ANALYTICS:
                            string values = $"{AnalyticsEvents.TUTORIALS.ToString().ToLower()}";

                            foreach (KeyValuePair<string, object> param in _params)
                            {
                                values += $":{param.Value}";
                            }

                            LogGameAnalyticsDesignEvent(values);

                            break;
                    }
                }
        }

        public void LogUIButton(AnalyticsUIButtons buttonEvent, AnalyticsProvider provider = AnalyticsProvider.ALL, params KeyValuePair<string, object>[] @params)
        {
            //not used

            //if (!NetworkPass()) return;
            //if (!miscEventsSwitch[AnalyticsEvents.UI]) return;

            //lastLoggedButton = lastLoggedButtonConsumable = buttonEvent;

            //Dictionary<string, object> _params = new Dictionary<string, object>();

            //foreach (KeyValuePair<string, object> param in @params)
            //{
            //    _params.Add(param.Key, param.Value);
            //}

            //foreach (Enum value in Enum.GetValues(provider.GetType()))
            //    if (provider.HasFlag(value))
            //    {
            //        switch (value)
            //        {
            //            case AnalyticsProvider.FIREBASE:
            //                LogFirebaseEvent(AnalyticsEvents.UI.ToString().ToLower() + "_" + buttonEvent.ToString().ToLower(), _params);

            //                break;

            //            case AnalyticsProvider.PLAYFAB:
            //                LogPlayFabPlayerEvent(AnalyticsEvents.UI.ToString().ToLower() + "_" + buttonEvent.ToString().ToLower(), _params);

            //                break;

            //            case AnalyticsProvider.GAME_ANALYTICS:
            //                string values = $"{AnalyticsEvents.UI.ToString().ToLower()}:{buttonEvent.ToString().ToLower()}";

            //                foreach (KeyValuePair<string, object> param in @params)
            //                {
            //                    values += $":{param.Value}";
            //                }

            //                LogGameAnalyticsDesignEvent(values);

            //                break;
            //        }
            //    }
        }

        //public static GameResultType ResultType(IClientFourzy game)
        //{
        //    if (game.IsWinner()) return GameResultType.Win;
        //    else if (game.)
        //}

        private void LogFirebaseEvent(string eventType, Dictionary<string, object> @params)
        {
            List<Parameter> firebaseParams = new List<Parameter>();

            if (@params != null)
            {
                foreach (KeyValuePair<string, object> _param in @params)
                {
                    if (_param.Key != null && _param.Value != null)
                    {
                        firebaseParams.Add(new Parameter(_param.Key, _param.Value.ToString().ToLower()));
                    }
                }
            }

            FirebaseAnalytics.LogEvent(eventType, firebaseParams.ToArray());

            if (DEBUG)
            {
                Debug.Log("Firebase event sent: " + eventType);
            }
        }

        private void LogPlayFabPlayerEvent(string eventName, Dictionary<string, object> @params)
        {
            PlayFabClientAPI.WritePlayerEvent(new WriteClientPlayerEventRequest
            {
                EventName = eventName,
                Body = @params
            }, null, null);

            if (DEBUG)
            {
                Debug.Log("Playfab event sent: " + eventName);
            }
        }

        private void LogGameAnalyticsDesignEvent(string value)
        {
            GameAnalytics.NewDesignEvent(value);

            if (DEBUG)
            {
                Debug.Log("Game Analytics event sent: " + value);
            }
        }

        private void LogGameAnalyticsErrorEvent(GAErrorSeverity severity, string value)
        {
            GameAnalytics.NewErrorEvent(severity, value);

            if (DEBUG)
            {
                Debug.Log("Game Analytics Error event sent: " + value);
            }
        }

        private bool NetworkPass() => GameManager.NetworkAccess;
    }
}
