//@vadym udod

using Fourzy._Updates.Managers;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Helpers;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class SettingsScreen : MenuScreen
    {
        public ButtonExtended puzzlesResetButton;

        public void ChangeName()
        {
            menuController.GetScreen<ChangeNamePromptScreen>()._Prompt();
        }

        public void ResetTutorial()
        {
            menuController.CloseCurrentScreen();
            PersistantMenuController.instance.GetScreen<OnboardingScreen>().OpenTutorial(HardcodedTutorials.tutorials[0]);
            //MenuController.AddMenuEvent(Constants.MAIN_MENU_CANVAS_NAME, new KeyValuePair<string, object>("openScreen", "puzzlesScreen"));
        }

        public void ResetPuzzles()
        {
            GameContentManager.Instance.ResetPuzzlePacks();
            GameContentManager.Instance.tokensDataHolder.ResetTokenInstructions();
        }

        public void ForceAIPresentation()
        {
            //StandaloneInputModuleExtended.instance.TriggerNoInputEvent("startDemoGame");
            GameManager.Instance.StartPresentataionGame();
        }

        public void ToggleSfx() => SettingsManager.Instance.Toggle(SettingsManager.KEY_SFX);

        public void ToggleAudio() => SettingsManager.Instance.Toggle(SettingsManager.KEY_AUDIO);

        public void ToggleDemoMode() => SettingsManager.Instance.Toggle(SettingsManager.KEY_DEMO_MODE);

        protected override void OnInitialized()
        {
            base.OnInitialized();

            puzzlesResetButton.SetActive(GameManager.Instance.ExtraFeatures);
        }
    }
}
