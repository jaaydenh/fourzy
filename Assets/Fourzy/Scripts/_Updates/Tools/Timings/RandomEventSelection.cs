//@vadym udod

using ByteSheep.Events;
using UnityEngine;

public class RandomEventSelection : MonoBehaviour
{
    public AdvancedEvent[] randomEventsSet;

    public void InvokeRandom()
    {
        if (randomEventsSet.Length > 0)
            randomEventsSet[Random.Range(0, randomEventsSet.Length)].Invoke();
    }
}
