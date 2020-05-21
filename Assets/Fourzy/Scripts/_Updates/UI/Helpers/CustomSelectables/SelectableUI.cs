//@vadym udod

using ByteSheep.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Helpers
{
    public class SelectableUI : MonoBehaviour
    {
        public AdvancedEvent onPointerEnter;
        public AdvancedEvent onPointerExit;

        private List<SelectableUIHook> hooks = new List<SelectableUIHook>();

        public bool interactable
        {
            get
            {
                Selectable selectable = GetComponentInParent<Selectable>();

                if (selectable)
                    return selectable.interactable;
                else
                    return gameObject.activeInHierarchy;
            }
        }

        protected void Awake()
        {
            hooks.AddRange(GetComponentsInChildren<SelectableUIHook>());
        }

        public virtual void OnEnter()
        {
            hooks.ForEach(hook => hook.OnEnter());
            onPointerEnter.Invoke();
        }

        public virtual void OnExit()
        {
            hooks.ForEach(hook => hook.OnExit());
            onPointerExit.Invoke();
        }
    }
}