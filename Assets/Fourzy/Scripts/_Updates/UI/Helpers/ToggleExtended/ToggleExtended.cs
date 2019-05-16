//@vadym udod

using ByteSheep.Events;
using Fourzy._Updates.Audio;
using Fourzy._Updates.Serialized;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
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
        public AdvancedEvent onClick;
        [HideInInspector]
        public AdvancedBoolEvent state;
        [HideInInspector]
        public AudioTypes onSfx = AudioTypes.TOGGLE_ON;

        public Func<bool> onCondition;

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

            if (!Application.isPlaying)
                return;

            OnValueChanged(isOn);
        }

        protected void OnValueChanged(bool value)
        {
            if (value)
            {
                onState.Invoke();
                AudioHolder.instance.PlaySelfSfxOneShotTracked(onSfx);
            }
            else
                offState.Invoke();

            state.Invoke(value);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (onCondition != null && !onCondition())
                return;

            base.OnPointerClick(eventData);

            onClick.Invoke();
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            if (onCondition != null && !onCondition())
                return;

            base.OnSubmit(eventData);
        }
    }
}
