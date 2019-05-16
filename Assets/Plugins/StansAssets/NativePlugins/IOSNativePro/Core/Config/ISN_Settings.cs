////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;


using SA.Foundation.Patterns;
using SA.Foundation.Config;

using SA.iOS.UIKit;
using SA.iOS.GameKit;
using SA.iOS.StoreKit;
using SA.iOS.Utilities;


using SA.iOS.XCode;


namespace SA.iOS
{
   

    public class ISN_Settings : SA_ScriptableSingleton<ISN_Settings>
    {
        public const string PLUGIN_NAME = "IOS Native";
        public const string DOCUMENTATION_URL = "https://unionassets.com/ios-native-pro/manual";
        public const string IOS_NATIVE_FOLDER       = SA_Config.STANS_ASSETS_NATIVE_PLUGINS_PATH + "IOSNativePro/";
       
        public const string IOS_NATIVE_API = IOS_NATIVE_FOLDER + "API/";
        public const string IOS_NATIVE_XCODE = IOS_NATIVE_FOLDER + "XCode/";
        public const string IOS_NATIVE_XCODE_SOURCE = IOS_NATIVE_FOLDER + "XCodeDisabled/";

        public const string DEFAULT_ENTITLEMENTS_FILE = IOS_NATIVE_XCODE + "XCodeFiles/default.entitlements";


        public const string AV_KIT_API_LOCATION= IOS_NATIVE_API + "AVKit/Internal/";
        public const string CONTACTS_API_LOCATION = IOS_NATIVE_API + "Contacts/Internal/";
        public const string GAME_KIT_API_LOCATION = IOS_NATIVE_API + "GameKit/Internal/";
        public const string PHOTOS_API_LOCATION = IOS_NATIVE_API + "Photos/Internal/";
        public const string STORE_KIT_API_LOCATION = IOS_NATIVE_API + "StoreKit/Internal/";
        public const string USER_NOTIFICATIONS_API_LOCATION = IOS_NATIVE_API + "UserNotifications/Internal/";

        public const string TEST_SCENE_PATH = IOS_NATIVE_FOLDER + "Tests/Scene/ISN_TestScene.unity";


        //--------------------------------------
        // API Settings
        //--------------------------------------

        public bool Contacts = false;
        public bool Photos = false;
        public bool ReplayKit = false;
        public bool Social = false;
		public bool AVKit = false;
        public bool CoreLocation = false;
        public bool AssetsLibrary = false;
        public bool AppDelegate = false;
        public bool UserNotifications = false;
        public bool MediaPlayer = false;


        public ISN_LogLevel LogLevel = new ISN_LogLevel();


       


        //--------------------------------------
        // Foundation
        //--------------------------------------


        //--------------------------------------
        // StoreKit Settings
        //--------------------------------------

        public List<ISN_SKProduct> InAppProducts = new List<ISN_SKProduct>();


        //--------------------------------------
        // GameKit Settings
        //--------------------------------------

        public List<ISN_GKAchievement> Achievements = new List<ISN_GKAchievement>();
        public bool SavingAGame = false;



        //--------------------------------------
        // App Delegate Settings
        //--------------------------------------

        public List<ISN_UIApplicationShortcutItem> ShortcutItems = new List<ISN_UIApplicationShortcutItem>();
        public List<ISN_UIUrlType> UrlTypes = new List<ISN_UIUrlType>();

        //--------------------------------------
        // UIKit Settings
        //--------------------------------------

        public List<ISN_UIUrlType> ApplicationQueriesSchemes = new List<ISN_UIUrlType>();

        public bool CameraUsageDescriptionEnabled = true;
        public string CameraUsageDescription {
            get {
               return GetPlistKeyValue("NSCameraUsageDescription", "Please change 'Camera Usage Description' with IOS Native UI Kit Editor Settings", CameraUsageDescriptionEnabled);
            }

            set {
                SetPlistKeyValue("NSCameraUsageDescription", value, CameraUsageDescriptionEnabled);  
            }
        }


