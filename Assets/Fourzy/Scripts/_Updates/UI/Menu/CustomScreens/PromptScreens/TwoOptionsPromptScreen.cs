//@vadym udod

using Fourzy._Updates.UI.Helpers;
using System;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class TwoOptionsPromptScreen : PromptScreen
    {
        public ButtonExtended option2Button;

        protected Action onOption2;

        public virtual PromptScreen _Prompt(string title, string text, string option1text, string option2text, Action accept = null, Action decline = null, Action option2 = null)
        {
            onOption2 = option2;

            UpdateButton(option2Button, option2text);

            return Prompt(title, text, option1text, "", accept, decline);
        }

        public void Option2(bool force = false)
        {
            if (inputBlocked && !force) return;

            onOption2?.Invoke();

            if (closeOnAccept)
            {
                CloseSelf();
            }
        }
    }
}
