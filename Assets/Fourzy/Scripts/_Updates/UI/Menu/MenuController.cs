//@vadym udod

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
        public static MenuController current;

        public Camera _camera;
        public MenuScreen[] extraScreens;

        public List<MenuScreen> screens { get; protected set; }
        public MenuScreen currentScreen { get; protected set; }
        public Stack<MenuScreen> screensStack { get; protected set; }

        protected virtual void Awake()
        {
            screens = new List<MenuScreen>(transform.GetComponentsInChildren<MenuScreen>());
            screens.AddRange(extraScreens);

            screensStack = new Stack<MenuScreen>();

            if (!menus.ContainsKey(gameObject.name))
                menus.Add(gameObject.name, this);

            foreach (MenuScreen screen in extraScreens)
                if (!screen.menuController)
                    screen.menuController = this;
        }

        protected virtual void Update()
        {
            //back button functionality
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //first send ESC to static menu controller
                if (PersistantMenuController.current.screensStack.Count > 0)
                    return;

                if (screensStack.Count > 0)
                {
                    MenuScreen menuScreen = screensStack.Peek();

                    if (!menuScreen.interactable)
                        return;

                    menuScreen.OnBack();
                }
            }
        }

        protected virtual void OnEnable()
        {
            current = this;
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

        public void OpenScreen<T>(bool addIfNotExists = false) where T : MenuScreen
        {
            foreach (MenuScreen screen in screens)
                if (screen.GetType() == typeof(T))
                {
                    OpenScreen(screen);
                    return;
                }

            if (addIfNotExists)
                OpenScreen(AddScreen<T>());
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

        public T GetScreen<T>(bool newIfOpened = false) where T : MenuScreen
        {
            foreach (MenuScreen screen in screens)
                if (screen.GetType() == typeof(T) && ((screen.isOpened && !newIfOpened) || !screen.isOpened))
                    return screen as T;

            return AddScreen<T>();
        }

        public T AddScreen<T>(MenuScreen prefab) where T : MenuScreen
        {
            if (!prefab)
                return null;

            MenuScreen newScreen = Instantiate(prefab, transform);

            newScreen.transform.localScale = Vector3.one;
            screens.Add(newScreen);

            return (T)newScreen;
        }

        /// <summary>
        /// Add screen from GameContentManager typed prefabs list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T AddScreen<T>() where T : MenuScreen
        {
            Type screenType = typeof(T);

            foreach (GameContentManager.PrefabTypePair pair in GameContentManager.Instance.typedPrefabs)
            {
                MenuScreen screenPrefab = pair.prefab.GetComponent<T>();

                if (screenPrefab)
                    return AddScreen<T>(screenPrefab);
            }

            return null;
        }

        public void SetState(bool state)
        {
            if (gameObject.activeInHierarchy == state)
                return;

            gameObject.SetActive(state);

            if (_camera)
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