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
        public float from;
        public float to;

        private List<GraphicsColorGroup> alphaGroup = new List<GraphicsColorGroup>();

        protected void Awake()
        {
            if (customObjects.Length != 0)
                foreach (Transform obj in customObjects)
                    Parse(obj);
            else
                Parse(transform);
        }

        public override void AtProgress(float value, PlaybackDirection direction)
        {
            base.AtProgress(value, direction);

            switch (direction)
            {
                case PlaybackDirection.FORWARD:
                    if (value < 1f)
                    {
                        foreach (GraphicsColorGroup group in alphaGroup)
                        {
                            if (group.canvasGroup)
                                group.canvasGroup.alpha = Mathf.Lerp(from, to, curve.Evaluate(value));
                            else if (group.uiGraphics)
                                group.uiGraphics.color = new Color(group.uiGraphics.color.r, group.uiGraphics.color.g, group.uiGraphics.color.b, Mathf.Lerp(from, to, curve.Evaluate(value)));
                            else if (group.spriteRenderer)
                                group.spriteRenderer.color = new Color(group.spriteRenderer.color.r, group.spriteRenderer.color.g, group.spriteRenderer.color.b, Mathf.Lerp(from, to, curve.Evaluate(value)));
                        }
                    }
                    else
                    {
                        isPlaying = false;

                        foreach (GraphicsColorGroup group in alphaGroup)
                        {
                            if (group.canvasGroup)
                                group.canvasGroup.alpha = Mathf.Lerp(from, to, curve.Evaluate(1f));
                            else if (group.uiGraphics)
                                group.uiGraphics.color = new Color(group.uiGraphics.color.r, group.uiGraphics.color.g, group.uiGraphics.color.b, Mathf.Lerp(from, to, curve.Evaluate(1f)));
                            else if (group.spriteRenderer)
                                group.spriteRenderer.color = new Color(group.spriteRenderer.color.r, group.spriteRenderer.color.g, group.spriteRenderer.color.b, Mathf.Lerp(from, to, curve.Evaluate(1f)));
                        }
                    }
                    break;
                case PlaybackDirection.BACKWARD:
                    if (value < 1f)
                    {
                        foreach (GraphicsColorGroup group in alphaGroup)
                        {
                            if (group.canvasGroup)
                                group.canvasGroup.alpha = Mathf.Lerp(to, from, curve.Evaluate(value));
                            else if (group.uiGraphics)
                                group.uiGraphics.color = new Color(group.uiGraphics.color.r, group.uiGraphics.color.g, group.uiGraphics.color.b, Mathf.Lerp(to, from, curve.Evaluate(value)));
                            else if (group.spriteRenderer)
                                group.spriteRenderer.color = new Color(group.spriteRenderer.color.r, group.spriteRenderer.color.g, group.spriteRenderer.color.b, Mathf.Lerp(to, from, curve.Evaluate(value)));
                        }
                    }
                    else
                    {
                        isPlaying = false;

                        foreach (GraphicsColorGroup group in alphaGroup)
                        {
                            if (group.canvasGroup)
                                group.canvasGroup.alpha = Mathf.Lerp(to, from, curve.Evaluate(1f));
                            else if (group.uiGraphics)
                                group.uiGraphics.color = new Color(group.uiGraphics.color.r, group.uiGraphics.color.g, group.uiGraphics.color.b, Mathf.Lerp(to, from, curve.Evaluate(1f)));
                            else if (group.spriteRenderer)
                                group.spriteRenderer.color = new Color(group.spriteRenderer.color.r, group.spriteRenderer.color.g, group.spriteRenderer.color.b, Mathf.Lerp(to, from, curve.Evaluate(1f)));
                        }
                    }
                    break;
            }
        }

        public override void OnReset()
        {
            foreach (GraphicsColorGroup group in alphaGroup)
            {
                if (group.canvasGroup)
                    group.canvasGroup.alpha = from;
                else if (group.uiGraphics)
                    group.uiGraphics.color = new Color(1f, 1f, 1f, from);
                else if (group.spriteRenderer)
                    group.spriteRenderer.color = new Color(1f, 1f, 1f, from);
            }
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
    }
}