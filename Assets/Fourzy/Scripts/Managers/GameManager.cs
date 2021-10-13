//@vadym udod

using ExitGames.Client.Photon;
using Fourzy._Updates;
using Fourzy._Updates.Audio;
using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Managers;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Threading;
using Fourzy._Updates.Tools;
using Fourzy._Updates.UI.Camera3D;
using Fourzy._Updates.UI.Menu;
using Fourzy._Updates.UI.Menu.Screens;
using FourzyGameModel.Model;
using Hellmade.Net;
using MoreMountains.NiceVibrations;
using Newtonsoft.Json;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Purchasing;
using UnityEngine.SceneManagement;

namespace Fourzy
{
    public class GameManager : RoutinesBase
    {
        public static Action<Product> onPurchaseComplete;
        public static Action<Product> onPurchaseFailed;

        public static Action onNewsFetched;
        public static Action<bool> onNetworkAccess;
        public static Action<string> onSceneChanged;
        public static Action<string> onDailyChallengeFileName;
        public static Action<PlacementStyle> onPlacementStyle;
        public static Action<RatingGameCompleteResult> ratingDataReceived;

        public static GameManager Instance;

#if UNITY_STANDALONE || UNITY_EDITOR 
        public static bool HAS_POINTER = true;
#elif UNITY_IOS || UNITY_ANDROID
        public static bool HAS_POINTER = false;
#endif

        [Header("Display tutorials?")]
        public bool displayTutorials = true;
        [Header("Display tutorial even if it was already displayed")]
        public bool forceDisplayTutorials = true;
        [Tooltip("Options effected:\n   - Reset Puzzles\n   - Pass&Play rewards screen"), SerializeField]
        private bool extraFeatures = true;

        [Header("Pass And Play games only")]
        public bool tapToStartGame = true;
        public PassPlayCharactersType characterType = PassPlayCharactersType.SELECTED_RANDOM;

        [Header("Misc settings")]
        public bool showInfoToasts = true;
        public bool defaultGauntletState = true;
        public bool defaultPuzzlesState = true;
        public bool resetGameOnClose = true;
        public BotGameType botGameType = BotGameType.NONE;
        public BuildIntent buildIntent = BuildIntent.MOBILE_REGULAR;
        public string customUserId = "";
        public float fallbackLatitude = 37.7833f;
        public float fallbackLongitude = 122.4167f;
        public List<TokenType> excludeInstructionsFor;

        /// <summary>
        /// Pieces placement style
        /// </summary>
        public PlacementStyle placementStyle
        {
            get => _placementStyle;

            set
            {
                _placementStyle = value;
                PlayerPrefsWrapper.SetPlacementStyle((int)value);

                onPlacementStyle?.Invoke(_placementStyle);
            }
        }
        public bool Landscape => Screen.width > Screen.height;
        public GameTypeLocal ExpectedGameType { get; private set; }
        public PuzzleData dailyPuzzlePack { get; private set; }
        public IClientFourzy activeGame { get; set; }
        public BasicPuzzlePack currentPuzzlePack { get; set; }
        public Camera3dItemProgressionMap currentMap { get; set; }
        public List<TitleNewsItem> latestNews { get; private set; } = new List<TitleNewsItem>();
        public OpponentData RealtimeOpponent { get; set; }
        public bool RejoinAbandonedGame { get; set; }
        public Player Bot { get; set; }
        public string sessionId { get; private set; }
        public LocationInfo? lastLocation { get; private set; } = null;
        public string MainMenuSceneName
        {
            get
            {
                switch (buildIntent)
                {
                    case BuildIntent.MOBILE_SKILLZ:
                        return Constants.MAIN_MENU_SKILLZ_SCENE_NAME;

                    case BuildIntent.DESKTOP_REGULAR:
                        return Constants.MAIN_MENU_L_SCENE_NAME;

                    default:
                        return Constants.MAIN_MENU_P_SCENE_NAME;
                }
            }
        }

        private PlacementStyle _placementStyle;
        private string lastErrorMessage;

        public bool isMainMenuLoaded
        {
            get
            {
                bool mainMenuLoaded = false;
                for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
                {
                    if (SceneManager.GetSceneAt(sceneIndex).name == MainMenuSceneName)
                    {
                        mainMenuLoaded = true;
                        break;
                    }
                }

                return mainMenuLoaded;
            }
        }

        public bool isGameplaySceneLoaded
        {
            get
            {
                bool mainMenuLoaded = false;
                for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
                {
                    if (SceneManager.GetSceneAt(sceneIndex).name == Constants.GAMEPLAY_SCENE_NAME)
                    {
                        mainMenuLoaded = true;
                        break;
                    }
                }

                return mainMenuLoaded;
            }
        }

