using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	[System.Serializable]
	public class UtilitySettings 
	{
		#region Fields

		[SerializeField]
		[Tooltip("Rate My App dialog settings.")]
		private 	RateMyAppSettings		m_rateMyApp;

		#endregion

		#region Properties

		public RateMyAppSettings RateMyApp
		{
			get
			{
				return m_rateMyApp;
			}
			private set
			{
				m_rateMyApp	= value;
			}
		}

		#endregion
	}
}