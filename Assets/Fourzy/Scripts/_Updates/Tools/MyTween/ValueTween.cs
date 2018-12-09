//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.Tween
{
    public class ValueTween : TweenBase
    {
        public float from = 0f;
        public float to = 1f;

        public override void AtProgress(float value, PlaybackDirection direction)
        {
            if (onProgress != null)
                onProgress.Invoke(Mathf.Lerp(from, to, curve.Evaluate(value)));

            if (_onProgress != null)
                _onProgress.Invoke(Mathf.Lerp(from, to, curve.Evaluate(value)));

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

        public override void OnReset()
        {
            AtProgress(0f);
        }
    }
}
