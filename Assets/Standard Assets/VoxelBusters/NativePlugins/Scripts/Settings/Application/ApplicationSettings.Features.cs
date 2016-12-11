using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	public partial class ApplicationSettings 
	{
		[System.Serializable]
		public partial class Features
		{
			#region Fields

			[SerializeField]
			[NotifyNPSettingsOnValueChange]
			[Tooltip("If enabled, Address Book feature will be active within your application.")]
			private		bool		m_usesAddressBook 	= true;

#if !NATIVE_PLUGINS_LITE_VERSION
			[SerializeField]
			[NotifyNPSettingsOnValueChange]
			[Tooltip("If enabled, Billing feature will be active within your application.")]
			private		bool		m_usesBilling 		= true;

			[NotifyNPSettingsOnValueChange]
			[SerializeField]
			[Tooltip("If enabled, Cloud Services feature will be active within your application.")]
			private		bool		m_usesCloudServices = true;

			[SerializeField]
			[NotifyNPSettingsOnValueChange]
			[Tooltip("If enabled, Game Services feature will be active within your application.")]
			private		bool		m_usesGameServices 	= true;

			[SerializeField]
			[NotifyNPSettingsOnValueChange]
			[Tooltip("If enabled, Media Library feature will be active within your application.")]
			private		bool		m_usesMediaLibrary 	= true;
#endif
			[SerializeField]
			[NotifyNPSettingsOnValueChange]
			[Tooltip("If enabled, Network Connectivity feature will be active within your application.")]
			private		bool		m_usesNetworkConnectivity = true;

#if !NATIVE_PLUGINS_LITE_VERSION
			[SerializeField]
			[NotifyNPSettingsOnValueChange]
			[Tooltip("If enabled, Notification Service feature will be active within your application.")]
			private		bool		m_usesNotificationService = true;
#endif
			[SerializeField]
			[NotifyNPSettingsOnValueChange]
			[Tooltip("If enabled, Sharing feature will be active within your application.")]
			private		bool		m_usesSharing 		= true;

#if !NATIVE_PLUGINS_LITE_VERSION
			[SerializeField]
			[NotifyNPSettingsOnValueChange]
			[Tooltip("If enabled, Twitter feature will be active within your application.")]
			private		bool		m_usesTwitter 		= true;

			[SerializeField]
			[NotifyNPSettingsOnValueChange]
			[Tooltip("If enabled, WebView feature will be active within your application.")]
			private		bool		m_usesWebView 		= true;
#endif

			#endregion

			#region Properties

			public bool UsesAddressBook
			{
				get
				{
					return m_usesAddressBook;
				}
			}

			public bool UsesBilling
			{
				get
				{
#if !NATIVE_PLUGINS_LITE_VERSION
					return m_usesBilling;
#else
					return false;
#endif
				}
			}

			public bool UsesCloudServices
			{
				get
				{
#if !NATIVE_PLUGINS_LITE_VERSION
					return m_usesCloudServices;
#else
					return false;
#endif
				}
			}
			
			public bool UsesGameServices
			{
				get
				{
#if !NATIVE_PLUGINS_LITE_VERSION
					return m_usesGameServices;
#else
					return false;
#endif
				}
			}
			
			public bool UsesMediaLibrary
			{
				get
				{
#if !NATIVE_PLUGINS_LITE_VERSION
					return m_usesMediaLibrary;
#else
					return false;
#endif
				}
			}

			public bool UsesNetworkConnectivity
			{
				get
				{
					return m_usesNetworkConnectivity;
				}
			}
			
			public bool UsesNotificationService
			{
				get
				{
#if !NATIVE_PLUGINS_LITE_VERSION
					return m_usesNotificationService;
#else
					return false;
#endif
				}
			}

			public bool UsesSharing
			{
				get
				{
					return m_usesSharing;
				}
			}

			public bool UsesTwitter
			{
				get
				{
#if !NATIVE_PLUGINS_LITE_VERSION
					return m_usesTwitter;
#else
					return false;
#endif
				}
			}

			public bool UsesWebView
			{
				get
				{
#if !NATIVE_PLUGINS_LITE_VERSION
					return m_usesWebView;
#else
					return false;
#endif
				}
			}
			
			#endregion
		}
	}
}