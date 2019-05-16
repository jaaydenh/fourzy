using UnityEngine;
using UnityEditor;
using SA.Foundation.Editor;
using SA.iOS.UIKit;


namespace SA.iOS
{
    public class ISN_UIKitUI : ISN_ServiceSettingsUI
    {


        readonly GUIContent PhotoLibraryUsageDescription = new GUIContent("Photo Library Usage Description[?]:", "Describes the reason that the app accesses the device’s photo library. When the system prompts the user to allow access, this string is displayed as part of the alert.");
        readonly GUIContent CameraUsageDescription = new GUIContent("Camera Usage Description[?]:", "Describes the reason that the app accesses the device’s camera. When the system prompts the user to allow access, this string is displayed as part of the alert.");
        readonly GUIContent PhotoLibraryAddUsageDescription = new GUIContent("Photo Library Add Usage Description[?]:", "Describes the reason for the app to add content to the device’s photo library. When the system prompts the user to allow access, this string is displayed as part of the alert.");
        readonly GUIContent MicrophoneUsageDescription = new GUIContent("Microphone Usage Description[?]:", "Describes the reason that the app accesses the device’s microphone. When the system prompts the user to allow access, this string is displayed as part of the alert.");

        private SA_PluginActiveTextLink m_privacyLink;

        public override void OnAwake() {
            base.OnAwake();

            m_privacyLink = new SA_PluginActiveTextLink("[?] Read More");

            AddFeatureUrl("Native Pop-ups", "https://unionassets.com/ios-native-pro/popups-preloaders-619");
            AddFeatureUrl("Native Preloader", "https://unionassets.com/ios-native-pro/popups-preloaders-619#preloader");
            AddFeatureUrl("Device Info", "https://unionassets.com/ios-native-pro/device-info-620");
            AddFeatureUrl("Application Badges", "https://unionassets.com/ios-native-pro/applicationicon-badge-number-609");
            AddFeatureUrl("Identifier For Vendor", "https://unionassets.com/ios-native-pro/device-info-620#identifier-for-vendor");
            AddFeatureUrl("User Interface Idiom", "https://unionassets.com/ios-native-pro/device-info-620#user-interface-idiom");

           
            AddFeatureUrl("Time Picker", "https://unionassets.com/ios-native-pro/date-time-picker-604#time-picker");
            AddFeatureUrl("Date Picker", "https://unionassets.com/ios-native-pro/date-time-picker-604#date-picker");
            AddFeatureUrl("Date & Time Picker", "https://unionassets.com/ios-native-pro/date-time-picker-604");
            AddFeatureUrl("Countdown Timer", "https://unionassets.com/ios-native-pro/date-time-picker-604#countdown-timer");

            AddFeatureUrl("Calendar Date Picker", "https://unionassets.com/ios-native-pro/calendar-605");
            AddFeatureUrl("Save to Camera Roll", "https://unionassets.com/ios-native-pro/save-to-camera-roll-622");

            
            AddFeatureUrl("URL Schemes", "https://unionassets.com/ios-native-pro/url-queries-schemes-607");
            AddFeatureUrl("Add URL Scheme", "https://unionassets.com/ios-native-pro/url-queries-schemes-607#registering%C2%A0url-schemes");
            AddFeatureUrl("Check if App installed", "https://unionassets.com/ios-native-pro/url-queries-schemes-607#how-to-check-programmatically-if-an-app-is-installed");
           
            AddFeatureUrl("Open URL", "https://unionassets.com/ios-native-pro/open-url-829");
            AddFeatureUrl("App Settings Page", "https://unionassets.com/ios-native-pro/open-url-829#open-app-settings-page");
            AddFeatureUrl("iOS System URLs ", "https://unionassets.com/ios-native-pro/open-url-829#ios-system-urls");


            AddFeatureUrl("Img Picker Controller", "https://unionassets.com/ios-native-pro/uiimage-picker-controller-621");
            AddFeatureUrl("Pick an Image", "https://unionassets.com/ios-native-pro/get-image-or-video-from-albut-624#pick-an-image-from-photo-library");
            AddFeatureUrl("Capture an Image", "https://unionassets.com/ios-native-pro/get-image-or-video-from-albut-624#capture-an-image-from-camera");
            AddFeatureUrl("Pick a Video", "https://unionassets.com/ios-native-pro/capture-image-from-camera-625#pick-a-video-from-photo-library");
            AddFeatureUrl("Capture a Video", "https://unionassets.com/ios-native-pro/capture-image-from-camera-625#capture-a-video-from-camera");
        }

        public override string Title {
            get {
                return "UIKit";
            }
        }

        public override string Description {
            get {
                return "Construct and manage a graphical, event-driven user interface for your app.";
            }
        }

        protected override Texture2D Icon {
            get {
               return  SA_EditorAssets.GetTextureAtPath(ISN_Skin.ICONS_PATH + "UIKit_icon.png");
            }
        }

        public override SA_iAPIResolver Resolver {
            get {
                return ISN_Preprocessor.GetResolver<ISN_UIKitResolver>();
            }
        }

