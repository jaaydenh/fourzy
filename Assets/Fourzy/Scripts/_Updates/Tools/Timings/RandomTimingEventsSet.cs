//@vadym udod

using System.Collections;
using UnityEngine;

namespace Fourzy._Updates.Tools.Timing
{
    public class RandomTimingEventsSet : MonoBehaviour
    {
        public AdvancedTimingEvent[] eventsSet;

        [HideInInspector]
        public float from = 1f;
        [HideInInspector]
        public float to = 2f;
        [HideInInspector]
        public float offsetFrom = 1f;
        [HideInInspector]
        public float offsetTo = 2f;

        public bool onStart = false;
        public bool repeat = false;

        private int currentIndex = 0;

        void Start()
        {
            if (onStart)
                CallWithDefaultOffset();
        }

        public void CallWithDefaultOffset()
        {
            CallEvents(Random.Range(offsetFrom, offsetTo));
        }

        public void CallEvents(float offset)
        {
            Invoke("ExecuteEvents", offset);
        }

        public void ExecuteEvents()
        {
            currentIndex = 0;
            if (eventsSet.Length > 0 && gameObject.activeInHierarchy)
                StartCoroutine(wait(eventsSet[currentIndex].time));

            if (repeat)
                CallEvents(Random.Range(from, to));
        }

        public void StopRepeating()
        {
            StopAllCoroutines();
            CancelInvoke();
        }

        private IEnumerator wait(float time)
        {
            yield return new WaitForSeconds(time);
            eventsSet[currentIndex].onTimerEnd.Invoke();

            if ((currentIndex < eventsSet.Length - 1) && gameObject.activeInHierarchy)
            {
                currentIndex++;
                StartCoroutine(wait(eventsSet[currentIndex].time));
            }
        }
    }
}