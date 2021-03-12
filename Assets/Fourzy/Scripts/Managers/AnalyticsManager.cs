//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Managers;
using Fourzy._Updates.Tools;
using FourzyGameModel.Model;
using Newtonsoft.Json;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;

namespace Fourzy
{
    public class AnalyticsManager : RoutinesBase
    {
        private const string AMP_PROD_KEY = "300f3bfc4f1180cf072c49fcd198950f";
        private const string AMP_DEBUG_KEY = "4c62628ff8687c70a9fd201aea80db00";

        public const string BOARD_ID_KEY = "boardId";
        public const string LEVEL_ID = "levelId";
        public const string EVENT_ID_KEY = "eventId";
        public const string GAUNTLET_ID_KEY = "gauntletId";
        public const string SESSION_ID_KEY = "sessionId";

        public const string AREA_KEY = "area";
        public const string GAME_RESULT_KEY = "result";
        public const string NUM_TURNS_TAKEN_KEY = "numTurnsTaken";
        public const string PLAYER_TURNS_COUNT_KEY = "playerTurnsCount";
        public const string PUZZLE_TURNS_LIMIT_KEY = "puzzleTurnsLimit";
        public const string LEVEL_INDEX_KEY = "levelIndex";
        public const string GAUNTLET_MEMBERS_LEFT_KEY = "piecesRemaining";
        public const string MAGIC_LEFT_KEY = "magicRemaining";
        public const string GAME_AI_PROFILE_KEY = "botProfile";
        public const string GAME_BOSS_TYPE_KEY = "bossType";
        public const string BOARD_JSON_KEY = "boardJson";
        public const string BOSS_MOVES_COUNT = "bossMovesCount";
        public const string HINT_AVAILABLE_KEY = "isHintAvailable";
        public const string HINT_NUMBER_KEY = "hintTurnNumber";
        public const string HINT_STORE_ITEMS_KEY = "hintStoreItems";
        public const string STORE_ITEM_KEY = "storeItemId";
        public const string GAMEPIECE_SELECT_KEY = "gamepieceSelect";
        public const string SETTINGS_VALUE_NAME_KEY = "valueName";
        public const string SETTINGS_NEW_VALUE_KEY = "newValue";
        public const string SETTINGS_OLD_VALUE_KEY = "oldValue";
        public const string HINT_USED_KEY = "hintUsed";
        public const string TUTORIAL_NAME_KEY = "name";
        public const string TUTORIAL_STAGE_KEY = "key";

        public static AnalyticsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    Initialize();
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

        public enum AnalyticsEvents
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

            string apiKey = AMP_PROD_KEY;

            if (Debug.isDebugBuild || Application.isEditor)
            {
                apiKey = AMP_DEBUG_KEY;
            }

            Amplitude amplitude = Amplitude.getInstance();
            amplitude.logging = true;
            amplitude.trackSessionEvents(true);
            amplitude.useAdvertisingIdForDeviceId();
            amplitude.init(apiKey);
        }

        protected void Start()
        {
            PlayerPrefsWrapper.AddAppOpened();

            int timesOpened = PlayerPrefsWrapper.GetAppOpened();
            if (timesOpened == 2)
            {
               Amplitude.Instance.setUserProperty("first", false);
            }
            Amplitude.Instance.setUserProperty("totalSessions", timesOpened);
        }

