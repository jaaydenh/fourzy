using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	public interface IIdentifierContainer
	{
		#region Properties

		string GlobalID
		{
			get;
			set;
		}

		PlatformID[] PlatformIDs
		{
			get;
			set;
		}

		#endregion
	}
}