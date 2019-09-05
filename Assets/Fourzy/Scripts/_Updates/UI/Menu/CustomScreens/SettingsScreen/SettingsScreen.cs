//@vadym udod

using Fourzy._Updates.Managers;
using Fourzy._Updates.UI.Helpers;
using mixpanel;
using UnityEngine;

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
            Mixpanel.Track("Reset Tutorial Button Press");

            menuController.GetScreen<PromptScreen>().Prompt("Reset Tutorial", "Replay tutorial next time the game is opened?", "Yes", "No", () =>
            {
                GameContentManager.Instance.ResetOnboarding();

                menuController.CloseCurrentScreen();
            });
        }

        public void ResetPuzzles()
        {
            GameContentManager.Instance.puzzlePacksDataHolder.ResetPuzzlesPlayerPrefs();
            GameContentManager.Instance.puzzlePacksDataHolder.ResetPuzzlePacksPlayerPrefs();
            GameContentManager.Instance.tokensDataHolder.ResetTokenInstructions();
        }

        public void ToggleSfx() => SettingsManager.Instance.Toggle(SettingsManager.KEY_SFX);

        public void ToggleAudio() => SettingsManager.Instance.Toggle(SettingsManager.KEY_AUDIO);

        public void ToggkeDemoMode() => SettingsManager.Instance.Toggle(SettingsManager.KEY_DEMO_MODE);
    }
}
