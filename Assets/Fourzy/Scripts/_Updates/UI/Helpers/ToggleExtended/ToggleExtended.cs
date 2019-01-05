//@vadym udod

using ByteSheep.Events;
using Fourzy._Updates.Serialized;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Helpers
{
    public class ToggleExtended : Toggle
    {
        [HideInInspector]
        public AdvancedEvent onState;
        [HideInInspector]
        public AdvancedEvent offState;
        [HideInInspector]
        public AdvancedBoolEvent state;
        [HideInInspector]
        public AudioTypes onSfx = AudioTypes.TOGGLE_ON;
        [HideInInspector]
        public AudioTypes offSfx = AudioTypes.TOGGLE_OFF;

        protected override void Awake()
        {
            base.Awake();

            if (!Application.isPlaying)
                return;

            onValueChanged.AddListener(OnValueChanged);
        }

        protected override void Start()
        {
            base.Start();

            OnValueChanged(isOn);
        }

        protected void OnValueChanged(bool value)
        {
            if (value)
                onState.Invoke();
            else
                offState.Invoke();

            state.Invoke(value);
        }
    }
}
