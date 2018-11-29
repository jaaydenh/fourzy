//@vadym udod

using ByteSheep.Events;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchZone : EventTrigger
{
    public AdvancedVector2Event onPointerDown;
    public AdvancedVector2Event onPointerMove;
    public AdvancedVector2Event onPointerUp;

    private Vector2 originPosition;
    private Canvas canvas;

    public void Awake()
    {
        canvas = CheckObj(transform);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        originPosition = new Vector2(eventData.position.x / canvas.transform.lossyScale.x, eventData.position.y / canvas.transform.lossyScale.y);

        onPointerDown.Invoke(originPosition);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        
        onPointerMove.Invoke(new Vector2(eventData.position.x / canvas.transform.lossyScale.x, eventData.position.y / canvas.transform.lossyScale.y) - originPosition);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        Vector2 pos = eventData.position;
        pos.Scale(canvas.transform.lossyScale);
        onPointerUp.Invoke(pos);
    }

    private Canvas CheckObj(Transform @object)
    {
        if (@object)
        {
            if (@object.GetComponent<Canvas>() != null)
                return @object.GetComponent<Canvas>();
            else
                return CheckObj(@object.parent);
        }
        else
            return null;
    }
}