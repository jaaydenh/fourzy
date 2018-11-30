//@vadym udod

using Fourzy._Updates.UI.Toasts;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class ChangeNamePromptScreen : PromptScreen
    {
        public TMP_InputField inputField;

        private void OnEnable()
        {
            UserManager.OnUpdateUserInfo += UserManagerOnUpdateName;
        }

        private void OnDisable()
        {
            UserManager.OnUpdateUserInfo -= UserManagerOnUpdateName;
        }

        public override void Accept()
        {
            base.Accept();

            //change name
            if (string.IsNullOrEmpty(inputField.text))
            {
                GamesToastsController.ShowToast(GamesToastsController.ToastStyle.ACTION_WARNING, "New name can't be empty!");
                return;
            }

            UserManager.Instance.UpdatePlayerDisplayName(inputField.text);
        }

        public override void Open()
        {
            base.Open();

            inputField.text = "";
            inputField.ActivateInputField();
        }

        private void UserManagerOnUpdateName()
        {
            promptText.text = "Current name: " + UserManager.Instance.userName;
        }
    }
}
