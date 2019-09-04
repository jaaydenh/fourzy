//@vadym udod

using Fourzy._Updates.UI.Menu;
using UnityEngine;

namespace Fourzy._Updates.UI.Helpers
{
    public class NavigationOverlay : MonoBehaviour
    {
        public static NavigationOverlay Instance
        {
            get
            {
                if (!instance)
                    instance = PersistantMenuController.instance.GetComponentInChildren<NavigationOverlay>();

                return instance;
            }
        }

        private static NavigationOverlay instance = null;

        public NavigationCursor cursor;

        protected void LateUpdate()
        {
            //make cursor stick to its target
        }

        public void StickTo(EventTriggerExtended eventTrigger)
        {


            cursor.Show(.3f);
            cursor.SizeTo(eventTrigger.size);
            cursor.MoveTo(cursor.transform.position, eventTrigger.center, .5f);
        }
    }
}
