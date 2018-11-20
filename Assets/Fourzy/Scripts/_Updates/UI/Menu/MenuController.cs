//@vadym udod

using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu
{
    /// <summary>
    /// Handles all menu screens in current scene
    /// </summary>
    public class MenuController : MonoBehaviour
    {
        /// <summary>
        /// Reference to current instance
        /// </summary>
        //public static MenuController instance;

        //public PromptScreen promptScreenPrefab;
        public MenuScreen[] extraScreens;

        public List<MenuScreen> screens { get; private set; }
        public MenuScreen currentScreen { get; private set; }
        public Stack<MenuScreen> screensStack { get; private set; }

        protected void Awake()
        {
            //looks like multiple menu controllers will exists at the same time, so cant rely on instance now
            //instance = this;

            screens = new List<MenuScreen>(transform.GetComponentsInChildren<MenuScreen>());
            screens.AddRange(extraScreens);

            screensStack = new Stack<MenuScreen>();
        }

        protected void Start()
        {
            ////if fade screen is opened, close it
            //if (FadeScreen.instance)
            //    FadeScreen.instance.StopLoading();

            //Time.timeScale = 1f;

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
        /// <param name="openPrevious">Open previous one?</param>
        public void CloseCurrentScreen(bool openPrevious)
        {
            if (screensStack.Count > 0)
            {
                screensStack.Pop().Close();

                if (screensStack.Count > 0)
                {
                    MenuScreen menuScreen = screensStack.Peek();

                    SetCurrentScreen(menuScreen);

                    if (!menuScreen.isOpened && openPrevious)
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

        //currently need screen handling only

        ///// <summary>
        ///// Show prompt screen
        ///// </summary>
        ///// <param name="text">Text</param>
        ///// <param name="onAccept">On Accepted</param>
        ///// <param name="onDecline">On Declined</param>
        //public void Prompt(string text, Action onAccept, Action onDecline = null)
        //{
        //    if (!PromptScreen.instance)
        //    {
        //        PromptScreen promptScreen = Instantiate(promptScreenPrefab, transform);
        //        promptScreen.transform.localPosition = Vector3.zero;
        //        promptScreen.transform.localScale = Vector3.one;

        //        screens.Add(promptScreen);
        //    }

        //    PromptScreen.instance.Prompt(text, onAccept, onDecline);
        //}

        ///// <summary>
        ///// Load game scene
        ///// </summary>
        ///// <param name="time">Delay time</param>
        //public void LoadGameScene(float time)
        //{
        //    if (currentScreen)
        //        currentScreen.SetInteractable(false);

        //    FadeScreen.instance.LoadGameScene(time);
        //}

        ///// <summary>
        ///// Load multiplayer scene
        ///// </summary>
        ///// <param name="time">Delay time</param>
        //public void LoadMultiplayerScene(float time)
        //{
        //    if (currentScreen)
        //        currentScreen.SetInteractable(false);

        //    FadeScreen.instance.LoadMultiplayerScene(time);
        //}

        ///// <summary>
        ///// Load main menu scene
        ///// </summary>
        ///// <param name="time">Delay time</param>
        //public void LoadMainMenu(float time)
        //{
        //    if (currentScreen)
        //        currentScreen.SetInteractable(false);

        //    FadeScreen.instance.LoadMainMenu(time);
        //}
    }
}