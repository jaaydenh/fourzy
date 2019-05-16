using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SA.Foundation.Editor;
using SA.Foundation.Utility;

using SA.iOS.XCode;

namespace SA.iOS
{
    public abstract class ISN_ServiceSettingsUI : SA_ServiceLayout
    {
        protected override IEnumerable<string> SupportedPlatforms {
            get {
                return new List<string>() { "iOS", "tvOS", "Unity Editor" };
            }
        }

        protected override int IconSize {
            get {
                return 25;
            }
        }

        
        protected override int TitleVerticalSpace {
            get {
                return 2;
            }
        }

        protected override void DrawServiceRequirements() {

            var resolver = (ISN_APIResolver)Resolver;

            if (!HasRequirements(resolver)) {
                return;
            }


            using (new SA_WindowBlockWithSpace(new GUIContent("Requirements"))) {
                DrawRequirementsUI(resolver.XcodeRequirements);
            }
        }

        public static void DrawRequirementsUI(ISN_XcodeRequirements xcodeRequirements) {


            if(xcodeRequirements.Frameworks.Count > 0) {
                using (new SA_H2WindowBlockWithSpace(new GUIContent("FRAMEWORKS"))) {
                    foreach (var freamwork in xcodeRequirements.Frameworks) {
                        SA_EditorGUILayout.SelectableLabel(new GUIContent(freamwork.Type.ToString() + ".framework", ISD_Skin.GetIcon("frameworks.png")));
                    }
                }
            }


            if (xcodeRequirements.Libraries.Count > 0) {
                using (new SA_H2WindowBlockWithSpace(new GUIContent("LIBRARIES"))) {
                    foreach (var freamwork in xcodeRequirements.Libraries) {
                        SA_EditorGUILayout.SelectableLabel(new GUIContent(freamwork.Type.ToString() + ".framework", ISD_Skin.GetIcon("frameworks.png")));
                    }
                }
            }


            if (xcodeRequirements.Capabilities.Count > 0) {
                using (new SA_H2WindowBlockWithSpace(new GUIContent("CAPABILITIES"))) {
                    foreach (var capability in xcodeRequirements.Capabilities) {
                        SA_EditorGUILayout.SelectableLabel(new GUIContent(capability + " Capability", ISD_Skin.GetIcon("capability.png")));
                    }
                }
            }


            if (xcodeRequirements.PlistKeys.Count > 0) {
                using (new SA_H2WindowBlockWithSpace(new GUIContent("PLIST KEYS"))) {
                    foreach (var key in xcodeRequirements.PlistKeys) {
                        SA_EditorGUILayout.SelectableLabel(new GUIContent(key.Name, ISD_Skin.GetIcon("plistVariables.png")));
                    }
                }
            }

            if (xcodeRequirements.Properties.Count > 0) {
                using (new SA_H2WindowBlockWithSpace(new GUIContent("BUILD PROPERTIES"))) {
                    foreach (var property in xcodeRequirements.Properties) {
                        SA_EditorGUILayout.SelectableLabel(new GUIContent(property.Name + " | " + property.Value, ISD_Skin.GetIcon("buildSettings.png")));
                    }
                }
            }

        }


        protected bool HasRequirements(ISN_APIResolver resolver ) {
          
                return resolver.XcodeRequirements.Frameworks.Count > 0 ||
                               resolver.XcodeRequirements.Libraries.Count > 0 ||
                               resolver.XcodeRequirements.PlistKeys.Count > 0 ||
                               resolver.XcodeRequirements.Properties.Count > 0;
            
        }


    }
}