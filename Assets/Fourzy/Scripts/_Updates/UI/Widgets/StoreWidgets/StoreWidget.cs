//@vadym udod

using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tools;
using TMPro;
using UnityEngine;
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
            
#if UNITY_IOS || UNITY_EDITOR
            description.text = data.metadata.localizedTitle;
#elif UNITY_ANDROID 
            description.text = data.metadata.localizedDescription;
#endif
            return this;
        }

        public Vector2 IconViewportPosition() => icon.rectTransform.GetViewportPosition();

        public void OnTap()
        {
            CodelessIAPStoreListener.Instance.InitiatePurchase(data.definition.id);
        }
    }
}