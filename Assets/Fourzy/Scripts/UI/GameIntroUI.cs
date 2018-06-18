using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Fourzy
{
    public class GameIntroUI : MonoBehaviour
    {
        public CanvasGroup canvasGroup;
        public Text titleText;
        public Text subTitleText;
        public float fadeInSpeed = 3f;
        public float fadeOutSpeed = 4f;
        public float timeBeforeFade = 3f;

        public void Open(string title, string subTitle, bool fade)
        {
            GamePlayManager.Instance.isLoadingUI = true;
            titleText.text = title;
            subTitleText.text = subTitle;
            gameObject.SetActive(true);
            if (fade) {
                canvasGroup.alpha = 0;
                StartCoroutine(FadeIn());
                StartCoroutine(FadeOut());
            } else {
                canvasGroup.alpha = 1;
            }
        }

        private IEnumerator FadeIn()    {
            while(canvasGroup.alpha < 1){
                canvasGroup.alpha += Time.deltaTime * fadeInSpeed;
                yield return null;
            }
        }

        private IEnumerator FadeOut()    {
            yield return new WaitForSeconds(timeBeforeFade);
            while(canvasGroup.alpha > 0){
                canvasGroup.alpha -= Time.deltaTime * fadeOutSpeed;
                yield return null;
            }

            Close();
            GamePlayManager.Instance.isLoadingUI = false;
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}