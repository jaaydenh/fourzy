//@vadym udod

using Fourzy._Updates.UI.Menu.Screens;
using Fourzy._Updates.UI.Toasts;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class ChangeThemeWidget : WidgetBase
    {
        public Image areaImage;

        private AreaSelectScreen areaSelectScreen;

        protected void Start()
        {
            areaSelectScreen = menuScreen.menuController.GetScreen<AreaSelectScreen>();

            PlayerPrefsWrapper.onThemeChanged += OnThemeChanged;
            OnThemeChanged(PlayerPrefsWrapper.GetCurrentTheme());
        }

        protected void OnDestroy()
        {
            PlayerPrefsWrapper.onThemeChanged -= OnThemeChanged;
        }

        public void ChangeTheme()
        {
            //open AreaSelect screen
            if (!areaSelectScreen)
            {
                GamesToastsController.ShowToast(GamesToastsController.ToastStyle.ACTION_WARNING, "Break devs arms!\nHe forgot to add area change\nscreen");
                return;
            }

            menuScreen.menuController.OpenScreen(areaSelectScreen);
        }

        private void OnThemeChanged(int theme)
        {
            areaImage.sprite = GameContentManager.Instance.gameThemes[theme].preview;
        }
    }
}