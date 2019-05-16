using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace SA.Foundation.Editor
{
    [Serializable]
    public class SA_WindowBlockWithSpace : IDisposable
    {
        private int m_indentLevel;

        public SA_WindowBlockWithSpace(string header, int space = 15):this(new GUIContent(header), space)  { }
        public SA_WindowBlockWithSpace(GUIContent header, int space = 15) {
            if (header.image != null) {
                header.text = " " + header.text;
            }
            GUILayout.Space(10);
            using (new SA_GuiBeginHorizontal()) {
                GUILayout.Space(10);
                EditorGUILayout.LabelField(header, SA_PluginSettingsWindowStyles.ServiceBlockHeader);
            }
            GUILayout.Space(5);

            m_indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(space);
            EditorGUILayout.BeginVertical();
        }

        public void Dispose() {

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel = m_indentLevel;

            GUILayout.Space(10);
            EditorGUILayout.BeginVertical(SA_PluginSettingsWindowStyles.SeparationStyle);
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
        }
    }
}



