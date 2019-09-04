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

        private SerializedProperty batchesProperty;
        private SerializedProperty showPlayer2Property;
        private SerializedProperty onFinishedProperty;
        private SerializedProperty gameTypeProperty;
        private SerializedProperty stringProperty;
        private SerializedProperty openScreenProperty;
        private SerializedProperty nameProperty;

        private SerializedProperty tasksProperty;
        private SerializedProperty messageProperty;
        private SerializedProperty pointAtProperty;
        private SerializedProperty intProperty;
        private SerializedProperty onGameFinishedProperty;
        private SerializedProperty onMoveFinishedProperty;
        private SerializedProperty directionProperty;
        private SerializedProperty highlightProperty;

        protected void OnEnable()
        {
            trigger = target as OnboardingDataHolder;

            if (trigger.batches == null) trigger.batches = new OnboardingDataHolder.OnboardingTasksBatch[0];

            batchesProperty = serializedObject.FindProperty("batches");
            showPlayer2Property = serializedObject.FindProperty("showPlayer2");
            onFinishedProperty = serializedObject.FindProperty("onFinished");
            gameTypeProperty = serializedObject.FindProperty("gameType");
            stringProperty = serializedObject.FindProperty("stringValue");
            openScreenProperty = serializedObject.FindProperty("openScreen");
            nameProperty = serializedObject.FindProperty("tutorialName");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(showPlayer2Property);
            EditorGUILayout.PropertyField(nameProperty);
            EditorGUILayout.PropertyField(onFinishedProperty);

            switch (trigger.onFinished)
            {
                case OnboardingDataHolder.OnFinished.LOAD_GAME_SCENE:
                    EditorGUILayout.PropertyField(gameTypeProperty);

                    switch (trigger.gameType)
                    {

                        case GameType.PUZZLE:
                            EditorGUILayout.PropertyField(stringProperty, new GUIContent("Pack ID"));

                            break;

                        case GameType.PASSANDPLAY:
                            EditorGUILayout.PropertyField(stringProperty, new GUIContent("Board ID"));

                            break;
                    }

                    break;

                case OnboardingDataHolder.OnFinished.LOAD_MAIN_MENU:
                    EditorGUILayout.PropertyField(openScreenProperty);

                    break;
            }

            for (int batchIndex = 0; batchIndex < batchesProperty.arraySize; batchIndex++)
            {
                DrawLine(Color.red, 2, 15);
                tasksProperty = batchesProperty.GetArrayElementAtIndex(batchIndex).FindPropertyRelative("tasks");

                for (int taskIndex = 0; taskIndex < tasksProperty.arraySize; taskIndex++)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        trigger.batches[batchIndex].tasks[taskIndex].unfolded = EditorGUILayout.Foldout(
                            trigger.batches[batchIndex].tasks[taskIndex].unfolded,
                            trigger.batches[batchIndex].tasks[taskIndex].action.ToString());

                        if (GUILayout.Button("-", GUILayout.Width(50f)))
                        {
                            tasksProperty.DeleteArrayElementAtIndex(taskIndex);
                            continue;
                        }

                        if (taskIndex > 0)
                            if (GUILayout.Button("▲", GUILayout.Width(30f)))
                            {
                                tasksProperty.MoveArrayElement(taskIndex, taskIndex - 1);
                                continue;
                            }

                        if (taskIndex < tasksProperty.arraySize - 1)
                            if (GUILayout.Button("▼", GUILayout.Width(30f)))
                            {
                                tasksProperty.MoveArrayElement(taskIndex, taskIndex + 1);
                                continue;
                            }
                    }
                    EditorGUILayout.EndHorizontal();

                    if (trigger.batches[batchIndex].tasks[taskIndex].unfolded)
                    {
                        SerializedProperty action = tasksProperty.GetArrayElementAtIndex(taskIndex).FindPropertyRelative("action");
                        EditorGUILayout.PropertyField(action);

                        switch (trigger.batches[batchIndex].tasks[taskIndex].action)
                        {
                            case OnboardingDataHolder.OnboardingActions.SHOW_MESSAGE:
                                messageProperty = tasksProperty.GetArrayElementAtIndex(taskIndex).FindPropertyRelative("message");
                                EditorGUILayout.PropertyField(messageProperty);
                                break;

                            case OnboardingDataHolder.OnboardingActions.POINT_AT:
                                pointAtProperty = tasksProperty.GetArrayElementAtIndex(taskIndex).FindPropertyRelative("pointAt");

                                EditorGUILayout.PropertyField(pointAtProperty);
                                break;

                            case OnboardingDataHolder.OnboardingActions.OPEN_GAME:
                                intProperty = tasksProperty.GetArrayElementAtIndex(taskIndex).FindPropertyRelative("intValue");
                                onGameFinishedProperty = tasksProperty.GetArrayElementAtIndex(taskIndex).FindPropertyRelative("onGameFinished");

                                EditorGUILayout.PropertyField(intProperty, new GUIContent("Game ID"));
                                EditorGUILayout.PropertyField(onGameFinishedProperty, new GUIContent("On Game Finished"));
                                break;

                            case OnboardingDataHolder.OnboardingActions.HIGHLIGHT:
                            case OnboardingDataHolder.OnboardingActions.LIMIT_BOARD_INPUT:
                            case OnboardingDataHolder.OnboardingActions.SHOW_MASKED_AREA:
                                highlightProperty = tasksProperty.GetArrayElementAtIndex(taskIndex).FindPropertyRelative("areas");

                                for (int highlighAreaIndex = 0; highlighAreaIndex < highlightProperty.arraySize; highlighAreaIndex++)
                                {
                                    EditorGUILayout.PropertyField(highlightProperty.GetArrayElementAtIndex(highlighAreaIndex), new GUIContent("Area - " + highlighAreaIndex));

                                    if (GUILayout.Button("-", GUILayout.Width(50f)))
                                    {
                                        highlightProperty.DeleteArrayElementAtIndex(highlighAreaIndex);
                                        continue;
                                    }

                                    DrawLine(Color.gray);
                                }

                                if (GUILayout.Button("Add Area", GUILayout.Width(100f)))
                                    trigger.batches[batchIndex].tasks[taskIndex].areas = trigger.batches[batchIndex].tasks[taskIndex].areas.AddElementToEnd(new Rect(0, 0, 1, 1));
                                break;

                            case OnboardingDataHolder.OnboardingActions.ON_PLAYER2_MOVE_ENDED:
                            case OnboardingDataHolder.OnboardingActions.ON_PLAYER1_MOVE_ENDED:
                            case OnboardingDataHolder.OnboardingActions.ON_PLAYER1_MOVE_STARTED:
                            case OnboardingDataHolder.OnboardingActions.ON_PLAYER2_MOVE_STARTED:
                                onMoveFinishedProperty = tasksProperty.GetArrayElementAtIndex(taskIndex).FindPropertyRelative("nextAction");

                                EditorGUILayout.PropertyField(onMoveFinishedProperty);
                                break;

                            case OnboardingDataHolder.OnboardingActions.PLAYER_1_PLACE_GAMEPIECE:
                            case OnboardingDataHolder.OnboardingActions.PLAYER_2_PLACE_GAMEPIECE:
                                intProperty = tasksProperty.GetArrayElementAtIndex(taskIndex).FindPropertyRelative("intValue");
                                directionProperty = tasksProperty.GetArrayElementAtIndex(taskIndex).FindPropertyRelative("direction");

                                EditorGUILayout.PropertyField(directionProperty);
                                EditorGUILayout.PropertyField(intProperty, new GUIContent("At"));
                                break;

                            case OnboardingDataHolder.OnboardingActions.LOG_TUTORIAL:
                                messageProperty = tasksProperty.GetArrayElementAtIndex(taskIndex).FindPropertyRelative("message");
                                EditorGUILayout.PropertyField(messageProperty, new GUIContent("Stage"));

                                break;
                        }
                    }
                }

                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Add Task", GUILayout.Width(100f)))
                        trigger.batches[batchIndex].tasks = trigger.batches[batchIndex].tasks.AddElementToEnd(new OnboardingDataHolder.OnboardingTask() { unfolded = true, });

                    if (GUILayout.Button("-", GUILayout.Width(30f)))
                    {
                        batchesProperty.DeleteArrayElementAtIndex(batchIndex);
                        break;
                    }

                    if (batchIndex > 0)
                        if (GUILayout.Button("▲", GUILayout.Width(30f)))
                            batchesProperty.MoveArrayElement(batchIndex, batchIndex - 1);

                    if (batchIndex < batchesProperty.arraySize - 1)
                        if (GUILayout.Button("▼", GUILayout.Width(30f)))
                            batchesProperty.MoveArrayElement(batchIndex, batchIndex + 1);
                }
                EditorGUILayout.EndHorizontal();
            }

            DrawLine(Color.green);

            if (GUILayout.Button("Add Batch", GUILayout.Width(150f)))
                trigger.batches = trigger.batches.AddElementToEnd(new OnboardingDataHolder.OnboardingTasksBatch());

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
