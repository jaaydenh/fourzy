//@vadym udod

using Fourzy._Updates.Serialized;
using System;
using TMPro;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class AreaSelectWidgetLandscape : WidgetBase
    {
        public Action<AreaSelectWidgetLandscape> onClick;

        public TMP_Text label;
        public Image icon;

        private LocalizedText localizedText;

        public ThemesDataHolder.GameTheme Data { get; private set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            localizedText = GetComponentInChildren<LocalizedText>();
        }

        protected void Start()
        {
            localizedText.enabled = true;
        }

        public AreaSelectWidgetLandscape SetData(ThemesDataHolder.GameTheme data)
        {
            Data = data;

            localizedText.key = data.id;
            icon.sprite = data.landscapePreview;

            return this;
        }

        public void OnClick() => onClick?.Invoke(this);
    }
}
