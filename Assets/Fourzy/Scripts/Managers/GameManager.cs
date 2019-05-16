//

using Firebase;
using Firebase.RemoteConfig;
using Firebase.Storage;
using Fourzy._Updates.Audio;
using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Threading;
using Fourzy._Updates.UI.Menu;
using Fourzy._Updates.UI.Menu.Screens;
using Fourzy._Updates.Tools;
using mixpanel;
using MoreMountains.NiceVibrations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using FourzyGameModel.Model;
using Newtonsoft.Json;
using System.IO;

namespace Fourzy
{
    [UnitySingleton(UnitySingletonAttribute.Type.ExistsInScene)]
    public class GameManager : UnitySingleton<GameManager>
    {
        public static Action<string> onDailyChallengeFileName;

        public static Dictionary<string, object> APP_REMOTE_SETTINGS_DEFAULTS;

        [Header("Display tutorials?")]
        public bool displayTutorials = true;
        [Header("Display tutorial even if it was already displayed")]
        public bool forceDisplayTutorials = true;
        public bool showInfoToasts = true;

        public PuzzleData dailyPuzzlePack { get; private set; }
        public IClientFourzy activeGame { get; set; }
        public DependencyStatus dependencyStatus;

        private bool configFetched = false;

        public bool isMainMenuLoaded
        {
            get
            {
                //load main menu if needed
                bool mainMenuLoaded = false;
                for (int sceneIndex = 0; sceneIndex < SceneManager.sceneCount; sceneIndex++)
                    if (SceneManager.GetSceneAt(sceneIndex).name == Constants.MAIN_MENU_SCENE_NAME)
                    {
                        mainMenuLoaded = true;
                        break;
                    }

                return mainMenuLoaded;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if (InstanceExists) return;

#if UNITY_IOS
            MMVibrationManager.iOSInitializeHaptics();
#endif

            NetworkAccess.Initialize(DEBUG: true);

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            //to modify manifest file
            bool value = false;
            if (value) Handheld.Vibrate();

            APP_REMOTE_SETTINGS_DEFAULTS = new Dictionary<string, object>()
            {
                [Constants.KEY_APP_VERSION] = Application.version,
                [Constants.KEY_DAILY_PUZZLE] = "",
            };

            FirebaseRemoteConfig.SetDefaults(APP_REMOTE_SETTINGS_DEFAULTS);
        }

        protected void Start()
        {
            Mixpanel.Track("Game Started");
#if UNITY_IOS
            UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
            UnityEngine.iOS.NotificationServices.ClearRemoteNotifications();
#endif
            //init  threadqueuer
            ThreadsQueuer.Instance.QueueFuncToExecuteFromMainThread(null);

            NetworkAccess.onNetworkAccess += OnNetworkAccess;
        }

        protected void OnDestory()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;

            NetworkAccess.onNetworkAccess -= OnNetworkAccess;

#if UNITY_IOS
            MMVibrationManager.iOSReleaseHaptics ();
#endif
        }

        public void StartGame(IClientFourzy game)
        {
            switch (game._Type)
            {
                case GameType.TURN_BASED:
                    Debug.Log($"Starting challenge, id: {game.GameID}");
                    break;
            }

            //if gameplay scene is already opened, just load game
            if (activeGame != null)
            {
                GamePlayManager.instance.LoadGame(game);
            }
            else
            {
                if (isMainMenuLoaded)
                    SceneManager.LoadScene(Constants.GAMEPLAY_SCENE_NAME, LoadSceneMode.Additive);
                else
                    SceneManager.LoadScene(Constants.GAMEPLAY_SCENE_NAME);
            }

            activeGame = game;
        }

        public void OpenMainMenu()
        {
            //unload gameplay scene 
            if (activeGame != null)
                GamePlayManager.instance.UnloadGamePlayScreen();

            if (!isMainMenuLoaded)
                SceneManager.LoadScene(Constants.MAIN_MENU_SCENE_NAME);
        }

        public static void Vibrate(HapticTypes type) => MMVibrationManager.Haptic(type);

        public static void Vibrate() => Vibrate(HapticTypes.Success);

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            switch (scene.name)
            {
                case Constants.GAMEPLAY_SCENE_NAME:
                    MenuController.SetState("MainMenuCanvas", false);

                    AudioHolder.instance.StopBGAudio(AudioTypes.BG_MAIN_MENU, .5f);
                    break;
            }
        }

        private void OnSceneUnloaded(Scene scene)
        {
            switch (scene.name)
            {
                case Constants.GAMEPLAY_SCENE_NAME:
                    MenuController.SetState("MainMenuCanvas", true);

                    AudioHolder.instance.PlayBGAudio(AudioTypes.BG_MAIN_MENU, true, 1f, 3f);
                    break;
            }
        }

        private void GetRomoteSettings()
        {
            Task fetchTask = FirebaseRemoteConfig.FetchAsync(TimeSpan.Zero);
            fetchTask.ContinueWith(FetchComplete);
        }

        private bool VersionCheck()
        {
            string value = FirebaseRemoteConfig.GetValue(Constants.KEY_APP_VERSION).StringValue;

            Debug.Log($"Version checker: {Application.version} app version, {value} required app version");

            //version checker
            if (Application.version != value)
            {
                //show version popup
                PersistantMenuController.instance.GetScreen<PromptScreen>().Prompt(
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

        private void DailyPuzzleCheck()
        {
            string value = FirebaseRemoteConfig.GetValue(Constants.KEY_DAILY_PUZZLE).StringValue;

            Debug.Log($"Old Daily Puzzle file '{PlayerPrefsWrapper.GetDailyPuzzleFileName()}', new '{value}'");

            //filename check
            if (PlayerPrefsWrapper.GetDailyPuzzleFileName() != value)
            {
                PlayerPrefsWrapper.SetDailyPuzzleFileName(value);

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

        private void DisplayDailyPuzzlePopup()
        {
            dailyPuzzlePack = JsonConvert.DeserializeObject<PuzzleData>(File.ReadAllText(Application.persistentDataPath + "/" + PlayerPrefsWrapper.GetDailyPuzzleFileName()));

            if (dailyPuzzlePack != null)
            {
                //new daily puzzle available


                //show daily puzzle popup
                PersistantMenuController.instance.GetScreen<PromptScreen>().Prompt(
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

                    Debug.Log($"Remote data loaded and ready (last fetch time {info.FetchTime}).");

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
                        });
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.ToString());
                    }

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
                FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
                {
                    dependencyStatus = task.Result;
                    if (dependencyStatus == DependencyStatus.Available)
                    {
                        GetRomoteSettings();
                    }
                    else
                    {
                        Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
                    }
                });
            }
        }

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
    }
}
