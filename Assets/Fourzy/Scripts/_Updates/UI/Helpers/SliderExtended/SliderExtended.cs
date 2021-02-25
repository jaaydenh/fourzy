//@vadym udod

using ByteSheep.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Helpers
{
    public class SliderExtended : Slider
    {
        [HideInInspector]
        public QuickStringEvent onValueChangeString;
        [HideInInspector]
        public string format = "{0}";

        protected override void Awake()
        {
            base.Awake();

            if (!Application.isPlaying)
                return;

            onValueChanged.AddListener(OnValueChanged);
        }

        protected void OnValueChanged(float value)
        {
            onValueChangeString.Invoke(string.Format(format, value));
        }

        public void SetFillColor(Color color)
        {
            if (fillRect && fillRect.GetComponent<Image>())
                fillRect.GetComponent<Image>().color = color;
        }

        public void SetMinMaxValue(float min, float max, float value)
        {
            minValue = min;
            maxValue = max;
            this.value = value;
        }

        public void SetMaxValue(float max, float value)
        {
            maxValue = max;
            this.value = value;
        }

        public void SetMinMaxValue(int min, int max, int value)
        {
            minValue = min;
            maxValue = max;
            this.value = value;
        }

        public void SetMaxValue(int max, int value)
        {
            maxValue = max;
            this.value = value;
        }
    }
}