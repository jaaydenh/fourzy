//@vadym udod

using Fourzy._Updates.Managers;
using mixpanel;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class SettingsScreen : MenuScreen
    {
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

        public void ToggleSfx() => SettingsManager.Instance.Toggle(SettingsManager.KEY_SFX);

        public void ToggleAudio() => SettingsManager.Instance.Toggle(SettingsManager.KEY_AUDIO);

        public void ToggleMoveOrigin() => SettingsManager.Instance.Toggle(SettingsManager.KEY_MOVE_ORIGIN);
    }
}
