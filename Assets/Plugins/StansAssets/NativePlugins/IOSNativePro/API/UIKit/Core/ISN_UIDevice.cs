using System;
using UnityEngine;

#if UNITY_IPHONE || UNITY_TVOS
using System.Runtime.InteropServices;
#endif



namespace SA.iOS.UIKit
{

    /// <summary>
    /// A representation of the current device.
    /// </summary>
    [Serializable]
    public class ISN_UIDevice
    { 
        #if UNITY_IPHONE || UNITY_TVOS
        [DllImport("__Internal")] private static extern string _ISN_UI_GetCurrentDevice();
        #endif


        [SerializeField] string m_name = null;
        [SerializeField] string m_systemName = null;
        [SerializeField] string m_model = null;
        [SerializeField] string m_localizedModel = null;
        [SerializeField] string m_systemVersion = null;
        [SerializeField] ISN_UIUserInterfaceIdiom m_userInterfaceIdiom = ISN_UIUserInterfaceIdiom.Pad;

        [SerializeField] string m_identifierForVendor = null;

        //Additional fields
        [SerializeField] int m_majorIOSVersion = 0;



        private static ISN_UIDevice m_currentDevice = null;


        /// <summary>
        /// Returns an object representing the current device.
        /// You access the properties of the returned <see cref="ISN_UIDevice"/> instance to obtain information about the device.
        /// </summary>
        public static ISN_UIDevice CurrentDevice  {
            get {
                if(m_currentDevice == null) {

                    #if (UNITY_IPHONE || UNITY_TVOS) && !UNITY_EDITOR
                    string data = _ISN_UI_GetCurrentDevice();
                    m_currentDevice = JsonUtility.FromJson<ISN_UIDevice>(data);
                    #else
                    m_currentDevice = new ISN_UIDevice();
                    #endif
                }
                return m_currentDevice;
            }
        }



        /// <summary>
        /// The name identifying the device.
        /// The value of this property is an arbitrary alphanumeric string that is associated with the device as an identifier. 
        /// For example, you can find the name of an iOS device in the General > About settings.
        /// </summary>
        public string Name {
            get {
                return m_name;
            }
        }

        /// <summary>
        /// The name of the operating system running on the device represented by the receiver.
        /// </summary>
        public string SystemName {
            get {
                return m_systemName;
            }
        }

        /// <summary>
        /// The model of the device.
        /// Possible examples of model strings are ”iPhone” and ”iPod touch”.
        /// </summary>
        public string Model {
            get {
                return m_model;
            }
        }

        /// <summary>
        /// The model of the device as a localized string.
        /// The value of this property is a string that contains a localized version of the string returned from <see cref="Model"/>.
        /// </summary>
        public string LocalizedModel {
            get {
                return m_localizedModel;
            }
        }


        /// <summary>
        /// The current version of the operating system.
        /// </summary>
        public string SystemVersion {
            get {
                return m_systemVersion;
            }
        }

        /// <summary>
        /// The style of interface to use on the current device.
        /// 
        /// For universal applications, you can use this property to tailor the behavior of your application for a specific type of device. 
        /// For example, iPhone and iPad devices have different screen sizes, so you might want to create different views and controls 
        /// based on the type of the current device.
        /// </summary>
        public ISN_UIUserInterfaceIdiom UserInterfaceIdiom {
            get {
                return m_userInterfaceIdiom;
            }
        }


        /// <summary>
        /// An alphanumeric string that uniquely identifies a device to the app’s vendor.
        /// 
        /// The value of this property is the same for apps that comes from the same vendor running on the same device. 
        /// A different value is returned for apps on the same device that come from different vendors, and for apps on different devices regardless of vendor.
        /// 
        /// Normally, the vendor is determined by data provided by the App Store. If the app was not installed from the app store (such as enterprise apps and apps still in development), 
        /// then a vendor identifier is calculated based on the app’s bundle ID. 
        /// The bundle ID is assumed to be in reverse-DNS format.
        /// </summary>
        public string IdentifierForVendor {
            get {
                return m_identifierForVendor;
            }
        }

        /// <summary>
        /// The current major version number of the operating system.
        /// Example: 11
        /// </summary>
        public int MajorIOSVersion {
            get {
                return m_majorIOSVersion;
            }
        }
    }
}