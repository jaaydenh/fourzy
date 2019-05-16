//@vadym udod

using Fourzy._Updates.Tools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu
{
    /// <summary>
    /// Handles all menu screens in current scene
    /// </summary>
    public class MenuController : MonoBehaviour
    {
        public static Dictionary<string, MenuController> menus = new Dictionary<string, MenuController>();

        public Camera _camera;
        public bool closeCurrentOnOpen = false;
        public MenuScreen[] extraScreens;

        public List<MenuScreen> screens { get; protected set; }
        public StackModified<MenuScreen> screensStack { get; protected set; }
        public MenuScreen currentScreen { get; protected set; }

        public CanvasScaler canvaseScaler { get; private set; }
        public Vector2 canvasToScreenRatio { get; private set; }

        public float widthScaled { get; private set; }
        public float heightScaled { get; private set; }

        public float widthRatioAdjusted => Mathf.Lerp(1f, Screen.width / widthScaled, canvaseScaler.matchWidthOrHeight);
        public float heightRatioAdjusted => Mathf.Lerp(1f, Screen.height / heightScaled, 1f - canvaseScaler.matchWidthOrHeight);
        public float widthAdjusted => canvaseScaler.referenceResolution.x * widthRatioAdjusted;
        public float heightAdjusted => canvaseScaler.referenceResolution.y * heightRatioAdjusted;

        //editor version of widthAdjusted
        public float _widthAdjusted
        {
            get
            {
                CanvasScaler _canvasScaler = GetComponent<CanvasScaler>();
                return _canvasScaler.referenceResolution.x * 
                    Mathf.Lerp(1f, Screen.width / (Screen.height / _canvasScaler.referenceResolution.y * _canvasScaler.referenceResolution.x), _canvasScaler.matchWidthOrHeight);
            }
        }

        protected virtual void Awake()
        {
            screens = new List<MenuScreen>(transform.GetComponentsInChildren<MenuScreen>());
            screens.AddRange(extraScreens);

            screensStack = new StackModified<MenuScreen>();

            canvaseScaler = GetComponent<CanvasScaler>();

            widthScaled = Screen.height / canvaseScaler.referenceResolution.y * canvaseScaler.referenceResolution.x;
            heightScaled = Screen.width / canvaseScaler.referenceResolution.x * canvaseScaler.referenceResolution.y;

            if (!menus.ContainsKey(gameObject.name))
                menus.Add(gameObject.name, this);

            foreach (MenuScreen screen in extraScreens)
                if (!screen.menuController)
                    screen.menuController = this;
        }

        protected virtual void OnDestroy()
        {
            if (menus.ContainsKey(gameObject.name))
                menus.Remove(gameObject.name);
        }

        protected virtual void Update()
        {
            //back button functionality
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //first send ESC to static menu controller
                if (PersistantMenuController.instance.screensStack.Count > 0)
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
        public void CloseCurrentScreen(bool animate = true)
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
            }
        }

        public void CloseScreen(MenuScreen screen, bool animate = true)
        {
            if (screensStack.Contains(screen))
            {
                if (currentScreen == screen)
                    CloseCurrentScreen(animate);
                else
                {
                    screensStack.Remove(screen);
                    screen.Close(animate);
                }
            }
        }

        public void BackToRoot()
        {
            MenuScreen first = screensStack.Peek();

            while (screensStack.Count > 2)
                if (first == screensStack.Peek())
                    screensStack.Pop().Close(true);
                else
                    screensStack.Pop().Close(false);

            if (screensStack.Count > 1)
                CloseCurrentScreen(false);
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
            if (closeCurrentOnOpen && currentScreen)
                currentScreen.Close();

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
            foreach (GameContentManager.Screen pair in GameContentManager.Instance.screens.list)
            {
                MenuScreen screenPrefab = pair.prefab.GetComponent<T>();

                if (screenPrefab)
                    return AddScreen<T>(screenPrefab);
            }

            return null;
        }

        //only works for portrait orientation
        public Vector2 WorldToCanvasPoint(Vector3 worldPoint)
        {
            Vector3 screenPoint = _camera == null ? Camera.main.WorldToViewportPoint(worldPoint) : _camera.WorldToViewportPoint(worldPoint);

            return new Vector2(screenPoint.x * canvaseScaler.referenceResolution.x * widthRatioAdjusted, screenPoint.y * canvaseScaler.referenceResolution.y * heightRatioAdjusted);
        }

        public Vector2 WorldToCanvasSize(Vector2 size)
        {
            return new Vector2(WorldToCanvasPoint(new Vector3(size.x, 0f)).x - WorldToCanvasPoint(Vector3.zero).x, WorldToCanvasPoint(new Vector3(0f, size.y)).y - WorldToCanvasPoint(Vector3.zero).y);
        }

        public void SetState(bool state)
        {
            if (gameObject.activeInHierarchy == state)
                return;

            gameObject.SetActive(state);

            if (state && currentScreen)
                currentScreen.Open();

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