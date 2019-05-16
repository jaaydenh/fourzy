////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Koretsky Konstantin (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;
using SA.Foundation.Events;

namespace SA.iOS.Foundation.Internal
{

    internal class ISN_NSEditorAPI : ISN_NSAPI
    {

        private SA_Event<ISN_NSStoreDidChangeExternallyNotification> m_storeDidChangeReceived = new SA_Event<ISN_NSStoreDidChangeExternallyNotification>();

        public void SetString(string key, string val) {
            PlayerPrefs.SetString(key, val);
        }

        public bool Synchronize() {
            return true;
        }

        public ISN_NSKeyValueObject KeyValueStoreObjectForKey(string key) {
            string val = PlayerPrefs.GetString(key);
            if(string.IsNullOrEmpty(val)) {
                return null;
            }
            var obj = new ISN_NSKeyValueObject(key, val);
            return obj;
        }

        private void OnStoreDidChange(string data, Action<ISN_NSStoreDidChangeExternallyNotification> callback) {
            
        }

        public void ResetCloud() {
            PlayerPrefs.DeleteAll();
        }


        public SA_Event<ISN_NSStoreDidChangeExternallyNotification> StoreDidChangeReceiveResponse {
            get {
                return m_storeDidChangeReceived;
            }
        }



        //--------------------------------------
        // Time Zone
        //--------------------------------------


        public void ResetSystemTimeZone() {

        }



        public ISN_NSTimeZone LocalTimeZone {
            get {
                return new ISN_NSTimeZone();
            }
        }

        public ISN_NSTimeZone SystemTimeZone {
            get {
                return new ISN_NSTimeZone();
            }
        }

        public ISN_NSTimeZone DefaultTimeZone {
            get {
                return new ISN_NSTimeZone();
            }
        }

        public string UbiquityIdentityToken {
            get {
                return string.Empty;
            }
        }


        //--------------------------------------
        // Locale
        //--------------------------------------

        public ISN_NSLocale CurrentLocale {
            get {
                return new ISN_NSLocale();
            }
        }

        public ISN_NSLocale AutoUpdatingCurrentLocale {
            get {
                return new ISN_NSLocale();
            }
        }
    }
}
