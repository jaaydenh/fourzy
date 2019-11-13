//@vadym udod

using ByteSheep.Events;
using Fourzy._Updates.Audio;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tools;
using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu
{
    /// <summary>
    /// Represents each separate screen in a menu
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class MenuScreen : RoutinesBase
    {
        //[SerializeField]
        //private Selectable defaultSelectable;

        public AdvancedEvent onClose;
        public AdvancedEvent onOpen;
        public AdvancedEvent onBack;

        [Tooltip("What audio to play when onBack invoked")]
        public AudioTypes onBackSfx = AudioTypes.MENU_BACK;
        public bool closePreviousWhenOpened = true;

        [HideInInspector]
        public bool @default;
        [HideInInspector]
        public bool isOpened = false;
        [HideInInspector]
        public bool defaultCalls = true;
        [HideInInspector]
        public MenuController menuController;

        public bool initialized { get; protected set; }
        public RectTransform rectTransform { get; protected set; }
        public LayoutElement layoutElement { get; protected set; }
        public bool inputBlocked => canvasGroup.blocksRaycasts && !canvasGroup.interactable;

        protected CanvasGroup canvasGroup;
        protected TweenBase tween;
        protected List<WidgetBase> widgets;

        public bool interactable
        {
            get
            {
                if (canvasGroup)
                    return canvasGroup.interactable;
                else
                    return gameObject.activeInHierarchy;
            }
        }

        public virtual bool isCurrent
        {
            get
            {
                if (!menuController) return false;

                return menuController.currentScreen == this;
            }
        }

        public virtual bool containsSelected
        {
            get
            {
                if (!EventSystem.current.currentSelectedGameObject) return false;

                return EventSystem.current.currentSelectedGameObject.transform.IsChildOf(transform);
            }
        }

        //public virtual Selectable DefaultSelectable => defaultSelectable;

        protected override void Awake()
        {
            base.Awake();

            menuController = GetComponentInParent<MenuController>();

            canvasGroup = GetComponent<CanvasGroup>();
            tween = GetComponent<TweenBase>();
            rectTransform = GetComponent<RectTransform>();
            layoutElement = GetComponent<LayoutElement>();

            widgets = new List<WidgetBase>(GetComponentsInChildren<WidgetBase>()).Where(widget => widget.GetComponentInParent<MenuScreen>() == this).ToList();
        }

        protected virtual void Start()
        {
            //if @default, set this screen as default one
            if (@default)
            {
                menuController.SetCurrentScreen(this);
                menuController.screensStack.Push(this);

                isOpened = true;
            }

            OnInitialized();

            initialized = true;
        }

        /// <summary>
        /// Open screen
        /// </summary>
        public virtual void Open()
        {
            UpdateWidgets();

            onOpen.Invoke();

            if (isOpened) return;

            isOpened = true;

            if (defaultCalls)
            {
                if (tween) tween.PlayForward(true);

                SetInteractable(true);
                canvasGroup.blocksRaycasts = true;
            }
        }

        /// <summary>
        /// Close screen
        /// </summary>
        public virtual void Close(bool animate = true)
        {
            isOpened = false;

            if (defaultCalls)
            {
                if (tween)
                {
                    if (animate)
                        tween.PlayBackward(true);
                    else
                        tween.Progress(0f, PlaybackDirection.FORWARD);
                }

                SetInteractable(false);
                canvasGroup.blocksRaycasts = false;
            }

            onClose.Invoke();
        }

        //will remove itself from menuController screens stack
        public void CloseSelf(bool animate = true)
        {
            menuController.CloseScreen(this, animate);
        }

        public virtual void ExecuteMenuEvent(MenuEvents menuEvent) { }

        public virtual bool IsWidgetVisible(WidgetBase widget)
        {
            return widget.gameObject.activeInHierarchy && widget.canvasGroup && widget.canvasGroup.alpha > 0f;
        }

        public virtual void BlockInput()
        {
            SetInteractable(false);
            canvasGroup.blocksRaycasts = true;
        }

        public virtual void OnBack()
        {
            onBack.Invoke();

            if (onBackSfx != AudioTypes.NONE)
                AudioHolder.instance.PlaySelfSfxOneShotTracked(onBackSfx);
        }

        public virtual void BackToRoot() => menuController.BackToRoot();

        public void SetInteractable(bool state)
        {
            if (canvasGroup)
                canvasGroup.interactable = state;
        }

        public void UpdateWidgets() => widgets.ForEach(widget => widget._Update());

        //public virtual void HighlightSelectable()
        //{
        //    if (defaultSelectable)
        //        defaultSelectable.Select();
        //    else
        //        EventSystem.current.SetSelectedGameObject(null, null);
        //}

        public virtual List<T> GetWidgets<T>() => widgets.Where(wgt => wgt.GetType() == typeof(T) || wgt.GetType().IsSubclassOf(typeof(T))).Cast<T>().ToList();

        protected virtual void OnInitialized() { }
    }
}