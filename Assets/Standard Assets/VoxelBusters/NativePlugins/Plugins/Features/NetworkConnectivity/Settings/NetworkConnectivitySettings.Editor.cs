using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	public partial class NetworkConnectivitySettings
	{
		[System.Serializable]
		internal class EditorSettings
		{
			#region Fields
			
			[SerializeField]
			[Tooltip ("The number of seconds to wait before the request times out.")]		
			private 	int 		m_timeOutPeriod 		= 60;
			[SerializeField]
			[Tooltip ("The number of retry attempts, when a response is not received from the host.")]		
			private 	int 		m_maxRetryCount 		= 2;
			[SerializeField]
			[Tooltip ("The time interval between consecutive poll.")]		
			private 	float 		m_timeGapBetweenPolling = 2.0f;
			
			#endregion
			
			#region Properties
			
			internal int TimeOutPeriod
			{
				get 
				{ 
					return m_timeOutPeriod; 
				}
			}
			
			internal int MaxRetryCount
			{
				get 
				{ 
					return m_maxRetryCount; 
				}
			}
			
			internal float TimeGapBetweenPolling
			{
				get 
				{ 
					return m_timeGapBetweenPolling;
				}
			}

			#endregion
		}
	}
}