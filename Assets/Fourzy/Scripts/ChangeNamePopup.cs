using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy
{
    public class ChangeNamePopup : MonoBehaviour, IPopup
    {
        [SerializeField] Text currentNameText;
        [SerializeField] InputField inputField;

        private void OnEnable()
        {
            UserManager.OnUpdateUserInfo += UserManagerOnUpdateName;
        }

        private void OnDisable()
        {
            UserManager.OnUpdateUserInfo -= UserManagerOnUpdateName;
        }

        private void UserManagerOnUpdateName()
        {
            currentNameText.text = "Current name: " + UserManager.Instance.userName;
        }

        public void ChangeNameOnClick()
        {
            UserManager.Instance.UpdatePlayerDisplayName(inputField.text);
            this.Close();
        }

        public void CloseOnClick()
        {
            this.Close();
        }

        private void Close()
        {
            inputField.DeactivateInputField();
            this.gameObject.SetActive(false);

            PopupManager.Instance.ClosePopup();
        }

        void IPopup.Open()
        {
            this.gameObject.SetActive(true);
            inputField.text = string.Empty;
            currentNameText.text = "Current name: " + UserManager.Instance.userName;
            inputField.ActivateInputField();
        }
    }
}
