//@vadym udod

using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Helpers;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    public class TabButton : MonoBehaviour
    {
        public int tabIndex;

        private bool isOpened = false;
        private TweenBase tween;

        public ButtonExtended button { get; private set; }

        protected void Awake()
        {
            tween = GetComponent<TweenBase>();
            button = GetComponent<ButtonExtended>();
        }

        public void Open(bool animate)
        {
            if (isOpened) return;

            isOpened = true;
            
            tween.playbackTime = animate ? tween.defaultPlaybackTime : 0f;
            tween.PlayForward(true);
        }

        public void Close()
        {
            isOpened = false;
            
            tween.ResetPlaybackTime();
            tween.PlayBackward(true);
        }
    }
}
