using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SA.Foundation.Editor;

namespace SA.iOS
{
    public class ISN_ContactsUI : ISN_ServiceSettingsUI
    {
        GUIContent ContactsUsageDescription = new GUIContent("Contacts Usage Description[?]:", " The key lets you describe the reason your app accesses the user’s contacts. When the system prompts the user to allow access, this string is displayed as part of the alert.");


        public override void OnAwake() {
            base.OnAwake();
            AddFeatureUrl("Getting Started", "https://unionassets.com/ios-native-pro/getting-started-636");
            AddFeatureUrl("Contacts Store", "https://unionassets.com/ios-native-pro/contacts-store-638");
            AddFeatureUrl("Contacts Picker", "https://unionassets.com/ios-native-pro/contacts-picker-639");
        }

        public override string Title {
            get {
                return "Contacts";
            }
        }

        public override string Description {
            get {
                return "Access the user's contacts and format and localize contact information.";
            }
        }

        protected override Texture2D Icon {
            get {
                return SA_EditorAssets.GetTextureAtPath(ISN_Skin.ICONS_PATH + "Contacts_icon.png");
            }
        }

        public override SA_iAPIResolver Resolver {
            get {
                return ISN_Preprocessor.GetResolver<ISN_ContactsResolver>();
            }
        }


        protected override IEnumerable<string> SupportedPlatforms {
            get {
                return new List<string>() { "iOS", "Unity Editor" };
            }
        }

        protected override void OnServiceUI() {
            using (new SA_WindowBlockWithSpace(new GUIContent("Contact Store"))) {
                EditorGUILayout.HelpBox("Once you link with iOS 10 you must declare access to any user private data types.", MessageType.Info);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField(ContactsUsageDescription);
                using (new SA_GuiIndentLevel(1)) {
                    ISN_Settings.Instance.ContactsUsageDescription = EditorGUILayout.TextArea(ISN_Settings.Instance.ContactsUsageDescription, SA_PluginSettingsWindowStyles.TextArea, GUILayout.Height(30));
                }
            }
        }

    }

}