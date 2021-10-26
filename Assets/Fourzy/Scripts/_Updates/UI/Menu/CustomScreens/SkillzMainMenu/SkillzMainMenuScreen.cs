//@vadym udod

using Fourzy._Updates.Managers;
using Fourzy._Updates.Tools;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class SkillzMainMenuScreen : RoutinesBase
    {
        public void StartSkillz()
        {
            SkillzMainMenuController.Instance.StartSkillzUI();
        }
    }
}
