using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	public static class IIdentifierContainerUtils
	{
		#region Static Methods
		
		public static IIdentifierContainer FindObjectWithGlobalID (this IIdentifierContainer[] _collection, string _globalID)
		{
			foreach (IIdentifierContainer _currentObject in _collection)
			{
				string	_currentObjectGlobalID	= _currentObject.GlobalID;

				if (_currentObjectGlobalID == null)
					continue;

				if (_currentObjectGlobalID.Equals(_globalID))
					return _currentObject;
			}
			
			// Couldn't find a matching identifier
			Debug.Log(string.Format("[IDContainer] Couldn't find id container with global ID= {0}.", _globalID));

			return null;
		}
		
		public static IIdentifierContainer FindObjectWithPlatformID (this IIdentifierContainer[] _collection, string _platformID)
		{
			foreach (IIdentifierContainer _currentObject in _collection)
			{
				string 	_currentObjectPlatformID	= _currentObject.GetCurrentPlatformID();

				if (_currentObjectPlatformID == null)
					continue;

				if (_currentObjectPlatformID.Equals(_platformID))
					return _currentObject;
			}
			
			// Couldn't find a matching identifier
			Debug.Log(string.Format("[IDContainer] Couldn't find id container with platform ID= {0}.", _platformID));

			return null;
		}

		public static string GetCurrentPlatformID (this IIdentifierContainer _object)
		{
			return _object.PlatformIDs.GetCurrentPlatformID();
		}

		#endregion
	}
}