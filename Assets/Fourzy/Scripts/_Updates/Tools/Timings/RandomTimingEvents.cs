//@vadym udod

using ByteSheep.Events;
using UnityEngine;

namespace Fourzy._Updates.Tools.Timing
{
    public class RandomTimingEvents : MonoBehaviour
    {
        public AdvancedEvent events;

        [HideInInspector, SerializeField]
        public float from = 1f;
        [HideInInspector, SerializeField]
        public float to = 2f;
        [HideInInspector, SerializeField]
        public float offsetFrom = 1f;
        [HideInInspector, SerializeField]
        public float offsetTo = 2f;

        public bool onStart = false;
        public bool repeat = false;

        void Start()
        {
            if (onStart)
                CallWithDefaultOffset();
        }

        public void CallWithDefaultOffset()
        {
            CallEvents(Random.Range(offsetFrom, offsetTo));
        }

        public void CallEvents(float offset = 0f)
        {
            Invoke("ExecuteEvents", offset);
        }

        public void ExecuteEvents()
        {
            events.Invoke();

            if (repeat)
                CallEvents(Random.Range(from, to));
        }

        public void StopRepeating()
        {
            CancelInvoke("ExecuteEvents");
        }
    }
}