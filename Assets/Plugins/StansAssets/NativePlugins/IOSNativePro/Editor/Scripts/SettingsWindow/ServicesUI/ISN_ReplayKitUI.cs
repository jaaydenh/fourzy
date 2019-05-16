using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SA.Foundation.Editor;


namespace SA.iOS
{
    public class ISN_ReplayKitUI : ISN_ServiceSettingsUI
    {

        public override void OnAwake() {
            base.OnAwake();

            AddFeatureUrl("Getting Started", "https://unionassets.com/ios-native-pro/getting-started-with-replaykit-618");
            AddFeatureUrl("Capturing & Sharing", "https://unionassets.com/ios-native-pro/capturing-sharing-626");


        }

        public override string Title {
            get {
                return "Replay Kit";
            }
        }

        public override string Description {
            get {
                return "Record video from the screen, and audio from the app and microphone.";
            }
        }

        protected override Texture2D Icon {
            get {
               return  SA_EditorAssets.GetTextureAtPath(ISN_Skin.ICONS_PATH + "ReplayKit_icon.png");
            }
        }

        public override SA_iAPIResolver Resolver {
            get {
                return ISN_Preprocessor.GetResolver<ISN_ReplayKitResolver>();
            }
        }

        protected override IEnumerable<string> SupportedPlatforms {
            get {
                return new List<string>() { "iOS" };
            }
        }

        protected override void OnServiceUI() {

        }
    }
}