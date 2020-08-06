//@vadym udod

using Fourzy._Updates.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu
{
    /// <summary>
    /// Handles all menu screens in current scene
    /// </summary>
    public class MenuController : MonoBehaviour
    {
        public static MenuController activeMenu
        {
            get
            {
                if (PersistantMenuController.instance.screensStack.Count > 0)
                    return PersistantMenuController.instance;
                else
                    return _activeMenu;
            }
            set => _activeMenu = value;
        }
        private static MenuController _activeMenu;

        public static Dictionary<string, MenuController> menus = new Dictionary<string, MenuController>();
        public static Dictionary<string, MenuEvents> menuEvents = new Dictionary<string, MenuEvents>();

        public Camera _camera;
        public RectTransform newScreensParent;
        public RectTransform canvasRoot;
        public MenuScreen[] extraScreens;

        public List<MenuScreen> screens { get; protected set; }
        public StackModified<MenuScreen> screensStack { get; protected set; }
        public MenuScreen currentScreen { get; protected set; }

        public CanvasScaler canvaseScaler { get; private set; }
        public Vector2 canvasToScreenRatio { get; private set; }
        public Canvas canvas { get; private set; }

        public bool state
        {
            get
            {
                if (canvasRoot)
                    return canvasRoot.gameObject.activeInHierarchy;
                else
                    return gameObject.activeInHierarchy;
            }
        }

        public float widthScaled { get; private set; }
        public float heightScaled { get; private set; }
        public bool initialized { get; private set; }

        public float widthRatioAdjusted => Mathf.Lerp(1f, Screen.width / widthScaled, canvaseScaler.matchWidthOrHeight);
        public float heightRatioAdjusted => Mathf.Lerp(1f, Screen.height / heightScaled, 1f - canvaseScaler.matchWidthOrHeight);
        public float widthAdjusted => canvaseScaler.referenceResolution.x * widthRatioAdjusted;
        public float heightAdjusted => canvaseScaler.referenceResolution.y * heightRatioAdjusted;
        public Vector2 size { get; private set; }

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
            StandaloneInputModuleExtended.onBackPressed += OnBack;

            screens = new List<MenuScreen>(transform.GetComponentsInChildren<MenuScreen>());
            screens.AddRange(extraScreens);
            foreach (MenuScreen screen in extraScreens) if (!screen.menuController) screen.menuController = this;

            screensStack = new StackModified<MenuScreen>();
            canvaseScaler = GetComponent<CanvasScaler>();
            canvas = GetComponent<Canvas>();

            widthScaled = Screen.height / canvaseScaler.referenceResolution.y * canvaseScaler.referenceResolution.x;
            heightScaled = Screen.width / canvaseScaler.referenceResolution.x * canvaseScaler.referenceResolution.y;

            if (!menus.ContainsKey(gameObject.name)) menus.Add(gameObject.name, this);
            if (GetType() != typeof(PersistantMenuController)) activeMenu = this;
        }

        protected virtual void Start()
        {
            size = new Vector2(widthAdjusted * transform.localScale.x, heightAdjusted * transform.localScale.y);
            //print(size + " " + widthScaled + " " + heightScaled + " " + widthAdjusted + " " + heightAdjusted);

            StartCoroutine(InitializedRoutine());
        }

        protected virtual void OnDestroy()
        {
            StandaloneInputModuleExtended.onBackPressed -= OnBack;

            if (menus.ContainsKey(gameObject.name)) menus.Remove(gameObject.name);
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
        public virtual void CloseCurrentScreen(bool animate = true)
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
                    currentScreen = null;
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

            if (screensStack.Count == 0) currentScreen = null;
        }

        public void BackToRoot()
        {
            MenuScreen first = screensStack.Peek();

            while (screensStack.Count > 2)
                if (first == screensStack.Peek())
                    screensStack.Pop().Close(true);
                else
                    screensStack.Pop().Close(false);

            if (screensStack.Count > 1) CloseCurrentScreen(false);
        }

        public void OpenScreen<T>(bool addIfNotExists = false) where T : MenuScreen
        {
            foreach (MenuScreen screen in screens)
                if (screen.GetType() == typeof(T))
                {
                    OpenScreen(screen);
                    return;
                }

            if (addIfNotExists) OpenScreen(AddScreen<T>());
        }

        public void OpenScreen(int index) => OpenScreen(screens[index]);

        public void OpenScreen(MenuScreen screen)
        {
            if (currentScreen && currentScreen.isOpened && screen.closePreviousWhenOpened) currentScreen.Close();

            SetCurrentScreen(screen);
            screensStack.Push(currentScreen);

            currentScreen.Open();
        }

        public T GetScreen<T>() where T : MenuScreen
        {
            foreach (MenuScreen screen in screens) if (screen.GetType() == typeof(T)) return screen as T;

            return null;
        }

        public T GetOrAddScreen<T>(bool newIfOpened = false) where T : MenuScreen
        {
            foreach (MenuScreen screen in screens)
                if (screen.GetType() == typeof(T) && ((screen.isOpened && !newIfOpened) || !screen.isOpened))
                    return screen as T;

            return AddScreen<T>();
        }

        public T AddScreen<T>(MenuScreen prefab) where T : MenuScreen
        {
            if (!prefab) return null;

            MenuScreen newScreen = Instantiate(prefab, newScreensParent == null ? transform : newScreensParent);

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

                if (screenPrefab) return AddScreen<T>(screenPrefab);
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
            => new Vector2(
                WorldToCanvasPoint(new Vector3(size.x, 0f)).x - WorldToCanvasPoint(Vector3.zero).x, 
                WorldToCanvasPoint(new Vector3(0f, size.y)).y - WorldToCanvasPoint(Vector3.zero).y);

        public Vector2 WorldToViewport(Vector2 worldPoint)
            => _camera ? _camera.WorldToViewportPoint(worldPoint) : Camera.main.WorldToViewportPoint(worldPoint);

        public virtual void SetState(bool state)
        {
            if (canvasRoot != null)
            {
                if (canvasRoot.gameObject.activeInHierarchy == state) return;

                canvasRoot.gameObject.SetActive(state);
            }
            else
            {
                if (gameObject.activeInHierarchy == state) return;

                gameObject.SetActive(state);
            }

            if (state)
            {
                activeMenu = this;
                if (currentScreen) currentScreen.Open();

                ExecuteMenuEvents();
            }

            if (_camera) _camera.gameObject.SetActive(state);
        }

        public void ExecuteMenuEvents()
        {
            if (!menuEvents.ContainsKey(name)) menuEvents.Add(name, new MenuEvents());

            if (menuEvents.Count == 0 || !state) return;

            StartCoroutine(InvokeMenuEvents(menuEvents[name]));
        }

        protected virtual void OnBack()
        {
            if (PersistantMenuController.instance.screensStack.Count > 0 || !state || !StandaloneInputModuleExtended.BackEventAvailable) return;
            
            if (screensStack.Count > 0)
            {
                MenuScreen menuScreen = screensStack.Peek();

                if (!menuScreen.interactable) return;

                menuScreen.OnBack();
            }
        }

        protected virtual void OnInvokeMenuEvents(MenuEvents events) { }

        protected virtual void OnInitialized() { }

        private IEnumerator InitializedRoutine()
        {
            while (!screens.TrueForAll(screen => screen.Initialized)) yield return null;

            OnInitialized();
            initialized = true;

            ExecuteMenuEvents();
        }

        private IEnumerator InvokeMenuEvents(MenuEvents events)
        {
            while (!initialized) yield return null;

            OnInvokeMenuEvents(events);

            screens.ForEach(screen => screen.ExecuteMenuEvent(events));

            //remove events
            menuEvents[name].Clear();
        }

        public static void AddMenuEvent(string menuName, params KeyValuePair<string, object>[] events)
        {
            if (!menuEvents.ContainsKey(menuName)) menuEvents.Add(menuName, new MenuEvents());

            foreach (var @event in events) menuEvents[menuName][@event.Key] = @event.Value;
        }

        public static void SetState(string key, bool state)
        {
            if (menus.ContainsKey(key)) menus[key].SetState(state);
        }

        public static MenuController GetMenu(string key)
        {
            if (menus.ContainsKey(key)) return menus[key];

            return null;
        }
    }

    public class MenuEvents
    {
        public string @event;

        public Dictionary<string, object> data = new Dictionary<string, object>();

        public object this[string idx]
        {
            get => data[idx];
            set => data.Add(idx, value);
        }

        public MenuEvents(params KeyValuePair<string, object>[] @params)
        {
            foreach (KeyValuePair<string, object> _param in @params)
                this[_param.Key] = _param.Value;
        }

        public int Count => data.Count;

        public void Clear() => data.Clear();
    }
}