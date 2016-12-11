using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	public partial class BillingSettings 
	{
		[System.Serializable]
		public class iOSSettings
		{
			#region Fields

			[SerializeField]
			[Tooltip("If enabled, payment receipts are validated before sending events. It's an optional measure used to avoid unauthorized puchases.")]
			private 		bool		m_supportsReceiptValidation		= true;
			[SerializeField]
			[Tooltip("Custom server URL used for receipt validation. By default, Apple server is used.")]
			private 		string		m_validateUsingServerURL;

			#endregion

			#region Properties

			internal bool SupportsReceiptValidation
			{
				get 
				{ 
					return m_supportsReceiptValidation; 
				}
			}

			internal string ValidateUsingServerURL
			{
				get 
				{ 
					return m_validateUsingServerURL; 
				}
			}

			#endregion
		}
	}
}