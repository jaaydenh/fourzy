//@vadym udod

using Fourzy._Updates.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class RandomPickWidget : WidgetBase
    {
        public static float ANIMATION_TIME = 2f;
        public static float SCALE_VALUE = 1.3f;

        public RandomPickItemWidget itemWidgetPrefab;
        public RectTransform widgetsParent;
        public AnimationCurve animationCurve;
        public int itemsCount;
        public float step = 60f;

        private List<RandomPickItemWidget> widgets;
        private float targetSpinValue = 0f;
        private float spinValue = 0f;
        private float startSpinValue;
        private float offset;
        private int lastItemIndex;
        private string[] values;

        public float size => itemsCount * step;

        public void SetData(params string[] values)
        {
            this.values = values;

            CreateItems();
        }

        public float StartPick(float delay, bool show = true)
        {
            if (show) Show(0f);

            spinValue = 0f;
            lastItemIndex = 0;
            targetSpinValue = (itemsCount * 5f) * step;

            StartRoutine("spin", SpinRoutine(delay), null, null);

            return delay + ANIMATION_TIME;
        }

        public void Cancel()
        {
            CancelRoutine("spin");
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            widgets = new List<RandomPickItemWidget>();
        }

        private void CreateItems()
        {
            foreach (RandomPickItemWidget widget in widgets) Destroy(widget.gameObject);
            widgets.Clear();

            spinValue = startSpinValue = Random.value > .5 ? step : 0f;
            offset = (rectTransform.rect.height - (itemsCount * step)) * .5f;

            for (int index = 0; index < itemsCount; index++)
            {
                RandomPickItemWidget widgetInstance = Instantiate(itemWidgetPrefab, widgetsParent);

                widgetInstance.SetData(index * step, values[index % values.Length]);
                widgets.Add(widgetInstance);

                UpdateWidget(widgetInstance);
            }
        }

        private void UpdateWidget(RandomPickItemWidget widget)
        {
            widget.rectTransform.anchoredPosition = new Vector3(0f, (widget.offset + spinValue) % size + offset);

            float nomalizedRelativePosition = 1f - Mathf.Clamp01((Mathf.Abs(widget.rectTransform.localPosition.y) / (rectTransform.rect.height / 2f)));

            widget.alphaTween.SetAlpha(nomalizedRelativePosition);
            widget.scaleTween.SetScale(Vector3.one * Mathf.Clamp(nomalizedRelativePosition * SCALE_VALUE, .75f, SCALE_VALUE));
        }

        private IEnumerator SpinRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);

            float timer = 0f;
            while ((timer += Time.deltaTime) < ANIMATION_TIME)
            {
                spinValue = Mathf.LerpUnclamped(
                    startSpinValue, targetSpinValue, animationCurve.Evaluate(timer / ANIMATION_TIME));

                if (lastItemIndex != (int)(spinValue / step))
                {
                    lastItemIndex = (int)(spinValue / step);
                    AudioHolder.instance.PlaySelfSfxOneShot("scroll");
                }

                for (int index = 0; index < itemsCount; index++)
                {
                    UpdateWidget(widgets[index]);
                }

                yield return new WaitForEndOfFrame();
            }
        }
    }
}