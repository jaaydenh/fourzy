//@vadym udod

using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class OnboardingScreenDialog : WidgetBase
    {
        public TMP_Text message;

        public void DisplayText(float yAnchor, string text)
        {
            if (!visible) Show(.2f);

            SetAnchors(new Vector2(.5f, yAnchor));

            message.text = text;
        }

        public void _Hide()
        {
            if (visible) Hide(.3f);
        }
    }
}
