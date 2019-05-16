////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Koretsky Konstantin (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using SA.iOS.Utilities;
using UnityEngine;
using SA.Foundation.Events;

#if UNITY_IPHONE || UNITY_TVOS
using System.Runtime.InteropServices;
#endif

namespace SA.iOS.Foundation.Internal
{

    internal class ISN_NSNativeAPI : ISN_Singleton<ISN_NSNativeAPI>, ISN_NSAPI
    {
#if UNITY_IPHONE || UNITY_TVOS

        [DllImport ("__Internal")] private static extern void _ISN_SetString(string key, string val);
        [DllImport("__Internal")] private static extern bool _ISN_Synchronize();
        [DllImport ("__Internal")] private static extern string _ISN_KeyValueStoreObjectForKey(string key);
        [DllImport ("__Internal")] private static extern void _ISN_iCloud_Reset();


        //Time Zone
        [DllImport("__Internal")] private static extern string _ISN_NS_TimeZone_LocalTimeZone();
        [DllImport("__Internal")] private static extern string _ISN_NS_TimeZone_SystemTimeZone();
        [DllImport("__Internal")] private static extern string _ISN_NS_TimeZone_DefaultTimeZone();
        [DllImport("__Internal")] private static extern void _ISN_NS_TimeZone_ResetSystemTimeZone();

        //Locale
        [DllImport("__Internal")] private static extern string _ISN_NS_Locale_CurrentLocale();
        [DllImport("__Internal")] private static extern string _ISN_NS_Locale_AutoupdatingCurrentLocale();


        [DllImport("__Internal")] private static extern string _ISN_UbiquityIdentityToken();

#endif

        private SA_Event<ISN_NSStoreDidChangeExternallyNotification> m_storeDidChangeReceived = new SA_Event<ISN_NSStoreDidChangeExternallyNotification>();

        public void SetString(string key, string val) {
            #if UNITY_IPHONE || UNITY_TVOS
                _ISN_SetString(key, val);
            #endif
        }

        public bool Synchronize() {
            #if UNITY_IPHONE || UNITY_TVOS
                return _ISN_Synchronize();
            #else
                return false;
            #endif
        }

        public ISN_NSKeyValueObject KeyValueStoreObjectForKey(string key) {
            #if UNITY_IPHONE || UNITY_TVOS
                var result =  JsonUtility.FromJson<ISN_NSKeyValueResult>(_ISN_KeyValueStoreObjectForKey(key));
                if(result.HasError) {
                    return null;
                } else {
                    return result.KeyValueObject;
                }
                
            #else
                return null;
            #endif
        }


        public void ResetCloud() {
            #if UNITY_IPHONE || UNITY_TVOS
            _ISN_iCloud_Reset();
            #endif
        }

        private void OnStoreDidChange(string data) {
            var result = JsonUtility.FromJson<ISN_NSStoreDidChangeExternallyNotification>(data);
            m_storeDidChangeReceived.Invoke(result);
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
#if UNITY_IPHONE || UNITY_TVOS
            _ISN_NS_TimeZone_ResetSystemTimeZone();
#endif
        }



        public ISN_NSTimeZone LocalTimeZone {
            get {
#if UNITY_IPHONE || UNITY_TVOS
                return JsonUtility.FromJson<ISN_NSTimeZone>(_ISN_NS_TimeZone_LocalTimeZone());
#else
                return null;
#endif
            }
        }

        public ISN_NSTimeZone SystemTimeZone {
            get {
#if UNITY_IPHONE || UNITY_TVOS
                return JsonUtility.FromJson<ISN_NSTimeZone>(_ISN_NS_TimeZone_SystemTimeZone());
#else
                return null;
#endif
            }
        }

        public ISN_NSTimeZone DefaultTimeZone {
            get {
#if UNITY_IPHONE || UNITY_TVOS
                return JsonUtility.FromJson<ISN_NSTimeZone>(_ISN_NS_TimeZone_DefaultTimeZone());
#else
                return null;
#endif
            }
        }

        //--------------------------------------
        // Locale
        //--------------------------------------


        public ISN_NSLocale CurrentLocale {
            get {
#if UNITY_IPHONE || UNITY_TVOS
                return JsonUtility.FromJson<ISN_NSLocale>(_ISN_NS_Locale_CurrentLocale());
#else
                return null;
#endif
            }
        }

        public ISN_NSLocale AutoUpdatingCurrentLocale {
            get {
#if UNITY_IPHONE || UNITY_TVOS
                return JsonUtility.FromJson<ISN_NSLocale>(_ISN_NS_Locale_AutoupdatingCurrentLocale());
#else
                return null;
#endif
            }
        }


        //--------------------------------------
        // ISN_NSFileManager
        //--------------------------------------


        public string UbiquityIdentityToken {
            get {
#if UNITY_IPHONE || UNITY_TVOS
                return _ISN_UbiquityIdentityToken();
#else
                return string.Empty;
#endif
            }
        }
    }
}
