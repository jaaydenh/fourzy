using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.Foundation.Utility;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SA.Foundation.UtilitiesEditor
{
    public static class SA_EditorUtility
    {
        /// <summary>
        /// Marks target object as dirty. (Only suitable for non-scene objects).
        /// </summary>
        /// <param name="target"> The object to mark as dirty.</param> 
        public static void SetDirty(Object target) {
            #if UNITY_EDITOR
            //TODO use Undo
            EditorUtility.SetDirty(target);
            #endif
        }


        /// <summary>
        /// Open's script file with assosialted application.
        /// </summary>
        /// <param name="pathToScript"> Asset/ folder relative path to a script file </param> 
        /// <param name="lineNumber"> Script line number you want to put a cursor on </param> 
        public static void OpenScript(string pathToScript, int lineNumber) {

            #if UNITY_EDITOR
            if (SA_FilesUtil.IsFileExists(pathToScript)) {
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(pathToScript);
                AssetDatabase.OpenAsset(script.GetInstanceID(), lineNumber);
            } else {
                Debug.LogError("Script you trying to open doesn't exist: " + pathToScript);
            }
            #endif
        }
    }
}