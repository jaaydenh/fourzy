//@vadym udod

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.Tween
{
    public class AlphaTween : TweenBase
    {
        public Transform[] customObjects;
        public bool propagate = false;      //do the same for all child objects?
        public float from = 0f;
        public float to = 1f;

        public List<GraphicsColorGroup> alphaGroup { get; private set; }

        public float _value { get; private set; }

        public override void AtProgress(float value, PlaybackDirection direction)
        {
            float newValue = 0f;

            switch (direction)
            {
                case PlaybackDirection.FORWARD:
                    if (value < 1f)
                        newValue = Mathf.Lerp(from, to, curve.Evaluate(value));
                    else
                    {
                        newValue = Mathf.Lerp(from, to, curve.Evaluate(1f));
                        isPlaying = false;
                    }
                    break;

                case PlaybackDirection.BACKWARD:
                    if (value < 1f)
                        newValue = Mathf.Lerp(to, from, curve.Evaluate(value));
                    else
                    {
                        newValue = Mathf.Lerp(to, from, curve.Evaluate(1f));
                        isPlaying = false;
                    }
                    break;
            }

            SetAlpha(newValue);
        }

        public void SetAlpha(float value)
        {
            _value = value;
            foreach (GraphicsColorGroup group in alphaGroup) group.alpha = value;
        }

        public override void OnReset()
        {
            SetAlpha(from);
        }

        public void DoParse()
        {
            alphaGroup = new List<GraphicsColorGroup>();

            if (customObjects != null && customObjects.Length != 0)
                foreach (Transform obj in customObjects)
                    Parse(obj);
            else
                Parse(transform);
        }

        public void Parse(Transform obj)
        {
            SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
            MaskableGraphic uiGraphics = obj.GetComponent<MaskableGraphic>();
            CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();

            alphaGroup.Add(new GraphicsColorGroup() { spriteRenderer = spriteRenderer, uiGraphics = uiGraphics, canvasGroup = canvasGroup });

            if (propagate)
                for (int i = 0; i < obj.childCount; i++)
                    Parse(obj.GetChild(i));
        }

        public override void OnInitialized()
        {
            DoParse();

            if (alphaGroup.Count > 0) _value = alphaGroup[0].alpha;
        }
    }
}