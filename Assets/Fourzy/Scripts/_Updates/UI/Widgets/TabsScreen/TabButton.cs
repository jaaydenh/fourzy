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
        private ValueTween sizeTween;
        private ColorTween colorTween;

        protected void Awake()
        {
            tabsScreen = GetComponentInParent<TabsScreen>();
            sizeTween = GetComponent<ValueTween>();
            colorTween = GetComponent<ColorTween>();
        }

        public void Open(bool animate)
        {
            if (isOpened)
                return;

            isOpened = true;
            tabsScreen.OpenTab(tabIndex, animate);

            sizeTween.playbackTime = animate ? sizeTween.defaultPlaybackTime : 0f;
            colorTween.playbackTime = animate ? colorTween.defaultPlaybackTime : 0f;

            sizeTween.PlayForward(true);
            colorTween.PlayForward(true);
        }

        public void Close()
        {
            isOpened = false;

            sizeTween.ResetPlaybackTime();
            colorTween.ResetPlaybackTime();

            sizeTween.PlayBackward(true);
            colorTween.PlayBackward(true);
        }
    }
}
