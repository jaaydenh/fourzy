using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SA.Foundation.Editor;

namespace SA.iOS.XCode
{

    public class ISD_SettingsWindow : SA_PluginSettingsWindow<ISD_SettingsWindow>
    {
   
        protected override void OnAwake() {
            SetHeaderTitle(ISD_Settings.PLUGIN_NAME);
            SetHeaderDescription("The plugin gives you an ability to work with Apple Native API. " +
                                 "Every module that has additional XCode requirement can be disabled. " +
                                 "Enable only modules you need for the current project.");
            SetHeaderVersion(ISD_Settings.FormattedVersion);
            SetDocumentationUrl(ISD_Settings.DOCUMENTATION_URL);

            
            AddMenuItem("GENERAL", CreateInstance<ISD_GeneralWindowTab>()  );
			AddMenuItem("COMPATIBILITIES", CreateInstance<ISD_CapabilitiesTab>());
            AddMenuItem("INFO", CreateInstance<ISD_InfoPlistWindowTab>());
            AddMenuItem("ABOUT", CreateInstance<SA_PluginAboutLayout>());

        }

		protected override void BeforeGUI() {
            EditorGUI.BeginChangeCheck();
        }


        protected override void AfterGUI() {
            if (EditorGUI.EndChangeCheck()) {
				ISD_Settings.Save();
            }
        }

    }
}