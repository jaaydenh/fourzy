using System.IO;
using UnityEngine;
using UnityEditor;

using UnityEngine.Networking;


namespace SA.Foundation.Editor
{
    public static class SA_PackageManager
    {

        public static void DownloadAndImport(string packageName, string packageUrl, bool interactive) {

            var request = new SA_EditorWebRequest(packageUrl);

            request.SetProgressDialog(packageName);
            request.Send((unityRequest) => {

                if(unityRequest.error != null) {
                    EditorUtility.DisplayDialog("Package Download failed.", unityRequest.error, "Ok");
                    return;
                }

                //Asset folder name remove
                var projectPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
                var tmpPackageFile = projectPath + FileUtil.GetUniqueTempPathInProject() + ".unityPackage";
                

                File.WriteAllBytes(tmpPackageFile, unityRequest.downloadHandler.data);


                //TODO this isn't working, but I would like to delete tmp package file after the impoty
                //AssetDatabase.importPackageCompleted += OnImportPackageCallback;
                //AssetDatabase.importPackageCancelled += OnImportPackageCallback;

                AssetDatabase.ImportPackage(tmpPackageFile, interactive);

            });
            
        }

        /*
        private static void OnImportPackageCallback(string packageName) {
            Debug.Log("imported: " + packageName);
        }*/

    }
}