//@vadym udod

using UnityEditor;
using UnityEditor.UI;

namespace Fourzy._Updates.UI.Helpers
{
    [CustomEditor(typeof(SliderExtended), true)]
    public class SliderExtendedEditor : SliderEditor
    {
        private SerializedProperty onValueProperty;
        private SerializedProperty formatProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            onValueProperty = serializedObject.FindProperty("onValueChangeString");
            formatProperty = serializedObject.FindProperty("format");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.PropertyField(onValueProperty);
            EditorGUILayout.PropertyField(formatProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}