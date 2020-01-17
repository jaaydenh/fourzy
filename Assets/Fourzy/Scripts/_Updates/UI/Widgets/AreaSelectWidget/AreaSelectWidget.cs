//@vadym udod


using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Menu.Screens;
using TMPro;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class AreaSelectWidget : WidgetBase
    {
        public TMP_Text themeNameField;
        public Image themePreview;

        private ThemesDataHolder.GameTheme theme;

        public ToggleExtended toggleExtended { get; private set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            toggleExtended = GetComponent<ToggleExtended>();
        }

        public void SetData(ThemesDataHolder.GameTheme theme, ToggleGroup toggleGroup)
        {
            this.theme = theme;

            themeNameField.text = theme.id;
            themePreview.sprite = theme.preview;

            toggleExtended.group = toggleGroup;
        }

        public void SelectTheme()
        {
            menuScreen.menuController.GetOrAddScreen<AreaInfoPromptScreen>().Prompt(theme.themeID);
        }
    }
}