//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.Tween
{
    public class SquishStretchTween : TweenBase
    {
        public float xAxisPower = -1f;
        public float yAxisPower = 1f;

        public override void AtProgress(float value, PlaybackDirection direction)
        {
            switch (direction)
            {
                case PlaybackDirection.FORWARD:
                    if (value < 1f)
                    {
                        transform.localScale = Vector3.one + new Vector3(curve.Evaluate(value) * xAxisPower, curve.Evaluate(value) * yAxisPower);
                    }
                    else
                    {
                        isPlaying = false;

                        transform.localScale = Vector3.one + new Vector3(curve.Evaluate(1f) * xAxisPower, curve.Evaluate(1f) * yAxisPower);
                    }
                    break;

                case PlaybackDirection.BACKWARD:
                    if (value < 1f)
                    {
                        transform.localScale = Vector3.one + new Vector3(curve.Evaluate(1f - value) * xAxisPower, curve.Evaluate(1f - value) * yAxisPower);
                    }
                    else
                    {
                        isPlaying = false;

                        transform.localScale = Vector3.one + new Vector3(curve.Evaluate(0f) * xAxisPower, curve.Evaluate(0f) * yAxisPower);
                    }
                    break;
            }
        }

        public override void OnReset()
        {
            transform.localScale = Vector3.one;
        }

        public override void OnInitialized() { }
    }
}