        protected override bool CanBeDisabled {
            get {
                return false;
            }
        }

        protected override void OnServiceUI() {


            using (new SA_WindowBlockWithSpace(new GUIContent("Protecting the User's Privacy"))) {
                EditorGUILayout.HelpBox("Once you link with iOS 10 you must declare access to any user private data types. " +
                                           "Since by default Unity includes libraries that may access API user private data, " +
                                           "the app info.plist mus contains the key's spsifayed bellow. " +
                                           "How ever user will only see this message if you call API that requires private permission. " +
                                           "If you not using such API, you can leave it as is.", MessageType.Info);



                using (new SA_GuiBeginHorizontal()) {
                    GUILayout.FlexibleSpace();
                    bool click = m_privacyLink.DrawWithCalcSize();
                    if (click) {
                        Application.OpenURL("https://developer.apple.com/documentation/uikit/core_app/protecting_the_user_s_privacy?language=objc");
                    }
                }

                ISN_Settings.Instance.CameraUsageDescriptionEnabled = SA_EditorGUILayout.ToggleFiled(CameraUsageDescription, ISN_Settings.Instance.CameraUsageDescriptionEnabled, SA_StyledToggle.ToggleType.EnabledDisabled);
                using (new SA_GuiEnable(ISN_Settings.Instance.CameraUsageDescriptionEnabled)) {
                    using (new SA_GuiIndentLevel(1)) {
                        ISN_Settings.Instance.CameraUsageDescription = EditorGUILayout.TextArea(ISN_Settings.Instance.CameraUsageDescription, SA_PluginSettingsWindowStyles.TextArea, GUILayout.Height(30));
                    }
                }
                  
                      

                EditorGUILayout.Space();
                ISN_Settings.Instance.PhotoLibraryUsageDescriptionEnabled = SA_EditorGUILayout.ToggleFiled(PhotoLibraryUsageDescription, ISN_Settings.Instance.PhotoLibraryUsageDescriptionEnabled, SA_StyledToggle.ToggleType.EnabledDisabled);
                using (new SA_GuiEnable(ISN_Settings.Instance.PhotoLibraryUsageDescriptionEnabled)) {
                    using (new SA_GuiIndentLevel(1)) {
                        ISN_Settings.Instance.PhotoLibraryUsageDescription = EditorGUILayout.TextArea(ISN_Settings.Instance.PhotoLibraryUsageDescription, SA_PluginSettingsWindowStyles.TextArea, GUILayout.Height(30));
                    }
                }

   

                EditorGUILayout.Space();
                ISN_Settings.Instance.PhotoLibraryAddUsageDescriptionEnabled = SA_EditorGUILayout.ToggleFiled(PhotoLibraryAddUsageDescription, ISN_Settings.Instance.PhotoLibraryAddUsageDescriptionEnabled, SA_StyledToggle.ToggleType.EnabledDisabled);
                using (new SA_GuiEnable(ISN_Settings.Instance.PhotoLibraryAddUsageDescriptionEnabled)) {
                    using (new SA_GuiIndentLevel(1)) {
                        ISN_Settings.Instance.PhotoLibraryAddUsageDescription = EditorGUILayout.TextArea(ISN_Settings.Instance.PhotoLibraryAddUsageDescription, SA_PluginSettingsWindowStyles.TextArea, GUILayout.Height(30));
                    }
                }


                EditorGUILayout.Space();
                ISN_Settings.Instance.MicrophoneUsageDescriptionEnabled = SA_EditorGUILayout.ToggleFiled(MicrophoneUsageDescription, ISN_Settings.Instance.MicrophoneUsageDescriptionEnabled, SA_StyledToggle.ToggleType.EnabledDisabled);
                using (new SA_GuiEnable(ISN_Settings.Instance.MicrophoneUsageDescriptionEnabled)) {
                    using (new SA_GuiIndentLevel(1)) {
                        ISN_Settings.Instance.MicrophoneUsageDescription = EditorGUILayout.TextArea(ISN_Settings.Instance.MicrophoneUsageDescription, SA_PluginSettingsWindowStyles.TextArea, GUILayout.Height(30));
                    }
                }
            }



            using (new SA_WindowBlockWithSpace(new GUIContent("Allowed schemes to query"))) {
                SA_EditorGUILayout.ReorderablList(ISN_Settings.Instance.ApplicationQueriesSchemes,
                  (ISN_UIUrlType scheme) => {
                      return scheme.Identifier;
                  },
                  (ISN_UIUrlType scheme) => {

                      EditorGUILayout.BeginHorizontal();
                      EditorGUILayout.LabelField("Identifier");
                      scheme.Identifier = EditorGUILayout.TextField(scheme.Identifier);
                      EditorGUILayout.EndHorizontal();
                  },
                  () => {
                      ISN_UIUrlType newUlr = new ISN_UIUrlType("url_sheme");
                      ISN_Settings.Instance.ApplicationQueriesSchemes.Add(newUlr);
                  });

            }
        }


    }
}