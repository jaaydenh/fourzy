//modded @vadym udod

using DG.Tweening;
using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.Tools;
using Fourzy._Updates.UI.Helpers;
using System.Collections;
using UnityEngine;

namespace Fourzy
{
    public class GamePieceView : MonoBehaviour
    {
        private const int indexBaseLayer = 0;
        private const int indexEyeMouthLayer = 1;
        private const int indexMaterialLayer = 2;
        
        public SpriteRenderer body;
        public Animator pieceAnimator;

        private int h_win = Animator.StringToHash("win");
        private int h_blink = Animator.StringToHash("blink");

        private int h_Idle = Animator.StringToHash("Idle");
        private int h_ShowWinningOutline = Animator.StringToHash("ShowWinningOutline");
        private int h_Jumping = Animator.StringToHash("Jumping");
        private int h_Sleep = Animator.StringToHash("Sleep");
        private int h_WakeUp = Animator.StringToHash("WakeUp");

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

        public SpriteRenderer[] sprites { get; private set; }
        public CustomOutline customOutline { get; private set; }
        public RectTransform parentRectTransform { get; private set; }
        public GameBoardView gameboard { get; private set; }

        private void Awake()
        {
            Debug.Assert(pieceAnimator != null, "Setup pieceAnimator for GamePieceView in editor");
            Debug.Assert(body != null, "Setup body for GamePieceView in editor");

            if (pieceAnimator == null)
                pieceAnimator = GetComponent<Animator>();

            if (body == null)
                body = transform.Find("Sprites").Find("Body").GetComponent<SpriteRenderer>();

            sprites = GetComponentsInChildren<SpriteRenderer>(true);
            parentRectTransform = GetComponentInParent<RectTransform>();
            gameboard = GetComponentInParent<GameBoardView>();

            if (parentRectTransform)
            {
                foreach (SpriteRenderer spriteRendederer in sprites)
                    spriteRendederer.gameObject.AddComponent<SrpiteToImage>();

                //size it
                if (gameboard)
                    transform.localScale = gameboard.step * .9f;
            }
            else
                transform.localScale = Vector3.one;
        }

        public void SetSortingLayer(int layer)
        {
            foreach (SpriteRenderer s in sprites)
            {
                s.sortingLayerID = layer;
            }
        }

        public void UseSecondaryColor(bool value)
        {

        }

        public void PutMovementDirection(int x, int y)
        {
            moveDirection = this.ChooseDirection(x, y);

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
            {
                pieceAnimator.Play(h_Idle, indexEyeMouthLayer);
            }
            else
            {
                pieceAnimator.CrossFade(h_Idle, 0.1f, indexEyeMouthLayer);
            }

        }

        private MoveDirection ChooseDirection(int x, int y)
        {
            MoveDirection direction = MoveDirection.IDLE;
            if (x > 0)
            {
                direction = MoveDirection.RIGHT;
            }
            else if (x < 0)
            {
                direction = MoveDirection.LEFT;
            }

            if (y > 0)
            {
                direction = MoveDirection.DOWN;
            }
            else if (y < 0)
            {
                direction = MoveDirection.TOP;
            }

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
            body.DOFade(alpha, fadeTime);
            foreach (SpriteRenderer sr in sprites)
            {
                sr.DOFade(alpha, fadeTime);
            }
        }

        public void SetAlpha(float alpha)
        {
            body.SetAlpha(alpha);
            foreach (SpriteRenderer sr in sprites)
            {
                sr.SetAlpha(alpha);
            }
        }

        public void SetupZOrder(int zorder)
        {
            foreach (SpriteRenderer sr in sprites)
            {
                sr.sortingOrder = zorder + 1;
            }

            body.sortingOrder = zorder;
        }

        public void PlayWinAnimation(float delay)
        {
            pieceAnimator.Play(h_ShowWinningOutline);

            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(delay);
            sequence.AppendCallback(() =>
            {
                pieceAnimator.SetBool(h_win, true);
            });
        }

        public void ShowTurnAnimation()
        {
            StartCoroutine(ShowTurnAnimationRoutine());

            //check if piece is part of canvas
            if (parentRectTransform)
            {
                //add outline to body
                if (!customOutline)
                    customOutline = body.gameObject.AddComponent<CustomOutline>();

                customOutline.outlineColor = Color.green;
                customOutline.Animate(0f, 1f, 2f, true);
            }
        }

        public void StopTurnAnimation()
        {
            if (parentRectTransform)
            {
                if (customOutline)
                    customOutline.StopAnimation();
            }

            StopAllCoroutines();
            
            pieceAnimator.Play(h_Sleep, indexEyeMouthLayer);
            pieceAnimator.CrossFade(h_Idle, 0.35f, indexBaseLayer);
        }

        private IEnumerator ShowTurnAnimationRoutine()
        {
            pieceAnimator.Play(h_WakeUp, indexEyeMouthLayer);

            int countOfJumps = 3;
            yield return StartCoroutine(JumpRoutine(countOfJumps));
            yield return StartCoroutine(BlinkingRoutine());
        }

        public void ShowUIWinAnimation()
        {
            StopAllCoroutines();
            StartCoroutine(ShowUIWinAnimationRoutine());
        }

        private IEnumerator ShowUIWinAnimationRoutine()
        {
            yield return this.StartCoroutine(JumpRoutine(5));
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

        public void StartBlinking()
        {
            StartCoroutine(BlinkingRoutine());
        }

        private IEnumerator BlinkingRoutine()
        {
            float t = 0;
            float nextBlink = 2.0f;
            while (true)
            {
                t += Time.deltaTime;
                if (t > nextBlink)
                {
                    Blink();
                    nextBlink = Random.Range(5.0f, 15.0f);
                    t = 0;
                }
                yield return null;
            }
        }

        public void Blink()
        {
            pieceAnimator.SetTrigger(h_blink);
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