//@vadym udod

using Fourzy._Updates.UI.Widgets;
using System;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class StorePromptScreen : PromptScreen
    {
        public StoreWidget storeItemWidget;
        public WatchAdStoreWidget watchAdWidget;
        public RectTransform productsParent;

        public StoreItemType filter { get; private set; }

        public void Prompt(StoreItemType filter)
        {
            this.filter = filter;

            Prompt($"Buy {StoreItemTypeToString(filter)}", "");
        }

        protected override void OnInitialized()
        {
            if (initialized) return;

            initialized = true;

            switch (filter)
            {
                case StoreItemType.HINTS:
                    widgets.Add(Instantiate(watchAdWidget, productsParent));

                    break;
            }

            try
            {
                //add widgets
                foreach (Product product in CodelessIAPStoreListener.Instance.StoreController.products.all)
                    if (product.definition.id.IndexOf(filter.ToString(), System.StringComparison.InvariantCultureIgnoreCase) >= 0)
                        widgets.Add(Instantiate(storeItemWidget, productsParent).SetData(product));
            }
            catch (Exception) { }
        }

        public static string StoreItemTypeToString(StoreItemType filter)
        {
            switch (filter)
            {
                default:
                    return "Hints";
            }
        }

        public enum StoreItemType
        {
            HINTS,
        }
    }
}