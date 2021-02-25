//@vadym udod

using Fourzy._Updates.UI.Widgets;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

[CustomEditor(typeof(ProgressionEvent), true), CanEditMultipleObjects]
public class ProgressionEventEditor : OdinEditor
{
    protected ProgressionEvent trigger;

    protected override void OnEnable()
    {
        base.OnEnable();

        trigger = target as ProgressionEvent;
    }

    protected void OnSceneGUI()
    {
        if (!trigger) return;

        foreach (ProgressionEvent point in trigger.unlockWhenComplete)
        {
            if (!point) return;

            Handles.DrawLine(trigger.transform.position, point.transform.position);
        }
    }
}
