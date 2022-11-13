//@vadym udod

using Fourzy._Updates.Managers;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class SkillzMainMenuScreen : MenuScreen
    {
        private int counter = 0;

        private void Update()
        {
            if (isCurrent)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (Input.mousePosition.magnitude < Screen.width * .2f)
                    {
                        counter++;

                        if (counter > 4)
                        {
                            SettingsManager.Toggle(SettingsManager.KEY_DEMO_MODE);
                            counter = 0;
                        }
                    }
                    else
                    {
                        counter = 0;
                    }
                }
            }
        }

        public override void OnBack()
        {
            base.OnBack();

            GameManager.Instance.CloseApp();
        }

        public void StartSkillz()
        {
            SkillzMainMenuController.Instance.StartSkillzUI();
        }
    }
}
