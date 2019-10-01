//@vadym udod

using Fourzy._Updates.Mechanics.Rewards;
using Fourzy._Updates.UI.Menu;
using Fourzy._Updates.UI.Menu.Screens;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class PortalWidget : WidgetBase
    {
        public CurrencyType type;

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

        public void OnTap()
        {
            switch (type)
            {
                case CurrencyType.PORTAL_POINTS:
                    PersistantMenuController.instance.GetScreen<PortalScreen>().SetData(RewardsManager.PortalType.SIMPLE);
                    UserManager.Instance.portalPoints -= Constants.PORTAL_POINTS;

                    break;

                case CurrencyType.RARE_PORTAL_POINTS:
                    PersistantMenuController.instance.GetScreen<PortalScreen>().SetData(RewardsManager.PortalType.RARE);
                    UserManager.Instance.portalPoints -= Constants.RARE_PORTAL_POINTS;

                    break;
            }
        }
    }
}