        public float latitude
        {
            get
            {
                if (lastLocation == null)
                {
                    return fallbackLatitude;
                }
                else
                {
                    return lastLocation.Value.latitude;
                }
            }
        }

        public float longitude
        {
            get
            {
                if (lastLocation == null)
                {
                    return fallbackLongitude;
                }
                else
                {
                    return lastLocation.Value.longitude;
                }
            }
        }

        public List<TitleNewsItem> unreadNews =>
            latestNews?.Where(titleNews => !PlayerPrefsWrapper.GetNewsOpened(titleNews.NewsId)).ToList() ??
            new List<TitleNewsItem>();

        protected override void Awake()
        {
            base.Awake();

            if (Instance) return;

            Instance = this;

            // #if UNITY_IOS
            //             MMVibrationManager.iOSInitializeHaptics();
            // #endif

            ExecutePerVersion.TryExecute();
            ThreadsQueuer.Initialize();
            AnalyticsManager.Initialize();

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            FourzyPhotonManager.onEvent += OnEventCall;
            Application.logMessageReceived += HandleException;

            EazyNetChecker.OnConnectionStatusChanged += OnNetStatusChanged;
            EazyNetChecker.OnCheckTimeout += OnNetStatusChanged;
            FourzyPhotonManager.onPlayerEnteredRoom += OnPlayerEntered;
            FourzyPhotonManager.onPlayerPpopertiesUpdate += OnPlayerPropertiesUpdate;

            //to modify manifest file
            bool value = false;

#if UNITY_IOS || UNITY_ANDROID
            if (value) Handheld.Vibrate();
#endif

            //initialize photon
            FourzyPhotonManager.Initialize(DEBUG: true);

            PlayerPrefsWrapper.AddAppOpened();
        }

        protected void Start()
        {
            if (Instance != this) return;

#if UNITY_IOS
            UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
            UnityEngine.iOS.NotificationServices.ClearRemoteNotifications();
#endif

            if (SceneManager.GetActiveScene().name == MainMenuSceneName)
            {
                StandaloneInputModuleExtended.GamepadFilter =
                    StandaloneInputModuleExtended.GamepadControlFilter.ANY_GAMEPAD;
            }

            //StandaloneInputModuleExtended.instance.AddNoInputFilter("startDemoGame", Constants.DEMO_IDLE_TIME);
            //StandaloneInputModuleExtended.instance.AddNoInputFilter("highlightMoves", Constants.DEMO_HIGHLIGHT_POSSIBLE_MOVES_TIME);

            //PointerInputModuleExtended.noInput += OnNoInput;

            EazyNetChecker.CheckIntervalNormal = 5f;
            EazyNetChecker.Timeout = 5f;
            EazyNetChecker.StartConnectionCheck(false, true);
            placementStyle = (PlacementStyle)PlayerPrefsWrapper.GetPlacementStyle();

#if !UNITY_IOS && !UNITY_ANDROID
            StartRoutine("location", GetLocation());
#endif
        }

        protected void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            FourzyPhotonManager.onEvent -= OnEventCall;
            Application.logMessageReceived -= HandleException;

            FourzyPhotonManager.onPlayerPpopertiesUpdate -= OnPlayerPropertiesUpdate;
            FourzyPhotonManager.onPlayerEnteredRoom -= OnPlayerEntered;

            //NetworkAccess.onNetworkAccess -= OnNetworkAccess;

            // #if UNITY_IOS
            //             MMVibrationManager.iOSReleaseHaptics();
            // #endif
        }

        protected void OnApplicationQuit()
        {
            if (resetGameOnClose)
            {
                ResetGames();
            }
        }

        public void SetExpectedGameType(GameTypeLocal gameType)
        {
            ExpectedGameType = gameType;
        }

        public Coroutine StartGame(IClientFourzy game, GameTypeLocal gameType)
        {
            SetExpectedGameType(gameType);

            switch (game._Type)
            {
                case GameType.TURN_BASED:
                    Debug.Log($"Starting challenge, id: {game.BoardID}");

                    break;
            }

            //if gameplay scene is already opened, just load game
            if (activeGame != null)
            {
                GamePlayManager.Instance.LoadGame(game);
            }
            else
            {
                sessionId = Guid.NewGuid().ToString();

                if (isMainMenuLoaded)
                {
                    SceneManager.LoadScene(Constants.GAMEPLAY_SCENE_NAME, LoadSceneMode.Additive);
                }
                else
                {
                    SceneManager.LoadScene(Constants.GAMEPLAY_SCENE_NAME);
                }
            }

            activeGame = game;

            return StartRoutine("startingGame", StartingGameRoutine());
        }

