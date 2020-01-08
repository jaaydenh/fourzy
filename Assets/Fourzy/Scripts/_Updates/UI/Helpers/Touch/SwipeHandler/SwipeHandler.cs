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
        [Range(0f, 1f)]
        public float swipeDistance = .07f;
        [Tooltip("Time during which swipe can occur")]
        public float swipeTime = .2f;

        public Action<SwipeDirection> onSwipe;

        private float swipeTimer;
        private bool checking = false;
        private float _swipeDistance;

        protected void Awake()
        {
            _swipeDistance = Mathf.Min(Screen.width, Screen.height) * swipeDistance;
        }

        protected void Update()
        {
            if (checking)
                swipeTimer += Time.deltaTime;
        }

        public void OnPointerDown(Vector2 position)
        {
            swipeTimer = 0f;
            checking = true;
        }

        public void OnPointerMove(Vector2 position)
        {
            if (!checking)
                return;

            if (swipeTimer <= swipeTime)
            {
                switch (swipeDirection)
                {
                    case SwipeAxis.HORIZONTAL:
                        if (position.x > _swipeDistance)
                            InvokeSwipe(SwipeDirection.RIGHT);
                        else if (position.x <= -_swipeDistance)
                            InvokeSwipe(SwipeDirection.LEFT);
                        break;

                    case SwipeAxis.VERTICAL:
                        if (position.y >= _swipeDistance)
                            InvokeSwipe(SwipeDirection.UP);
                        else if (position.y <= -_swipeDistance)
                            InvokeSwipe(SwipeDirection.DOWN);
                        break;

                    case SwipeAxis.BOTH:
                        if (Mathf.Abs(position.x) > Mathf.Abs(position.y))
                        {
                            if (position.x > _swipeDistance)
                                InvokeSwipe(SwipeDirection.RIGHT);
                            else if (position.x <= -_swipeDistance)
                                InvokeSwipe(SwipeDirection.LEFT);
                        }
                        else
                        {
                            if (position.y >= _swipeDistance)
                                InvokeSwipe(SwipeDirection.UP);
                            else if (position.y <= -_swipeDistance)
                                InvokeSwipe(SwipeDirection.DOWN);
                        }
                        break;
                }
            }
            else
                checking = false;
        }

        public void InvokeSwipe(SwipeDirection swipeDirection)
        {
            onSwipe?.Invoke(swipeDirection);

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

    [Serializable]
    public enum SwipeDirection
    {
        LEFT,
        RIGHT,
        UP,
        DOWN,
    }

    [Serializable] public class AdvancedSwipeEvent : AdvancedEvent<SwipeDirection> { }
}
