using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SA.Foundation.Utility;
using SA.Foundation.Config;


namespace SA.Foundation.Editor
{
    public static class SA_PluginSettingsWindowStyles
    {

        public const int INDENT_PIXEL_SIZE = 13;
        public const int LAYOUT_PADDING = 5;

        private static GUIStyle m_separationStyle = null;
        public static GUIStyle SeparationStyle {
            get {
                if (m_separationStyle == null) {
                    m_separationStyle = new GUIStyle();
                }

                if (m_separationStyle.normal.background == null) {
                    if (EditorGUIUtility.isProSkin) {
                        m_separationStyle.normal.background = SA_IconManager.GetIconFromHtmlColorString("#292929FF");
                    } else {
                        m_separationStyle.normal.background = SA_IconManager.GetIconFromHtmlColorString("#A2A2A2FF");
                    }
                }
                return m_separationStyle;
            }
        }



        private static GUIStyle m_labelServiceBlockStyle = null;
        public static GUIStyle LabelServiceBlockStyle {
            get {
                if (m_labelServiceBlockStyle == null) {
                    m_labelServiceBlockStyle = new GUIStyle(EditorStyles.miniLabel);
                    m_labelServiceBlockStyle.fontSize = 18;
                    m_labelServiceBlockStyle.padding = new RectOffset();
                    m_labelServiceBlockStyle.richText = true;

                    m_labelServiceBlockStyle.font = SA_EditorAssets.GetFontAtPath(SA_Config.STANS_ASSETS_EDITOR_FONTS + "Raleway-Light.ttf");
                    if (EditorGUIUtility.isProSkin) {
                        m_labelServiceBlockStyle.normal.textColor = Color.white;
                    } else {
                        m_labelServiceBlockStyle.normal.textColor = Color.black;
                    }
                }

                return m_labelServiceBlockStyle;
            }
        }


        private static GUIStyle m_labelHeaderStyle = null;
        public static GUIStyle LabelHeaderStyle {
            get {
                if (m_labelHeaderStyle == null) {
                    m_labelHeaderStyle = new GUIStyle();
                    m_labelHeaderStyle.fontSize = 18;
                    m_labelHeaderStyle.fontStyle = FontStyle.Bold;
                }

                if (EditorGUIUtility.isProSkin) {
                    m_labelHeaderStyle.normal.textColor = SA_IconManager.GetColorFromHtml("#F8F8F8FF");
                }



                return m_labelHeaderStyle;
            }
        }


        private static GUIStyle m_describtionLabelStyle = null;
        public static GUIStyle DescribtionLabelStyle {
            get {
                if (m_describtionLabelStyle == null) {

                    m_describtionLabelStyle = new GUIStyle();
                    m_describtionLabelStyle.wordWrap = true;
                }

                if (EditorGUIUtility.isProSkin) {
                    m_describtionLabelStyle.normal.textColor = SA_IconManager.GetColorFromHtml("#959995FF");
                }

                return m_describtionLabelStyle;
            }
        }

        private static GUIStyle m_versionLabelStyle = null;
        public static GUIStyle VersionLabelStyle {
            get {
                if (m_versionLabelStyle == null) {
                    m_versionLabelStyle = new GUIStyle(DescribtionLabelStyle);
                }

                m_versionLabelStyle.alignment = TextAnchor.MiddleRight;


                return m_versionLabelStyle;
            }
        }

        private static GUIStyle m_selectebleLabelStyle = null;
        public static GUIStyle SelectebleLabelStyle {
            get {
                if (m_selectebleLabelStyle == null) {

                    m_selectebleLabelStyle = new GUIStyle();
                    m_selectebleLabelStyle.wordWrap = true;
                }

                if (EditorGUIUtility.isProSkin) {
                    m_selectebleLabelStyle.normal.textColor = SA_IconManager.GetColorFromHtml("#DFDFDFFF");
                } else {
                    m_selectebleLabelStyle.normal.textColor = SA_IconManager.GetColorFromHtml("#0054C7ED");
                }
                return m_selectebleLabelStyle;
            }
        }

