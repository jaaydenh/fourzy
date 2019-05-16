////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native 2018 - New Generation
// @author Stan's Assets team 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;
using System;
using SA.Foundation.Async;
using SA.Foundation.Events;
using SA.Foundation.Templates;
using SA.Foundation.UtilitiesEditor;

namespace SA.iOS.Contacts.Internal
{

    internal class ISN_CNEditorAPI : ISN_iCNAPI
    {
        public void RetrievePhoneContacts(Action<ISN_CNContactsResult> callback) {
            SA_Coroutine.WaitForSeconds(DelayTime, () => {
                callback.Invoke(CreateFakeResult());
            });
        }

        public void ShowContactsPicker(Action<ISN_CNContactsResult> callback) {
            SA_Coroutine.WaitForSeconds(DelayTime, () => {
                callback.Invoke(CreateFakeResult());
            });
        }




        private ISN_CNContactsResult CreateFakeResult() {
            TextAsset editorData = SA_AssetDatabase.LoadAssetAtPath<TextAsset>(ISN_Settings.CONTACTS_API_LOCATION + "ISN_CNContactsEditorResponce.txt");
            ISN_CNContactsResult result = JsonUtility.FromJson<ISN_CNContactsResult>(editorData.text);
            return result;
        }

        public ISN_CNAuthorizationStatus GetAuthorizationStatus(ISN_CNEntityType entityType) {
            return ISN_CNAuthorizationStatus.Authorized;
        }

        public void RequestAccess(ISN_CNEntityType entityType, Action<SA_Result> callback) {
            SA_Coroutine.WaitForSeconds(DelayTime, () => {
                callback.Invoke(new SA_Result());
           });
        }

        private float DelayTime {
            get {
                return UnityEngine.Random.Range(0.1f, 3f);
            }
        }
    }
}
