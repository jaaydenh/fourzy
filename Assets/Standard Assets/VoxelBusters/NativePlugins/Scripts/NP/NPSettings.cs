using UnityEngine;
using System.Collections;
using VoxelBusters.Utility;
using VoxelBusters.AssetStoreProductUtility;
using System;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityPlayerSettings	= UnityEditor.PlayerSettings;
using VBPlayerSettings		= VoxelBusters.Utility.PlayerSettings;

[assembly: InternalsVisibleTo("Assembly-CSharp-Editor")]
#endif

namespace VoxelBusters.NativePlugins
{
	using Internal;

#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	public class NPSettings : 	AdvancedScriptableObject <NPSettings>, 
#if UNITY_EDITOR
								IRateMyAppDelegate,
#endif
								IAssetStoreProduct
	{
		#region Constants

		// Product info
		private		const	bool		kIsFullVersion					= true;
		private 	const 	string 		kProductName					= "Native Plugins";
		private 	const 	string 		kProductVersion					= "1.5.1";

		// Pref key
		private		const	string		kPrefsKeyBuildIdentifier		= "np-build-identifier";
		internal	const	string		kPrefsKeyPropertyModified		= "np-property-modified";
		internal	const	string 		kMethodPropertyChanged 			= "OnPropertyModified";		

		// Macro symbols
		internal	const 	string		kLiteVersionMacro				= "NATIVE_PLUGINS_LITE_VERSION";
		private		const	string		kMacroAddressBook				= "USES_ADDRESS_BOOK";
		private		const	string		kMacroBilling					= "USES_BILLING";
		private		const	string		kMacroCloudServices				= "USES_CLOUD_SERVICES";
		private		const	string		kMacroGameServices				= "USES_GAME_SERVICES";
		private		const	string		kMacroMediaLibrary				= "USES_MEDIA_LIBRARY";
		private		const	string		kMacroNetworkConnectivity		= "USES_NETWORK_CONNECTIVITY";
		private		const	string		kMacroNotificationService		= "USES_NOTIFICATION_SERVICE";
		private		const	string		kMacroSharin					= "USES_SHARING";
		private		const	string		kMacroTwitter					= "USES_TWITTER";
		private		const	string		kMacroWebView					= "USES_WEBVIEW";

		// External SDKs macro symbols
		private		const	string		kMacroSoomlaGrowService			= "USES_SOOMLA_GROW";
		private		const	string		kMacroOneSignalService			= "USES_ONE_SIGNAL";

		// Macro actions
#if UNITY_EDITOR
		private 	readonly char[] 	defineSeperators 				= new char[] {
			';',
			',',
			' '
		};
	
		private		readonly BuildTargetGroup[]	buildTargetGroups		= new BuildTargetGroup[]{
			BuildTargetGroup.Android,
#if UNITY_5 || UNITY_6 || UNITY_7
			BuildTargetGroup.iOS,
#else
			BuildTargetGroup.iPhone, 
#endif
#if UNITY_5 || UNITY_6 || UNITY_7			
			BuildTargetGroup.WSA, 
#else
			BuildTargetGroup.Metro,
			BuildTargetGroup.WP8,
#endif
#if !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3_0) && (UNITY_5 || UNITY_6 || UNITY_7)
			BuildTargetGroup.tvOS,
#endif
			BuildTargetGroup.Standalone

		};
#endif
		
		#endregion

		#region Fields

		[NonSerialized]
		private	AssetStoreProduct			m_assetStoreProduct;
		private	RateMyApp					m_rateMyApp;
		[SerializeField]
		[HideInInspector]
		private	string						m_lastOpenedDateString;

		[SerializeField]
		private ApplicationSettings			m_applicationSettings			= new ApplicationSettings();
#if USES_NETWORK_CONNECTIVITY
		[SerializeField]
		private NetworkConnectivitySettings	m_networkConnectivitySettings	= new NetworkConnectivitySettings();
#endif
		[SerializeField]
		private UtilitySettings				m_utilitySettings				= new UtilitySettings();
#if USES_BILLING
		[SerializeField]
		private BillingSettings				m_billingSettings				= new BillingSettings();
#endif
#if USES_CLOUD_SERVICES
		[SerializeField]
		private CloudServicesSettings		m_cloudServicesSettings			= new CloudServicesSettings();
#endif
#if USES_MEDIA_LIBRARY
		[SerializeField]
		private MediaLibrarySettings		m_mediaLibrarySettings			= new MediaLibrarySettings();
#endif
#if USES_NOTIFICATION_SERVICE
		[SerializeField]
		private NotificationServiceSettings	m_notificationSettings			= new NotificationServiceSettings();
#endif
#if USES_TWITTER
		[SerializeField]
		private SocialNetworkSettings		m_socialNetworkSettings			= new SocialNetworkSettings();
#endif
#if USES_GAME_SERVICES
		[SerializeField]
		private GameServicesSettings		m_gameServicesSettings			= new GameServicesSettings();
#endif
		[SerializeField]
		private	AddonServicesSettings		m_addonServicesSettings			= new AddonServicesSettings();

		#endregion

		#region Static Properties

		/// <summary>
		/// Gets the application settings.
		/// </summary>
		/// <value>The application settings.</value>
		public static ApplicationSettings Application
		{
			get 
			{ 
				return Instance.m_applicationSettings; 
			}
		}

#if USES_NETWORK_CONNECTIVITY
		/// <summary>
		/// Gets the network connectivity settings.
		/// </summary>
		/// <value>The network connectivity settings.</value>
		public static NetworkConnectivitySettings NetworkConnectivity
		{
			get 
			{ 
				return Instance.m_networkConnectivitySettings; 
			}
		}
#endif

		/// <summary>
		/// Gets the utility settings.
		/// </summary>
		/// <value>The utility settings.</value>
		public static UtilitySettings Utility
		{
			get 
			{ 
				return Instance.m_utilitySettings; 
			}
		}

#if USES_BILLING
		/// <summary>
		/// Gets the billing settings.
		/// </summary>
		/// <value>The billing settings.</value>
		public static BillingSettings Billing
		{
			get 
			{ 
				return Instance.m_billingSettings; 
			}
		}
#endif

#if USES_CLOUD_SERVICES
		/// <summary>
		/// Gets the cloud services settings.
		/// </summary>
		/// <value>The cloud services settings.</value>
		public static CloudServicesSettings CloudServices
		{
			get 
			{ 
				return Instance.m_cloudServicesSettings; 
			}
		}
#endif

#if USES_MEDIA_LIBRARY
		/// <summary>
		/// Gets the media library settings.
		/// </summary>
		/// <value>The media library settings.</value>
		public static MediaLibrarySettings MediaLibrary
		{
			get 
			{ 
				return Instance.m_mediaLibrarySettings; 
			}
		}
#endif

#if USES_NOTIFICATION_SERVICE
		/// <summary>
		/// Gets the notification settings.
		/// </summary>
		/// <value>The notification settings.</value>
		public static NotificationServiceSettings Notification
		{
			get 
			{ 
				return Instance.m_notificationSettings; 
			}
		}
#endif

#if USES_TWITTER
		/// <summary>
		/// Gets the twitter settings.
		/// </summary>
		/// <value>The twitter settings.</value>
		public static SocialNetworkSettings SocialNetworkSettings
		{
			get 
			{ 
				return Instance.m_socialNetworkSettings; 
			}
		}
#endif

#if USES_GAME_SERVICES
		/// <summary>
		/// Gets the Game Services settings.
		/// </summary>
		/// <value>The Game Services settings.</value>
		public static GameServicesSettings GameServicesSettings
		{
			get 
			{ 
				return Instance.m_gameServicesSettings; 
			}
		}
#endif

		public static AddonServicesSettings AddonServicesSettings
		{
			get
			{
				return Instance.m_addonServicesSettings; 
			}
		}

		#endregion

		#region Properties
		
		public AssetStoreProduct AssetStoreProduct 
		{
			get 
			{
				return m_assetStoreProduct;
			}
		}

		#endregion

		#region Constructor

#if UNITY_EDITOR && !DISABLE_NPSETTINGS_GENERATION
		static NPSettings ()
		{
			EditorUtils.Invoke(
				_method: ()=>
				{
					NPSettings _instance	= NPSettings.Instance;
					_instance.SaveConfigurationChanges();
				}, 
				_time: 1f);

			EditorUtils.InvokeRepeating(
				_method: ()=>
				{
					NPSettings _instance	= NPSettings.Instance;
					_instance.MonitorPlayerSettings();
				}, 
				_time: 1f, 
				_repeatRate: 1f);
		}
#endif

		#endregion

		#region Unity Methods

		protected override void Reset ()
		{
			base.Reset();

#if UNITY_EDITOR
			m_assetStoreProduct	= new AssetStoreProduct(kProductName, kProductVersion, Constants.kLogoPath);
#endif
		}

		protected override void OnEnable ()
		{
			base.OnEnable ();

			Initialise();
		}

		#endregion

		#region Private Methods

		private void Initialise ()
		{
			m_assetStoreProduct	= new AssetStoreProduct(kProductName, kProductVersion, Constants.kLogoPath);

#if UNITY_EDITOR
			SetupRateNPSettings();
			
#if USES_BILLING
			// Rebuilding asset bundles is required
			if (m_billingSettings.Products != null)
			{
				foreach (BillingProduct _billingProduct in m_billingSettings.Products)
					_billingProduct.RebuildObject();
			}
#endif

#endif
		}

#if UNITY_EDITOR
		private void SetupRateNPSettings ()
		{
			// Set rate my app 
			RateMyAppSettings _settings	= new RateMyAppSettings()
			{
				Title							= "Rate Cross Platform Native Plugins",
				Message							= "If you enjoy using Native Plugin, Don't forget to rate us. Its like an extra beer for our whole team ;) " +
					"It wont take more than a minute. Thanks for your support",
				ShowFirstPromptAfterHours		= 1,
				SuccessivePromptAfterHours 		= 0,
				SuccessivePromptAfterLaunches 	= 20,
				DontAskButtonText 				= null
			};
			m_rateMyApp				= new RateMyApp(_settings, new RateNPSettingsController());
			m_rateMyApp.Delegate	= this;

			// Record load
			DateTime _lastOpenedDate;
			DateTime.TryParse(m_lastOpenedDateString, out _lastOpenedDate);

			if ((DateTime.Today - _lastOpenedDate).TotalHours > 24)
			{
				m_lastOpenedDateString	= DateTime.Today.ToString();

				m_rateMyApp.RecordAppLaunch();
			}
			m_rateMyApp.AskForReview();
		}
#endif

		#endregion

		#region Editor Methods

#if UNITY_EDITOR
		public void SaveConfigurationChanges ()
		{
			// Actions
			UpdateDefineSymbols();
			UpdatePluginResources();
			WriteAndroidManifestFile();

			// Refresh Database
			AssetDatabase.Refresh();

			// Reset flags
			EditorPrefs.DeleteKey(kPrefsKeyPropertyModified);
		}

		private void UpdateDefineSymbols ()
		{
			foreach (BuildTargetGroup _curBuildTargetGroup in buildTargetGroups)
			{
				string[]		_curDefineSymbols	= UnityPlayerSettings.GetScriptingDefineSymbolsForGroup(_curBuildTargetGroup).Split(defineSeperators, StringSplitOptions.RemoveEmptyEntries);
				List<string>	_newDefineSymbols	= new List<string>(_curDefineSymbols);

				// Asset type
#pragma warning disable
				if (kIsFullVersion)
				{
					if (_newDefineSymbols.Contains(kLiteVersionMacro))
						_newDefineSymbols.Remove(kLiteVersionMacro);
				}
				else
				{
					if (!_newDefineSymbols.Contains(kLiteVersionMacro))
						_newDefineSymbols.Add(kLiteVersionMacro);
				}
#pragma warning restore

				ApplicationSettings.Features 		_supportedFeatures		= m_applicationSettings.SupportedFeatures;
				ApplicationSettings.AddonServices	_supportedAddonServices	= m_applicationSettings.SupportedAddonServices;

				// Regarding supported features
				AddOrRemoveFeatureDefineSymbol(_newDefineSymbols, _supportedFeatures.UsesAddressBook, 	kMacroAddressBook);
				AddOrRemoveFeatureDefineSymbol(_newDefineSymbols, _supportedFeatures.UsesBilling, 		kMacroBilling);
				AddOrRemoveFeatureDefineSymbol(_newDefineSymbols, _supportedFeatures.UsesCloudServices, kMacroCloudServices);
				AddOrRemoveFeatureDefineSymbol(_newDefineSymbols, _supportedFeatures.UsesGameServices, 	kMacroGameServices);
				AddOrRemoveFeatureDefineSymbol(_newDefineSymbols, _supportedFeatures.UsesMediaLibrary, 	kMacroMediaLibrary);
				AddOrRemoveFeatureDefineSymbol(_newDefineSymbols, _supportedFeatures.UsesNetworkConnectivity, kMacroNetworkConnectivity);
				AddOrRemoveFeatureDefineSymbol(_newDefineSymbols, _supportedFeatures.UsesNotificationService, kMacroNotificationService);
				AddOrRemoveFeatureDefineSymbol(_newDefineSymbols, _supportedFeatures.UsesSharing, kMacroSharin);
				AddOrRemoveFeatureDefineSymbol(_newDefineSymbols, _supportedFeatures.UsesTwitter, kMacroTwitter);
				AddOrRemoveFeatureDefineSymbol(_newDefineSymbols, _supportedFeatures.UsesWebView, kMacroWebView);

				// Regarding supported addon features
				AddOrRemoveFeatureDefineSymbol(_newDefineSymbols, _supportedAddonServices.UsesSoomlaGrow,	kMacroSoomlaGrowService);
				AddOrRemoveFeatureDefineSymbol(_newDefineSymbols, _supportedAddonServices.UsesOneSignal,	kMacroOneSignalService);

				// Now save these changes
				UnityPlayerSettings.SetScriptingDefineSymbolsForGroup(_curBuildTargetGroup, string.Join(";", _newDefineSymbols.ToArray()));
			}
		}

		private void AddOrRemoveFeatureDefineSymbol (List<string> _defineSymbols, bool _usesFeature, string _featureSymbol)
		{
			if (_usesFeature)
			{
				if (!_defineSymbols.Contains(_featureSymbol))
					_defineSymbols.Add(_featureSymbol);
			}
			else
			{
				if (_defineSymbols.Contains(_featureSymbol))
					_defineSymbols.Remove(_featureSymbol);
			}
		}

		private void WriteAndroidManifestFile ()
		{
			string _manifestFolderPath = Constants.kAndroidPluginsLibraryPath;
			
			if (AssetsUtility.FolderExists(_manifestFolderPath))
			{
				NPAndroidManifestGenerator _generator	= new NPAndroidManifestGenerator();
				_generator.SaveManifest("com.voxelbusters.androidnativeplugin", _manifestFolderPath + "/AndroidManifest.xml");
			}
		}
		
		private void UpdatePluginResources ()
		{
#if UNITY_ANDROID
			// Update JAR files
			UpdateResourcesBasedOnFeaturesUsage();
#endif
	
			// Copy required assets
			CopyNotificationAssets();
		}

		private void CopyNotificationAssets ()
		{
#if USES_NOTIFICATION_SERVICE
			// Copy save the texture data in res/drawable folder
			Texture2D[] _smallIcons = new Texture2D[]
			{	
				m_notificationSettings.Android.WhiteSmallIcon,
				m_notificationSettings.Android.ColouredSmallIcon
			};
			string[]	_paths		= new string[]
			{	
				Constants.kAndroidPluginsLibraryPath + "/res/drawable/app_icon_custom_white.png",
				Constants.kAndroidPluginsLibraryPath + "/res/drawable/app_icon_custom_coloured.png"
			};
			int 		_iconsConfiguredCount = 0;

			for (int _i = 0 ; _i < _smallIcons.Length ; _i++)
			{
				if (_smallIcons[_i] != null)
				{
					string _destinationPath = UnityEngine.Application.dataPath + "/../" + _paths[_i];

					IOExtensions.CopyFile(AssetDatabase.GetAssetPath(_smallIcons[_i]), _destinationPath, true);
					_iconsConfiguredCount++;
				}
			}
			
			if (_iconsConfiguredCount == 1)
			{
#if NP_DEBUG
				Debug.LogError("[NPSettings] You need to set both(white & coloured) icons for proper functionality on all devices. As, White icon will be used by post Android L devices and coloured one by pre Android L Devices.");
#endif
			}
#endif
		}

		private void MonitorPlayerSettings ()
		{
			string	_oldBuildIdentifier	= EditorPrefs.GetString(kPrefsKeyBuildIdentifier, null);
			string	_curBuildIdentifier	= VBPlayerSettings.GetBundleIdentifier();
			
			if (string.Equals(_oldBuildIdentifier, _curBuildIdentifier))
				return;
			
			// Update value
			EditorPrefs.SetString(kPrefsKeyBuildIdentifier, _curBuildIdentifier);

			// Save changes
			SaveConfigurationChanges();
		}
		
		private void UpdateResourcesBasedOnFeaturesUsage()
		{
			ApplicationSettings.Features 		_supportedFeatures		= m_applicationSettings.SupportedFeatures;
			ApplicationSettings.AddonServices 	_supportedAddOnServices	= m_applicationSettings.SupportedAddonServices;
			
			UpdateJARFile(_supportedFeatures.UsesAddressBook, 			Constants.kAddressBookJARName);
			UpdateJARFile(_supportedFeatures.UsesBilling, 				Constants.kBillingJARName);
			UpdateJARFile(_supportedFeatures.UsesBilling, 				Constants.kBillingAmazonJARName);
			UpdateJARFile(_supportedFeatures.UsesCloudServices, 		Constants.kCloudServicesJARName);
			UpdateJARFile(_supportedFeatures.UsesGameServices, 			Constants.kGameServicesJARName);
			UpdateJARFile(_supportedFeatures.UsesMediaLibrary, 			Constants.kMediaLibraryJARName);
			UpdateJARFile(_supportedFeatures.UsesNotificationService, 	Constants.kNotificationJARName);
			UpdateJARFile(_supportedFeatures.UsesNetworkConnectivity, 	Constants.kNetworkConnectivityJARName);
			UpdateJARFile(_supportedFeatures.UsesSharing, 				Constants.kSharingJARName);
			UpdateJARFile(_supportedFeatures.UsesTwitter, 				Constants.kSocialNetworkTwitterJARName);
			UpdateJARFile(_supportedFeatures.UsesWebView, 				Constants.kWebviewJARName);
			UpdateJARFile(_supportedAddOnServices.UsesSoomlaGrow, 		Constants.kSoomlaIntegrationJARName);		
		}

		private void UpdateJARFile (bool _usesFeature, string _JARName)
		{
			string 	_filePath	= Constants.kAndroidPluginsJARPath + "/" + _JARName;
			
			if (_usesFeature)
			{
				FileOperations.Rename(_filePath + ".jar.unused"		, _JARName + ".jar" );
				FileOperations.Rename(_filePath + ".jar.unused.meta", _JARName + ".jar.meta" );
			}
			else
			{
				FileOperations.Rename(_filePath + ".jar", _JARName + ".jar.unused" );
				FileOperations.Rename(_filePath + ".jar.meta", _JARName + ".jar.unused.meta" );
			}
		}
#endif

		#endregion


#if UNITY_EDITOR
		#region Rate NPSettings Delegate Methods

		public bool CanShowRateMyAppDialog ()
		{
			return !(EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPaused);
		}

		public void OnBeforeShowingRateMyAppDialog ()
		{}

		#endregion
#endif


		#region Editor Callback Methods

#if UNITY_EDITOR
		private void RefreshEditorGameCenter ()
		{
#if USES_GAME_SERVICES
			EditorGameCenter.Instance.Refresh();
#endif
		}

		private void ResetEditorGameCenterAchievements ()
		{
#if USES_GAME_SERVICES
			EditorGameCenter.Instance.ResetAchievements();
#endif
		}

		private void ClearPurchaseHistory ()
		{
#if USES_BILLING
			EditorStore.ClearPurchaseHistory();
#endif
		}

		private void OnPropertyModified ()
		{
			EditorPrefs.SetBool(kPrefsKeyPropertyModified, true);
		}
#endif

		#endregion
	}
}
