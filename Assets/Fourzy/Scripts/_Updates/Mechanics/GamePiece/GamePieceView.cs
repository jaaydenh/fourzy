//modded @vadym udod

using DG.Tweening;
using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.Tools;
using Fourzy._Updates.UI.Helpers;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Fourzy._Updates.Mechanics._GamePiece
{
    public class GamePieceView : RoutinesBase
    {
        private const int indexBaseLayer = 0;
        private const int indexEyeMouthLayer = 1;
        private const int indexMaterialLayer = 2;

        public SpriteRenderer body;
        public Animator pieceAnimator;

        private int h_win = Animator.StringToHash("win");

        private int h_Idle = Animator.StringToHash("Idle");
        private int h_Jumping = Animator.StringToHash("Jumping");

        private int h_MovingHorizontal = Animator.StringToHash("MovingHorizontal");
        private int h_MovingVertical = Animator.StringToHash("MovingHorizontal");
        private int h_RightHit = Animator.StringToHash("RightHit");
        private int h_LeftHit = Animator.StringToHash("LeftHit");
        private int h_BottomHit = Animator.StringToHash("BottomHit");
        private int h_TopHit = Animator.StringToHash("TopHit");

        private int h_MoveLeft = Animator.StringToHash("MoveLeft");
        private int h_MoveRight = Animator.StringToHash("MoveRight");
        private int h_MoveTop = Animator.StringToHash("MoveTop");
        private int h_MoveBottom = Animator.StringToHash("MoveDown");

        private MoveDirection moveDirection = MoveDirection.IDLE;

        public GamePieceMouth mouth { get; private set; }
        public GamePieceEyes eyes { get; private set; }
        public GameBoardView gameboard { get; private set; }

        private SpriteRenderer[] renderers;
        private SortingGroup sortingGroup;
        private OutlineBase outline;
        private RectTransform parentRectTransform;

        protected override void Awake()
        {
            base.Awake();

            Debug.Assert(pieceAnimator != null, "Setup pieceAnimator for GamePieceView in editor");
            Debug.Assert(body != null, "Setup body for GamePieceView in editor");

            if (pieceAnimator == null)
                pieceAnimator = GetComponent<Animator>();

            if (body == null)
                body = transform.Find("Sprites").Find("Body").GetComponent<SpriteRenderer>();

            renderers = GetComponentsInChildren<SpriteRenderer>(true);
            parentRectTransform = GetComponentInParent<RectTransform>();
            gameboard = GetComponentInParent<GameBoardView>();
            mouth = GetComponentInChildren<GamePieceMouth>();
            eyes = GetComponentInChildren<GamePieceEyes>();
            sortingGroup = GetComponent<SortingGroup>();

            if (parentRectTransform)
            {
                foreach (SpriteRenderer spriteRendederer in renderers)
                    spriteRendederer.gameObject.AddComponent<SpriteToImage>();

                //size it
                if (gameboard)
                    transform.localScale = gameboard.step * .9f;
            }
            else
                transform.localScale = Vector3.one;
        }

        public void SetSortingLayer(int layer)
        {
            sortingGroup.sortingOrder = layer;
        }

        public void UseSecondaryColor(bool value)
        {
            //if (parentRectTransform)
            //    customOutline.outlineColor = game
        }

        public void PutMovementDirection(int x, int y)
        {
            moveDirection = ChooseDirection(x, y);

            const float transitionTime = 0.1f;
            switch (moveDirection)
            {
                case MoveDirection.RIGHT:
                    pieceAnimator.CrossFade(h_MovingHorizontal, transitionTime, indexBaseLayer);
                    pieceAnimator.CrossFade(h_MoveRight, transitionTime, indexEyeMouthLayer);
                    break;
                case MoveDirection.LEFT:
                    pieceAnimator.CrossFade(h_MovingHorizontal, transitionTime, indexBaseLayer);
                    pieceAnimator.CrossFade(h_MoveLeft, transitionTime, indexEyeMouthLayer);
                    break;
                case MoveDirection.DOWN:
                    pieceAnimator.CrossFade(h_MovingVertical, transitionTime, indexBaseLayer);
                    pieceAnimator.CrossFade(h_MoveBottom, transitionTime, indexEyeMouthLayer);
                    break;
                case MoveDirection.TOP:
                    pieceAnimator.CrossFade(h_MovingVertical, transitionTime, indexBaseLayer);
                    pieceAnimator.CrossFade(h_MoveTop, transitionTime, indexEyeMouthLayer);
                    break;
            }
        }

        public void PlayFinishMovement(bool animateHit)
        {
            const float transitionTime = 0.03f;
            if (animateHit)
            {
                switch (moveDirection)
                {
                    case MoveDirection.RIGHT:
                        pieceAnimator.CrossFade(h_RightHit, transitionTime, indexBaseLayer);
                        break;
                    case MoveDirection.LEFT:
                        pieceAnimator.CrossFade(h_LeftHit, transitionTime, indexBaseLayer);
                        break;
                    case MoveDirection.DOWN:
                        pieceAnimator.CrossFade(h_BottomHit, transitionTime, indexBaseLayer);
                        break;
                    case MoveDirection.TOP:
                        pieceAnimator.CrossFade(h_TopHit, transitionTime, indexBaseLayer);
                        break;
                }
            }
            else
            {
                pieceAnimator.CrossFade(h_Idle, transitionTime, indexBaseLayer);
            }

            if (pieceAnimator.GetCurrentAnimatorStateInfo(indexEyeMouthLayer).shortNameHash == h_Idle)
                pieceAnimator.Play(h_Idle, indexEyeMouthLayer);
            else
                pieceAnimator.CrossFade(h_Idle, 0.1f, indexEyeMouthLayer);
        }

        private MoveDirection ChooseDirection(int x, int y)
        {
            MoveDirection direction = MoveDirection.IDLE;
            if (x > 0)
                direction = MoveDirection.RIGHT;
            else if (x < 0)
                direction = MoveDirection.LEFT;

            if (y > 0)
                direction = MoveDirection.DOWN;
            else if (y < 0)
                direction = MoveDirection.TOP;

            return direction;
        }

        public void FadeAfterPit()
        {
            transform.DOScale(Vector3.zero, 1.0f);

            //this.gameObject.SetActive(false);
            // GamePiece animation
        }

        public void Fade(float alpha, float fadeTime)
        {
            if (!parentRectTransform)
                foreach (SpriteRenderer sr in renderers)
                    sr.DOFade(alpha, fadeTime);
        }

        public void SetAlpha(float alpha)
        {
            if (!parentRectTransform)
                foreach (SpriteRenderer sr in renderers)
                    sr.SetAlpha(alpha);
        }

        public void PlayWinAnimation(float delay)
        {
            ShowOutline(false);

            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(delay);
            sequence.AppendCallback(() =>
            {
                pieceAnimator.SetBool(h_win, true);
            });
        }

        public void ShowTurnAnimation()
        {
            WakeUp();

            CancelRoutine("jumping");
            StartRoutine("jumping", JumpRoutine(3), () =>
            {
                StartRoutine("blinking", BlinkingRoutine(), null, () => { eyes.SetState(GamePieceEyes.EyesState.OPENED); });
            },
            () => {
                CancelRoutine("blinking");
            });

            ShowOutline(true);
        }

        public void StopTurnAnimation()
        {
            HideOutline(false);

            StopAllCoroutines();

            Sleep();
            pieceAnimator.CrossFade(h_Idle, 0.35f, indexBaseLayer);
        }

        public void ShowUIWinAnimation()
        {
            StopAllCoroutines();
            StartCoroutine(ShowUIWinAnimationRoutine());
        }

        public void Blink()
        {
            eyes.Blink(Random.Range(.25f, .45f));
        }

        public void WakeUp()
        {
            if (!eyes.IsRoutineActive("blink"))
                eyes.SetState(GamePieceEyes.EyesState.OPENED);

            mouth.SetState(GamePieceMouth.MouthState.OPENED);
        }

        public void Sleep()
        {
            eyes.CancelRoutine("blink");
            eyes.SetState(GamePieceEyes.EyesState.CLOSED);

            if (mouth.statesFastAccess.ContainsKey(GamePieceMouth.MouthState.SLEEPY))
                mouth.SetState(GamePieceMouth.MouthState.SLEEPY);
            else
                mouth.SetState(GamePieceMouth.MouthState.CLOSED);
        }

        public void ShowOutline(bool animate)
        {
            //check if piece is part of canvas
            if (parentRectTransform)
            {
                //add outline to body
                if (!outline)
                    outline = body.gameObject.AddComponent<UIOutline>();
            }
            else
            {
                if (!outline)
                    outline = body.gameObject.AddComponent<SpriteRendererOutline>();
            }

            outline.outlineColor = Color.green;
            outline.Animate(0f, 1f, 1f, animate);
        }

        public void HideOutline(bool force)
        {
            if (outline)
            {
                if (force)
                    outline.HideOutline();
                else
                    outline.StopAnimation();
            }
        }

        public void StartBlinking()
        {
            if (!gameObject.activeInHierarchy)
                return;

            StartCoroutine(BlinkingRoutine());
        }

        private IEnumerator ShowUIWinAnimationRoutine()
        {
            yield return StartCoroutine(JumpRoutine(5));
            //this.PlayWinAnimation(0.0f);
        }

        private IEnumerator JumpRoutine(int count)
        {
            pieceAnimator.Play(h_Jumping);

            while (pieceAnimator.GetCurrentAnimatorStateInfo(indexBaseLayer).shortNameHash != h_Jumping) yield return true;

            AnimatorStateInfo stateInfo = pieceAnimator.GetCurrentAnimatorStateInfo(indexBaseLayer);

            yield return new WaitForSeconds(stateInfo.length * count);

            pieceAnimator.Play(h_Idle, indexBaseLayer);
        }

        private IEnumerator BlinkingRoutine()
        {
            yield return new WaitForSeconds(Random.Range(1f, 5f));

            while (true)
            {
                Blink();
                yield return new WaitForSeconds(Random.Range(5f, 15f));
            }
        }
    }

    enum MoveDirection
    {
        LEFT,
        RIGHT,
        TOP,
        DOWN,
        IDLE
    }
}