//@vadym udod

using Fourzy._Updates._Tutorial;
using Fourzy._Updates.Managers;
using Fourzy._Updates.UI.Helpers;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class SettingsScreen : MenuScreen
    {
        public ButtonExtended puzzlesResetButton;

        public void ChangeName()
        {
            menuController.GetOrAddScreen<ChangeNamePromptScreen>()._Prompt();
        }

        public void ResetTutorial()
        {
            menuController.CloseCurrentScreen();

            PersistantMenuController.instance.GetOrAddScreen<OnboardingScreen>().OpenTutorial(HardcodedTutorials.GetByName((GameManager.Instance.Landscape ? "OnboardingLandscape" : "Onboarding")));
        }

        public void ResetPuzzles()
        {
            GameContentManager.Instance.ResetFastPuzzles();
            GameContentManager.Instance.ResetPuzzlePacks();
            GameContentManager.Instance.tokensDataHolder.ResetTokenInstructions();
        }

        public void ForceAIPresentation()
        {
            //StandaloneInputModuleExtended.instance.TriggerNoInputEvent("startDemoGame");
            GameManager.Instance.StartPresentataionGame();
        }

        public void ToggleSfx() => SettingsManager.Toggle(SettingsManager.KEY_SFX);

        public void ToggleAudio() => SettingsManager.Toggle(SettingsManager.KEY_AUDIO);

        public void ToggleDemoMode() => SettingsManager.Toggle(SettingsManager.KEY_DEMO_MODE);

        public void StartRealtime()
        {
            menuController.GetScreen<MatchmakingScreen>().OpenRealtime();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            puzzlesResetButton.SetActive(GameManager.Instance.ExtraFeatures && !GameManager.Instance.Landscape);
        }
    }
}
