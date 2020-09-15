//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.UI.Toasts
{
    public class TopGameToast : GameToast
    {
        public float landscapeHeight = 40f;
        //public float landscapeFontSize = 16f;

        private float defaultHeight;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            defaultHeight = rectTransform.rect.height;
        }

        public override void Show(float time)
        {
            base.Show(time);

            if (GameManager.Instance.Landscape)
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, landscapeHeight);
            else
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, defaultHeight);
        }

        public override void Hide(float time)
        {
            if (IsRoutineActive("removeFromQueue"))
                return;

            base.Hide(time);

            CancelRoutine("displayCoroutine");
            StartRoutine("removeFromQueue", time, () =>
            {
                if (owner)
                {
                    if (owner.topActiveToasts.Contains(this))
                        owner.topActiveToasts.Dequeue();

                    owner.topMovableToast.Dequeue();
                }

                available = true;
            }, null);
        }
    }
}