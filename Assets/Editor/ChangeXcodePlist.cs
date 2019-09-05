using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using UnityEditor.iOS.Xcode;
using System.IO;
 
public class PostBuildSteps {

    [PostProcessBuild]
    public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject)
    {

        if (buildTarget == BuildTarget.iOS)
        {

            // Get plist
            string plistPath = pathToBuiltProject + "/Info.plist";
            var plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            // Get root
            var rootDict = plist.root;

            var buildKey = "ITSAppUsesNonExemptEncryption";
            rootDict.SetString(buildKey, "false");

//              // Change value of CFBundleVersion in Xcode plist
//              var buildKey = "UIBackgroundModes";
//              rootDict.CreateArray (buildKey).AddString ("remote-notification");

            // Write to file
            File.WriteAllText(plistPath, plist.WriteToString());
        }
    }
}