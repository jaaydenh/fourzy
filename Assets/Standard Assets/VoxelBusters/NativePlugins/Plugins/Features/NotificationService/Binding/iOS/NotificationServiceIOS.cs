using UnityEngine;
using System.Collections;

#if USES_NOTIFICATION_SERVICE && UNITY_IOS
using System.Collections.Generic;
using System.Runtime.InteropServices;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	public partial class NotificationServiceIOS : NotificationService
	{
		#region Native Methods

		[DllImport("__Internal")]
		private static extern void initNotificationService (bool _captureLocalNotifications, bool _captureRemoteNotifications, string _keyForUserInfo);

		[DllImport("__Internal")]
		private static extern void registerNotificationTypes (int notificationTypes);

		[DllImport("__Internal")]
		private static extern int enabledNotificationTypes ();

		[DllImport("__Internal")]
		private static extern void scheduleLocalNotification (string _payload);

		[DllImport("__Internal")]
		private static extern void cancelLocalNotification (string _notificationID);

		[DllImport("__Internal")]
		private static extern void cancelAllLocalNotifications ();

		[DllImport("__Internal")]
		private static extern void clearNotifications ();
	
		[DllImport("__Internal")]
		private static extern void registerForRemoteNotifications ();

		[DllImport("__Internal")]
		private static extern void unregisterForRemoteNotifications ();

		#endregion

		#region Initialise Methods 
		
		protected override void Initialise (NotificationServiceSettings _settings)
		{
			base.Initialise(_settings);

			// Initialise native component
			string 	_keyForUserInfo				= _settings.iOS.UserInfoKey;
			bool	_captureLocalNotifications	= true;
			bool	_captureRemoteNotifications	= true;

			if (NPSettings.Application.SupportedAddonServices.UsesOneSignal)
				_captureRemoteNotifications		= false;

			initNotificationService(_captureLocalNotifications, _captureRemoteNotifications, _keyForUserInfo);
		}
		
		#endregion

		#region Notification Methods

		public override void RegisterNotificationTypes (NotificationType _notificationTypes)
		{
			base.RegisterNotificationTypes(_notificationTypes);

			// Native call
			registerNotificationTypes((int)_notificationTypes);
		}

		public override NotificationType EnabledNotificationTypes ()
		{
			return (NotificationType)enabledNotificationTypes();
		}

		#endregion

		#region Local Notification Methods

		public override string ScheduleLocalNotification (CrossPlatformNotification _notification)
		{
			// Append notification id to user info
			string _notificationID	= _notification.GenerateNotificationID();
			string _payload			= iOSNotificationPayload.CreateNotificationPayload(_notification).ToJSON();

			// Schedule notification
			scheduleLocalNotification(_payload);

			return _notificationID;
		}
		
		public override void CancelLocalNotification (string _notificationID)
		{
			cancelLocalNotification(_notificationID);
		}
		
		public override void CancelAllLocalNotification ()
		{
			cancelAllLocalNotifications();
		}

		public override void ClearNotifications ()
		{
			clearNotifications();
		}

		#endregion
		
		#region Remote Notification Methods

#if (!USES_ONE_SIGNAL || UNITY_EDITOR)
		public override void RegisterForRemoteNotifications ()
		{		
			registerForRemoteNotifications();
		}
#endif

		public override void UnregisterForRemoteNotifications ()
		{
			base.UnregisterForRemoteNotifications();

			unregisterForRemoteNotifications();
		}

		#endregion
	}
}
#endif