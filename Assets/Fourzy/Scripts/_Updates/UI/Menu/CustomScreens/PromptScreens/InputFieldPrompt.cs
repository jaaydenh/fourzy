//@vadym udod

using System;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class InputFieldPrompt : PromptScreen
    {
        public TMP_InputField inputField;

        private Action<string> onInput;

        public InputFieldPrompt _Prompt(
            Action<string> onInput, 
            string title, 
            string text, 
            string buttonYes = "", 
            string buttonNo = "", 
            Action accept = null, 
            Action decline = null)
        {
            this.onInput = onInput;

            Prompt(title, text, buttonYes, buttonNo, accept, decline);

            return this;
        }

        public override PromptScreen Prompt()
        {
            CancelRoutine("delayedTitle");
            CancelRoutine("delayedTitleColor");

            return base.Prompt();
        }

        public override void Open()
        {
            base.Open();

            inputField.text = "";
            inputField.ActivateInputField();
        }

        public override void Accept(bool force = false)
        {
            onInput?.Invoke(inputField.text);

            base.Accept(force);
        }

        public InputFieldPrompt SetText(string text, float duration)
        {
            if (!promptText)
            {
                Debug.LogWarning($"{name} requires title to be set");
                return this;
            }

            promptText.text = text;
            StartRoutine("delayedTitle", duration, () => promptText.text = "", null);

            return this;
        }

        public InputFieldPrompt SetTextColor(Color color, float duration)
        {
            if (!promptText)
            {
                Debug.LogWarning($"{name} requires title to be set");
                return this;
            }

            promptText.color = color;
            StartRoutine("delayedTitleColor", duration, () => promptText.color = Color.white);

            return this;
        }
    }
}