        public void StartGame(GameTypeLocal gameType)
        {
            SetExpectedGameType(gameType);

            if (isGameplaySceneLoaded)
            {
                GamePlayManager.Instance.CheckGameMode();
            }
            else
            {
                if (isMainMenuLoaded)
                {
                    SceneManager.LoadScene(Constants.GAMEPLAY_SCENE_NAME, LoadSceneMode.Additive);
                }
                else
                {
                    SceneManager.LoadScene(Constants.GAMEPLAY_SCENE_NAME);
                }
            }
        }

        public void OpenMainMenu()
        {
            //unload gameplay scene 
            if (isGameplaySceneLoaded && isMainMenuLoaded)
            {
                SceneManager.UnloadSceneAsync(Constants.GAMEPLAY_SCENE_NAME);
            }

            if (!isMainMenuLoaded)
            {
                SceneManager.LoadScene(MainMenuSceneName);
            }
        }

        public void OpenDiscordPage()
        {
            AnalyticsManager.Instance.LogEvent(
                "discordButtonPress",
                AnalyticsManager.AnalyticsProvider.ALL,
                new KeyValuePair<string, object>("playfabPlayerId", LoginManager.masterAccountId));

            Application.OpenURL(/*UnityWebRequest.EscapeURL(*/"https://discord.gg/t2zW7j3XRs"/*)*/);
        }

        public void ResetGames(bool resetHintInstructions = false)
        {
            GameContentManager.Instance.ResetFastPuzzles();
            GameContentManager.Instance.ResetPuzzlePacks();
            GameContentManager.Instance.tokensDataHolder.ResetTokenInstructions();

            if (resetHintInstructions)
            {
                ResetHintInstructions();
            }

            AnalyticsManager.Instance.LogEvent("progressReset");

            PlayerPrefsWrapper.AddAdventurePuzzlesResets();
            Amplitude.Instance.setUserProperty(
                "totalAdventurePuzzlesReset",
                PlayerPrefsWrapper.GetAdventurePuzzleResets());
        }

