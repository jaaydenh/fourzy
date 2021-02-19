//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Managers;
using Fourzy._Updates.Tools;
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
        /// <summary>
        /// Log events instead of sending them
        /// </summary>
        public static bool DEBUG = false;

#if UNITY_EDITOR
        private const string AMP_API_KEY = "4c62628ff8687c70a9fd201aea80db00";
#else
        private const string AMP_API_KEY = "300f3bfc4f1180cf072c49fcd198950f";
#endif

        public const string BOARD_ID_KEY = "board_id";
        public const string LEVEL_ID = "level_id";
        public const string EVENT_ID_KEY = "event_id";
        public const string GAUNTLET_ID_KEY = "gauntlet_id";
        public const string SESSION_ID_KEY = "session_id";

        public const string AREA_KEY = "area";
        public const string GAME_RESULT_KEY = "game_result";
        public const string ALL_TURNS_COUNT_KEY = "all_turns_count";
        public const string PLAYER_TURNS_COUNT_KEY = "player_turns_count";
        public const string PUZZLE_TURNS_LIMIT_KEY = "puzzle_turns_limit";
        public const string LEVEL_INDEX_KEY = "level_index";
        public const string GAUNTLET_MEMBERS_LEFT_KEY = "members_left";
        public const string MAGIC_LEFT_KEY = "magic_left";
        public const string GAME_AI_PROFILE_KEY = "ai_profile";
        public const string GAME_BOSS_TYPE_KEY = "boss_type";
        public const string BOARD_JSON_KEY = "board_json";
        public const string BOSS_MOVES_COUNT = "boss_moves_count";
        public const string HINT_AVAILABLE_KEY = "hint_available";
        public const string HINT_NUMBER_KEY = "hint_turn_number";
        public const string HINT_STORE_ITEMS_KEY = "hint_store_items";
        public const string STORE_ITEM_KEY = "store_item_id";
        public const string GAMEPIECE_SELECT_KEY = "gamepiece_select";
        public const string SETTINGS_VALUE_NAME_KEY = "value_name";
        public const string SETTINGS_NEW_VALUE_KEY = "new_value";
        public const string SETTINGS_OLD_VALUE_KEY = "old_value";
        public const string HINT_USED_KEY = "hint_used";
        public const string TUTORIAL_NAME_KEY = "name";
        public const string TUTORIAL_STAGE_KEY = "key";

        public static AnalyticsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    Initialize(true);
                }

                return instance;
            }
        }

        private static AnalyticsManager instance;

        public enum AnalyticsProvider
        {
            AMPLITUDE = 1,
            PLAYFAB = 2,
            UNITY_ANALYTICS = 4,

            ALL = AMPLITUDE | PLAYFAB | UNITY_ANALYTICS,
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

            Amplitude amplitude = Amplitude.getInstance("your project");
            amplitude.logging = true;
            amplitude.trackSessionEvents(true);
            amplitude.useAdvertisingIdForDeviceId();
            amplitude.init(AMP_API_KEY);
        }

        public static void Initialize(bool _DEBUG = false)
        {
            if (instance != null) return;

            GameObject go = new GameObject("AnalyticsManager");
            go.transform.SetParent(null);
            instance = go.AddComponent<AnalyticsManager>();

            DEBUG = _DEBUG;

            DontDestroyOnLoad(go);

            if (DEBUG)
            {
                Debug.Log("Analytics manager initialized.");
            }
        }

        public void LogGame(
            AnalyticsGameEvents gameEventType,
            IClientFourzy game,
            AnalyticsProvider provider = AnalyticsProvider.ALL,
            params KeyValuePair<string, object>[] extraParams)
        {
            Dictionary<string, object> @params = new Dictionary<string, object>();

            foreach (KeyValuePair<string, object> p in extraParams) @params.Add(p.Key, p.Value);

            @params.Add(BOARD_ID_KEY, game.BoardID);
            @params.Add(AREA_KEY, game._Area);

            switch (gameEventType)
            {
                case AnalyticsGameEvents.VERSUS_GAME_END:
                    @params.Add(ALL_TURNS_COUNT_KEY, game._allTurnRecord.Count);

                    break;

                case AnalyticsGameEvents.RANDOM_PUZZLE_START:
                    @params.Add(SESSION_ID_KEY, GameManager.Instance.sessionID);
                    @params.Add(LEVEL_ID, game.puzzleData.ID);

                    break;

                case AnalyticsGameEvents.RANDOM_PUZZLE_END:
                    @params.Add(SESSION_ID_KEY, GameManager.Instance.sessionID);
                    @params.Add(LEVEL_ID, game.puzzleData.ID);
                    @params.Add(PLAYER_TURNS_COUNT_KEY, game._playerTurnRecord.Count);
                    @params.Add(PUZZLE_TURNS_LIMIT_KEY, game.puzzleData.MoveLimit);

                    break;

                case AnalyticsGameEvents.GAUNTLET_LEVEL_START:
                    @params.Add(GAUNTLET_ID_KEY, game.puzzleData.pack.packID);
                    @params.Add(LEVEL_INDEX_KEY, game.puzzleData.puzzleIndex);
                    @params.Add(GAUNTLET_MEMBERS_LEFT_KEY, game.myMembers.Count);
                    @params.Add(MAGIC_LEFT_KEY, game.me.Magic);
                    @params.Add(GAME_AI_PROFILE_KEY, game.puzzleData.aiProfile);
                    @params.Add(BOARD_JSON_KEY, JsonConvert.SerializeObject(game.puzzleData.gameBoardDefinition));

                    break;

                case AnalyticsGameEvents.GAUNTLET_LEVEL_END:
                    @params.Add(GAUNTLET_ID_KEY, game.puzzleData.pack.packID);
                    @params.Add(LEVEL_INDEX_KEY, game.puzzleData.puzzleIndex);
                    @params.Add(GAUNTLET_MEMBERS_LEFT_KEY, game.myMembers.Count);
                    @params.Add(MAGIC_LEFT_KEY, game.me.Magic);
                    @params.Add(GAME_AI_PROFILE_KEY, game.puzzleData.aiProfile);
                    @params.Add(BOARD_JSON_KEY, JsonConvert.SerializeObject(game.puzzleData.gameBoardDefinition));
                    @params.Add(PLAYER_TURNS_COUNT_KEY, game._playerTurnRecord.Count);

                    break;

                case AnalyticsGameEvents.AI_LEVEL_START:
                    @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);
                    @params.Add(LEVEL_ID, game.puzzleData.ID);
                    @params.Add(LEVEL_INDEX_KEY, game.puzzleData.puzzleIndex);
                    @params.Add(GAME_AI_PROFILE_KEY, game.puzzleData.aiProfile);

                    break;

                case AnalyticsGameEvents.AI_LEVEL_END:
                    @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);
                    @params.Add(LEVEL_ID, game.puzzleData.ID);
                    @params.Add(LEVEL_INDEX_KEY, game.puzzleData.puzzleIndex);
                    @params.Add(GAME_AI_PROFILE_KEY, game.puzzleData.aiProfile);
                    @params.Add(PLAYER_TURNS_COUNT_KEY, game._playerTurnRecord.Count);

                    break;

                case AnalyticsGameEvents.BOSS_AI_LEVEL_START:
                    @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);
                    @params.Add(LEVEL_ID, game.puzzleData.ID);
                    @params.Add(LEVEL_INDEX_KEY, game.puzzleData.puzzleIndex);
                    @params.Add(GAME_AI_PROFILE_KEY, game.puzzleData.aiProfile);
                    @params.Add(GAME_BOSS_TYPE_KEY, game.puzzleData.aiBoss);

                    break;

                case AnalyticsGameEvents.BOSS_AI_LEVEL_END:
                    @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);
                    @params.Add(LEVEL_ID, game.puzzleData.ID);
                    @params.Add(LEVEL_INDEX_KEY, game.puzzleData.puzzleIndex);
                    @params.Add(GAME_AI_PROFILE_KEY, game.puzzleData.aiProfile);
                    @params.Add(GAME_BOSS_TYPE_KEY, game.puzzleData.aiBoss);
                    @params.Add(PLAYER_TURNS_COUNT_KEY, game._playerTurnRecord.Count);
                    @params.Add(BOSS_MOVES_COUNT, game.BossMoves);

                    break;

                case AnalyticsGameEvents.PUZZLE_LEVEL_START:
                    @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);
                    @params.Add(LEVEL_ID, game.puzzleData.ID);
                    @params.Add(LEVEL_INDEX_KEY, game.puzzleData.puzzleIndex);

                    break;

                case AnalyticsGameEvents.PUZZLE_LEVEL_END:
                    @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);
                    @params.Add(LEVEL_ID, game.puzzleData.ID);
                    @params.Add(LEVEL_INDEX_KEY, game.puzzleData.puzzleIndex);
                    @params.Add(PLAYER_TURNS_COUNT_KEY, game._playerTurnRecord.Count);
                    @params.Add(PUZZLE_TURNS_LIMIT_KEY, game.puzzleData.MoveLimit);

                    break;

                case AnalyticsGameEvents.PUZZLE_LEVEL_HINT_BUTTON_PRESS:
                    //try add eventid
                    if (game.puzzleData.pack) @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);

                    @params.Add(LEVEL_ID, game.puzzleData.ID);
                    @params.Add(LEVEL_INDEX_KEY, game.puzzleData.puzzleIndex);
                    @params.Add(HINT_AVAILABLE_KEY, UserManager.Instance.hints > 0);
                    @params.Add(PLAYER_TURNS_COUNT_KEY, game._playerTurnRecord.Count);
                    @params.Add(HINT_NUMBER_KEY, PlayerPrefsWrapper.GetPuzzleHintProgress(game.BoardID));

                    break;

                case AnalyticsGameEvents.PUZZLE_HINT_STORE_HINT_PURCHASE:
                    //try add eventid
                    if (game.puzzleData.pack) @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);

                    @params.Add(LEVEL_ID, game.puzzleData.ID);
                    @params.Add(LEVEL_INDEX_KEY, game.puzzleData.puzzleIndex);
                    @params.Add(PLAYER_TURNS_COUNT_KEY, game._playerTurnRecord.Count);
                    @params.Add(HINT_NUMBER_KEY, PlayerPrefsWrapper.GetPuzzleHintProgress(game.BoardID));

                    break;
            }

            foreach (Enum value in Enum.GetValues(provider.GetType()))
            {
                if (provider.HasFlag(value))
                {
                    switch (value)
                    {

                        case AnalyticsProvider.PLAYFAB:
                            LogPlayFabPlayerEvent(gameEventType.ToString().ToLower(), @params);

                            break;

                    }
                }
            }
        }
        public void LogEvent(
            AnalyticsGameEvents gameEventType,
            AnalyticsProvider provider = AnalyticsProvider.ALL,
            params KeyValuePair<string, object>[] extraParams)
        {
            Dictionary<string, object> @params = new Dictionary<string, object>();

            foreach (KeyValuePair<string, object> p in extraParams) @params.Add(p.Key, p.Value);



            foreach (Enum value in Enum.GetValues(provider.GetType()))
            {
                if (provider.HasFlag(value))
                {
                    switch (value)
                    {

                        case AnalyticsProvider.PLAYFAB:
                            LogPlayFabPlayerEvent(gameEventType.ToString().ToLower(), @params);

                            break;
                    }
                }
            }
        }

        public void LogSettingsChange(
            string settingsKey, 
            string newValue, 
            string oldValue, 
            AnalyticsProvider provider = AnalyticsProvider.ALL)
        {

        }

        public void LogError(string errorString, AnalyticsProvider provider = AnalyticsProvider.ALL)
        {

        }

        public void LogTutorialEvent(string name, string stage, AnalyticsProvider provider = AnalyticsProvider.ALL)
        {

        }

        private void LogPlayFabPlayerEvent(string eventName, Dictionary<string, object> @params)
        {
            PlayFabClientAPI.WritePlayerEvent(new WriteClientPlayerEventRequest
            {
                EventName = eventName,
                Body = @params
            }, null, null);

            if (DEBUG) Debug.Log("Playfab event sent: " + eventName);
        }
    }
}
