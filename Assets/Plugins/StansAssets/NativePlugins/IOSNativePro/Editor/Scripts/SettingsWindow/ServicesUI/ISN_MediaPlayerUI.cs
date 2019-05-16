using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using SA.iOS.XCode;
using SA.Foundation.Editor;
using Rotorz.ReorderableList;

using SA.Foundation.UtilitiesEditor;


namespace SA.iOS
{
    public class ISN_MediaPlayerUI : ISN_ServiceSettingsUI
    {

        readonly GUIContent MediaLibraryUsageDescription = new GUIContent("Media Library Usage Description[?]:", "Describes the reason that the app accesses the device’s media library. When the system prompts the user to allow access, this string is displayed as part of the alert.");
       

        public override void OnAwake() {
            base.OnAwake();

            AddFeatureUrl("Music Player", "https://unionassets.com/ios-native-pro/music-player-819");
            AddFeatureUrl("Music Player Track", "https://unionassets.com/ios-native-pro/music-player-819#current-track-info");
            AddFeatureUrl("Music Player Events", "https://unionassets.com/ios-native-pro/music-player-819#playback-notifications");
            AddFeatureUrl("Remote Commands", "https://unionassets.com/ios-native-pro/remote-command-center-721");

        }


        public override string Title {
            get {
                return "Media Player";
            }
        }

        public override string Description {
            get {
                return "Add the ability to find and play songs, audio podcasts, audio books, and more from within your app.";
            }
        }

        protected override Texture2D Icon {
            get {
                return SA_EditorAssets.GetTextureAtPath(ISN_Skin.ICONS_PATH + "MediaPlayer_icon.png");
            }
        }

        public override SA_iAPIResolver Resolver {
            get {
                return ISN_Preprocessor.GetResolver<ISN_MediaPlayerResolver>();
            }
        }

        protected override IEnumerable<string> SupportedPlatforms {
            get {
                return new List<string>() { "iOS" };
            }
        }


        protected override void OnServiceUI() {
            using (new SA_WindowBlockWithSpace(new GUIContent("Music Player Controller"))) {
                ISN_Settings.Instance.MediaLibraryUsageDescriptionEnabled = SA_EditorGUILayout.ToggleFiled("API State", ISN_Settings.Instance.MediaLibraryUsageDescriptionEnabled, SA_StyledToggle.ToggleType.EnabledDisabled);
                using (new SA_GuiEnable(ISN_Settings.Instance.MediaLibraryUsageDescriptionEnabled)) {
                    EditorGUILayout.LabelField(MediaLibraryUsageDescription);
                    using (new SA_GuiIndentLevel(1)) {
                        ISN_Settings.Instance.MediaLibraryUsageDescription = EditorGUILayout.TextArea(ISN_Settings.Instance.MediaLibraryUsageDescription, SA_PluginSettingsWindowStyles.TextArea, GUILayout.Height(30));
                    }
                }
            }
        }

    }


}