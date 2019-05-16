﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace SA.Foundation.Editor
{
   
    /// <summary>
    /// Provides method for managing the <see cref="PlayerSettings"/> script defines 
    /// </summary>
    public static class SA_EditorDefines 
    {

        /// <summary>
        /// Attempts to add a new #define constant to the Player Settings
        /// </summary>
        /// <param name="newDefineCompileConstant">constant to attempt to define</param>
        /// <param name="targets">platforms to add this for (default will add to all platforms)</param>
        
        public static void AddCompileDefine(string newDefineCompileConstant, params BuildTarget[] targets) {

            if (targets.Length == 0)
                targets = (BuildTarget[])Enum.GetValues(typeof(BuildTarget));

            foreach (BuildTarget target in targets) {
                var targetGroup = BuildPipeline.GetBuildTargetGroup(target);

                if (!IsBuildTargetSupported(targetGroup, target)) {
                    continue;
                }
                if (targetGroup == BuildTargetGroup.Unknown)        //the unknown group does not have any constants location
                    continue;

                string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
                if (!defines.Contains(newDefineCompileConstant)) {
                    if (defines.Length > 0)         //if the list is empty, we don't need to append a semicolon first
                        defines += ";";
                    defines += newDefineCompileConstant;
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
                }
            }
        }

        /// <summary>
        /// Attempts to remove a #define constant from the Player Settings
        /// </summary>
        /// <param name="defineCompileConstant">define constant</param>
        /// <param name="targets">platforms to add this for (default will add to all platforms)</param>
        
        public static void RemoveCompileDefine(string defineCompileConstant, params BuildTarget[] targetGroups) {
            if (targetGroups.Length == 0)
                targetGroups = (BuildTarget[])Enum.GetValues(typeof(BuildTarget));

            foreach (BuildTarget target in targetGroups) {
                var targetGroup = BuildPipeline.GetBuildTargetGroup(target);
                if (!IsBuildTargetSupported(targetGroup, target)) {
                    continue;
                }

                string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
                int index = defines.IndexOf(defineCompileConstant, StringComparison.CurrentCulture);
                if (index < 0)
                    continue;           //this target does not contain the define
                else if (index > 0)
                    index -= 1;         //include the semicolon before the define
                                        //else we will remove the semicolon after the define

                //Remove the word and it's semicolon, or just the word (if listed last in defines)
                int lengthToRemove = Math.Min(defineCompileConstant.Length + 1, defines.Length - index);

                //remove the constant and it's associated semicolon (if necessary)
                defines = defines.Remove(index, lengthToRemove);

                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
            }
        }
        

        /// <summary>
        /// Check if define exists
        /// </summary>
        /// <param name="defineCompileConstant">constant to attempt to define</param>
        /// <param name="targetGroups">platforms to add this for (default will add to all platforms)</param>
        public static bool HasCompileDefine(string defineCompileConstant, params BuildTarget[] targets) {
            if (targets.Length == 0)
                targets = (BuildTarget[])Enum.GetValues(typeof(BuildTarget));

            foreach (BuildTarget target in targets) {
                var targetGroup = BuildPipeline.GetBuildTargetGroup(target);
                if (!IsBuildTargetSupported(targetGroup, target)) {
                    continue;
                }
                if (targetGroup == BuildTargetGroup.Unknown)        //the unknown group does not have any constants location
                    continue;

                string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
                if (!defines.Contains(defineCompileConstant)) {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Get user-specified symbols for script compilation for the current build target group
        /// </summary>
        public static string[] GetScriptingDefines() {
            var target = EditorUserBuildSettings.activeBuildTarget;
            var targetGroup = BuildPipeline.GetBuildTargetGroup(target);

            return GetScriptingDefines(targetGroup);
        }

        /// <summary>
        /// Get user-specified symbols for script compilation for the given build target group
        /// </summary>
        /// <param name="targetGroup">build target group</param>
        public static string[] GetScriptingDefines(BuildTargetGroup targetGroup) {
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            return defines.Split(';');
        }

        private static bool IsBuildTargetSupported(BuildTargetGroup targetGroup, BuildTarget target) {
#if UNITY_2018_1_OR_NEWER
            return BuildPipeline.IsBuildTargetSupported(targetGroup, target);
#else
            MethodInfo isBuildTargetSupported = typeof(BuildPipeline).GetMethod("IsBuildTargetSupported", BindingFlags.Static | BindingFlags.NonPublic);
            return Convert.ToBoolean(isBuildTargetSupported.Invoke(null, new object[] { targetGroup, target }));
#endif
        }

    }
}