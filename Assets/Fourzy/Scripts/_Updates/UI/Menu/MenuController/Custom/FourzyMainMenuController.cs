//@vadym udod

using Fourzy._Updates._Tutorial;
using Fourzy._Updates.Audio;
using Fourzy._Updates.UI.Menu.Screens;
using Newtonsoft.Json;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu
{
    public class FourzyMainMenuController : MenuController
    {
        public static FourzyMainMenuController instance;

        private MatchmakingScreen matchmakingScreen;

        protected override void Awake()
        {
            base.Awake();

            instance = this;
        }

        protected override void Start()
        {
            base.Start();

            matchmakingScreen = GetScreen<MatchmakingScreen>();

            //play bg audio
            if (!AudioHolder.instance.IsBGAudioPlaying("bg_main_menu"))
            {
                AudioHolder.instance.PlayBGAudio("bg_main_menu", true, .75f, 1f);
            }
        }

        protected void Update()
        {
            ////try load fast puzzlePack
            //if (Input.GetKeyDown(KeyCode.A))
            //{
            //    //GetScreen<PrePackPrompt>().Prompt(GameContentManager.Instance.externalPuzzlePacks.Values.First());
            //    PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            //    {
            //        FunctionName = "updateFastPuzzlesStat",
            //        FunctionParameter = new { value = 5 },
            //        GeneratePlayStreamEvent = true,
            //    },
            //    (result) => { Debug.Log($"Fast puzzles stat updated {5}"); },
            //    (error) => { Debug.LogError(error.ErrorMessage); });
            //}
            //else if (Input.GetKeyDown(KeyCode.W))
            //{
            //    //create turn based game
            //    PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            //    {
            //        FunctionName = "helloWorld",
            //        FunctionParameter = "-input-",

            //    }, OnResult, OnError);
            //}

            //if (Input.GetKeyDown(KeyCode.X))
            //{
            //    GameManager.Instance.StartGame(GameContentManager.Instance.GetFastPuzzle("ENCHANTED_FOREST_Puzzle_39_WinIn2"));
            //}

            //if (Input.GetKeyDown(KeyCode.Q))
            //{
            //    PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            //    {
            //        FunctionName = "getFastPuzzleLB",
            //        FunctionParameter = new { maxCount = 6, }
            //    }, OnResult, OnError);
            //}
        }

        private void OnResult(ExecuteCloudScriptResult result)
        {
            print("good: " + result.ToJson());
        }

        private void OnError(PlayFabError error)
        {
            print("bad: " + error.ToString());
        }

        protected override void OnInvokeMenuEvents(MenuEvents events)
        {
            if (events.data.ContainsKey("openScreen"))
            {
                switch (events.data["openScreen"].ToString())
                {
                    case "puzzlesScreen":
                        MenuScreen puzzleSelectScreen = GetOrAddScreen<PuzzleSelectionScreen>();

                        if (currentScreen != puzzleSelectScreen)
                        {
                            BackToRoot();
                            OpenScreen(puzzleSelectScreen);
                        }

                        break;

                    case "tutorialProgressionMap":
                        ProgressionMapScreen progressionMapScreen = GetOrAddScreen<ProgressionMapScreen>();

                        if (currentScreen != progressionMapScreen)
                        {
                            BackToRoot();
                            progressionMapScreen.Open(GameContentManager.Instance.progressionMaps[0]);
                        }

                        break;
                }
            }

            if (currentScreen == matchmakingScreen)
            {
                matchmakingScreen.CloseSelf(false);
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            //skip if tutorial running
            if (OnboardingScreen.Instance && OnboardingScreen.Instance.isTutorialRunning) return;

            //check for news
            if (PlayerPrefsWrapper.GetTutorialFinished("Onboarding"))
            {
                //only force news if onboarding was finished
                if (GameManager.Instance.unreadNews.Count > 0)
                {
                    GetOrAddScreen<NewsPromptScreen>()._Prompt();
                }
            }
        }
    }
}