using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

class MoPubSampleBuild
{
    private const string LAST_COMMIT_PREFIX = "lastCommit=";

    /// <summary>
    /// Build sample app for the currently active build target (Android or iOS). Mostly needed due to the lack of
    /// the batchmode options -buildAndroidPlayer and -buildiOSPlayer.
    /// </summary>
    [UsedImplicitly]
    static void PerformBuild ()
    {
        var platform = EditorUserBuildSettings.activeBuildTarget;
        var sdkVersion = MoPub.MoPubSdkVersion;
        var args = Environment.GetCommandLineArgs();
        var lastCommit = args.First(a => a.StartsWith(LAST_COMMIT_PREFIX)).Substring(LAST_COMMIT_PREFIX.Length);
        var filename = string.Format("MoPubSampleUnity{0}_{1}+{2}{3}", platform, sdkVersion, lastCommit,
                                     platform.ToString() == "Android" ? ".apk" : "");

        BuildPipeline.BuildPlayer(new BuildPlayerOptions {
            scenes = EditorBuildSettings.scenes.Select(s => s.path).ToArray(),
            locationPathName = "Build/" + filename,
            target = platform
        });
    }
}
