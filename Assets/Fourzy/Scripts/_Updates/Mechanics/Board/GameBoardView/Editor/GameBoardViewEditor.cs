//@vadym udod

using UnityEditor;

namespace Fourzy._Updates.Mechanics.Board
{
    [CustomEditor(typeof(GameBoardView))]
    public class GameBoardViewEditor : Editor
    {
        private SerializedProperty interactableProperty;
        private SerializedProperty spawnHintAreaProperty;
        private SerializedProperty menuControllerProperty;

        private GameBoardView trigger;

        protected void OnEnable()
        {
            interactableProperty = serializedObject.FindProperty("interactable");
            spawnHintAreaProperty = serializedObject.FindProperty("spawnHintArea");
            menuControllerProperty = serializedObject.FindProperty("menuController");

            trigger = target as GameBoardView;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.PropertyField(spawnHintAreaProperty);
            EditorGUILayout.PropertyField(interactableProperty);
            EditorGUILayout.PropertyField(menuControllerProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
