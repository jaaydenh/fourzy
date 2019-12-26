//@vadym udod

using Fourzy._Updates._Tutorial;
using Fourzy._Updates.UI.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class OnboardingScreenPointer : WidgetBase
    {
        public Image hand;
        public Badge messagaBox;
        public VerticalLayoutGroup container;

        private RectTransform root;

        public override WidgetBase SetAnchors(Vector2 anchor)
        {
            if (anchor.x > .8f) hand.transform.localScale = Vector3.one; 
            else hand.transform.localScale = new Vector3(-1f, 1f, 1f);

            return base.SetAnchors(anchor);
        }

        public override void Hide(float time)
        {
            messagaBox.transform.SetParent(transform);
            base.Hide(time);
        }

        public void Reset()
        {
            Hide(0f);
        }

        public OnboardingScreenPointer SetMessage(string message, OnboardingTask_HighlightButton.MessageBoxPositionData messagePositionData = null)
        {
            messagaBox.SetValue(message);

            if (messagePositionData != null)
            {
                messagaBox.transform.SetParent(root);
                messagaBox.SetAnchors(messagePositionData.pivot);
                messagaBox.SetPosition(messagePositionData.positionOffset);
            }
            else
            {
                messagaBox.transform.SetParent(transform);

                Vector2 pivot = new Vector2(0f, 1f);
                Vector2 position = new Vector2(0f, -120f);

                if (rectTransform.anchorMin.x > .8f)
                {
                    pivot.x = 1f;
                    container.childAlignment = TextAnchor.MiddleRight;
                }
                else if (rectTransform.anchorMin.x < .2f)
                    container.childAlignment = TextAnchor.MiddleLeft;
                else
                {
                    pivot.x = .5f;
                    container.childAlignment = TextAnchor.MiddleCenter;
                }

                if (rectTransform.anchorMin.y < .3f)
                {
                    pivot.y = 0f;
                    position.y = 0f;
                }

                messagaBox.ResetAnchors();
                messagaBox.SetPivot(pivot);
                messagaBox.SetPosition(position);
            }

            return this;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            SetInteractable(false);
            BlockRaycast(false);

            root = transform.parent as RectTransform;
        }
    }
}