        public void ReportRealtimeGameFinished(IClientFourzy game, string winnerID, string opponentID, bool abandoned)
        {
            if (activeGame == null) return;
            if (string.IsNullOrEmpty(opponentID)) return;

            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "reportRatingGameComplete",
                FunctionParameter = new
                {
                    winnerID,
                    opponentID,
                    activeGame.draw,
                    abandoned,
                },
                GeneratePlayStreamEvent = true,
            },
            (result) =>
            {
                RatingGameCompleteResult gameCompleteResult =
                    JsonConvert.DeserializeObject<RatingGameCompleteResult>(result.FunctionResult.ToString());
                OnRatingDataAquired(gameCompleteResult);

                LogGameComplete(game);

                //try send rating update to other client 
                if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.PlayerListOthers.Length > 0)
                {
                    var eventOptions = new Photon.Realtime.RaiseEventOptions();
                    eventOptions.Flags.HttpForward = true;
                    eventOptions.Flags.WebhookFlags = Photon.Realtime.WebFlags.HttpForwardConst;

                    var photonEventResult = PhotonNetwork.RaiseEvent(
                        Constants.RATING_GAME_DATA,
                        result.FunctionResult.ToString(),
                        eventOptions,
                        SendOptions.SendReliable);
                }
            },
            (error) =>
            {
                Debug.LogError(error.ErrorMessage);

                ReportPlayFabError(error.ErrorMessage);
            });
        }

        public void ReportPlayFabError(string errorMessage)
        {
            if (lastErrorMessage == errorMessage) return;

            AnalyticsManager.Instance.LogEvent(
                "playfabError",
                AnalyticsManager.AnalyticsProvider.ALL,
                new KeyValuePair<string, object>("error", errorMessage));

            lastErrorMessage = errorMessage;
        }

        private void OnRatingDataAquired(RatingGameCompleteResult data)
        {
            foreach (RatingGamePlayer player in data.players)
            {
                if (player.playfabID == LoginManager.playfabId)
                {
                    UserManager.Instance.lastCachedRating = player.rating;
                    UserManager.Instance.realtimeGamesComplete += 1;

                    if (data.draw)
                    {
                        UserManager.Instance.playfabDrawsCount += 1;
                    }
                    else
                    {
                        if (player.winner)
                        {
                            UserManager.Instance.playfabWinsCount += 1;
                        }
                        else
                        {
                            UserManager.Instance.playfabLosesCount += 1;
                        }
                    }
                }
                else
                {
                    if (RealtimeOpponent != null)
                    {
                        RealtimeOpponent.SetExternalRating(player.rating);
                        RealtimeOpponent.SetExternalTotalGames(RealtimeOpponent.TotalGames + 1);
                    }
                }
            }

            ratingDataReceived?.Invoke(data);
        }

        public void ReportBotGameFinished(IClientFourzy game, bool updateRating, bool abandoned)
        {
            if (game == null) return;
            if (string.IsNullOrEmpty(RealtimeOpponent.Id)) return;

            float winner = game.draw ? .5f : (game.IsWinner() ? 1f : 0f);

            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "reportBotGameComplete",
                FunctionParameter = new
                {
                    playerId = LoginManager.playfabId,
                    winner,
                    botId = game.opponent.Profile.ToString(),
                    updateRating,
                    abandoned,
                },
                GeneratePlayStreamEvent = true,
            },
            (result) =>
            {
                RatingGameCompleteResult ratingGameResult =
                    JsonConvert.DeserializeObject<RatingGameCompleteResult>(result.FunctionResult.ToString());
                OnRatingDataAquired(ratingGameResult);

                if (RealtimeOpponent != null)
                {
                    //manually update opponent data
                    foreach (RatingGamePlayer playerData in ratingGameResult.players)
                    {
                        if (playerData.playfabID == "bot")
                        {
                            if (updateRating)
                            {
                                RealtimeOpponent.SetExternalRating(playerData.rating);
                            }

                            RealtimeOpponent.TotalGames += 1;
                        }
                    }
                }

                LogGameComplete(game);
            },
            (error) =>
            {
                Debug.LogError(error.ErrorMessage);

                ReportPlayFabError(error.ErrorMessage);
            });
        }

        public void ReportAreaProgression(Area _area)
        {
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "reportAreaProgression",
                FunctionParameter = new
                {
                    area = _area.ToString(),
                },
                GeneratePlayStreamEvent = true,
            },
            (result) =>
            {
                if (result.FunctionResult != null)
                {
                    ProgressionReward[] newItems =
                        JsonConvert.DeserializeObject<ProgressionReward[]>(result.FunctionResult.ToString());

                    bool rewardDisplayed = false;

                    foreach (var item in newItems)
                    {
                        switch (item.itemClass)
                        {
                            case Constants.PLAYFAB_BUNDLE_CLASS:
                                if (!GameContentManager.Instance.bundlesInPlayerInventory.Any(_bundle => _bundle == item.itemId))
                                {
                                    //new bundle added to player
                                    GameContentManager.Instance.bundlesInPlayerInventory.Add(item.itemId);
                                }

                                if (Application.isEditor || Debug.isDebugBuild)
                                {
                                    Debug.Log($"New bundle granted BY server {item.itemId}");
                                    Debug.Log($"Player bundles {string.Join(", ", GameContentManager.Instance.bundlesInPlayerInventory)}");
                                }

                                break;

                            case Constants.PLAYFAB_TOKEN_CLASS:
                                //unlock token
                                TokenType _token = (TokenType)Enum.Parse(typeof(TokenType), item.itemId);
                                UserManager.Instance.UnlockToken(_token, TokenUnlockType.AREA_PROGRESS);

                                if (Application.isEditor || Debug.isDebugBuild)
                                {
                                    Debug.Log($"Token unlocked {item.itemId}");
                                }

                                break;

                            case Constants.PLAYFAB_GAMEPIECE_CLASS:
                                GamePieceData gp = GameContentManager.Instance.piecesDataHolder.GetGamePieceData(item.itemId);

                                if (gp != null)
                                {
                                    gp.Pieces = item.count;
                                }
                                else
                                {
                                    string message = $"Gamepiece id {item.itemId} not found";

                                    if (Application.isEditor || Debug.isDebugBuild)
                                    {
                                        Debug.LogWarning(message);
                                    }

                                    Instance.ReportPlayFabError(message);
                                }

                                break;

                            case Constants.PLAYFAB_AREA_CLASS:
                                Area _area = (Area)Enum.Parse(typeof(Area), item.itemId);
                                GameContentManager.Instance.areasDataHolder.SetAreaUnlockedState(_area, true);

                                break;
                        }
                    }
                }

                UserManager.Instance.SetAreaProgression(_area, UserManager.Instance.GetAreaProgression(_area) + 1);

                if (Application.isEditor || Debug.isDebugBuild)
                {
                    Debug.Log($"Games in {_area} {UserManager.Instance.GetAreaProgression(_area)}");
                }
            },
            (error) =>
            {
                Debug.LogError(error.ErrorMessage);

                ReportPlayFabError(error.ErrorMessage);
            });
        }

        public void LogGameComplete(IClientFourzy game)
        {
            if (game._Type == GameType.ONBOARDING) return;

            AnalyticsManager.GameResultType gameResult = AnalyticsManager.GameResultType.none;
            Dictionary<string, object> extraParams = new Dictionary<string, object>();

            bool isPlayer1 = game.me == game.player1;

            if (GamePlayManager.Instance)
            {
                if (GamePlayManager.Instance.GameplayScreen.timersEnabled)
                {
                    float player1TimeLeft = isPlayer1 ?
                        GamePlayManager.Instance.GameplayScreen.MyTimerLeft :
                        GamePlayManager.Instance.GameplayScreen.OpponentTimerLeft;
                    float player2TimeLeft = isPlayer1 ?
                        GamePlayManager.Instance.GameplayScreen.OpponentTimerLeft :
                        GamePlayManager.Instance.GameplayScreen.MyTimerLeft;

                    if (player1TimeLeft <= 0f)
                    {
                        gameResult = AnalyticsManager.GameResultType.player1TimeExpired;
                    }
                    else if (player2TimeLeft <= 0f)
                    {
                        gameResult = AnalyticsManager.GameResultType.player2TimeExpired;
                    }

                    extraParams.Add("player1TimeRemaining", player1TimeLeft);
                    extraParams.Add("player2TimeRemaining", player2TimeLeft);
                }
            }

            if (gameResult == AnalyticsManager.GameResultType.none)
            {
                if (!game.turnEvaluator.IsAvailableSimpleMove())
                {
                    gameResult = AnalyticsManager.GameResultType.noPossibleMoves;
                }
                else if (game.draw)
                {
                    gameResult = AnalyticsManager.GameResultType.draw;
                }
                else
                {
                    bool checkPlayer1or2Win = false;

                    switch (ExpectedGameType)
                    {
                        case GameTypeLocal.REALTIME_BOT_GAME:
                        case GameTypeLocal.REALTIME_LOBBY_GAME:
                        case GameTypeLocal.REALTIME_QUICKMATCH:
                            checkPlayer1or2Win = true;

                            break;

                        case GameTypeLocal.LOCAL_GAME:
                            switch (game._Mode)
                            {
                                case GameMode.VERSUS:
                                    checkPlayer1or2Win = true;

                                    break;
                            }

                            break;
                    }

                    if (checkPlayer1or2Win)
                    {
                        gameResult = game.IsWinner(game.player1) ?
                            AnalyticsManager.GameResultType.player1Win :
                            AnalyticsManager.GameResultType.player2Win;
                    }
                    else
                    {
                        gameResult = game.IsWinner() ?
                            AnalyticsManager.GameResultType.win :
                            AnalyticsManager.GameResultType.lose;
                    }
                }
            }

            extraParams.Add(AnalyticsManager.GAME_RESULT_KEY, gameResult.ToString());

            if (gameResult != AnalyticsManager.GameResultType.none)
            {
                AnalyticsManager.Instance.LogGame(game.GameToAnalyticsEvent(false), game, extraParams);
            }
        }

        public void ChallengePlayerRealtime(string playfabName)
        {
            FourzyPhotonManager.CreateRoom(RoomType.DIRECT_INVITE, expectedUser: playfabName);
        }

        public void ResetHintInstructions()
        {
            PlayerPrefsWrapper.SetHintTutorialStage(0);
        }

        public void OnPurchaseComplete(Product product)
        {
            onPurchaseComplete?.Invoke(product);

            //try get product data
            MiscGameContentHolder.StoreItemExtraData _data =
                GameContentManager.Instance.miscGameDataHolder.GetStoreItem(product.definition.id);

            if (!_data) return;

            //Amplitude.Instance.setUserProperty("totalSpent", UserManager.Instance.totalSpentUSD);

            //if (product.definition.id.Contains("hints"))
            //{
            //    UserManager.Instance.hints += _data.quantity;

            //    //analytics
            //    if (activeGame != null)
            //    {
            //        AnalyticsManager.Instance.LogGame(
            //            AnalyticsManager.AnalyticsEvents.hintPurchase,
            //            activeGame,
            //            AnalyticsManager.AnalyticsProvider.ALL,
            //            new KeyValuePair<string, object>(AnalyticsManager.HINT_STORE_ITEMS_KEY, StorePromptScreen.ProductsToString(StorePromptScreen.StoreItemType.HINTS)),
            //            new KeyValuePair<string, object>(AnalyticsManager.STORE_ITEM_KEY, _data.id));
            //    }
            //}
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
        {
            onPurchaseFailed?.Invoke(product);
        }

        public void StartPresentataionGame()
        {
            OnNoInput(new KeyValuePair<string, float>("startDemoGame", 0f));
        }

        public void CheckNews()
        {
            if (!NetworkAccess) return;
            Debug.Log("Fetching news..");

            PlayFabClientAPI.GetTitleNews(new GetTitleNewsRequest(),
            result =>
            {
                Debug.Log("News fetched " + result.News.Count);
                latestNews = result.News;

                UserManager.Instance.AddPlayfabValueLoaded(PlayfabValuesLoaded.NEWS_CHECKED);

                onNewsFetched?.Invoke();
            },
            error =>
            {
                Debug.LogError(error.GenerateErrorReport());
                ReportPlayFabError(error.ErrorMessage);
            });
        }

        public void StartRealtimeQuickGame()
        {
            //check if there is abandoned realtime game
            string roomName = PlayerPrefsWrapper.GetAbandonedRealtimeRoomName();
            if (!string.IsNullOrEmpty(roomName) && FourzyPhotonManager.Instance.cachedRooms.Any(info => info.Name == roomName && info.IsOpen))
            {
                MenuController.activeMenu.GetOrAddScreen<PromptScreen>().Prompt("Abandoned Room Available", "You can join previously abandoned room", "Join", "Ignore", () =>
                {
                    RejoinAbandonedGame = true;
                    FourzyPhotonManager.JoinRoom(roomName, true);
                    FourzyPhotonManager.onJoinedRoom += OnRoomJoinedRematch;
                    FourzyPhotonManager.onJoinRoomFailed += OnRoomJoinFailed;
                }, () =>
                {
                    MenuController.activeMenu.currentScreen.CloseSelf();
                    MenuController.activeMenu.GetScreen<MatchmakingScreen>().OpenRealtime();
                })
                    .CloseOnAccept();
            }
            else
            {
                MenuController.activeMenu.GetScreen<MatchmakingScreen>().OpenRealtime();
            }
        }

        private void OnRoomJoinFailed(string roomName)
        {
            RejoinAbandonedGame = false;
            FourzyPhotonManager.onJoinRoomFailed -= OnRoomJoinFailed;
        }

        private void OnRoomJoinedRematch(string roomName)
        {
            RoomType roomType = FourzyPhotonManager.GetRoomProperty(Constants.REALTIME_ROOM_TYPE_KEY, RoomType.NONE);
            switch (roomType)
            {
                case RoomType.LOBBY_ROOM:
                case RoomType.DIRECT_INVITE:
                    ExpectedGameType = GameTypeLocal.REALTIME_LOBBY_GAME;
                    RealtimeOpponent = new OpponentData(PhotonNetwork.PlayerListOthers[0]);

                    break;

                case RoomType.QUICKMATCH:
                    ExpectedGameType = GameTypeLocal.REALTIME_QUICKMATCH;
                    RealtimeOpponent = new OpponentData(PhotonNetwork.PlayerListOthers[0]);

                    break;
            }

            FourzyPhotonManager.onJoinedRoom -= OnRoomJoinedRematch;
            StartGame(ExpectedGameType);
        }

        public static void Vibrate(HapticTypes type) => MMVibrationManager.Haptic(type);

        public static void Vibrate() => Vibrate(HapticTypes.Success);

        public static bool NetworkAccess => EazyNetChecker.Status == NetStatus.Connected;

        public static void UpdateFastPuzzlesStat(int _value)
        {
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "reportFastPuzzlesCount",
                FunctionParameter = new { value = _value },
                GeneratePlayStreamEvent = true,
            },
            (result) => { Debug.Log($"Fast puzzles stat updated {_value}"); },
            (error) =>
            {
                Debug.LogError(error.ErrorMessage);

                Instance.ReportPlayFabError(error.ErrorMessage);
            });
        }

        public static void GetTitleData(Action<object> onDataLoaded, Action onFailed)
        {
            PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(),
                result =>
                {
                    onDataLoaded?.Invoke(result.Data);
                }, error =>
                {
                    Instance.ReportPlayFabError(error.ErrorMessage);
                    Debug.Log(error.ErrorMessage);
                    onFailed?.Invoke();
                });
        }

        public static int ValueFromCurrencyType(CurrencyType type)
        {
            switch (type)
            {
                case CurrencyType.COINS:
                    return UserManager.Instance.coins;

                case CurrencyType.GEMS:
                    return UserManager.Instance.gems;

                case CurrencyType.MAGIC:
                    return UserManager.Instance.coins;

                case CurrencyType.PORTAL_POINTS:
                    return UserManager.Instance.portalPoints;

                case CurrencyType.RARE_PORTAL_POINTS:
                    return UserManager.Instance.rarePortalPoints;

                case CurrencyType.TICKETS:
                    return UserManager.Instance.tickets;

                case CurrencyType.HINTS:
                    return UserManager.Instance.hints;

                case CurrencyType.XP:
                    return UserManager.Instance.xp;
            }

            return 0;
        }

        public static CurrencyType RewardToCurrency(RewardType type)
        {
            switch (type)
            {
                case RewardType.COINS:
                    return CurrencyType.COINS;

                case RewardType.XP:
                    return CurrencyType.XP;

                case RewardType.TICKETS:
                    return CurrencyType.TICKETS;

                case RewardType.MAGIC:
                    return CurrencyType.MAGIC;

                case RewardType.GEMS:
                    return CurrencyType.GEMS;

                case RewardType.PORTAL_POINTS:
                    return CurrencyType.PORTAL_POINTS;

                case RewardType.RARE_PORTAL_POINTS:
                    return CurrencyType.RARE_PORTAL_POINTS;

                case RewardType.HINTS:
                    return CurrencyType.HINTS;

                default:
                    return CurrencyType.NONE;
            }
        }

        internal void BotWinsByPlayerForfeit()
        {
            if (activeGame == null) return;
            if (string.IsNullOrEmpty(RealtimeOpponent.Id)) return;

            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "reportRatingGameComplete",
                FunctionParameter = new
                {
                    winnerID = RealtimeOpponent,
                    opponentID = LoginManager.playfabId,
                    draw = false,
                },
                GeneratePlayStreamEvent = true,
            },
            (result) =>
            {
                RatingGameCompleteResult gameResult = JsonConvert
                    .DeserializeObject<RatingGameCompleteResult>(result.FunctionResult.ToString());

                foreach (RatingGamePlayer player in gameResult.players)
                {
                    Debug.Log($"{player.playfabID} {player.rating} {player.ratingChange} {player.winner}");
                    if (player.playfabID == LoginManager.playfabId)
                    {
                        UserManager.Instance.playfabLosesCount += 1;
                        UserManager.Instance.lastCachedRating = player.rating;
                    }
                }
            },
            (error) =>
            {
                Debug.LogError(error.ErrorMessage);

                Instance.ReportPlayFabError(error.ErrorMessage);
            });
        }

        private void HandleException(string condition, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                    AnalyticsManager.Instance.LogEvent(
                        "error",
                        AnalyticsManager.AnalyticsProvider.ALL,
                        new KeyValuePair<string, object>("error", condition));

                    break;
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            switch (scene.name)
            {
                case Constants.GAMEPLAY_SCENE_NAME:
                    MenuController.SetState("MainMenuCanvas", false);

                    AudioHolder.instance.StopBGAudio("bg_main_menu", .5f);
                    SceneManager.SetActiveScene(scene);

                    break;

                case Constants.MAIN_MENU_L_SCENE_NAME:
                case Constants.MAIN_MENU_P_SCENE_NAME:

                    break;
            }

            onSceneChanged?.Invoke(scene.name);
        }

        private void OnSceneUnloaded(Scene scene)
        {
            switch (scene.name)
            {
                case Constants.GAMEPLAY_SCENE_NAME:
                    MenuController.SetState("MainMenuCanvas", true);

                    AudioHolder.instance.PlayBGAudio("bg_main_menu", true, .75f, 3f);

                    //change gamepad mode
                    StandaloneInputModuleExtended.GamepadFilter = StandaloneInputModuleExtended.GamepadControlFilter.ANY_GAMEPAD;

                    ////check if there are any news
                    //CheckNews();

                    activeGame = null;
                    currentPuzzlePack = null;

                    break;
            }

            onSceneChanged?.Invoke(SceneManager.GetActiveScene().name);
        }

        private void OnPlayerEntered(Photon.Realtime.Player obj)
        {
            //lock room 
            //PhotonNetwork.CurrentRoom.IsVisible = false;
        }

        private void OnPlayerPropertiesUpdate(
            Photon.Realtime.Player player,
            ExitGames.Client.Photon.Hashtable data)
        {
            if (PhotonNetwork.PlayerListOthers.Length == 0 || PhotonNetwork.PlayerListOthers[0] != player) return;

            if (RealtimeOpponent != null)
            {
                if (data.ContainsKey(Constants.REALTIME_WINS_KEY) ||
                    data.ContainsKey(Constants.REALTIME_LOSES_KEY) ||
                    data.ContainsKey(Constants.REALTIME_DRAWS_KEY))
                {
                    RealtimeOpponent.useExternalTotalGamesValue = false;
                    RealtimeOpponent.TotalGames = FourzyPhotonManager.GetPlayerTotalGamesCount(player);
                }
                else if (data.ContainsKey(Constants.REALTIME_RATING_KEY))
                {
                    RealtimeOpponent.useExternalRatingValue = false;
                    RealtimeOpponent.Rating = (int)data[Constants.REALTIME_RATING_KEY];
                }
            }
        }

        private void OnNetStatusChanged()
        {
            onNetworkAccess?.Invoke(EazyNetChecker.Status == NetStatus.Connected);
        }

        private void OnNoInput(KeyValuePair<string, float> noInputFilter)
        {
            if (!SettingsManager.Get(SettingsManager.KEY_DEMO_MODE) || noInputFilter.Key != "startDemoGame") return;

            GameContentManager.Instance.ResetFastPuzzles();
            GameContentManager.Instance.ResetPuzzlePacks();
            GameContentManager.Instance.tokensDataHolder.ResetTokenInstructions();

            //check
            if (activeGame == null || (activeGame._Type != GameType.ONBOARDING && activeGame._Type != GameType.PRESENTATION))
            {
                ClientFourzyGame _game = new ClientFourzyGame(GameContentManager.Instance.areasDataHolder.areas.Random().areaID,
                    new Player(1, "AI Player 1") { PlayerString = "1" },
                    new Player(2, "AI Player 2") { PlayerString = "2" }, 1)
                { _Type = GameType.PRESENTATION, };
                _game.UpdateFirstState();

                //start demo mode
                StartGame(_game, GameTypeLocal.LOCAL_GAME);
            }
        }

        private void OnEventCall(EventData data)
        {
            switch (data.Code)
            {
                case Constants.RATING_GAME_DATA:
                    OnRatingDataAquired(
                        JsonConvert.DeserializeObject<RatingGameCompleteResult>(data.CustomData.ToString()));

                    break;
            }
        }

        private IEnumerator StartingGameRoutine()
        {
            while (!GamePlayManager.Instance || !GamePlayManager.Instance.IsBoardReady) yield return null;
        }

