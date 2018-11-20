//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.Tween
{
    public class PositionTween : TweenBase
    {
        public Vector3 from;
        public Vector3 to;

        private RectTransform rectTransform;

        protected void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void FromCurrentPosition()
        {
            from = transform.localPosition;
        }

        public override void AtProgress(float value, PlaybackDirection direction)
        {
            base.AtProgress(value, direction);

            switch (direction)
            {
                case PlaybackDirection.FORWARD:
                    if (value < 1f)
                    {
                        if (rectTransform)
                            rectTransform.anchoredPosition = Vector3.Lerp(from, to, curve.Evaluate(value));
                        else
                            transform.localPosition = Vector3.Lerp(from, to, curve.Evaluate(value));
                    }
                    else
                    {
                        isPlaying = false;

                        if (rectTransform)
                            rectTransform.anchoredPosition = Vector3.Lerp(from, to, curve.Evaluate(1f));
                        else
                            transform.localPosition = Vector3.Lerp(from, to, curve.Evaluate(1f));
                    }
                    break;
                case PlaybackDirection.BACKWARD:
                    if (value < 1f)
                    {
                        if (rectTransform)
                            rectTransform.anchoredPosition = Vector3.Lerp(to, from, curve.Evaluate(value));
                        else
                            transform.localPosition = Vector3.Lerp(to, from, curve.Evaluate(value));
                    }
                    else
                    {
                        isPlaying = false;

                        if (rectTransform)
                            rectTransform.anchoredPosition = Vector3.Lerp(to, from, curve.Evaluate(1f));
                        else
                            transform.localPosition = Vector3.Lerp(to, from, curve.Evaluate(1f));
                    }
                    break;
            }
        }

        public override void OnReset()
        {
            transform.localPosition = from;
        }
    }
}