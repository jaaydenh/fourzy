//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.Tween
{
    public class RotationTween : TweenBase
    {
        public Vector3 from;
        public Vector3 to;

        public override void PlayForward(bool resetValue)
        {
            if (from.z < 0f && to.z > 0f)
                to.z -= 360f;
            else if (to.z < 0f && from.z > 0f)
                from.z -= 360f;

            base.PlayForward(resetValue);
        }

        public override void AtProgress(float value, PlaybackDirection direction)
        {
            base.AtProgress(value, direction);

            switch (direction)
            {
                case PlaybackDirection.FORWARD:
                    if (value < 1f)
                    {
                        transform.localEulerAngles = Vector3.Lerp(from, to, curve.Evaluate(value));
                    }
                    else
                    {
                        isPlaying = false;

                        transform.localEulerAngles = Vector3.Lerp(from, to, curve.Evaluate(1f));
                    }
                    break;
                case PlaybackDirection.BACKWARD:
                    if (value < 1f)
                        transform.localEulerAngles = Vector3.Lerp(to, from, curve.Evaluate(value));
                    else
                    {
                        isPlaying = false;
                        transform.localEulerAngles = Vector3.Lerp(to, from, curve.Evaluate(1f));
                    }
                    break;
            }
        }

        public override void OnReset()
        {
            transform.localEulerAngles = from;
        }
    }
}