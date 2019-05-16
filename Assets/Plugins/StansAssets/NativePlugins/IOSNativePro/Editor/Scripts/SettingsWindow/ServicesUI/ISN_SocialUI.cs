using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SA.Foundation.Editor;


namespace SA.iOS
{
    public class ISN_SocialUI : ISN_ServiceSettingsUI
    {

        public override void OnAwake() {
            base.OnAwake();

            AddFeatureUrl("Facebook", "https://unionassets.com/ios-native-pro/facebook-644");
            AddFeatureUrl("Twitter", "https://unionassets.com/ios-native-pro/twitter-648");
            AddFeatureUrl("Instagram", "https://unionassets.com/ios-native-pro/instagram-649");
            AddFeatureUrl("E-Mail", "https://unionassets.com/ios-native-pro/e-mail-651");
            AddFeatureUrl("WhatsApp", "https://unionassets.com/ios-native-pro/whats-650");
            AddFeatureUrl("Text Message", "https://unionassets.com/ios-native-pro/text-message-652");
            AddFeatureUrl("Default Sharing Dialog", "https://unionassets.com/ios-native-pro/default-sharing-dialog-653");
        }

        public override string Title {
            get {
                return "Social";
            }
        }

        public override string Description {
            get {
                return "Integrate your app with supported social networking services.";
            }
        }

        protected override Texture2D Icon {
            get {
               return  SA_EditorAssets.GetTextureAtPath(ISN_Skin.ICONS_PATH + "Social_icon.png");
            }
        }

        public override SA_iAPIResolver Resolver {
            get {
                return ISN_Preprocessor.GetResolver<ISN_SocialResolver>();
            }
        }


        protected override IEnumerable<string> SupportedPlatforms {
            get {
                return new List<string>() { "iOS", "Unity Editor" };
            }
        }


        protected override void OnServiceUI() {
              
        }

    }
}