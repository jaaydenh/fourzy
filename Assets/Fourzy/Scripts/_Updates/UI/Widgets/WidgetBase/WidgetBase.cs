//vadym udod

using Fourzy._Updates.Tools;
using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Menu;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    [RequireComponent(typeof(AlphaTween))]
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class WidgetBase : RoutinesBase
    {
        public bool initialized { get; private set; }
        public MenuScreen menuScreen { get; set; }

        public LayoutElement layoutElement { get; protected set; }
        public RectTransform rectTransform { get; protected set; }
        public CanvasGroup canvasGroup { get; protected set; }
        public ScaleTween scaleTween { get; protected set; }
        public PositionTween positionTween { get; protected set; }
        public AlphaTween alphaTween { get; protected set; }

        public bool visible => menuScreen.IsWidgetVisible(this);

        protected override void Awake()
        {
            Initialize();
        }

        public virtual void SetAlpha(float value)
        {
            canvasGroup.alpha = value;
        }

        public virtual void Hide(float time)
        {
            if (time == 0f)
                alphaTween.SetAlpha(0f);
            else
            {
                alphaTween.playbackTime = time;
                alphaTween.PlayBackward(true);
            }
        }

        public virtual void Show(float time)
        {
            alphaTween.playbackTime = time;
            alphaTween.PlayForward(true);
        }

        public virtual void MoveTo(Vector3 to, float time)
        {
            if (rectTransform)
                MoveTo(rectTransform.anchoredPosition, to, time);
            else
                MoveTo(transform.localPosition, to, time);
        }

        public virtual void MoveTo(Vector3 from, Vector3 to, float time)
        {
            positionTween.from = from;
            positionTween.to = to;
            positionTween.playbackTime = time;

            positionTween.PlayForward(true);
        }

        public virtual void MoveRelative(Vector3 to, float time)
        {
            if (rectTransform)
                MoveTo(rectTransform.anchoredPosition, positionTween.to + to, time);
            else
                MoveTo(transform.localPosition, positionTween.to + to, time);
        }

        public virtual void ScaleTo(Vector3 to, float time)
        {
            ScaleTo(transform.localScale, to, time);
        }

        public virtual void ScaleTo(Vector3 from, Vector3 to, float time)
        {
            scaleTween.from = from;
            scaleTween.to = to;
            scaleTween.playbackTime = time;

            scaleTween.PlayForward(true);
        }

        public virtual void SetInteractable(bool state)
        {
            if (!canvasGroup)
                return;

            canvasGroup.interactable = state;
        }

        public virtual void BlockRaycast(bool state)
        {
            if (!canvasGroup)
                return;

            canvasGroup.blocksRaycasts = state;
        }

        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }

        public virtual void _Update() { }

        public virtual void Initialize()
        {
            if (initialized) return;

            initialized = true;
            base.Awake();

            OnInitialized();
        }

        protected virtual void OnInitialized()
        {
            alphaTween = GetComponent<AlphaTween>();
            positionTween = GetComponent<PositionTween>();
            scaleTween = GetComponent<ScaleTween>();

            if (alphaTween) alphaTween.Initialize();

            if (positionTween) positionTween.Initialize();

            if (scaleTween) scaleTween.Initialize();

            rectTransform = GetComponent<RectTransform>();
            layoutElement = GetComponent<LayoutElement>();

            canvasGroup = GetComponent<CanvasGroup>();
            
            menuScreen = GetComponentInParent<MenuScreen>();
        }
    }
}