        public bool MediaLibraryUsageDescriptionEnabled = false;
        public string MediaLibraryUsageDescription {
            get {
                return GetPlistKeyValue("NSAppleMusicUsageDescription", "Please change 'Media Library Usage Description' with IOS Native Media Player Editor Settings", PhotoLibraryUsageDescriptionEnabled);
            }

            set {
                SetPlistKeyValue("NSAppleMusicUsageDescription", value, PhotoLibraryUsageDescriptionEnabled);
            }
        }

        public bool PhotoLibraryUsageDescriptionEnabled = true;
        public string PhotoLibraryUsageDescription {
            get {
                return GetPlistKeyValue("NSPhotoLibraryUsageDescription", "Please change 'Photo Library Usage Description' with IOS Native UI Kit Editor Settings", PhotoLibraryUsageDescriptionEnabled);
            }

            set {
                SetPlistKeyValue("NSPhotoLibraryUsageDescription", value, PhotoLibraryUsageDescriptionEnabled);
            }
        }

        public bool PhotoLibraryAddUsageDescriptionEnabled = true;
        public string PhotoLibraryAddUsageDescription {
            get {
                return GetPlistKeyValue("NSPhotoLibraryAddUsageDescription", "Please change 'Photo Library Add Usage Description' with IOS Native UI Kit Editor Settings", PhotoLibraryAddUsageDescriptionEnabled);
            }

            set {
                SetPlistKeyValue("NSPhotoLibraryAddUsageDescription", value, PhotoLibraryAddUsageDescriptionEnabled);
            }
        }

        public bool MicrophoneUsageDescriptionEnabled = true;
        public string MicrophoneUsageDescription {
            get {
                return GetPlistKeyValue("NSMicrophoneUsageDescription", "Please change 'Microphone Usage Description' with IOS Native UI Kit Editor Settings", MicrophoneUsageDescriptionEnabled);
            }

            set {
                SetPlistKeyValue("NSMicrophoneUsageDescription", value, MicrophoneUsageDescriptionEnabled);
            }
        }



        private string GetPlistKeyValue(string key, string defaultValue, bool enabled) {
            if(!enabled) {
                return defaultValue;
            }
            var plistKey = ISD_API.GetInfoPlistKey(key);
            if(plistKey == null) {
                plistKey = new ISD_PlistKey();
                plistKey.Name = key;
                plistKey.StringValue = defaultValue;
                plistKey.Type = ISD_PlistKeyType.String;
                ISD_API.SetInfoPlistKey(plistKey);
            } 

            return plistKey.StringValue;
        }

        private void SetPlistKeyValue(string key, string val, bool enabled) {
            if(!enabled) {
                return;
            }

            if(!val.Equals(GetPlistKeyValue(key, val, enabled))) {
                //we are sure it's not null
                ISD_API.GetInfoPlistKey(key).StringValue = val;
            }
        }

        //--------------------------------------
        // Contacts Settings
        //--------------------------------------


        public string ContactsUsageDescription = "Please change 'Contacts Usage Description' with IOS Native Contacts Editor Settings";

        //--------------------------------------
        // Core Location
        //--------------------------------------

         public string LocationAlwaysAndWhenInUseUsageDescription = "Please change 'Location Always And When In Use Usage Descriptionn' with IOS Native Contacts Editor Settings";
         public string LocationWhenInUseUsageDescription = "Please change 'Location When In Use Usage Descriptionn' with IOS Native Contacts Editor Settings";


        //--------------------------------------
        // SA_ScriptableSettings
        //--------------------------------------


        protected override string BasePath {
            get { return IOS_NATIVE_FOLDER; }
        }


        public override string PluginName {
            get {
                return PLUGIN_NAME;
            }
        }

        public override string DocumentationURL {
            get {
                return DOCUMENTATION_URL;
            }
        }


        public override string SettingsUIMenuItem {
            get {
                return SA_Config.EDITOR_MENU_ROOT + "iOS/Services";
            }
        }
    }
}
