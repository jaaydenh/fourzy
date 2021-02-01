//@vadym udod

using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class OnboardingScreenDialog : WidgetBase
    {
        public TMP_Text message;
        public TMP_Text hint;

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

        protected override void OnInitialized()
        {
            base.OnInitialized();

            hint.text = LocalizationManager.Value(
                GameManager.HAS_POINTER ? "click_to_continue" : "tap_to_continue");
        }
    }
}
