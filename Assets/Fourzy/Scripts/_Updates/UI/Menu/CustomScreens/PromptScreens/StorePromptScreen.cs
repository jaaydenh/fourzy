//@vadym udod

using Fourzy._Updates.Serialized;
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

        protected void OnDestroy()
        {
            GameManager.onPurchaseComplete -= OnPurchaseComplete;
        }

        public void Prompt(StoreItemType filter)
        {
            this.filter = filter;

            Prompt($"{LocalizationManager.Value("buy")} {StoreItemTypeToString(filter)}", "");
        }

        public static string ProductsToString(StoreItemType filter)
        {
            try
            {
                //return string.Join(",", CodelessIAPStoreListener.Instance.StoreController.products.all
                //    .Where(product => product.definition.id.IndexOf(filter.ToString(), System.StringComparison.InvariantCultureIgnoreCase) >= 0)
                //    .Select(product => product.definition.id));
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
                ////add widgets
                //foreach (Product product in CodelessIAPStoreListener.Instance.StoreController.products.all)
                //    if (product.definition.id.IndexOf(filter.ToString(), System.StringComparison.InvariantCultureIgnoreCase) >= 0)
                //        widgets.Add(Instantiate(storeItemWidget, productsParent).SetData(product));
            }
            catch (Exception) { }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            GameManager.onPurchaseComplete += OnPurchaseComplete;
        }

        public void OnPurchaseComplete(Product product)
        {
            //try get product data
            MiscGameContentHolder.StoreItemExtraData _data = GameContentManager.Instance.miscGameDataHolder.GetStoreItem(product.definition.id);

            if (!_data) return;

            if (product.definition.id.Contains("hints"))
            {
                StoreWidget targetWidget = GetWidgets<StoreWidget>().Find(widget => widget.data == product);

                //animate reward
                PersistantOverlayScreen.instance.AnimateReward(false, RewardType.HINTS, _data.quantity, targetWidget.IconViewportPosition());
            }
        }

        public static string StoreItemTypeToString(StoreItemType filter)
        {
            switch (filter)
            {
                default:
                    return LocalizationManager.Value("hints");
            }
        }

        public enum StoreItemType
        {
            HINTS,
        }
    }
}