using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

using SA.iOS.XCode;


namespace SA.iOS.Utilities {

    public static class ISN_TestManager
    {


        public static void ApplyExampleConfig() {


            PlayerSettings.iOS.applicationDisplayName = "IOS Native";
            PlayerSettings.iOS.appleEnableAutomaticSigning = true;
            PlayerSettings.iOS.appleDeveloperTeamID = "P42C7H5LKK";
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, "com.iosnative");


            var settings = ISN_Settings.Instance;

            //Contacts
            settings.Contacts = true;

            //GameKit
            ISD_API.Capability.GameCenter.Enabled = true;


            //Replay Kit
            settings.ReplayKit = true;

            //User Notifications
            settings.UserNotifications = true;
            ISD_API.Capability.PushNotifications.Enabled = true;


            //Makign environment for Vending Test
            ISD_API.Capability.InAppPurchase.Enabled = true;

            //social 
            settings.Social = true;


        }

        public static void OpenTestScene() {
            EditorSceneManager.OpenScene(ISN_Settings.TEST_SCENE_PATH, OpenSceneMode.Single);
        }


        public static void MakeTestBuild() {
            BuildPlayerOptions playerOptions = new BuildPlayerOptions();
            playerOptions.target = BuildTarget.iOS;
            playerOptions.scenes = new string[] { ISN_Settings.TEST_SCENE_PATH };
            playerOptions.locationPathName = "ios_native_test";


            playerOptions.options = BuildOptions.Development | BuildOptions.AutoRunPlayer;

            BuildPipeline.BuildPlayer(playerOptions);
        }

    }
}



