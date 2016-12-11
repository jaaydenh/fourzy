using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	public static class PlatformIDUtils
	{
		#region Static Methods

		public static string GetCurrentPlatformID (this PlatformID[] _identifiers)
		{
			if (_identifiers == null)
				return null;

			PlatformID.ePlatform _targetPlatform = 0;

#if UNITY_EDITOR
			_targetPlatform = PlatformID.ePlatform.EDITOR;
#elif UNITY_ANDROID
			_targetPlatform = PlatformID.ePlatform.ANDROID;
#elif UNITY_IOS
			_targetPlatform = PlatformID.ePlatform.IOS;
#endif

			foreach (PlatformID _currentIdentifier in _identifiers)
			{
				if (_currentIdentifier.Platform == _targetPlatform)
					return _currentIdentifier.Value;
			}
			
			return null;
		}

		#endregion
	}
}