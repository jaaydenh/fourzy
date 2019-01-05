using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Menu.Screens;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class TabButton : MonoBehaviour
    {
        public int tabIndex;

        private bool isOpened = false;
        private TabsScreen tabsScreen;
        private ColorTween colorTween;

        protected void Awake()
        {
            tabsScreen = GetComponentInParent<TabsScreen>();
            colorTween = GetComponent<ColorTween>();
        }

        public void Open(bool animate)
        {
            if (isOpened)
                return;

            isOpened = true;
            tabsScreen.OpenTab(tabIndex, animate);
            
            colorTween.playbackTime = animate ? colorTween.defaultPlaybackTime : 0f;
            colorTween.PlayForward(true);
        }

        public void Close()
        {
            isOpened = false;
            
            colorTween.ResetPlaybackTime();
            colorTween.PlayBackward(true);
        }
    }
}
