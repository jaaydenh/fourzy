//@vadym udod

using Fourzy._Updates._Tutorial;
using Fourzy._Updates.UI.Helpers;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class PlayScreen : MenuTab
    {
        [SerializeField]
        private ButtonExtended discordButton;
        [SerializeField]
        private RectTransform body;
        [SerializeField]
        private bool gauntletGameUnlocked = false;

        private OnIPhoneX onIPhoneX;
        private bool fastPuzzlesUnlocked;

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
                //.OpenTutorial(HardcodedTutorials.GetByName((GameManager.Instance.Landscape ? "OnboardingLandscape" : "Onboarding")));
                .OpenTutorial(HardcodedTutorials.GetByName("Onboarding"));

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

        public void OpenDiscord()
        {
            discordButton.GetBadge("attention").badge.Hide();
            PlayerPrefs.SetInt("discord_button_pressed", 1);
            GameManager.Instance.OpenDiscordPage();
        }

        public void OpenOnlineLobby()
        {
            GameManager.Instance.OpenLobbyGame();
        }

        public void StartRealtimeQuickmatch()
        {
            GameManager.Instance.StartRealtimeQuickGame();
        }

        public void ChangeName() => menuController.GetOrAddScreen<ChangeNamePromptScreen>()._Prompt();

        protected override void OnInitialized()
        {
            base.OnInitialized();

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

            discordButton
                .GetBadge("attention")
                .badge
                .SetState(PlayerPrefs.GetInt("discord_button_pressed", 0) == 0);
        }
    }
}