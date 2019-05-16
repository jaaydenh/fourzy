////////////////////////////////////////////////////////////////////////////////
//  
// @module Assets Common Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

namespace SA.Foundation.Patterns {


	/// <summary>
	/// This class simplifies a singleton pattern implementation
	/// </summary>
	public abstract class SA_NonMonoSingleton<T>  where T : SA_NonMonoSingleton<T>, new() {

		private static T _Instance = null;

		/// <summary>
		/// Returns a singleton class instance
		/// </summary>
		public static T Instance {
			get {
				if (_Instance == null) {
					_Instance = new T();
				}

				return _Instance;
			}

		}
	}
}
