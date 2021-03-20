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

        public const string EVENT_ID_KEY = "eventId";

        public const string GAME_RESULT_KEY = "result";
        public const string HINT_STORE_ITEMS_KEY = "hintStoreItems";
        public const string STORE_ITEM_KEY = "storeItemId";
        public const string GAMEPIECE_SELECT_KEY = "gamepieceSelect";
        public const string SETTINGS_VALUE_NAME_KEY = "valueName";
        public const string SETTINGS_NEW_VALUE_KEY = "newValue";
        public const string SETTINGS_OLD_VALUE_KEY = "oldValue";
        public const string HINT_USED_KEY = "hintUsed";
        public const string TUTORIAL_NAME_KEY = "name";
        public const string TUTORIAL_STAGE_KEY = "key";

        private Action playFabCachedEvents;

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
            none,
            versusGameStart,
            versusGameEnd,
            randomPuzzleStart,
            randomPuzzleEnd,
            gauntletLevelStart,
            gauntletLevelEnd,
            aiLevelStart,
            aiLevelEnd,
            bossAILevelStart,
            bossAILevelEnd,
            puzzleLevelStart,
            puzzleLevelEnd,
            hintButtonPressed,
            hintPurchase,
            eventOpened,
            eventComplete,
            selectGamepiece,
            realtimeBotGameStart,
            realtimeBotGameEnd,
            realtimeLobbyGameStart,
            realtimeLobbyGameEnd,
            realtimeQuickmatchStart,
            realtimeQuickmatchEnd,
        }

        public enum GameResultType
        {
            none,
            player1Win,
            player2Win,
            win,
            lose,
            draw,
            reset,
            skip,
            noPossibleMoves,
            abandoned,
            player1TimeExpired,
            player2TimeExpired,
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

            LogEvent("lobbyGameJoined", values, provider);
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
            LogEvent("tutorialStepComplete",
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

            @params.Add("boardId", game.BoardID);
            @params.Add("area", game._Area);

            switch (eventType)
            {
                case AnalyticsEvents.versusGameEnd:
                    @params.Add("turnsCount", game._allTurnRecord.Count);

                    break;

                case AnalyticsEvents.randomPuzzleStart:
                    @params.Add("sessionId", GameManager.Instance.sessionId);
                    @params.Add("puzzleId", game.puzzleData.ID);

                    break;

                case AnalyticsEvents.randomPuzzleEnd:
                    @params.Add("sessionId", GameManager.Instance.sessionId);
                    @params.Add("puzzleId", game.puzzleData.ID);
                    @params.Add("turnsCount", game._playerTurnRecord.Count);
                    @params.Add("turnsLimit", game.puzzleData.MoveLimit);

                    break;

                case AnalyticsEvents.gauntletLevelStart:
                    @params.Add("gauntletId", game.puzzleData.pack.packID);
                    @params.Add("levelIndex", game.puzzleData.puzzleIndex);
                    @params.Add("currentNumPieces", game.myMembers.Count);
                    @params.Add("currentMagicQty", game.me.Magic);
                    @params.Add("aiProfileId", game.puzzleData.aiProfile);
                    @params.Add("boardJson", JsonConvert.SerializeObject(game.puzzleData.gameBoardDefinition));

                    break;

                case AnalyticsEvents.gauntletLevelEnd:
                    @params.Add("gauntletId", game.puzzleData.pack.packID);
                    @params.Add("levelIndex", game.puzzleData.puzzleIndex);
                    @params.Add("currentNumPieces", game.myMembers.Count);
                    @params.Add("currentMagicQty", game.me.Magic);
                    @params.Add("aiProfileId", game.puzzleData.aiProfile);
                    @params.Add("boardJson", JsonConvert.SerializeObject(game.puzzleData.gameBoardDefinition));
                    @params.Add("turnsCount", game._playerTurnRecord.Count);

                    break;

                case AnalyticsEvents.aiLevelStart:
                    @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);
                    @params.Add("levelId", game.puzzleData.ID);
                    @params.Add("levelIndex", game.puzzleData.puzzleIndex);
                    @params.Add("aiProfile", game.puzzleData.aiProfile);

                    break;

                case AnalyticsEvents.aiLevelEnd:
                    @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);
                    @params.Add("levelId", game.puzzleData.ID);
                    @params.Add("levelIndex", game.puzzleData.puzzleIndex);
                    @params.Add("aiProfileId", game.puzzleData.aiProfile);
                    @params.Add("turnsCount", game._playerTurnRecord.Count);

                    break;

                case AnalyticsEvents.bossAILevelStart:
                    @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);
                    @params.Add("levelId", game.puzzleData.ID);
                    @params.Add("levelIndex", game.puzzleData.puzzleIndex);
                    @params.Add("aiProfileId", game.puzzleData.aiProfile);
                    @params.Add("bossType", game.puzzleData.aiBoss);

                    break;

                case AnalyticsEvents.bossAILevelEnd:
                    @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);
                    @params.Add("levelId", game.puzzleData.ID);
                    @params.Add("levelIndex", game.puzzleData.puzzleIndex);
                    @params.Add("aiProfileId", game.puzzleData.aiProfile);
                    @params.Add("bossType", game.puzzleData.aiBoss);
                    @params.Add("turnsCount", game._playerTurnRecord.Count);
                    @params.Add("bossMovesCount", game.BossMoves);

                    break;

                case AnalyticsEvents.puzzleLevelStart:
                    @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);
                    @params.Add("levelId", game.puzzleData.ID);
                    @params.Add("levelIndex", game.puzzleData.puzzleIndex);

                    break;

                case AnalyticsEvents.puzzleLevelEnd:
                    @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);
                    @params.Add("levelId", game.puzzleData.ID);
                    @params.Add("levelIndex", game.puzzleData.puzzleIndex);
                    @params.Add("turnsCount", game._playerTurnRecord.Count);
                    @params.Add("turnsLimit", game.puzzleData.MoveLimit);

                    break;

                case AnalyticsEvents.hintButtonPressed:
                    //try add eventid
                    if (game.puzzleData.pack)
                    {
                        @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);
                    }

                    @params.Add("levelId", game.puzzleData.ID);
                    @params.Add("levelIndex", game.puzzleData.puzzleIndex);
                    @params.Add("isHintAvailable", UserManager.Instance.hints > 0);
                    @params.Add("turnsCount", game._playerTurnRecord.Count);
                    @params.Add("hintTurnNumber", PlayerPrefsWrapper.GetPuzzleHintProgress(game.BoardID));

                    break;

                case AnalyticsEvents.hintPurchase:
                    //try add eventid
                    if (game.puzzleData.pack) @params.Add(EVENT_ID_KEY, game.puzzleData.pack.packID);

                    @params.Add("levelId", game.puzzleData.ID);
                    @params.Add("levelIndex", game.puzzleData.puzzleIndex);
                    @params.Add("turnsCount", game._playerTurnRecord.Count);
                    @params.Add("hintTurnNumber", PlayerPrefsWrapper.GetPuzzleHintProgress(game.BoardID));

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

        public void FlushPlayfabEvents()
        {
            if (playFabCachedEvents != null)
            {
                playFabCachedEvents.Invoke();
                playFabCachedEvents = null;
            }
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
                            if (PlayFabClientAPI.IsClientLoggedIn())
                            {
                                PlayFabClientAPI.WritePlayerEvent(new WriteClientPlayerEventRequest
                                {
                                    EventName = @event,
                                    Body = values
                                }, null, null);
                            }
                            else
                            {
                                playFabCachedEvents += () => LogEvent(@event, values, AnalyticsProvider.PLAYFAB);
                            }

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
