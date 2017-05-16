#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VoxelBusters.NativePlugins.Internal
{
	public class UtilityUnsupported : IUtilityPlatform 
	{
		#region Public Methods

		public void OpenStoreLink (string _applicationID)
		{
#if UNITY_EDITOR
#if NP_DEBUG
			Debug.Log("[Utility] Opening store, ApplicationID=" + _applicationID);
#endif

			if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
			{
				Application.OpenURL("https://play.google.com/store/apps/details?id=" + _applicationID);	
			}
#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7 
			else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iPhone)
#else
			else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
#endif
			{
				Application.OpenURL("https://itunes.apple.com/app/id" + _applicationID);
			}
#else
			Debug.LogWarning(Constants.kNotSupported);
#endif
		}
		
		public void SetApplicationIconBadgeNumber (int _badgeNumber)
		{
#if NP_DEBUG
			Debug.LogWarning(Constants.kiOSFeature);
#endif
		}

		#endregion
	}
}
#endif