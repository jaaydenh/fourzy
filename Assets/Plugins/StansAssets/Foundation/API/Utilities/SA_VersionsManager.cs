////////////////////////////////////////////////////////////////////////////////
//  
// @module Stan's Assets Commons Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

#if UNITY_EDITOR
using System;
using SA.Foundation.Utility;


namespace SA.Foundation.Editor {


	public static class SA_VersionsManager  {
        


		public static int ParceMagorVersion(string stringVersionId) {
			string[] versions = stringVersionId.Split (new char[] {'.', '/'});
			int intVersion = Int32.Parse(versions[0]) * 100;
			return  intVersion;
		} 


		private static int GetMagorVersionCode(string versionFilePath) {
            string stringVersionId = SA_FilesUtil.Read (versionFilePath);
			return ParceMagorVersion(stringVersionId);
		}



		public static int ParceVersion(string stringVersionId) {
			string[] versions = stringVersionId.Split (new char[] {'.', '/'});
			int intVersion = Int32.Parse(versions[0]) * 100 + Int32.Parse(versions[1]) * 10;
			return  intVersion;
		} 



		private static int GetVersionCode(string versionFilePath) {
            string stringVersionId = SA_FilesUtil.Read (versionFilePath);
			return ParceVersion(stringVersionId);
		}



		private static string GetStringVersionId(string versionFilePath) {
            if(SA_FilesUtil.IsFileExists(versionFilePath)) {
                return SA_FilesUtil.Read (versionFilePath);
			} else {
				return "0.0";
			}
		}

	}

}

#endif
