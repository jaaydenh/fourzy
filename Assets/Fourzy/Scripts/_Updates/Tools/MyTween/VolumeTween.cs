//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.Tween
{
    public class VolumeTween : TweenBase
    {
        [HideInInspector]
        public AudioSource audioSource;

        public float from = 0f;
        public float to = 1f;

        public float _value { get; private set; }

        public override void OnReset()
        {
            audioSource.volume = from;
        }

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

            SetVolume(_value);
        }

        public void SetVolume(float value)
        {
            audioSource.volume = value;
        }

        public override void OnInitialized()
        {
            audioSource = GetComponent<AudioSource>();
        }
    }
}