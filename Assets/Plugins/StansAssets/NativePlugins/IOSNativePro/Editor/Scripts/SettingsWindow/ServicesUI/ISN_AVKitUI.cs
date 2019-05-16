using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using SA.Foundation.Editor;

namespace SA.iOS
{
    public class ISN_AVKitUI : ISN_ServiceSettingsUI
    {

        public override void OnAwake() {
            base.OnAwake();

            AddFeatureUrl("AVPlayer View Controller", "https://unionassets.com/ios-native-pro/avplayerviewcontroller-663");

        }

        public override string Title {
            get {
                return "AVKit";
            }
        }

        public override string Description {
            get {
                return " The AVKit framework provides a high-level interface for playing video content..";
            }
        }

        protected override Texture2D Icon {
            get {
                return SA_EditorAssets.GetTextureAtPath(ISN_Skin.ICONS_PATH + "AVKit_icon.png");
            }
        }

        public override SA_iAPIResolver Resolver {
            get {
                return ISN_Preprocessor.GetResolver<ISN_AVKitResolver>();
            }
        }


        protected override void OnServiceUI() {
           
        }


    }

}