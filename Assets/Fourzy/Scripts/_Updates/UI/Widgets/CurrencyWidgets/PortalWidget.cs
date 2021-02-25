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

        protected void OnDestroy()
        {
            UserManager.onCurrencyUpdate -= OnCurrencyUpdate;
        }

        public override void _Update()
        {
            if (!initialized) return;

            int quantity = 0;

            switch (type)
            {
                case CurrencyType.PORTAL_POINTS:
                    quantity = Mathf.FloorToInt(GameManager.ValueFromCurrencyType(type) / Constants.PORTAL_POINTS);

                    break;

                case CurrencyType.RARE_PORTAL_POINTS:
                    quantity = Mathf.FloorToInt(GameManager.ValueFromCurrencyType(type) / Constants.RARE_PORTAL_POINTS);

                    break;
            }

            button.GetBadge("value").badge.SetValue(quantity);
            button.SetState(quantity > 0);
        }

        public void OnTap()
        {
            switch (type)
            {
                case CurrencyType.PORTAL_POINTS:
                    PersistantMenuController.Instance.GetOrAddScreen<PortalScreen>().SetData(RewardsManager.PortalType.SIMPLE);
                    UserManager.Instance.portalPoints -= Constants.PORTAL_POINTS;

                    break;

                case CurrencyType.RARE_PORTAL_POINTS:
                    PersistantMenuController.Instance.GetOrAddScreen<PortalScreen>().SetData(RewardsManager.PortalType.RARE);
                    UserManager.Instance.rarePortalPoints -= Constants.RARE_PORTAL_POINTS;

                    break;
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            UserManager.onCurrencyUpdate += OnCurrencyUpdate;
        }

        private void OnCurrencyUpdate(CurrencyType currency)
        {
            if (currency != type) return;

            _Update();
        }
    }
}