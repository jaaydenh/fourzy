//vadym udod

using Fourzy._Updates.Tween;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Widgets
{
    [RequireComponent(typeof(AlphaTween))]
    [RequireComponent(typeof(PositionTween))]
    [RequireComponent(typeof(ScaleTween))]
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class WidgetBase : MonoBehaviour
    {
        protected AlphaTween alphaTween;
        protected PositionTween positionTween;
        protected ScaleTween scaleTween;
        protected CanvasGroup canvasGroup;

        protected void Awake()
        {
            alphaTween = GetComponent<AlphaTween>();
            positionTween = GetComponent<PositionTween>();
            scaleTween = GetComponent<ScaleTween>();

            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void SetVisibility(float value)
        {
            canvasGroup.alpha = value;
        }

        public void Hide(float time)
        {
            alphaTween.playbackTime = time;
            alphaTween.PlayBackward(true);
        }

        public void Show(float time)
        {
            alphaTween.playbackTime = time;
            alphaTween.PlayForward(true);
        }

        public void MoveTo(Vector3 to, float time)
        {
            MoveTo(transform.localPosition, to, time);
        }

        public void MoveTo(Vector3 from, Vector3 to, float time)
        {
            positionTween.from = from;
            positionTween.to = to;
            positionTween.playbackTime = time;

            positionTween.PlayForward(true);
        }

        public void ScaleTo(Vector3 to, float time)
        {
            ScaleTo(transform.localScale, to, time);
        }

        public void ScaleTo(Vector3 from, Vector3 to, float time)
        {
            scaleTween.from = from;
            scaleTween.to = to;
            scaleTween.playbackTime = time;

            scaleTween.PlayForward(true);
        }
    }
}
