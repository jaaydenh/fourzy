﻿//@vadym udod


using UnityEngine;
using UnityEngine.EventSystems;

namespace Fourzy._Updates.UI.Menu
{
    public class PersistantMenuController : MenuController
    {
        public static PersistantMenuController instance;

        protected override void Awake()
        {
            if (instance)
            {
                DestroyImmediate(gameObject);

                return;
            }

            DontDestroyOnLoad(gameObject);
            instance = this;
            base.Awake();
        }

        protected override void OnBack()
        {
            if (screensStack.Count == 0) return;

            if (screensStack.Count > 0)
            {
                MenuScreen menuScreen = screensStack.Peek();

                if (!menuScreen.interactable) return;

                menuScreen.OnBack();

                StandaloneInputModuleExtended.BackEventAvailable = false;
            }
        }

        public override void CloseCurrentScreen(bool animate = true)
        {
            if (screensStack.Count > 0)
            {
                screensStack.Pop().Close(animate);

                if (screensStack.Count > 0)
                {
                    MenuScreen menuScreen = screensStack.Peek();

                    SetCurrentScreen(menuScreen);
                    menuScreen.Open();
                }
                else
                {
                    currentScreen = null;

                    MenuController underlayingMenuController = GetMenu(Constants.GAMEPLAY_MENU_CANVAS_NAME) ?? GetMenu(Constants.MAIN_MENU_CANVAS_NAME);

                    if (underlayingMenuController.screensStack.Count > 0) underlayingMenuController.screensStack.Peek().Open();
                }
            }
        }
    }
}
