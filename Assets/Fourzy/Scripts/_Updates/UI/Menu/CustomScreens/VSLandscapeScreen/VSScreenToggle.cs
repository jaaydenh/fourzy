//@vadym udod

using System;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class VSScreenToggle : WidgetBase
    {
        public Action<bool> onValueSet;

        public GameObject onGraphics;
        public GameObject offGraphics;
        public TMP_Text label;

        private bool state;

        public void Toggle() => SetState(!state);

        public void SetState(bool value, bool sendEvent = true)
        {
            state = value;

            UpdateVisuals(value);
            onValueSet?.Invoke(value);
        }

        public VSScreenToggle UpdateVisuals(bool value)
        {
            label.alignment = value ? TextAlignmentOptions.MidlineLeft : TextAlignmentOptions.MidlineRight;
            label.text = value ? "On" : "Off";
            onGraphics.SetActive(value);
            offGraphics.SetActive(!value);

            return this;
        }
    }
}