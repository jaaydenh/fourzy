//@vadym udod

using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class OnboardingScreenDialog : WidgetBase
    {
        public TMP_Text message;

        protected override void Awake()
        {
            base.Awake();

            Hide(0f);
        }

        public void DisplayText(string text)
        {
            Show(.2f);

            message.text = text;
        }
    }
}
