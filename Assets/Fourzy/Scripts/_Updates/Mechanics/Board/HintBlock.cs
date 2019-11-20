//@vadym udod

using ByteSheep.Events;
using Fourzy._Updates.Tools;
using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Helpers;
using System.Collections;
using UnityEngine;

namespace Fourzy._Updates.Mechanics.Board
{
    [RequireComponent(typeof(AlphaTween))]
    public class HintBlock : RoutinesBase
    {
        public AdvancedEvent onShow;
        public AdvancedEvent onHide;
        public ScaleTween scaleTween;

        private AlphaTween alphaTween;
        private SpriteRenderer spriteRenderer;
        private Collider2D _collider2D;
        private Selectable3D selectable;

        public bool shown { get; private set; }
        public HintBlockMode mode { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            alphaTween = GetComponent<AlphaTween>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            _collider2D = GetComponent<Collider2D>();
            selectable = GetComponent<Selectable3D>();

            shown = false;
        }

        protected void Start()
        {
            alphaTween.AtProgress(0f);
            scaleTween.AtProgress(0f);
        }

        public void Show(float time = .6f)
        {
            shown = true;

            alphaTween.playbackTime = time;
            alphaTween.PlayForward(true);

            scaleTween.playbackTime = time;
            scaleTween.PlayForward(true);

            onShow.Invoke();
        }

        public void Hide(float time = .6f)
        {
            if (!shown) return;

            shown = false;

            alphaTween.playbackTime = time;
            alphaTween.PlayBackward(false);

            scaleTween.playbackTime = time;
            scaleTween.PlayBackward(false);

            onHide.Invoke();
        }

        /// <summary>
        /// Only for EDGE_TAP placement style
        /// </summary>
        public void ShowUnderPointer(float time)
        {
            if (GameManager.Instance.placementStyle != GameManager.PlacementStyle.EDGE_TAP) return;

            Show(time);
        }

        /// <summary>
        /// Only for EDGE_TAP placement style
        /// </summary>
        public void HideUndexPointer(float time)
        {
            if (GameManager.Instance.placementStyle != GameManager.PlacementStyle.EDGE_TAP) return;

            Hide(time);
        }

        public void Animate(float duration = .7f, bool loop = false)
        {
            CancelRoutine("animation");
            StartRoutine("animation", Animation(duration, loop));
        }

        public void CancelAnimation()
        {
            CancelRoutine("animation");
            Hide();
        }

        public void SetColliderState(bool state)
        {
            _collider2D.enabled = state;
        }

        public void SetSelectableState(bool state)
        {
            selectable.enabled = state;
        }

        private IEnumerator Animation(float duration, bool loop)
        {
            do
            {
                Show();

                yield return new WaitForSeconds(duration);

                Hide();

                yield return new WaitForSeconds(duration);
            } while (loop);
        }

        public enum HintBlockMode
        {
            DEFAULT,
            SPELL,
        }
    }
}