        protected void OnApplicationQuit()
        {
            Amplitude.Instance.setUserProperty("lastSeenDate",
               (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
        }

        public static void Initialize()
        {
            if (instance != null) return;

            GameObject go = new GameObject("AnalyticsManager");
            go.transform.SetParent(null);
            instance = go.AddComponent<AnalyticsManager>();

            DontDestroyOnLoad(go);
        }
        
        public static void SetUsetID(string userId)
        {
            Analytics.SetUserId(userId);
            Amplitude.Instance.setUserId(userId);
        }

        public void LogOtherJoinedLobby(
            string playerId, 
            float timePassed, 
            AnalyticsProvider provider = AnalyticsProvider.ALL)
        {
            Dictionary<string, object> values = new Dictionary<string, object>()
            {
                ["playerId"] = playerId,
                ["creatorPlayerId"] = LoginManager.playfabId,
                ["timer"] = SettingsManager.Get(SettingsManager.KEY_REALTIME_TIMER),
                ["area"] = ((Area)PlayerPrefsWrapper.GetCurrentArea()).ToString(),
                ["isMagicEnabled"] = SettingsManager.Get(SettingsManager.KEY_REALTIME_MAGIC),
                ["isPrivate"] = !string.IsNullOrEmpty(FourzyPhotonManager.PASSWORD),
                ["complexityScore"] = "",
                ["creatorPlayerId"] = LoginManager.playfabId,
                ["timeSinceGameCreated"] = timePassed,
            };

            LogEvent("LOBBY_GAME_JOINED", values, provider);
        }

        public void LogRealtimeGameCompleted(
            string area, 
            string result,
            int turnsCount,
            string complexityScore,
            float winnerTimerLeft,
            float opponentTimerLeft,
            bool magicEnabled,
            bool timerEnabled,
            string winnerId,
            string opponentId)
        {
            LogEvent(
                "REALTIME_GAME_COMPLETE",
                new Dictionary<string, object>()
                {
                    ["area"] = area,
                    ["result"] = result,
                    ["turnsTaken"] = turnsCount,
                    ["complexityScore"] = complexityScore,
                    ["winnerTimerLeft"] = winnerTimerLeft,
                    ["opponentTimerLeft"] = opponentTimerLeft,
                    ["isMagicEnabled"] = magicEnabled,
                    ["timer"] = timerEnabled,
                    ["winnerPlayerId"] = winnerId,
                    ["opponentPlyerId"] = opponentId,
                });
        }

        public void LogSettingsChange(
            string settingsKey,
            string newValue,
            string oldValue,
            AnalyticsProvider provider = AnalyticsProvider.ALL)
        {

        }

        public void LogTutorialEvent(
            string tutorialName, 
            string id, 
            AnalyticsProvider provider = AnalyticsProvider.ALL)
        {
            LogEvent("TUTORIAL_STEP_COMPLETE",
                new Dictionary<string, object>()
                {
                    ["id"] = id
                },
                provider);
        }

        public void LogGame(
            AnalyticsEvents eventType,
            IClientFourzy game,
            AnalyticsProvider provider = AnalyticsProvider.ALL,
            params KeyValuePair<string, object>[] extraParams)
        {
            Dictionary<string, object> @params = new Dictionary<string, object>();

            @params.Add(BOARD_ID_KEY, game.BoardID);
            @params.Add(AREA_KEY, game._Area);

            switch (eventType)
            {
                case AnalyticsEvents.VERSUS_GAME_END:
                    @params.Add(NUM_TURNS_TAKEN_KEY, game._allTurnRecord.Count);

                    break;

                case AnalyticsEvents.RANDOM_PUZZLE_START:
                    @params.Add(SESSION_ID_KEY, GameManager.Instance.sessionID);
                    @params.Add(LEVEL_ID, game.puzzleData.ID);

                    break;

                case AnalyticsEvents.RANDOM_PUZZLE_END:
                    @params.Add(SESSION_ID_KEY, GameManager.Instance.sessionID);
                    @params.Add(LEVEL_ID, game.puzzleData.ID);
                    @params.Add(PLAYER_TURNS_COUNT_KEY, game._playerTurnRecord.Count);
                    @params.Add(PUZZLE_TURNS_LIMIT_KEY, game.puzzleData.MoveLimit);

                    break;

                case AnalyticsEvents.GAUNTLET_LEVEL_START:
                    @params.Add(GAUNTLET_ID_KEY, game.puzzleData.pack.packID);
                    @params.Add(LEVEL_INDEX_KEY, game.puzzleData.puzzleIndex);
                    @params.Add(GAUNTLET_MEMBERS_LEFT_KEY, game.myMembers.Count);
                    @params.Add(MAGIC_LEFT_KEY, game.me.Magic);
                    @params.Add(GAME_AI_PROFILE_KEY, game.puzzleData.aiProfile);
                    @params.Add(BOARD_JSON_KEY, JsonConvert.SerializeObject(game.puzzleData.gameBoardDefinition));

                    break;

                case AnalyticsEvents.GAUNTLET_LEVEL_END:
                    @params.Add(GAUNTLET_ID_KEY, game.puzzleData.pack.packID);
                    @params.Add(LEVEL_INDEX_KEY, game.puzzleData.puzzleIndex);
                    @params.Add(GAUNTLET_MEMBERS_LEFT_KEY, game.myMembers.Count);
                    @params.Add(MAGIC_LEFT_KEY, game.me.Magic);
                    @params.Add(GAME_AI_PROFILE_KEY, game.puzzleData.aiProfile);
                    @params.Add(BOARD_JSON_KEY, JsonConvert.SerializeObject(game.puzzleData.gameBoardDefinition));
                    @params.Add(PLAYER_TURNS_COUNT_KEY, game._playerTurnRecord.Count);

                    break;

                case AnalyticsEvents.AI_LEVEL_START:
                    @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);
                    @params.Add(LEVEL_ID, game.puzzleData.ID);
                    @params.Add(LEVEL_INDEX_KEY, game.puzzleData.puzzleIndex);
                    @params.Add(GAME_AI_PROFILE_KEY, game.puzzleData.aiProfile);

                    break;

                case AnalyticsEvents.AI_LEVEL_END:
                    @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);
                    @params.Add(LEVEL_ID, game.puzzleData.ID);
                    @params.Add(LEVEL_INDEX_KEY, game.puzzleData.puzzleIndex);
                    @params.Add(GAME_AI_PROFILE_KEY, game.puzzleData.aiProfile);
                    @params.Add(PLAYER_TURNS_COUNT_KEY, game._playerTurnRecord.Count);

                    break;

                case AnalyticsEvents.BOSS_AI_LEVEL_START:
                    @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);
                    @params.Add(LEVEL_ID, game.puzzleData.ID);
                    @params.Add(LEVEL_INDEX_KEY, game.puzzleData.puzzleIndex);
                    @params.Add(GAME_AI_PROFILE_KEY, game.puzzleData.aiProfile);
                    @params.Add(GAME_BOSS_TYPE_KEY, game.puzzleData.aiBoss);

                    break;

                case AnalyticsEvents.BOSS_AI_LEVEL_END:
                    @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);
                    @params.Add(LEVEL_ID, game.puzzleData.ID);
                    @params.Add(LEVEL_INDEX_KEY, game.puzzleData.puzzleIndex);
                    @params.Add(GAME_AI_PROFILE_KEY, game.puzzleData.aiProfile);
                    @params.Add(GAME_BOSS_TYPE_KEY, game.puzzleData.aiBoss);
                    @params.Add(PLAYER_TURNS_COUNT_KEY, game._playerTurnRecord.Count);
                    @params.Add(BOSS_MOVES_COUNT, game.BossMoves);

