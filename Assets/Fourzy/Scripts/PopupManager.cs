using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Fourzy
{
    public interface IPopup
    {
        void Open();
    }

    [UnitySingleton(UnitySingletonAttribute.Type.ExistsInScene, true)]
    public class PopupManager : UnitySingleton<PopupManager>
    {
        [SerializeField]
        private TokenPopupUI tokenPopupUI;

        [SerializeField]
        private ChangeNamePopup changeNamePopup;
        
        [SerializeField]
        private ConfirmPopup confirmPopup;

        [SerializeField]
        private GameObject overlayImage;

        [SerializeField]
        private Camera popupCamera;

        private Dictionary<Type, IPopup> popups = new Dictionary<Type, IPopup>();

        private void Start()
        {
            popups[tokenPopupUI.GetType()] = tokenPopupUI;
            popups[changeNamePopup.GetType()] = changeNamePopup;
            popups[confirmPopup.GetType()] = confirmPopup;
        }

        public void OpenPopup<T>(bool overlay = true) where T : IPopup
        {
            overlayImage.SetActive(overlay);
            popupCamera.enabled = true;

            popups[typeof(T)].Open();
        }

        public T GetPopup<T>() where T : IPopup
        {
            return (T)popups[typeof(T)];
        }

        public void ClosePopup()
        {
            overlayImage.SetActive(false);
            popupCamera.enabled = false;
        }
    }
}
