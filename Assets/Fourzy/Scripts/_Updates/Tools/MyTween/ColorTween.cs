//@vadym udod

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.Tween
{
    public class ColorTween : TweenBase
    {
        public Transform[] customObjects;
        public bool propagate = false;      //do the same for all child objects?
        public Color from;
        public Color to;

        private List<GraphicsColorGroup> spriteColorGroups;

        protected void Awake()
        {
            TryParse();
        }

        public override void PlayForward(bool resetValue)
        {
            TryParse();

            base.PlayForward(resetValue);
        }

        public override void PlayBackward(bool resetValue)
        {
            TryParse();

            base.PlayBackward(resetValue);
        }

        public override void AtProgress(float value, PlaybackDirection direction)
        {
            base.AtProgress(value, direction);

            switch (direction)
            {
                case PlaybackDirection.FORWARD:
                    if (value < 1f)
                    {
                        foreach (GraphicsColorGroup group in spriteColorGroups)
                        {
                            if (group.uiGraphics)
                                group.uiGraphics.color = Color.Lerp(from, to, curve.Evaluate(value));
                            else if (group.spriteRenderer)
                                group.spriteRenderer.color = Color.Lerp(from, to, curve.Evaluate(value));
                            else if (group.lineRenderer)
                                group.lineRenderer.startColor = Color.Lerp(from, to, curve.Evaluate(value));
                        }
                    }
                    else
                    {
                        isPlaying = false;

                        foreach (GraphicsColorGroup group in spriteColorGroups)
                        {
                            if (group.uiGraphics)
                                group.uiGraphics.color = Color.Lerp(from, to, curve.Evaluate(1f));
                            else if (group.spriteRenderer)
                                group.spriteRenderer.color = Color.Lerp(from, to, curve.Evaluate(1f));
                            else if (group.lineRenderer)
                                group.lineRenderer.startColor = Color.Lerp(from, to, curve.Evaluate(1f));
                        }
                    }
                    break;
                case PlaybackDirection.BACKWARD:
                    if (value < 1f)
                    {
                        foreach (GraphicsColorGroup group in spriteColorGroups)
                        {
                            if (group.uiGraphics)
                                group.uiGraphics.color = Color.Lerp(to, from, curve.Evaluate(value));
                            else if (group.spriteRenderer)
                                group.spriteRenderer.color = Color.Lerp(to, from, curve.Evaluate(value));
                            else if (group.lineRenderer)
                                group.lineRenderer.startColor = Color.Lerp(to, from, curve.Evaluate(value));
                        }
                    }
                    else
                    {
                        isPlaying = false;

                        foreach (GraphicsColorGroup group in spriteColorGroups)
                        {
                            if (group.uiGraphics)
                                group.uiGraphics.color = Color.Lerp(to, from, curve.Evaluate(1f));
                            else if (group.spriteRenderer)
                                group.spriteRenderer.color = Color.Lerp(to, from, curve.Evaluate(1f));
                            else if (group.lineRenderer)
                                group.lineRenderer.startColor = Color.Lerp(to, from, curve.Evaluate(1f));
                        }
                    }
                    break;
            }
        }

        public override void OnReset()
        {
            TryParse();

            foreach (GraphicsColorGroup group in spriteColorGroups)
            {
                if (group.uiGraphics)
                    group.uiGraphics.color = from;
                else if (group.spriteRenderer)
                    group.spriteRenderer.color = from;
            }
        }

        public void TryParse()
        {
            spriteColorGroups = new List<GraphicsColorGroup>();

            if (customObjects.Length != 0)
                foreach (Transform obj in customObjects)
                    Parse(obj);
            else
                Parse(transform);
        }

        public void Parse(Transform obj)
        {
            SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
            MaskableGraphic graphics = obj.GetComponent<MaskableGraphic>();
            LineRenderer lineRenderer = obj.GetComponent<LineRenderer>();

            spriteColorGroups.Add(new GraphicsColorGroup() { spriteRenderer = renderer, uiGraphics = graphics, lineRenderer = lineRenderer });

            if (propagate)
                for (int i = 0; i < obj.childCount; i++)
                    Parse(obj.GetChild(i));
        }
    }

}