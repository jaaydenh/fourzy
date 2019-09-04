//@vadym udod

using ByteSheep.Events;
using UnityEngine;

namespace Fourzy._Updates.UI.Helpers
{
    public class Selectable3D : MonoBehaviour
    {
        public AdvancedEvent onPointerEnter;
        public AdvancedEvent onPointerExit;

        public void OnEnter()
        {
            onPointerEnter.Invoke();
        }

        public void OnExit()
        {
            onPointerExit.Invoke();
        }
    }
}