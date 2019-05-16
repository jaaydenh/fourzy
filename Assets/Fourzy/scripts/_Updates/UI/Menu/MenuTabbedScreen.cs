﻿//@vadym udod

using ByteSheep.Events;
using Fourzy._Updates.Audio;
using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu
{
    public class MenuTabbedScreen : MenuScreen
    {
        public MenuTab defaultTab;
        public bool warp = false;

        public RectTransform tabsParent;
        public List<TabButton> tabsButtons;

        public AdvancedVector2Event onPointerDown;
        public AdvancedVector2Event onPointerMove;

        private List<MenuTab> tabs;
        private SwipeHandler swipeHandler;

        private PositionTween tabsParentTween;
        private int currentTab = -1;
        private Vector3 pointerOrigin;

        protected override void Awake()
        {
            base.Awake();

            tabsParentTween = tabsParent.GetComponent<PositionTween>();
            swipeHandler = GetComponent<SwipeHandler>();

            swipeHandler.onSwipe += OnSwipe;

            //get tabs
            tabs = new List<MenuTab>(GetComponentsInChildren<MenuTab>());

            //play bg audio
            if (!AudioHolder.instance.IsBGAudioPlaying(Serialized.AudioTypes.BG_MAIN_MENU))
                AudioHolder.instance.PlayBGAudio(Serialized.AudioTypes.BG_MAIN_MENU, true, 1f, 1f);
        }

        protected void OnDestroy()
        {
            swipeHandler.onSwipe -= OnSwipe;
        }

        protected override void Start()
        {
            base.Start();

            if (defaultTab && tabs.Contains(defaultTab)) OpenTab(tabs.IndexOf(defaultTab), false);
        }

        protected void Update()
        {
            //only continue if current opened screen is GameplayScreen
            if (menuController.currentScreen != this)
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

            if (currentTab == -1)
                return;
            
            tabs[currentTab].Open();
            tabsButtons[currentTab].Open(false);
        }

        public void OpenTab(int index, bool animate)
        {
            if (index == currentTab) return;

            //close previous tab
            if (currentTab > -1)
            {
                tabsButtons[currentTab].Close();
                tabs[currentTab].Close();
            }

            currentTab = index;

            tabsParentTween.playbackTime = animate ? tabsParentTween.defaultPlaybackTime : 0f;
            tabsParentTween.FromCurrentPosition();
            tabsParentTween.to = Vector3.left * index * menuController.widthAdjusted;

            tabsParentTween.PlayForward(true);

            tabs[index].Open();
            tabsButtons[index].Open(true);
        }

        public void OpenTab(MenuTab tab, bool animate)
        {
            if (tab && tabs.Contains(tab))
                OpenTab(tabs.IndexOf(tab), animate);
        }

        public void OpenNext(bool animate)
        {
            if (warp)
            {
                if (currentTab + 1 == tabs.Count)
                    OpenTab(0, animate);
                else
                    OpenTab(currentTab + 1, animate);
            }
            else
            {
                if (currentTab + 1 < tabs.Count) OpenTab(currentTab + 1, animate);
            }
        }

        public void OpenPrevious(bool animate)
        {
            if (warp)
            {
                if (currentTab - 1 < 0)
                    OpenTab(tabs.Count - 1, animate);
                else
                    OpenTab(currentTab - 1, animate);
            }
            else
            {
                if (currentTab - 1 >= 0) OpenTab(currentTab - 1, animate);
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
    }
}
