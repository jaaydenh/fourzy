//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.Tween
{
    public class VolumeTween : TweenBase
    {
        [HideInInspector]
        public AudioSource audioSource;

        public float from;
        public float to;

        protected override void Awake()
        {
            base.Awake();

            audioSource = GetComponent<AudioSource>();
        }

        public override void OnReset()
        {
            audioSource.volume = from;
        }

        public override void AtProgress(float value, PlaybackDirection direction)
        {
            base.AtProgress(value, direction);

            switch (direction)
            {
                case PlaybackDirection.FORWARD:
                    if (value < 1f)
                        audioSource.volume = Mathf.Lerp(from, to, curve.Evaluate(value));
                    else
                    {
                        isPlaying = false;

                        audioSource.volume = Mathf.Lerp(from, to, curve.Evaluate(1f));
                    }
                    break;
                case PlaybackDirection.BACKWARD:
                    if (value < 1f)
                        audioSource.volume = Mathf.Lerp(to, from, curve.Evaluate(value));
                    else
                    {
                        isPlaying = false;

                        audioSource.volume = Mathf.Lerp(to, from, curve.Evaluate(1f));
                    }
                    break;
            }
        }
    }

}