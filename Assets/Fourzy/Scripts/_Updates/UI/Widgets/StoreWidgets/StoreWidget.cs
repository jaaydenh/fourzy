//@vadym udod

using Fourzy._Updates.Serialized;
using TMPro;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class StoreWidget : WidgetBase
    {
        public TMP_Text price;
        public TMP_Text description;
        public Image icon;

        public Product data { get; private set; }

        public StoreWidget SetData(Product data)
        {
            this.data = data;

            MiscGameContentHolder.StoreItemExtraData extraData = GameContentManager.Instance.miscGameDataHolder.GetStoreItem(data.definition.id);
            icon.sprite = extraData.icon;
            price.text = data.metadata.isoCurrencyCode + data.metadata.localizedPriceString;
            description.text = data.metadata.localizedTitle;

            return this;
        }

        public void OnTap()
        {
            CodelessIAPStoreListener.Instance.InitiatePurchase(data.definition.id);
        }
    }
}