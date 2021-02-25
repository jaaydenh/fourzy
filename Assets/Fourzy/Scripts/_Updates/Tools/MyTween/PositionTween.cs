//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.Tween
{
    public class PositionTween : TweenBase
    {
        public Vector3 from;
        public Vector3 to;

        public bool local = true;

        private RectTransform rectTransform;

        public Vector3 _value { get; private set; }

        public void FromCurrentPosition()
        {
            if (rectTransform)
                from = rectTransform.anchoredPosition;
            else
                from = transform.localPosition;
        }

        public override void AtProgress(float value, PlaybackDirection direction)
        {
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
            if (local)
            {
                if (rectTransform)
                    rectTransform.anchoredPosition = position;
                else
                    transform.localPosition = position;
            }
            else
            {
                transform.position = position;
            }
        }

        public override void OnReset()
        {
            transform.localPosition = from;
        }

        public override void OnInitialized()
        {
            rectTransform = GetComponent<RectTransform>();
        }
    }
}