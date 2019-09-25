//@vadym udod

using Fourzy._Updates.UI.Menu.Screens;
using PlayFab;
using PlayFab.ClientModels;
using System.Linq;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu
{
    public class FourzyMainMenuController : MenuController
    {
        private bool onboardingFinishedEventSent = false;

        protected override void Start()
        {
            base.Start();

            //onboarding finished event
            if (GameContentManager.Instance.GetTutorial("Onboarding").wasFinishedThisSession && !onboardingFinishedEventSent)
            {
                AnalyticsManager.Instance.LogTutorialEvent("Onboarding", "10");
                onboardingFinishedEventSent = true;
            }

            //check for news
            if (PlayerPrefsWrapper.GetTutorialFinished(GameContentManager.Instance.GetTutorial("Onboarding").data))
            {
                //only force news if onboarding was finished
                if (GameManager.Instance.unreadNews.Count > 0) GetScreen<NewsPromptScreen>()._Prompt();
            }
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
                }
            }
        }
    }
}