//@vadym udod

using Fourzy._Updates._Tutorial;
using Fourzy._Updates.Managers;
using Fourzy._Updates.UI.Helpers;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class SettingsScreen : MenuScreen
    {
        public ButtonExtended puzzlesResetButton;
        public ButtonExtended fullscreenButton;

        public void ChangeName()
        {
            menuController.GetOrAddScreen<ChangeNamePromptScreen>()._Prompt();
        }

        public void ResetTutorial()
        {
            menuController.CloseCurrentScreen();

            PersistantMenuController.Instance
                .GetOrAddScreen<OnboardingScreen>()
                .OpenTutorial(HardcodedTutorials.GetByName((GameManager.Instance.Landscape ? "OnboardingLandscape" : "Onboarding")));
        }

        public void ResetPuzzles()
        {
            menuController.GetOrAddScreen<PromptScreen>().Prompt(
                "Reset Progress",
                "All puzzles progress will be removed",
                LocalizationManager.Value("yes"),
                LocalizationManager.Value("no"),
                () =>
                {
                    GameContentManager.Instance.ResetFastPuzzles();
                    GameContentManager.Instance.ResetPuzzlePacks();
                    GameContentManager.Instance.tokensDataHolder.ResetTokenInstructions();
                },
                null)
                .CloseOnAccept()
                .CloseOnDecline();
        }

        public void ForceAIPresentation()
        {
            //StandaloneInputModuleExtended.instance.TriggerNoInputEvent("startDemoGame");
            GameManager.Instance.StartPresentataionGame();
        }

        public void ToggleSfx() => SettingsManager.Toggle(SettingsManager.KEY_SFX);

        public void ToggleAudio() => SettingsManager.Toggle(SettingsManager.KEY_AUDIO);

        public void ToggleDemoMode() => SettingsManager.Toggle(SettingsManager.KEY_DEMO_MODE);

        public void StartRealtime() => menuController.GetScreen<MatchmakingScreen>().OpenRealtime();

        public void ShowFriendsScreen() => menuController.GetOrAddScreen<FriendsScreen>().Prompt();

        public void ToggleFullscreen() => Screen.fullScreen = !Screen.fullScreen;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            puzzlesResetButton.SetActive(GameManager.Instance.ExtraFeatures && !GameManager.Instance.Landscape);

#if !UNITY_STANDALONE
            if (fullscreenButton) fullscreenButton.SetActive(false);
#endif
        }
    }
}
