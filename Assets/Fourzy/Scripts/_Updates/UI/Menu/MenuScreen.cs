//@vadym udod

using ByteSheep.Events;
using Fourzy._Updates.Audio;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tools;
using Fourzy._Updates.Tween;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu
{
    /// <summary>
    /// Represents each separate screen in a menu
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class MenuScreen : RoutinesBase
    {
        public AdvancedEvent onClose;
        public AdvancedEvent onOpen;
        public AdvancedEvent onBack;

        [Tooltip("What audio to play when onBack invoked")]
        public AudioTypes onBackSfx = AudioTypes.MENU_BACK;

        [HideInInspector]
        public bool @default;
        [HideInInspector]
        public bool isOpened = false;
        [HideInInspector]
        public bool defaultCalls = true;
        [HideInInspector]
        public MenuController menuController;
        [HideInInspector]
        public RectTransform rectTransform;
        [HideInInspector]
        public LayoutElement layoutElement;

        protected CanvasGroup canvasGroup;
        protected TweenBase tween;

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

        protected override void Awake()
        {
            base.Awake();

            menuController = GetComponentInParent<MenuController>();

            canvasGroup = GetComponent<CanvasGroup>();
            tween = GetComponent<TweenBase>();
            rectTransform = GetComponent<RectTransform>();
            layoutElement = GetComponent<LayoutElement>();
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
        }

        /// <summary>
        /// Open screen
        /// </summary>
        public virtual void Open()
        {
            if (isOpened)
                return;

            isOpened = true;

            if (defaultCalls)
            {
                if (tween)
                    tween.PlayForward(true);

                SetInteractable(true);
                canvasGroup.blocksRaycasts = true;
            }

            onOpen.Invoke();
        }

        /// <summary>
        /// Close screen
        /// </summary>
        public virtual void Close()
        {
            isOpened = false;

            if (defaultCalls)
            {
                if (tween)
                    tween.PlayBackward(true);

                SetInteractable(false);
                canvasGroup.blocksRaycasts = false;
            }

            onClose.Invoke();
        }

        public virtual void OnBack()
        {
            onBack.Invoke();

            if (onBackSfx != AudioTypes.NONE)
                AudioHolder.instance.PlaySelfSfxOneShotTracked(onBackSfx);
        }

        public void SetInteractable(bool state)
        {
            if (canvasGroup)
                canvasGroup.interactable = state;
        }
    }
}