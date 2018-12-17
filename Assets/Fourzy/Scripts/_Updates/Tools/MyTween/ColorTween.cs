﻿//@vadym udod

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.Tween
{
    public class ColorTween : TweenBase
    {
        public Transform[] customObjects;
        public bool propagate = false;      //do the same for all child objects?
        public bool changeAlpha = true;
        public Color from = Color.white;
        public Color to = Color.white;

        private List<GraphicsColorGroup> spriteColorGroups;
        private Color temp;

        protected override void Awake()
        {
            base.Awake();

            DoParse();
        }

        public override void AtProgress(float value, PlaybackDirection direction)
        {
            base.AtProgress(value, direction);

            switch (direction)
            {
                case PlaybackDirection.FORWARD:
                    if (value < 1f)
                        temp = Color.Lerp(from, to, curve.Evaluate(value));
                    else
                    {
                        temp = Color.Lerp(from, to, curve.Evaluate(1f));
                        isPlaying = false;
                    }
                    break;
                case PlaybackDirection.BACKWARD:
                    if (value < 1f)
                        temp = Color.Lerp(to, from, curve.Evaluate(value));
                    else
                    {
                        Color.Lerp(to, from, curve.Evaluate(1f));
                        isPlaying = false;
                    }
                    break;
            }

            SetColor(temp);
        }

        public void SetColor(Color color)
        {
            foreach (GraphicsColorGroup group in spriteColorGroups)
            {
                if (group.uiGraphics)
                    group.uiGraphics.color = changeAlpha ? color : new Color(color.r, color.g, color.b, group.uiGraphics.color.a);
                else if (group.spriteRenderer)
                    group.spriteRenderer.color = changeAlpha ? color : new Color(color.r, color.g, color.b, group.spriteRenderer.color.a);
                else if (group.lineRenderer)
                    group.lineRenderer.startColor = changeAlpha ? color : new Color(color.r, color.g, color.b, group.lineRenderer.startColor.a);
            }
        }

        public override void OnReset()
        {
            SetColor(from);
        }

        public void DoParse()
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