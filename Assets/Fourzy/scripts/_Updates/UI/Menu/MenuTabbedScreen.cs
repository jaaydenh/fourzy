//@vadym udod

using ByteSheep.Events;
using Fourzy._Updates.Audio;
using Fourzy._Updates.Managers;
using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Menu.Screens;
using Fourzy._Updates.UI.Toasts;
using Fourzy._Updates.UI.Widgets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu
{
    public class MenuTabbedScreen : MenuScreen
    {
        public static MenuTabbedScreen Instance;

        public MenuTab defaultTab;

        public RectTransform tabsParent;
        public RectTransform tabsBar;
        public List<TabButton> tabsButtons;

        public AdvancedVector2Event onPointerDown;
        public AdvancedVector2Event onPointerMove;

        private SwipeHandler swipeHandler;

        private PositionTween tabsParentTween;
        private int currentTab = -1;
        private Vector3 pointerOrigin;

        private int demoCounter = 0;

        public override bool containsSelected => CurrentTab.containsSelected;
        public List<MenuTab> tabs { get; private set; }
        public MenuTab CurrentTab => tabs[currentTab];

        protected override void Awake()
        {
            base.Awake();

            Instance = this;

            tabsParentTween = tabsParent.GetComponent<PositionTween>();
            swipeHandler = GetComponent<SwipeHandler>();

            swipeHandler.onSwipe += OnSwipe;

            //get tabs
            tabs = new List<MenuTab>(GetComponentsInChildren<MenuTab>(true));

            //play bg audio
            if (!AudioHolder.instance.IsBGAudioPlaying(Serialized.AudioTypes.BG_MAIN_MENU))
                AudioHolder.instance.PlayBGAudio(Serialized.AudioTypes.BG_MAIN_MENU, true, .75f, 1f);
        }

        protected override void Start()
        {
            base.Start();

            if (GameManager.Instance.Landscape) tabsBar.gameObject.SetActive(!GameManager.Instance.hideTabsBar);
        }

        protected void OnDestroy()
        {
            swipeHandler.onSwipe -= OnSwipe;
        }

        protected void Update()
        {
            //only continue if current opened screen is GameplayScreen
            if (menuController.currentScreen != this || PersistantMenuController.instance.screensStack.Count > 0)
                return;

            if (Input.GetMouseButtonDown(0))
            {
                pointerOrigin = Input.mousePosition;
                onPointerDown.Invoke(pointerOrigin);
            }
            else if (Input.GetMouseButton(0))
                onPointerMove.Invoke(Input.mousePosition - pointerOrigin);
        }

        public override void Open()
        {
            base.Open();

            HeaderScreen.instance.Open();

            if (currentTab == -1) return;

            tabs[currentTab].Open();
            tabsButtons[currentTab].Open(false);
        }

        public override void OnBack()
        {
            base.OnBack();

            Debug.Log("Closing/Suspending the app.");
            Application.Quit();
        }

        public void OpenTab(int index, bool animate)
        {
            if (index == currentTab)
            {
                demoCounter++;
                if (demoCounter == 5)
                {
                    demoCounter = 0;

                    //toggle demo mode
                    SettingsManager.Toggle(SettingsManager.KEY_DEMO_MODE);
                    GamesToastsController.ShowTopToast("Demo mode: " + SettingsManager.Get(SettingsManager.KEY_DEMO_MODE));
                }

                return;
            }
            else
                demoCounter = 0;

            //close previous tab
            if (currentTab > -1)
            {
                tabsButtons[currentTab].Close();
                tabs[currentTab].Close();
            }

            currentTab = index;

            if (animate)
            {
                tabsParentTween.playbackTime = tabsParentTween.defaultPlaybackTime;
                tabsParentTween.FromCurrentPosition();
                tabsParentTween.to = Vector3.left * index * menuController.widthAdjusted;

                tabsParentTween.PlayForward(true);
            }
            else
            {
                tabsParentTween.SetPosition(Vector3.left * index * menuController.widthAdjusted);
            }

            tabs[index].Open();
            tabsButtons[index].Open(true);
        }

        public void OpenTab(MenuTab tab, bool animate)
        {
            if (tab && tabs.Contains(tab)) OpenTab(tabs.IndexOf(tab), animate);
        }

        public void OpenNext(bool animate)
        {
            //if (warp)
            //{
            //    if (currentTab + 1 == tabs.Count)
            //        OpenTab(0, animate);
            //    else
            //        OpenTab(currentTab + 1, animate);
            //}
            //else
            //{
            //if (currentTab + 1 < tabs.Count)
            //{
            //    if (tabsButtons[currentTab + 1].button.interactable)
            //        OpenTab(currentTab + 1, animate);
            //    else
            //    {
            //        int nextIndex = tabsButtons.FindIndex(currentTab)
            //        bool found = false;
            //        for (int tabIndex = currentTab + 1; tabIndex < tabsButtons.Count; tabIndex++)
            //        {

            //        }
            //    }
            //}
            //}
            if (currentTab + 1 < tabs.Count)
            {
                int index = tabsButtons.FindIndex(currentTab + 1, tab => tab.button.interactable);
                if (index > -1) OpenTab(index, animate);
            }
        }

        public void OpenPrevious(bool animate)
        {
            //if (warp)
            //{
            //    if (currentTab - 1 < 0)
            //        OpenTab(tabs.Count - 1, animate);
            //    else
            //        OpenTab(currentTab - 1, animate);
            //}
            //else
            //{
            //if (currentTab - 1 >= 0) OpenTab(currentTab - 1, animate);
            //}
            if (currentTab > 0)
            {
                int index = tabsButtons.FindLastIndex(currentTab - 1, tab => tab.button.interactable);
                if (index > -1) OpenTab(index, animate);
            }
        }

        public bool IsCurrentTab(MenuTab tab)
        {
            return tabs[currentTab] == tab;
        }

        public void OnSwipe(SwipeDirection swipeDirection)
        {
            switch (swipeDirection)
            {
                case SwipeDirection.LEFT:
                    OpenNext(true);
                    break;

                case SwipeDirection.RIGHT:
                    OpenPrevious(true);
                    break;
            }
        }

        //public override void HighlightSelectable()
        //{
        //    tabs[currentTab].HighlightSelectable();
        //}

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (defaultTab && tabs.Contains(defaultTab))
            {
                HeaderScreen.instance.Open();
                OpenTab(tabs.IndexOf(defaultTab), false);
            }
        }
    }
}
