//@vadym udod

using Fourzy._Updates._Tutorial;
using Fourzy._Updates.UI.Helpers;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class PlayScreen : MenuTab
    {
        private const string kLobbyScreenOpened = "lobbyScreenOpened";

        public RectTransform body;
        public RectTransform portalsHolder;
        public ButtonExtended fastPuzzleButton;
        public ButtonExtended gauntletGameButton;

        private MatchmakingScreen matchmakingScreen;
        private OnIPhoneX onIPhoneX;

        private bool fastPuzzlesUnlocked;
        private bool gauntletGameUnlocked;

        public override void Open()
        {
            base.Open();

            fastPuzzlesUnlocked = 
                PlayerPrefsWrapper.GetRewardRewarded("unlock_fast_puzzles_mode") || 
                GameManager.Instance.defaultPuzzlesState;
            fastPuzzleButton.GetBadge("locked").badge.SetState(!fastPuzzlesUnlocked);

            gauntletGameUnlocked = 
                PlayerPrefsWrapper.GetRewardRewarded("unlock_gauntlet_mode") || 
                GameManager.Instance.defaultGauntletState;

            gauntletGameButton.GetBadge("locked").badge.SetState(!gauntletGameUnlocked);
        }

        public override void OnBack()
        {
            base.OnBack();

            if (!tabsParent)
            {
                Application.Quit();
                Debug.Log("App close");
            }
        }

        public void StartGauntletAIPack()
        {
            //check
            if (!gauntletGameUnlocked)
            {
                menuController.GetOrAddScreen<PromptScreen>().Prompt(LocalizationManager.Value("gauntlet_title"),
                    LocalizationManager.Value("gauntlet_desc"),
                    LocalizationManager.Value("ok"),
                    null,
                    () => menuController.CloseCurrentScreen(),
                    null);
                return;
            }

            menuController.GetOrAddScreen<GauntletIntroScreen>()._Prompt();
        }

        public void StartTutorialAdventure() => menuController.GetOrAddScreen<ProgressionMapScreen>().Open(GameContentManager.Instance.progressionMaps[0]);

        public void ResetTutorial() =>
            PersistantMenuController.Instance.GetOrAddScreen<OnboardingScreen>()
                .OpenTutorial(HardcodedTutorials.GetByName((GameManager.Instance.Landscape ? 
                    "OnboardingLandscape" : 
                    "Onboarding")));

        public void OpenFastPuzzleScreen()
        {
            //check
            if (!fastPuzzlesUnlocked)
            {
                menuController.GetOrAddScreen<PromptScreen>()
                    .Prompt(LocalizationManager.Value("puzzle_Ladder_title"),
                        LocalizationManager.Value("puzzle_Ladder_desc"),
                        LocalizationManager.Value("ok"),
                        null,
                        () => menuController.CloseCurrentScreen(),
                        null);
                return;
            }

            tabsParent.menuController.OpenScreen<FastPuzzlesScreen>();
        }

        public void OpenNews() => menuController.GetOrAddScreen<NewsPromptScreen>()._Prompt();

        public void OpenDiscord() => GameManager.Instance.OpenDiscordPage();

        public void OpenOnlineLobby()
        {
            if (PlayerPrefsWrapper.GetBool(kLobbyScreenOpened))
            {
                menuController.GetScreen<LobbyScreen>().CheckLobby();
            }
            else
            {
                menuController.GetOrAddScreen<ChangeNamePromptScreen>()._Prompt();

                PlayerPrefsWrapper.SetBool(kLobbyScreenOpened, true);
            }
        }

        public void StartRealtimeQuickmatch() => menuController.GetScreen<MatchmakingScreen>().OpenRealtime();

        protected override void OnInitialized()
        {
            base.OnInitialized();

            matchmakingScreen = menuController.GetOrAddScreen<MatchmakingScreen>();
            if (body)
            {
                onIPhoneX = body.GetComponent<OnIPhoneX>();
            }
            if (onIPhoneX)
            {
                onIPhoneX.CheckPlatform();
            }

            //force open
            if (GameManager.Instance.Landscape && @default)
            {
                Open();
            }
        }
    }
}