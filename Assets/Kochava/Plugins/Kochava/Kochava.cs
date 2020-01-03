// Build our defines to assign each platform in simple way.
#if UNITY_ANDROID && !UNITY_EDITOR
#define ANDROID
#endif
#if UNITY_IPHONE && !UNITY_EDITOR
#define IOS
#endif
#if UNITY_WSA_10_0 && !UNITY_EDITOR
#define WINDOWS_UWP
#endif
#if WINDOWS_UWP && ENABLE_WINMD_SUPPORT
#define WINDOWS_UWP_SUPPORTED
#endif

// Assign a define that encompasses all wrapped natives vs using the Unity implementation.
#if IOS || ANDROID || WINDOWS_UWP
#define WRAPPED_NATIVES      
#endif

using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using UnityEngine.Networking;

// Main entrypoint to the Kochava SDK.
public class Kochava : MonoBehaviour {
    
    // SDK Version Constants
	private const bool isUnityStoreBuild = false;
	private const string sdkVersionNumber = "4.0.5";
	private const string sdkNativeName = isUnityStoreBuild ? "Unity NET Store" : "Unity NET";
	private const string sdkName = isUnityStoreBuild ? "Unity Store" : "Unity";
	private const string sdkBuildDate = "2019-09-11T17:50:36Z";

	// Logging levels
    public enum DebugLogLevel
    {
        none = 0,
        error = 1,
        warn = 2,
        info = 3,
        debug = 4,
        trace = 5
    }

	// standard event types
	public enum EventType
	{
		Achievement,
		AddToCart,
		AddToWishList,
		CheckoutStart,
		LevelComplete,
		Purchase,
		Rating,
		RegistrationComplete,
		Search,
		TutorialComplete,
		View,
		AdView,
		PushReceived,
		PushOpened,
        ConsentGranted,
	    DeepLink,
	    AdClick,
	    StartTrial,
	    Subscribe
	}
    
    // central attribution callback for all platforms
    public delegate void AttributionCallbackDelegate(string response);
	
    // central consent status callback for all platforms
    public delegate void ConsentStatusChangeDelegate();
    
    // Instance and Singleton Management
    private static Kochava instance = null;
    private string instanceId = System.Guid.NewGuid().ToString().Substring(0, 5)+"-";
    private GameObject kochavaGameObject = null;
    private static bool initialized = false;
    
#if ANDROID
    private static AndroidJavaObject androidTracker = null;
#endif

#if WINDOWS_UWP_SUPPORTED
    private static KochavaUWP.Tracker windowsTracker = null;
#endif
    
    // Input parameter backing fields
    private static string _appGUID = "";
    private static bool _appAdLimitTracking = false;
	private static bool _appAdLimitTrackingSet = false;
    private static bool _sleep = false;
    private static int _logLevel;
    private static string _partnerName = "";
	private static string _partnerApp = "";
    private static bool _requestAttributionCallback = false;
    private static bool _intelligentConsentManagement = false;
    private static bool _manualManagedConsentRequirements = false;
    private static Dictionary<string, object> _dictionaryCustom = new Dictionary<string, object>();
    private static AttributionCallbackDelegate _attributionCallbackDelegate;
    private static ConsentStatusChangeDelegate _consentStatusChangeDelegate;
#if !WRAPPED_NATIVES
    private static string _productName = "";
#endif
    
    // log message level
    private const int LOG_NONE = (int)DebugLogLevel.none;
    private const int LOG_ERROR = (int)DebugLogLevel.error;
    private const int LOG_WARN = (int)DebugLogLevel.warn;
    private const int LOG_INFO = (int)DebugLogLevel.info;
    private const int LOG_DEBUG = (int)DebugLogLevel.debug;
    private const int LOG_TRACE = (int)DebugLogLevel.trace;
    
    // generic implementation tracker
    public static class Tracker
    {
        // static configuration object - host app will prepare the tracker using this
        public static class Config
        {
            // setters for configuration values
            public static void SetAppGuid(string value)
            {
                if (CheckInitialized()) return;
                if (string.IsNullOrEmpty(value)) return;
                _appGUID = value;
                LOG("Config: appGUID = " + _appGUID, LOG_DEBUG);
            }

            public static void SetPartnerName(string value)
            {
                if (CheckInitialized()) return;
                if (string.IsNullOrEmpty(value)) return;
                _partnerName = value;
                LOG("Config: partnerName = " + value, LOG_DEBUG);
            }

			public static void SetPartnerApp(string value)
			{
				if (CheckInitialized()) return;
				if (string.IsNullOrEmpty(value)) return;
				_partnerApp = value;
				LOG("Config: partnerApp = " + value, LOG_DEBUG);
			}

            public static void SetLogLevel(DebugLogLevel value)
            {
                if (CheckInitialized()) return;
                _logLevel = (int)value;
                LOG("Config: debugLogLevel = " + _logLevel.ToString(), LOG_DEBUG);                
            }

            public static void SetAppLimitAdTracking(bool value)
            {
                if (CheckInitialized()) return;
                _appAdLimitTracking = value;
				_appAdLimitTrackingSet = true;
                LOG("Config: appAdLimitTracking = " + _appAdLimitTracking.ToString(), LOG_DEBUG);                
            }

            public static void SetRetrieveAttribution(bool value)
            {
                if (CheckInitialized()) return;
                _requestAttributionCallback = value;
                LOG("Config: requestAttributionCallback = " + _requestAttributionCallback.ToString(), LOG_DEBUG);
            }

            // add a custom key/value pair to the config dictionary
            public static void SetCustomKeyValuePair(string key, object value)
            {
                if (CheckInitialized()) return;
                if(string.IsNullOrEmpty(key) || value == null) return;
				_dictionaryCustom [key] = value;
                LOG("Config: custom: " + key + " = " + value.ToString(), LOG_DEBUG);
            }

			public static void SetIntelligentConsentManagement(bool value)
			{
				if (CheckInitialized()) return;
				_intelligentConsentManagement = value;
				LOG("Config: consentIntelligentManagement = " + _intelligentConsentManagement.ToString(), LOG_DEBUG);  
			}

            public static void SetManualManagedConsentRequirements(bool value)
            {
                if (CheckInitialized()) return;
                _manualManagedConsentRequirements = value;
                LOG("Config: consentManualManagedRequirements = " + _manualManagedConsentRequirements.ToString(), LOG_DEBUG);  
            }

            public static void SetSleep(bool value)
            {
                if (CheckInitialized()) return;
                _sleep = value;
                LOG("Config: sleep = " + _sleep.ToString(), LOG_DEBUG);    
            }

            // helper function that logs if the tracker was already initialized
            private static bool CheckInitialized()
            {
                if (instance == null) return false;
                LOG("Config: Kochava Already Initialized.", LOG_DEBUG);
                return true;
            }
        }

        // initialization
        public static void Initialize()
        {
			if (isUnityStoreBuild) {
				//Check if Unity Analytics is enabled as the SDK is not permitted to be initialized without it.
				bool unityAnalyticsEnabled = false;
#if UNITY_ANALYTICS
			unityAnalyticsEnabled = UnityEngine.Analytics.Analytics.enabled;
#endif
				if (!unityAnalyticsEnabled) {
					LOG ("CANNOT INITIALIZE KOCHAVA - Unity Analytics must be both enabled and active for this project in order to utilize the Kochava SDK.", LOG_ERROR);
					return;
				}
			}

            // make sure config input parameters exist
#if WRAPPED_NATIVES
			if (string.IsNullOrEmpty(_appGUID) && string.IsNullOrEmpty(_partnerName))
			{
				LOG("CANNOT INITIALIZE KOCHAVA - App GUID or Partner Name Not Set", LOG_ERROR);
				return;
			}
#else
			if (string.IsNullOrEmpty(_appGUID) && (string.IsNullOrEmpty(_partnerName) || string.IsNullOrEmpty(_partnerApp)))
			{
				LOG("CANNOT INITIALIZE KOCHAVA - App GUID or Partner Name and App Not Set", LOG_ERROR);
				return;
			}
#endif

            if (instance == null)
            {
                GameObject temp = new GameObject();
                temp.name = "KochavaTracker";
                DontDestroyOnLoad(temp);
                instance = temp.AddComponent<Kochava>();
                instance.kochavaGameObject = temp;
            }
            else
            {
                LOG("ALREADY INITIALIZED KOCHAVA - Kochava can only be initialized once.", LOG_DEBUG);
                return;
            }

#if ANDROID
            LOG("Initializing Kochava native Android library", LOG_INFO);
			androidNativeStart(_appGUID, _partnerName, _logLevel, _appAdLimitTracking, _appAdLimitTrackingSet, _sleep, _requestAttributionCallback, _intelligentConsentManagement, _manualManagedConsentRequirements, _dictionaryCustom);
#elif IOS
            LOG("Initializing Kochava native iOS library", LOG_INFO);
			iOSNativeStart(_appGUID, _partnerName, _logLevel, _appAdLimitTracking, _appAdLimitTrackingSet, _sleep, _requestAttributionCallback, _intelligentConsentManagement, _manualManagedConsentRequirements, _dictionaryCustom);
#elif WINDOWS_UWP
            LOG("Initializing Kochava native Windows Store (UWP) library", LOG_INFO);
			windowsUwpNativeStart(_appGUID, _partnerName, _logLevel, _appAdLimitTracking, _appAdLimitTrackingSet, _sleep, _requestAttributionCallback, _intelligentConsentManagement, _manualManagedConsentRequirements, _dictionaryCustom);
#else
            LOG("Initializing Kochava Unity Net", LOG_INFO);
            instance.UnityNetStart();
#endif
            initialized=true;
        }

        //DeInitializes the Tracker
        private static void DeInitialize()
        {
            // Input Parameters
            _appGUID = "";
            _appAdLimitTracking = false;
			_appAdLimitTrackingSet = false;
            _sleep = false;
            _logLevel = LOG_INFO;
            _partnerName = "";
			_partnerApp = "";
            _requestAttributionCallback = false;
            _intelligentConsentManagement = false;
            _manualManagedConsentRequirements = false;
            _dictionaryCustom = new Dictionary<string, object>();
            _attributionCallbackDelegate = null;
            _consentStatusChangeDelegate = null;
            
            // Unity Net extras
#if !WRAPPED_NATIVES
            _productName = "";
            Settings.apiURLInit = "https://kvinit-prod.api.kochava.com";
            Settings.apiURLTracker = "https://control.kochava.com";
            Settings.apiURLQuery = "https://control.kochava.com";
            Settings.trackerURL = "/track/json";
            Settings.initURL = "/track/kvinit";
            Settings.queryURL = "/track/kvquery";
#endif
            
            // Reset the singleton instance.
#if ANDROID
            androidTracker = null;
#endif
#if WSA
            if(windowsTracker != null) windowsTracker.SendEvent("___expires", "1");                            
            if(_requestAttributionCallback == true) 
            {
                // unsubscribe from attribution event
                KochavaUWP.Tracker.ReceivedAttributionEvent -= instance.KochavaUWPAttributionListener;                
            }            
            if (_logLevel != 0)
            {
                // unsubscribe from log event
                KochavaUWP.Tracker.LogUpdateEvent -= KochavaUWPLogListener;                
            }
            windowsTracker = null;
#endif
            initialized = false;
            if(instance != null && instance.kochavaGameObject != null)
            {
                GameObject.Destroy (instance.kochavaGameObject);
                instance.kochavaGameObject = null;
            }
            instance = null;
        }

        // set the attribution handler
        public static void SetAttributionHandler(AttributionCallbackDelegate callback)
        {
            _attributionCallbackDelegate = callback;
            LOG("Config: attribution handler set", LOG_DEBUG);
        }

		// set the consent status change handler
		public static void SetConsentStatusChangeHandler(ConsentStatusChangeDelegate callback)
		{
			_consentStatusChangeDelegate = callback;
			LOG("Config: consent status change handler set", LOG_DEBUG);
		}

        // send event with name only
        public static void SendEvent(string eventName)
        {
            SendEvent(eventName, "");
        }
        
        // send event with name and extra data (usually json)
        public static void SendEvent(string eventName, string eventData)
        {
            if (CheckTrackerInitialized("SendEvent()")) return;
			if (string.IsNullOrEmpty (eventName))
			{
				LOG("Invalid Event Name - Cannot send event", LOG_WARN);
				return;
			}
#if ANDROID
            androidNativeSendEvent(eventName, eventData ?? "");
#elif IOS
            iOSNativeSendEvent(eventName, eventData ?? "");
#elif WINDOWS_UWP
            windowsUwpNativeSendEvent(eventName, eventData ?? "");
#else
            instance.IngestEvent(eventName, eventData ?? "", "");
#endif
        }

        // send standard event
        public static void SendEvent(Event kochavaEvent)
        {
            if (CheckTrackerInitialized("SendEvent(kochavaEvent)")) return;
			if (kochavaEvent == null || string.IsNullOrEmpty(kochavaEvent.getEventName()))
			{
				LOG("Invalid Event Name - Cannot send event", LOG_WARN);
				return;
			}
#if ANDROID
			androidNativeSendKochavaEvent(kochavaEvent.getEventName(), kochavaEvent.getDictionary(),
					kochavaEvent.getReceiptDataFromGoogleStore(), kochavaEvent.getReceiptDataSignatureFromGoogleStore());
#elif IOS
			iOSNativeSendKochavaEvent(kochavaEvent.getEventName(), Json.ToString(kochavaEvent.getDictionary()), kochavaEvent.getAppStoreReceiptBase64EncodedString());
#elif WINDOWS_UWP
            windowsUwpNativeSendKochavaEvent(kochavaEvent.getEventName(), kochavaEvent.getDictionary());
#else
            instance.IngestEvent(kochavaEvent);
#endif
        }
        
        // send a deeplink
        public static void SendDeepLink(string deepLinkURI, string sourceApplication = "")
        {
            if (CheckTrackerInitialized("SendDeepLink()")) return;
#if ANDROID
            androidNativeSendDeepLink(deepLinkURI, sourceApplication);
#elif IOS
            iOSNativeSendDeepLink(deepLinkURI, sourceApplication);
#elif WINDOWS_UWP
            windowsUwpNativeSendDeepLink(deepLinkURI, sourceApplication);
#else
            instance.IngestDeeplink(deepLinkURI, sourceApplication);
#endif
        }
        
        // getter - attribution
        public static string GetAttribution()
        {
            if (CheckTrackerInitialized("GetAttribution()")) return "";
#if ANDROID
            return androidNativeGetAttribution();
#elif IOS
            return iOSNativeGetAttributionString();
#elif WINDOWS_UWP
            return windowsUwpNativeGetAttribution();
#else
            if (PlayerPrefs.HasKey(Settings.keyAttribution)) return PlayerPrefs.GetString(Settings.keyAttribution);
            return "";
#endif
        }

        // getter - device id
        public static string GetDeviceId()
        {
            if (CheckTrackerInitialized("GetDeviceId()")) return "";
#if ANDROID
            return androidNativeGetDeviceId();
#elif IOS
            return iOSNativeGetDeviceId();
#elif WINDOWS_UWP
            return windowsUwpNativeGetDeviceId();
#else
            if (instance.profile != null && instance.profile.kochavaDeviceID != null) return instance.profile.kochavaDeviceID;
            return "";
#endif
        }
        
        // setter - LAT
        public static void SetAppLimitAdTracking(bool desiredAppLimitAdTracking)
        {
            if (CheckTrackerInitialized("SetAppLimitAdTracking()")) return;
#if ANDROID
            androidNativeSetAppLimitAdTrackingBool(desiredAppLimitAdTracking);
#elif IOS
            iOSNativeSetAppLimitAdTrackingBool(desiredAppLimitAdTracking);
#elif WINDOWS_UWP
            windowsUwpNativeSetAppLimitAdTrackingBool(desiredAppLimitAdTracking);
#else
            _appAdLimitTracking = desiredAppLimitAdTracking;
			_appAdLimitTrackingSet = true;
            instance.CheckWatchlist();
#endif
        }
        
        // setter - sleep
        public static void SetSleep(bool sleep)
        {
            if (CheckTrackerInitialized("SetSleep()")) return;
#if ANDROID
            androidNativeSetSleepBool(sleep);
#elif IOS
            iOSNativeSetSleepBool(sleep);
#elif WINDOWS_UWP
            windowsUwpNativeSetSleepBool(sleep);
#else
            LOG("SetSleep() Sleep is not supported on this platform.", LOG_ERROR);
#endif
        }
        
