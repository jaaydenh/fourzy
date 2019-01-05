//@vadym udod

using UnityEditor;
using UnityEditor.EventSystems;

[CustomEditor(typeof(TouchZone), false)]
public class TouchZoneEditor : EventTriggerEditor
{
    private SerializedProperty pointerDownProperty;
    private SerializedProperty pointerMoveProperty;
    private SerializedProperty pointerUpProperty;
    private SerializedProperty dragPositionRelativeProperty;
    private SerializedProperty scalePositionProperty;

    protected override void OnEnable()
    {
        base.OnEnable();

        pointerDownProperty = serializedObject.FindProperty("onPointerDown");
        pointerMoveProperty = serializedObject.FindProperty("onPointerMove");
        pointerUpProperty = serializedObject.FindProperty("onPointerUp");
        dragPositionRelativeProperty = serializedObject.FindProperty("dragPositionRelative");
        scalePositionProperty = serializedObject.FindProperty("scalePosition");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.PropertyField(pointerDownProperty);
        EditorGUILayout.PropertyField(pointerMoveProperty);
        EditorGUILayout.PropertyField(pointerUpProperty);
        EditorGUILayout.PropertyField(dragPositionRelativeProperty);
        EditorGUILayout.PropertyField(scalePositionProperty);

        serializedObject.ApplyModifiedProperties();
    }
}
