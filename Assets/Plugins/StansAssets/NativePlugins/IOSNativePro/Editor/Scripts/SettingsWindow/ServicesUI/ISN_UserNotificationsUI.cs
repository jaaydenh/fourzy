using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using SA.iOS.XCode;
using SA.Foundation.Editor;
using Rotorz.ReorderableList;

using SA.Foundation.UtilitiesEditor;


namespace SA.iOS
{
    public class ISN_UserNotificationsUI : ISN_ServiceSettingsUI
    {

        GUIContent m_note = new GUIContent("Note: Enabling User Notification, will also enable App Delegate.");
        GUIContent m_APN_Description = new GUIContent("Remote notifications are appropriate " +
            "when some or all of the app’s data is" +
            " managed by your company’s servers.");

        public override void OnAwake() {
            base.OnAwake();

            AddFeatureUrl("Getting Started", "https://unionassets.com/ios-native-pro/getting-started-612");
            AddFeatureUrl("Scheduling", "https://unionassets.com/ios-native-pro/scheduling-notifications-633");
            AddFeatureUrl("Handling Notifications", "https://unionassets.com/ios-native-pro/responding-to-notification-634");
            AddFeatureUrl("Remote Notifications", "https://unionassets.com/ios-native-pro/remote-notifications-635");
        }

        public override string Title {
            get {
                return "User Notifications";
            }
        }

        public override string Description {
            get {
                return "Supports the delivery and handling of local and remote notifications.";
            }
        }

        protected override Texture2D Icon {
            get {
                return SA_EditorAssets.GetTextureAtPath(ISN_Skin.ICONS_PATH + "UserNotifications_icon.png");
            }
        }

        public override SA_iAPIResolver Resolver {
            get {
                return ISN_Preprocessor.GetResolver<ISN_UserNotificationsResolver>();
            }
        }

        protected override IEnumerable<string> SupportedPlatforms {
            get {
                return new List<string>() { "iOS" };
            }
        }


        protected override void GettingStartedBlock() {
            base.GettingStartedBlock();
            using (new SA_GuiBeginHorizontal()) {
                GUILayout.Space(15);
                GUILayout.Label(m_note, SA_PluginSettingsWindowStyles.AssetLabel);
            }
        }


        protected override void OnServiceUI() {
            using (new SA_WindowBlockWithSpace(new GUIContent("Local Notifications"))) {

                ReorderableListGUI.Title("Custom Sounds");
                ReorderableListGUI.ListField(ISN_EditorSettings.Instance.NotificationAlertSounds, DrawObjectField, DrawEmptySounds);

                UpdateDeploySettings();
            }

            using (new SA_WindowBlockWithSpace(new GUIContent("Apple Push Notification Service"))) {

                using (new SA_GuiBeginHorizontal()) {
                    GUILayout.Space(15);
                    EditorGUILayout.LabelField(m_APN_Description, SA_PluginSettingsWindowStyles.DescribtionLabelStyle);
                }

                EditorGUILayout.Space();

                using (new SA_GuiIndentLevel(1)) {
                    ISD_API.Capability.PushNotifications.Enabled = SA_EditorGUILayout.ToggleFiled("API Status", ISD_API.Capability.PushNotifications.Enabled, SA_StyledToggle.ToggleType.EnabledDisabled);
                    ISD_API.Capability.PushNotifications.development = SA_EditorGUILayout.ToggleFiled("Development Environment", ISD_API.Capability.PushNotifications.development, SA_StyledToggle.ToggleType.EnabledDisabled);
                }
            }
        }


      

        private void UpdateDeploySettings() {
            foreach (var asset in ISN_EditorSettings.Instance.NotificationAlertSounds) {
                if (asset == null) {
                    continue;
                }

                bool exists = ISD_API.HasFile(asset);
                if(!exists) {
                    ISD_AssetFile xCodeFileLink = new ISD_AssetFile();
                    xCodeFileLink.Asset = asset;
                    ISD_API.AddFile(xCodeFileLink);
                }

            }
        }


        private T DrawObjectField<T>(Rect position, T itemValue) where T : Object {
            Rect drawRect = new Rect(position);
            drawRect.y += 2;
            drawRect.height = 15;
            return (T)EditorGUI.ObjectField(drawRect, itemValue, typeof(T), false);
        }

        private void DrawEmptySounds() {
            EditorGUILayout.LabelField("Add sound clips you want to use as custom notification alert sound. The phone default alert sound will be used by default", SA_Skin.MiniLabelWordWrap);
        }

    }



}