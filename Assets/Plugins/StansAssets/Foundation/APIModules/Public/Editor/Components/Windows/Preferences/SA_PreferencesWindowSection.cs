using System;
using System.Collections.Generic;
using UnityEngine;


namespace SA.Foundation.Editor
{

    [Serializable]
    public class SA_PreferencesWindowSection
    {

        [SerializeField] GUIContent m_content;
        [SerializeField] SA_GUILayoutElement m_layout;


        public SA_PreferencesWindowSection(string name, SA_GUILayoutElement layout) {
            m_content = new GUIContent(name);
            m_layout = layout;
        }

        public string Name {
            get {
                return m_content.text;
            }
        }

        public SA_GUILayoutElement Layout {
            get {
                return m_layout;
            }
        }

        public GUIContent Content {
            get {
                return m_content;
            }
        }
    }
}