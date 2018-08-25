using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Anima2D;
using DG.Tweening;

namespace Fourzy
{
    public class GamePieceView : MonoBehaviour
    {
        [SerializeField]
        private SpriteMeshInstance body;

        [SerializeField]
        private Animator pieceAnimator;

        [SerializeField]
        public AnimationCurve movementCurve;

        private Transform cachedTransform;


        private void Awake()
        {
            Debug.Assert(pieceAnimator != null, "Setup pieceAnimator for GamePieceView in editor");
            Debug.Assert(body != null, "Setup body for GamePieceView in editor");

            if (pieceAnimator == null)
            {
                pieceAnimator = this.GetComponent<Animator>();
            }
            if (body == null)
            {
                body = this.transform.Find("Sprites").Find("Body").GetComponent<SpriteMeshInstance>();
            }

            cachedTransform = transform;
            body.sharedMaterial = new Material(body.sharedMaterial);
        }

        public void SetupHSVColor(Vector4 vec)
        {
            body.sharedMaterial.SetVector("_HSVAAdjust", vec);
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
    }

}

