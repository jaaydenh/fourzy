#if USES_NOTIFICATION_SERVICE
using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins.Internal
{
	public class NotificationServiceUnsupported : INotificationServicePlatform 
	{
		#region Public Methods

		public void Initialise (NotificationServiceSettings _settings)
		{
#if NP_DEBUG
			Debug.LogWarning(Constants.kNotSupported);
#endif
		}

		public void RegisterNotificationTypes (NotificationType _notificationTypes)
		{
#if NP_DEBUG
			Debug.LogWarning(Constants.kNotSupported);
#endif
		}

		public NotificationType EnabledNotificationTypes ()
		{
#if NP_DEBUG
			Debug.LogWarning(Constants.kNotSupported);
#endif
			return (NotificationType)0;
		}

		public void ScheduleLocalNotification (CrossPlatformNotification _notification)
		{
#if NP_DEBUG
			Debug.LogWarning(Constants.kNotSupported);
#endif
		}
			
		public void CancelLocalNotification (string _notificationID)
		{
#if NP_DEBUG
			Debug.LogWarning(Constants.kNotSupported);
#endif
		}

		public void CancelAllLocalNotification ()
		{
#if NP_DEBUG
			Debug.LogWarning(Constants.kNotSupported);
#endif
		}

		public void ClearNotifications ()
		{
#if NP_DEBUG
			Debug.LogWarning(Constants.kNotSupported);
#endif
		}

		public void RegisterForRemoteNotifications ()
		{
#if NP_DEBUG
			Debug.LogWarning(Constants.kNotSupported);
#endif
		}

		public void UnregisterForRemoteNotifications ()
		{
#if NP_DEBUG
			Debug.LogWarning(Constants.kNotSupported);
#endif
		}

		public void ParseReceivedNotificationEventData (IDictionary _JSONDict, out CrossPlatformNotification _receivedNotification, out bool _isLaunchNotification)
		{
			_receivedNotification	= null;
			_isLaunchNotification	= false;
		}

		#endregion
	}
}
#endif