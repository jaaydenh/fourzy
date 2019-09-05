//@vadym udod

using Fourzy._Updates.Mechanics.Rewards;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Menu;
using Fourzy._Updates.UI.Menu.Screens;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class PortalWidget : WidgetBase
    {
        public bool updateOnStart = true;
        public CurrencyType type;

        private ButtonExtended button;

        protected void Start()
        {
            if (updateOnStart) _Update();
        }

        public override void _Update()
        {
            int quantity = 0;

            switch (type)
            {
                case CurrencyType.PORTAL_POINTS:
                    quantity = Mathf.FloorToInt(CurrencyWidget.ValueFromCurrencyType(type) / Constants.PORTAL_POINTS);
                    button.GetBadge("value").badge.SetValue(quantity);

                    button.SetState(quantity > 0);
                    //button.SetState(true);
                    break;

                case CurrencyType.RARE_PORTAL_POINTS:
                    quantity = Mathf.FloorToInt(CurrencyWidget.ValueFromCurrencyType(type) / Constants.RARE_PORTAL_POINTS);
                    button.GetBadge("value").badge.SetValue(quantity);

                    button.SetState(quantity > 0);
                    break;
            }
        }

        public void OpenPortalScreen()
        {
            switch (type)
            {
                case CurrencyType.PORTAL_POINTS:
                    PersistantMenuController.instance.GetScreen<PortalScreen>().SetData(RewardsManager.PortalType.SIMPLE);

                    break;

                case CurrencyType.RARE_PORTAL_POINTS:
                    PersistantMenuController.instance.GetScreen<PortalScreen>().SetData(RewardsManager.PortalType.RARE);

                    break;
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            button = GetComponent<ButtonExtended>();
        }
    }
}