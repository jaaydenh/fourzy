using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MoreMountains.Tools
{
    [CustomEditor(typeof(MMTransformRandomizer), true)]
    [CanEditMultipleObjects]
    public class MMTransformRandomizerEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Undo.RecordObject(target, "Modified MMTransformRandomizer");
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Test", EditorStyles.boldLabel);

            if (GUILayout.Button("Randomize"))
            {
                foreach (MMTransformRandomizer randomizer in targets)
                {
                    randomizer.Randomize();
                }
            }
        }
    }
}
