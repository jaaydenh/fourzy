//@vadym udod

using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Widgets;
using System.Collections.Generic;
using UnityEngine;
using Fourzy._Updates.UI.Helpers;
using ByteSheep.Events;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class TabsScreen : MenuScreen
    {
        public MenuScreen defaultTab;

        public RectTransform tabsParent;
        public List<TabButton> tabsButtons;

        public AdvancedVector2Event onPointerDown;
        public AdvancedVector2Event onPointerMove;

        private List<MenuScreen> tabs;

        private PositionTween tabsParentTween;
        private int currentTab = -1;
        private Vector3 pointerOrigin;

        protected override void Awake()
        {
            base.Awake();

            tabsParentTween = tabsParent.GetComponent<PositionTween>();

            //get tabs
            tabs = new List<MenuScreen>(GetComponentsInChildren<MenuScreen>());
            tabs.Remove(this);
        }

        protected override void Start()
        {
            base.Start();

            if (defaultTab && tabs.Contains(defaultTab))
                tabsButtons[tabs.IndexOf(defaultTab)].Open(false);
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
            {
                onPointerMove.Invoke(Input.mousePosition - pointerOrigin);
            }
        }

        public void OpenTab(int index, bool animate)
        {
            //close previous tab
            if (currentTab > -1)
            {
                tabsButtons[currentTab].Close();
                tabs[currentTab].Close();
            }

            currentTab = index;

            tabsParentTween.playbackTime = animate ? tabsParentTween.defaultPlaybackTime : 0f;
            tabsParentTween.FromCurrentPosition();
            tabsParentTween.to = Vector3.left * index * 750f;  //750 = tab width

            tabsParentTween.PlayForward(true);

            tabs[index].Open();
        }

        public void OpenTab(MenuScreen tab, bool animate)
        {
            if (tab && tabs.Contains(tab))
                OpenTab(tabs.IndexOf(tab), animate);
        }

        public void OpenNext(bool animate)
        {
            if (currentTab + 1 == tabs.Count)
                tabsButtons[0].Open(animate);
            else
                tabsButtons[currentTab + 1].Open(animate);
        }

        public void OpenPrevious(bool animate)
        {
            if (currentTab - 1 < 0)
                tabsButtons[tabs.Count - 1].Open(animate);
            else
                tabsButtons[currentTab - 1].Open(animate);
        }

        public void OnSwipe(SwipeHandler.SwipeDirection swipeDirection)
        {
            switch(swipeDirection)
            {
                case SwipeHandler.SwipeDirection.LEFT:
                    OpenNext(true);
                    break;

                case SwipeHandler.SwipeDirection.RIGHT:
                    OpenPrevious(true);
                    break;
            }
        }
    }
}
