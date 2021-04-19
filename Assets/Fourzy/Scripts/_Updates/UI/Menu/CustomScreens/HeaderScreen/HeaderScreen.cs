//@vadym udod

using Fourzy._Updates.UI.Widgets;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class HeaderScreen : MenuScreen
    {
        public static HeaderScreen Instance;

        public CurrencyWidget hintsWidget;
        public CurrencyWidget gemsWidget;

        protected override void Awake()
        {
            base.Awake();

            if (!Instance) Instance = this;
        }

        public override void Close(bool animate = true)
        {
            if (!isOpened) return;

            base.Close(animate);
        }

        public override void Open()
        {
            if (isOpened) return;

            base.Open();
        }

        public void SneakABit(float duration = 3f)
        {
            CancelRoutine("hide");

            if (!isOpened)
            {
                Open();
                StartRoutine("hide", duration, () => Close());
            }
        }

        public CurrencyWidget GetCurrencyWidget(RewardType type) => GetWidgets<CurrencyWidget>().Find(widget => widget.type == GameManager.RewardToCurrency(type));

        public void OpenStore()
        {
            menuController.BackToRoot();
            MenuTabbedScreen tabbedScreen = menuController.GetOrAddScreen<MenuTabbedScreen>();

            tabbedScreen.OpenTab(0, tabbedScreen.isOpened);
        }
    }
}
