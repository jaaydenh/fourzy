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
        public bool playOnAwake = false;
        public RepeatType repeat = RepeatType.NONE;
        public QuickFloatEvent onProgress;

        //for instances where event need to be assigned from code
        public Action<float> _onProgress;

        public float value { get; protected set; }
        public float defaultPlaybackTime { get; protected set; }
        public bool isPlaying { get; protected set; }
        public bool initialized { get; protected set; }

        protected virtual void Awake()
        {
            Initialize();
        }

        protected virtual void Update()
        {
            if (!isPlaying)
                return;

            value = Mathf.Clamp01(value + ((unscaledTime) ? Time.unscaledDeltaTime : Time.deltaTime) / playbackTime);

            Progress(value, playbackDirection);
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

            Progress(value, playbackDirection);
        }

        public virtual void PlayBackward(bool resetValue)
        {
            if (resetValue)
                value = 0f;
            else
            {
                if (value != 0f)
                    value = 1f - value;
            }

            isPlaying = true;
            playbackDirection = PlaybackDirection.BACKWARD;

            Progress(value, playbackDirection);
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

        public virtual void ResetPlaybackTime()
        {
            playbackTime = defaultPlaybackTime;
        }

        public virtual void AtProgress(Single value)
        {
            AtProgress(value, playbackDirection);
        }

        public virtual void Progress(Single value, PlaybackDirection direction)
        {
            Initialize();

            onProgress?.Invoke((direction == PlaybackDirection.FORWARD) ? value : 1f - value);
            _onProgress?.Invoke((direction == PlaybackDirection.FORWARD) ? value : 1f - value);

            AtProgress(value, direction);

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

        public void Initialize()
        {
            if (initialized) return;

            initialized = true;
            defaultPlaybackTime = playbackTime;

            OnInitialized();

            if (playOnAwake)
                switch (playbackDirection)
                {
                    case PlaybackDirection.BACKWARD:
                        PlayBackward(true);
                        break;

                    case PlaybackDirection.FORWARD:
                        PlayForward(true);
                        break;
                }
        }

        public abstract void OnReset();
        public abstract void OnInitialized();
        public abstract void AtProgress(Single value, PlaybackDirection direction);
    }

    public class GraphicsColorGroup
    {
        public CanvasGroup canvasGroup;
        public MaskableGraphic uiGraphics;
        public SpriteRenderer spriteRenderer;
        public LineRenderer lineRenderer;

        public float alpha
        {
            set
            {
                if (canvasGroup)
                    canvasGroup.alpha = value;
                else if (uiGraphics)
                    uiGraphics.color = new Color(uiGraphics.color.r, uiGraphics.color.g, uiGraphics.color.b, value);
                else if (spriteRenderer)
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, value);
                else if (lineRenderer)
                    lineRenderer.startColor = lineRenderer.endColor = new Color(lineRenderer.startColor.r, lineRenderer.startColor.g, lineRenderer.startColor.b, value);
            }
            get
            {
                if (canvasGroup) return canvasGroup.alpha;
                else if (uiGraphics) return uiGraphics.color.a;
                else if (spriteRenderer) return spriteRenderer.color.a;
                else if (lineRenderer) return lineRenderer.startColor.a;

                return 0f;
            }
        }

        public Color color
        {
            get
            {
                if (uiGraphics) return uiGraphics.color;
                else if (spriteRenderer) return spriteRenderer.color;
                else if (lineRenderer) return lineRenderer.startColor;

                return Color.clear;
            }
        }

        public void SetColor(Color _color, bool changeAlpha)
        {
            if (uiGraphics)
                uiGraphics.color = changeAlpha ? _color : new Color(_color.r, _color.g, _color.b, uiGraphics.color.a);
            else if (spriteRenderer)
                spriteRenderer.color = changeAlpha ? _color : new Color(_color.r, _color.g, _color.b, spriteRenderer.color.a);
            else if (lineRenderer)
                lineRenderer.startColor = lineRenderer.endColor = changeAlpha ? _color : new Color(_color.r, _color.g, _color.b, lineRenderer.startColor.a);
        }
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