        // getter - sleep
        public static bool GetSleep()
        {
            if (CheckTrackerInitialized("GetSleep()")) return false;
#if ANDROID
            return androidNativeGetSleepBool();
#elif IOS
            return iOSNativeGetSleepBool();
#elif WINDOWS_UWP
            return windowsUwpNativeGetSleepBool();
#else
            LOG("GetSleep() Sleep is not supported on this platform.", LOG_ERROR);
            return false;
#endif
        }
        
        // setter - identityLink
        public static void SetIdentityLink(Dictionary<string, string> identityLinkDictionary)
        {
            if (CheckTrackerInitialized("SetIdentityLink()")) return;
#if ANDROID
            androidNativeSendIdentityLink(identityLinkDictionary);
#elif IOS
            iOSNativeSendIdentityLink(Json.ToString(identityLinkDictionary));
#elif WINDOWS_UWP
            windowsUwpNativeSendIdentityLink(identityLinkDictionary);
#else
            instance.IngestIdentityLink(identityLinkDictionary);
#endif
        }

        // getter - version
        public static string GetVersion()
        {
            if (CheckTrackerInitialized("getVersion()")) return "";
#if ANDROID
            return androidNativeGetVersion();
#elif IOS
            return iOSNativeGetVersion();
#elif WINDOWS_UWP
            return windowsUwpNativeGetVersion();
#else
            return Settings.sdkVersion;
#endif
        }
        
        // add a push token with byte array
        public static void AddPushToken(byte[] token)
        {
            if (CheckTrackerInitialized("AddPushToken(byte[])")) return;
#if IOS
            iOSNativeAddPushToken(token, token != null ? (UInt64)token.Length : 0);
#else
            string tokenString = BitConverter.ToString(token).Replace("-", string.Empty);
            AddPushToken(tokenString);
#endif
        }
        
        // add a push token with string
        public static void AddPushToken(string token)
        {
            if (CheckTrackerInitialized("AddPushToken(string)")) return;
#if ANDROID
            androidNativeAddPushToken(token);
#elif IOS
            AddPushToken(Util.hexStringToByteArr(token));
#elif WINDOWS_UWP
            LOG("AddPushToken(string) Push tokens are not supported on this platform.", LOG_ERROR);
#else
            LOG("AddPushToken(string) Push tokens are not supported on this platform.", LOG_ERROR);
#endif
        }
        
        // remove a push token with byte array
        public static void RemovePushToken(byte[] token)
        {
            if (CheckTrackerInitialized("RemovePushToken(byte[])")) return;
#if IOS
            iOSNativeRemovePushToken(token, token != null ? (UInt64)token.Length : 0);
#else
            string tokenString = BitConverter.ToString(token).Replace("-", string.Empty);
            RemovePushToken(tokenString);
#endif
        }
        
        // remove a push token with string
        public static void RemovePushToken(string token)
        {
            if (CheckTrackerInitialized("RemovePushToken(string)")) return;
#if ANDROID
            androidNativeRemovePushToken(token);
#elif IOS
            RemovePushToken(Util.hexStringToByteArr(token));
#elif WINDOWS_UWP
            LOG("RemovePushToken(string) Push tokens are not supported on this platform.", LOG_ERROR);
#else
            LOG("RemovePushToken(string) Push tokens are not supported on this platform.", LOG_ERROR);
#endif
        }
  
        public static bool GetConsentRequired()
        {
            if (CheckTrackerInitialized("GetConsentRequired()")) return true;
#if ANDROID
            return androidNativeGetConsentRequired();
#elif IOS
            return iOSNativeGetConsentRequired();
#elif WINDOWS_UWP
            return windowsUwpNativeGetConsentRequired();
#else
            LOG("GetConsentRequired() Kochava Intelligent Consent Management is not supported on this platform.", LOG_ERROR);
#endif
            return true;
        }
        
        public static void SetConsentRequired(bool isRequired)
        {
            if (CheckTrackerInitialized("SetConsentRequired(bool)")) return;
#if ANDROID
            androidNativeSetConsentRequired(isRequired);
#elif IOS
            iOSNativeSetConsentRequired(isRequired);
#elif WINDOWS_UWP
            windowsUwpNativeSetConsentRequired(isRequired);
#else
            LOG("SetConsentRequired(bool) Kochava Intelligent Consent Management is not supported on this platform.", LOG_ERROR);
#endif
        }

        public static void SetConsentGranted(bool isGranted)
        {
            if (CheckTrackerInitialized("SetConsentGranted(bool)")) return;
#if ANDROID
            androidNativeSetConsentGranted(isGranted);
#elif IOS
            iOSNativeSetConsentGranted(isGranted);
#elif WINDOWS_UWP
            windowsUwpNativeSetConsentGranted(isGranted);
#else
            LOG("SetConsentGranted(bool) Kochava Intelligent Consent Management is not supported on this platform.", LOG_ERROR);
#endif
        }

        public static bool GetConsentGranted()
        {
            if (CheckTrackerInitialized("GetConsentGranted()")) return false;
#if ANDROID
            return androidNativeGetConsentGranted();
#elif IOS
            return iOSNativeGetConsentGranted();
#elif WINDOWS_UWP
            return windowsUwpNativeGetConsentGranted();
#else
            LOG("GetConsentGranted() Kochava Intelligent Consent Management is not supported on this platform.", LOG_ERROR);
            return false;
#endif
        }

        public static bool GetConsentGrantedOrNotRequired()
        {
            return GetConsentGranted() || !GetConsentRequired();
        }

        public static bool GetConsentShouldPrompt()
        {
            if (CheckTrackerInitialized("GetConsentShouldPrompt()")) return false;
#if ANDROID
            return androidNativeGetConsentShouldPrompt();
#elif IOS
            return iOSNativeGetConsentShouldPrompt();
#elif WINDOWS_UWP
            return windowsUwpNativeGetConsentShouldPrompt();
#else
            LOG("GetConsentShouldPrompt() Kochava Intelligent Consent Management is not supported on this platform.", LOG_ERROR);
            return false;
#endif
        }

        public static void SetConsentPrompted()
        {
            if (CheckTrackerInitialized("SetConsentPrompted()")) return;
#if ANDROID
            androidNativeSetConsentPrompted();
#elif IOS
            iOSNativeSetConsentPrompted();
#elif WINDOWS_UWP
            windowsUwpNativeSetConsentPrompted();
#else
            LOG("SetConsentPrompted() Kochava Intelligent Consent Management is not supported on this platform.", LOG_ERROR);
#endif
        }

        public static string GetConsentPartnersJson()
        {
            if (CheckTrackerInitialized("GetConsentPartnersJson()")) return "[]";
#if ANDROID
            return androidNativeGetConsentPartnersJson();
#elif IOS
            return iOSNativeGetConsentPartnersJson();
#elif WINDOWS_UWP
            return windowsUwpNativeGetConsentPartnersJson();
#else
            LOG("GetConsentPartnersJson() Kochava Intelligent Consent Management is not supported on this platform.", LOG_ERROR);
            return "";
#endif
        }

		public static ConsentPartner[] GetConsentPartners()
		{
			if (CheckTrackerInitialized("GetConsentPartners()")) return new ConsentPartner[0];
#if ANDROID
			return androidNativeGetConsentPartners();
#elif IOS
			return iOSNativeGetConsentPartners();
#elif WINDOWS_UWP
			return windowsUwpNativeGetConsentPartners();
#else
			LOG("GetConsentPartners() Kochava Intelligent Consent Management is not supported on this platform.", LOG_ERROR);
			return new ConsentPartner[0];
#endif
		}
        
        public static string GetConsentDescription()
        {
            if (CheckTrackerInitialized("GetConsentDescription()")) return "";
#if ANDROID
            return androidNativeGetConsentDescription();
#elif IOS
            return iOSNativeGetConsentDescription();
#elif WINDOWS_UWP
            return windowsUwpNativeGetConsentDescription();
#else
            LOG("GetConsentDescription() Kochava Intelligent Consent Management is not supported on this platform.", LOG_ERROR);
            return "";
#endif
        }

        public static long GetConsentResponseTime()
        {
            if (CheckTrackerInitialized("GetConsentResponseTime()")) return 0;
#if ANDROID
            return androidNativeGetConsentResponseTime();
#elif IOS
            return iOSNativeGetConsentResponseTime();
#elif WINDOWS_UWP
            return windowsUwpNativeGetConsentResponseTime();
#else
            LOG("GetConsentPartners() Kochava Intelligent Consent Management is not supported on this platform.", LOG_ERROR);
            return 0;
#endif
        }

        public static bool GetConsentRequirementsKnown()
        {
            if (CheckTrackerInitialized("GetConsentRequirementsKnown()")) return false;
#if ANDROID
            return androidNativeGetConsentRequirementsKnown();
#elif IOS
            return iOSNativeGetConsentRequirementsKnown();
#elif WINDOWS_UWP
            return windowsUwpNativeGetConsentRequirementsKnown();
#else
            LOG("GetConsentRequirementsKnown() Kochava Intelligent Consent Management is not supported on this platform.", LOG_ERROR);
            return false;
#endif
        }

        // helper to check whether we've completed initialization
        private static bool CheckTrackerInitialized(string sourceName)
        {
            if (!initialized)
            {
                Debug.Log("KOCHAVA ERROR: NOT YET INITIALIZED: " + sourceName);
                return true;
            }
#if ANDROID
            if(androidTracker == null)
            {
                Debug.Log("KOCHAVA ERROR: NOT YET INITIALIZED (androidTracker invalid): " + sourceName);
                return true;
            }
#endif
#if WSA
            if(windowsTracker == null)
            {
                Debug.Log("KOCHAVA ERROR: NOT YET INITIALIZED (windowsTracker invalid): " + sourceName);
                return true;
            }
#endif
            return false;
        }

    }
	
	// Represents an individual consent partner when using Intelligent Consent Management.
	public class ConsentPartner
	{
		public readonly string name;
		public readonly string description;
		public readonly bool granted;
		public readonly long responseTime;

		internal ConsentPartner(Dictionary<string, object> json)
		{
			object nameObject;
			object descriptionObject;
			object grantedObject;
			object responseTimeObject;

			json.TryGetValue("name", out nameObject);
			json.TryGetValue("description", out descriptionObject);
			json.TryGetValue("granted", out grantedObject);
			json.TryGetValue("response_time", out responseTimeObject);

			name = ((string) nameObject) ?? "";
			description = ((string) descriptionObject) ?? "";
			granted = (bool) (grantedObject ?? false);
			if(responseTimeObject is int)
			{
				responseTime =  (int) (responseTimeObject ?? 0);
			} else if(responseTimeObject is long)
			{
				responseTime =  (long) (responseTimeObject ?? 0L);
			} else
			{
				responseTime = 0;
			}
		}
	}

    // represents a standard event to be used when sending events
    public class Event
    {
        // event name and dictionary backer to be used
        private string eventName;
        private Dictionary<string, object> dictionary;
		private string appStoreReceiptBase64EncodedString = null;
		private string receiptDataFromGoogleStore = null;
		private string receiptDataSignatureFromGoogleStore = null;

        // getters
        public string getEventName()
        {
            return eventName;
        }

        public Dictionary<string, object> getDictionary()
        {
            return dictionary;
        }

		public string getAppStoreReceiptBase64EncodedString()
		{
			return appStoreReceiptBase64EncodedString;
		}

		public string getReceiptDataFromGoogleStore()
		{
			return receiptDataFromGoogleStore;
		}

		public string getReceiptDataSignatureFromGoogleStore()
		{
			return receiptDataSignatureFromGoogleStore;
		}

        public Event(EventType standardEventType)
        {
            dictionary = new Dictionary<string, object>();
            if (standardEventType == EventType.Achievement) eventName = "Achievement";
            else if (standardEventType == EventType.AddToCart) eventName = "Add to Cart";
            else if (standardEventType == EventType.AddToWishList) eventName = "Add to Wish List";
            else if (standardEventType == EventType.CheckoutStart) eventName = "Checkout Start";
            else if (standardEventType == EventType.LevelComplete) eventName = "Level Complete";
            else if (standardEventType == EventType.Purchase) eventName = "Purchase";
            else if (standardEventType == EventType.Rating) eventName = "Rating";
            else if (standardEventType == EventType.RegistrationComplete) eventName = "Registration Complete"; 
            else if (standardEventType == EventType.Search) eventName = "Search";
            else if (standardEventType == EventType.TutorialComplete) eventName = "Tutorial Complete";
            else if (standardEventType == EventType.View) eventName = "View";
            else if (standardEventType == EventType.AdView) eventName = "Ad View";
            else if (standardEventType == EventType.PushReceived) eventName = "Push Received";
            else if (standardEventType == EventType.PushOpened) eventName = "Push Opened";
            else if (standardEventType == EventType.ConsentGranted) eventName = "Consent Granted";
            else if (standardEventType == EventType.DeepLink) eventName = "_Deeplink";
            else if (standardEventType == EventType.AdClick) eventName = "Ad Click";
            else if (standardEventType == EventType.StartTrial) eventName = "Start Trial";
            else if (standardEventType == EventType.Subscribe) eventName = "Subscribe";
        }

        public Event(string customEventType)
        {
            dictionary = new Dictionary<string, object>();
            eventName = customEventType ?? "";
        }