        private static GUIStyle m_serviceBlockHeader = null;
        public static GUIStyle ServiceBlockHeader {
            get {
                if (m_serviceBlockHeader == null) {
                    m_serviceBlockHeader = new GUIStyle();
                    m_serviceBlockHeader.fontSize = 13;
                    m_serviceBlockHeader.fontStyle = FontStyle.Bold;

                    m_serviceBlockHeader.normal.textColor = DisabledImageColor;
                }

                return m_serviceBlockHeader;
            }
        }


        private static GUIStyle m_serviceBlockHeader2 = null;
        public static GUIStyle ServiceBlockHeader2 {
            get {
                if (m_serviceBlockHeader2 == null) {
                    m_serviceBlockHeader2 = new GUIStyle(EditorStyles.boldLabel);
                    m_serviceBlockHeader2.alignment = TextAnchor.MiddleLeft;
                    m_serviceBlockHeader2.fontSize = 10;
                }

                return m_serviceBlockHeader2;
            }
        }


        private static GUIStyle m_assetLabel = null;
        public static GUIStyle AssetLabel {
            get {
                if (m_assetLabel == null) {
                    m_assetLabel = new GUIStyle(GUI.skin.GetStyle("AssetLabel"));
                }
                return m_assetLabel;
            }
        }

        private static GUIStyle m_textArea = null;
        public static GUIStyle TextArea {
            get {
                if (m_textArea == null) {
                    m_textArea = new GUIStyle(EditorStyles.textArea);
                    m_textArea.wordWrap = true;
                }

                return m_textArea;
            }
        }


        public static Color ActiveLinkColor {
            get {
                if (EditorGUIUtility.isProSkin) {
                    return SA_IconManager.GetColorFromHtml("#1B97F2");
                } else {
                    return SA_IconManager.GetColorFromHtml("#3066B3");
                }
            }
        }

        public static Color SelectedActiveLinkColor {
            get {
                if (EditorGUIUtility.isProSkin) {
                    return SA_IconManager.GetColorFromHtml("#1BE1F2ED");
                } else {
                    return SA_IconManager.GetColorFromHtml("#417BCD");
                }
            }
        }


        


        public static Color SelectedElementColor {
            get {
                if (EditorGUIUtility.isProSkin) {
                    return SA_IconManager.GetColorFromHtml("#1BE1F2ED");
                } else {
                    // SA_IconManager.GetColorFromHtml("#3AA3B2ED");
                    return SA_IconManager.GetColorFromHtml("#5CBFCD");
                }
            }
        }




        public static Color SelectedImageColor {
            get {
                if (EditorGUIUtility.isProSkin) {
                    return SA_IconManager.GetColorFromHtml("#0CB4CCED");
                } else {
                    return SA_IconManager.GetColorFromHtml("#018D98ED");
                }
            }
        }


        public static Color DisabledImageColor {
            get {
                if (EditorGUIUtility.isProSkin) {
                    return ProDisabledImageColor;
                } else {
                    return SA_IconManager.GetColorFromHtml("#3C3C3CFF");
                }
            }
        }


        public static Color ProDisabledImageColor {
            get {
                return SA_IconManager.GetColorFromHtml("#999999ED");
            }
        }


        public static Color GeryGainsboroColor {
            get {
                return SA_IconManager.GetColorFromHtml("#DCDCDC");
            }
        }

        public static Color GerySilverColor {
            get {
                return SA_IconManager.GetColorFromHtml("#C0C0C0");
            }
        }


        



        public static Color DefaultImageContentColor {
            get {
                if (EditorGUIUtility.isProSkin) {
                    return SA_IconManager.GetColorFromHtml("#B4B4B4FF");
                } else {
                    return Color.black;
                }
            }
        }
       
    }
}