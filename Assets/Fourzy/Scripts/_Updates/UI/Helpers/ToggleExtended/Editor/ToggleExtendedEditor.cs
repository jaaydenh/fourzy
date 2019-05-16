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
        private SerializedProperty onClickProperty;

        private SerializedProperty onSfxProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            onStateProperty = serializedObject.FindProperty("onState");
            offStateProperty = serializedObject.FindProperty("offState");
            stateProperty = serializedObject.FindProperty("state");
            onClickProperty = serializedObject.FindProperty("onClick");

            onSfxProperty = serializedObject.FindProperty("onSfx");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.PropertyField(onStateProperty);
            EditorGUILayout.PropertyField(offStateProperty);
            EditorGUILayout.PropertyField(stateProperty);
            EditorGUILayout.PropertyField(onClickProperty);

            EditorGUILayout.PropertyField(onSfxProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
