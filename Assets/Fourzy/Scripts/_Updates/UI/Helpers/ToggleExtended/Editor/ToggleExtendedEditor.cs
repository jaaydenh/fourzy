//@vadym udod

using UnityEditor;
using UnityEditor.UI;

namespace Fourzy._Updates.UI.Helpers
{
    [CustomEditor(typeof(ToggleExtended), true)]
    public class ToggleExtendedEditor : ToggleEditor
    {
        private SerializedProperty onStateProperty;
        private SerializedProperty offStateProperty;
        private SerializedProperty stateProperty;

        private SerializedProperty onSfxProperty;
        private SerializedProperty offSfxProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            onStateProperty = serializedObject.FindProperty("onState");
            offStateProperty = serializedObject.FindProperty("offState");
            stateProperty = serializedObject.FindProperty("state");

            onSfxProperty = serializedObject.FindProperty("onSfx");
            offSfxProperty = serializedObject.FindProperty("offSfx");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.PropertyField(onStateProperty);
            EditorGUILayout.PropertyField(offStateProperty);
            EditorGUILayout.PropertyField(stateProperty);

            EditorGUILayout.PropertyField(onSfxProperty);
            EditorGUILayout.PropertyField(offSfxProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
