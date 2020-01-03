using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class KochavaConfiguration : MonoBehaviour {

	[Serializable]
	public class ConsentStatusChangeEvent : UnityEvent {}

	[Serializable]
	public class AttributionChangeEvent : UnityEvent<string> {}

    // Editor-configurable settings
    #region Settings

    [Header("App GUID Configuration")]
    public string appGUID_UnityEditor = "";
    public string appGUID_iOS = "";
    public string appGUID_Android = "";
    public string appGUID_WindowsStoreUWP = "";
    public string appGUID_WindowsPC = "";
    public string appGUID_MacOS = "";
    public string appGUID_Linux = "";
    public string appGUID_WebGL = "";

	[Header("Partner Configuration (Optional)")]
	public string kochavaPartnerName = "";
	public string kochavaPartnerApp = "";
    
    [Header("Settings")]
    public bool appAdLimitTracking = false;
	public Kochava.DebugLogLevel logLevel;
    public bool sleepOnStart = false;
    
    [Header("Attribution Callback")]
    public bool requestAttributionCallback = false;
	public AttributionChangeEvent attributionChangeEvent = null;
    
    [Header("Intelligent Consent Management")]
	public bool intelligentConsentManagement = false;
    public bool manualManagedConsentRequirements = false;
	public ConsentStatusChangeEvent consentStatusChangeEvent = null;
    
    #endregion

    // initialize kochava -- it will be ready to use during Start()
    void Awake()
    {
        // DO NOT EDIT THIS CODE        

        // decide which app guid to use
        string platformAppGUID = appGUID_UnityEditor;
#if UNITY_IPHONE && !UNITY_EDITOR
        if(appGUID_iOS.Length > 0) platformAppGUID = appGUID_iOS;
#elif UNITY_ANDROID && !UNITY_EDITOR
        if(appGUID_Android.Length > 0) platformAppGUID = appGUID_Android;
#elif UNITY_WEBGL && !UNITY_EDITOR
        if(appGUID_WebGL.Length > 0) platformAppGUID = appGUID_WebGL;
#elif UNITY_STANDALONE_OSX && !UNITY_EDITOR
        if(appGUID_MacOS.Length > 0) platformAppGUID = appGUID_MacOS;
#elif UNITY_STANDALONE_LINUX && !UNITY_EDITOR
        if(appGUID_Linux.Length > 0) platformAppGUID = appGUID_Linux;
#elif UNITY_STANDALONE_WIN && !UNITY_EDITOR
        if(appGUID_WindowsPC.Length > 0) platformAppGUID = appGUID_WindowsPC;
#elif UNITY_WSA_10_0 && !UNITY_EDITOR
        if(appGUID_WindowsStoreUWP.Length > 0) platformAppGUID = appGUID_WindowsStoreUWP;
#endif

        // initialize Kochava using the configuration provided
        // NOTE: If you wish to initialize Kochava elsewhere (in your own code) please consult the documentation for examples at https://support.kochava.com/sdk-integration/unity-sdk-integration
        Kochava.Tracker.Config.SetLogLevel(logLevel);
        Kochava.Tracker.Config.SetAppGuid(platformAppGUID);
        Kochava.Tracker.Config.SetRetrieveAttribution(requestAttributionCallback);
		Kochava.Tracker.Config.SetIntelligentConsentManagement(intelligentConsentManagement);
        Kochava.Tracker.Config.SetPartnerName(kochavaPartnerName);
		Kochava.Tracker.Config.SetPartnerApp(kochavaPartnerApp);
        Kochava.Tracker.Config.SetSleep(sleepOnStart);

		// To prevent overriding values set using the setter we only apply this value once on the first launch.
		bool latSet = PlayerPrefs.HasKey("kou__latset");
		if (!latSet)
		{
			Kochava.Tracker.Config.SetAppLimitAdTracking(appAdLimitTracking);
			PlayerPrefs.SetInt ("kou__latset", 1);
		}

        Kochava.Tracker.Initialize();

		if (requestAttributionCallback && attributionChangeEvent != null)
		{
            Kochava.Tracker.SetAttributionHandler((Kochava.AttributionCallbackDelegate)((string attribution)=>attributionChangeEvent.Invoke(attribution)));
        }

		if (intelligentConsentManagement && consentStatusChangeEvent != null)
		{
			Kochava.Tracker.SetConsentStatusChangeHandler((Kochava.ConsentStatusChangeDelegate)(()=>consentStatusChangeEvent.Invoke()));
		}

        // Ensure this config game object is not destroyed.
        DontDestroyOnLoad(gameObject);
    }

}
