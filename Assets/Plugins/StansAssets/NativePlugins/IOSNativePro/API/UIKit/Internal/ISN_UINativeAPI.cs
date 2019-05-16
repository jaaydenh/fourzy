////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native 2018 - New Generation
// @author Stan's Assets team 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using UnityEngine;
using SA.iOS.Utilities;
using SA.Foundation.Templates;
using SA.Foundation.Events;


#if UNITY_IPHONE
using System.Runtime.InteropServices;
#endif

namespace SA.iOS.UIKit.Internal
{
	
    internal class ISN_UINativeAPI : ISN_Singleton<ISN_UINativeAPI>, ISN_iUIAPI {

#if UNITY_IPHONE
        [DllImport("__Internal")] static extern void _ISN_UI_SaveToCameraRoll(string media);
        [DllImport("__Internal")] static extern string _ISN_UI_GetAvailableMediaTypesForSourceType(int type);
        [DllImport("__Internal")] static extern bool _ISN_UI_IsSourceTypeAvailable(int type);
        [DllImport("__Internal")] static extern void _ISN_UI_PresentPickerController(string data);

        [DllImport("__Internal")] static extern void _ISN_UI_PresentUIAlertController(string data);
        [DllImport("__Internal")] static extern void _ISN_UI_DismissUIAlertController(int alertId);

        [DllImport("__Internal")] static extern void _ISN_UI_PreloaderLockScreen();
        [DllImport("__Internal")] static extern void _ISN_UI_PreloaderUnlockScreen();
#endif


        private SA_Event<ISN_UIAlertActionId> m_onUIAlertActionPerformed = new SA_Event<ISN_UIAlertActionId>();

       
   
        Action<SA_Result> m_onImageSave;
        public void SaveTextureToCameraRoll(string media, Action<SA_Result> callback) {
            m_onImageSave = callback;
            #if UNITY_IPHONE 
            _ISN_UI_SaveToCameraRoll(media);
            #endif
        }

        void OnImageSave(string data) {
            SA_Result result = JsonUtility.FromJson<SA_Result>(data);
            m_onImageSave.Invoke(result);
        }


        public List<string> GetAvailableMediaTypes(ISN_UIImagePickerControllerSourceType type) {
            #if UNITY_IPHONE
            string data = _ISN_UI_GetAvailableMediaTypesForSourceType((int)type);
            ISN_UIAvailableMediaTypes result = JsonUtility.FromJson<ISN_UIAvailableMediaTypes>(data);
            return result.Types;
            #else
            return new List<string>();
            #endif
        }


        public bool IsSourceTypeAvailable(ISN_UIImagePickerControllerSourceType type) {
            #if UNITY_IPHONE
            return _ISN_UI_IsSourceTypeAvailable((int)type);
            #else
            return true;
            #endif
        }

        Action<ISN_UIPickerControllerResult> m_didFinishPickingMedia;
        public void PresentPickerController(ISN_UIPickerControllerRequest request, Action<ISN_UIPickerControllerResult> callback) {
            m_didFinishPickingMedia = callback;
            #if UNITY_IPHONE
            _ISN_UI_PresentPickerController(JsonUtility.ToJson(request));
            #endif
        }


        void didFinishPickingMedia(string data) {
            ISN_UIPickerControllerResult result = JsonUtility.FromJson<ISN_UIPickerControllerResult>(data);
            m_didFinishPickingMedia.Invoke(result);
        }



        public void PresentUIAlertController(ISN_UIAlertController alert) {
            #if UNITY_IPHONE
            string data = JsonUtility.ToJson(alert);
            _ISN_UI_PresentUIAlertController(data);
            #endif
        }

        public void DismissUIAlertController(ISN_UIAlertController alert) {
            #if UNITY_IPHONE
            _ISN_UI_DismissUIAlertController(alert.Id);
            #endif
        }





        public SA_iEvent<ISN_UIAlertActionId> OnUIAlertActionPerformed { 
            get {
                return m_onUIAlertActionPerformed;
            }
        }


        void OnUIAlertAction(string data) {
            ISN_UIAlertActionId result = JsonUtility.FromJson<ISN_UIAlertActionId>(data);
            m_onUIAlertActionPerformed.Invoke(result);
        }


        public void PreloaderLockScreen() {
            #if UNITY_IPHONE
            _ISN_UI_PreloaderLockScreen();
            #endif
        }

        public void PreloaderUnlockScreen() {
            #if UNITY_IPHONE
            _ISN_UI_PreloaderUnlockScreen();
            #endif
        }

    }
}
