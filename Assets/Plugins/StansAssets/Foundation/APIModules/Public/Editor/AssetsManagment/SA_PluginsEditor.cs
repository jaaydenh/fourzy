using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SA.Foundation.Config;
using SA.Foundation.UtilitiesEditor;

namespace SA.Foundation.Editor
{
    public static class SA_PluginsEditor
    {
        public const string DISABLED_LIB_EXTENSION = ".txt";



        public static void UninstallLibFolder(string path) {
            if (SA_AssetDatabase.IsDirectoryExists(path)) {
                EditorUtility.DisplayProgressBar("Stan's Assets.", "Uninstalling: " + path, 1);
                SA_AssetDatabase.DeleteAsset(path);
                EditorUtility.ClearProgressBar();
            }
        }

        public static void InstallLibs(string source, string destination, List<string> libs) {

            for (int i = 0; i < libs.Count; i++) {
                var lib = libs[i];
                string disabledLib = lib + DISABLED_LIB_EXTENSION;
                string sourcePath = source + disabledLib;
                string destinationPath = destination + lib;


                if (!SA_AssetDatabase.IsFileExists(sourcePath)) {
                    Debug.LogError("Can't find the source lib folder at path: " + sourcePath);
                    continue;
                }

                float progress = (float)(i + 1) / (float)libs.Count;
                EditorUtility.DisplayProgressBar("Stan's Assets.", "Installing: " + lib, progress);

                SA_AssetDatabase.CopyAsset(sourcePath, destinationPath);
            }

            EditorUtility.ClearProgressBar();
        }

        public static void UninstallLibs(string path, List<string> libs) {
            for (int i = 0; i < libs.Count; i++) {
                var lib = libs[i];
                float progress = (float)(i + 1) / (float)libs.Count;
                EditorUtility.DisplayProgressBar("Stan's Assets.", "Uninstalling: " + lib, progress);

                string libPath = path + lib;
                if(SA_AssetDatabase.IsFileExists(libPath) || SA_AssetDatabase.IsDirectoryExists(libPath)) {
                    SA_AssetDatabase.DeleteAsset(path + lib);
                } else {
                    Debug.LogWarning("There is no file to deleted at: " + libPath);
                }
            }

            EditorUtility.ClearProgressBar();
        }

        public static void InstallLibFolder(string source, string destination) {
            if (!SA_AssetDatabase.IsDirectoryExists(source)) {
                Debug.LogError("Can't find the source lib folder at path: " + source);
                return;
            }


            //Clean before install
            if (SA_AssetDatabase.IsDirectoryExists(destination)) {
                SA_AssetDatabase.DeleteAsset(destination);
            }

            SA_AssetDatabase.CopyAsset(source, destination);
            EnableLibsAtPath(destination);
        }



        public static void DisableLibstAtPath(string path) {
            List<string> files = SA_AssetDatabase.FindAssetsWithExtentions(path);
            for (int i = 0; i < files.Count; i++) {
                var filePath = files[i];

                //Make sure this is not a folder
                if (SA_AssetDatabase.IsValidFolder(filePath)) {
                    continue;
                }

                //Already disabled
                if(SA_AssetDatabase.GetExtension(filePath).Equals(DISABLED_LIB_EXTENSION)) {
                    continue;
                }

                string newFilePath;
                newFilePath = filePath + DISABLED_LIB_EXTENSION;


                float progress = (float)(i + 1) / (float)files.Count;
                string fileName = SA_AssetDatabase.GetFileName(newFilePath);
                EditorUtility.DisplayProgressBar("Stan's Assets.", "Packing: " + fileName, progress);
                SA_AssetDatabase.MoveAsset(filePath, newFilePath);
                SA_AssetDatabase.ImportAsset(newFilePath);
            }

            EditorUtility.ClearProgressBar();


        }

        public static void EnableLibsAtPath(string path) {
            List<string> files = SA_AssetDatabase.FindAssetsWithExtentions(path);

            for(int i = 0; i < files.Count; i++) {
                var file = files[i];
                //Make sure this is not a folder
                if (SA_AssetDatabase.IsValidFolder(file)) {
                    continue;
                }


                if (SA_AssetDatabase.GetExtension(file).Equals(DISABLED_LIB_EXTENSION)) {
                    string newFileName = file.Replace(DISABLED_LIB_EXTENSION, string.Empty);

                    string fileName = SA_AssetDatabase.GetFileName(newFileName);

                    float progress = (float) (i + 1) / (float)files.Count;
                    EditorUtility.DisplayProgressBar("Stan's Assets.", "Installing: " + fileName, progress);


                    SA_AssetDatabase.MoveAsset(file, newFileName);
                    SA_AssetDatabase.ImportAsset(newFileName);
                }
            }

            EditorUtility.ClearProgressBar();

        }

    }
}