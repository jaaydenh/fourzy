using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Fourzy
{
    public class RewardScreen : MonoBehaviour
    {
        const float timeForDisplayPortalFirstTime = 3.0f;

        [SerializeField] Image portal;
        [SerializeField] Image spinner;

        [SerializeField] List<Sprite> portalSpinners = new List<Sprite>();

        [SerializeField] GameObject rewardGO;
        [SerializeField] Text coinsReward;
        [SerializeField] GamePieceUI gamePieceReward;
        [SerializeField] AnimationCurve rewardAppearCurve;

        public bool IsOpen 
        {
            get
            {
                return this.gameObject.activeSelf;
            }
        }

        private bool canSkipPortal = false;
        private bool canShowPortal = false;
        private bool canDisplayNextReward = true;

        private void Awake()
        {
            this.ResetScreen();
        }

        private void ResetScreen()
        {
            Image backgroundImage = this.GetComponent<Image>();
            backgroundImage.SetAlpha(0.0f);

            rewardGO.SetActive(false);
            canShowPortal = false;
            canSkipPortal = false;
            canDisplayNextReward = true;
            portal.SetAlpha(0.0f);
            spinner.SetAlpha(0.0f);
            portal.transform.localPosition = Vector3.zero;
        }

        public void Open(List<Reward> rewards)
        {
            this.gameObject.SetActive(true);

            this.StartCoroutine(OpenRoutine(rewards));
        }

        private void Close()
        {
            this.gameObject.SetActive(false);
        }

        private IEnumerator OpenRoutine(List<Reward> rewards)
        {
            this.ResetScreen();

            yield return new WaitUntil(() => canShowPortal);

            Coroutine coroutine = this.StartCoroutine(DisplayPortalRoutine(true));

            yield return new WaitUntil(() => canSkipPortal);

            if (coroutine != null)
            {
                this.StopCoroutine(coroutine);
            }

            Image backgroundImage = this.GetComponent<Image>();
            yield return backgroundImage.DOFade(1.0f, 0.5f).WaitForCompletion();

            float screenHeight = this.GetComponent<RectTransform>().rect.height;
            portal.rectTransform.localPosition = new Vector3(0, screenHeight * 0.25f);
            yield return this.StartCoroutine(DisplayPortalRoutine(false));

            rewardGO.SetActive(true);

            // For test purpose generation of rewards
            rewards = this.GenerateTestRewards();

            while(true)
            {
                if (canDisplayNextReward)
                {
                    if (rewards.Count == 0)
                    {
                        this.Close();
                        break;
                    }

                    Reward reward = rewards[0];
                    rewards.RemoveAt(0);
                    canDisplayNextReward = false;

                    this.StartCoroutine(DisplayRewardRoutine(reward));
                }
                yield return null;
            }
        }

        private IEnumerator DisplayPortalRoutine(bool hideAfterShow)
        {
            portal.SetAlpha(0.0f);
            spinner.SetAlpha(0.0f);

            yield return portal.DOFade(1.0f, 0.3f).WaitForCompletion();
            yield return spinner.DOFade(1.0f, 1.0f).WaitForCompletion();

            Coroutine rotateSpinnerRoutine = this.StartCoroutine(RotateSpinnerRoutine());

            if (hideAfterShow)
            {
                yield return new WaitForSeconds(timeForDisplayPortalFirstTime);

                canSkipPortal = true;

                portal.SetAlpha(0.0f);
                spinner.SetAlpha(0.0f);
                this.StopCoroutine(rotateSpinnerRoutine);
            }
        }

        private IEnumerator RotateSpinnerRoutine()
        {
            Transform spinnerTransform = spinner.transform;
            float speed = 50.0f;

            while (true)
            {
                spinnerTransform.Rotate(Vector3.back, speed * Time.deltaTime);
                yield return null;
            }
        }

        private IEnumerator DisplayRewardRoutine(Reward reward)
        {
            if (reward.Type == Reward.RewardType.Coins)
            {
                gamePieceReward.gameObject.SetActive(false);
                coinsReward.transform.parent.gameObject.SetActive(true);
                coinsReward.text = "+" + reward.NumberOfCoins + " Coins";
            }
            else if (reward.Type == Reward.RewardType.CollectedGamePiece)
            {
                coinsReward.transform.parent.gameObject.SetActive(false);
                gamePieceReward.gameObject.SetActive(true);
                gamePieceReward.InitWithRewardData(reward.CollectedGamePiece);
            }

            Transform rewardTransform = rewardGO.transform;
            CanvasGroup canvasGroup = rewardGO.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0.0f;
            rewardTransform.localPosition = portal.transform.localPosition;

            float portalAnimationTime = 1.5f;
            Transform spinnerTransform = spinner.transform;
            float speed = 150.0f;
            float acceleration = 500.0f;

            for (float t = 0; t < portalAnimationTime; t += Time.deltaTime)
            {
                speed += Time.deltaTime * acceleration;
                spinnerTransform.Rotate(Vector3.back, speed * Time.deltaTime);
                yield return null;
            }

            const float appearTime = 0.5f;

            canvasGroup.DOFade(1.0f, appearTime);
            yield return rewardTransform.DOLocalMove(Vector3.zero, appearTime).SetEase(rewardAppearCurve).WaitForCompletion();
        }

        public void ScreenOnClick()
        {
            if (canSkipPortal)
            {
                canDisplayNextReward = true;
            }
            else if (canShowPortal)
            {
                canSkipPortal = true;
            }
            else
            {
                canShowPortal = true;
            }
        }

        private List<Reward> GenerateTestRewards()
        {
            List<Reward> rewards = new List<Reward>();
            rewards.Add(new Reward()
            {
                Type = Reward.RewardType.Coins,
                NumberOfCoins = 10
            });
            rewards.Add(new Reward()
            {
                Type = Reward.RewardType.CollectedGamePiece,
                CollectedGamePiece = new GamePieceData()
            });
            rewards.Add(new Reward()
            {
                Type = Reward.RewardType.CollectedGamePiece,
                CollectedGamePiece = new GamePieceData()
            });
            return rewards;
        }
    }
}
