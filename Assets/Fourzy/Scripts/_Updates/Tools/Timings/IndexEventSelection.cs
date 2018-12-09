//@vadym udod

using System;
using System.Collections;
using UnityEngine;

public class IndexEventSelection : MonoBehaviour
{
    public IndexedAdvancedTimingEvent[] events;

    public void ExecureAtIndex(int index)
    {
        foreach (IndexedAdvancedTimingEvent @event in events)
            if (@event.index == index)
                StartCoroutine(Execute(@event));
    }

    public IEnumerator Execute(AdvancedTimingEvent @event)
    {
        yield return new WaitForSeconds(@event.time);
        @event.onTimerEnd.Invoke();
    }
}

[Serializable]
public class IndexedAdvancedTimingEvent : AdvancedTimingEvent
{
    public int index;
}
