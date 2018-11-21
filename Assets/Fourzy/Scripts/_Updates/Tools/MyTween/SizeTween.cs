//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.Tween
{
    public class SizeTween : TweenBase
    {
        public Vector2 from;
        public Vector2 to;

        private RectTransform rectTransform;

        protected void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public override void AtProgress(float value, PlaybackDirection direction)
        {
            base.AtProgress(value, direction);

            switch (direction)
            {
                case PlaybackDirection.FORWARD:
                    if (value < 1f)
                        rectTransform.sizeDelta = Vector2.Lerp(from, to, curve.Evaluate(value));
                    else
                    {
                        isPlaying = false;
                        rectTransform.sizeDelta = Vector2.Lerp(from, to, curve.Evaluate(1f));
                    }
                    break;
                case PlaybackDirection.BACKWARD:
                    if (value < 1f)
                        rectTransform.sizeDelta = Vector2.Lerp(to, from, curve.Evaluate(value));
                    else
                    {
                        isPlaying = false;
                        rectTransform.sizeDelta = Vector2.Lerp(to, from, curve.Evaluate(1f));
                    }
                    break;
            }
        }

        public override void OnReset()
        {
            rectTransform.sizeDelta = from;
        }
    }
}