//@vadym udod

using Fourzy._Updates.Audio;
using Fourzy._Updates.UI.Menu.Screens;
using FourzyGameModel.Model;
using System.Collections.Generic;

namespace Fourzy._Updates.UI.Menu
{
    public class FourzyMainMenuController : MenuController
    {
        public static FourzyMainMenuController instance;

        private MatchmakingScreen matchmakingScreen;

        private List<ProgressionReward> progressionRewardsQueue = new List<ProgressionReward>();

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
#if !MOBILE_SKILLZ
                //only force news if onboarding was finished
                if (GameManager.Instance.unreadNews.Count > 0)
                {
                    GetOrAddScreen<NewsPromptScreen>()._Prompt();
                }
#endif
            }
        }
    }
}