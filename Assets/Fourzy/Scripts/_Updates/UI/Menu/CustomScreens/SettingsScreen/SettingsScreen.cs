//@vadym udod

using Fourzy._Updates.Managers;
using Fourzy._Updates.UI.Helpers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class SettingsScreen : MenuScreen
    {
        public ButtonExtended puzzlesResetButton;

        protected override void Start()
        {
            base.Start();

            puzzlesResetButton.SetActive(GameManager.Instance.ExtraFeatures);
        }

        public void ChangeName()
        {
            menuController.GetScreen<ChangeNamePromptScreen>().Prompt("Change Name", "Current name: " + UserManager.Instance.userName, () => { menuController.CloseCurrentScreen(); });
        }

        public void ResetTutorial()
        {
            PersistantMenuController.instance.GetScreen<OnboardingScreen>().OpenOnboarding(GameContentManager.Instance.GetTutorial("Onboarding"));
            MenuController.AddMenuEvent(Constants.MAIN_MENU_CANVAS_NAME, new KeyValuePair<string, object>("openScreen", "puzzlesScreen"));
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
    }
}
