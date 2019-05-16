using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using SA.Foundation.Editor;

namespace SA.iOS
{
    public class ISN_CoreLocationUI : ISN_ServiceSettingsUI
    {

        GUIContent LocationWhenInUseUsageDescription = new GUIContent("In Use Usage Description[?]:", " The key lets you describe the reason your app accesses the user’s location. When the system prompts the user to allow access, this string is displayed as part of the alert.");
        GUIContent LocationAlwaysAndWhenInUseUsageDescription = new GUIContent("Always Use Usage Description[?]:", " The key lets you describe the reason your app accesses the user’s location. When the system prompts the user to allow access, this string is displayed as part of the alert.");


        public override void OnAwake() {
            base.OnAwake();

            AddFeatureUrl("Getting Started", "https://unionassets.com/ios-native-pro/getting-started-754");
            AddFeatureUrl("Requesting Permission", "https://unionassets.com/ios-native-pro/getting-started-754#requesting-permission-to-use-location-services");
            
            AddFeatureUrl("Location Availability", "https://unionassets.com/ios-native-pro/availability-830");
            
            AddFeatureUrl("Location Updates", "https://unionassets.com/ios-native-pro/location-updates-831");
            AddFeatureUrl("Request Location", "https://unionassets.com/ios-native-pro/location-updates-831#request-location");
            AddFeatureUrl("Pause Updates", "https://unionassets.com/ios-native-pro/location-updates-831#pause-location-updates");
            
            AddFeatureUrl("Location Delegate", "https://unionassets.com/ios-native-pro/location-delegate-832");
        }

        public override string Title {
            get {
                return "Core Location";
            }
        }

        public override string Description {
            get {
                return "Provides services for determining a device’s geographic location.";
            }
        }

        protected override Texture2D Icon {
            get {
                return SA_EditorAssets.GetTextureAtPath(ISN_Skin.ICONS_PATH + "CoreLocation_icon.png");
            }
        }

        public override SA_iAPIResolver Resolver {
            get {
                return ISN_Preprocessor.GetResolver<ISN_CoreLocationResolver>();
            }
        }


        protected override void OnServiceUI() {

            using (new SA_WindowBlockWithSpace(new GUIContent("Useage Describtion"))) {
                EditorGUILayout.HelpBox("Once you link with iOS 10 you must declare access to any user private data types.", MessageType.Info);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField(LocationWhenInUseUsageDescription);
                using (new SA_GuiIndentLevel(1)) {
                    ISN_Settings.Instance.LocationWhenInUseUsageDescription = EditorGUILayout.TextArea(ISN_Settings.Instance.LocationWhenInUseUsageDescription, SA_PluginSettingsWindowStyles.TextArea, GUILayout.Height(30));
                }
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(LocationAlwaysAndWhenInUseUsageDescription);
                using (new SA_GuiIndentLevel(1)) {
                    ISN_Settings.Instance.LocationAlwaysAndWhenInUseUsageDescription = EditorGUILayout.TextArea(ISN_Settings.Instance.LocationAlwaysAndWhenInUseUsageDescription, SA_PluginSettingsWindowStyles.TextArea, GUILayout.Height(30));
                }

              
            }
        }


    }

}