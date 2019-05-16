using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SA.Foundation.Config;

namespace SA.Foundation.Editor
{
    /// <summary>
    /// Contains common styles and images for Stan's Assets Editor UI's
    /// </summary>
    public static class SA_Skin
    {



        public const string ABOUT_ICONS_PATH = SA_Config.STANS_ASSETS_EDITOR_ICONS + "About/";
        public const string GENERIC_ICONS_PATH = SA_Config.STANS_ASSETS_EDITOR_ICONS + "Generic/";
        public const string SOCIAL_ICONS_PATH = SA_Config.STANS_ASSETS_EDITOR_ICONS + "Social/";


        public static Texture2D GetAboutIcon(string iconName) {
            return SA_EditorAssets.GetTextureAtPath(ABOUT_ICONS_PATH + iconName);
        }

        public static Texture2D GetGenericIcon(string iconName) {
            return SA_EditorAssets.GetTextureAtPath(GENERIC_ICONS_PATH + iconName);
        }

        public static Texture2D GetSocialIcon(string iconName) {
            return SA_EditorAssets.GetTextureAtPath(SOCIAL_ICONS_PATH + iconName);
        }


        private static GUIStyle s_boxStyle = null;
        public static GUIStyle BoxStyle {
            get {
                if (s_boxStyle == null) {
                    s_boxStyle = new GUIStyle(GUI.skin.box);
                }

                return s_boxStyle;
            }
        }


        private static GUIStyle s_labelBold = null;
        public static GUIStyle LabelBold {
            get {
                if (s_labelBold == null) {
                    s_labelBold = new GUIStyle(EditorStyles.label);
                    s_labelBold.fontStyle = FontStyle.Bold;
                }

                return s_labelBold;
            }
        }

        private static GUIStyle s_miniLabel = null;
        public static GUIStyle MiniLabelWordWrap {
            get {
                if (s_miniLabel == null) {
                    s_miniLabel = new GUIStyle(EditorStyles.miniLabel);
                    s_miniLabel.wordWrap = true;
                }

                return s_miniLabel;
            }
        }


        

    }
}