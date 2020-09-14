//@vadym udod

using System;
using TMPro;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class InputFieldPrompt : PromptScreen
    {
        public TMP_InputField inputField;

        private Action<string> onInput;

        public void _Prompt(Action<string> onInput, string title, string text, string buttonYes = "", string buttonNo = "", Action accept = null, Action decline = null)
        {
            this.onInput = onInput;

            Prompt(title, text, buttonYes, buttonNo, accept, decline);
        }

        public override void Open()
        {
            base.Open();

            inputField.text = "";
        }

        public override void Accept()
        {
            onInput?.Invoke(inputField.text);

            base.Accept();

            if (onAccept == null) CloseSelf();
        }
    }
}