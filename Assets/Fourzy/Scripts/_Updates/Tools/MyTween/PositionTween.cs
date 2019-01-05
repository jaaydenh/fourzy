//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.Tween
{
    public class PositionTween : TweenBase
    {
        public Vector3 from;
        public Vector3 to;

        private RectTransform rectTransform;

        public Vector3 _value { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            rectTransform = GetComponent<RectTransform>();
        }

        public void FromCurrentPosition()
        {
            if (rectTransform)
                from = rectTransform.anchoredPosition;
            else
                from = transform.localPosition;
        }

        public override void AtProgress(float value, PlaybackDirection direction)
        {
            base.AtProgress(value, direction);

            switch (direction)
            {
                case PlaybackDirection.FORWARD:
                    if (value < 1f)
                        _value = Vector3.Lerp(from, to, curve.Evaluate(value));
                    else
                    {
                        isPlaying = false;

                        _value = Vector3.Lerp(from, to, curve.Evaluate(1f));
                    }
                    break;
                case PlaybackDirection.BACKWARD:
                    if (value < 1f)
                        _value = Vector3.Lerp(to, from, curve.Evaluate(value));
                    else
                    {
                        isPlaying = false;

                        _value = Vector3.Lerp(to, from, curve.Evaluate(1f));
                    }
                    break;
            }

            SetPosition(_value);
        }

        public void SetPosition(Vector3 position)
        {
            if (rectTransform)
                rectTransform.anchoredPosition = position;
            else
                transform.localPosition = position;
        }

        public override void OnReset()
        {
            transform.localPosition = from;
        }
    }
}