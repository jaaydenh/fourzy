//@vadym udod

using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.Tween
{
    [RequireComponent(typeof(Image))]
    public class FillTween : TweenBase
    {
        public float from = 0f;
        public float to = 1f;

        private Image image;

        public float _value { get; private set; }

        public override void AtProgress(float value, PlaybackDirection direction)
        {
            switch (direction)
            {
                case PlaybackDirection.FORWARD:
                    if (value < 1f)
                        _value = Mathf.Lerp(from, to, curve.Evaluate(value));
                    else
                    {
                        _value = Mathf.Lerp(from, to, curve.Evaluate(1f));
                        isPlaying = false;
                    }
                    break;
                case PlaybackDirection.BACKWARD:
                    if (value < 1f)
                        _value = Mathf.Lerp(to, from, curve.Evaluate(value));
                    else
                    {
                        _value = Mathf.Lerp(to, from, curve.Evaluate(1f));
                        isPlaying = false;
                    }
                    break;
            }

            SetFillValue(_value);
        }

        public void SetFillValue(float value)
        {
            image.fillAmount = value;
        }

        public override void OnReset()
        {
            image.fillAmount = 0f;
        }

        public override void OnInitialized()
        {
            image = GetComponent<Image>();
        }
    }
}
