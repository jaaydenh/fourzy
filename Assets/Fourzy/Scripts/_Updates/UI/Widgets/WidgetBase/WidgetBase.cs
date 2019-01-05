//vadym udod

using Fourzy._Updates.Tools;
using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Menu;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Widgets
{
    [RequireComponent(typeof(AlphaTween))]
    [RequireComponent(typeof(PositionTween))]
    [RequireComponent(typeof(ScaleTween))]
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class WidgetBase : RoutinesBase
    {
        [HideInInspector]
        public AlphaTween alphaTween;
        [HideInInspector]
        public PositionTween positionTween;
        [HideInInspector]
        public ScaleTween scaleTween;
        [HideInInspector]
        public CanvasGroup canvasGroup;
        [HideInInspector]
        public RectTransform rectTransform;
        [HideInInspector]
        public LayoutElement layoutElement;
        [HideInInspector]
        public MenuScreen menuScreen;

        protected override void Awake()
        {
            base.Awake();

            alphaTween = GetComponent<AlphaTween>();
            positionTween = GetComponent<PositionTween>();
            scaleTween = GetComponent<ScaleTween>();
            rectTransform = GetComponent<RectTransform>();
            layoutElement = GetComponent<LayoutElement>();

            canvasGroup = GetComponent<CanvasGroup>();

            menuScreen = GetComponentInParent<MenuScreen>();
        }

        public virtual void SetVisibility(float value)
        {
            canvasGroup.alpha = value;
        }

        public virtual void Hide(float time)
        {
            alphaTween.playbackTime = time;
            alphaTween.PlayBackward(true);
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
            if(rectTransform)
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
    }
}
