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
        private SpriteRenderer eye;

        [SerializeField]
        private SpriteRenderer mouth;

        [SerializeField]
        private Animator pieceAnimator;

        [SerializeField]
        private Shader outlineShader;

        [SerializeField]
        private Sprite openEye;

        [SerializeField]
        private Sprite closedEye;

        [SerializeField]
        private Sprite openMouth;

        [SerializeField]
        private Sprite closedMouth;

        private Transform cachedTransform;

        private Material bodyMaterial;
        private Material bodyOutlineMaterial;

        private void Awake()
        {
            Debug.Assert(pieceAnimator != null, "Setup pieceAnimator for GamePieceView in editor");
            Debug.Assert(body != null, "Setup body for GamePieceView in editor");
            Debug.Assert(eye != null, "Setup eye for GamePieceView in Editor");

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
            bodyOutlineMaterial.SetVector("_HSVAAdjust", bodyMaterial.GetVector("_HSVAAdjust"));
        }

        public void SetupHSVColor(Vector4 vec)
        {
            bodyMaterial.SetVector("_HSVAAdjust", vec);
            bodyOutlineMaterial.SetVector("_HSVAAdjust", vec);
        }

        public void PlayMovement()
        {
            pieceAnimator.SetBool("isMoving", true);
        }

        public void PutMovementDirection(int x, int y)
        {
            pieceAnimator.SetFloat("directionX", x);
            pieceAnimator.SetFloat("directionY", y);
        }

        public void PlayFinishMovement(bool animateHit)
        {
            // 2 samples, 0.58 scale
            // hit animation = 20 samples
            pieceAnimator.SetBool("animateHit", animateHit);
            pieceAnimator.SetBool("isMoving", false);
        }


        public void SetupAsleep()
        {
            this.StartCoroutine(Blinking());
        }

        private IEnumerator Blinking()
        {
            float t = 0;
            float nextBlink = 5.0f;
            while(true)
            {
                t += Time.deltaTime;
                if (t > nextBlink)
                {
                    pieceAnimator.SetTrigger("blink");
                    nextBlink = Random.Range(5.0f, 15.0f);
                    t = 0;
                }
                yield return null;
            }
        }

        public void FadeAfterPit()
        {
            cachedTransform.DOScale(Vector3.zero, 1.0f);

            //this.gameObject.SetActive(false);
            // GamePiece animation
        }

        public void SetupZOrder(int zorder)
        {
            body.sortingOrder = zorder;
            eye.sortingOrder = zorder + 1;
            mouth.sortingOrder = zorder + 1;
        }

        public void PlayWinAnimation(Color color, float delay)
        {
            bodyOutlineMaterial.SetColor("_OutlineColor", color);
            body.sharedMaterial = bodyOutlineMaterial;

            pieceAnimator.Play("ShowWinningOutline");

            this.StartCoroutine(PlayWinAnimationWithDelay(delay));
        }

        IEnumerator PlayWinAnimationWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            pieceAnimator.SetBool("win", true);
        }

        public void ShowWinOutline(Color color)
        {
            bodyOutlineMaterial.SetColor("_OutlineColor", color);
            body.sharedMaterial = bodyOutlineMaterial;

            pieceAnimator.Play("ShowOutline");
            pieceAnimator.SetBool("win", true);
        }

        public void ShowTurnAnimation(Color color)
        {
            bodyOutlineMaterial.SetColor("_OutlineColor", color);
            body.sharedMaterial = bodyOutlineMaterial;

            this.SetupAsleep();

            pieceAnimator.Play("ShowOutline");
        }

        public void StopTurnAnimation()
        {            
            this.StopAllCoroutines();

            pieceAnimator.Play("HideOutline");
            //this.StartCoroutine(HideOutlineAnimation());
        }

        public void SetupClosedEye()
        {
            eye.sprite = closedEye;
        }

        public void SetupOpenEye()
        {
            eye.sprite = openEye;
        }

        public void SetupClosedMouth()
        {
            mouth.sprite = closedMouth;
        }

        public void SetupOpenMouth()
        {
            mouth.sprite = openMouth;
        }

    }

}

