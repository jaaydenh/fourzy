//@vadym udod


using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Menu.Screens;
using TMPro;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class AreaWidget : WidgetBase
    {
        public TMP_Text themeNameField;
        public Image themePreview;

        private AreasDataHolder.GameArea area;

        public ToggleExtended toggleExtended { get; private set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            toggleExtended = GetComponent<ToggleExtended>();
        }

        public void SetData(AreasDataHolder.GameArea area, ToggleGroup toggleGroup)
        {
            this.area = area;

            themeNameField.text = area.name;
            themePreview.sprite = area._16X9;

            toggleExtended.group = toggleGroup;
        }

        public void SelectTheme()
        {
            menuScreen.menuController.GetOrAddScreen<AreaInfoPromptScreen>().Prompt(area.areaID);
        }
    }
}