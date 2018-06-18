using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

namespace Fourzy
{
    public class AlertUI : MonoBehaviour
    {
        public Text alertText;
        public float fadeOutSpeed = 1f;
        public float timeBeforeFade = 1.5f;
        private float originalY;

        void Start() {
            originalY = transform.position.y;
        }

        // TODO: spawn new alert UI each time open is called
        public void Open(string text)
        {
            Debug.Log("Alert UI Open");
            alertText.text = text;
            alertText.CrossFadeAlpha(1f,0f, true);
            gameObject.SetActive(true);

            StartCoroutine(FadeOut());
        }

        public void OpenStatic(string text) {
            alertText.text = text;
            alertText.CrossFadeAlpha(1f,0f, true);
            gameObject.SetActive(true);
            StartCoroutine(OpenAndClose());
        }

        private IEnumerator OpenAndClose() {
            yield return new WaitForSeconds(timeBeforeFade);
            Reset();
        }

        private IEnumerator FadeOut()    {
            transform.DOMoveY(transform.position.y + 50, 1.5f).OnComplete(() => Reset());
            yield return new WaitForSeconds(timeBeforeFade);
            alertText.CrossFadeAlpha(0.0f, fadeOutSpeed, false);
            //alertText.DOFade(0f,fadeOutSpeed).OnComplete(() => transform.DOMoveY(originalY, 0f));
        }

        private void Reset() {
            transform.DOMoveY(originalY, 0f).OnComplete(() => Close());
            //Close();
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}