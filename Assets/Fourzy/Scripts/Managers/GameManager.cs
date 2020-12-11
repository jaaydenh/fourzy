//@vadym udod

using Firebase;
using Firebase.Analytics;
using Firebase.RemoteConfig;
using Firebase.Storage;
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
using MoreMountains.NiceVibrations;
using Newtonsoft.Json;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        public static Dictionary<string, object> APP_REMOTE_SETTINGS_DEFAULTS;

        public static GameManager Instance;

        [Header("Display tutorials?")]
        public bool displayTutorials = true;
        [Header("Display tutorial even if it was already displayed")]
        public bool forceDisplayTutorials = true;
        public bool showInfoToasts = true;
        public bool debugMessages = true;
        [Tooltip("Options effected:\n   - Reset Puzzles\n   - Pass&Play rewards screen"), SerializeField]
        private bool extraFeatures = true;

        [Header("Pass And Play games only")]
        public bool tapToStartGame = true;
        public PassPlayCharactersType characterType = PassPlayCharactersType.SELECTED_RANDOM;

        [Header("Landscape settings")]
        public bool hideTabsBar = true;
        public bool hideGems = true;
        public bool hideTickets = true;
        public bool hidePortalWidgets = true;

        [Header("Misc settings")]
        public bool defaultGauntletState = true;
        public bool defaultPuzzlesState = true;
        public bool resetGameOnClose = true;
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
        public bool ExtraFeatures => extraFeatures || Application.isEditor;
        public GameTypeLocal ExpectedGameType { get; private set; }
        public PuzzleData dailyPuzzlePack { get; private set; }
        public IClientFourzy activeGame { get; set; }
        public BasicPuzzlePack currentPuzzlePack { get; set; }
        public Camera3dItemProgressionMap currentMap { get; set; }
        public DependencyStatus dependencyStatus { get; set; }
        public List<TitleNewsItem> latestNews { get; private set; } = new List<TitleNewsItem>();
        public string sessionID { get; private set; }
        public LocationInfo? lastLocation { get; private set; } = null;
        public string MainMenuSceneName => Landscape ? Constants.MAIN_MENU_L_SCENE_NAME : Constants.MAIN_MENU_P_SCENE_NAME;

        private bool configFetched = false;
        private PlacementStyle _placementStyle;

        public bool isMainMenuLoaded
        {
            get
            {
                bool mainMenuLoaded = false;
                for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
                    if (SceneManager.GetSceneAt(sceneIndex).name == MainMenuSceneName)
                    {
                        mainMenuLoaded = true;
                        break;
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
                    if (SceneManager.GetSceneAt(sceneIndex).name == Constants.GAMEPLAY_SCENE_NAME)
                    {
                        mainMenuLoaded = true;
                        break;
                    }

                return mainMenuLoaded;
            }
        }

        public float latitude
        {
            get
            {
                if (lastLocation == null) return fallbackLatitude;
                else return lastLocation.Value.latitude;
            }
        }

        public float longitude
        {
            get
            {
                if (lastLocation == null) return fallbackLongitude;
                else return lastLocation.Value.longitude;
            }
        }

        public List<TitleNewsItem> unreadNews => latestNews?.Where(titleNews => !PlayerPrefsWrapper.GetNewsOpened(titleNews.NewsId)).ToList() ?? new List<TitleNewsItem>();

        protected override void Awake()
        {
            base.Awake();

            if (Instance) return;

            Instance = this;

#if UNITY_IOS
            MMVibrationManager.iOSInitializeHaptics();
#endif

            ExecutePerVersion.TryExecute();
            ThreadsQueuer.Initialize();
            AnalyticsManager.Initialize(true);

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            NetworkManager.onStatusChanged += OnNetStatusChanged;
            FourzyPhotonManager.onPlayerEnteredRoom += OnPlayerEntered;

            //to modify manifest file
            bool value = false;


#if UNITY_IOS || UNITY_ANDROID
            if (value) Handheld.Vibrate();
#endif

            APP_REMOTE_SETTINGS_DEFAULTS = new Dictionary<string, object>()
            {
                [Constants.KEY_APP_VERSION] = Application.version,
                [Constants.KEY_DAILY_PUZZLE] = "",
                [Constants.KEY_STORE_STATE] = "1",

                //rewards
                [Constants.KEY_REWARDS_TURNBASED] = "0",
                [Constants.KEY_REWARDS_PASSPLAY] = "0",
                [Constants.KEY_REWARDS_PUZZLEPLAY] = "0",
                [Constants.KEY_REWARDS_REALTIME] = "0",
            };

            FirebaseRemoteConfig.SetDefaults(APP_REMOTE_SETTINGS_DEFAULTS);

            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                dependencyStatus = task.Result;

                if (dependencyStatus != DependencyStatus.Available)
                {
                    if (debugMessages)
                        Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
                }
                else
                {
                    //if (NetworkAccess.HAVE_ACCESS) FirebaseUpdate();
                    //if (EazyNetChecker.Status == NetStatus.Connected) FirebaseUpdate();
                }
            });

            if (!PlayerPrefsWrapper.GetInitialPropertiesSet())
            {
                //unlock areas
                foreach (Area area in Enum.GetValues(typeof(Area)))
                {
                    bool state = ((int)area & Constants.DEFAULT_UNLOCKED_THEMES) > 0;
                    if (state) PlayerPrefsWrapper.SetThemeUnlocked((int)area, state);
                }

                PlayerPrefsWrapper.InitialPropertiesSet(true);
            }
            else
            {
                // temporary until unlock areas is implemented
                foreach (Area area in Enum.GetValues(typeof(Area)))
                {
                    bool state = ((int)area & Constants.DEFAULT_UNLOCKED_THEMES) > 0;
                    if (state) PlayerPrefsWrapper.SetThemeUnlocked((int)area, state);
                }
            }

            //initialize photon
            FourzyPhotonManager.Initialize(DEBUG: true);
        }

        protected void Start()
        {
            if (Instance != this) return;

#if UNITY_IOS
            UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
            UnityEngine.iOS.NotificationServices.ClearRemoteNotifications();
#endif

            //NetworkAccess.onNetworkAccess += OnNetworkAccess;

            if (SceneManager.GetActiveScene().name == MainMenuSceneName) StandaloneInputModuleExtended.GamepadFilter = StandaloneInputModuleExtended.GamepadControlFilter.ANY_GAMEPAD;

            //StandaloneInputModuleExtended.instance.AddNoInputFilter("startDemoGame", Constants.DEMO_IDLE_TIME);
            //StandaloneInputModuleExtended.instance.AddNoInputFilter("highlightMoves", Constants.DEMO_HIGHLIGHT_POSSIBLE_MOVES_TIME);

            //PointerInputModuleExtended.noInput += OnNoInput;

            NetworkManager.instance.StartChecking();
            placementStyle = (PlacementStyle)PlayerPrefsWrapper.GetPlacementStyle();

#if !UNITY_IOS && !UNITY_ANDROID
            StartRoutine("location", GetLocation());
#endif
        }

        protected void Update()
        {
            //force demo game
            if (StandaloneInputModuleExtended.OnHotkey1Press()) StandaloneInputModuleExtended.instance.TriggerNoInputEvent("startDemoGame");
            //if (Input.GetKeyDown(KeyCode.Z)) PhotonNetwork.Disconnect();
            //if (Input.GetKeyDown(KeyCode.X)) print(PhotonNetwork.NetworkClientState);
        }

        protected void OnDestroy()
        {
            FourzyPhotonManager.onPlayerEnteredRoom -= OnPlayerEntered;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;

            //NetworkAccess.onNetworkAccess -= OnNetworkAccess;

#if UNITY_IOS
            MMVibrationManager.iOSReleaseHaptics();
#endif
        }

        protected void OnApplicationQuit()
        {
            if (resetGameOnClose) ResetGames();
        }

        protected void OnApplicationPause(bool pause)
        {
            //if (!pause)
            //{
            //    if (NetworkAccess.HAVE_ACCESS)
            //    {
            //        if (dependencyStatus == DependencyStatus.Available)
            //            FirebaseUpdate();
            //    }
            //}
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
                    if (debugMessages)
                        Debug.Log($"Starting challenge, id: {game.BoardID}");
                    break;
            }

            //if gameplay scene is already opened, just load game
            if (activeGame != null)
                GamePlayManager.Instance.LoadGame(game);
            else
            {
                sessionID = Guid.NewGuid().ToString();

                if (isMainMenuLoaded)
                    SceneManager.LoadScene(Constants.GAMEPLAY_SCENE_NAME, LoadSceneMode.Additive);
                else
                    SceneManager.LoadScene(Constants.GAMEPLAY_SCENE_NAME);
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
                    SceneManager.LoadScene(Constants.GAMEPLAY_SCENE_NAME, LoadSceneMode.Additive);
                else
                    SceneManager.LoadScene(Constants.GAMEPLAY_SCENE_NAME);
            }
        }

        public void OpenMainMenu()
        {
            if (!isMainMenuLoaded) SceneManager.LoadScene(MainMenuSceneName);

            //unload gameplay scene 
            if (isGameplaySceneLoaded) GamePlayManager.Instance.UnloadGamePlaySceene();
        }

        public void ResetGames()
        {
            GameContentManager.Instance.ResetFastPuzzles();
            GameContentManager.Instance.ResetPuzzlePacks();
            GameContentManager.Instance.tokensDataHolder.ResetTokenInstructions();
            ResetHintInstructions();
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
            MiscGameContentHolder.StoreItemExtraData _data = GameContentManager.Instance.miscGameDataHolder.GetStoreItem(product.definition.id);

            if (!_data) return;

            if (product.definition.id.Contains("hints"))
            {
                UserManager.Instance.hints += _data.quantity;

                //analytics
                if (activeGame != null)
                    AnalyticsManager.Instance.LogGame(
                        AnalyticsManager.AnalyticsGameEvents.PUZZLE_HINT_STORE_HINT_PURCHASE,
                        activeGame,
                        AnalyticsManager.AnalyticsProvider.ALL,
                        new KeyValuePair<string, object>(AnalyticsManager.HINT_STORE_ITEMS_KEY, StorePromptScreen.ProductsToString(StorePromptScreen.StoreItemType.HINTS)),
                        new KeyValuePair<string, object>(AnalyticsManager.STORE_ITEM_KEY, _data.id));
            }
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
        {
            onPurchaseFailed?.Invoke(product);
        }

        public void StartPresentataionGame() => OnNoInput(new KeyValuePair<string, float>("startDemoGame", 0f));

        public void CheckNews()
        {
            if (!NetworkAccess) return;
            Debug.Log("Fetching news..");

            StartRoutine("fetching_news", FetchingNews());
        }

        public static void UpdateGameTypeUserProperty(GameType gameType)
        {
            Debug.Log("Updating user property");
            FirebaseAnalytics.SetUserProperty("game_type", gameType.ToString());
        }

        public static void Vibrate(HapticTypes type) => MMVibrationManager.Haptic(type);

        public static void Vibrate() => Vibrate(HapticTypes.Success);

        public static bool NetworkAccess => NetworkManager.Status == NetStatus.Connected;

        public static void UpdateStatistic(string stat, int _value)
        {
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "updateStatistic",
                FunctionParameter = new { stat_name = stat, value = _value },
                GeneratePlayStreamEvent = true,
            }, (result) => { Debug.Log($"{stat} updated {_value}"); }, (error) => { Debug.LogError(error.ErrorMessage); });
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

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            switch (scene.name)
            {
                case Constants.GAMEPLAY_SCENE_NAME:
                    MenuController.SetState("MainMenuCanvas", false);

                    AudioHolder.instance.StopBGAudio(AudioTypes.BG_MAIN_MENU, .5f);
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

                    AudioHolder.instance.PlayBGAudio(AudioTypes.BG_MAIN_MENU, true, .75f, 3f);

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

        private void FirebaseUpdate()
        {
            FirebaseAnalytics.SetUserProperty(Constants.KEY_EXTRA_FEATURES, ExtraFeatures ? "1" : "0");

            StartCoroutine(GetRemoteSettingsRoutine());
        }

        private void GetRemoteSettings()
        {
            Task fetchTask = FirebaseRemoteConfig.FetchAsync(TimeSpan.Zero);
            fetchTask.ContinueWith(FetchComplete);
        }

        private bool VersionCheck()
        {
            string value = FirebaseRemoteConfig.GetValue(Constants.KEY_APP_VERSION).StringValue;

            if (debugMessages)
                Debug.Log($"Version check: {Application.version} app version, {value} required app version");

            //version checker
            if (Application.version != value)
            {
                //show version popup
                PersistantMenuController.instance.GetOrAddScreen<PromptScreen>().Prompt(
                    "New Version Available",
                    "New app version available,\nplease update your app pressing button below",
                    "Update",
                    null,
                    () =>
                {
                    //open app page in related store
                    PersistantMenuController.instance.CloseCurrentScreen(true);
                },
                //do nothing on decline
                () => { }
                );
            }

            return Application.version != value;
        }

        private void RewardsStateCheck()
        {
            //check rewards for turnbased/passplay/puzzleplay/realtime
            new List<string>() {
                Constants.KEY_REWARDS_TURNBASED,
                Constants.KEY_REWARDS_PASSPLAY,
                Constants.KEY_REWARDS_PUZZLEPLAY,
                Constants.KEY_REWARDS_REALTIME,
            }.ForEach(key =>
                {
                    string value = FirebaseRemoteConfig.GetValue(key).StringValue;
                    Debug.Log($"Rewards {key}: '{PlayerPrefsWrapper.GetRemoteSetting(key)}' old state, '{(value == "1" ? true : false)}' new state");
                    PlayerPrefsWrapper.SetRemoteSetting(key, value);
                });
        }

        private void DailyPuzzleCheck()
        {
            string value = FirebaseRemoteConfig.GetValue(Constants.KEY_DAILY_PUZZLE).StringValue;

            if (debugMessages)
                Debug.Log($"Old Daily Puzzle file '{PlayerPrefsWrapper.GetRemoteSetting(Constants.KEY_DAILY_PUZZLE)}', new '{value}'");

            if (!string.IsNullOrEmpty(value))
            {
                //filename check
                if (PlayerPrefsWrapper.GetRemoteSetting(Constants.KEY_DAILY_PUZZLE) != value)
                {
                    PlayerPrefsWrapper.SetRemoteSetting(Constants.KEY_DAILY_PUZZLE, value);

                    onDailyChallengeFileName?.Invoke(value);

                    //download file
                    FirebaseStorage storage = FirebaseStorage.GetInstance("gs://fourzytesting.appspot.com");
                    StorageReference reference = storage.GetReference("puzzles/daily/" + value);

                    reference.GetFileAsync(Application.persistentDataPath + "/" + value).ContinueWith(task =>
                    {
                        Debug.Log(task.Status);
                        if (!task.IsFaulted && !task.IsCanceled)
                        {
                            //downloaded
                            ThreadsQueuer.Instance.QueueFuncToExecuteFromMainThread(() => DisplayDailyPuzzlePopup());
                        }
                    });
                }
            }
        }

        private void StoreCheck()
        {
            string value = FirebaseRemoteConfig.GetValue(Constants.KEY_STORE_STATE).StringValue;
            if (debugMessages) Debug.Log($"Old Store State '{PlayerPrefsWrapper.GetRemoteSetting(Constants.KEY_STORE_STATE)}', new '{(value == "1" ? true : false)}'");
            PlayerPrefsWrapper.SetRemoteSetting(Constants.KEY_STORE_STATE, value);
        }

        private void DisplayDailyPuzzlePopup()
        {
            dailyPuzzlePack = JsonConvert.DeserializeObject<PuzzleData>(File.ReadAllText(Application.persistentDataPath + "/" + PlayerPrefsWrapper.GetRemoteSetting(Constants.KEY_DAILY_PUZZLE)));

            if (dailyPuzzlePack != null)
            {
                //new daily puzzle available


                //show daily puzzle popup
                PersistantMenuController.instance.GetOrAddScreen<PromptScreen>().Prompt(
                    "New Daily Puzzle",
                    "New Daily Puzzle Available!",
                    "Update",
                    null,
                    () =>
                    {
                        //open app page in related store
                        PersistantMenuController.instance.CloseCurrentScreen(true);
                    },
                    null
                );
            }
        }

        private void FetchComplete(Task fetchTask)
        {
            if (fetchTask.IsCanceled)
                Debug.Log("Remote config canceled.");
            else if (fetchTask.IsFaulted)
                Debug.Log("Remote config encountered an error.");
            else if (fetchTask.IsCompleted)
                Debug.Log("Remote config completed successfully!");

            var info = FirebaseRemoteConfig.Info;

            switch (info.LastFetchStatus)
            {
                case LastFetchStatus.Success:
                    FirebaseRemoteConfig.ActivateFetched();

                    if (debugMessages) Debug.Log($"Remote data loaded and ready (last fetch time {info.FetchTime}).");

                    //initial settings fetch only once
                    if (!configFetched)
                    {
                        try
                        {
                            ThreadsQueuer.Instance.QueueFuncToExecuteFromMainThread(() =>
                            {
                                configFetched = true;

                                //do version check, if no new version available, check daily challenge
                                if (!VersionCheck())
                                {
                                    //daily puzzle check
                                    DailyPuzzleCheck();
                                }

                                StoreCheck();
                            });
                        }
                        catch (Exception e)
                        {
                            Debug.Log(e.ToString());
                        }
                    }

                    //fetch other values
                    ThreadsQueuer.Instance.QueueFuncToExecuteFromMainThread(() =>
                    {
                        RewardsStateCheck();
                        //...
                    });

                    break;

                case LastFetchStatus.Failure:
                    switch (info.LastFetchFailureReason)
                    {
                        case FetchFailureReason.Error:
                            Debug.Log("Remote config failed for unknown reason");

                            break;

                        case FetchFailureReason.Throttled:
                            Debug.Log("Remote config throttled until " + info.ThrottledEndTime);

                            break;
                    }
                    break;

                case LastFetchStatus.Pending:
                    Debug.Log("Latest Remote config call still pending.");
                    break;
            }
        }

        private void OnNetworkAccess(bool networkAccess)
        {
            if (networkAccess)
            {
                if (dependencyStatus == DependencyStatus.Available)
                    FirebaseUpdate();
            }
        }

        private void OnPlayerEntered(Photon.Realtime.Player obj)
        {
            //lock room 
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }

        private void OnNetStatusChanged(NetStatus status) => onNetworkAccess?.Invoke(status == NetStatus.Connected);

        private void OnNoInput(KeyValuePair<string, float> noInputFilter)
        {
            if (!SettingsManager.Get(SettingsManager.KEY_DEMO_MODE) || noInputFilter.Key != "startDemoGame") return;

            GameContentManager.Instance.ResetFastPuzzles();
            GameContentManager.Instance.ResetPuzzlePacks();
            GameContentManager.Instance.tokensDataHolder.ResetTokenInstructions();

            //check
            if (activeGame == null || (activeGame._Type != GameType.ONBOARDING && activeGame._Type != GameType.PRESENTATION))
            {
                //start demo mode
                StartGame(new ClientFourzyGame(GameContentManager.Instance.themesDataHolder.GetRandomTheme(Area.NONE),
                    new Player(1, "AI Player 1") { PlayerString = "1" },
                    new Player(2, "AI Player 2") { PlayerString = "2" }, 1)
                { _Type = GameType.PRESENTATION, }, GameTypeLocal.LOCAL_GAME);
            }
        }

        private IEnumerator FetchingNews()
        {
            while (!LoginManager.Instance.languageChecked) yield return null;

            try
            {
                PlayFabClientAPI.GetTitleNews(new GetTitleNewsRequest(),
                    result =>
                    {
                        Debug.Log("News fetched " + result.News.Count);
                        latestNews = result.News;
                        onNewsFetched?.Invoke();
                    },
                    error => Debug.LogError(error.GenerateErrorReport()));
            }
            catch (Exception) { }
        }

        private IEnumerator GetRemoteSettingsRoutine()
        {
            yield return new WaitForSeconds(.1f);

            GetRemoteSettings();
        }

        private IEnumerator StartingGameRoutine()
        {
            while (!GamePlayManager.Instance || !GamePlayManager.Instance.isBoardReady) yield return null;
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
            SWIPE_STYLE_2,
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

    }
}
