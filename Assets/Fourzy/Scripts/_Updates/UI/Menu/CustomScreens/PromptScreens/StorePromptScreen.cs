//@vadym udod

using Fourzy._Updates.UI.Widgets;
using System;
using System.Linq;
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

        public static string ProductsToString(StoreItemType filter)
        {
            try
            {
                return string.Join(",", CodelessIAPStoreListener.Instance.StoreController.products.all
                    .Where(product => product.definition.id.IndexOf(filter.ToString(), System.StringComparison.InvariantCultureIgnoreCase) >= 0)
                    .Select(product => product.definition.id));
            }
            catch (Exception) { }

            return $"failed_to_fetch_products: {Application.platform}";
        }

        protected override void Awake()
        {
            base.Awake();

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