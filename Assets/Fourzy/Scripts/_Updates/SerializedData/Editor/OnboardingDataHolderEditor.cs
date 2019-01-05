//@vadym udod

using Fourzy._Updates.Tools;
using UnityEditor;
using UnityEngine;

namespace Fourzy._Updates.Serialized
{
    [CustomEditor(typeof(OnboardingDataHolder))]
    public class OnboardingDataHolderEditor : Editor
    {
        protected OnboardingDataHolder trigger;

        private SerializedProperty tasksProperty;
        private SerializedProperty messageProperty;
        private SerializedProperty pointAtProperty;
        private SerializedProperty highlightProperty;

        protected void OnEnable()
        {
            trigger = target as OnboardingDataHolder;

            if (trigger.tasks == null)
                trigger.tasks = new OnboardingDataHolder.OnboardingTask[0];

            tasksProperty = serializedObject.FindProperty("tasks");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            for (int taskIndex = 0; taskIndex < tasksProperty.arraySize; taskIndex++)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    trigger.tasks[taskIndex].unfolded = EditorGUILayout.Foldout(trigger.tasks[taskIndex].unfolded, trigger.tasks[taskIndex].action.ToString());

                    if (GUILayout.Button("-", GUILayout.Width(50f)))
                        tasksProperty.DeleteArrayElementAtIndex(taskIndex);

                    if (taskIndex > 0)
                        if (GUILayout.Button("UP", GUILayout.Width(50f)))
                            tasksProperty.MoveArrayElement(taskIndex, taskIndex - 1);

                    if (taskIndex < tasksProperty.arraySize - 1)
                        if (GUILayout.Button("DOWN", GUILayout.Width(50f)))
                            tasksProperty.MoveArrayElement(taskIndex, taskIndex + 1);
                }
                EditorGUILayout.EndHorizontal();

                if (trigger.tasks[taskIndex].unfolded)
                {
                    SerializedProperty action = tasksProperty.GetArrayElementAtIndex(taskIndex).FindPropertyRelative("action");
                    EditorGUILayout.PropertyField(action);

                    SerializedProperty hideOther = tasksProperty.GetArrayElementAtIndex(taskIndex).FindPropertyRelative("hideOther");
                    EditorGUILayout.PropertyField(hideOther);

                    switch (trigger.tasks[taskIndex].action)
                    {
                        case OnboardingDataHolder.OnboardingActions.SHOW_MESSAGE:
                            messageProperty = tasksProperty.GetArrayElementAtIndex(taskIndex).FindPropertyRelative("message");
                            EditorGUILayout.PropertyField(messageProperty);
                            break;

                        case OnboardingDataHolder.OnboardingActions.POINT_AT:
                            pointAtProperty = tasksProperty.GetArrayElementAtIndex(taskIndex).FindPropertyRelative("pointAt");

                            EditorGUILayout.PropertyField(pointAtProperty);
                            break;

                        case OnboardingDataHolder.OnboardingActions.HIGHLIGHT:
                            highlightProperty = tasksProperty.GetArrayElementAtIndex(taskIndex).FindPropertyRelative("areas");

                            for (int highlighAreaIndex = 0; highlighAreaIndex < highlightProperty.arraySize; highlighAreaIndex++)
                            {
                                SerializedProperty from = highlightProperty.GetArrayElementAtIndex(highlighAreaIndex).FindPropertyRelative("from");
                                EditorGUILayout.PropertyField(from);

                                SerializedProperty to = highlightProperty.GetArrayElementAtIndex(highlighAreaIndex).FindPropertyRelative("to");
                                EditorGUILayout.PropertyField(to);

                                DrawLine(Color.gray);
                            }

                            if (GUILayout.Button("Add Area", GUILayout.Width(100f)))
                                trigger.tasks[taskIndex].areas = trigger.tasks[taskIndex].areas.AddElement(new OnboardingDataHolder.OnboardingTask.HighlightArea());
                            break;
                    }
                }
            }

            if (GUILayout.Button("Add Task", GUILayout.Width(150f)))
                trigger.tasks = trigger.tasks.AddElement(new OnboardingDataHolder.OnboardingTask());

            serializedObject.ApplyModifiedProperties();
        }

        public void DrawLine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }
    }
}
