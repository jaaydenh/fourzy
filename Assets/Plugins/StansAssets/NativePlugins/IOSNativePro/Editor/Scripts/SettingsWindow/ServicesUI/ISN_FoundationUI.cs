using UnityEngine;
using UnityEditor;


using SA.iOS.XCode;
using SA.Foundation.Editor;


namespace SA.iOS
{
    public class ISN_FoundationUI : ISN_ServiceSettingsUI
    {

        public override void OnAwake() {
            base.OnAwake();

            AddFeatureUrl("iCloud Key-Value", "https://unionassets.com/ios-native-pro/icloud-key-value-storage-615");
            AddFeatureUrl("Build Info", "https://unionassets.com/ios-native-pro/buildinfo-616");
            AddFeatureUrl("App Environment", "https://unionassets.com/ios-native-pro/buildinfo-616#app-environment");
            AddFeatureUrl("Time Zone", "https://unionassets.com/ios-native-pro/time-zone-671");
            AddFeatureUrl("Check If App Installed", "https://unionassets.com/ios-native-pro/url-queries-schemes-607#how-to-check-programmatically-if-an-app-is-installed");

            AddFeatureUrl("Notification Center", "https://unionassets.com/ios-native-pro/notification-center-820");
            AddFeatureUrl("System Locale", "https://unionassets.com/ios-native-pro/locale-823");


            //Av foundation
            AddFeatureUrl("Audio Session", "https://unionassets.com/ios-native-pro/audio-session-617");
            AddFeatureUrl("Camera Permission", "https://unionassets.com/ios-native-pro/camera-permission-757");

        }
        public override string Title {
            get {
                return "Foundation";
            }
        }

        public override string Description {
            get {
                return "Access essential data types, collections, and operating-system services to define the base layer of functionality for your app.";
            }
        }

        protected override Texture2D Icon {
            get {
                return SA_EditorAssets.GetTextureAtPath(ISN_Skin.ICONS_PATH + "Foundation_icon.png");
            }
        }

        public override SA_iAPIResolver Resolver {
            get {
                return ISN_Preprocessor.GetResolver<ISN_FoundationResolver>();
            }
        }

        protected override bool CanBeDisabled {
            get {
                return false;
            }
        }



        protected override void OnServiceUI() {
            DrawICloudSettings();
        }

        private void DrawICloudSettings() {

            using (new SA_WindowBlockWithSpace(new GUIContent("iCloud Key-Value Storage"))) {

                var description = new GUIContent("Key-value storage is similar to Unity PlayerPrefs; " +
                                                  "but values that you place in key-value storage are available to every " +
                                                  "instance of your app on all of a user’s various devices.");



                using (new SA_GuiBeginHorizontal()) {
                    GUILayout.Space(15);
                    EditorGUILayout.LabelField(description, SA_PluginSettingsWindowStyles.DescribtionLabelStyle);
                }

                EditorGUILayout.Space();

                bool KeyValueStorageEnabled = ISD_API.Capability.iCloud.Enabled && ISD_API.Capability.iCloud.keyValueStorage;
                EditorGUI.BeginChangeCheck();
                KeyValueStorageEnabled = SA_EditorGUILayout.ToggleFiled("API Status", KeyValueStorageEnabled, SA_StyledToggle.ToggleType.EnabledDisabled);

                if (EditorGUI.EndChangeCheck()) {
                    if (KeyValueStorageEnabled) {
                        ISD_API.Capability.iCloud.Enabled = true;
                        ISD_API.Capability.iCloud.keyValueStorage = true;
                    } else {
                        ISD_API.Capability.iCloud.Enabled = false;
                        ISD_API.Capability.iCloud.keyValueStorage = false;
                    }
                }
 
            }
        }
    }

}