using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace SA.Foundation.Editor
{
    [Serializable]
    public class SA_H2WindowBlockWithSpace : IDisposable
    {

        private int m_indentLevel;

        public SA_H2WindowBlockWithSpace(GUIContent header) {
            if(header.image != null) {
                header.text = " " + header.text;
            }
            using (new SA_GuiBeginHorizontal()) {
                GUILayout.Space(10);
                EditorGUILayout.LabelField(header, SA_PluginSettingsWindowStyles.ServiceBlockHeader2);
            }
          //  GUILayout.Space(5);

            m_indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            EditorGUILayout.BeginVertical();

        }

        public void Dispose() {

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel = m_indentLevel;

            GUILayout.Space(5);
        }
    }
}



