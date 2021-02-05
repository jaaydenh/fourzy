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
        public Action<AreasDataHolder.GameArea> onAreaSeleected;

        public AreaWidgetLandscape randomArea;
        public AreaWidgetLandscape comingSoonArea;
        public RectTransform widgetsParent;
        public MenuScreen toOpen;

        private List<AreaWidgetLandscape> areaWidgets = new List<AreaWidgetLandscape>();

        protected override void OnInitialized()
        {
            base.OnInitialized();

            randomArea.onClick += OnWidgetSelected;
            foreach (AreasDataHolder.GameArea area in GameContentManager.Instance.enabledAreas)
            {
                AreaWidgetLandscape newInstance = Instantiate(comingSoonArea, widgetsParent);
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

        private void OnWidgetSelected(AreaWidgetLandscape widget)
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