//@vadym udod

using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu
{
    public class PersistantMenuController : MenuController
    {
        public new static MenuController current;

        protected override void Awake()
        {
            current = this;

            screens = new List<MenuScreen>(transform.GetComponentsInChildren<MenuScreen>());
            screens.AddRange(extraScreens);

            screensStack = new Stack<MenuScreen>();

            foreach (MenuScreen screen in extraScreens)
                if (!screen.menuController)
                    screen.menuController = this;
        }

        protected override void Update()
        {
            //back button functionality
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (screensStack.Count > 0)
                {
                    MenuScreen menuScreen = screensStack.Peek();

                    if (!menuScreen.interactable)
                        return;

                    menuScreen.OnBack();
                }
            }
        }

        protected override void OnEnable() { }
    }
}
