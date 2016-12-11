using UnityEngine;
using System.Collections;

#if USES_NOTIFICATION_SERVICE && USES_ONE_SIGNAL
using System.Collections.Generic;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins.Internal
{
	public sealed class OneSignalNotificationPayload : CrossPlatformNotification 
	{
		#region Constant

		private 	const 	string 		kTitleKey				= "title";
		private 	const 	string 		kActionButtonsKey		= "actionButtons";
		private 	const 	string 		kSoundKey				= "sound";

		#endregion

		#region Constructor

		public OneSignalNotificationPayload (string _message, Dictionary<string, object> _additionalData)
		{
			// Set properties
			this.AlertBody			= _message;
			this.iOSProperties		= new iOSSpecificProperties();
			this.AndroidProperties	= new AndroidSpecificProperties();
			this.SoundName			= _additionalData.GetIfAvailable<string>(kSoundKey);

		
			// Extract other properties from data dictionary
			if (_additionalData == null)
				return;

			// iOS specific properties
			iOSSpecificProperties		_iOSProperties		= this.iOSProperties;

			_iOSProperties.HasAction		= _additionalData.ContainsKey(kActionButtonsKey);

			// Android specific properties
			AndroidSpecificProperties	_androidProperties	= this.AndroidProperties;

			_androidProperties.ContentTitle	= _additionalData.GetIfAvailable<string>(kTitleKey);

			// Get user info dictionary by removing used property keys
			IDictionary		_userInfoDict	= new Dictionary<string, object>(_additionalData);

			_userInfoDict.Remove(kTitleKey);

			this.UserInfo	= _userInfoDict;
		}

		#endregion
	}
}
#endif