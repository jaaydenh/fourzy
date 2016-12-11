using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	public partial class NetworkConnectivitySettings
	{
		[System.Serializable]
		internal class AndroidSettings
		{
			#region Fields
			
			[SerializeField]
			[Tooltip ("The connection port of the host. For DNS IP, it will be 53 or else 80.")]		
			private 	int 		m_port 					= 53;
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
			
			internal int Port
			{
				get 
				{ 
					return m_port; 
				}
			}
			
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
					return m_maxRetryCount; //TODO should allow setters as well
				}
			}
			
			internal float TimeGapBetweenPolling
			{
				get 
				{ 
					return m_timeGapBetweenPolling; //TODO should allow setters as well
				}
			}

			#endregion
		}
	}
}