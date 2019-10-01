//@vadym udod

using Fourzy._Updates.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class PlayerUIMessagesWidget : RoutinesBase
    {
        public PlayerUIMessageWidget widgetPrefab;
        public RectTransform widgetsParent;

        private List<PlayerUIMessageWidget> availableWidgets = new List<PlayerUIMessageWidget>();

        public void AddMessage(string message, float duration = 2f, bool dismissable = true)
        {
            PlayerUIMessageWidget widget = GetWidget();
            widget.SetData(this, message, duration, dismissable);
        }

        private PlayerUIMessageWidget GetWidget()
        {
            foreach (PlayerUIMessageWidget widget in availableWidgets) if (widget.available) return widget;

            return AddWidget();
        }

        private PlayerUIMessageWidget AddWidget() => Instantiate(widgetPrefab, widgetsParent);
    }
}