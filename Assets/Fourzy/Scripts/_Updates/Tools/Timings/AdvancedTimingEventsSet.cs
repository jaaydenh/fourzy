//@vadym udod

using ByteSheep.Events;
using System;
using System.Collections;
using UnityEngine;

namespace Fourzy._Updates.Tools.Timing
{
    public class AdvancedTimingEventsSet : MonoBehaviour
    {
        public bool onStart = false;

        public AdvancedTimingEvent[] eventsSet;

        private int currentIndex = 0;

        void OnEnable()
        {
            if (onStart)
                StartTimer();
        }

        public void StartTimer()
        {
            StopAllCoroutines();
            currentIndex = 0;

            if (eventsSet.Length > 0 && gameObject.activeInHierarchy)
                StartCoroutine(Wait(eventsSet[currentIndex].time));
        }

        public void ForceNext()
        {
            StopCoroutine("wait");
            eventsSet[currentIndex].onTimerEnd.Invoke();

            DequeueNext();
        }

        public float TotalSetExecutionTime()
        {
            float result = 0f;
            foreach (AdvancedTimingEvent @event in eventsSet)
                result += @event.time;

            return result;
        }

        private IEnumerator Wait(float time)
        {
            yield return new WaitForSeconds(time);
            eventsSet[currentIndex].onTimerEnd.Invoke();

            DequeueNext();
        }

        private void DequeueNext()
        {
            if ((currentIndex < eventsSet.Length - 1) && gameObject.activeSelf)
            {
                currentIndex++;
                StartCoroutine(Wait(eventsSet[currentIndex].time));
            }
        }
    }

    [Serializable]
    public class AdvancedTimingEvent
    {
        public float time;
        public AdvancedEvent onTimerEnd;
    }
}