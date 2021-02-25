//@vadym udod

using Fourzy._Updates.Tween;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class ChangeNamePromptScreen : PromptScreen
    {
        public TMP_InputField inputField;
        public TMP_Text errorLabel;
        public CanvasGroup changingNameOverlay;
        public AlphaTween changingNameTween;

        private string previousName;

        protected void OnEnable()
        {
            UserManager.onDisplayNameChanged += OnDisplayNameChanged;
            UserManager.onDisplayNameChangeFailed += OnDisplayNameChangeFailed;
        }

        protected void OnDisable()
        {
            UserManager.onDisplayNameChanged -= OnDisplayNameChanged;
            UserManager.onDisplayNameChangeFailed -= OnDisplayNameChangeFailed;
        }

        public void _Prompt()
        {
            Prompt(LocalizationManager.Value("change_name"), "", LocalizationManager.Value("change_name"), null);
        }

        public override void Accept(bool force = false)
        {
            //change name
            if (string.IsNullOrEmpty(inputField.text))
            {
                SetError(LocalizationManager.Value("cant_be_empty"));

                return;
            }
            else
            {
                if (inputField.text.Length <= 3 || inputField.text.Length >= 25)
                {
                    if (inputField.text.Length <= 3)
                    {
                        SetError(LocalizationManager.Value("too_short"));
                    }
                    else
                    {
                        SetError(LocalizationManager.Value("too_long"));
                    }
                }
                else
                {
                    //hide error label
                    SetError();

                    //try apply name
                    UserManager.Instance.SetDisplayName(inputField.text);
                    SetChangingNameOverlayState(true);
                }
            }
        }

        public override void Open()
        {
            base.Open();

            inputField.text = "";
            inputField.ActivateInputField();

            previousName = UserManager.Instance.userName;
            promptText.text = previousName;

            if (UserManager.Instance.currentlyChangingName) SetChangingNameOverlayState(true);
        }

        private void SetError(string text = null)
        {
            errorLabel.gameObject.SetActive(!string.IsNullOrEmpty(text));
            errorLabel.text = text;
        }

        protected void SetChangingNameOverlayState(bool state)
        {
            changingNameOverlay.interactable = state;
            changingNameOverlay.blocksRaycasts = state;

            if (state)
            {
                changingNameTween.PlayForward(true);
            }
            else
            {
                changingNameTween.PlayBackward(true);
            }
        }

        private void OnDisplayNameChanged()
        {
            promptText.text = $"{LocalizationManager.Value("current_name")}: " + UserManager.Instance.userName;
            SetChangingNameOverlayState(false);

            menuController.CloseCurrentScreen();
        }

        private void OnDisplayNameChangeFailed(string message)
        {
            //ignore if setting random name
            if (UserManager.Instance.settingRandomName) return;

            //set back previous name
            UserManager.Instance.SetDisplayName(previousName, false);

            SetChangingNameOverlayState(false);
            SetError(message);
        }
    }
}
