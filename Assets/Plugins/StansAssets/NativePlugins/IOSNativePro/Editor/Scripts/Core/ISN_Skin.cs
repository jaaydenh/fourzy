using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SA.Foundation.Editor;
using SA.Foundation.Utility;


namespace SA.iOS
{
    public static class ISN_Skin
    {

        public const string ICONS_PATH = ISN_Settings.IOS_NATIVE_FOLDER + "Editor/Art/Icons/";
        public const string SOCIAL_ICONS_PATH = ISN_Settings.IOS_NATIVE_FOLDER + "Editor/Art/Social/";

        public static Texture2D SettingsWindowIcon {
            get {
                if (EditorGUIUtility.isProSkin) {
                    return SA_EditorAssets.GetTextureAtPath(ICONS_PATH + "ios_pro.png");
                } else {
                    return SA_EditorAssets.GetTextureAtPath(ICONS_PATH + "ios.png");
                }
            }
        }

        public static Texture2D GetIcon(string iconName) {
            return SA_EditorAssets.GetTextureAtPath(ICONS_PATH + iconName);
        }
    }
}