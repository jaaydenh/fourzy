using UnityEngine;
using System.Collections;

#if USES_NOTIFICATION_SERVICE && UNITY_ANDROID
using System.Collections.Generic;
using VoxelBusters.Utility;
using VoxelBusters.DebugPRO;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	public partial class NotificationServiceAndroid : NotificationService
	{
		#region Constructors
		
		NotificationServiceAndroid()
		{
			Plugin = AndroidPluginUtility.GetSingletonInstance(NativeInfo.Class.NAME);
		}
		
		#endregion

		#region Initialise
		
		protected override void Initialise (NotificationServiceSettings _settings)
		{
			base.Initialise(_settings);

			// Pass sender id list and customkeys to Native platform
			Dictionary<string, string> customKeys = GetCustomKeysForNotfication(_settings);
			
			SendConfigInfoToNative(_settings.Android.SenderIDList, customKeys, _settings.Android.NeedsBigStyle, _settings.Android.WhiteSmallIcon, _settings.Android.AllowVibration);
		}

		public override void RegisterNotificationTypes (NotificationType _notificationTypes)
		{
			base.RegisterNotificationTypes(_notificationTypes);
			
			Plugin.Call(NativeInfo.Methods.SET_NOTIFICATION_TYPES, (int)_notificationTypes);

#if USES_ONE_SIGNAL
			OneSignal.EnableSound(((int)_notificationTypes & (int)NotificationType.Sound) == 1);
#endif
		}
		
		public override NotificationType EnabledNotificationTypes ()
		{
			return (NotificationType)Plugin.Call<int>(NativeInfo.Methods.GET_ALLOWED_NOTIFICATION_TYPES);
		}


#if USES_ONE_SIGNAL
		protected override void InitialiseOneSignalService ()
		{
			base.InitialiseOneSignalService();

			// Other settings
			OneSignal.EnableVibrate(NPSettings.Notification.Android.AllowVibration);			
		}
#endif
		
		#endregion

		#region Overriden Local Notification API'S

		public override string ScheduleLocalNotification (CrossPlatformNotification _notification)
		{
			string _notificationID		= _notification.GenerateNotificationID();

			// Create meta info and pass to native
			IDictionary _payLoadInfo	= AndroidNotificationPayload.CreateNotificationPayload(_notification);

			// Scheduling notification
			Plugin.Call(NativeInfo.Methods.SCHEDULE_LOCAL_NOTIFICATION, _payLoadInfo.ToJSON());
			
			return _notificationID;
		}
		
		public override void CancelLocalNotification (string _notificationID)
		{
			Plugin.Call(NativeInfo.Methods.CANCEL_LOCAL_NOTIFICATION, _notificationID);
		}
		
		public override void CancelAllLocalNotification ()
		{
			Plugin.Call(NativeInfo.Methods.CANCEL_ALL_LOCAL_NOTIFICATIONS);
		}
		
		#endregion
		
		#region Overriden Remote Notification API's

#if (!USES_ONE_SIGNAL || UNITY_EDITOR)
		public override void RegisterForRemoteNotifications ()
		{
			Plugin.Call(NativeInfo.Methods.REGISTER_REMOTE_NOTIFICATIONS);
		}
#endif

		public override void UnregisterForRemoteNotifications ()
		{
			base.UnregisterForRemoteNotifications();

			Plugin.Call(NativeInfo.Methods.UNREGISTER_REMOTE_NOTIFICATIONS);
		}

		#endregion


		#region Overriden Common Local & Notification Notification API'S
		
		public override void ClearNotifications ()
		{
			Plugin.Call(NativeInfo.Methods.CLEAR_ALL_NOTIFICATIONS);
		}
		
		#endregion


		#region Helpers

		private Dictionary<string, string> GetCustomKeysForNotfication(NotificationServiceSettings _settings)
		{
			Dictionary<string, string> _data =  new Dictionary<string, string>();
			_data = AndroidNotificationPayload.GetNotificationKeyMap();			

			return _data;
			
		}

		protected void SendConfigInfoToNative(string[] _senderIDs, Dictionary<string,string> _customKeysInfo, bool _needsBigStyle, Texture2D _whiteSmallNotificationIcon, bool _allowVibration)
		{
			if (_senderIDs.Length == 0)
			{
				Console.LogError(Constants.kDebugTag, "Add senderid list for notifications to work");
			}

			List<string> list =  new List<string>(_senderIDs);	
			bool _usesExternalRemoteNotificationService = NPSettings.Application.SupportedAddonServices.UsesOneSignal;

			//Pass this to android
			Plugin.Call(NativeInfo.Methods.INITIALIZE,list.ToJSON(),_customKeysInfo.ToJSON(), _whiteSmallNotificationIcon == null ? false : true, _allowVibration, _usesExternalRemoteNotificationService);
		}
		
		#endregion
	}
}
#endif