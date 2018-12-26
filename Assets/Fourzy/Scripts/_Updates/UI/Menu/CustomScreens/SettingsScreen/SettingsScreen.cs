//@vadym udod

using mixpanel;
using UnityEngine;

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

            menuController.GetScreen<PromptScreen>().Prompt("Clear Tutorial", "Clear tutorail data?", "Yes", "No", () =>
            {
                PlayerPrefs.DeleteKey("onboardingStage");
                PlayerPrefs.DeleteKey("onboardingStep");

                menuController.CloseCurrentScreen();
            });
        }
    }
}
