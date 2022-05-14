//@vadym udod

using Fourzy._Updates._Tutorial;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class InfinityTableMainScreen : MenuScreen
    {
        public override void OnBack()
        {
            base.OnBack();

            GameManager.Instance.CloseApp();
        }

        public void PlayLocal()
        {
            menuController.GetScreen<PlayerPositioningPromptScreen>()._Prompt(OnPositioningSelected);
        }

        public void ResetTutorial()
        {
            PersistantMenuController.Instance
                .GetOrAddScreen<OnboardingScreen>()
                .OpenTutorial(HardcodedTutorials.GetByName("Onboarding"));
        }
        public void StartTutorialAdventure()
        {
            menuController.GetOrAddScreen<ProgressionMapScreen>().Open(GameContentManager.Instance.progressionMaps[0]);
        }

        private void OnPositioningSelected()
        {
            Close();

            menuController.OpenScreen<AreaSelectLandscapeScreen>();
        }
    }
}