//@vadym udod

using Fourzy._Updates.Tools;
using Fourzy._Updates.UI.Menu.Screens;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu
{
    public class FourzyMainMenuController : MenuController
    {
        private bool onboardingFinishedEventSent = false;

        protected void OnEnable()
        {
            //onboarding finished event
            if (GameContentManager.Instance.GetTutorial("Onboarding").wasFinishedThisSession && !onboardingFinishedEventSent)
            {
                AnalyticsManager.Instance.LogTutorialEvent("Onboarding", "10");
                onboardingFinishedEventSent = true;
            }
        }

        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                //create turn based game
                PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
                {
                    FunctionName = "createTurnBased",

                }, OnResult, OnError);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                //create turn based game
                PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
                {
                    FunctionName = "helloWorld",
                    FunctionParameter = "-input-",

                }, OnResult, OnError);
            }
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