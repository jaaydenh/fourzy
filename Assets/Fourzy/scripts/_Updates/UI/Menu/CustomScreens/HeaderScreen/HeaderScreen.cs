//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class HeaderScreen : MenuScreen
    {
        protected override void Start()
        {
            base.Start();

            isOpened = canvasGroup.alpha > 0f;
        }

        public override void Close(bool animate = true)
        {
            if (!isOpened) return;

            base.Close(animate);
        }

        public void OpenStore()
        {
            menuController.BackToRoot();
            MenuTabbedScreen tabbedScreen = menuController.GetScreen<MenuTabbedScreen>();

            tabbedScreen.OpenTab(0, tabbedScreen.isOpened);
        }
    }
}
