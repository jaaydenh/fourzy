using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Fourzy
{
    public class GamePieceView : MonoBehaviour
    {
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

        private int h_isMoving = Animator.StringToHash("isMoving");
        private int h_directionX = Animator.StringToHash("directionX");
        private int h_directionY = Animator.StringToHash("directionY");
        private int h_animateHit = Animator.StringToHash("animteHit");
        private int h_win = Animator.StringToHash("win");
        private int h_blink = Animator.StringToHash("blink");

        private int h_Idle = Animator.StringToHash("Idle");
        private int h_ShowWinningOutline = Animator.StringToHash("ShowWinningOutline");
        private int h_HideOutline = Animator.StringToHash("HideOutline");
        private int h_ShowOutline = Animator.StringToHash("ShowOutline");
        private int h_Jumping = Animator.StringToHash("Jumping");
        private int h_Sleep = Animator.StringToHash("Sleep");
        private int h_WakeUp = Animator.StringToHash("WakeUp");

        private const int indexBaseLayer = 0;
        private const int indexEyeMouthLayer = 1;
        private const int indexMaterialLayer = 2;

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

            sprites = this.GetComponentsInChildren<SpriteRenderer>(true);
        }

        public void SetupHSVColor(Vector4 vec)
        {
            bodyMaterial.SetVector(h_HSVAAdjust, vec);
            bodyOutlineMaterial.SetVector(h_HSVAAdjust, vec);
        }

        public void PlayMovement()
        {
            pieceAnimator.SetBool(h_isMoving, true);
        }

        public void PutMovementDirection(int x, int y)
        {
            pieceAnimator.SetFloat(h_directionX, x);
            pieceAnimator.SetFloat(h_directionY, y);
        }

        public void PlayFinishMovement(bool animateHit)
        {
            pieceAnimator.SetBool(h_animateHit, animateHit);
            pieceAnimator.SetBool(h_isMoving, false);
        }

        public void FadeAfterPit()
        {
            cachedTransform.DOScale(Vector3.zero, 1.0f);

            //this.gameObject.SetActive(false);
            // GamePiece animation
        }

        public void SetupZOrder(int zorder)
        {
            foreach(SpriteRenderer sr in sprites)
            {
                sr.sortingOrder = zorder + 1;
            }

            body.sortingOrder = zorder;
        }

        public void PlayWinAnimation(Color color, float delay)
        {
            bodyOutlineMaterial.SetColor(h_OutlineColor, color);
            body.sharedMaterial = bodyOutlineMaterial;

            pieceAnimator.Play(h_ShowWinningOutline);

            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(delay);
            sequence.AppendCallback(() => {
                pieceAnimator.SetBool(h_win, true);
            });
        }

        public void ShowTurnAnimation(Color color)
        {
            bodyOutlineMaterial.SetColor(h_OutlineColor, color);
            body.sharedMaterial = bodyOutlineMaterial;

            this.StartCoroutine(ShowTurnAnimation());
        }

        public void StopTurnAnimation()
        {
            this.StopAllCoroutines();

            pieceAnimator.Play(h_HideOutline);
            pieceAnimator.Play(h_Sleep, indexEyeMouthLayer);
            pieceAnimator.CrossFade(h_Idle, 0.35f, indexBaseLayer);
        }

        private IEnumerator ShowTurnAnimation()
        {
            pieceAnimator.Play(h_ShowOutline);
            pieceAnimator.Play(h_WakeUp, indexEyeMouthLayer);

            int countOfJumps = 3;
            yield return this.StartCoroutine(Jump(countOfJumps));
            yield return this.StartCoroutine(Blinking());
        }

        private IEnumerator Jump(int count)
        {
            pieceAnimator.Play(h_Jumping);

            while (pieceAnimator.GetCurrentAnimatorStateInfo(indexBaseLayer).shortNameHash != h_Jumping) yield return true;

            AnimatorStateInfo stateInfo = pieceAnimator.GetCurrentAnimatorStateInfo(indexBaseLayer);

            yield return new WaitForSeconds(stateInfo.length * count);

            pieceAnimator.Play(h_Idle, indexBaseLayer);
        }

        private IEnumerator Blinking()
        {
            float t = 0;
            float nextBlink = 2.0f;
            while (true)
            {
                t += Time.deltaTime;
                if (t > nextBlink)
                {
                    pieceAnimator.SetTrigger(h_blink);
                    nextBlink = Random.Range(5.0f, 15.0f);
                    t = 0;
                }
                yield return null;
            }
        }
    }

}

