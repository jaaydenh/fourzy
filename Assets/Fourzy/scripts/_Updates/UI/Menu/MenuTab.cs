//@vadym udod


using Fourzy._Updates.UI.Widgets;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu
{
    public class MenuTab : MenuScreen
    {
        protected MenuTabbedScreen tabsParent;

        protected override void Awake()
        {
            base.Awake();

            tabsParent = GetComponentInParent<MenuTabbedScreen>();

            AdjustSize();
        }

        public void AdjustSize()
        {
            if (Application.isPlaying && layoutElement)
                layoutElement.minWidth = menuController.widthAdjusted;
            else
                GetComponent<LayoutElement>().minWidth = GetComponentInParent<MenuController>()._widthAdjusted;
        }

        public override bool isCurrent => tabsParent?.IsCurrentTab(this) ?? false;

        public override bool IsWidgetVisible(WidgetBase widget) => base.IsWidgetVisible(widget) && isCurrent;
    }
}