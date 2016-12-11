using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	[System.Serializable]
	public partial class NetworkConnectivitySettings
	{
		#region Fields
		
		[SerializeField]
		[Tooltip("The host IP address in IPv4 format.")]
		private 	string 			m_hostAddressIPv4 	= "8.8.8.8";
		[SerializeField]
		[Tooltip("The host IP address in IPv6 format.")]
		private 	string 			m_hostAddressIPv6 	= "0:0:0:0:0:FFFF:0808:0808";
		[SerializeField]
		private 	EditorSettings	m_editor			= new EditorSettings();
		[SerializeField]
		private 	AndroidSettings	m_android			= new AndroidSettings();

		#endregion

		#region Properties

		internal string HostAddressIPv4
		{
			get 
			{ 
				return m_hostAddressIPv4; 
			}
		}
		
		internal string HostAddressIPv6
		{
			get 
			{ 
				return m_hostAddressIPv6; 
			}
		}

		internal EditorSettings Editor
		{
			get 
			{ 
				return m_editor; 
			}
		}

		internal AndroidSettings Android
		{
			get 
			{ 
				return m_android; 
			}
		}
		
		#endregion
	}
}