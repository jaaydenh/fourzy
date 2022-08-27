//@vadym udod

using Fourzy._Updates._Tutorial;
using Fourzy._Updates.Managers;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class SettingsScreen : MenuScreen
    {
        public void ChangeName()
        {
            menuController.GetOrAddScreen<ChangeNamePromptScreen>()._Prompt();
        }

        public void ResetTutorial()
        {
            menuController.CloseCurrentScreen();

            AnalyticsManager.Instance.LogEvent("tutorialReplay");

            PersistantMenuController.Instance
                .GetOrAddScreen<OnboardingScreen>()
                .OpenTutorial(HardcodedTutorials.GetByName(
                    GameManager.Instance.Landscape ? 
                        "OnboardingLandscape" : 
                        "Onboarding"));
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
                    GameManager.Instance.ResetGames(false);
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

        // Audio is for Music
        public void ToggleAudio() => SettingsManager.Toggle(SettingsManager.KEY_AUDIO);

        public void ToggleDemoMode() => SettingsManager.Toggle(SettingsManager.KEY_DEMO_MODE);

        public void ShowFriendsScreen() => menuController.GetOrAddScreen<FriendsScreen>().Prompt();

        public void ToggleFullscreen() => Screen.fullScreen = !Screen.fullScreen;

        public void ToggleMagic() => SettingsManager.Toggle(SettingsManager.KEY_REALTIME_MAGIC);

        public void ToggleRealtimeTimer() => SettingsManager.Toggle(SettingsManager.KEY_REALTIME_TIMER);

        public void ToggleVibration() => SettingsManager.Toggle(SettingsManager.KEY_VIBRATION);

        public void ToggleTokenInstructions() => SettingsManager.Toggle(SettingsManager.KEY_TOKEN_INSTRUCTION);

        public void OpenSkillzRules()
        {
            menuController.GetOrAddScreen<SkillzRulesScreen>()._OpenScreen();
        }
    }
}
