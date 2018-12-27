﻿//@vadym udod

using Fourzy._Updates.UI.Helpers;
using System;
using TMPro;

namespace Fourzy._Updates.UI.Menu.Screens
{
    /// <summary>
    /// Promp screen can be used in various places
    /// </summary>
    public class PromptScreen : MenuScreen
    {
        public TextMeshProUGUI promptTitle;
        public TextMeshProUGUI promptText;

        public ButtonExtended acceptButton;
        public ButtonExtended declineButton;

        private Action onAccept;
        private Action onDecline;

        public override void OnBack()
        {
            base.OnBack();

            if (defaultCalls)
                Decline();
        }

        public virtual void Prompt(string title, string text, Action accept, Action decline)
        {
            onDecline = decline;

            Prompt(title, text, accept);
        }

        public virtual void Prompt(string title, string text, string yes, string no, Action accept, Action decline)
        {
            onDecline = decline;

            Prompt(title, text, yes, no, accept);
        }

        public virtual void Prompt(string title, string text, Action accept)
        {
            Prompt(title, text, "Yes", "No", accept);
        }

        public virtual void Prompt(string title, string text, string yes, string no, Action accept)
        {
            if (acceptButton)
            {
                if (string.IsNullOrEmpty(yes))
                    acceptButton.SetActive(false);
                else
                {
                    acceptButton.SetActive(true);
                    acceptButton.SetLabel(yes);
                }
            }

            if (declineButton)
            {
                if (string.IsNullOrEmpty(no))
                    declineButton.SetActive(false);
                else
                {
                    declineButton.SetActive(true);
                    declineButton.SetLabel(no);
                }
            }

            promptTitle.text = title;
            promptText.text = text;
            onAccept = accept;

            Prompt();
        }

        public virtual void Prompt()
        {
            transform.SetAsLastSibling();
            menuController.OpenScreen(this);
        }

        public virtual void Accept()
        {
            if (onAccept != null)
                onAccept.Invoke();

            onAccept = null;
            onDecline = null;
        }

        public virtual void Decline()
        {
            if (onDecline != null)
                onDecline.Invoke();
            else
                menuController.CloseCurrentScreen();

            onAccept = null;
            onDecline = null;
        }

        public enum PromptType
        {
            GENERIC,
            CHANGE_NAME,
        }
    }
}
