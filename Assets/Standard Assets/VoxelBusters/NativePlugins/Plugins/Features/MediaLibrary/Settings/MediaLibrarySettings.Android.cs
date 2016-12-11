using UnityEngine;
using System.Collections;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins
{
	public partial class MediaLibrarySettings
	{
		[System.Serializable]
		public class AndroidSettings 
		{
			#region Fields
			
			[SerializeField]
			[Tooltip("Youtube API key assigned to your application.")]
			private 	string 		m_youtubeAPIKey;
			
			#endregion
			
			#region Properties
			
			internal string YoutubeAPIKey
			{
				get 
				{ 
					return m_youtubeAPIKey; 
				}
			}
			
			#endregion
		}
	}
}