                    break;

                case AnalyticsEvents.PUZZLE_LEVEL_START:
                    @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);
                    @params.Add(LEVEL_ID, game.puzzleData.ID);
                    @params.Add(LEVEL_INDEX_KEY, game.puzzleData.puzzleIndex);

                    break;

                case AnalyticsEvents.PUZZLE_LEVEL_END:
                    @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);
                    @params.Add(LEVEL_ID, game.puzzleData.ID);
                    @params.Add(LEVEL_INDEX_KEY, game.puzzleData.puzzleIndex);
                    @params.Add(PLAYER_TURNS_COUNT_KEY, game._playerTurnRecord.Count);
                    @params.Add(PUZZLE_TURNS_LIMIT_KEY, game.puzzleData.MoveLimit);

                    break;

                case AnalyticsEvents.PUZZLE_LEVEL_HINT_BUTTON_PRESS:
                    //try add eventid
                    if (game.puzzleData.pack) @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);

                    @params.Add(LEVEL_ID, game.puzzleData.ID);
                    @params.Add(LEVEL_INDEX_KEY, game.puzzleData.puzzleIndex);
                    @params.Add(HINT_AVAILABLE_KEY, UserManager.Instance.hints > 0);
                    @params.Add(PLAYER_TURNS_COUNT_KEY, game._playerTurnRecord.Count);
                    @params.Add(HINT_NUMBER_KEY, PlayerPrefsWrapper.GetPuzzleHintProgress(game.BoardID));

                    break;

                case AnalyticsEvents.PUZZLE_HINT_STORE_HINT_PURCHASE:
                    //try add eventid
                    if (game.puzzleData.pack) @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);

                    @params.Add(LEVEL_ID, game.puzzleData.ID);
                    @params.Add(LEVEL_INDEX_KEY, game.puzzleData.puzzleIndex);
                    @params.Add(PLAYER_TURNS_COUNT_KEY, game._playerTurnRecord.Count);
                    @params.Add(HINT_NUMBER_KEY, PlayerPrefsWrapper.GetPuzzleHintProgress(game.BoardID));

                    break;
            }

            foreach (KeyValuePair<string, object> p in extraParams)
            {
                @params.Add(p.Key, p.Value);
            }

            LogEvent(eventType, @params, provider);
        }

        public void LogEvent(
            AnalyticsEvents eventType,
            AnalyticsProvider provider = AnalyticsProvider.ALL,
            params KeyValuePair<string, object>[] values)
        {
            LogEvent(eventType, values.ToDictionary(_value => _value.Key, _value => _value.Value), provider);
        }

        public void LogEvent(
           AnalyticsEvents eventType,
           Dictionary<string, object> values,
           AnalyticsProvider provider = AnalyticsProvider.ALL)
        {
            LogEvent(eventType.ToString(), values, provider);
        }

        public void LogEvent(
           string eventType,
           AnalyticsProvider provider = AnalyticsProvider.ALL,
           params KeyValuePair<string, object>[] values)
        {
            LogEvent(eventType, values.ToDictionary(_value => _value.Key, _value => _value.Value), provider);
        }

        public void LogEvent(
           string @event,
           Dictionary<string, object> values,
           AnalyticsProvider provider = AnalyticsProvider.ALL)
        {
            foreach (Enum value in Enum.GetValues(provider.GetType()))
            {
                if (provider.HasFlag(value))
                {
                    switch (value)
                    {
                        case AnalyticsProvider.AMPLITUDE:
                            Amplitude.Instance.logEvent(@event, values);

                            break;

                        case AnalyticsProvider.PLAYFAB:
                            PlayFabClientAPI.WritePlayerEvent(new WriteClientPlayerEventRequest
                            {
                                EventName = @event,
                                Body = values
                            }, null, null);

                            break;

                        case AnalyticsProvider.UNITY_ANALYTICS:
                            Analytics.CustomEvent(@event, values);

                            break;

                    }
                }
            }
        }
    }
}
