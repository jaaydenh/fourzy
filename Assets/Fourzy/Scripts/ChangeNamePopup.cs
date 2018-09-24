using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy
{
    public class ChangeNamePopup : MonoBehaviour, IPopup
    {
        [SerializeField]
        private Text currentNameText;

        [SerializeField]
        private InputField inputField;

        public void ChangeNameOnClick()
        {
            UserManager.instance.UpdatePlayerDisplayName(inputField.text);
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
            currentNameText.text = "Current name: " + UserManager.instance.userName;
            inputField.ActivateInputField();
        }
    }
}
