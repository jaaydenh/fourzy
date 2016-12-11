using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	[System.Serializable]
	public class PlatformID
	{
		[System.Serializable]
		public enum ePlatform
		{
			IOS,
			ANDROID,
			EDITOR
		}

		#region Fields

		[SerializeField]
		private		ePlatform	m_platform;

		[SerializeField]
		private		string		m_value;

		#endregion

		#region Properties

		public ePlatform Platform
		{
			get
			{
				return m_platform;
			}

			private set
			{
				m_platform	= value;
			}
		}

		public string Value
		{
			get
			{
				return m_value;
			}
			
			private set
			{
				m_value		= value;
			}
		}

		#endregion

		#region Constructors

		private PlatformID ()
		{}

		private PlatformID (ePlatform _platform, string _identifier)
		{
			// Initialize
			Platform	= _platform;
			Value		= _identifier;
		}

		#endregion

		#region Static Methods

		public static PlatformID IOS (string _identifier)
		{
			return new PlatformID(ePlatform.IOS, _identifier);
		}

		public static PlatformID Android (string _identifier)
		{
			return new PlatformID(ePlatform.ANDROID, _identifier);
		}

		public static PlatformID Editor (string _identifier)
		{
			return new PlatformID(ePlatform.EDITOR, _identifier);
		}

		#endregion
	}
}