//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.Tween
{
    public class SizeTween : TweenBase
    {
        public Vector2 from;
        public Vector2 to;

        private RectTransform rectTransform;
        private SpriteRenderer spriteRenderer;

        public Vector2 _value { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            rectTransform = GetComponent<RectTransform>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public override void AtProgress(float value, PlaybackDirection direction)
        {
            base.AtProgress(value, direction);

            switch (direction)
            {
                case PlaybackDirection.FORWARD:
                    if (value < 1f)
                        _value = Vector2.Lerp(from, to, curve.Evaluate(value));
                    else
                    {
                        isPlaying = false;
                        _value = Vector2.Lerp(from, to, curve.Evaluate(1f));
                    }
                    break;
                case PlaybackDirection.BACKWARD:
                    if (value < 1f)
                        _value = Vector2.Lerp(to, from, curve.Evaluate(value));
                    else
                    {
                        isPlaying = false;
                        _value = Vector2.Lerp(to, from, curve.Evaluate(1f));
                    }
                    break;
            }

            SetSize(_value);
        }

        public void SetSize(Vector2 size)
        {
            if (rectTransform)
                rectTransform.sizeDelta = size;
            else if (spriteRenderer)
                spriteRenderer.size = size;
        }

        public override void OnReset()
        {
            rectTransform.sizeDelta = from;
        }
    }
}