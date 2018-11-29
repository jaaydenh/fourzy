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

        protected override void Awake()
        {
            base.Awake();

            image = GetComponent<Image>();
        }

        public override void AtProgress(float value, PlaybackDirection direction)
        {
            base.AtProgress(value, direction);

            switch (direction)
            {
                case PlaybackDirection.FORWARD:
                    if (value < 1f)
                        image.fillAmount = Mathf.Lerp(from, to, curve.Evaluate(value));
                    else
                    {
                        isPlaying = false;

                        image.fillAmount = Mathf.Lerp(from, to, curve.Evaluate(1f));
                    }
                    break;
                case PlaybackDirection.BACKWARD:
                    if (value < 1f)
                        image.fillAmount = Mathf.Lerp(to, from, curve.Evaluate(value));
                    else
                    {
                        isPlaying = false;

                        image.fillAmount = Mathf.Lerp(to, from, curve.Evaluate(1f));
                    }
                    break;
            }
        }

        public override void OnReset()
        {
            image.fillAmount = 0f;
        }
    }
}
