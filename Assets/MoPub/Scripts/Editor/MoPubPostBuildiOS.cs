#if UNITY_IOS
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

// ReSharper disable once CheckNamespace
namespace MoPubInternal.Editor.Postbuild
{
    public static class MoPubPostBuildiOS
    {
        [PostProcessBuild(100)]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string buildPath)
        {
            if (buildTarget != BuildTarget.iOS)
                return;

            // Make sure that the proper location usage string is in Info.plist

            const string locationKey = "NSLocationWhenInUseUsageDescription";

            var plistPath = Path.Combine(buildPath, "Info.plist");
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);
            PlistElement element = plist.root[locationKey];
            var usage = MoPubConsent.LocationAwarenessUsageDescription;
            // Add or overwrite the key in the info.plist file if necessary.
            // (Note:  does not overwrite if the string has been manually changed in the Xcode project and our string is just the default.)
            if (element == null || usage != element.AsString() && usage != MoPubConsent.DefaultLocationAwarenessUsage) {
                plist.root.SetString(locationKey, usage);
                plist.WriteToFile(plistPath);
            }
        }
    }
}
#endif
