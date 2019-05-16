using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using SA.Foundation.Editor;
using SA.Foundation.Utility;
using SA.Foundation.UtilitiesEditor;
using SA.iOS.XCode;

namespace SA.iOS
{
    [Serializable]
    public class ISN_XCodeTab : SA_GUILayoutElement
    {
        [SerializeField] SA_HyperToolbar m_menuToolbar;
        [SerializeField] List<SA_GUILayoutElement> m_tabsLayout = new List<SA_GUILayoutElement>();

        public override void OnAwake() {
            m_tabsLayout = new List<SA_GUILayoutElement>();
            m_menuToolbar = new SA_HyperToolbar();

            AddMenuItem("GENERAL", CreateInstance<ISD_GeneralWindowTab>());
            AddMenuItem("COMPATIBILITIES", CreateInstance<ISD_CapabilitiesTab>());
            AddMenuItem("INFO.PLIST", CreateInstance<ISD_InfoPlistWindowTab>());
        }


        public override void OnLayoutEnable() {
            foreach(var tab in m_tabsLayout) {
                tab.OnLayoutEnable();
            }
        }

        private void AddMenuItem(string itemName, SA_GUILayoutElement layout) {
            var button = new SA_HyperLabel(new GUIContent(itemName), EditorStyles.boldLabel);
            button.SetMouseOverColor(SA_PluginSettingsWindowStyles.SelectedElementColor);
            m_menuToolbar.AddButtons(button);

            m_tabsLayout.Add(layout);
            layout.OnAwake();
        }

        public override void OnGUI() {

            EditorGUI.BeginChangeCheck();

            GUILayout.Space(2);
            int index = m_menuToolbar.Draw();
            GUILayout.Space(4);
            EditorGUILayout.BeginVertical(SA_PluginSettingsWindowStyles.SeparationStyle);
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();

            m_tabsLayout[index].OnGUI();

            if (EditorGUI.EndChangeCheck()) {
                ISD_Settings.Save();
            }
        }

    }


}