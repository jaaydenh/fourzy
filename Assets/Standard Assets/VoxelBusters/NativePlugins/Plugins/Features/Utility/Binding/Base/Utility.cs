using UnityEngine;
using System.Collections;
using VoxelBusters.DebugPRO;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	/// <summary>
	/// Provides a cross-platform interface to access useful features such as RateMyApp, app's bundle information etc.
	/// </summary>
	public class Utility : MonoBehaviour 
	{
		#region Fields

		private 	RateMyApp	m_rateMyApp;

		#endregion

		#region Properties

		/// <summary>
		/// The shared instance of <see cref="RateMyApp"/> feature. (read-only)
		/// </summary>
		/// <remarks>
		/// \note Returns <c>null</c> value, if feature is marked disabled in Utility Settings.
		/// </remarks>
		public RateMyApp RateMyApp
		{
			get 
			{
				return	m_rateMyApp;
			}

			private set
			{
				m_rateMyApp	= value;
			}
		}

		#endregion

		#region Unity Methods

		protected virtual void Awake ()
		{
			if (NPSettings.Utility.RateMyApp.IsEnabled)
			{
				m_rateMyApp	= gameObject.AddComponent<RateMyApp>();
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Returns a new unique identifier.
		/// </summary>
		/// <returns>New unique identifier.</returns>
		public virtual string GetUUID ()
		{
			return System.Guid.NewGuid().ToString();
		}

		/// <summary>
		/// Opens the Store page of the specified application.
		/// </summary>
		/// <description>
		///	For iOS platform, id is the value that identifies your app on App Store. 
		/// And on Android, it will be same as app's bundle identifier (com.example.test).
		/// </description>
		/// <param name="_applicationIDList">An array of string values, that holds app id's of each supported platform.</param>
		/// <example>
		/// The following code example shows how to open store link.
		/// <code>
		/// using UnityEngine;
		/// using System.Collections;
		/// using VoxelBusters.NativePlugins;
		/// 
		/// public class ExampleClass : MonoBehaviour 
		/// {
		/// 	public void OpenStorePage ()
		/// 	{
		/// 		NPBinding.Utility.OpenStoreLink(Platform.Android("com.example.app"), 
		/// 										Platform.IOS("ios-app-id"));
		///     }
		/// }
		/// </code>
		/// </example>
		public void OpenStoreLink (params PlatformID[] _applicationIDList)
		{
			string	_applicationID	= NPUtility.GetActivePlatformID(_applicationIDList);
			
			if (string.IsNullOrEmpty(_applicationID))
			{
				Console.Log(Constants.kDebugTag, "[Utility] The operation could not be completed because application identifier is invalid.");
				return;
			}
			
			OpenStoreLink(_applicationID);
		}
		
		/// <summary>
		/// Opens the Store page of the specified application.
		/// </summary>
		/// <description>
		///	For iOS platform, id is the value that identifies your app on App Store. 
		/// And on Android, it will be same as app's bundle identifier (com.example.test).
		/// </description>
		/// <param name="_applicationID">A string that identifies an application in the current platform's Store.</param>
		public virtual void OpenStoreLink (string _applicationID)
		{}

		/// <summary>
		/// Sets the specified numeric value as the application's badge number.
		/// </summary>
		/// <param name="_badgeNumber">The numeric value to be set as badge number.</param>
		/// <remarks>
		/// \note Setting this property to 0 (zero) will hide the badge number.
		/// </remarks>
		public virtual void SetApplicationIconBadgeNumber (int _badgeNumber)
		{}

		/// <summary>
		/// Returns the string that specifies build version number of the bundle.
		/// </summary>
		/// <returns>The bundle version.</returns>
		public string GetBundleVersion ()
		{
			return VoxelBusters.Utility.PlayerSettings.GetBundleVersion();
		}

		/// <summary>
		/// Returns the string that identifies your application to the system.
		/// </summary>
		/// <returns>The bundle identifier.</returns>
		public string GetBundleIdentifier ()
		{
			return VoxelBusters.Utility.PlayerSettings.GetBundleIdentifier();
		}

		#endregion
	}
}
