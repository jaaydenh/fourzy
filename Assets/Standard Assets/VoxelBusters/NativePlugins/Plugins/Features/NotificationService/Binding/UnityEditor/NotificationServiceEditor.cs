using UnityEngine;
using System.Collections;

#if USES_NOTIFICATION_SERVICE && UNITY_EDITOR
namespace VoxelBusters.NativePlugins
{
	using Internal;

	public partial class NotificationServiceEditor : NotificationService 
	{
		#region Properties

		private EditorNotificationCenter		m_notificationCenter;

		#endregion

		#region Initialise Methods

		protected override void Initialise (NotificationServiceSettings _settings)
		{		
			base.Initialise(_settings);

			// Cache reference
			m_notificationCenter	= EditorNotificationCenter.Instance;

			// Initialise notificaton center
			m_notificationCenter.Initialise();
		}

		#endregion

		#region Notification Methods

		public override void RegisterNotificationTypes (NotificationType _notificationTypes)
		{
			base.RegisterNotificationTypes(_notificationTypes);

			// Registering
			m_notificationCenter.RegisterNotificationTypes(_notificationTypes);
		}

		public override NotificationType EnabledNotificationTypes ()
		{
			return m_notificationCenter.EnabledNotificationTypes();
		}

		#endregion

		#region Local Notification Methods

		public override string ScheduleLocalNotification (CrossPlatformNotification _notification)
		{
			// Append notification id to userinfo
			string _notificationID	= _notification.GenerateNotificationID();

			// Scheduling new notification
			m_notificationCenter.ScheduleLocalNotification(_notification);

			return _notificationID;
		}

		public override void CancelLocalNotification (string _notificationID)
		{
			IList _scheduledNotifications	= m_notificationCenter.ScheduledLocalNotifications;
			int _scheduledNotificationCount	= _scheduledNotifications.Count;

			for (int _iter = 0; _iter < _scheduledNotificationCount; _iter++)
			{
				CrossPlatformNotification _scheduledNotification	= _scheduledNotifications[_iter] as CrossPlatformNotification;
				string _scheduledNotificationID						= _scheduledNotification.GetNotificationID();

				// Cancel the notification which matches the given id
				if (!string.IsNullOrEmpty(_scheduledNotificationID) && _scheduledNotificationID.Equals(_notificationID))
				{
					m_notificationCenter.CancelLocalNotification(_scheduledNotification);
					break;
				}
			}
		}

		public override void CancelAllLocalNotification ()
		{
			m_notificationCenter.CancelAllLocalNotifications();
		}
			
		public override void ClearNotifications ()
		{
			m_notificationCenter.ClearNotifications();
		}
		
		#endregion

		#region Remote Notification Methods

		public override void RegisterForRemoteNotifications ()
		{
			m_notificationCenter.RegisterForRemoteNotifications();
		}

		public override void UnregisterForRemoteNotifications ()
		{
			base.UnregisterForRemoteNotifications();

			m_notificationCenter.UnregisterForRemoteNotifications();
		}

		#endregion
	}
}
#endif