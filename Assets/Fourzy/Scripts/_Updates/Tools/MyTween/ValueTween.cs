//@vadym udod

using ByteSheep.Events;
using System;
using UnityEngine;

namespace Fourzy._Updates.Tween
{
    public class ValueTween : TweenBase
    {
        public float from = 0f;
        public float to = 1f;

        public AdvancedFloatEvent onValue;
        public Action<float> _onValue;

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
        }

        public void InvokeValue(float value)
        {
            onValue?.Invoke(value);
            _onValue?.Invoke(value);
        }

        public override void OnReset()
        {
            AtProgress(0f);
        }

        public override void OnInitialized() { }
    }
}
