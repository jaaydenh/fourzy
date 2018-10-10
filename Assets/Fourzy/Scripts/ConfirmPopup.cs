using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy
{
    public class ConfirmPopup : MonoBehaviour, IPopup
    {
        public void YesOnClick()
        {
            this.Close();
            ViewMatchMaking.instance.isRealtime = true;
            ViewController.instance.ChangeView(ViewController.instance.viewMatchMaking);
            ViewController.instance.HideTabView();
            ViewController.instance.headerUI.SetActive(false);
        }

        public void NoOnClick()
        {
            this.Close();
        }

        public void CloseOnClick()
        {
            this.Close();
        }

        private void Close()
        {
            this.gameObject.SetActive(false);

            PopupManager.Instance.ClosePopup();
        }

        void IPopup.Open()
        {
            this.gameObject.SetActive(true);
        }
    }
}
