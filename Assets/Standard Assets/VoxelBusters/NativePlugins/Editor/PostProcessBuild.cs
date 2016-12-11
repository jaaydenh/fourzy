#if UNITY_EDITOR && !(UNITY_WINRT || UNITY_WEBPLAYER || UNITY_WEBGL)
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using VoxelBusters.Utility;
using VoxelBusters.ThirdParty.XUPorter;
using PlayerSettings = UnityEditor.PlayerSettings;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	public class PostProcessBuild
	{
		#region Constants

		// File folders
		private const string	kRelativePathNativePluginsFolder		= Constants.kPluginAssetsPath;
		private	const string	kRelativePathIOSNativeCodeFolder		= kRelativePathNativePluginsFolder + "/Plugins/NativeIOSCode";
		private const string	kRelativePathXcodeModDataCollectionFile	= kRelativePathNativePluginsFolder + "/XCodeModData.txt";
		private const string 	kRelativePathInfoPlistFile				= "Info.plist";
		private const string 	kRelativePathInfoPlistBackupFile		= "Info.backup.plist";
		private	const string	kRelativePathNativePluginsSDKFolder		= "NativePlugins";

		// Mod keys
		private	const string	kModKeyAddressBook						= "NativePlugins-AddressBook";
		private	const string	kModKeyBilling							= "NativePlugins-Billing";
		private	const string	kModKeyCloudServices					= "NativePlugins-CloudServices";
		private	const string	kModKeyCommon							= "NativePlugins-Common";
		private	const string	kModKeyGameServices						= "NativePlugins-GameServices";
		private	const string	kModKeyMediaLibrary						= "NativePlugins-MediaLibrary";
		private	const string	kModKeyNetworkConnectivity				= "NativePlugins-NetworkConnectivity";
		private	const string	kModKeyNotification						= "NativePlugins-Notification";
		private	const string	kModKeySharing							= "NativePlugins-Sharing";
		private	const string	kModKeyTwitter							= "NativePlugins-Twitter";
		private	const string	kModKeyTwitterSDK						= "NativePlugins-TwitterSDK";
		private	const string	kModKeyWebView							= "NativePlugins-WebView";
		private	const string	kModKeySoomlaGrow						= "NativePlugins-SoomlaGrow";

		// PlayerPrefs keys
		private	const string	kTwitterConfigKey						= "twitter-config";
	
		// Fabric data
		private const string 	kFabricKitJsonStringFormat				= "{{\"Fabric\":{{\"APIKey\":\"{0}\",\"Kits\":[{{\"KitInfo\":{{\"consumerKey\":\"\",\"consumerSecret\":\"\"}},\"KitName\":\"Twitter\"}}]}}}}";
		
		// Pch file modification
		private const string 	kPrecompiledFileRelativeDirectoryPath	= "Classes/";
		private const string 	kPrecompiledHeaderExtensionPattern		= "*.pch";
		private const string	kPrecompiledHeaderRegexPattern			= @"#ifdef __OBJC__(\n?\t?[#|//](.)*)*";
		private const string	kPrecompiledHeaderEndIfTag				= "#endif";
		private const string	kPrecompiledHeaderInsertText			= "#import \"Defines.h\"";

		#endregion

		#region Static Fields

		private static Plist	infoPlist								= null;

		#endregion

		#region Methods

		[PostProcessBuild(0)]
		public static void OnPostProcessBuildActionStart (BuildTarget _target, string _buildPath) 
		{			
			string 	_targetStr	= _target.ToString();
			
			if (_targetStr.Equals("iOS") || _targetStr.Equals("iPhone"))
			{
				iOSPostProcessBuild(_target, _buildPath);
				return;
			}
		}
		
		[PostProcessBuild(1000)]
		public static void OnPostProcessBuildActionFinish (BuildTarget _target, string _buildPath) 
		{
			string 	_targetStr	= _target.ToString();
			
			if (_targetStr.Equals("iOS") || _targetStr.Equals("iPhone"))
			{
				CleanupProject();
				return;
			}
		}

		private static void iOSPostProcessBuild (BuildTarget _target, string _buildPath) 
		{
			// Load plist info
			string 	_infoPlistFilePath	= GetInfoPlistFilePath(_buildPath);
			infoPlist					= Plist.LoadPlistAtPath(_infoPlistFilePath);

			// Removing old automation related files
			CleanupProject();

			// Post process actions
			ProcessFeatureSpecificOperations();
			GenerateXcodeModFiles();
			ModifyInfoPlist(_buildPath);
			ModifyPchFile(_buildPath);
		}

		private static void CleanupProject ()
		{
			foreach (string _filePath in Directory.GetFiles(kRelativePathIOSNativeCodeFolder, "*.xcodemods", SearchOption.AllDirectories))
			{
				// Delete file
				File.SetAttributes(_filePath, FileAttributes.Normal);
				File.Delete(_filePath);
				
				// Delete meta file
				string	_metaFilePath	= _filePath + ".meta";
				if (File.Exists(_metaFilePath))
				{
					File.SetAttributes(_metaFilePath, FileAttributes.Normal);
					File.Delete(_metaFilePath);
				}
			}
		}

		private static void ProcessFeatureSpecificOperations ()
		{
			// Remove NP temp folder
			if (Directory.Exists(kRelativePathNativePluginsSDKFolder))
			{
				IOExtensions.AssignPermissionRecursively(kRelativePathNativePluginsSDKFolder, FileAttributes.Normal);
				Directory.Delete(kRelativePathNativePluginsSDKFolder, true);
			}

			// Create temp folder to place extracted files
			Directory.CreateDirectory(kRelativePathNativePluginsSDKFolder);

			// Add player settings info to receipt verification code
			ApplicationSettings.Features	_supportedFeatures	= NPSettings.Application.SupportedFeatures;

			if (_supportedFeatures.UsesBilling)
				AddBuildInfoToReceiptVerificationManger();

			// Decompress zip files and add it to project
			if (_supportedFeatures.UsesTwitter)
				DecompressTwitterSDKFiles();
		}
		
		private static void AddBuildInfoToReceiptVerificationManger ()
		{
			string		_rvFilePath			= Path.Combine(kRelativePathIOSNativeCodeFolder, "Billing/Source/ReceiptVerification/Manager/ReceiptVerificationManager.m");
			string[]	_contents			= File.ReadAllLines(_rvFilePath);
			int			_lineCount			= _contents.Length;
			
			for (int _iter = 0; _iter < _lineCount; _iter++)
			{
				string	_curLine			= _contents[_iter];
				
				if (!_curLine.StartsWith("const"))
					continue;
				
				if (_curLine.Contains("bundleIdentifier"))
				{
					const string _kBundleVersionKey		= "CFBundleVersion";

					_contents[_iter]		= string.Format("const NSString *bundleIdentifier\t= @\"{0}\";", PlayerSettings.bundleIdentifier);
					_contents[_iter + 1]	= string.Format("const NSString *bundleVersion\t\t= @\"{0}\";", infoPlist[_kBundleVersionKey]);
					break;
				}
			}
			
			// Now rewrite updated contents
			File.WriteAllLines(_rvFilePath, _contents);
		}
		
		private static void DecompressTwitterSDKFiles ()
		{
			string		_projectPath					= AssetsUtility.GetProjectPath();
			string		_twitterNativeCodeFolderPath	= Path.Combine(_projectPath, kRelativePathIOSNativeCodeFolder + "/Twitter");

			if (!Directory.Exists(_twitterNativeCodeFolderPath)) 
				return;

			foreach (string _filePath in Directory.GetFiles(_twitterNativeCodeFolderPath, "*.gz", SearchOption.AllDirectories))
				Zip.DecompressToDirectory(_filePath, kRelativePathNativePluginsSDKFolder);
		}

		private static void	GenerateXcodeModFiles ()
		{
			string		_xcodeModDataCollectionText	= File.ReadAllText(kRelativePathXcodeModDataCollectionFile);

			if (_xcodeModDataCollectionText == null)
				return;

			IDictionary							_xcodeModDataCollectionDict	= JSONUtility.FromJSON(_xcodeModDataCollectionText) as IDictionary;
			ApplicationSettings.Features		_supportedFeatures			= NPSettings.Application.SupportedFeatures;
			ApplicationSettings.AddonServices	_supportedAddonServices		= NPSettings.Application.SupportedAddonServices;

			// Create mod file related to supported features
			ExtractAndSerializeXcodeModInfo(_xcodeModDataCollectionDict,		kModKeyCommon, 			kRelativePathIOSNativeCodeFolder);

			if (_supportedFeatures.UsesAddressBook)
				ExtractAndSerializeXcodeModInfo(_xcodeModDataCollectionDict,	kModKeyAddressBook,		kRelativePathIOSNativeCodeFolder);

			if (_supportedFeatures.UsesBilling)
				ExtractAndSerializeXcodeModInfo(_xcodeModDataCollectionDict,	kModKeyBilling, 		kRelativePathIOSNativeCodeFolder);

			if (_supportedFeatures.UsesCloudServices)
				ExtractAndSerializeXcodeModInfo(_xcodeModDataCollectionDict,	kModKeyCloudServices, 	kRelativePathIOSNativeCodeFolder);

			if (_supportedFeatures.UsesGameServices)
				ExtractAndSerializeXcodeModInfo(_xcodeModDataCollectionDict,	kModKeyGameServices, 	kRelativePathIOSNativeCodeFolder);
			
			if (_supportedFeatures.UsesMediaLibrary)
				ExtractAndSerializeXcodeModInfo(_xcodeModDataCollectionDict,	kModKeyMediaLibrary, 	kRelativePathIOSNativeCodeFolder);
			
			if (_supportedFeatures.UsesNetworkConnectivity)
				ExtractAndSerializeXcodeModInfo(_xcodeModDataCollectionDict,	kModKeyNetworkConnectivity, kRelativePathIOSNativeCodeFolder);

			if (_supportedFeatures.UsesNotificationService)
				ExtractAndSerializeXcodeModInfo(_xcodeModDataCollectionDict,	kModKeyNotification, 	kRelativePathIOSNativeCodeFolder);
			
			if (_supportedFeatures.UsesSharing)
				ExtractAndSerializeXcodeModInfo(_xcodeModDataCollectionDict,	kModKeySharing, 		kRelativePathIOSNativeCodeFolder);

			if (_supportedFeatures.UsesTwitter)
			{
				ExtractAndSerializeXcodeModInfo(_xcodeModDataCollectionDict,	kModKeyTwitter, 		kRelativePathIOSNativeCodeFolder);
				ExtractAndSerializeXcodeModInfo(_xcodeModDataCollectionDict,	kModKeyTwitterSDK,		kRelativePathNativePluginsSDKFolder);
			}

			if (_supportedFeatures.UsesWebView)
				ExtractAndSerializeXcodeModInfo(_xcodeModDataCollectionDict,	kModKeyWebView, 		kRelativePathIOSNativeCodeFolder);

			// Create mod file related to supported addon features
			if (_supportedAddonServices.UsesSoomlaGrow)
				ExtractAndSerializeXcodeModInfo(_xcodeModDataCollectionDict, 	kModKeySoomlaGrow,		kRelativePathIOSNativeCodeFolder);
		}

		private static void ExtractAndSerializeXcodeModInfo (IDictionary _modCollectionDict, string _modKey, string _folderRelativePath)
		{
			IDictionary		_modInfoDict	= (IDictionary)_modCollectionDict[_modKey];
			string			_newModFileName	= _modKey + ".xcodemods";

			File.WriteAllText(Path.Combine(_folderRelativePath, _newModFileName), _modInfoDict.ToJSON());
		}

//		{
//			"Fabric": {
//				"APIKey": "{0}",
//				"Kits": [
//				    {
//					"KitInfo": {
//						"consumerKey": "",
//						"consumerSecret": ""
//					},
//					"KitName": "Twitter"
//				    }
//				    ]
//			}
//		}

		private static void ModifyInfoPlist (string _buildPath)
		{
			ApplicationSettings.Features 	_supportedFeatures			= NPSettings.Application.SupportedFeatures;
			Dictionary<string, object> 		_newPermissionsDict			= new Dictionary<string, object>();

			// Add twitter info 
#if USES_TWITTER
			{
				const string 	_kFabricKitRootKey 		= "Fabric";
				TwitterSettings _twitterSettings		= NPSettings.SocialNetworkSettings.TwitterSettings;
				string 			_fabricJsonStr			= string.Format(kFabricKitJsonStringFormat, _twitterSettings.ConsumerKey);
				IDictionary 	_fabricJsonDictionary	= (IDictionary)JSONUtility.FromJSON(_fabricJsonStr);

				// Add fabric
				_newPermissionsDict[_kFabricKitRootKey]	= _fabricJsonDictionary[_kFabricKitRootKey];
			}
#endif

			// Add background process details
#if USES_NOTIFICATION_SERVICE && USES_ONE_SIGNAL
			const string		_kUIBackgroundModesKey	= "UIBackgroundModes";
			IList				_backgroundModesList	= (IList)infoPlist.GetKeyPathValue(_kUIBackgroundModesKey);

			if (_backgroundModesList == null)
				_backgroundModesList					= new List<string>();

			const string		_kRemoteNotificationProcess	= "remote-notification";
			
			if (!_backgroundModesList.Contains(_kRemoteNotificationProcess))
				_backgroundModesList.Add(_kRemoteNotificationProcess);
			
			_newPermissionsDict[_kUIBackgroundModesKey]	= _backgroundModesList;
#endif

			// Device capablities addition
			if (_supportedFeatures.UsesGameServices)
			{
				const string	_kDeviceCapablitiesKey	= "UIRequiredDeviceCapabilities";
				const string	_kGameKitKey			= "gamekit";
				IList			_deviceCapablitiesList	= (IList)infoPlist.GetKeyPathValue(_kDeviceCapablitiesKey);

				if (_deviceCapablitiesList == null)
					_deviceCapablitiesList				= new List<string>();

				if (!_deviceCapablitiesList.Contains(_kGameKitKey))
					_deviceCapablitiesList.Add(_kGameKitKey);

				_newPermissionsDict[_kDeviceCapablitiesKey]	= _deviceCapablitiesList;
			}

			// Query scheme related key inclusion
			if (_supportedFeatures.UsesSharing)
			{
				const string	_kQuerySchemesKey		= "LSApplicationQueriesSchemes";
				const string	_kWhatsAppKey			= "whatsapp";
				IList			_queriesSchemesList		= (IList)infoPlist.GetKeyPathValue(_kQuerySchemesKey);

				if (_queriesSchemesList == null)
					_queriesSchemesList					= new List<string>();

				if (!_queriesSchemesList.Contains(_kWhatsAppKey))
					_queriesSchemesList.Add(_kWhatsAppKey);

				_newPermissionsDict[_kQuerySchemesKey]	= _queriesSchemesList;
			}

			// Apple transport security
			if (_supportedFeatures.UsesNetworkConnectivity || _supportedFeatures.UsesWebView)
			{
				const string	_kATSKey				= "NSAppTransportSecurity";
				const string	_karbitraryLoadsKey		= "NSAllowsArbitraryLoads";
				IDictionary		_transportSecurityDict	= (IDictionary)infoPlist.GetKeyPathValue(_kATSKey);

				if (_transportSecurityDict == null)
					_transportSecurityDict				= new Dictionary<string, object>();

				_transportSecurityDict[_karbitraryLoadsKey]	= true;
				_newPermissionsDict[_kATSKey]			= _transportSecurityDict;
			}

			if (_newPermissionsDict.Count == 0)
				return;

			// First create a backup of old data
			string			_infoPlistBackupSavePath	= GetInfoPlistBackupFilePath(_buildPath);

			infoPlist.Save(_infoPlistBackupSavePath);

			// Now add new permissions
			foreach (string _key in _newPermissionsDict.Keys)
				infoPlist.AddValue(_key, _newPermissionsDict[_key]);

			// Save these changes
			string			_infoPlistSavePath			= GetInfoPlistFilePath(_buildPath);

			infoPlist.Save(_infoPlistSavePath);
		}

		private static void ModifyPchFile (string _buildPath)
		{
			string 		_pchFileDirectory	= Path.Combine(_buildPath, kPrecompiledFileRelativeDirectoryPath);
			string[] 	_pchFiles 			= Directory.GetFiles(_pchFileDirectory, kPrecompiledHeaderExtensionPattern);
			string 		_pchFilePath 		= null;

			// There will be only one file per project if it exists.
			if (_pchFiles.Length > 0)
				_pchFilePath =  _pchFiles[0];

			if (File.Exists(_pchFilePath))
			{
				string 	_fileContents 		= File.ReadAllText(_pchFilePath);

				// Make sure content doesnt exist
				if (_fileContents.Contains(kPrecompiledHeaderInsertText))
					return;

				Regex 	_regex				= new Regex(kPrecompiledHeaderRegexPattern);
				Match 	_match 				= _regex.Match(_fileContents);
				int		_endOfPatternIndex	= _match.Groups[0].Index + _match.Groups[0].Length;

				// We should append text within end tag
				if (_match.Value.Contains(kPrecompiledHeaderEndIfTag))
					_endOfPatternIndex	-= kPrecompiledHeaderEndIfTag.Length;

				string 	_updatedContents	= _fileContents.Insert(_endOfPatternIndex, "\t" + kPrecompiledHeaderInsertText + "\n");

				// Write updated text
				File.WriteAllText(_pchFilePath, _updatedContents);
			}
		}

		private static string GetInfoPlistFilePath (string _buildPath)
		{
			return Path.Combine(_buildPath, kRelativePathInfoPlistFile);
		}

		private static string GetInfoPlistBackupFilePath (string _buildPath)
		{
			return Path.Combine(_buildPath, kRelativePathInfoPlistBackupFile);
		}
		
		#endregion
	}
}
#endif