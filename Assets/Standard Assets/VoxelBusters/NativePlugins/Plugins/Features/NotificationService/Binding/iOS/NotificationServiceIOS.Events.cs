using UnityEngine;
using System.Collections;

#if USES_NOTIFICATION_SERVICE && UNITY_IOS
using System.Collections.Generic;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	public partial class NotificationServiceIOS : NotificationService
	{
		#region Constants

		private		const		string		 kIsLaunchNotificationKey		= "is-launch-notification";
		private		const		string		 kPayloadKey					= "payload";

		#endregion

		#region Parse Methods

		protected override void ParseReceivedNotificationData (string _dataStr, out CrossPlatformNotification _receivedNotification, out bool _isLaunchNotification)
		{
			IDictionary		_dataDict		= JSONUtility.FromJSON(_dataStr) as IDictionary;

			// Extract properties
			IDictionary		_payloadDict	= _dataDict.GetIfAvailable<IDictionary>(kPayloadKey);
			_receivedNotification			= new iOSNotificationPayload(_payloadDict);
			_isLaunchNotification			= _dataDict.GetIfAvailable<bool>(kIsLaunchNotificationKey);

			if (_receivedNotification.UserInfo != null)
			{
#if USES_ONE_SIGNAL
				// Our system should ignore raising events sent from One Signal
				const string 	_kOneSignalIdentifierKeyPath	= "custom/i";

				if (_receivedNotification.UserInfo.ContainsKeyPath(_kOneSignalIdentifierKeyPath))
				{
					_receivedNotification	= null;

					return;
				}
#endif
			}
		}

		#endregion
	}
}
#endif