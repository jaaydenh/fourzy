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

        private int h_Idle = Animator.StringToHash("Idle");
        private int h_Logo = Animator.StringToHash("Logo");

        private void Start()
        {
            GameAnalytics.Initialize();

            this.StartCoroutine(LoadingRoutine());
        }

        private IEnumerator LoadingRoutine()
        {
            AsyncOperation async = SceneManager.LoadSceneAsync("tabbedUI");
            async.allowSceneActivation = false;

            yield return this.StartCoroutine(LogoAnimationRoutine());

            while (!LocalizationManager.Instance.GetIsReady()) yield return null;

            async.allowSceneActivation = true;

            loadingScreenObj.SetActive(true);
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

            AnimatorStateInfo stateInfo = logoAnimator.GetCurrentAnimatorStateInfo(baseLayerIndex);

            yield return new WaitForSeconds(stateInfo.length);

            logoAnimator.Play(h_Idle, baseLayerIndex);
        }
    }
}