#if !UNITY_IOS && !UNITY_ANDROID
        private IEnumerator GetLocation()
        {
            //if (!Input.location.isEnabledByUser) { Debug.LogWarning("location services disabled"); yield break; }

            Input.location.Start();

            int maxWait = 5;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            if (maxWait < 1)
            {
                print("locaiton timed out"); yield break;
            }

            if (Input.location.status == LocationServiceStatus.Failed)
            {
                print("Unable to determine device location");
                yield break;
            }
            else
                lastLocation = Input.location.lastData;

            Input.location.Stop();
        }
#endif

        [System.Serializable]
        public class PuzzleData
        {
            //data goes in here
            public int packID;
            public string packName;
            //...

            //gameboards data
            public List<GameBoardDefinitionData> puzzles = new List<GameBoardDefinitionData>();
        }

        [System.Serializable]
        public class GameBoardDefinitionData
        {
            public string puzzleName;

            public string assetGUID;
            public GameBoardDefinition gameboard;
            [JsonIgnore]
            public TextAsset assetFile;
        }

        public enum BotGameType
        {
            NONE,
            REGULAR,
            FTUE_RATED,
            FTUE_NOT_RATED,
        }

        public enum PassPlayCharactersType
        {
            SELECTED_VARIATION,
            SELECTED_RANDOM,
            RANDOM,
        }

        public enum PlacementStyle
        {
            EDGE_TAP,
            DEMO_STYLE,
            SWIPE,
            TAP_AND_DRAG,
            TWO_STEP_SWIPE,
        }
    }

    /// <summary>
    /// For client use only
    /// </summary>
    public enum GameTypeLocal
    {
        LOCAL_GAME,
        REALTIME_LOBBY_GAME,
        REALTIME_QUICKMATCH,
        REALTIME_BOT_GAME,
        ASYNC_SKILLZ_GAME,
    }

    public enum BuildIntent
    {
        MOBILE_REGULAR,
        DESKTOP_REGULAR,
        MOBILE_SKILLZ,
    }
}
