//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.Tween
{
    public class ValueTween : TweenBase
    {
        public float from = 0f;
        public float to = 1f;

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
                        isPlaying = false;

                        _value = Mathf.Lerp(from, to, curve.Evaluate(1f));
                    }
                    break;

                case PlaybackDirection.BACKWARD:
                    if (value < 1f)
                        _value = Mathf.Lerp(to, from, curve.Evaluate(value));
                    else
                    {
                        isPlaying = false;

                        _value = Mathf.Lerp(to, from, curve.Evaluate(1f));
                    }
                    break;
            }

            InvokeValue(_value);

            if (value >= 1f)
                switch (repeat)
                {
                    case RepeatType.ZERO_TO_ONE:
                        if (direction == PlaybackDirection.FORWARD)
                            PlayForward(true);
                        else
                            PlayBackward(true);
                        break;
                    case RepeatType.PING_PONG:
                        Toggle(true);
                        break;
                }
        }

        public void InvokeValue(float value)
        {
            if (onProgress != null)
                onProgress.Invoke(value);

            if (_onProgress != null)
                _onProgress.Invoke(value);
        }

        public override void OnReset()
        {
            AtProgress(0f);
        }
    }
}
