//@vadym udod


namespace Fourzy._Updates.UI.Widgets
{
    public class AreaSelectWidget : WidgetBase
    {
        public int themeIndex;

        public void SelectTheme()
        {
            GameContentManager.Instance.CurrentTheme = themeIndex;

            //close this screen
            menuScreen.menuController.CloseCurrentScreen();
        }
    }
}