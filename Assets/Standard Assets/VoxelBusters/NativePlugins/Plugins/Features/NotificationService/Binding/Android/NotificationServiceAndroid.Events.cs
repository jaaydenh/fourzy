using UnityEngine;
using System.Collections;

#if USES_NOTIFICATION_SERVICE && UNITY_ANDROID
using System.Collections.Generic;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	public partial class NotificationServiceAndroid : NotificationService
	{
		#region Constants
		
		private		const		string		 kIsLaunchNotificationKey		= "is-launch-notification";
		private		const		string		 kPayloadKey					= "notification-payload";
		
		#endregion

		#region Parse Methods

		protected override void ParseReceivedNotificationData (string _dataStr, out CrossPlatformNotification _receivedNotification, out bool _isLaunchNotification)
		{
			IDictionary		_dataDict		= JSONUtility.FromJSON(_dataStr) as IDictionary;

			_receivedNotification			= new AndroidNotificationPayload(_dataDict);
			_isLaunchNotification			= _dataDict.GetIfAvailable<bool>(kIsLaunchNotificationKey);

		}

		#endregion
	}
}
#endif