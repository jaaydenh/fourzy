using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	public partial class ApplicationSettings 
	{
		/// <summary>
		/// Application Settings specific to iOS platform.
		/// </summary>
		[System.Serializable]
		public class iOSSettings
		{
			#region Properties

			[SerializeField]
			[Tooltip("The string that identifies your app in iOS App Store.")]
			private 	string		m_storeIdentifier;

			#endregion

			#region Fields

			/// <summary>
			/// The string that identifies your app in App Store. (read-only)
			/// </summary>
			public string StoreIdentifier
			{
				get
				{
					return m_storeIdentifier;
				}
				
				private set
				{
					m_storeIdentifier	= value;
				}
			}

			#endregion
		}
	}
}