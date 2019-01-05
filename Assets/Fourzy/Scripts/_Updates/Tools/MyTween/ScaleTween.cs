//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.Tween
{
    public class ScaleTween : TweenBase
    {
        public Vector3 from = Vector3.one;
        public Vector3 to = Vector3.one;

        public Vector3 _value { get; private set; }

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

            SetScale(_value);
        }

        public void SetScale(Vector3 scale)
        {
            transform.localScale = scale;
        }

        public override void OnReset()
        {
            transform.localScale = from;
        }
    }
}