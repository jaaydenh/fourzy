//@vadym udod

using Fourzy._Updates.UI.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    public class OnboardingScreenPointer : WidgetBase
    {
        public Image hand;
        public Badge messagaBox;

        protected override void Awake()
        {
            base.Awake();

            SetInteractable(false);
            BlockRaycast(false);

            Hide(0f);
        }

        public override WidgetBase SetAnchors(Vector2 anchor)
        {
            if (anchor.x > .8f)
                hand.transform.localScale = Vector3.one; 
            else
                hand.transform.localScale = new Vector3(-1f, 1f, 1f);

            return base.SetAnchors(anchor);
        }

        public OnboardingScreenPointer SetMessage(string message)
        {
            messagaBox.SetValue(message);

            if (rectTransform.anchorMin.x > .8f)
                messagaBox.SetPivot(Vector2.one);
            else if (rectTransform.anchorMin.x < .2f)
                messagaBox.SetPivot(Vector2.up);
            else
                messagaBox.SetPivot(new Vector2(.5f, 1f));

            messagaBox.SetPosition(new Vector2(0f, -120f));

            return this;
        }
    }
}