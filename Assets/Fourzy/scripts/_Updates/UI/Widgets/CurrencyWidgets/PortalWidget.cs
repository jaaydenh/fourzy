//@vadym udod

using Fourzy._Updates.UI.Helpers;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class PortalWidget : WidgetBase
    {
        public bool updateOnStart = true;
        public CurrencyWidget.CurrencyType type;

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
                case CurrencyWidget.CurrencyType.PORTAL_POINTS:
                    quantity = Mathf.FloorToInt(CurrencyWidget.ValueFromCurrencyType(type) / CurrencyWidget.PORTAL_POINTS);
                    button.GetBadge("value").badge.SetValue(quantity);

                    button.SetState(quantity > 0);
                    break;

                case CurrencyWidget.CurrencyType.RARE_PORTAL_POINTS:
                    quantity = Mathf.FloorToInt(CurrencyWidget.ValueFromCurrencyType(type) / CurrencyWidget.RARE_PORTAL_POINTS);
                    button.GetBadge("value").badge.SetValue(quantity);

                    button.SetState(quantity > 0);
                    break;
            }
        }

        public void OpenPortalScreen()
        {

        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            button = GetComponent<ButtonExtended>();
        }
    }
}