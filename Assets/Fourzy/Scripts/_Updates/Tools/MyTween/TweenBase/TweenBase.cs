//@vadym udod

using ByteSheep.Events;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.Tween
{
    public abstract class TweenBase : MonoBehaviour
    {
        public PlaybackDirection playbackDirection = PlaybackDirection.FORWARD;
        public AnimationCurve curve;
        public bool unscaledTime = false;
        public float playbackTime = .3f;
        public RepeatType repeat = RepeatType.NONE;
        public QuickFloatEvent onProgress;

        //for instances where event need to be assigned from code
        public Action<float> _onProgress;

        [HideInInspector]
        public float defaultPlaybackTime;
        [HideInInspector]
        public float value = 0f;
        [HideInInspector]
        public bool isPlaying = false;

        protected virtual void Awake()
        {
            defaultPlaybackTime = playbackTime;
        }

        protected virtual void Update()
        {
            if (!isPlaying)
                return;

            value = Mathf.Clamp01(value + ((unscaledTime) ? Time.unscaledDeltaTime : Time.deltaTime) / playbackTime);

            AtProgress(value, playbackDirection);
        }

        public virtual void PlayForward(bool resetValue)
        {
            if (resetValue)
                value = 0f;
            else
            {
                if (value != 0)
                    value = 1f - value;
            }

            isPlaying = true;
            playbackDirection = PlaybackDirection.FORWARD;

            AtProgress(value, playbackDirection);
        }

        public virtual void PlayBackward(bool resetValue)
        {
            if (resetValue)
                value = 0f;
            else
            {
                if (value != 0)
                    value = 1f - value;
            }

            isPlaying = true;
            playbackDirection = PlaybackDirection.BACKWARD;

            AtProgress(value, playbackDirection);
        }

        public virtual void Toggle(bool reset)
        {
            switch (playbackDirection)
            {
                case PlaybackDirection.BACKWARD:
                    PlayForward(reset);
                    break;
                case PlaybackDirection.FORWARD:
                    PlayBackward(reset);
                    break;
            }
        }

        public virtual void StopTween(bool reset)
        {
            isPlaying = false;

            if (reset)
                OnReset();
        }

        public virtual void AtProgress(Single value)
        {
            AtProgress(value, playbackDirection);
        }

        public virtual void ResetPlaybackTime()
        {
            playbackTime = defaultPlaybackTime;
        }

        public virtual void AtProgress(Single value, PlaybackDirection direction)
        {
            if (onProgress != null)
                onProgress.Invoke(value);

            if (_onProgress != null)
                _onProgress.Invoke(value);

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

        public abstract void OnReset();
    }

    public class GraphicsColorGroup
    {
        public CanvasGroup canvasGroup;
        public SpriteRenderer spriteRenderer;
        public MaskableGraphic uiGraphics;
        public LineRenderer lineRenderer;
    }

    public enum PlaybackDirection
    {
        FORWARD,
        BACKWARD,
    }

    public enum RepeatType
    {
        NONE,
        ZERO_TO_ONE,
        PING_PONG,
    }
}