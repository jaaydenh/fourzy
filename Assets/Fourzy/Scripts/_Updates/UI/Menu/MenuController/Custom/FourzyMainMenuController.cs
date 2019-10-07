//@vadym udod

using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Menu.Screens;
using PlayFab;
using PlayFab.ClientModels;
using System.Linq;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu
{
    public class FourzyMainMenuController : MenuController
    {
        public static FourzyMainMenuController instance;

        private bool onboardingFinishedEventSent = false;

        protected override void Awake()
        {
            base.Awake();

            instance = this;
        }

        protected void Update()
        {
            //try load fast puzzlePack
            if (Input.GetKeyDown(KeyCode.A))
            {
                GetScreen<PrePackPrompt>().Prompt(GameContentManager.Instance.externalPuzzlePacks.Values.First());
            }

            //if (Input.GetKeyDown(KeyCode.Q))
            //{
            //    //create turn based game
            //    PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            //    {
            //        FunctionName = "createTurnBased",

            //    }, OnResult, OnError);
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
                        MenuScreen puzzleSelectScreen = GetScreen<PuzzleSelectionScreen>();

                        if (currentScreen != puzzleSelectScreen)
                        {
                            BackToRoot();
                            OpenScreen(puzzleSelectScreen);
                        }

                        break;

                    case "tutorialProgressionMap":
                        ProgressionMapScreen progressionMapScreen = GetScreen<ProgressionMapScreen>();

                        if (currentScreen != progressionMapScreen)
                        {
                            BackToRoot();
                            progressionMapScreen.Open(GameContentManager.Instance.progressionMaps[0]);
                        }

                        break;
                }
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            //skip if tutorial running
            if (OnboardingScreen.instance && OnboardingScreen.instance.isTutorialRunning) return;

            //check for news
            if (PlayerPrefsWrapper.GetTutorialFinished(HardcodedTutorials.tutorials[0].data))
            {
                //only force news if onboarding was finished
                if (GameManager.Instance.unreadNews.Count > 0) GetScreen<NewsPromptScreen>()._Prompt();
            }
        }
    }
}