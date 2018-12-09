//@vadym udod

using Fourzy._Updates.UI.Menu.Screens;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu
{
    /// <summary>
    /// Handles all menu screens in current scene
    /// </summary>
    public class MenuController : MonoBehaviour
    {
        public static Dictionary<string, MenuController> menus = new Dictionary<string, MenuController>();

        public Camera _camera;
        public MenuScreen[] extraScreens;

        public List<MenuScreen> screens { get; private set; }
        public MenuScreen currentScreen { get; private set; }
        public Stack<MenuScreen> screensStack { get; private set; }

        public Dictionary<Type, PromptScreen> availablePromptScreens;

        protected void Awake()
        {
            screens = new List<MenuScreen>(transform.GetComponentsInChildren<MenuScreen>());
            availablePromptScreens = new Dictionary<Type, PromptScreen>();
            screens.AddRange(extraScreens);

            screensStack = new Stack<MenuScreen>();

            if (!menus.ContainsKey(gameObject.name))
                menus.Add(gameObject.name, this);
        }

        protected void Start()
        {
            foreach (MenuScreen screen in extraScreens)
                if (!screen.menuController)
                    screen.menuController = this;
        }

        protected void Update()
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

        public void SetCurrentScreen(int index)
        {
            currentScreen = screens[index];
        }

        public void SetCurrentScreen(MenuScreen screen)
        {
            SetCurrentScreen(screens.IndexOf(screen));
        }

        /// <summary>
        /// Invokes OnBack on current screen
        /// </summary>
        public void CurrentScreenOnBack()
        {
            if (currentScreen)
                currentScreen.OnBack();
        }

        /// <summary>
        /// Closes current screen
        /// </summary>
        public void CloseCurrentScreen()
        {
            if (screensStack.Count > 0)
            {
                screensStack.Pop().Close();

                if (screensStack.Count > 0)
                {
                    MenuScreen menuScreen = screensStack.Peek();

                    SetCurrentScreen(menuScreen);
                    menuScreen.Open();
                }
            }
        }

        public void OpenScreen<T>() where T : MenuScreen
        {
            foreach (MenuScreen screen in screens)
                if (screen.GetType() == typeof(T))
                {
                    OpenScreen(screen);
                    break;
                }
        }

        public void OpenScreen(int index)
        {
            OpenScreen(screens[index]);
        }

        public void OpenScreen(MenuScreen screen)
        {
            SetCurrentScreen(screen);
            screensStack.Push(currentScreen);

            currentScreen.Open();
        }

        public T GetScreen<T>() where T : MenuScreen
        {
            foreach (MenuScreen screen in screens)
                if (screen.GetType() == typeof(T))
                    return screen as T;

            return null;
        }

        public MenuScreen AddScreen(MenuScreen screenPrefab)
        {
            if (!screenPrefab)
                return null;

            MenuScreen newScreen = Instantiate(screenPrefab, transform);

            newScreen.transform.localScale = Vector3.one;
            screens.Add(newScreen);

            return newScreen;
        }

        public T AddScreen<T>(MenuScreen prefab) where T : MenuScreen
        {
            return (T)AddScreen(prefab);
        }

        public T GetPrompt<T>() where T : PromptScreen
        {
            Type promptType = typeof(T);

            if (!availablePromptScreens.ContainsKey(promptType))
            {
                foreach (GameContentManager.PrefabTypePair pair in GameContentManager.Instance.typedPrefabs)
                {
                    PromptScreen promptScreen = pair.prefab.GetComponent<T>();

                    if (promptScreen)
                    {
                        availablePromptScreens.Add(promptType, AddScreen<T>(promptScreen));
                        break;
                    }
                }
            }

            return (T)availablePromptScreens[promptType];
        }

        public void SetState(bool state)
        {
            gameObject.SetActive(state);
            _camera.gameObject.SetActive(state);
        }

        public static void SetState(string key, bool state)
        {
            if (menus.ContainsKey(key))
                menus[key].SetState(state);
        }

        public static MenuController GetMenu(string key)
        {
            if (menus.ContainsKey(key))
                return menus[key];

            return null;
        }
    }
}