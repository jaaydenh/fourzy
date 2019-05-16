using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SA.Foundation.Utility;


namespace SA.Foundation.Editor
{
    public class SA_EditorStyles 
    {

        public static GUIStyle PreferencesSectionBox = "PreferencesSectionBox";
        public static GUIStyle OLTitle = "OL Title";
        public static GUIStyle OLBox = "OL Box";
        public static GUIStyle WordWrappedLabel = "WordWrappedLabel";
        public static GUIStyle PreferencesSection = "PreferencesSection";
        public static GUIStyle EntryBackEven = "CN EntryBackEven";
        public static GUIStyle EntryBackOdd = "CN EntryBackOdd";
        public static GUIStyle PreferencesKeysElement = "PreferencesKeysElement";
        public static GUIStyle EntryWarn = "CN EntryWarn";


        public GUIStyle SectionHeader;
        public GUIStyle CacheFolderLocation;
        public GUIStyle ToolbarStyle;
        public GUIStyle ToolbarSeachTextFieldStyle;
        public GUIStyle ToolbarSeachCancelButtonStyle;

        public SA_EditorStyles() {
            SectionHeader = new GUIStyle(EditorStyles.largeLabel);

            PreferencesSectionBox = new GUIStyle(PreferencesSectionBox);
            PreferencesSectionBox.overflow.bottom++;

            ToolbarStyle = GUI.skin.FindStyle("Toolbar");
            ToolbarSeachTextFieldStyle = GUI.skin.FindStyle("ToolbarSeachTextField");
            ToolbarSeachCancelButtonStyle = GUI.skin.FindStyle("ToolbarSeachCancelButton");

            SectionHeader.fontStyle = FontStyle.Bold;
            SectionHeader.fontSize = 18;
            SectionHeader.margin.top = 10;
            SectionHeader.margin.left++;

            if (!EditorGUIUtility.isProSkin) {
                SectionHeader.normal.textColor = new Color(0.4f, 0.4f, 0.4f, 1f);
            } else {
                SectionHeader.normal.textColor = new Color(0.7f, 0.7f, 0.7f, 1f);
            }


            CacheFolderLocation = new GUIStyle(GUI.skin.label);
            CacheFolderLocation.wordWrap = true;
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




        private static SA_EditorStyles m_instance = null;
        public static SA_EditorStyles Collection {
            get {
                if(m_instance == null) {
                    m_instance = new SA_EditorStyles();
                }

                return m_instance;
            }
        }

    }
}