//@vadym udod

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

        private void OnPositioningSelected()
        {
            Close();

            menuController.OpenScreen<AreaSelectLandscapeScreen>();
        }
    }
}