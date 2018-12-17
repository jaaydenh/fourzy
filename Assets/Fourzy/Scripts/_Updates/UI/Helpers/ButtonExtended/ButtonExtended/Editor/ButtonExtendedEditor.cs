﻿//@vadym udod

using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace Fourzy._Updates.UI.Helpers
{
    [CustomEditor(typeof(ButtonExtended), true)]
    public class ButtonExtendedEditor : ButtonEditor
    {
        private SerializedProperty eventProperty;
        private SerializedProperty sfxProperty;
        private SerializedProperty labelsProperty;
        private SerializedProperty badgesProperty;

        private bool foldout = false;
        private ButtonExtended trigger;

        protected override void OnEnable()
        {
            base.OnEnable();

            eventProperty = serializedObject.FindProperty("events");
            sfxProperty = serializedObject.FindProperty("playOnClick");
            labelsProperty = serializedObject.FindProperty("labels");
            badgesProperty = serializedObject.FindProperty("badges");

            trigger = target as ButtonExtended;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            EditorGUILayout.PropertyField(eventProperty);
            EditorGUILayout.PropertyField(sfxProperty);

            if (foldout = EditorGUILayout.Foldout(foldout, "Extra data"))
            {
                //labels
                GUILayout.Space(10f);
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Labels");
                    if (GUILayout.Button("+", GUILayout.Width(30f)))
                        trigger.labels.Add(new ButtonExtended.LabelPair());
                }
                EditorGUILayout.EndHorizontal();

                for (int index = 0; index < labelsProperty.arraySize; index++)
                {
                    //need a reference
                    ButtonExtended.LabelPair value = trigger.labels[index];
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.PropertyField(labelsProperty.GetArrayElementAtIndex(index).FindPropertyRelative("labelName"));
                        if (GUILayout.Button("-", GUILayout.Width(30f)))
                            trigger.labels.Remove(value);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.PropertyField(labelsProperty.GetArrayElementAtIndex(index).FindPropertyRelative("label"));

                    DrawLine(Color.grey, 1, 20);
                }

                //badges
                GUILayout.Space(10f);
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Badges");
                    if (GUILayout.Button("+", GUILayout.Width(30f)))
                        trigger.badges.Add(new ButtonExtended.BadgePair());
                }
                EditorGUILayout.EndHorizontal();

                for (int index = 0; index < badgesProperty.arraySize; index++)
                {
                    //need a reference
                    ButtonExtended.BadgePair value = trigger.badges[index];
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.PropertyField(badgesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("badgeName"));
                        if (GUILayout.Button("-", GUILayout.Width(30f)))
                            trigger.badges.Remove(value);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.PropertyField(badgesProperty.GetArrayElementAtIndex(index).FindPropertyRelative("badge"));
                }
            }

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