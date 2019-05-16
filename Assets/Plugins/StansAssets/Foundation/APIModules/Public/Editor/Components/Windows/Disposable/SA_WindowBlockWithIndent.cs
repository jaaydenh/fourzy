using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace SA.Foundation.Editor
{
    [Serializable]
    public class SA_WindowBlockWithIndent : IDisposable
    {

        public SA_WindowBlockWithIndent(GUIContent header) {
            if(header.image != null) {
                header.text = " " + header.text;
            }
            GUILayout.Space(10);
            using (new SA_GuiBeginHorizontal()) {
                GUILayout.Space(10);
                EditorGUILayout.LabelField(header, SA_PluginSettingsWindowStyles.ServiceBlockHeader);
            }
            GUILayout.Space(5);

            EditorGUI.indentLevel++;

        }

        public void Dispose() {
            EditorGUI.indentLevel--;
            GUILayout.Space(5);
            EditorGUILayout.BeginVertical(SA_PluginSettingsWindowStyles.SeparationStyle);
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
        }
    }
}