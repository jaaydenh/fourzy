//@vadym udod

using ByteSheep.Events;
using System;
using UnityEngine;

namespace Fourzy._Updates.UI.Helpers
{
    public class SwipeHandler : MonoBehaviour
    {
        public SwipeAxis swipeDirection = SwipeAxis.HORIZONTAL;

        [Tooltip("Distance that pointer need to trave\n to trigger swipe event")]
        public float swipeDistance = 100f;
        [Tooltip("Time during which swipe can occur")]
        public float swipeTime = .2f;

        public AdvancedSwipEvent onSwipe;

        private float swipeTimer;
        private bool checking = false;

        protected void Update()
        {
            if (checking)
                swipeTimer += Time.deltaTime;
        }

        public void OnPointerDow(Vector2 position)
        {
            swipeTimer = 0f;
            checking = true;
        }

        public void OnPointerMove(Vector2 position)
        {
            if (!checking)
                return;

            if (position.magnitude > swipeDistance && swipeTimer <= swipeTime)
            {
                switch (swipeDirection)
                {
                    case SwipeAxis.HORIZONTAL:
                        if (position.x > 0f)
                            onSwipe.Invoke(SwipeDirection.RIGHT);
                        else
                            onSwipe.Invoke(SwipeDirection.LEFT);
                        break;

                    case SwipeAxis.VERTICAL:
                        if (position.y > 0f)
                            onSwipe.Invoke(SwipeDirection.UP);
                        else
                            onSwipe.Invoke(SwipeDirection.DOWN);
                        break;

                    case SwipeAxis.BOTH:
                        if (Mathf.Abs(position.x) > Mathf.Abs(position.y))
                        {
                            if (position.x > 0f)
                                onSwipe.Invoke(SwipeDirection.RIGHT);
                            else
                                onSwipe.Invoke(SwipeDirection.LEFT);
                        }
                        else
                        {
                            if (position.y > 0f)
                                onSwipe.Invoke(SwipeDirection.UP);
                            else
                                onSwipe.Invoke(SwipeDirection.DOWN);
                        }
                        break;
                }

                swipeTimer = 0f;
                checking = false;
            }
        }

        public enum SwipeAxis
        {
            HORIZONTAL,
            VERTICAL,
            BOTH,
        }

        public enum SwipeDirection
        {
            LEFT,
            RIGHT,
            UP,
            DOWN,
        }

        [Serializable] public class AdvancedSwipEvent : AdvancedEvent<SwipeDirection> { }
    }
}
