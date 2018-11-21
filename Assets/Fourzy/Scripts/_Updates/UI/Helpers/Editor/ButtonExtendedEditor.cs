//@vadym udod

using UnityEditor;
using UnityEditor.UI;

namespace Fourzy._Updates.UI.Menu.Helpers
{
    [CustomEditor(typeof(ButtonExtended))]
    public class ButtonExtendedEditor : ButtonEditor
    {
        private SerializedProperty eventProperty;
        private SerializedProperty sfxProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            eventProperty = serializedObject.FindProperty("events");
            sfxProperty = serializedObject.FindProperty("playOnClick");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.PropertyField(eventProperty);
            EditorGUILayout.PropertyField(sfxProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
