using UnityEngine;
using System.Collections.Generic;

using SA.Foundation.Config;
using SA.Foundation.Patterns;

namespace SA.iOS
{
    public class ISN_EditorSettings : SA_ScriptableSingletonEditor<ISN_EditorSettings>
    {
        public List<AudioClip> NotificationAlertSounds = new List<AudioClip>();


        //--------------------------------------
        // SA_ScriptableSettings
        //--------------------------------------

        protected override string BasePath {
            get { return ISN_Settings.IOS_NATIVE_FOLDER; }
        }


        public override string PluginName {
            get {
                return ISN_Settings.Instance.PluginName + " Editor";
            }
        }

        public override string DocumentationURL {
            get {
                return ISN_Settings.Instance.DocumentationURL;
            }
        }


        public override string SettingsUIMenuItem {
            get {
                return ISN_Settings.Instance.SettingsUIMenuItem;
            }
        }
    }
}