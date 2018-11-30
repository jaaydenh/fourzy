//@vadym udod

using mixpanel;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class SettingsScreen : MenuScreen
    {
        public void ChangeName()
        {
            menuController.GetPrompt<ChangeNamePromptScreen>().Prompt("Change Name", "Current name: " + UserManager.Instance.userName, () => { menuController.CloseCurrentScreen(false); });
        }

        public void ResetTutorial()
        {
            Mixpanel.Track("Reset Tutorial Button Press");

            PlayerPrefs.DeleteKey("onboardingStage");
            PlayerPrefs.DeleteKey("onboardingStep");
        }
    }
}
