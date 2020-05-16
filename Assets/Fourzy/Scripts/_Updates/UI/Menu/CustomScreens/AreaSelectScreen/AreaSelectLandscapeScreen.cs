//@vadym udod

using Fourzy._Updates.UI.Widgets;
using System.Collections.Generic;
using UnityEngine;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tools;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class AreaSelectLandscapeScreen : MenuScreen
    {
        public AreaSelectWidgetLandscape randomArea;
        public AreaSelectWidgetLandscape comingSoonArea;
        public RectTransform widgetsParent;

        private List<AreaSelectWidgetLandscape> areaWidgets = new List<AreaSelectWidgetLandscape>();

        protected override void OnInitialized()
        {
            base.OnInitialized();

            foreach (ThemesDataHolder.GameTheme area in GameContentManager.Instance.enabledThemes)
            {
                AreaSelectWidgetLandscape newInstance = Instantiate(comingSoonArea, widgetsParent);
                newInstance.SetData(area);
                newInstance.onClick += OnWidgetSelected;

                areaWidgets.Add(newInstance);
            }

            comingSoonArea.transform.SetAsLastSibling();
        }

        public override void OnBack()
        {
            base.OnBack();

            CloseSelf();
        }

        public void PickRandom() => OnWidgetSelected(areaWidgets.Random());

        private void OnWidgetSelected(AreaSelectWidgetLandscape widget)
        {
            GameContentManager.Instance.currentTheme = widget.Data;

            //open vs screen
            menuController.OpenScreen<VSLandscapeScreen>();
        }
    }
}