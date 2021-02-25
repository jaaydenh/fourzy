//@vadym udod

using ByteSheep.Events;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchZone : EventTrigger
{
    public Action<Vector2, int> onPointerDownData;
    public Action<Vector2, int> onPointerUpData;

    public AdvancedVector2Event onPointerDown;
    public AdvancedVector2Event onPointerMove;
    public AdvancedVector2Event onPointerUp;

    public bool dragPositionRelative = false;
    public bool scalePosition = false;

    private Vector2 originPosition;
    private Canvas canvas;

    public void Awake()
    {
        canvas = CheckObj(transform);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (scalePosition)
            originPosition = new Vector2(eventData.position.x / canvas.transform.lossyScale.x, eventData.position.y / canvas.transform.lossyScale.y);
        else
            originPosition = new Vector2(eventData.position.x, eventData.position.y);

        onPointerDown.Invoke(originPosition);
        onPointerDownData?.Invoke(originPosition, eventData.pointerId);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        
        if (dragPositionRelative)
        {
            if (scalePosition)
                onPointerMove.Invoke(new Vector2(eventData.position.x / canvas.transform.lossyScale.x, eventData.position.y / canvas.transform.lossyScale.y) - originPosition);
            else
                onPointerMove.Invoke(new Vector2(eventData.position.x, eventData.position.y) - originPosition);
        }
        else
        {
            if (scalePosition)
                onPointerMove.Invoke(new Vector2(eventData.position.x / canvas.transform.lossyScale.x, eventData.position.y / canvas.transform.lossyScale.y));
            else
                onPointerMove.Invoke(new Vector2(eventData.position.x, eventData.position.y));
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        Vector2 _result;
        if (scalePosition) _result = new Vector2(eventData.position.x / canvas.transform.lossyScale.x, eventData.position.y / canvas.transform.lossyScale.y);
        else _result = new Vector2(eventData.position.x, eventData.position.y);

        onPointerUp.Invoke(_result);
        onPointerUpData.Invoke(_result, eventData.pointerId);
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