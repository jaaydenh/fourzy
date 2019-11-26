//@vadym udod

using Fourzy._Updates.Tools;
using Fourzy._Updates.Tween;
using FourzyGameModel.Model;
using UnityEngine;

namespace Fourzy._Updates.Mechanics.Board
{
    public class MoveArrow : RoutinesBase
    {
        public GameObject arrow;
        public GameObject arrowGlow;
        public GameObject marker;
        public ParticleSystem _particleSystem;

        private SizeTween arrowSizeTween;
        private ScaleTween arrowScaleTween;
        private AlphaTween arrowAlphaTween;
        private SizeTween glowSizeTween;
        private AlphaTween glowAlphaTween;
        private AlphaTween markerAlphaTween;

        private GameboardView board;
        private bool isShown = false;

        protected override void Awake()
        {
            base.Awake();

            arrowSizeTween = arrow.GetComponent<SizeTween>();
            arrowScaleTween = arrow.GetComponent<ScaleTween>();
            arrowAlphaTween = arrow.GetComponent<AlphaTween>();

            glowSizeTween = arrowGlow.GetComponent<SizeTween>();
            glowAlphaTween = arrowGlow.GetComponent<AlphaTween>();

            markerAlphaTween = marker.GetComponent<AlphaTween>();

            board = GetComponentInParent<GameboardView>();

            arrowSizeTween.playbackTime = GameboardView.HOLD_TIME * .6f;
            arrowAlphaTween.playbackTime = GameboardView.HOLD_TIME * .6f;
            markerAlphaTween.playbackTime = GameboardView.HOLD_TIME;
            glowAlphaTween.playbackTime = GameboardView.HOLD_TIME * 1.35f;
        }

        public void _Reset()
        {
            arrowSizeTween.StopTween(true);
            arrowScaleTween.StopTween(true);
            arrowAlphaTween.StopTween(true);

            glowAlphaTween.StopTween(true);
            glowSizeTween.StopTween(true);

            markerAlphaTween.StopTween(true);

            CancelRoutine("glow");
            CancelRoutine("end");
            isShown = false;
        }

        public void Animate()
        {
            isShown = true;

            arrowSizeTween.PlayForward(true);
            arrowAlphaTween.PlayForward(true);

            glowSizeTween.PlayForward(true);

            markerAlphaTween.playbackTime = arrowSizeTween.playbackTime;
            markerAlphaTween.PlayForward(true);

            StartRoutine("glow", markerAlphaTween.playbackTime - .15f, () => glowAlphaTween.PlayForward(true));

            StartRoutine("end", markerAlphaTween.playbackTime + .3f, () =>
            {
                ParticleExplode();

                Hide();
            });
        }

        public void Hide()
        {
            if (!isShown) return;
            isShown = false;

            arrowScaleTween.PlayForward(true);

            glowAlphaTween.playbackTime = arrowScaleTween.playbackTime;
            glowAlphaTween.PlayBackward(true);

            markerAlphaTween.playbackTime = arrowScaleTween.playbackTime;
            markerAlphaTween.PlayBackward(true);
        }

        public void SetProgress(float value)
        {
            arrowSizeTween.AtProgress(value);
            arrowAlphaTween.AtProgress(value, PlaybackDirection.FORWARD);

            glowSizeTween.AtProgress(value);
            markerAlphaTween.AtProgress(value);
            glowAlphaTween.AtProgress(value, PlaybackDirection.FORWARD);

            if (value > 0f) isShown = true;
        }

        public void Rotate(Direction direction)
        {
            bool origin = false;

            switch (direction)
            {
                case Direction.DOWN:
                    transform.localEulerAngles = Vector3.forward * (origin ? 90f : -90f);
                    break;

                case Direction.UP:
                    transform.localEulerAngles = Vector3.forward * (origin ? -90f : 90f);
                    break;

                case Direction.RIGHT:
                    transform.localEulerAngles = origin ? Vector3.forward * 180f : Vector3.zero;
                    break;

                case Direction.LEFT:
                    transform.localEulerAngles = origin ? Vector3.zero : Vector3.forward * 180f;
                    break;
            }
        }

        public void ParticleExplode() => _particleSystem.Play();

        public void Position(BoardLocation boardLocation) => transform.localPosition = board.BoardLocationToVec2(boardLocation.Mirrored(board.game));
    }
}