        // setters for each parameter (universal)
        public string checkoutAsGuest { set { dictionary.Add("checkout_as_guest", value); } }
        public string contentId { set { dictionary.Add("content_id", value); } }
        public string contentType { set { dictionary.Add("content_type", value); } }
        public string currency { set { dictionary.Add("currency", value); } }
        public string dateString { set { dictionary.Add("date", value); } }
        public DateTime date { set { dictionary.Add("date", value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffK")); } } // convert to a string in ISO 8601 format
        public string description { set { dictionary.Add("description", value); } }
        public string destination { set { dictionary.Add("destination", value); } }
        public double duration { set { dictionary.Add("duration", value); } }
        public string endDateString { set { dictionary.Add("end_date", value); } }
        public DateTime endDate { set { dictionary.Add("end_date", value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffK")); } } // convert to a string in ISO 8601 format     
        public string itemAddedFrom { set { dictionary.Add("item_added_from", value); } }
        public string level { set { dictionary.Add("level", value); } }
        public double maxRatingValue { set { dictionary.Add("max_rating_value", value); } }
        public string name { set { dictionary.Add("name", value); } }
        public string orderId { set { dictionary.Add("order_id", value); } }
        public string origin { set { dictionary.Add("origin", value); } }
        public double price { set { dictionary.Add("price", value); } }
        public double quantity { set { dictionary.Add("quantity", value); } }
        public double ratingValue { set { dictionary.Add("rating_value", value); } }
        public string receiptId { set { dictionary.Add("receipt_id", value); } }
        public string referralFrom { set { dictionary.Add("referral_from", value); } }
        public string registrationMethod { set { dictionary.Add("registration_method", value); } }
        public string results { set { dictionary.Add("results", value); } }
        public string score { set { dictionary.Add("score", value); } }
        public string searchTerm { set { dictionary.Add("search_term", value); } }
        public double spatialX { set { dictionary.Add("spatial_x", value); } }
        public double spatialY { set { dictionary.Add("spatial_y", value); } }
        public double spatialZ { set { dictionary.Add("spatial_z", value); } }
        public string startDateString { set { dictionary.Add("start_date", value); } }
        public DateTime startDate { set { dictionary.Add("start_date", value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffK")); } } // convert to a string in ISO 8601 format
        public string success { set { dictionary.Add("success", value); } }
        public string userId { set { dictionary.Add("user_id", value); } }
        public string userName { set { dictionary.Add("user_name", value); } }
        public string validated { set { dictionary.Add("validated", value); } }
        public bool background { set { dictionary.Add("background", value); } }
        public string action { set { dictionary.Add("action", value); } }
        public bool completed { set { dictionary.Add("completed", value); } }
		public Dictionary<string, object> payload { set { dictionary.Add("payload", value); } }
        public string uri { set { dictionary.Add("uri", value); } }
        public string source { set { dictionary.Add("source", value); } }
        // LTV params
        public string adNetworkName { set { dictionary.Add("ad_network_name", value); } }
        public string adMediationName { set { dictionary.Add("ad_mediation_name", value); } }
        public string adDeviceType { set { dictionary.Add("device_type", value); } }
        public string adPlacement { set { dictionary.Add("placement", value); } }
        public string adType { set { dictionary.Add("ad_type", value); } }
        public string adCampaignID { set { dictionary.Add("ad_campaign_id", value); } }
        public string adCampiagnName { set { dictionary.Add("ad_campaign_name", value); } }
        public string adSize { set { dictionary.Add("ad_size", value); } }
        public string adGroupName { set { dictionary.Add("ad_group_name", value); } }
        public string adGroupID { set { dictionary.Add("ad_group_id", value); } }

        // set a custom value
        public void SetCustomValue(string key, double value)    
        {
            dictionary.Add(key, value);
        }
        public void SetCustomValue(string key, bool value)
        {
            dictionary.Add(key, value);
        }
        public void SetCustomValue(string key, string value)
        {
            dictionary.Add(key, value);         
        }

        // platform-specific setters:

        // ANDROID only:
        public void setReceiptFromGooglePlayStore(string receiptDataFromStore, string receiptDataSignatureFromStore)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
				receiptDataFromGoogleStore = receiptDataFromStore;
				receiptDataSignatureFromGoogleStore = receiptDataSignatureFromStore;
            }
            else
            {
                LOG("Event safely ignoring setReceiptFromGooglePlayStore() (only available on Android)", LOG_WARN);
            }
        }

        // IOS only:
        public void setReceiptFromAppleAppStoreBase64EncodedString(string value)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
				appStoreReceiptBase64EncodedString = value;
            }
            else
            {
                LOG("Event safely ignoring setReceiptFromAppleAppStoreBase64EncodedString() (only available on iOS)", LOG_WARN);
            }
        }

    }

    // getter for initialized
    public static bool IsInitialized()
    {
        return initialized;
    }

    /// <summary>
    /// End of public interface, private implementation begins here
    /// </summary>

    // logging function
    private static void LOG(string msg, int msglogLevel)
    {
        if (_logLevel < msglogLevel) return;

        string outStr = "[Kochava][" + Util.TimeStamp() + "]";

        if (msglogLevel == LOG_ERROR) outStr += "[ERROR]";
        else if (msglogLevel == LOG_WARN) outStr += "[WARNING]";
        else if (msglogLevel == LOG_INFO) outStr += "[I]";
        else if (msglogLevel == LOG_DEBUG) outStr += "[D]";
        else if (msglogLevel == LOG_TRACE) outStr += "[T]";

        outStr += " " + msg;

        UnityEngine.Debug.Log(outStr);
    }

    // static utility functions
    private class Util
    {
        // gets next amount of time to wait after a failed network call
        public static double GetNetworkRetryTime(int failedCount)
        {
            if (failedCount <= 1)
            {
                return 3; // 3 seconds
            }
            else if (failedCount == 2)
            {
                return 10; // 10 seconds
            }
            else if (failedCount == 3)
            {
                return 30; // 30 seconds
            }
            else if (failedCount == 4)
            {
                return 60; // 1 minute
            }
            else
            {
                return 300; // 5 minutes (limit)
            }
        }

        // gets current system time in human readable format (for logging)
        public static string TimeStamp()
        {
            return string.Format("{0:hh:mm:ss.fff}", System.DateTime.Now);
        }

        // get the current unix timestamp
        public static double UnixTime()
        {
            var epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
            return (System.DateTime.UtcNow - epochStart).TotalSeconds;
        }

        // add or replace a value within a dictionary with replacement awareness
        public static void SetValue(ref Dictionary<string, object> source, string key, object value, bool doNotReplace = true, bool doNotAllowEmptyString = true)
        {
            if (source != null && value != null)
            {
                try
                {
                    if (source.ContainsKey(key))
                    {
                        if (doNotReplace) return; // bail if this would be replacing
                                                  // ok to replace:
                        source.Remove(key);
                    }
                    // bail if we're not allowing empty strings?
                    if (doNotAllowEmptyString && value is string && value.ToString().Length < 1) return;
                    // add/update it                 
                    source.Add(key, value);
                }
                catch (Exception e)
                {
                    LOG("SetValue() failed for key=" + key + ": " + e.Message, LOG_ERROR);
                    return;
                }
            }
        }

        // helper to convert a hex string to a byte[]. If the string is invalid null will be returned.
        public static byte[] hexStringToByteArr(string hexString)
        {
            if (hexString == null || hexString.Length < 2 || (hexString.Length % 2) != 0)
            {
                return null;
            }
            try {
                byte[] raw = new byte[hexString.Length / 2];
                for (int i = 0; i < raw.Length; i++)
                {
                    raw[i] = System.Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                }
                return raw;
            } catch(Exception e) {
                return null;
            }
        }
        
    }

#if !WINDOWS_UWP // windows store does not import the JsonFX dll
    
    // handles all json parsing -- could be replaced with some other library
    // currently using JsonFx.Json.dll
    private static class Json
    {

        // json string to object
        public static T ToObj<T>(string jsonString)
        {
            return JsonFx.Json.JsonReader.Deserialize<T>(jsonString);
        }

        // object to json string
        public static string ToString(object jsonObject)
        {
            return JsonFx.Json.JsonWriter.Serialize(jsonObject);
        }

    }
    
#endif
    
#if WRAPPED_NATIVES
    //Scheduled callbacks to the host app.
	private readonly object cacheLock = new object();
	private string cacheAttributionCallback = null;
	private bool cacheConsentStatusChangeCallback = false;

	//Update method called every frame.
	void Update()
	{
		//If the callback occurred asynchronously its stored here and then we check for it on the update and then clear it.
		lock(cacheLock)
		{
			if (cacheAttributionCallback != null)
			{
				if(_attributionCallbackDelegate != null) _attributionCallbackDelegate(cacheAttributionCallback);
				cacheAttributionCallback = null;
			}

			if (cacheConsentStatusChangeCallback)
			{
				if (_consentStatusChangeDelegate != null) _consentStatusChangeDelegate();
				cacheConsentStatusChangeCallback = false;
			}
		}
	}
#endif
    
#if ANDROID // native Android SDK

    // internal attribution handler
    private class AndroidAttributionHandler : AndroidJavaProxy
    {
        public AndroidAttributionHandler() : base("com.kochava.base.AttributionUpdateListener") { }

        void onAttributionUpdated(string msg)
        {
            // pass msg along to generic attribution handler
            lock(instance.cacheLock)
            {
                instance.cacheAttributionCallback = msg;
            }
        }
    }

    // internal consent status change handler
    private class AndroidConsentStatusHandler : AndroidJavaProxy
    {
        public AndroidConsentStatusHandler() : base("com.kochava.base.ConsentStatusChangeListener") { }

        void onConsentStatusChange()
        {
            // pass msg along to generic attribution handler
            lock(instance.cacheLock)
            {
                instance.cacheConsentStatusChangeCallback = true;
            }
        }
    }
    
	private static void androidNativeStart(string appGuid, string partnerName, int logLevel, bool appLimitAdTracking, bool appLimitAdTrackingSet, bool sleep, bool retrieveAttribution, bool icm, bool icmManual, Dictionary<string, object> custom)
    {
        try
        {
            // Retrieve the Android Application Context.
            AndroidJavaObject context = null;
            using (AndroidJavaClass androidUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject androidActivity = androidUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                context = androidActivity.Call<AndroidJavaObject>("getApplicationContext");
            }

            // Get the Tracker and set the version extension.
            androidTracker = new AndroidJavaClass("com.kochava.base.Tracker");
            androidTracker.CallStatic("ext", new AndroidJavaObject("java.lang.String", sdkName + " " + sdkVersionNumber), new AndroidJavaObject("java.lang.String", sdkBuildDate));

            // Configure the Android Native SDK.
            using (AndroidJavaClass androidUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {                        
                // create the configuration object
                var kochavaConfig = new AndroidJavaObject("com.kochava.base.Tracker$Configuration", context);

                // Set all of the parameters.
                if(!string.IsNullOrEmpty(appGuid))
                {
                    kochavaConfig.Call<AndroidJavaObject>("setAppGuid", new AndroidJavaObject("java.lang.String", appGuid));
                }
                if(!string.IsNullOrEmpty(partnerName))
                {
                    kochavaConfig.Call<AndroidJavaObject>("setPartnerName", new AndroidJavaObject("java.lang.String", partnerName));
                }
                kochavaConfig.Call<AndroidJavaObject>("setLogLevel", logLevel);
				if(appLimitAdTrackingSet)
				{
					kochavaConfig.Call<AndroidJavaObject>("setAppLimitAdTracking", appLimitAdTracking);
				}
                kochavaConfig.Call<AndroidJavaObject>("setSleep", sleep);
                if(retrieveAttribution)
                {
                    kochavaConfig.Call<AndroidJavaObject>("setAttributionUpdateListener", new AndroidAttributionHandler());
                }
                kochavaConfig.Call<AndroidJavaObject>("setIntelligentConsentManagement", icm);
                kochavaConfig.Call<AndroidJavaObject>("setManualManagedConsentRequirements", icmManual);
                if(icm)
                {
                    kochavaConfig.Call<AndroidJavaObject>("setConsentStatusChangeListener", new AndroidConsentStatusHandler());
                }

                //Add custom
                if (custom != null && custom.Count > 0)
                {
                    foreach (var it in custom)
                    {
                        kochavaConfig.Call<AndroidJavaObject>("addCustom", new AndroidJavaObject("java.lang.String", it.Key), new AndroidJavaObject("java.lang.String", it.Value.ToString()));
                    }
                }

                // Configure
                androidTracker.CallStatic("configure", kochavaConfig);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("ERROR: Kochava Android initialization failed: " + e);
        }
    }

    private static void androidNativeSendEvent(string eventName, string eventData)
    {
        androidTracker.CallStatic("sendEvent", new AndroidJavaObject("java.lang.String", eventName ?? ""), new AndroidJavaObject("java.lang.String", eventData ?? ""));
    }

	private static void androidNativeSendKochavaEvent(string eventName, Dictionary<string, object> dictionary, string receiptJson, string receiptSignature)
    {
        // create the event
        AndroidJavaObject myEvent = new AndroidJavaObject("com.kochava.base.Tracker$Event", eventName);
        if(dictionary != null)
        {
            // check for receipt data:
			if (!string.IsNullOrEmpty (receiptJson) && !string.IsNullOrEmpty (receiptSignature)) {
				myEvent.Call<AndroidJavaObject>("setGooglePlayReceipt",
						new AndroidJavaObject("java.lang.String", receiptJson),
						new AndroidJavaObject("java.lang.String", receiptSignature));
			}

            // Add the dictionary of items.
            try
            {
                AndroidJavaObject eventData = new AndroidJavaObject("org.json.JSONObject", Json.ToString(dictionary));
                myEvent.Call<AndroidJavaObject>("addCustom", eventData);
            }
            catch (Exception e)
            {
                LOG("Kochava failed to add event parameters:" + e.Message, LOG_WARN);
            }
        }
        // call it
        androidTracker.CallStatic("sendEvent", myEvent);
    }

    private static void androidNativeSendDeepLink(string deepLinkURI, string sourceApplicationString)
    {
        AndroidJavaObject myEvent = new AndroidJavaObject("com.kochava.base.Tracker$Event", "_Deeplink");
        if(deepLinkURI != null)
        {
            myEvent.Call<AndroidJavaObject>("setUri", new AndroidJavaObject("java.lang.String", deepLinkURI));
        }
        if(sourceApplicationString != null)
        {
            myEvent.Call<AndroidJavaObject>("setSource", new AndroidJavaObject("java.lang.String", sourceApplicationString));
        }
        androidTracker.CallStatic("sendEvent", myEvent);
    }

    private static void androidNativeSendIdentityLink(Dictionary<string, string> identityLinkDictionary)
    {
        AndroidJavaObject identityLinkObj = new AndroidJavaObject("com.kochava.base.Tracker$IdentityLink");
        if(identityLinkDictionary != null)
        {
            foreach (var it in identityLinkDictionary)
            {
                identityLinkObj.Call<AndroidJavaObject>("add", new AndroidJavaObject("java.lang.String", it.Key ?? ""), new AndroidJavaObject("java.lang.String", it.Value ?? ""));
            }
        }
        androidTracker.CallStatic("setIdentityLink", identityLinkObj);
    }

    private static void androidNativeSetAppLimitAdTrackingBool(bool desiredAppLimitAdTracking)
    {
        androidTracker.CallStatic("setAppLimitAdTracking", desiredAppLimitAdTracking);
    }
    
    private static void androidNativeSetSleepBool(bool sleep)
    {
        androidTracker.CallStatic("setSleep", sleep);
    }
    
    private static bool androidNativeGetSleepBool()
    {
        return androidTracker.CallStatic<bool>("isSleep");
    }

    private static string androidNativeGetAttribution()
    {
        return androidTracker.CallStatic<string>("getAttribution");
    }

    private static string androidNativeGetDeviceId()
    {
        return androidTracker.CallStatic<string>("getDeviceId");
    }

    private static string androidNativeGetVersion()
    {
        return androidTracker.CallStatic<string>("getVersion");
    }

    private static void androidNativeAddPushToken(string token)
    {
        androidTracker.CallStatic("addPushToken", new AndroidJavaObject("java.lang.String", token ?? ""));
    }

    private static void androidNativeRemovePushToken(string token)
    {
        androidTracker.CallStatic("removePushToken", new AndroidJavaObject("java.lang.String", token ?? ""));
    }

    private static bool androidNativeGetConsentRequired()
    {
        return androidTracker.CallStatic<bool>("isConsentRequired");
    }
    
    private static void androidNativeSetConsentRequired(bool isRequired)
    {
        androidTracker.CallStatic("setConsentRequired", isRequired);
    }

    private static void androidNativeSetConsentGranted(bool isGranted)
    {
        androidTracker.CallStatic("setConsentGranted", isGranted);
    }

    private static bool androidNativeGetConsentGranted()
    {
        return androidTracker.CallStatic<bool>("isConsentGranted");
    }

    private static bool androidNativeGetConsentShouldPrompt()
    {
        return androidTracker.CallStatic<bool>("isConsentShouldPrompt");
    }

    private static void androidNativeSetConsentPrompted()
    {
        androidTracker.CallStatic("clearConsentShouldPrompt");
    }

    private static string androidNativeGetConsentPartnersJson()
    {
        return androidTracker.CallStatic<string>("getConsentPartnersJson");
    }

	private static ConsentPartner[] androidNativeGetConsentPartners()
	{
		string partnersSerialized = androidNativeGetConsentPartnersJson();
		Dictionary<string, object>[] partnersJson = Json.ToObj<Dictionary<string, object>[]> (partnersSerialized);
		ConsentPartner[] partners = new ConsentPartner[partnersJson.Length];
		for (int i = 0; i < partners.Length; i++) {
			partners[i] = new ConsentPartner(partnersJson[i]);
		}
		return partners;
	}

    private static string androidNativeGetConsentDescription()
    {
        return androidTracker.CallStatic<string>("getConsentDescription");
    }

    private static long androidNativeGetConsentResponseTime()
    {
        return androidTracker.CallStatic<long>("getConsentResponseTime");
    }

    private static bool androidNativeGetConsentRequirementsKnown()
    {
        return androidTracker.CallStatic<bool>("isConsentRequirementsKnown");
    }
    
#elif IOS // native iOS SDK

    //Callback method from the iOS .mm layer. Name and location cannot change.
    private void KochavaAttributionListener(string msg)
    {
        lock(instance.cacheLock)
        {
            instance.cacheAttributionCallback = msg;
        }
    }

    //Callback method from the iOS .mm layer. Name and location cannot change.
    private void KochavaConsentStatusChangeListener(string ignored)
    {
        lock(instance.cacheLock)
        {
            instance.cacheConsentStatusChangeCallback = true;
        }
    }

	private static void iOSNativeStart(string appGuid, string partnerName, int logLevel, bool appLimitAdTracking, bool appLimitAdTrackingSet, bool sleep, bool retrieveAttribution, bool icm, bool icmManual, Dictionary<string, object> custom)
    {
        Dictionary<string, object> config = new Dictionary<string, object>();
        if(!string.IsNullOrEmpty(appGuid))
        {
            config ["appGUIDString"] = appGuid;
        }
        if(!string.IsNullOrEmpty(partnerName))
        {
            config ["partnerName"] = partnerName;
        }
		switch (logLevel)
		{
			case 0:
				config ["logLevel"] = "never";
				break;
			case 1:
				config ["logLevel"] = "error";
				break;
			case 2:
				config ["logLevel"] = "warn";
				break;
			case 3:
				config ["logLevel"] = "info";
				break;
			case 4:
				config ["logLevel"] = "debug";
				break;
			case 5:
				config ["logLevel"] = "trace";
				break;
		}
		if(appLimitAdTrackingSet)
		{
			config ["limitAdTracking"] = appLimitAdTracking;
		}
        config ["retrieveAttribution"] = retrieveAttribution;
        config ["consentIntelligentManagement"] = icm;
        config ["consentManualManagedRequirements"] = icmManual;
        config ["wrapperNameString"] = sdkName;
        config ["wrapperVersionString"] = sdkVersionNumber;
        config ["wrapperBuildDateString"] = sdkBuildDate;
        // append custom values directly to config dictionary
        foreach (var cus in custom) config.Add(cus.Key, cus.Value);
        //Call the mm layer start.
        iOSNativeStart(Json.ToString(config));
        
        //If sleep is set then enable it right after configuring.
        iOSNativeSetSleepBool(sleep);
    }

	private static ConsentPartner[] iOSNativeGetConsentPartners()
	{
		string partnersSerialized = iOSNativeGetConsentPartnersJson ();
		Dictionary<string, object>[] partnersJson = Json.ToObj<Dictionary<string, object>[]> (partnersSerialized);
		ConsentPartner[] partners = new ConsentPartner[partnersJson.Length];
		for (int i = 0; i < partners.Length; i++) {
			partners[i] = new ConsentPartner(partnersJson[i]);
		}
		return partners;
	}

    [DllImport("__Internal")]
    private static extern void iOSNativeStart(string inputParameters);
    [DllImport("__Internal")]
    private static extern void iOSNativeSendEvent(string eventName, string eventInfo);
    [DllImport("__Internal")]
	private static extern void iOSNativeSendKochavaEvent(string eventName, string kochavaEventStringifiedDictionary, string receipt);
    [DllImport("__Internal")]
    private static extern void iOSNativeSendDeepLink(string urisourceApplicationString, string sourceApplicationString);
    [DllImport("__Internal")]
    private static extern void iOSNativeSendIdentityLink(string identityLinkDictionary);
    [DllImport("__Internal")]
    private static extern void iOSNativeSetAppLimitAdTrackingBool(bool value);
    [DllImport("__Internal")]
    private static extern void iOSNativeSetSleepBool(bool value);
    [DllImport("__Internal")]
    private static extern bool iOSNativeGetSleepBool();
    [DllImport("__Internal")]
    private static extern string iOSNativeGetDeviceId();
    [DllImport("__Internal")]
    private static extern string iOSNativeGetAttributionString();
    [DllImport("__Internal")]
    private static extern string iOSNativeGetVersion();
    [DllImport("__Internal")]
    private static extern void iOSNativeAddPushToken(byte[] bytes, UInt64 length);
    [DllImport("__Internal")]
    private static extern void iOSNativeRemovePushToken(byte[] bytes, UInt64 length);
    [DllImport("__Internal")]
    private static extern bool iOSNativeGetConsentRequired();
    [DllImport("__Internal")]
    private static extern void iOSNativeSetConsentRequired(bool value);
    [DllImport("__Internal")]
    private static extern bool iOSNativeGetConsentGranted();
    [DllImport("__Internal")]
    private static extern string iOSNativeGetConsentPartnersJson();
    [DllImport("__Internal")]
    private static extern void iOSNativeSetConsentGranted(bool value);
    [DllImport("__Internal")]
    private static extern bool iOSNativeGetConsentShouldPrompt();
    [DllImport("__Internal")]
    private static extern void iOSNativeSetConsentPrompted();
    [DllImport("__Internal")]
    private static extern string iOSNativeGetConsentDescription();
    [DllImport("__Internal")]
    private static extern long iOSNativeGetConsentResponseTime();
    [DllImport("__Internal")]
    private static extern bool iOSNativeGetConsentRequirementsKnown();


#elif WINDOWS_UWP // native Windows Store SDK    

    private void KochavaUWPAttributionListener(string msg)
    {
        lock(cacheLock)
        {
            cacheAttributionCallback = msg;
        }        
    }

    private static void KochavaUWPLogListener(string msg)
    {
        Debug.Log(msg);
	}
    
	private static void KochavaUWPConsentStatusChangeListener(string ignored)
	{
		if (_consentStatusChangeDelegate!=null) _consentStatusChangeDelegate();
	}
    
	private static void windowsUwpNativeStart(string appGuid, string partnerName, int logLevel, bool appLimitAdTracking, bool appLimitAdTrackingSet, bool sleep, bool retrieveAttribution, bool icm, bool icmManual, Dictionary<string, object> custom)
    {
#if !WINDOWS_UWP_SUPPORTED
            // attempting to start the Windows Store (UWP) native sdk with a version of Unity less than 2017 and/or without enabling WINMD support
            LOG("CANNOT INITIALIZE KOCHAVA - Windows Runtime Support (ENABLE_WINMD_SUPPORT) must be enabled in order to use the Kochava Windows Store SDK. See https://docs.unity3d.com/Manual/IL2CPP-WindowsRuntimeSupport.html for more information.", LOG_WARN);
            return;
#else
            // if attribution was requested wire up any provided listener
            if(retrieveAttribution) 
            {
                KochavaUWP.Tracker.ReceivedAttributionEvent += instance.KochavaUWPAttributionListener;
            }

            // start the Windows Store (UWP) native            
            var config = new KochavaUWP.Config();
            config.AppGUID = appGuid ?? "";
            config.PartnerName = partnerName ?? "";
            config.SetCustomValue("___versionExtension", sdkName + " " + sdkVersionNumber);  
			if(appLimitAdTrackingSet)
			{
				config.AppLimitAdTracking = appLimitAdTracking;
			}
            config.RetrieveAttribution = retrieveAttribution;
            config.LogLevel = (logLevel==0) ? KochavaUWP.LogLevel.none : (logLevel==1) ? KochavaUWP.LogLevel.error : (logLevel==2) ? KochavaUWP.LogLevel.warn : (logLevel==3) ? KochavaUWP.LogLevel.info : (logLevel==4) ? KochavaUWP.LogLevel.debug : KochavaUWP.LogLevel.trace;
            // add custom values, if any
            foreach (var cus in custom) {
                try {
                    LOG("Assigning custom value \"" + cus.Key + "\" ("+cus.Value.GetType().ToString()+")", LOG_DEBUG);        
                    if(cus.Value.GetType()==typeof(string)) config.SetCustomValue(cus.Key, (string)cus.Value);
                    else if(cus.Value.GetType()==typeof(bool)) config.SetCustomValue(cus.Key, (bool)cus.Value);
                    else config.SetCustomValue(cus.Key, Convert.ToDouble(cus.Value));
                } catch(Exception e) {
                    LOG("Unable to assign custom value for key \"" + cus.Key + "\": " + e.Message, LOG_WARN);
                }
            }

            if (_logLevel != 0)
            {
                // wire up a default debug logging handler for windows store apps, if necessary, which will write all logging output to C:\Users\<USER>\AppData\Local\Packages\<PKGID>\TempState\UnityPlayer.log
                // (alternatively you can subscribe to the KochavaUWP.Tracker.LogUpdateEvent event anywhere in your code to handle log messages differently)
                KochavaUWP.Tracker.LogUpdateEvent += KochavaUWPLogListener;
            }        

            windowsTracker = new KochavaUWP.Tracker(config);                 
#endif
    }

    private static void windowsUwpNativeSendEvent(string eventName, string eventData)
    {
#if WINDOWS_UWP_SUPPORTED
            if(windowsTracker!=null) windowsTracker.SendEvent(eventName ?? "", eventData ?? "");
#endif
    }

    private static void windowsUwpNativeSendKochavaEvent(string eventName, Dictionary<string, object> dictionary)
    {
#if WINDOWS_UWP_SUPPORTED
        // translate name by setting name as a custom name
        KochavaUWP.EventParameters ep = new KochavaUWP.EventParameters(KochavaUWP.EventType.Custom);
        ep.SetCustomEventName(eventName);
        // translate data by attempting to set each top-level data point as a custom value
        if (dictionary != null)
        {
            foreach (var it in dictionary)
            {
                try 
                {
                    if(it.Key!=null && it.Value!=null) {
                        if (it.Value.GetType() == typeof(string)) ep.SetCustomValue(it.Key, (string)it.Value);
                        else if (it.Value.GetType() == typeof(bool)) ep.SetCustomValue(it.Key, (bool)it.Value);
                        else if (it.Value.GetType() == typeof(Dictionary<string,object>))
                        {
                            // convert only the top level items of this dictionary to a native json object
                            var json = new Windows.Data.Json.JsonObject();
                            foreach (var pi in (Dictionary<string, object>)it.Value)
                            {
                                try
                                {
                                    if (pi.Value.GetType() == typeof(string)) json.Add(pi.Key, Windows.Data.Json.JsonValue.CreateStringValue((string)pi.Value));
                                    else if (pi.Value.GetType() == typeof(bool)) json.Add(pi.Key, Windows.Data.Json.JsonValue.CreateBooleanValue((bool)pi.Value));
                                    else json.Add(pi.Key, Windows.Data.Json.JsonValue.CreateNumberValue(System.Convert.ToDouble(pi.Value)));  // at this point if it's not a number it won't translate to this native event
                                }
                                catch(Exception e)
                                {
                                    LOG("SendEvent() UWP failed to translate value of nest key within dictionary: " + pi.Key + " (nested type must be string, bool, or number)", LOG_WARN);
                                }
                            }
                            ep.SetCustomValue(it.Key, json);
                        }
                        else ep.SetCustomValue(it.Key, System.Convert.ToDouble(it.Value)); // at this point if it's not a number it won't translate to this native event
                    }   
                } 
                catch(Exception e) 
                {
                    LOG("SendEvent() UWP failed to translate value of key: " + it.Key, LOG_WARN);
                }
            }
        }
        // send it
        if(windowsTracker != null) windowsTracker.SendEvent(ep);
#endif
    }

    private static void windowsUwpNativeSendDeepLink(string deepLinkURI, string sourceApplicationString)
    {
#if WINDOWS_UWP_SUPPORTED
		KochavaUWP.EventParameters ep = new KochavaUWP.EventParameters(KochavaUWP.EventType.DeepLink);
		if(deepLinkURI != null)
		{
			ep.SetUri(deepLinkURI);
		}
		if(sourceApplicationString != null)
		{
			ep.SetSource(sourceApplicationString);
		}
		if(windowsTracker != null) windowsTracker.SendEvent(ep);
#endif
    }

    private static void windowsUwpNativeSendIdentityLink(Dictionary<string, string> identityLinkDictionary)
    {
#if WINDOWS_UWP_SUPPORTED
        if (identityLinkDictionary == null) return;
        foreach (var it in identityLinkDictionary)
        {
            if(windowsTracker != null) windowsTracker.SetIdentityLink(it.Key, it.Value);
        }
#endif
    }

    private static void windowsUwpNativeSetAppLimitAdTrackingBool(bool desiredAppLimitAdTracking)
    {
#if WINDOWS_UWP_SUPPORTED
        if(windowsTracker != null) windowsTracker.SetAppLimitAdTracking(desiredAppLimitAdTracking);
#endif
    }
    
    private static void windowsUwpNativeSetSleepBool(bool sleep)
    {
        LOG("SetSleep() Sleep is not supported on this platform.", LOG_ERROR);
    }
    
    private static bool windowsUwpNativeGetSleepBool()
    {
        LOG("GetSleep() Sleep is not supported on this platform.", LOG_ERROR);
        return false;
    }

    private static string windowsUwpNativeGetAttribution()
    {
#if WINDOWS_UWP_SUPPORTED
        if(windowsTracker != null) return windowsTracker.AttributionData;
#endif
        return "";
    }

    private static string windowsUwpNativeGetDeviceId()
    {
#if WINDOWS_UWP_SUPPORTED
        if(windowsTracker != null) return windowsTracker.DeviceId;
#endif
        return "";
    }

    private static string windowsUwpNativeGetVersion()
    {
#if WINDOWS_UWP_SUPPORTED
        if(windowsTracker != null) return windowsTracker.SdkVersion;
#endif
        return "";
    }

    private static bool windowsUwpNativeGetConsentRequired()
    {
        LOG("GetConsentRequired() Kochava Intelligent Consent Management is not supported on this platform.", LOG_ERROR);
        return true;
    }
    
    private static void windowsUwpNativeSetConsentRequired(bool isRequired)
    {
        LOG("SetConsentRequired() Kochava Intelligent Consent Management is not supported on this platform.", LOG_ERROR);
    }

    private static void windowsUwpNativeSetConsentGranted(bool isGranted)
    {
        LOG("SetConsentGranted() Kochava Intelligent Consent Management is not supported on this platform.", LOG_ERROR);
    }

    private static bool windowsUwpNativeGetConsentGranted()
    {
        LOG("GetConsentGranted() Kochava Intelligent Consent Management is not supported on this platform.", LOG_ERROR);
        return false;
    }

    private static bool windowsUwpNativeGetConsentShouldPrompt()
    {
        LOG("GetConsentShouldPrompt() Kochava Intelligent Consent Management is not supported on this platform.", LOG_ERROR);
        return false;
    }

    private static void windowsUwpNativeSetConsentPrompted()
    {
        LOG("SetConsentPrompted() Kochava Intelligent Consent Management is not supported on this platform.", LOG_ERROR);
    }

    private static string windowsUwpNativeGetConsentPartnersJson()
    {
        LOG("GetConsentPartnersJson() Kochava Intelligent Consent Management is not supported on this platform.", LOG_ERROR);
        return "[]";
    }

	private static ConsentPartner[] windowsUwpNativeGetConsentPartners()
	{
		LOG("GetConsentPartners() Kochava Intelligent Consent Management is not supported on this platform.", LOG_ERROR);
		return new ConsentPartner[0];
	}

    private static string windowsUwpNativeGetConsentDescription()
    {
        LOG("GetConsentDescription() Kochava Intelligent Consent Management is not supported on this platform.", LOG_ERROR);
        return "";
    }

    private static long windowsUwpNativeGetConsentResponseTime()
    {
        LOG("GetConsentResponseTime() Kochava Intelligent Consent Management is not supported on this platform.", LOG_ERROR);
        return 0;
    }

    private static bool windowsUwpNativeGetConsentRequirementsKnown()
    {
        LOG("GetConsentRequirementsKnown() Kochava Intelligent Consent Management is not supported on this platform.", LOG_ERROR);
        return false;
    }

#else // platform-independent SDK

    /// <summary>
    /// DO NOT EDIT THE CODE BELOW
    /// If we're not using wrapped native SDKs (iOS / Android / Windows Store) we'll use the platform-independent 'lite' SDK implementation below    
    /// </summary>

    // network callback(s)
    public delegate bool NetworkSuccessCallback(string response);
    public delegate void NetworkFailCallback(string errorMessage);

    // network status:
    const int NR_QUEUED = 0;
    const int NR_BUSY = 1;
    const int NR_ENDED = 2;

    // event send status
    const int SEND_QUEUED = 0;
    const int SEND_READY = 1;
    const int SEND_SENT = 2;
    const int SEND_COMPLETE = 3;

    // session flags
    private bool sessionInitComplete;
    private bool sessionPaused;
    private double sessionPausedTime;
    private double sessionPauseLastUpdated;
    private double sessionPauseUpdateWait;

    // tracker objects
    private NetworkManager networkManager;
    private List<Item> intakeQueue;
    private Profile profile;
    private ItemQueue itemQueue;

    // tracker values        
    private double wakeTime = 0;
    private double timeStart = 0;
    private double networkFailedWait = 0;
    private int networkFailedCount = 0;

    // settings
    private static class Settings
    {
        // build specific
        public const string sdkVersion = sdkNativeName + " " + sdkVersionNumber;
        public const string sdkProtocol = "7";

        // times
        public const double initWait = 60; // min time between new init send (or else skip)
        public const double getAttributionWait = 7; // seconds to wait after initial before requesting attribution
        public const double flushWait = 0; // seconds between queue flushes if anything exists in queue
        public const float WakeTime = .10f; // time between controller wakeup checks
        public const double PauseHold = 60.0; // minimum time which must elapse between a pause and resume (otherwise the short pause/resume is ignored)
        public const double PauseUpdate = 10.0; // cummulative update frequency to update pause event for this session

        // values
        public const int maxQueueSize = 500;

        // URL endpoints
        public static string apiURLInit = "https://kvinit-prod.api.kochava.com";
		public static string apiURLTracker = "https://control.kochava.com";
		public static string apiURLQuery = "https://control.kochava.com";
		public static string trackerURL = "/track/json";
		public static string initURL = "/track/kvinit";
		public static string queryURL = "/track/kvquery";

        // keys
        public const string keyProfile = "kou__profile";
        public const string keyQueue = "kou__queue";
        public const string keyAttribution = "kou__attribution";
        public static string advertisingIDKey = "ko_advertising_id";
        public const string temporaryQueueKey = "ko_temp_queue";
    }

    // profile (persisted data)    
    private class Profile
    {
        public bool persisted; // whether or not data was in fact persisted from a previous run (otherwise it's a first launch)
        public string kochavaDeviceID;
        public bool attributionReceived;
        public bool initialComplete;
        public double initLastSent;
        public bool initComplete;
        public double initWait;
        public double flushWait;
        public string[] blacklist;
        public string[] eventname_blacklist;
        public string[] whitelist;        
        public bool sessionTracking;
        public double getAttributionWait;
        public bool sendUpdates;
        public Dictionary<string, object> watchlist;
        public string appGUID;
        public string overrideAppGUID;
        public string sentIdentityLinks;

        public Profile()
        {
            // defaults
            sentIdentityLinks = "";
            appGUID = _appGUID;
            overrideAppGUID = "";
            sessionTracking = true;
            sendUpdates = true;
            initWait = Settings.initWait;
            flushWait = Settings.flushWait;
            getAttributionWait = Settings.getAttributionWait;
            // build our watchlist based on the datapoint key names
            watchlist = new Dictionary<string, object>(){
                { Settings.advertisingIDKey, "" },
                { "os_version", "" },
                { "language", "" },
                { "app_limit_tracking", false },
                { "app_version", "" },
                { "device_limit_tracking", false }
            };
        }

        // persists the current json data to a file
        public void Save()
        {
            string data = Json.ToString(this);
            PlayerPrefs.SetString(Settings.keyProfile, data);
            LOG("WRITE PERSISTED (" + Settings.keyProfile + "): " + data, LOG_TRACE);
        }

    }

    // manages all network requests
    private class NetworkManager : MonoBehaviour
    {
        // collection of active network requests
        private List<NetworkRequest> networkRequests;

        // constructor
        public NetworkManager()
        {
            networkRequests = new List<NetworkRequest>();
        }

        // creates a new network request
        public void NewRequest(string _name, string _data)
        {
            if (_name == "init") networkRequests.Add(new NetworkRequest(instance, _name, Settings.apiURLInit + Settings.initURL, _data, instance.NetSuccessInit, instance.NetFailInit));
            else if (_name == "get_attribution") networkRequests.Add(new NetworkRequest(instance, _name, Settings.apiURLQuery + Settings.queryURL, _data, instance.NetSuccessAttribution, instance.NetFailAttribution));
            else if (_name == "initial") networkRequests.Add(new NetworkRequest(instance, _name, Settings.apiURLTracker + Settings.trackerURL, _data, instance.NetSuccessInitial, instance.NetFailInitial));
            else networkRequests.Add(new NetworkRequest(instance, _name, Settings.apiURLTracker + Settings.trackerURL, _data, instance.NetSuccessQueue, instance.NetFailQueue));
        }        

        // returns the number of current requests by status
        public int requestCount(int requestStatus)
        {
            return networkRequests.FindAll(it => it.status == requestStatus).Count;
        }

        // returns total number of current requests
        public int requestCount()
        {
            return networkRequests.Count;
        }

        // processes/updates all current network requets
        public void Process()
        {
            // clean up ended requests
            networkRequests.RemoveAll(it => it.status == NR_ENDED);

            // queue the next request as long as we have no busy requests
            if (networkRequests.FindAll(it => it.status == NR_BUSY).Count == 0)
            {
                foreach (NetworkRequest it in networkRequests)
                {
                    if (it.status == NR_QUEUED) // not yet started
                    {
                        // start the network request
                        it.status = NR_BUSY;
                        LOG("Starting next queued network request: " + it.name, LOG_TRACE);
                        StartCoroutine(StartHttpRequest(it));
                        // break now, only start one at a time
                        break;
                    }
                }
            }
        }

		// begins an http request with a provided network request object
        private IEnumerator StartHttpRequest(NetworkRequest sourceRequest)
        {
            // set status (will have the same effect as applying a lock)
            sourceRequest.status = NR_BUSY;

            // create a copy of the string data to be posted
            string postString = sourceRequest.data;

            LOG("HTTP " + sourceRequest.name + " PAYLOAD: " + postString, LOG_DEBUG);

            // http call:
            UnityWebRequest www = new UnityWebRequest(sourceRequest.url, "POST");
            www.uploadHandler = new UploadHandlerRaw (System.Text.Encoding.UTF8.GetBytes (postString));
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            // return here once we have a response
#if UNITY_2017_2_OR_NEWER
            yield return www.SendWebRequest();
#else
			yield return www.Send();
#endif

            // was there an error?
#if UNITY_2017_1_OR_NEWER
            if(www.isNetworkError || www.isHttpError || !string.IsNullOrEmpty(www.error))
#else
            if (www.isError || !string.IsNullOrEmpty(www.error))
#endif
            {
                LOG("HTTP " + sourceRequest.name + " ERROR: " + www.error ?? "", LOG_DEBUG);
                // generic failure:
                NetFailed();
                // specific failed callback:
                sourceRequest.networkFailCallback(www.error ?? "");
            }
            else
            {
                // Deserialize JSON response from server
                if (www.downloadHandler.text != "")
                {
                    try
                    {
                        // attempt to parse the response
                        LOG("HTTP " + sourceRequest.name + " SUCCESS", LOG_TRACE);
                        // specific success callback:
                        if (sourceRequest.networkSuccessCallback(www.downloadHandler.text))
                        {
                            // only continue with success if no special failure cases were triggered within the callback
                            NetSuccess();
                        }
                        else
                        {
                            NetFailed();
                        }
                    }
                    catch (Exception e)
                    {
                        LOG("HTTP " + sourceRequest.name + " FAILED Json deserialization: " + e.Message, LOG_DEBUG);
                        // specific failed callback:
                        sourceRequest.networkFailCallback(e.Message);
                    }
                }
            }

            // mark as ended, we'll delete it from the list later
            sourceRequest.status = NR_ENDED;

            // controller will need to wakeup to deal with this response (or failure):
            WakeController();

            yield break; // end here
        }
    }

    // represents a network request
    private class NetworkRequest
    {

        public NetworkRequest(Kochava _thisTracker, string _name, string _url, string _data, NetworkSuccessCallback successCallback, NetworkFailCallback failCallback)
        {
            status = NR_QUEUED;
            name = _name;
            url = _url;
            data = _data;
            thisTracker = _thisTracker;
            LOG("New network request: " + name, LOG_TRACE);
            networkSuccessCallback = successCallback;
            networkFailCallback = failCallback;
        }

        ~NetworkRequest()
        {
            LOG("networkRequest destructor", LOG_TRACE);
        }

        public int status;
        public string name;
        public string url;
        public string data;
        public Kochava thisTracker;
        public NetworkSuccessCallback networkSuccessCallback;
        public NetworkFailCallback networkFailCallback;
    }

    // represents an item to be sent and it's properties       
    private class Item
    {

        public class Properties
        {
            public string action;
            public bool wait = false; // this must be completed before anything else in the queue can be
            public bool instant = false; // ignore any timers and sent instantly?
            public bool unique = false; // only one event with this action may exist
            public bool temporary = false; // does not persist
            public double hold = 0; // wait this long before sending
            public int sendStatus = 0; // current sent status
            public bool incomplete = true; // payload is complete?
        }
        public Properties properties;

        // payload key/value pairs
        public Dictionary<string, object> top;
        public Dictionary<string, object> data;

        // constructor - new
        public Item(string _action)
        {
            properties = new Properties();
            properties.action = _action;
            top = new Dictionary<string, object>();
            data = new Dictionary<string, object>();
        }

        // returns the stringified payload, ready to be sent
        public string Stringify()
        {
            // serialize dictionaries and return the current payload
            var dic = new Dictionary<string, object>();
            foreach (var pair in top)
            {
                dic.Add(pair.Key, pair.Value);
            }
            if (data.Count > 0)
            {
                var dicData = new Dictionary<string, object>();
                foreach (var pair in data)
                {
                    dicData.Add(pair.Key, pair.Value);
                }
                dic.Add("data", dicData);
            }

            return Json.ToString(dic);
        }


    }

    // queue managers which holds all events
    private class ItemQueue
    {
        // collection of currently queued events to send
        public List<Item> queue;

        public bool flush; //
        public double nextFlush; // time to flush next batch
        public bool mutated = false; // flag if we need to save it

        // add an event to the queue
        public void Add(Item ev)
        {
            // drop it if we're at the cap
            if (queue.Count > Settings.maxQueueSize)
            {
                LOG("Event queue limit reached (" + queue.Count + "/" + Settings.maxQueueSize + ") event dropped", LOG_ERROR);
                return;
            }

            // first make sure another event of this type does not already exist
            if (ev.properties.unique)
            {
                foreach (var it in queue)
                {
                    if (it.properties.action == ev.properties.action)
                    {
                        LOG("Unique event already exists (" + ev.properties.action + ")", LOG_DEBUG);
                        return;
                    }
                }
            }

            // add it
            queue.Add(ev);

            // persist it
            Save();

            // make sure controller wakes after this
            WakeController();
        }

        // reset status of all sent items to ready so they can be sent again
        public void Resend()
        {
            LOG("Resetting all sent items status to ready", LOG_DEBUG);
            foreach (var it in queue)
            {
                if (it.properties.sendStatus == SEND_SENT) it.properties.sendStatus = SEND_READY;
            }
            // persist queue now since we've reset some events
            Save();
        }

        // persists the currently queued events to a file
        public void Save()
        {
            mutated = false; // consume mutated flag if it was set
            try
            {
                var epa = new ItemPersistedAll();
                int saved = 0;
                foreach (var ev in this.queue)
                {
                    if (!ev.properties.temporary)
                    {
                        // convert all data into stringified json object
                        var ep = new ItemPersisted();
                        ep.properties = Json.ToString(ev.properties);
                        ep.top = Json.ToString(ev.top);
                        ep.data = Json.ToString(ev.data);
                        Array.Resize(ref epa.items, saved + 1);
                        epa.items[saved++] = Json.ToString(ep);
                    }
                }
                if (saved > 0)
                {
                    // package all objects in a json array before storing them
                    LOG("Saving (" + saved + ") events to persisted queue...", LOG_TRACE);
                    string data = Json.ToString(epa);
                    PlayerPrefs.SetString(Settings.keyQueue, data);
                }
                else
                {
                    // nothing to persist, make sure this is empty
                    LOG("No events to persist, clearing persisted key", LOG_TRACE);
                    PlayerPrefs.DeleteKey(Settings.keyQueue);
                }
            }
            catch (Exception e)
            {
                LOG("EventQueue.Save() failed: " + e.Message, LOG_ERROR);
            }
        }

        // attempts to load the persisted queue from a previous run
        public void Load()
        {
            try
            {
                if (PlayerPrefs.HasKey(Settings.keyQueue))
                {
                    string data = PlayerPrefs.GetString(Settings.keyQueue);
                    var epa = Json.ToObj<ItemPersistedAll>(data);
                    LOG("Loading (" + epa.items.Length + ") items from persisted queue...", LOG_DEBUG);
                    for (int i = 0; i < epa.items.Length; i++)
                    {
                        var ep = Json.ToObj<ItemPersisted>(epa.items[i]);
                        // create the event from the persisted data
                        var ev = new Item("");
                        ev.properties = Json.ToObj<Item.Properties>(ep.properties);
                        ev.top = Json.ToObj<Dictionary<string, object>>(ep.top);
                        ev.data = Json.ToObj<Dictionary<string, object>>(ep.data);
                        // any events that are still in the queue from a previous launch need to be sent right away
                        // (they should have been sent when the app suspended last run and failed for some reason)
                        ev.properties.sendStatus = SEND_READY;
                        ev.properties.incomplete = false;
                        if (i < 3) LOG("Loaded item: " + ev.properties.action + "", LOG_DEBUG);
                        // add it manually (we don't go through EventQueue.Add() for this because we're just restoring it directly)
                        queue.Add(ev);
                    }
                }
                else
                {
                    LOG("Persisted queue is empty (no items found)", LOG_DEBUG);
                }
            }
            catch (Exception e)
            {
                LOG("EventQueue.Load() failed: " + e.Message, LOG_ERROR);
            }
        }

        // constructor
        public ItemQueue()
        {
            queue = new List<Item>();
        }

        // process the event queue (called from the controller)
        public void Process()
        {
            if (instance.itemQueue != null && instance.itemQueue.queue.Count > 0 && instance.networkManager.requestCount(NR_BUSY) == 0)
            {
                // update queue batch timer (if we're not already flushing)                
                if (instance.itemQueue.nextFlush <= Util.UnixTime() && instance.itemQueue.nextFlush > 0)
                {
                    // we've reached the batch time, flush it all
                    LOG("Batch timer reached, flush queue now", LOG_DEBUG);
                    flush = true;
                    nextFlush = 0;
                }

                // prepare a list of ready items
                //var queue = new List<int>();
                bool waiting = false;
                bool queued = false;
                // precheck - find all events which are ready to send
                foreach (var ev in queue)
                {
                    // if this item is incomplete attempt to finish it (only if we have a blacklist, otherwise it's just going to fail again)
                    if (instance.profile.initComplete)
                    {
                        if (ev.properties.incomplete)
                        {
                            instance.ConstructPayload(ev);
                            if (!ev.properties.incomplete) mutated = true; // changed status, need to update persisted event queue
                        }
                        // still incomplete?
                        if (ev.properties.incomplete)
                        {
                            // dont send yet if it's incomplete - we'll check again soon (giving it enough time to finish collecting whatever data it needs)
                            LOG("QUEUE: Item payload still incomplete: " + ev.properties.action + "", LOG_DEBUG);
                            WakeController(2);
                        }
                    }

                    // wait-able event?
                    if (ev.properties.wait)
                    {
                        // wait-able event found, will treat this as the only event in the queue
                        waiting = true;
                        LOG("QUEUE: Awaiting item: " + ev.properties.action + "", LOG_TRACE);
                    }

                    // prepare this item to send?
                    if (ev.properties.sendStatus == SEND_QUEUED && !ev.properties.incomplete)
                    {
                        queued = true;
                        if (flush || ev.properties.instant)
                        {
                            // set status to ready                                                
                            ev.properties.sendStatus = SEND_READY;
                            LOG("QUEUE: Item ready: " + ev.properties.action + "", LOG_TRACE);
                            mutated = true; // changed status, need to update persisted event queue
                        }
                        else if (!flush && nextFlush == 0)
                        {
                            LOG("QUEUE: Item(s) are queued, will flush " + instance.profile.flushWait + "s from now...", LOG_DEBUG);
                            nextFlush = Util.UnixTime() + instance.profile.flushWait;
                        }
                    }
                }

                // there's nothing still queued, if we were flushing stop
                if (!queued && flush)
                {
                    LOG("QUEUE: Done flushing", LOG_DEBUG);
                    flush = false;
                }

                // process ready events                
                foreach (var ev in queue)
                {
                    // find the first ready event and prepare it to be sent (if we're waiting only find the first wait-able event)
                    if (ev.properties.sendStatus == SEND_READY && (!waiting || ev.properties.wait))
                    {
                        if (Util.UnixTime() >= ev.properties.hold) // if a hold applies make sure we've reached that time
                        {
                            // this can be sent now
                            LOG("QUEUE: Sending event: " + ev.properties.action + "", LOG_DEBUG);
                            ev.properties.sendStatus = SEND_SENT;
                            mutated = true; // changed status, need to update persisted event queue                                
                            string payload = ev.Stringify(); ;
                            string payloadAction = ev.properties.action;
                            // create the network request for this item
                            instance.networkManager.NewRequest(payloadAction, payload);
                            // we're only sending one item at a time (no batching)
                            break;
                        }
                        else
                        {
                            // this item is ready but it's waiting on a hold so make sure we come back then
                            WakeController(ev.properties.hold - Util.UnixTime());
                        }
                        // if we were waiting then we'll bail now so we don't send anything else (including other wait-able events)
                        if (waiting) break;
                    }
                }

                // need to update persisted events?
                if (mutated == true)
                {
                    queue.RemoveAll(it => it.properties.sendStatus == SEND_COMPLETE);
                    Save();
                }
            }

            // make sure we wake up in time to flush queue if needed
            if (nextFlush > 0) WakeController(nextFlush - Util.UnixTime());
        }

    }
    
    // initialization - kick off the controller loop
    private void UnityNetStart()
    {
#if WRAPPED_NATIVES
        Debug.Log("Failed to initialize the platform-independent implementation of Kochava while running on native iOS or Android, which can only happen if you've edited the code in Kochava.cs. Please re-import Kochava.cs from the unity package and do not edit this code!", LOG_ERROR);
        return;
#endif
        if (_dictionaryCustom != null)
        {
            foreach (var it in _dictionaryCustom)
            {
                try
                {
                    //Look for overrides
                    if("apiURLInit" == it.Key && it.Value is string)
                    {
                        Settings.apiURLInit = (string) it.Value;
                    } else if("apiURLQuery" == it.Key && it.Value is string)
                    {
                        Settings.apiURLQuery = (string) it.Value;
                    } else if("apiURLTracker" == it.Key && it.Value is string)
                    {
                        Settings.apiURLTracker = (string) it.Value;
                    } else if("initURL" == it.Key && it.Value is string)
                    {
                        Settings.initURL = (string) it.Value;
                    } else if("trackerURL" == it.Key && it.Value is string)
                    {
                        Settings.trackerURL = (string) it.Value;
                    } else if("queryURL" == it.Key && it.Value is string)
                    {
                        Settings.queryURL = (string) it.Value;
                    }
                }
                catch (Exception e)
                {
                    LOG("Custom value payload error: " + e.Message, LOG_ERROR);
                }
            }
        }
        StartCoroutine(MainLoop());
    }

    // main loop - called every X seconds
    public IEnumerator MainLoop()
    {
        while (true)
        {
            // run the controller if wakeTime has been met (<0 means idle)
            if (Util.UnixTime() >= wakeTime && wakeTime >= 0 && instance != null)
            {
                Controller();
            }

            // continue the loop again in X seconds (no need to put every-frame strain on resources)
            yield return new WaitForSeconds(Settings.WakeTime);
        }
    }

    // PAUSE / RESUME wrapper methods (varies by platform)
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) SessionPause();
        else SessionResume();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) SessionPause();
        else SessionResume();
    }

    void OnApplicationQuit()
    {
        SessionPause(true);
    }

    // pause session
    private void SessionPause(bool exiting = false)
    {
        if (!initialized || profile == null || !profile.sessionTracking || sessionPaused) return;

        sessionPaused = true;
        sessionPausedTime = Util.UnixTime();

        CreateSessionPauseEvent();
        LOG("SESSION PAUSE - EXIT", LOG_DEBUG);

    }

    // creates an updated session pause event to be loaded on the next launch or resume
    private void CreateSessionPauseEvent()
    {
        // create the session pause event
        var ev = new Item("session");
        ev.data.Add("state", "pause");
        ConstructPayload(ev);

        PlayerPrefs.SetString(Settings.temporaryQueueKey, ev.Stringify());
        LOG("UPDATING SESSION PAUSE (to temporary queue)", LOG_DEBUG);
    }

    // resume session
    private void SessionResume()
    {
        if (!initialized || profile == null || !profile.sessionTracking || !sessionPaused) return;

        sessionPaused = false;
        itemQueue.flush = false;

        double pausedSeconds = Util.UnixTime() - sessionPausedTime;
        if (pausedSeconds < Settings.PauseHold)
        {
            LOG("Skipping pause/resume (too fast @ " + (int)(pausedSeconds) + "/" + Settings.PauseHold + "s)", LOG_DEBUG);
            WakeController();
            // clear the last temporary session end event -- we won't be using it
            if (PlayerPrefs.HasKey(Settings.temporaryQueueKey)) PlayerPrefs.DeleteKey(Settings.temporaryQueueKey);
            // make sure a new pause is generated and reset counter
            sessionPauseUpdateWait = 0; 
            return;
        }

        // check for any temporary previous session pause
        ImportTemporaryQueue();

        LOG("SESSION RESUME (" + (int)(pausedSeconds) + " s)", LOG_DEBUG);
        // reset session times
        timeStart = Util.UnixTime();
        // session resume
        var ev = new Item("session");
        ev.data.Add("state", "resume");
        intakeQueue.Add(ev);
        // flush everything
        itemQueue.flush = true;
        WakeController();
    }

    // check for any temporary sessions from previous session pause
    private void ImportTemporaryQueue(bool intoEventQueue = false)
    {
        if (PlayerPrefs.HasKey(Settings.temporaryQueueKey))
        {
            string tempEvent = PlayerPrefs.GetString(Settings.temporaryQueueKey, "");
            PlayerPrefs.DeleteKey(Settings.temporaryQueueKey);
            // parse the event string back into an event to be queued
            var top = Json.ToObj<Dictionary<string, object>>(tempEvent);
            if (top.ContainsKey("action") && top.ContainsKey("data"))
            {
                string action = top["action"].ToString();
                LOG("Temporary queued item found (" + action + ") " + tempEvent, LOG_DEBUG);
                // reconstruct the event
                var ev = new Item(action);
                ev.data = (Dictionary<string, object>)top["data"];
                top.Remove("data");
                ev.top = top;
                // assume this has been completed and is ready to send because it was from a previous session
                ev.properties.sendStatus = SEND_READY;
                if (intoEventQueue == false) intakeQueue.Add(ev);
                else itemQueue.queue.Add(ev); // add directly (don't trigger a save)
                WakeController();
            }
        }
    }

    // ingest an event and place into intake queue (this could happen before initialization completes)
    private void IngestEvent(string name, string data, string receipt)
    {
        var ev = new Item("event");
        ev.data.Add("event_name", name);
        ev.data.Add("event_data", data);
        if (receipt.Length > 0) ev.data.Add("receipt", receipt);
        intakeQueue.Add(ev);
        if (initialized) WakeController();
    }

    private void IngestEvent(Event kochavaEvent)
    {
        var ev = new Item("event");
        ev.data.Add("event_name", kochavaEvent.getEventName());
        ev.data.Add("event_data", kochavaEvent.getDictionary());
        intakeQueue.Add(ev);
        if (initialized) WakeController();
    }

    private void IngestDeeplink(string deepLinkURI, string sourceApplication)
    {
		Kochava.Event myEvent = new Kochava.Event (Kochava.EventType.DeepLink);
		if (deepLinkURI != null) {
			myEvent.uri = deepLinkURI;
		}
		if (sourceApplication != null) {
			myEvent.source = sourceApplication;
		}
		IngestEvent (myEvent);
        if (initialized) WakeController();
    }

    private void IngestIdentityLink(Dictionary<string, string> identityLinkDictionary)
    {
        var ev = new Item("identityLink");
        ev.data = Json.ToObj<Dictionary<string, object>>(Json.ToString(identityLinkDictionary));
        ev.properties.instant = true;
        intakeQueue.Add(ev);        
        if (initialized) WakeController();
    }

    // represents a persisted queued event item when saving/loading
    private class ItemPersisted
    {
        public string properties;
        public string top;
        public string data;
    }

    // represents all of the events when saving/loading
    private class ItemPersistedAll
    {
        public string[] items;
    }
    
    // wake up the controller in this many seconds
    private static void WakeController(double secondsFromNow = 0)
    {
        if (secondsFromNow < 0) secondsFromNow = 0;
        // adjust the wake time only if it would be sooner that already planned
        if (instance.wakeTime < 0 || Util.UnixTime() + secondsFromNow < instance.wakeTime) instance.wakeTime = Util.UnixTime() + secondsFromNow;
    }

    // state controller - the brain
    private void Controller()
    {
        // consume wakeTime (set to -1 for idle, no future wake time)
        wakeTime = -1;

        LOG("CONTROLLER RUN", LOG_TRACE);

        // INITIALIZING?
        ProcessInitialize();

        // has initialized, follow normal behavior
        if (initialized)
        {

            // make sure DeviceInfo is ready to go
            if (!DeviceInfo.isReady())
            {
                WakeController();
                return;
            }

            // UPDATE THE PAUSE SESSION?
            if(profile.sessionTracking==true && profile.initComplete) {
				UpdatePauseSession();
			}

            // INTAKE QUEUE - move items from intake queue into the primary queue
            ProcessIntakeQueue();       

            // WAITING FOR RETRY? if we're waiting on a network retry stop here
            if (networkFailedWait > Util.UnixTime())
            {
                double secs = networkFailedWait - Util.UnixTime();
                LOG("Controller holding, retry in " + secs + " s...", LOG_TRACE);
                WakeController(secs); // wakeup slightly after retry time so we know enough time passes
                return;
            }

            // ITEM QUEUE - handle items which need to be constructed, updated or sent
            itemQueue.Process();          

            // NETWORK MANAGER - start/update network requests
            networkManager.Process();

            // IDLE? for debug purposes announce idle if wake time has been set and no active network requests exist (nothing is out standing at the moment)
            if (wakeTime < 0 && networkManager.requestCount() < 1)
            {
                LOG("Controller IDLE", LOG_DEBUG);
            }
            else if (networkManager.requestCount() < 1)
            {
                LOG("Controller WAITING (wake again in " + (wakeTime - Util.UnixTime()) + " s)", LOG_DEBUG);
            }
        }

    }

    // called after init is received or skipped
    private void CompleteInit()
    {
        // flag init as being completed
        sessionInitComplete = true;

        LOG("INIT: Completing", LOG_DEBUG);

        // update or create the watchlist
        CheckWatchlist();

    }
    
    // GENERIC NETWORK SUCCESS/FAIL
    // called when any network call fails (before the specific callback)
    private static void NetFailed()
    {
        // apply a wait time for future network calls
        instance.networkFailedCount++;
        double secs = Util.GetNetworkRetryTime(instance.networkFailedCount);
        instance.networkFailedWait = Util.UnixTime() + secs;
        LOG("Network call failed (" + instance.networkFailedCount + ") next retry in " + secs + "s...", LOG_DEBUG);

        // all sent items will need to be re-attempted
        instance.itemQueue.Resend();
    }

    // called when any network call succeeds (before the specific callback)
    private static void NetSuccess()
    {
        // success, so clear previously failed attempts or retry timers
        instance.networkFailedCount = 0;
        instance.networkFailedWait = 0;

        // clean up completed sent items
        instance.itemQueue.queue.RemoveAll(it => it.properties.sendStatus == SEND_SENT);
        // save queue now since we've removed some events
        instance.itemQueue.Save();
    }

    // INIT CALLBACKS
    // callback for SUCCESSFUL network response - INIT
    private bool NetSuccessInit(string result)
    {
        LOG("HTTP INIT RESPONSE: " + result, LOG_DEBUG);

        // parse the response
        var payloadInitResponse = Json.ToObj<Dictionary<string, object>>(result);

        // error message provided?
        try
        {
            if (payloadInitResponse.ContainsKey("error"))
            {
                LOG("INIT ERROR: " + payloadInitResponse["error"], LOG_ERROR);
                // trigger failed response
                return false;
            }
        }
        catch (Exception e)
        {
            LOG("Failed to parse init response error: " + e.Message, LOG_WARN);
            return false;
        }

        // successful response?
        try
        {
            if (payloadInitResponse.ContainsKey("success") && (string)payloadInitResponse["success"] != "1")
            {
                // success was not 1, trigger failed network behavior
                return false;
            }
        }
        catch (Exception e)
        {
            LOG("Failed to parse init response success: " + e.Message, LOG_WARN);
            return false;
        }

        // messages to display?
        try
        {
            if (payloadInitResponse.ContainsKey("msg") && payloadInitResponse["msg"].GetType().GetElementType() == typeof(string))
            {
                string[] msgs = (string[])payloadInitResponse["msg"];
                foreach (var item in msgs)
                {
                    LOG("INIT Message: " + item, LOG_WARN);
                }
            }
        }
        catch (Exception e)
        {
            LOG("Failed to parse init response msg[]: " + e.Message, LOG_WARN);
        }

        // good response, proceed:        

        // read flags        
        try
        {
            if (payloadInitResponse.ContainsKey("flags"))
            {
                Dictionary<string, object> flags = (Dictionary<string, object>)payloadInitResponse["flags"];

                if (flags.ContainsKey("kochava_app_id"))
                {
                    profile.appGUID = flags["kochava_app_id"].ToString();
                    profile.overrideAppGUID = profile.appGUID;
                    LOG("Updated (override) kochava_app_id to: " + profile.appGUID, LOG_DEBUG);
                }

                if (flags.ContainsKey("kochava_device_id"))
                {
                    profile.kochavaDeviceID = flags["kochava_device_id"].ToString();
                    LOG("Updated kochava_device_id to: " + profile.kochavaDeviceID, LOG_DEBUG);
                }

                if (flags.ContainsKey("resend_initial") && (bool)flags["resend_initial"] == true)
                {
                    profile.initialComplete = false;
                    LOG("Resend initial: true", LOG_DEBUG);
                    // create another initial
                    LOG("Resending initial...", LOG_DEBUG);
                    var ev = new Item("initial");
                    ev.properties.unique = true;
                    ev.properties.wait = true;
                    ev.properties.instant = true;
                    ConstructPayload(ev);
                    intakeQueue.Add(ev);
                }

                if (flags.ContainsKey("session_tracking") && flags["session_tracking"].ToString() == "none")
                {
                    profile.sessionTracking = false;
                    LOG("Session tracking OFF", LOG_DEBUG);
                } else 
				{
					profile.sessionTracking = true;
				}

                if (flags.ContainsKey("getattribution_wait"))
                {
                    profile.getAttributionWait = double.Parse(flags["getattribution_wait"].ToString());
                    LOG("Get attribution wait: " + profile.getAttributionWait + " s", LOG_DEBUG);
                }

                if (flags.ContainsKey("kvinit_wait"))
                {
                    profile.initWait = double.Parse(flags["kvinit_wait"].ToString());
                    LOG("Init wait: " + profile.initWait + " s", LOG_DEBUG);
                }

                if (flags.ContainsKey("kvtracker_wait"))
                {
                    profile.flushWait = double.Parse(flags["kvtracker_wait"].ToString());
					LOG("Tracker wait: " + profile.flushWait + " s", LOG_DEBUG);
                }

                if (flags.ContainsKey("send_updates"))
                {
                    profile.sendUpdates = (bool)flags["send_updates"];
                    LOG("Send Updates: " + profile.sendUpdates, LOG_DEBUG);
                }

            }
        }
        catch (Exception e)
        {
            LOG("Failed to parse init response flags: " + e.Message, LOG_WARN);
        }

        // read blacklist
        try
        {
            if (payloadInitResponse.ContainsKey("blacklist") && payloadInitResponse["blacklist"].GetType().GetElementType() == typeof(string))
            {
                profile.blacklist = (string[])payloadInitResponse["blacklist"];
                LOG("Blacklist items: " + profile.blacklist.Length, LOG_TRACE);
            }
        }
        catch (Exception e)
        {
            LOG("Failed to parse init response blacklist[]: " + e.Message, LOG_WARN);
        }

        // read eventname_blacklist
        try
        {
            if (payloadInitResponse.ContainsKey("eventname_blacklist") && payloadInitResponse["eventname_blacklist"].GetType().GetElementType() == typeof(string))
            {
                profile.eventname_blacklist = (string[])payloadInitResponse["eventname_blacklist"];
                LOG("Eventname_blacklist items: " + profile.eventname_blacklist.Length, LOG_TRACE);
            }
        }
        catch (Exception e)
        {
            LOG("Failed to parse init response eventname_blacklist[]: " + e.Message, LOG_WARN);
        }

        // read whitelist
        try
        {
            if (payloadInitResponse.ContainsKey("whitelist") && payloadInitResponse["whitelist"].GetType().GetElementType() == typeof(string))
            {
                profile.whitelist = (string[])payloadInitResponse["whitelist"];
                for (int i = 0; i < profile.whitelist.Length; i++)
                {
                    LOG("Whitelisted: " + profile.whitelist[i], LOG_DEBUG);                    
                }
            }
        }
        catch (Exception e)
        {
            LOG("Failed to parse init response whitelist[]: " + e.Message, LOG_WARN);
        }

        // update profile - we've received the init
        profile.initLastSent = Util.UnixTime();
        profile.initComplete = true;

        // persist updated values        
        profile.Save();

        // complete the init process
        CompleteInit();

        // return success
        return true;
    }

    // callback for FAILED network response - INIT
    private void NetFailInit(string errorMessage)
    {
        LOG("HTTP INIT FAILED: " + errorMessage, LOG_DEBUG);

        // if we already received a valid init in the past just move ahead and use persisted values   
        if (profile.initComplete)
        {
            LOG("Using previous init response...", LOG_DEBUG);
            // remove the init so it doesn't send again (no need to save after this because init is a temporary event)
            itemQueue.queue.RemoveAll(it => it.properties.action == "init");
            // simulate a successful response so we don't have any waiting retry timers
            NetSuccess();
            // complete the init process
            CompleteInit();
            return;
        }

    }

    // INITIAL CALLBACKS
    // callback for SUCCESSFUL network response - INITIAL
    private bool NetSuccessInitial(string result)
    {
        LOG("HTTP INITIAL RESPONSE: " + result, LOG_DEBUG);

        // successful response?
        try
        {
            // parse the response
            var payloadResponse = Json.ToObj<Dictionary<string, object>>(result);

            if (payloadResponse.ContainsKey("success") && (string)payloadResponse["success"] != "1")
            {
                // success was not 1, trigger failed network behavior
                return false;
            }
        }
        catch (Exception e)
        {
            LOG("Failed to parse initial response success: " + e.Message, LOG_WARN);
            return false;
        }

        // update profile - we've received the initial
        profile.initialComplete = true;
        profile.Save();

        // adjust attribution request if it exists from this moment (otherwise it would have been based on init completing time)
        foreach (var ev in itemQueue.queue)
        {
            if (ev.properties.action == "get_attribution")
            {
                LOG("Adjusting attribution request from now", LOG_DEBUG);
                ev.properties.hold = Util.UnixTime() + profile.getAttributionWait;
                break;
            }
        }

        return true;
    }

    // callback for FAILED network response - INITIAL
    private void NetFailInitial(string errorMessage)
    {
        LOG("HTTP INITIAL FAILED: " + errorMessage, LOG_DEBUG);
    }

    // adjusts the timer and resets the attribution request
    private void ResetAttributionRequest(double secs)
    {
        // adjust attribution request if it exists from this moment (otherwise it would have been based on init completing time)
        foreach (var ev in itemQueue.queue)
        {
            if (ev.properties.action == "get_attribution")
            {
                LOG("Adjusting attribution request to " + secs + " s from now", LOG_DEBUG);
                ev.properties.sendStatus = SEND_READY;
                ev.properties.hold = Util.UnixTime() + secs;
                break;
            }
        }
    }

    // ATTRIBUTION CALLBACKS
    // callback for SUCCESSFUL network response - ATTRIBUTION
    private bool NetSuccessAttribution(string result)
    {
        LOG("HTTP ATTRIBUTION RESPONSE: " + result, LOG_DEBUG);

        // Deserialize JSON response from server
        Dictionary<string, object> serverResponse = new Dictionary<string, object>();
        string attributionDataStr = "";

        try
        {
            serverResponse = Json.ToObj<Dictionary<string, object>>(result);
            int success = int.Parse(serverResponse["success"].ToString());

            if (serverResponse.ContainsKey("success") && success == 1 && serverResponse.ContainsKey("data"))
            {
                Dictionary<string, object> attributionDataChunk = new Dictionary<string, object>();

                attributionDataChunk = (Dictionary<string, object>)serverResponse["data"];

                int retry = 0;
                if (attributionDataChunk.ContainsKey("retry"))
                {
                    retry = int.Parse(attributionDataChunk["retry"].ToString());
                    LOG("Request attribution retry: " + retry, LOG_DEBUG);
                }

                if (retry == -1)
                {
                    if (attributionDataChunk.ContainsKey("attribution"))
                    {
                        // extract the attribution object only
                        var attrDict = new Dictionary<string, object>();
                        attrDict.Add("attribution", attributionDataChunk["attribution"]);
                        attributionDataStr = Json.ToString(attrDict);
                        
                        // persist the attribution data (not in the same chunk as profile)        
                        LOG("Persisting attribution: " + attributionDataStr, LOG_DEBUG);
                        PlayerPrefs.SetString(Settings.keyAttribution, attributionDataStr);
                        // mark attribution as received
                        profile.attributionReceived = true;
                        profile.Save();

                        // trigger callback to host app
                        try
                        {
                            LOG("Attribution result received (will pass along to host): " + attributionDataStr, LOG_DEBUG);
                            if (_attributionCallbackDelegate != null) _attributionCallbackDelegate(attributionDataStr);
                        }
                        catch (Exception e)
                        {
                            LOG("NetSuccessAttribution() Failed to trigger attribution callback" + e.Message, LOG_DEBUG);
                        }
                    }
                }
                else if (retry >= 0) // retry
                {
                    // adjust attribution timer to fire again based on retry
                    if (retry == 0) ResetAttributionRequest(profile.getAttributionWait);
                    else ResetAttributionRequest(retry);
                    // return true so that we don't trigger a network failure and we don't complete attribution yet
                    return true;
                }

            }
            else
            {
                // success was not 1, trigger failed network behavior
                return false;
            }
        }
        catch (Exception e)
        {
            // something went wrong with the response -- we'll attempt this again on the next run
            LOG("NetSuccessAttribution() Failed Deserialize JSON attribution response: " + e.Message, LOG_WARN);
            profile.attributionReceived = false;
        }

        return true;
    }

    // callback for FAILED network response - ATTRIBUTION
    private void NetFailAttribution(string errorMessage)
    {
        LOG("HTTP ATTRIBUTION FAILED: " + errorMessage, LOG_DEBUG);
    }

    // callback for SUCCESSFUL network response - QUEUE
    private bool NetSuccessQueue(string result)
    {
        LOG("HTTP QUEUE RESPONSE: " + result, LOG_DEBUG);

        // successful response?
        try
        {
            // parse the response
            var payloadResponse = Json.ToObj<Dictionary<string, object>>(result);

            if (payloadResponse.ContainsKey("success") && (string)payloadResponse["success"] != "1")
            {
                // success was not 1, trigger failed network behavior
                return false;
            }
        }
        catch (Exception e)
        {
            LOG("Failed to parse tracker response success: " + e.Message, LOG_WARN);
            return false;
        }

        return true;
    }

    // callback for FAILED network response - QUEUE
    private void NetFailQueue(string errorMessage)
    {
        LOG("HTTP QUEUE FAILED: " + errorMessage, LOG_DEBUG);
    }

    // keeps track of cached data points
    private class CacheManager
    {
        public string key = "";
        public double expires = 0;
        public double staleness = 0;
        public object value;
        public CacheManager(string _key, double _staleness)
        {
            key = _key;
            expires = 0;
            staleness = _staleness;
        }
    }
    private List<CacheManager> cache;

    // prepares to fetch a data point -- after checking the cache and blacklist
    private object getDataPoint(string key)
    {
        // create the cache if doesnt exist yet?
        // any items on this list will use a cached value when able
        // staleness in seconds: -1 = once per run
        if (cache == null)
        {
            cache = new List<CacheManager>();
            cache.Add(new CacheManager(Settings.advertisingIDKey, -1));
            cache.Add(new CacheManager("os_version", -1));
            cache.Add(new CacheManager("platform", -1));
            cache.Add(new CacheManager("package", -1));
            cache.Add(new CacheManager("architecture", -1));
            cache.Add(new CacheManager("processor", -1));
            cache.Add(new CacheManager("app_short_string", -1));
            cache.Add(new CacheManager("app_version", -1));
            cache.Add(new CacheManager("device", -1));
            cache.Add(new CacheManager("device_cores", -1));
            cache.Add(new CacheManager("device_memory", -1));
            cache.Add(new CacheManager("graphics_device_name", -1));
            cache.Add(new CacheManager("graphics_device_vendor", -1));
            cache.Add(new CacheManager("graphics_memory_size", -1));
            cache.Add(new CacheManager("graphics_device_version", -1));
            cache.Add(new CacheManager("graphics_shader_level", -1));
            cache.Add(new CacheManager("language", 300));
            cache.Add(new CacheManager("disp_count", 300));
        }

        // blacklisted data point?
        if (profile.blacklist != null && Array.FindAll(profile.blacklist, s => s.Equals(key)).Length > 0)
        {
            LOG("Blacklisted key (skipped): " + key, LOG_DEBUG);
            return null;
        }

        // use cached value?        
        foreach (var item in cache)
        {
            if (key == item.key)
            {
                if (item.expires <= Util.UnixTime() && (item.staleness >= 0 || item.value == null))
                {
                    // needs to refresh - get it and set future expiration time
                    item.expires = Util.UnixTime() + item.staleness;
                    item.value = getDataPointDirect(key);
                }
                // return the value of this now
                return item.value;
            }
        }

        // if we reached this point we're dealing with a non-cached value, just get it directly
        return getDataPointDirect(key);
    }

    // constructs a payload based on an event
    private void ConstructPayload(Item ev)
    {
        // make sure action is added
        LOG("Constructing payload for " + ev.properties.action, LOG_TRACE);
        Util.SetValue(ref ev.top, "action", ev.properties.action);

        // assume complete
        ev.properties.incomplete = false;

        // create the list of keys to use for this type of payload
        var keys = new List<string>();
        var keysData = new List<string>();

        // envelope
        if (ev.properties.action == "init" || profile.initComplete)
        {
            keys.Add("kochava_app_id");
            keys.Add("kochava_device_id");
        }
        else
        {
            // we still need to get the init response before we can set these
            ev.properties.incomplete = true;
        }
        keys.Add("sdk_version");
        keys.Add("sdk_protocol");
        keys.Add("nt_id");
        // times
        keysData.Add("usertime");
        keysData.Add("uptime");

        if (ev.properties.action == "init")
        {
            keysData.Add("partner_name");
            keysData.Add("package");
            keysData.Add("platform");
            keysData.Add("os_version");
			keys.Add("sdk_build_date");
        }

        // dealing with the blacklist items beyond this point (if we don't have it yet we'll have to come back later)
        if (ev.properties.action == "initial" || ev.properties.action == "event" || ev.properties.action == "session" || ev.properties.action == "deeplink")
        {
            // is blacklist ready? if not we'll come back to this
            // (if this event is constructed before init completes the previous init blacklist will be used if able)
            if (!profile.initComplete)
            {
                ev.properties.incomplete = true;
            }
            else
            {
                // construct the list of payload 

                if (ev.properties.action == "initial")
                {
                    // first add custom values directly (only if whitelisted)
                    if (_dictionaryCustom != null)
                    {
                        foreach (var it in _dictionaryCustom)
                        {
                            try
                            {
                                if (profile.whitelist != null && Array.FindAll(profile.whitelist, s => s.Equals(it.Key)).Length > 0)
                                {
                                    LOG("Sending whitelisted key: " + it.Key, LOG_DEBUG);
                                    Util.SetValue(ref ev.data, it.Key, it.Value);
                                }
                                else LOG("Custom key not whitelisted: " + it.Key, LOG_ERROR);
                            }
                            catch (Exception e)
                            {
                                LOG("Custom value payload error: " + e.Message, LOG_ERROR);
                            }
                        }
                    }
                    // next create the normal list (existing custom values will not be overwritten)
                    keysData.Add(Settings.advertisingIDKey);
                    keysData.Add("package");
                    keysData.Add("language");
                    keysData.Add("app_limit_tracking");
                    keysData.Add("device_limit_tracking");
                }

                // shared datapoints
                keysData.Add("os_version");
                keysData.Add("disp_w");
                keysData.Add("disp_h");
                keysData.Add("architecture");
                keysData.Add("processor");
                keysData.Add("app_short_string");
                keysData.Add("app_version");
                keysData.Add("processor");
                keysData.Add("device");
                keysData.Add("device_cores");
                keysData.Add("device_memory");
                keysData.Add("graphics_device_name");
                keysData.Add("graphics_device_vendor");
                keysData.Add("graphics_memory_size");
                keysData.Add("graphics_device_version");
                keysData.Add("graphics_shader_level");
                keysData.Add("device_orientation");
                keysData.Add("product_name");

                // unique
                keysData.Add("connected_devices");
                keysData.Add("disp_count");

            }
        }

        // top level
        foreach (var k in keys)
        {
            try
            {
                Util.SetValue(ref ev.top, k, getDataPoint(k));
            }
            catch (Exception e)
            {
                LOG("ConstructPayload - keys failed (" + k + "): " + e.Message, LOG_DEBUG);
            }
        }

        // data chunk
        if (keysData.Count > 0)
        {
            foreach (var k in keysData)
            {
                try
                {
                    Util.SetValue(ref ev.data, k, getDataPoint(k));
                }
                catch (Exception e)
                {
                    LOG("ConstructPayload - keysData failed (" + k + "): " + e.Message, LOG_DEBUG);
                }
            }
        }

    }

    // gets the value for a data point based on the key
    private object getDataPointDirect(string key)
    {

        if (key == Settings.advertisingIDKey) return DeviceInfo.advertisingIdentifier;
        else if (key == "disp_count") return Display.displays.Length;
        else if (key == "uptime") return Util.UnixTime() - timeStart;
        else if (key == "usertime") return (int)Util.UnixTime();
        else if (key == "kochava_app_id") return profile.appGUID;
        else if (key == "app_limit_tracking") return _appAdLimitTracking;
        else if (key == "nt_id") return instance.instanceId + System.Guid.NewGuid().ToString();
        else if (key == "partner_name") return _partnerName;
        else if (key == "kochava_device_id") return profile.kochavaDeviceID;
        else if (key == "sdk_protocol") return Settings.sdkProtocol;
		else if (key == "sdk_build_date") return sdkBuildDate;
        else if (key == "sdk_version") return Settings.sdkVersion;
        else if (key == "platform") return DeviceInfo.platform;
        else if (key == "architecture") return DeviceInfo.architecture;
        else if (key == "processor") return SystemInfo.processorType;
        else if (key == "device") return SystemInfo.deviceModel;
        else if (key == "device_cores") return SystemInfo.processorCount;
        else if (key == "device_memory") return SystemInfo.systemMemorySize;
        else if (key == "graphics_device_name") return SystemInfo.graphicsDeviceName;
        else if (key == "graphics_device_vendor") return SystemInfo.graphicsDeviceVendor;
        else if (key == "graphics_memory_size") return SystemInfo.graphicsMemorySize;
        else if (key == "graphics_device_version") return SystemInfo.graphicsDeviceVersion;
        else if (key == "graphics_shader_level") return SystemInfo.graphicsShaderLevel;
        else if (key == "disp_w") return DeviceInfo.dispW;
        else if (key == "disp_h") return DeviceInfo.dispH;
        else if (key == "app_short_string") return _productName;
#if UNITY_5_0_0_NEWER
        else if (key == "app_version") return _productName + " " + Application.version;
#else
        else if (key == "app_version") return _productName;
#endif
        else if (key == "product_name") return SystemInfo.deviceName;
		else if (key == "package") return _productName;
        else if (key == "language") return Application.systemLanguage.ToString();
        else if (key == "device_orientation") return DeviceInfo.orientation;
        else if (key == "device_limit_tracking")
        {
            if (((string)DeviceInfo.advertisingIdentifier).Length < 1) return true;
            else return false;
        }
        else if (key == "connected_devices")
        {
            // list of connected controllers/input devices
            var joys = Input.GetJoystickNames();
            if (Input.mousePresent)
            {
                Array.Resize(ref joys, joys.Length + 1);
                joys[joys.Length - 1] = "Mouse";
            }
            if (joys.Length > 0) return joys;
            else return null;
        }
        else if (key == "os_version") return SystemInfo.operatingSystem;        

        return null;
    }

    // check / update the watchlist
    private void CheckWatchlist()
    {
        if (sessionInitComplete && profile.sendUpdates)
        {
            // watchlist items: if we've already sent the initial we'll check for updated items to send            
            LOG("Checking watchlist...", LOG_DEBUG);
            try
            {
                // check for changes to watched values
                var updates = new Dictionary<string, object>();
                foreach (var it in profile.watchlist)
                {
					//We skip the app limit ad tracking item until it has been set.
					if(it.Key == "app_limit_tracking" && !_appAdLimitTrackingSet)
					{
						continue;
					}

                    object val = getDataPoint(it.Key);                    
                    if (val != null && !it.Value.Equals(val))
                    {
                        LOG("Watchlist key updated: " + it.Key + " -> " + val.ToString(), LOG_DEBUG);                        
                        updates.Add(it.Key, val);
                    }
                }
                if (updates.Count > 0)
                {
                    // apply updates to watchlist dictionary and prepare update payload                    
                    var ev = new Item("update");
                    ev.properties.sendStatus = SEND_READY;
                    foreach (var it in updates)
                    {
                        Util.SetValue(ref profile.watchlist, it.Key, it.Value, false, false);
                        Util.SetValue(ref ev.data, it.Key, it.Value);
                    }
                    // we only need to send an update if the initial has already been completed
                    if (profile.initialComplete)
                    {
                        LOG("Sending watchlist update...", LOG_DEBUG);
                        ConstructPayload(ev);
                        itemQueue.Add(ev);
                    } else
                    {
                        LOG("Watchlist update skipped (initial not complete)", LOG_DEBUG);
                    }
                    // persist new watchlist values
                    profile.Save();
                }
            }
            catch (Exception e)
            {
                LOG("Watchlist update error: " + e.Message, LOG_ERROR);
            }
        }
    }

    // updates the pause session and sets the next time to update
    private void UpdatePauseSession()
    {
        if (Util.UnixTime() >= sessionPauseLastUpdated + sessionPauseUpdateWait)
        {
            LOG("Updating Pause Event", LOG_DEBUG);
            CreateSessionPauseEvent();
            sessionPauseLastUpdated = Util.UnixTime();
            sessionPauseUpdateWait += Settings.PauseUpdate;
            if (sessionPauseUpdateWait > 300) sessionPauseUpdateWait = 300; // dont go beyond 5 minute updates
        }
        WakeController((sessionPauseLastUpdated + sessionPauseUpdateWait) - Util.UnixTime());
    }

    // processes items from intake queue
    public void ProcessIntakeQueue()
    {
        // move from intake queue
        while (intakeQueue.Count > 0)
        {
            // move this to the primary queue
            LOG("Adding item from intakeQueue: " + intakeQueue[0].properties.action, LOG_TRACE);
            var intakeItem = intakeQueue[0];
            intakeQueue.RemoveAt(0);
            // drop it now if blacklisted
			if (profile.eventname_blacklist != null && intakeItem.properties.action == "event" && Array.FindAll(profile.eventname_blacklist, s => s.Equals(intakeItem.data["event_name"])).Length > 0)
            {
                LOG("Blacklisted event name (dropping it): " + intakeItem.properties.action, LOG_DEBUG);
            }
            else
            {
                // special case - identity link
                if (intakeItem.properties.action == "identityLink")
                {
                    try
                    {
                        // we'll build a new dictionary of only the deltas
                        var newIdLink = new Dictionary<string, string>();
                        LOG("Processing Identity Link values...", LOG_DEBUG);
                        // 1) remove keys which have already been sent in the past (if any exist)
                        var pastIdLink = Json.ToObj<Dictionary<string, string>>(profile.sentIdentityLinks);
                        if (pastIdLink == null) pastIdLink = new Dictionary<string, string>(); // make sure we're working with an empty dictionary rather than null
                        foreach (var it in intakeItem.data)
                        {
                            bool duplicateEntry = false;
                            string pastValue;
                            if (pastIdLink.ContainsKey(it.Key) && pastIdLink.TryGetValue(it.Key, out pastValue))
                            {
                                if (pastValue != null && pastValue == (string)it.Value) duplicateEntry = true;
                            }
                            if (!duplicateEntry) newIdLink.Add(it.Key, (string)it.Value);
                        }
                        if (newIdLink.Count == 0)
                        {
                            // nothing to update, skip it
                            LOG("No updated Identity Link values found ("+ pastIdLink.Count + " previous)", LOG_DEBUG);
                            intakeItem = null;
                        } 
                        else
                        {
                            // 2) newIdLink now contains only the updated values -- update our past list and rebuild the payload with these new values
                            intakeItem.data.Clear();
                            foreach (var it in newIdLink)
                            {
                                if (pastIdLink.ContainsKey(it.Key)) pastIdLink.Remove(it.Key);
                                pastIdLink.Add(it.Key, it.Value);
                                intakeItem.data.Add(it.Key, it.Value);
                            }
                            if (pastIdLink.Count < 100) // don't save more beyond this many (this means that each identity link after 100 will go out regardless)
                            {
                                profile.sentIdentityLinks = Json.ToString(pastIdLink);
                                profile.Save();
                            }
                            // 3) if initial hasn't gone out yet add it to the existing payload instead of sending it alone
                            foreach (var ev in itemQueue.queue)
                            {
                                if (ev.properties.action == "initial")
                                {
                                    LOG("Adding new identity link values to Inital", LOG_DEBUG);
                                    var idLinkChunk = intakeItem.data;
                                    if (ev.data.ContainsKey("identity_link")) // if it already has an identity link chunk this will need to be added to it
                                    {
                                        object currentIdLink;
                                        ev.data.TryGetValue("identity_link", out currentIdLink);
                                        foreach (var c in ((Dictionary<string, object>)currentIdLink))
                                        {
                                            if (!idLinkChunk.ContainsKey(c.Key)) idLinkChunk.Add(c.Key, (string)c.Value);
                                        }
                                        ev.data.Remove("identity_link");
                                    }
                                    ev.data.Add("identity_link", idLinkChunk);
                                    // clear it so it's not used again
                                    intakeItem = null;
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        LOG("Failed to parse identity link: " + e.Message, LOG_ERROR);
                    }
                }
                // send if still valid
                if (intakeItem != null)
                {
                    // construct the rest of the payload (if able)
                    ConstructPayload(intakeItem);
                    // add it to the primary queue
                    itemQueue.Add(intakeItem);
                }

            }
        }
    }

    // handle initialization if needed
    public void ProcessInitialize()
    {
        // has not yet initialized
        if (!initialized)
        {
            LOG("Initialization sequence started...", LOG_DEBUG);

            // reset time(s)
            timeStart = Util.UnixTime();

            // initialize device info
            DeviceInfo.Initialize();

            // set product name
			if (_productName.Length < 1)
			{
                if(!string.IsNullOrEmpty(_appGUID))
                {
                    _productName = _appGUID;
                } else
                {
                    string temp = (_partnerName + "." + _partnerApp + "." + DeviceInfo.platform.ToString()).Replace (" ", "_");
                    Regex rgx = new Regex("[^a-zA-Z0-9._]");
                    _productName = rgx.Replace(temp, "");
                }
			}

            // decide which key we'll use for the advertising identifier
#if UNITY_STANDALONE_WIN
            // native windows (oculus, etc.)
            Settings.advertisingIDKey = "waid";
#elif UNITY_WSA
            // windows store app
            Settings.advertisingIDKey = "waid";
#endif

            // create our network manager by adding it as a component so that we can use coroutines within it
            networkManager = gameObject.AddComponent<NetworkManager>();

            // initialize the item and intake queue
            itemQueue = new ItemQueue();
            intakeQueue = new List<Item>();

            // create the profile and then populate with persisted data and input parameters
            profile = new Profile();

            // load persisted data from previous run?
            try
            {
                string data = PlayerPrefs.GetString(Settings.keyProfile);
                if (data != null && data.Length > 0)
                {
                    profile = Json.ToObj<Profile>(data);                    
                    LOG("READ PERSISTED DATA (" + Settings.keyProfile + "): " + data, LOG_TRACE);
                    // if we made it this far the persisted data was parsed correctly and we'll set the persisted flag to true
                    profile.persisted = true;
                    // if the profile was loaded its going to contain the app guid from the previous launch so lets make sure we use the most current                    
                    profile.appGUID = _appGUID;
                    // if we've received an override app guid in the past make sure we use that one instead
                    if(profile.overrideAppGUID.Length>0) profile.appGUID = profile.overrideAppGUID;
                    LOG("Loaded persisted profile", LOG_DEBUG);
                }
            }
            catch (Exception e)
            {
                LOG("Error reading persisted data: " + e.Message, LOG_ERROR);
                // reset the profile object since this failed
                profile = new Profile();
            }

            // check for any temporary sessions persisted from a previous run
            ImportTemporaryQueue(true);

            // now attempt to load the persisted event queue (otherwise make sure it's empty)
            if (profile.persisted) itemQueue.Load();
            else if (itemQueue.queue.Count > 0) itemQueue = new ItemQueue();

            // will init be skipped?
            bool skippingInit = (Util.UnixTime() - profile.initLastSent < profile.initWait);

            // if we're skipping the init we need to use the same appGUID from the last session
            if (!skippingInit && profile.persisted)
            {
                LOG("Using appGUID from previous session: " + profile.appGUID, LOG_TRACE);
            }

            // new profile? set some default values
            if (!profile.persisted)
            {
                LOG("Creating new profile...", LOG_DEBUG);
                // create a new kochava device id                
                profile.kochavaDeviceID = "KU" + System.Guid.NewGuid().ToString().Replace("-", "");
                LOG("Created new kdid: " + profile.kochavaDeviceID, LOG_DEBUG);
            }

            // flag as initialized
            initialized = true;

            LOG("Kochava initialized @ " + (Util.UnixTime() - timeStart) + "s", LOG_INFO);

            // queue the init
            if (skippingInit) // skip init
            {
                LOG("INIT: Skipping (last was " + (int)(Util.UnixTime() - profile.initLastSent) + " s ago)", LOG_DEBUG);
                CompleteInit();
            }
            else // sending
            {
                var ev = new Item("init");
                ev.properties.temporary = true;
                ev.properties.wait = true;
                ev.properties.sendStatus = SEND_READY;
                ConstructPayload(ev);
                itemQueue.Add(ev);
            }

            // queue the initial?
            if (!profile.initialComplete)
            {
                LOG("Initial will send...", LOG_DEBUG);
                var ev = new Item("initial");
                ev.properties.unique = true;
                ev.properties.wait = true;
                ev.properties.instant = true;
                ConstructPayload(ev);
                itemQueue.Add(ev);
            }

            // queue attribution request?
            if (_requestAttributionCallback && !profile.attributionReceived)
            {
                LOG("Attribution will send...", LOG_DEBUG);
                var ev = new Item("get_attribution");
                ev.properties.temporary = true;
                ev.properties.unique = true;
                ev.properties.sendStatus = SEND_READY;
                // for attribution we need to wait a few seconds before requesting it
                ev.properties.hold = Util.UnixTime() + profile.getAttributionWait;
                ConstructPayload(ev);
                itemQueue.Add(ev);
            }

            // session resume if this was not the first launch
            if (profile.persisted && profile.sessionTracking)
            {
                var ev = new Item("session");
                ev.data.Add("state", "resume");
                ConstructPayload(ev);
                itemQueue.Add(ev);
            }

        }
    }

    // fetches device data points
    private class DeviceInfo
    {


        // --- PROPERTIES

        // flags
        private static bool asyncComplete = false;
        private static bool asynGotAdvertisingID = false;

        // --- DATA POINTS

        // advertising id (idfa, adid, waid, etc)
        private static string advertisingId = "";
        public static object advertisingIdentifier
        {
            get
            {
                if (asynGotAdvertisingID == false) return "";
                else return advertisingId;
            }
        }
        // alternate unique id -- https://docs.unity3d.com/550/Documentation/ScriptReference/SystemInfo-deviceUniqueIdentifier.html        
        public static object uniqueIdentifier { get { return SystemInfo.deviceUniqueIdentifier; } }

        // screen size
        public static object dispW { get { return Screen.width; } }
        public static object dispH { get { return Screen.height; } }

        // system
        public static object platform
        {
            get
            {
                if (Application.isEditor) return "UnityEditor";
#if UNITY_EDITOR
                return "UnityEditor";
#elif UNITY_WEBGL
                return "WebGL";
#elif UNITY_STANDALONE_OSX
                return "MacOSX";
#elif UNITY_STANDALONE_LINUX
                return "Linux";
#elif UNITY_STANDALONE_WIN
                return "WindowsDesktop";
#elif UNITY_WII
                return "Wii";
#elif UNITY_PS4
                return "PS4";
#elif UNITY_XBOXONE
                return "XboxOne";
#elif UNITY_TIZEN
                return "Tizen";
#elif UNITY_TVOS
                return "tvos";
#elif UNITY_IOS //Native SDK defines are here for completeness but should never get hit.
                return "ios";
#elif UNITY_ANDROID
                return "android";
#elif UNITY_WSA
                return "windows";
#endif
                return "Other";
            }
        }

        public static object architecture
        {
            get
            {
                var str = System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
                return str;
            }
        }

        public static object orientation
        {
            get
            {
                if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft || Input.deviceOrientation == DeviceOrientation.LandscapeRight) return "landscape";
                else if (Input.deviceOrientation == DeviceOrientation.Portrait || Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown || (Screen.width < Screen.height)) return "portrait";
                return "landscape";
            }
        }

        // --- METHODS

        // shims by platform:
#if UNITY_STANDALONE_WIN
        [DllImport("KochavaShimWindows")]
        private static extern IntPtr getWaid();
#endif

        // initialize applicable device data points
        public static void Initialize()
        {
            LOG("DeviceInfo initialization started...", LOG_DEBUG);

            // advertising id collection
#if UNITY_STANDALONE_WIN
            // native windows will grab the advertising id via the native shim (KochavaShimWindows.dll)

            string waid = "";
            try
            {
                LOG("WAID collection started..", LOG_DEBUG);
                waid = Marshal.PtrToStringAuto(getWaid());
            }
            catch (Exception e)
            {
                LOG("WAID collection failed: " + e.Message, LOG_DEBUG);
                waid = "";
            }
            gotAdIdCallback(waid, true, "");
#elif UNITY_WSA
            // attmepting to target a windows store version less than 10 or without ENABLE_WINMD_SUPPORT = true (i.e. may not using Unity 2017 or targeting windows 8.1)
            LOG("WSA WAID collection via SystemInfo.deviceUniqueIdentifier...", LOG_DEBUG);
            gotAdIdCallback(SystemInfo.deviceUniqueIdentifier, true, "");            
            return;
#else
            // no advertising identifier to get - simulate an empty callback
            LOG("Advertising ID skipped...", LOG_DEBUG);
            gotAdIdCallback("", true, "");
#endif

        }

        // callback for async advertising id collection
        private static void gotAdIdCallback(string _advertisingId, bool _trackingEnabled, string errorMsg)
        {
            LOG("GOT ADVERTISING ID: " + _advertisingId + " Enabled:" + _trackingEnabled + " " + errorMsg, LOG_DEBUG);
            advertisingId = _advertisingId;
            asynGotAdvertisingID = true;
            isReady();
        }

        // returns true if all waiting/async data point initialization has been completed
        public static bool isReady()
        {
            // return false if any async data points have not been initialized yet
            if (asynGotAdvertisingID == false)
            {
                return false;
            }
            // all async data points have been initialized, we're ready
            if (asyncComplete == false)
            {
                asyncComplete = true;
                LOG("DeviceInfo initialization complete", LOG_DEBUG);
            }
            return true;
        }

    }

#endif

        }
