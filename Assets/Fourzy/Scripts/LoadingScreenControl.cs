using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GameAnalyticsSDK;

namespace Fourzy
{
    public class LoadingScreenControl : MonoBehaviour
    {
        [SerializeField]
        private GameObject loadingScreenObj;

        [SerializeField]
        private Slider slider;

        [SerializeField]
        private Animator logoAnimator;

        private int h_Logo = Animator.StringToHash("Logo");
        private float animLength = 1.0f;

        private bool wasLoginProcessFinished;
        private bool logoAnimationWasShown;

        private void Start()
        {
            GameAnalytics.Initialize();

            this.StartCoroutine(LoadingRoutine());
        }

        private void OnEnable()
        {
            LoginManager.OnDeviceLoginComplete += LoginManager_OnDeviceLoginComplete;
            LoginManager.OnFBLoginComplete += LoginManager_OnFBLoginComplete;
        }

        private void OnDisable()
        {
            LoginManager.OnDeviceLoginComplete -= LoginManager_OnDeviceLoginComplete;
            LoginManager.OnFBLoginComplete -= LoginManager_OnFBLoginComplete;
        }

        void LoginManager_OnDeviceLoginComplete(bool isSuccessful)
        {
            wasLoginProcessFinished = true;
        }

        void LoginManager_OnFBLoginComplete(bool isSuccessful)
        {
            wasLoginProcessFinished = true;
        }

        private IEnumerator LoadingRoutine()
        {
            loadingScreenObj.SetActive(true);
            this.StartCoroutine(LogoAnimationRoutine());

            AsyncOperation async = SceneManager.LoadSceneAsync(Constants.MAIN_MENU_SCENE_NAME);
            async.allowSceneActivation = false;

            float duration = 0;

            while (!logoAnimationWasShown || !LocalizationManager.Instance.GetIsReady() || !wasLoginProcessFinished)
            {
                if (!logoAnimationWasShown)
                {
                    duration += Time.deltaTime;
                    slider.value = duration / animLength * 0.75f;
                }
                yield return null;
            }

            async.allowSceneActivation = true;

            while (!async.isDone)
            {
                slider.value = async.progress;
                yield return null;
            }

        }

        private IEnumerator LogoAnimationRoutine()
        {
            int baseLayerIndex = 0;

            logoAnimator.Play(h_Logo);

            while (logoAnimator.GetCurrentAnimatorStateInfo(baseLayerIndex).shortNameHash != h_Logo) yield return true;

            AnimatorStateInfo logoStateInfo = logoAnimator.GetCurrentAnimatorStateInfo(baseLayerIndex);
            animLength = logoStateInfo.length;

            yield return new WaitForSeconds(logoStateInfo.length);

            logoAnimationWasShown = true;
        }
    }
}
