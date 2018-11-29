//@vadym udod

using UnityEditor;
using UnityEditor.EventSystems;

[CustomEditor(typeof(TouchZone), false)]
public class TouchZoneEditor : EventTriggerEditor
{
    private SerializedProperty pointerDownProperty;
    private SerializedProperty pointerMoveProperty;
    private SerializedProperty pointerUpProperty;

    protected override void OnEnable()
    {
        base.OnEnable();

        pointerDownProperty = serializedObject.FindProperty("onPointerDown");
        pointerMoveProperty = serializedObject.FindProperty("onPointerMove");
        pointerUpProperty = serializedObject.FindProperty("onPointerUp");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        EditorGUILayout.PropertyField(pointerDownProperty);
        EditorGUILayout.PropertyField(pointerMoveProperty);
        EditorGUILayout.PropertyField(pointerUpProperty);

        serializedObject.ApplyModifiedProperties();
    }
}
