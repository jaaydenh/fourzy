
using UnityEditor;
using UnityEngine;


using SA.Foundation.Config;

namespace SA.iOS
{
    public static class ISN_EditorMenu
    {
        //--------------------------------------
        //  PUBLIC METHODS
        //--------------------------------------


        // WARNING: same menu item path is duplicated for settings UI.
        // if you need to change it here, make a proper config first.
        // do not change MenuItem path before you 100% what is mean by a statement above.

        [MenuItem(SA_Config.EDITOR_MENU_ROOT + "iOS/Services", false, 299)]
        public static void Services() {
            var window = ISN_SettingsWindow.ShowTowardsInspector(WindowTitle);
            window.SetSelectedTabIndex(0);
        }

        [MenuItem(SA_Config.EDITOR_MENU_ROOT + "iOS/XCode", false, 299)]
        public static void XCode() {
            var window = ISN_SettingsWindow.ShowTowardsInspector(WindowTitle);
            window.SetSelectedTabIndex(1);
        }

        [MenuItem(SA_Config.EDITOR_MENU_ROOT + "iOS/Settings", false, 299)]
        public static void Settings() {


            var window = ISN_SettingsWindow.ShowTowardsInspector(WindowTitle);
            window.SetSelectedTabIndex(2);
        }


        [MenuItem(SA_Config.EDITOR_MENU_ROOT + "iOS/Documentation", false, 300)]
        public static void ISDSetupPluginSetUp() {
            Application.OpenURL(ISN_Settings.DOCUMENTATION_URL);
        }


        private static GUIContent WindowTitle {
            get {
                return new GUIContent(ISN_Settings.PLUGIN_NAME, ISN_Skin.SettingsWindowIcon);
            }
        }


    }

}
