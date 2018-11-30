//@vadym udod

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

        /// <summary>
        /// Sets data to this propt screen
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="accept">On accept</param>
        /// <param name="decline">on decline</param>
        public virtual void Prompt(string title, string text, Action accept, Action decline)
        {
            onDecline = decline;
            Prompt(title, text, accept);
        }

        public virtual void Prompt(string title, string text, Action accept)
        {
            transform.SetAsLastSibling();

            onAccept = accept;
            promptText.text = text;
            menuController.OpenScreen(this);
        }

        public virtual void Prompt(string title, string text, string yes, string no, Action accept)
        {
            if (acceptButton)
                acceptButton.SetLabel(yes);

            if (declineButton)
                declineButton.SetLabel(no);

            Prompt(title, text, accept);
        }

        public virtual void Prompt(string title, string text, string yes, string no, Action accept, Action decline)
        {
            onDecline = decline;
            Prompt(title, text, yes, no, accept);
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
                menuController.CloseCurrentScreen(true);

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
