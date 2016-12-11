using UnityEngine;
using System.Collections;

#if USES_NOTIFICATION_SERVICE 
using System.Collections.Generic;

namespace VoxelBusters.NativePlugins
{
	/// <summary>
	/// Provides cross-platform interface for scheduling, registering and handling notifications.
	/// </summary>
	public partial class NotificationService : MonoBehaviour 
	{
		#region properties

		private		bool		m_canSendEvents		= false;
#if (USES_ONE_SIGNAL && !UNITY_EDITOR)
		private		bool		m_registeredForOneSignalPushNotifications	= false;
#endif

		#endregion

		#region Unity Methods

		private void Awake ()
		{
			// Initialise Notification feature
			if (NPSettings.Application.SupportedFeatures.UsesNotificationService)
			{
				Initialise(NPSettings.Notification);

#if (USES_ONE_SIGNAL && !UNITY_EDITOR)
				InitialiseOneSignalService();
#endif
			}
		}

		private IEnumerator Start ()
		{
			yield return new WaitForEndOfFrame();

			// By now, all the components would be ready to receive events
			m_canSendEvents	= true;
		}

		#endregion

		#region Initialise Methods

		protected virtual void Initialise (NotificationServiceSettings _settings)
		{}

#if USES_ONE_SIGNAL
		protected virtual void InitialiseOneSignalService()
		{
			// Initialise One Signal service
			OneSignalServiceSettings	_oneSignalSettings	= NPSettings.AddonServicesSettings.OneSignalService;
			
			OneSignal.Init(_oneSignalSettings.AppID, 
			               _oneSignalSettings.GoogleProjectNumber, 
			               DidReceiveOneSignalNotification,
			               false);

		}
#endif

		#endregion

		#region Notification Methods

		/// <summary>
		/// Registers your preferred options for notifying the user.
		/// </summary>
		/// <description>
		/// If your app displays alerts, play sounds, or badges its icon, you must call this method soon after the app launch to request permission to alert the user in these ways.
		/// </description>
		/// <param name="_notificationTypes">The notification types that your app uses to alert user.</param>
		/// <remarks>
		/// \note It is recommended that you call this method before you schedule any local notifications or register with the push notification service.
		/// Multiple types can be combined together using the <c>|</c> operator.
		/// </remarks>
		public virtual void RegisterNotificationTypes (NotificationType _notificationTypes)
		{}

		/// <summary>
		/// Returns the types of notifications currently enabled for the app.
		/// </summary>
		/// <description>
		/// The values in the returned bit mask are first set when the app calls <see cref="RegisterNotificationTypes"/> method. Thereafter the user may modify these notification types from system preference.
		/// </description>
		/// <returns>The bit mask indicate the types of notifications currently enabled for the app.</returns>
		public virtual NotificationType EnabledNotificationTypes ()
		{
			return (NotificationType)0;
		}

		#endregion

		#region Local Notification Methods

		/// <summary>
		/// Schedules a local notification for delivery at specified date and time.
		/// </summary>
		/// <returns>A string used to uniquely identify the scheduled notification.</returns>
		/// <param name="_notification">The local notification object that you want to schedule.</param>
		/// <remarks>
		/// \note Prior to scheduling any local notifications, you must call the <see cref="RegisterNotificationTypes"/> method if you want system to display alerts, play sounds etc.
		/// </remarks>
		public virtual string ScheduleLocalNotification (CrossPlatformNotification _notification)
		{
			return null;
		}

		/// <summary>
		/// Cancels the delivery of the specified scheduled local notification.
		/// </summary>
		/// <param name="_notificationID">A string used to uniquely identify the notification.</param>
		public virtual void CancelLocalNotification (string _notificationID)
		{}

		/// <summary>
		/// Cancels the delivery of all scheduled local notifications.
		/// </summary>
		public virtual void CancelAllLocalNotification ()
		{}
			
		/// <summary>
		/// Discards all the received notifications.
		/// </summary>
		public virtual void ClearNotifications ()
		{}
		
		#endregion

		#region Remote Notification Methods

		/// <summary>
		/// Registers to receive remote notifications via Push Notification service.
		/// </summary>
		/// <description>
		/// Call this method to initiate the registration process with Push Notification service. 
		/// When registration process completes, <see cref="DidFinishRegisterForRemoteNotificationEvent"/> is fired.
		/// If registration succeeds, then you should pass device token to the server you use to generate remote notifications.
		/// </description>
		/// <remarks>
		/// \note If you want your app’s remote notifications to display alerts, play sounds etc you must call the <see cref="RegisterNotificationTypes"/> method before registering for remote notifications.
		/// </remarks>
#if (!USES_ONE_SIGNAL || UNITY_EDITOR)
		public virtual void RegisterForRemoteNotifications ()
		{}
#else
		public void RegisterForRemoteNotifications ()
		{
			if (!m_registeredForOneSignalPushNotifications)
			{
				m_registeredForOneSignalPushNotifications	= true;

				OneSignal.RegisterForPushNotifications();
				OneSignal.SetSubscription(true);
			}
		}
#endif

		/// <summary>
		/// Unregister for all remote notifications received via Push Notification service.
		/// </summary>
		/// <remarks>
		/// \note Apps unregistered through this method can always re-register.
		/// </remarks>
		public virtual void UnregisterForRemoteNotifications ()
		{
#if (USES_ONE_SIGNAL && !UNITY_EDITOR)
			if (m_registeredForOneSignalPushNotifications)
			{
				m_registeredForOneSignalPushNotifications	= false;

				OneSignal.SetSubscription(false);
			}
#endif
		}

		#endregion
	}
}
#endif