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

        private Vector2 _value;

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

            if (rectTransform)
                rectTransform.sizeDelta = _value;
            else if (spriteRenderer)
                spriteRenderer.size = _value;
        }

        public override void OnReset()
        {
            rectTransform.sizeDelta = from;
        }
    }
}