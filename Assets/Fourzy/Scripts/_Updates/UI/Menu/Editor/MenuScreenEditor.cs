//@vadym udod

using UnityEditor;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    [CustomEditor(typeof(MenuScreen), true)]
    public class MenuScreenEditor : Editor
    {
        private MenuScreen trigger;
        private bool state;

        private SerializedProperty onDefaultOpen;
        private SerializedProperty onDefaultClose;

        public void OnEnable()
        {
            trigger = target as MenuScreen;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            state = EditorGUILayout.Foldout(state, "Defaults");

            if (state)
            {
                trigger.defaultCalls = EditorGUILayout.Toggle("Make default calls?", trigger.defaultCalls);
                trigger.@default = EditorGUILayout.Toggle("Is default screen", trigger.@default);
            }
        }
    }
}
