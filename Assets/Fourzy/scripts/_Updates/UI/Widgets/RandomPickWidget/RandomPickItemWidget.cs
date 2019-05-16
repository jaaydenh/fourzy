//@vadym udod

using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class RandomPickItemWidget : WidgetBase
    {
        public static int NAME_MAX_CHARS = 13;

        public TMP_Text label;

        public float offset { get; private set; }

        public void SetData(float offset,  string value)
        {
            this.offset = offset;

            label.text = value.Length > NAME_MAX_CHARS ? value.Substring(0, NAME_MAX_CHARS) + "..." : value;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            alphaTween.OnInitialized();
            scaleTween.OnInitialized();
        }
    }
}