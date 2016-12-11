using UnityEngine;
using System.Collections;

#if UNITY_ANDROID
namespace VoxelBusters.NativePlugins
{
	public partial class UtilityAndroid : Utility
	{
		#region Platform Native Info
		
		private class Native
		{
			// Handler class name
			internal class Class
			{
				internal const string NAME								= "com.voxelbusters.nativeplugins.features.utility.UtilityHandler";
			}
			
			// For holding method names
			internal class Methods
			{			
				internal const string SET_APPLICATION_ICON_BADGE_NUMBER	= "setApplicationIconBadgeNumber";
			}
		}
		
		#endregion
		
		#region  Native Access Variables
		
		private AndroidJavaObject  	Plugin
		{
			get;
			set;
		}
		
		#endregion
	}
}
#endif