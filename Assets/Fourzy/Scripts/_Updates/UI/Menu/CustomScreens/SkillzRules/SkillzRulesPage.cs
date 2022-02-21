//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class SkillzRulesPage : MonoBehaviour
    {
        public void OnPageOpened()
        {
            foreach (ISKillzRulesPageComponent pageComponent in GetComponentsInChildren<ISKillzRulesPageComponent>())
            {
                pageComponent.OnPageOpened();
            }
        }

        public void OnPageClosed()
        {
            foreach (ISKillzRulesPageComponent pageComponent in GetComponentsInChildren<ISKillzRulesPageComponent>())
            {
                pageComponent.OnPageClosed();
            }
        }
    }
}
