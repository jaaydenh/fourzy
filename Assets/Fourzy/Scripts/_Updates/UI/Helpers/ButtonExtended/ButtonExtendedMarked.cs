//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.UI.Helpers
{
    public class ButtonExtendedMarked : ButtonExtended
    {
        [HideInInspector]
        public Badge badge;

        protected override void Awake()
        {
            base.Awake();

            badge = GetComponentInChildren<Badge>();
        }
    }
}