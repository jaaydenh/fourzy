//@vadym udod


using Fourzy._Updates.Serialized;
using Fourzy._Updates.UI.Widgets;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class AreaSelectScreen : MenuScreen
    {
        public AreaWidget themeSelectWidget;
        public RectTransform themesParent;

        public ToggleGroup group { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            group = GetComponent<ToggleGroup>();
        }

        public override void Open()
        {
            base.Open();

            HeaderScreen.Instance.Close();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            foreach (AreasDataHolder.GameArea theme in GameContentManager.Instance.enabledAreas)
            {
                AreaWidget widgetInstance = Instantiate(themeSelectWidget, themesParent);

                themesParent.localScale = Vector3.one;

                widgetInstance.SetData(theme, group);

                if (theme == GameContentManager.Instance.currentArea)
                    widgetInstance.toggleExtended.isOn = true;
            }
        }
    }
}
