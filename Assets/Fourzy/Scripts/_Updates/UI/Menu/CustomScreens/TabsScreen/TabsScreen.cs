//@vadym udod

using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Widgets;
using System.Collections.Generic;
using UnityEngine;
using Fourzy._Updates.UI.Helpers;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class TabsScreen : MenuScreen
    {
        public MenuScreen defaultTab;

        public RectTransform tabsParent;
        public List<TabButton> tabsButtons;

        private List<MenuScreen> tabs;

        private PositionTween tabsParentTween;
        private int currentTab = -1;

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

        public void OpenTab(int index, bool animate)
        {
            //close previous tab
            if (currentTab > -1)
                tabsButtons[currentTab].Close();

            currentTab = index;

            tabsParentTween.playbackTime = animate ? tabsParentTween.defaultPlaybackTime : 0f;
            tabsParentTween.FromCurrentPosition();
            tabsParentTween.to = Vector3.left * index * 750f;  //750 = tab width

            tabsParentTween.PlayForward(true);
        }

        public void OpenTab(MenuScreen tab, bool animate)
        {
            if (tab && tabs.Contains(tab))
                OpenTab(tabs.IndexOf(tab), animate);
        }

        public void OpenNext(bool animate)
        {
            if (currentTab + 1 == tabs.Count)
                tabsButtons[0].Open(false);
            else
                tabsButtons[currentTab + 1].Open(false);
        }

        public void OpenPrevious(bool animate)
        {
            if (currentTab - 1 < 0)
                tabsButtons[tabs.Count - 1].Open(false);
            else
                tabsButtons[currentTab - 1].Open(false);
        }

        public void OnSwipe(SwipeHandler.SwipeDirection swipeDirection)
        {
            switch(swipeDirection)
            {
                case SwipeHandler.SwipeDirection.LEFT:
                    OpenPrevious(true);
                    break;

                case SwipeHandler.SwipeDirection.RIGHT:
                    OpenNext(true);
                    break;
            }
        }
    }
}
