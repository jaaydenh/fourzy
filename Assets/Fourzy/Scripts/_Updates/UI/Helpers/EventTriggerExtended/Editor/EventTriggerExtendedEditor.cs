//@vadym udod

using UnityEditor;
using UnityEditor.EventSystems;
using UnityEngine;

namespace Fourzy._Updates.UI.Helpers
{
    [CustomEditor(typeof(EventTriggerExtended))]
    public class EventTriggerExtendedEditor : EventTriggerEditor
    {
        private SerializedProperty onSelectedProperty;
        private SerializedProperty onDeselectedProperty;
        private SerializedProperty adjustSelectableOnSelectProperty;
        private SerializedProperty onlyActiveSelectablesProperty;
        private SerializedProperty customSizeProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

            onSelectedProperty = serializedObject.FindProperty("onSelect");
            onDeselectedProperty = serializedObject.FindProperty("onDeselect");
            adjustSelectableOnSelectProperty = serializedObject.FindProperty("adjustSelectableOnSelect");
            onlyActiveSelectablesProperty = serializedObject.FindProperty("onlyActiveSelectables");
            customSizeProperty = serializedObject.FindProperty("customSize");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(onSelectedProperty);
            EditorGUILayout.PropertyField(onDeselectedProperty);
            EditorGUILayout.PropertyField(adjustSelectableOnSelectProperty);
            EditorGUILayout.PropertyField(onlyActiveSelectablesProperty);
            EditorGUILayout.PropertyField(customSizeProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}