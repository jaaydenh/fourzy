using UnityEngine;
using System.Collections;
using VoxelBusters.DebugPRO;

namespace VoxelBusters.NativePlugins.Internal
{
	public class NPUtility 
	{
		#region Methods

		public static string GetActivePlatformID (params PlatformID[] _platformIDList)
		{
			if (_platformIDList == null || _platformIDList.Length == 0)
			{
				Console.Log(Constants.kDebugTag, "[Utility] The operation could not be completed because identifier list is null.");
				return null;
			}

#if UNITY_EDITOR
			foreach (PlatformID _currentID in _platformIDList)
			{
				if (_currentID.Platform == PlatformID.ePlatform.EDITOR)
					return _currentID.Value;
			}
#else
			foreach (PlatformID _currentID in _platformIDList)
			{
#if UNITY_ANDROID
				if (_currentID.Platform == PlatformID.ePlatform.ANDROID)
					return _currentID.Value;
#elif UNITY_IOS
				if (_currentID.Platform == PlatformID.ePlatform.IOS)
					return _currentID.Value;
#endif
			}
#endif

			return null;
		}

		#endregion
	}
}