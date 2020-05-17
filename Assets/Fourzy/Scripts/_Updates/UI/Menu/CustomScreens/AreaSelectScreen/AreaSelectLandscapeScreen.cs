//@vadym udod

using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tools;
using Fourzy._Updates.UI.Widgets;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class AreaSelectLandscapeScreen : MenuScreen
    {
        public Action<ThemesDataHolder.GameTheme> onAreaSeleected;

        public AreaSelectWidgetLandscape randomArea;
        public AreaSelectWidgetLandscape comingSoonArea;
        public RectTransform widgetsParent;
        public MenuScreen toOpen;

        private List<AreaSelectWidgetLandscape> areaWidgets = new List<AreaSelectWidgetLandscape>();

        protected override void OnInitialized()
        {
            base.OnInitialized();

            randomArea.onClick += OnWidgetSelected;
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

        private void OnWidgetSelected(AreaSelectWidgetLandscape widget)
        {
            onAreaSeleected?.Invoke(widget.Data);

            if (menuController.screensStack.Contains(toOpen))
            {
                bool first = true;
                while (menuController.currentScreen != toOpen && menuController.screensStack.Count > 1)
                {
                    menuController.CloseCurrentScreen(first);
                    first = false;
                }
            }
            else
                menuController.OpenScreen(toOpen);
        }
    }
}