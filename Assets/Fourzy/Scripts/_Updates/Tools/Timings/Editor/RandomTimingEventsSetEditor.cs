//@vadym udod

using UnityEditor;
using UnityEngine;

namespace Fourzy._Updates.Tools.Timing
{
    [CustomEditor(typeof(RandomTimingEventsSet)), CanEditMultipleObjects]
    public class RandomTimingEventsSetEditor : Editor
    {
        private SerializedProperty from;
        private SerializedProperty to;
        private SerializedProperty offsetFrom;
        private SerializedProperty offsetTo;

        private float fromFloat;
        private float toFloat;
        private float offsetFromFloat;
        private float offsetToFloat;

        void OnEnable()
        {
            from = serializedObject.FindProperty("from");
            to = serializedObject.FindProperty("to");
            offsetFrom = serializedObject.FindProperty("offsetFrom");
            offsetTo = serializedObject.FindProperty("offsetTo");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(from);
            EditorGUILayout.PropertyField(to);

            fromFloat = from.floatValue;
            toFloat = to.floatValue;

            EditorGUILayout.MinMaxSlider(new GUIContent("Set timing range"), ref fromFloat, ref toFloat, .001f, 50f);

            EditorGUILayout.PropertyField(offsetFrom);
            EditorGUILayout.PropertyField(offsetTo);

            offsetFromFloat = offsetFrom.floatValue;
            offsetToFloat = offsetTo.floatValue;

            EditorGUILayout.MinMaxSlider(new GUIContent("Set offset range"), ref offsetFromFloat, ref offsetToFloat, .001f, 50f);

            from.floatValue = fromFloat;
            to.floatValue = toFloat;
            offsetFrom.floatValue = offsetFromFloat;
            offsetTo.floatValue = offsetToFloat;

            serializedObject.ApplyModifiedProperties();

            DrawDefaultInspector();
        }
    }
}