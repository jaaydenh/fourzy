//@vadym udod

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class HeaderScreen : MenuScreen
    {
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

        protected override void OnInitialized()
        {
            base.OnInitialized();

            isOpened = canvasGroup.alpha > 0f;
        }
    }
}
