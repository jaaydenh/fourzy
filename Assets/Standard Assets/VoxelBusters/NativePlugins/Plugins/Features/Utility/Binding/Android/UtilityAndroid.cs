using UnityEngine;
using System.Collections;
using VoxelBusters.DebugPRO;

#if UNITY_ANDROID
namespace VoxelBusters.NativePlugins
{
	using Internal;

	public partial class UtilityAndroid : Utility 
	{

		#region Constructors
		
		UtilityAndroid()
		{
			Plugin = AndroidPluginUtility.GetSingletonInstance(Native.Class.NAME);
		}
		
		#endregion
		

		#region API's

		public override void OpenStoreLink (string _applicationID)
		{
			Console.Log(Constants.kDebugTag, "[Utility] Opening store link, ApplicationID=" + _applicationID);

			//Not opening with Market scheme as it can crash on older deivices if market unavailable.
			Application.OpenURL("http://play.google.com/store/apps/details?id=" + _applicationID);
				
		}

		public override void SetApplicationIconBadgeNumber (int _badgeNumber)
		{
			Plugin.Call(Native.Methods.SET_APPLICATION_ICON_BADGE_NUMBER, _badgeNumber);
		}

		#endregion
	}
}
#endif