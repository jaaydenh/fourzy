//@vadym udod

using TMPro;

namespace Fourzy._Updates.UI.Widgets
{
    public class OnboardingScreenDialog : WidgetBase
    {
        public TMP_Text message;
        public TMP_Text hint;

        public OnboardingScreenDialog SetText(string text)
        {
            message.text = text;

            return this;
        }

        public OnboardingScreenDialog SetFontSize(float size)
        {
            message.fontSize = size;

            return this;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            hint.text = LocalizationManager.Value(
                GameManager.HAS_POINTER ? "click_to_continue" : "tap_to_continue");
        }
    }
}
