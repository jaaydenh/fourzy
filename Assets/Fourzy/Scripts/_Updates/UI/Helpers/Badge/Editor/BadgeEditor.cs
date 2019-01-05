//@vadym udod

using UnityEditor;

namespace Fourzy._Updates.UI.Helpers
{
    [CustomEditor(typeof(Badge))]
    public class BadgeEditor : Editor
    {
        private SerializedProperty targetObject;
        private SerializedProperty targetText;

        private Badge trigger;

        protected void OnEnable()
        {
            targetObject = serializedObject.FindProperty("targetObject");
            targetText = serializedObject.FindProperty("targetText");

            trigger = target as Badge;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!trigger.thisTarget)
            {
                EditorGUILayout.PropertyField(targetObject);
                EditorGUILayout.PropertyField(targetText);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}