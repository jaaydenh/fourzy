//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.Tween
{
    public class BezierPositionTween : TweenBase
    {
        public Vector2 from;
        public Vector2 to;
        public Vector2 control;

        private RectTransform rectTransform;

        protected override void Awake()
        {
            base.Awake();

            rectTransform = GetComponent<RectTransform>();
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
                            rectTransform.anchoredPosition = GetBezierPoint(from, from + control, to, curve.Evaluate(value));
                        else
                            transform.localPosition = GetBezierPoint(from, from + control, to, curve.Evaluate(value));
                    }
                    else
                    {
                        isPlaying = false;
                        
                        if (rectTransform)
                            rectTransform.anchoredPosition = GetBezierPoint(from, from + control, to, curve.Evaluate(1f));
                        else
                            transform.localPosition = GetBezierPoint(from, from + control, to, curve.Evaluate(1f));
                    }
                    break;

                case PlaybackDirection.BACKWARD:
                    if (value < 1f)
                    {
                        if (rectTransform)
                            rectTransform.anchoredPosition = GetBezierPoint(from, from + control, to, curve.Evaluate(1f - value));
                        else
                            transform.localPosition = GetBezierPoint(from, from + control, to, curve.Evaluate(1f - value));
                    }
                    else
                    {
                        isPlaying = false;

                        if (rectTransform)
                            rectTransform.anchoredPosition = GetBezierPoint(from, from + control, to, curve.Evaluate(0f));
                        else
                            transform.localPosition = GetBezierPoint(from, from + control, to, curve.Evaluate(0f));
                    }
                    break;
            }
        }

        public static Vector2 GetBezierPoint(Vector2 start, Vector2 control, Vector2 end, float time)
        {
            Vector2 t1 = Vector2.Lerp(start, control, time);
            Vector2 t2 = Vector2.Lerp(control, end, time);

            return Vector2.Lerp(t1, t2, time);
        }

        public override void OnReset()
        {
            if (rectTransform)
                rectTransform.anchoredPosition = from;
            else
                transform.localPosition = from;
        }
    }
}