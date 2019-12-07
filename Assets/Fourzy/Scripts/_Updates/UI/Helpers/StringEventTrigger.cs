//@vadym udod

using ByteSheep.Events;
using System;
using UnityEngine;

namespace Fourzy._Updates.UI.Helpers
{
    public class StringEventTrigger : MonoBehaviour
    {
        public StringEvent[] events;

        public void TryInvoke(string eventName) => Array.Find(events, (_event) => _event.command == eventName)?._event.Invoke();

        [System.Serializable]
        public class StringEvent
        {
            public string command;
            public AdvancedEvent _event;
        }
    }
}