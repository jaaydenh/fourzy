//@vadym udod

using Fourzy._Updates.UI.Widgets;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class StorePromptScreen : PromptScreen
    {
        public StoreWidget storeItemWidget;
        public WatchAdStoreWidget watchAdWidget;
        public RectTransform productsParent;

        public ProductCatalog catalog { get; private set; }
        public StoreItemType filter { get; private set; }

        protected bool initialized = false;

        public void Prompt(StoreItemType filter)
        {
            this.filter = filter;

            Initialize();

            Prompt($"Buy {StoreItemTypeToString(filter)}", "");
        }

        protected void Initialize()
        {
            if (initialized) return;

            initialized = true;

            catalog = ProductCatalog.LoadDefaultCatalog();

            switch (filter)
            {
                case StoreItemType.HINTS:
                    widgets.Add(Instantiate(watchAdWidget, productsParent));

                    break;
            }

            //add widgets
            foreach (Product product in CodelessIAPStoreListener.Instance.StoreController.products.all)
                if (product.definition.id.IndexOf(filter.ToString(), System.StringComparison.InvariantCultureIgnoreCase) >= 0)
                    widgets.Add(Instantiate(storeItemWidget, productsParent).SetData(product));
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