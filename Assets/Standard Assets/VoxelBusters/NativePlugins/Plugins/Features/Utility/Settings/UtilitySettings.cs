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
		private 	RateMyApp.Settings		m_rateMyApp;

		#endregion

		#region Properties

		public RateMyApp.Settings RateMyApp
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