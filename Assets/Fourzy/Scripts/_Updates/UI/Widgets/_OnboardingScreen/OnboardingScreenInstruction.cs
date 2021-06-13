//@vadym udod

using TMPro;

namespace Fourzy._Updates.UI.Widgets
{
    public class OnboardingScreenInstruction : WidgetBase
    {
        public TMP_Text label;

        public OnboardingScreenInstruction SetText(string message)
        {
            label.text = message;
            return this;
        }

        public OnboardingScreenInstruction SetFontSize(float size)
        {
            label.fontSize = size;

            return this;
        }
    }
}