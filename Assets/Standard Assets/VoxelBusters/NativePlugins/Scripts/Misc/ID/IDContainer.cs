using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	[System.Serializable]
	public class IDContainer
	{
		#region Fields
		
		[SerializeField]
		private 	string			m_globalID;
		[SerializeField]
		private		PlatformID[]	m_platformIDs;
		
		#endregion

		#region Properties

		public string GlobalID
		{
			get
			{
				return m_globalID;
			}
		}
		
		public PlatformID[] PlatformIDs
		{
			get
			{
				return m_platformIDs;
			}
		}

		#endregion

		#region Constructors

		private IDContainer ()
		{}

		public IDContainer (string _globalID, params PlatformID[] _platformIDs)
		{
			// Initialize properties
			m_globalID		= _globalID;
			m_platformIDs	= _platformIDs;
		}

		#endregion

		#region Methods

		public bool EqualsGlobalID (string _identifier)
		{
			if (m_globalID == null)
				return false;

			return m_globalID.Equals(_identifier);
		}

		public bool EqualsCurrentPlatformID (string _identifier)
		{
			string _curPlatformID	= m_platformIDs.GetCurrentPlatformID();

			if (_curPlatformID == null)
				return false;

			return _curPlatformID.Equals(_identifier);
		}

		#endregion
	}
}