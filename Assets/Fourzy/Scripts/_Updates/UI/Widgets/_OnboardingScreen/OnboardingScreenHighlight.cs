//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class OnboardingScreenHighlight : WidgetBase
    {
        public OnboardingScreenHighlightPrefab highlightPrefab;

        protected override void Awake()
        {
            base.Awake();

            Hide(0f);
            SetInteractable(false);
            BlockRaycast(false);
        }

        public void ShowHighlight(Rect[] areas)
        {
            foreach (Transform highlightInstance in transform)
                Destroy(highlightInstance.gameObject);

            foreach (Rect area in areas)
            {
                OnboardingScreenHighlightPrefab instance = Instantiate(highlightPrefab, transform);
                instance.transform.localScale = Vector3.one;

                instance.SetArea(area);
            }
        }
    }
}
