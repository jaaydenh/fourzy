//@vadym udod
//used to show toasts to players

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.UI.Toasts
{
    /// <summary>
    /// Used to displat ingame toast messages
    /// </summary>
    public class GamesToastsController : MonoBehaviour
    {
        public static GamesToastsController instance;

        public int maxToastsVisible = 2;
        public ToastStylePair[] prefabs;

        /// <summary>
        /// List of all spawned toast
        /// </summary>
        private Dictionary<ToastStyle, List<GameToast>> toasts;
        private Dictionary<ToastStyle, ToastStylePair> toastPrefabFastAccess;

        public Queue<GameToast> activeToasts { get; private set; }
        public Queue<GameToast> movableToast { get; private set; }

        public Queue<GameToast> topActiveToasts { get; private set; }
        public Queue<GameToast> topMovableToast { get; private set; }

        public void Awake()
        {
            if (instance != null)
                return;

            instance = this;

            toasts = new Dictionary<ToastStyle, List<GameToast>>();
            activeToasts = new Queue<GameToast>();
            movableToast = new Queue<GameToast>();

            topActiveToasts = new Queue<GameToast>();
            topMovableToast = new Queue<GameToast>();

            toastPrefabFastAccess = new Dictionary<ToastStyle, ToastStylePair>();
            foreach (ToastStylePair pair in prefabs)
                toastPrefabFastAccess.Add(pair.style, pair);
        }

        protected void Update()
        {
#if UNITY_EDITOR
            //if (Input.GetKeyDown(KeyCode.A))
            //    ShowToast(ToastStyle.ACTION_WARNING, "Generic Message");
#endif
        }

        public void ShowToast(GameToast newToast)
        {
            newToast.transform.SetAsLastSibling();
            newToast.ShowToast();

            //fastfade if needed
            if (activeToasts.Count == maxToastsVisible)
                activeToasts.Dequeue().Hide(.3f);

            //move other up
            foreach (GameToast toast in movableToast)
                toast.MoveRelative(newToast.toastStepDistance, .4f);

            activeToasts.Enqueue(newToast);
            movableToast.Enqueue(newToast);
        }

        public void ShowTopToast(GameToast newToast)
        {
            newToast.transform.SetAsLastSibling();
            newToast.ShowToast();

            //fastfade if needed
            if (topActiveToasts.Count == maxToastsVisible)
                topActiveToasts.Dequeue().Hide(.3f);

            //move other up
            foreach (GameToast toast in topMovableToast)
                toast.MoveRelative(newToast.toastStepDistance, .4f);

            topActiveToasts.Enqueue(newToast);
            topMovableToast.Enqueue(newToast);
        }

        public GameToast GetToast(ToastStyle style)
        {
            //check if toasts pool with this style exists
            if (!toasts.ContainsKey(style))
                toasts.Add(style, new List<GameToast>());

            //check if there is an available toasts, if not, create one
            foreach (GameToast toast in toasts[style])
                if (toast.available)
                    return toast;

            GameToast newToast = Instantiate(toastPrefabFastAccess[style].prefab, transform);

            newToast.SetAlpha(0f);
            newToast.transform.localScale = Vector3.one;

            toasts[style].Add(newToast);

            return newToast;
        }

        /// <summary>
        /// Displat toast with given message and icon
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="sprite">Icon</param>
        public static void ShowToast(ToastStyle toastStyle, string message, Sprite sprite)
        {
            if (!instance)
                return;

            GameToast toast = instance.GetToast(toastStyle);
            toast.SetData(instance, message, sprite);

            instance.ShowToast(toast);
        }

        /// <summary>
        /// Display toast with given message and icon
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="iconStyle">Icon style</param>
        public static void ShowToast(ToastStyle toastStyle, string message, ToastIconStyle iconStyle)
        {
            if (!instance)
                return;

            GameToast toast = instance.GetToast(toastStyle);
            toast.SetData(instance, message, iconStyle);

            instance.ShowToast(toast);
        }

        /// <summary>
        /// Display toast with given message
        /// </summary>
        /// <param name="message">Message</param>
        public static void ShowToast(ToastStyle toastStyle, string message)
        {
            if (!instance)
                return;

            GameToast toast = instance.GetToast(toastStyle);
            toast.SetData(instance, message);

            instance.ShowToast(toast);
        }

        public static void ShowTopToast(string message)
        {
            if (!instance)
                return;

            GameToast toast = instance.GetToast(ToastStyle.TOP_TOAST);
            toast.SetData(instance, message);

            instance.ShowToast(toast);
        }

        [Serializable]
        public class ToastStylePair
        {
            public GameToast prefab;
            public ToastStyle style;
        }

        public enum ToastStyle
        {
            ACTION_WARNING,
            INFO_TOAST,
            TOP_TOAST,
        }
    }
}