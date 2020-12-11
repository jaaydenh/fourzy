//@vadym udod

using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class OnboardingScreenInstruction : WidgetBase
    {
        public TMP_Text label;

        public OnboardingScreenInstruction DisplayText(float yAnchor, string message)
        {
            if (!visible || alphaTween._value != 0f) 
                Show(.2f);

            label.text = message;
            SetAnchors(new Vector2(.5f, yAnchor));

            return this;
        }

        public void _Hide()
        {
            if (visible || alphaTween._value != 0f)
                Hide(.3f);
        }
    }
}