
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;


using SA.Foundation.Config;
using SA.Foundation.Utility;

namespace SA.Foundation.Editor
{
    [Serializable]
    public class SA_AnimatedFoldoutBlock
    {

        [SerializeField] GUIContent m_header;
        [SerializeField] AnimBool m_showExtraFields = new AnimBool(false);


        public SA_AnimatedFoldoutBlock(GUIContent header) {
            if (header.image != null) {
                header.text = " " + header.text;
            }
            m_header = header;

        }

        protected virtual void OnAfterHeaderGUI() {

        }

        public void OnGUI(Action OnContentRender) {

            using (new SA_GuiBeginHorizontal()) {
                m_showExtraFields.target = EditorGUILayout.Foldout(m_showExtraFields.target, m_header, true);
            }

            using (new SA_GuiHorizontalSpace(15)) {
                if (EditorGUILayout.BeginFadeGroup(m_showExtraFields.faded)) {
                    OnContentRender.Invoke();
                }
                EditorGUILayout.EndFadeGroup();
            }
        }

    }
}