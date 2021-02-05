//@vadym udod

using Fourzy._Updates.Serialized;
using System;
using TMPro;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class AreaWidgetLandscape : WidgetBase
    {
        public Action<AreaWidgetLandscape> onClick;

        public TMP_Text label;
        public Image icon;

        private LocalizedText localizedText;

        public AreasDataHolder.GameArea Data { get; private set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            localizedText = GetComponentInChildren<LocalizedText>();
        }

        protected void Start()
        {
            localizedText.enabled = true;
        }

        public AreaWidgetLandscape SetData(AreasDataHolder.GameArea data)
        {
            Data = data;

            if (data != null)
            {
                localizedText.key = data.name;
                icon.sprite = data._4X3;
            }

            return this;
        }

        public void OnClick() => onClick?.Invoke(this);
    }
}
