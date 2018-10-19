using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using mixpanel;

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
            Mixpanel.Track("Realtime Confirm Close");
            this.gameObject.SetActive(false);

            PopupManager.Instance.ClosePopup();
        }

        void IPopup.Open()
        {
            var props = new Value();
            props["Source"] = "Confirm Popup";
            Mixpanel.Track("Start Realtime Matchmaking", props);
            this.gameObject.SetActive(true);
        }
    }
}
