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
        public const string GAMEPIECE_SELECT_KEY = "gamepieceId";
        public const string GAMEPIECE_NAME_KEY = "name";
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
            versusGameCreated,
            versusGameCompleted,
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
            realtimeLobbyGameStart,
            realtimeLobbyGameEnd,
            realtimeGameCreated,
            realtimeGameCompleted,
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

        protected void OnApplicationQuit()
        {
            Amplitude.Instance.setUserProperty("lastSeenDate", DateTime.Now.ToString());
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
            params KeyValuePair<string, object>[] values)
        {
            LogGame(eventType, game, values.ToDictionary(_value => _value.Key, _value => _value.Value), provider);
        }

        public void LogGame(
            AnalyticsEvents eventType,
            IClientFourzy game,
            Dictionary<string, object> _params,
            AnalyticsProvider provider = AnalyticsProvider.ALL)
        {
            Dictionary<string, object> _values = new Dictionary<string, object>();

            _values.Add("boardId", game.BoardID);
            _values.Add("area", game._Area);

            string player1Id;
            string player2Id;
            int player1Rating;
            int player2Rating;
            bool isBotOpponent = GameManager.Instance.ExpectedGameType == GameTypeLocal.REALTIME_BOT_GAME;

            switch (eventType)
            {
                case AnalyticsEvents.randomPuzzleStart:
                    _values.Add("sessionId", GameManager.Instance.sessionId);
                    _values.Add("puzzleId", game.puzzleData.ID);

                    break;

                case AnalyticsEvents.randomPuzzleEnd:
                    _values.Add("sessionId", GameManager.Instance.sessionId);
                    _values.Add("puzzleId", game.puzzleData.ID);
                    _values.Add("turnsCount", game._playerTurnRecord.Count);
                    _values.Add("turnsLimit", game.puzzleData.MoveLimit);

                    break;

                case AnalyticsEvents.gauntletLevelStart:
                    _values.Add("gauntletId", game.puzzleData.pack.packId);
                    _values.Add("levelIndex", game.puzzleData.puzzleIndex);
                    _values.Add("currentNumPieces", game.myMembers.Count);
                    _values.Add("currentMagicQty", game.me.Magic);
                    _values.Add("aiProfileId", game.puzzleData.aiProfile);
                    _values.Add("boardJson", JsonConvert.SerializeObject(game.puzzleData.gameBoardDefinition));

                    break;

                case AnalyticsEvents.gauntletLevelEnd:
                    _values.Add("gauntletId", game.puzzleData.pack.packId);
                    _values.Add("levelIndex", game.puzzleData.puzzleIndex);
                    _values.Add("currentNumPieces", game.myMembers.Count);
                    _values.Add("currentMagicQty", game.me.Magic);
                    _values.Add("aiProfileId", game.puzzleData.aiProfile);
                    _values.Add("boardJson", JsonConvert.SerializeObject(game.puzzleData.gameBoardDefinition));
                    _values.Add("turnsCount", game._playerTurnRecord.Count);

                    break;

                case AnalyticsEvents.aiLevelStart:
                    _values.Add(EVENT_ID_KEY, game.puzzleData.pack.packId);
                    _values.Add("levelId", game.puzzleData.ID);
                    _values.Add("levelIndex", game.puzzleData.puzzleIndex);
                    _values.Add("aiProfileId", game.puzzleData.aiProfile);

                    break;

                case AnalyticsEvents.aiLevelEnd:
                    _values.Add(EVENT_ID_KEY, game.puzzleData.pack.packId);
                    _values.Add("levelId", game.puzzleData.ID);
                    _values.Add("levelIndex", game.puzzleData.puzzleIndex);
                    _values.Add("aiProfileId", game.puzzleData.aiProfile);
                    _values.Add("turnsCount", game._playerTurnRecord.Count);
                    _values.Add(
                        "isAlreadySolved",
                        PlayerPrefsWrapper.GetPuzzleChallengeComplete(game.puzzleData.ID));

                    break;

                case AnalyticsEvents.bossAILevelStart:
                    _values.Add(EVENT_ID_KEY, game.puzzleData.pack.packId);
                    _values.Add("levelId", game.puzzleData.ID);
                    _values.Add("levelIndex", game.puzzleData.puzzleIndex);
                    _values.Add("aiProfileId", game.puzzleData.aiProfile);
                    _values.Add("bossType", game.puzzleData.aiBoss);

                    break;

                case AnalyticsEvents.bossAILevelEnd:
                    _values.Add(EVENT_ID_KEY, game.puzzleData.pack.packId);
                    _values.Add("levelId", game.puzzleData.ID);
                    _values.Add("levelIndex", game.puzzleData.puzzleIndex);
                    _values.Add("aiProfileId", game.puzzleData.aiProfile);
                    _values.Add("bossType", game.puzzleData.aiBoss);
                    _values.Add("turnsCount", game._playerTurnRecord.Count);
                    _values.Add("bossMovesCount", game.BossMoves);
                    _values.Add(
                        "isAlreadySolved",
                        PlayerPrefsWrapper.GetPuzzleChallengeComplete(game.puzzleData.ID));

                    break;

                case AnalyticsEvents.puzzleLevelStart:
                    _values.Add(EVENT_ID_KEY, game.puzzleData.pack.packId);
                    _values.Add("levelId", game.puzzleData.ID);
                    _values.Add("levelIndex", game.puzzleData.puzzleIndex);

                    break;

                case AnalyticsEvents.puzzleLevelEnd:
                    _values.Add(EVENT_ID_KEY, game.puzzleData.pack.packId);
                    _values.Add("levelId", game.puzzleData.ID);
                    _values.Add("levelIndex", game.puzzleData.puzzleIndex);
                    _values.Add("turnsCount", game._playerTurnRecord.Count);
                    _values.Add("turnsLimit", game.puzzleData.MoveLimit);
                    _values.Add(
                        "isAlreadySolved",
                        PlayerPrefsWrapper.GetPuzzleChallengeComplete(game.puzzleData.ID));

                    break;

                case AnalyticsEvents.hintButtonPressed:
                    //try add eventid
                    if (game.puzzleData.pack)
                    {
                        _values.Add(EVENT_ID_KEY, game.puzzleData.pack.packId);
                    }

                    _values.Add("levelId", game.puzzleData.ID);
                    _values.Add("levelIndex", game.puzzleData.puzzleIndex);
                    _values.Add("isHintAvailable", UserManager.Instance.hints > 0);
                    _values.Add("turnsCount", game._playerTurnRecord.Count);
                    _values.Add("hintTurnNumber", PlayerPrefsWrapper.GetPuzzleHintProgress(game.BoardID));

                    break;

                case AnalyticsEvents.hintPurchase:
                    //try add eventid
                    if (game.puzzleData.pack) _values.Add(EVENT_ID_KEY, game.puzzleData.pack.packId);

                    _values.Add("levelId", game.puzzleData.ID);
                    _values.Add("levelIndex", game.puzzleData.puzzleIndex);
                    _values.Add("turnsCount", game._playerTurnRecord.Count);
                    _values.Add("hintTurnNumber", PlayerPrefsWrapper.GetPuzzleHintProgress(game.BoardID));

                    break;

                case AnalyticsEvents.versusGameCreated:
                    _values.Add("player1", game.player1.Profile.ToString());
                    _values.Add("player2", game.player2.Profile.ToString());
                    _values.Add("isTimerEnabled", SettingsManager.Get(SettingsManager.KEY_LOCAL_TIMER));
                    _values.Add("isMagicEnabled", SettingsManager.Get(SettingsManager.KEY_REALTIME_MAGIC));

                    break;

                case AnalyticsEvents.versusGameCompleted:
                    _values.Add("player1", game.player1.Profile.ToString());
                    _values.Add("player2", game.player2.Profile.ToString());
                    _values.Add("isTimerEnabled", SettingsManager.Get(SettingsManager.KEY_LOCAL_TIMER));
                    _values.Add("isMagicEnabled", SettingsManager.Get(SettingsManager.KEY_REALTIME_MAGIC));
                    _values.Add("turnsCount", game._allTurnRecord.Count);

                    break;

                case AnalyticsEvents.realtimeGameCreated:
                    _values.Add("complexityScore", "");
                    _values.Add("isTimerEnabled", SettingsManager.Get(SettingsManager.KEY_REALTIME_TIMER));
                    _values.Add("isMagicEnabled", SettingsManager.Get(SettingsManager.KEY_REALTIME_MAGIC));

                    (player1Id, player2Id) = GetPlayersIds();
                    (player1Rating, player2Rating) = GetPlayersRatings();

                    _values.Add("player1PlayerId", player1Id);
                    _values.Add("player2PlayerId", player2Id);
                    _values.Add("player1Rating", player1Rating);
                    _values.Add("player2Rating", player2Rating);
                    _values.Add("isBotOpponent", isBotOpponent);
                    _values.Add("recipe", game._FirstState.Board.Recipe);
                    _values.Add("seed", game._FirstState.CompressedString);

                    break;

                case AnalyticsEvents.realtimeGameCompleted:
                    (player1Id, player2Id) = GetPlayersIds();
                    (player1Rating, player2Rating) = GetPlayersRatings();

                    _values.Add("turnsCount", game._allTurnRecord.Count);
                    _values.Add("complexityScore", "");
                    _values.Add("isTimerEnabled", SettingsManager.Get(SettingsManager.KEY_REALTIME_TIMER));
                    _values.Add("isMagicEnabled", SettingsManager.Get(SettingsManager.KEY_REALTIME_MAGIC));

                    _values.Add("player1PlayerId", player1Id);
                    _values.Add("player2PlayerId", player2Id);
                    _values.Add("player1Rating", player1Rating);
                    _values.Add("player2Rating", player2Rating);
                    _values.Add("isBotOpponent", isBotOpponent);
                    _values.Add("recipe", game._FirstState.Board.Recipe);
                    _values.Add("seed", game._FirstState.CompressedString);

                    break;
            }

            foreach (var p in _params)
            {
                _values.Add(p.Key, p.Value);
            }

            LogEvent(eventType, _values, provider);

            (string player1Id, string player2Id) GetPlayersIds()
            {
                bool isMePlayer1 = game.player1.PlayerString == game.me.PlayerString;
                string _player1Id = game.player1.Profile.ToString();
                string _player2Id = game.player2.Profile.ToString();

                switch (GameManager.Instance.ExpectedGameType)
                {
                    case GameTypeLocal.REALTIME_BOT_GAME:
                        if (isMePlayer1)
                        {
                            _player1Id = LoginManager.masterAccountId;
                        }
                        else
                        {
                            _player2Id = LoginManager.masterAccountId;
                        }

                        break;

                    case GameTypeLocal.REALTIME_LOBBY_GAME:
                    case GameTypeLocal.REALTIME_QUICKMATCH:
                        if (isMePlayer1)
                        {
                            _player1Id = LoginManager.masterAccountId;
                            _player2Id = GameManager.Instance.RealtimeOpponent.Id;
                        }
                        else
                        {
                            _player2Id = LoginManager.masterAccountId;
                            _player1Id = GameManager.Instance.RealtimeOpponent.Id;
                        }

                        break;
                }

                return (_player1Id, _player2Id);
            }

            (int player1Rating, int player2Rating) GetPlayersRatings()
            {
                bool isMePlayer1 = game.player1 == game.me;

                player1Rating = isMePlayer1 ?
                    UserManager.Instance.lastCachedRating :
                    GameManager.Instance.RealtimeOpponent.Rating;
                player2Rating = isMePlayer1 ?
                    GameManager.Instance.RealtimeOpponent.Rating :
                    UserManager.Instance.lastCachedRating;

                return (player1Rating, player2Rating);
            }
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
                                }, null, 
                                error =>
                                {
                                    GameManager.Instance.ReportPlayFabError(error.ErrorMessage);
                                });
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
