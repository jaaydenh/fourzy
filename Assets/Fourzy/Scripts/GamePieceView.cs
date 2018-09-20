using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Fourzy
{
    enum MoveDirection
    {
        LEFT,
        RIGHT,
        TOP,
        DOWN,
        IDLE
    }

    public class GamePieceView : MonoBehaviour
    {
        public Color OutlineColor;

        [SerializeField]
        private SpriteRenderer body;

        [SerializeField]
        private Animator pieceAnimator;

        [SerializeField]
        private Shader outlineShader;

        private Transform cachedTransform;

        private SpriteRenderer[] sprites;

        private Material bodyMaterial;
        private Material bodyOutlineMaterial;

        private int h_HSVAAdjust = Shader.PropertyToID("_HSVAAdjust");
        private int h_OutlineColor = Shader.PropertyToID("_OutlineColor");

        private int h_win = Animator.StringToHash("win");
        private int h_blink = Animator.StringToHash("blink");

        private int h_Idle = Animator.StringToHash("Idle");
        private int h_ShowWinningOutline = Animator.StringToHash("ShowWinningOutline");
        private int h_HideOutline = Animator.StringToHash("HideOutline");
        private int h_ShowOutline = Animator.StringToHash("ShowOutline");
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

        private const int indexBaseLayer = 0;
        private const int indexEyeMouthLayer = 1;
        private const int indexMaterialLayer = 2;

        MoveDirection moveDirection = MoveDirection.IDLE;

        private void Awake()
        {
            Debug.Assert(pieceAnimator != null, "Setup pieceAnimator for GamePieceView in editor");
            Debug.Assert(body != null, "Setup body for GamePieceView in editor");
            Debug.Assert(outlineShader != null, "Setup OutlineShader for GamePieceView in editor");

            if (pieceAnimator == null)
            {
                pieceAnimator = this.GetComponent<Animator>();
            }
            if (body == null)
            {
                body = this.transform.Find("Sprites").Find("Body").GetComponent<SpriteRenderer>();
            }

            cachedTransform = transform;
            bodyMaterial = body.material;
            bodyOutlineMaterial = new Material(outlineShader);
            bodyOutlineMaterial.SetVector(h_HSVAAdjust, bodyMaterial.GetVector(h_HSVAAdjust));
            bodyOutlineMaterial.SetColor(h_OutlineColor, OutlineColor);

            sprites = this.GetComponentsInChildren<SpriteRenderer>(true);
        }

        public void SetupHSVColor(Vector4 vec)
        {
            bodyMaterial.SetVector(h_HSVAAdjust, vec);
            bodyOutlineMaterial.SetVector(h_HSVAAdjust, vec);

            foreach (SpriteRenderer s in sprites)
            {
                s.material.SetVector(h_HSVAAdjust, vec);
            }
        }

        public void SetupOutlineColor(Color color)
        {
            bodyOutlineMaterial.SetColor(h_OutlineColor, color);
        }

        public void PutMovementDirection(int x, int y)
        {
            moveDirection = this.ChooseDirection(x, y);

            float transitionTime = 0.1f;
            switch(moveDirection)
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
            float transitionTime = 0.03f;
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

            pieceAnimator.CrossFade(h_Idle, 0.1f, indexEyeMouthLayer);
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
            cachedTransform.DOScale(Vector3.zero, 1.0f);

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
            foreach(SpriteRenderer sr in sprites)
            {
                sr.sortingOrder = zorder + 1;
            }

            body.sortingOrder = zorder;
        }

        public void PlayWinAnimation(float delay)
        {
            body.sharedMaterial = bodyOutlineMaterial;

            pieceAnimator.Play(h_ShowWinningOutline);

            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(delay);
            sequence.AppendCallback(() => {
                pieceAnimator.SetBool(h_win, true);
            });
        }

        public void ShowTurnAnimation()
        {
            body.sharedMaterial = bodyOutlineMaterial;

            this.StartCoroutine(ShowTurnAnimationRoutine());
        }

        public void StopTurnAnimation()
        {
            this.StopAllCoroutines();

            pieceAnimator.Play(h_HideOutline);
            pieceAnimator.Play(h_Sleep, indexEyeMouthLayer);
            pieceAnimator.CrossFade(h_Idle, 0.35f, indexBaseLayer);
        }

        private IEnumerator ShowTurnAnimationRoutine()
        {
            pieceAnimator.Play(h_ShowOutline);
            pieceAnimator.Play(h_WakeUp, indexEyeMouthLayer);

            int countOfJumps = 3;
            yield return this.StartCoroutine(JumpRoutine(countOfJumps));
            yield return this.StartCoroutine(BlinkingRoutine());
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
            float t = 0;
            float nextBlink = 2.0f;
            while (true)
            {
                t += Time.deltaTime;
                if (t > nextBlink)
                {
                    this.Blink();
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

}

