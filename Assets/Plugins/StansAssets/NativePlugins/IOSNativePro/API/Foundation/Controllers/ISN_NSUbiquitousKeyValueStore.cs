////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Koretsky Konstantin (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using System;
using SA.Foundation.Events;
using SA.Foundation.Time;
using SA.iOS.Foundation.Internal;

namespace SA.iOS.Foundation
{

    /// <summary>
    /// An iCloud-based container of key-value pairs you use to share data among instances of your app running on a user's connected devices.
    /// Use the iCloud key-value store to make preference, configuration, and app-state data available to every instance of your app on
    /// every device connected to a user’s iCloud account. You can store scalar values such as BOOL, as well as values containing 
    /// any of the property list object types: NSNumber, NSString, NSDate, NSData, NSArray, and NSDictionary.
    /// </summary>
    public static class ISN_NSUbiquitousKeyValueStore
    {
        /// <summary>
        /// Sets a string object for the specified key in the key-value store.
        /// </summary>
        public static void SetString(string key, string value) {
            ISN_NSLib.API.SetString(key, value);
        }

        /// <summary>
        /// Sets a int object for the specified key in the key-value store.
        /// </summary>
        public static void SetInt(string key, int value) {
            ISN_NSLib.API.SetString(key, Convert.ToString(value));
        }

        /// <summary>
        /// Sets a bool object for the specified key in the key-value store.
        /// </summary>
        public static void SetBool(string key, bool value) {
            ISN_NSLib.API.SetString(key, Convert.ToString(value));
        }

        /// <summary>
        /// Sets a float object for the specified key in the key-value store.
        /// </summary>
        public static void SetFloat(string key, float value) {
            ISN_NSLib.API.SetString(key, Convert.ToString(value));
        }

        /// <summary>
        /// Sets a long object for the specified key in the key-value store.
        /// </summary>
        public static void SetLong(string key, long value) {
            ISN_NSLib.API.SetString(key, Convert.ToString(value));
        }

        /// <summary>
        /// Sets a ulong object for the specified key in the key-value store.
        /// </summary>
        public static void SetULong(string key, ulong value) {
            ISN_NSLib.API.SetString(key, Convert.ToString(value));
        }

        /// <summary>
        /// Sets a bytes array for the specified key in the key-value store.
        /// </summary>
        public static void SetBytes(string key, byte[] value) {
            ISN_NSLib.API.SetString(key, Convert.ToBase64String(value));
        }

        /// <summary>
        /// Sets a DateTime object for the specified key in the key-value store.
        /// </summary>
        public static void SetDateTime(string key, DateTime value) {
             SetLong(key, SA_Unix_Time.ToUnixTime(value));
        }

        /// <summary>
        /// Sets an object for the specified key in the key-value store.
        /// </summary>
        public static void SetObject(string key, object obj) {
            ISN_NSLib.API.SetString(key, UnityEngine.JsonUtility.ToJson(obj));
        }

        /// <summary>
        /// Explicitly synchronizes in-memory keys and values with those stored on disk.
        /// YES if the in-memory and on-disk keys and values were synchronized, or NO if an error occurred. 
        /// For example, this method returns NO if an app was not built with the proper entitlement requests.
        /// </summary>
        public static bool Synchronize() {
            return ISN_NSLib.API.Synchronize();
        }


        /// <summary>
        /// Removes all keys and values from the iCloud key-value storage. 
        /// Use with caution.
        /// </summary>
        public static void Reset() {
            ISN_NSLib.API.ResetCloud();
        }

        /// <summary>
        /// The object associated with the specified key or string value "null" if the key was not found.
        /// </summary>
        public static ISN_NSKeyValueObject KeyValueStoreObjectForKey(string key) {
            return ISN_NSLib.API.KeyValueStoreObjectForKey(key);
        }

        /// <summary>
        /// Posted when the value of one or more keys in the local key-value store changed due to incoming data pushed from iCloud. 
        /// This notification is sent only upon a change received from iCloud; it is not sent when your app sets a value.
        /// </summary>
        public static SA_Event<ISN_NSStoreDidChangeExternallyNotification> StoreDidChangeExternallyNotification {
            get {
                return ISN_NSLib.API.StoreDidChangeReceiveResponse;
            }
        }
    }
}
