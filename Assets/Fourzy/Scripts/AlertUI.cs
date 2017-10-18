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
        public float timeBeforeFade = 1f;

        public void Open(string text, float initY)
        {
            alertText.text = text;
            alertText.CrossFadeAlpha(1f,0f, true);
            gameObject.SetActive(true);

            StartCoroutine(FadeOut());
        }

        private IEnumerator FadeOut()    {
            float originalY = transform.position.y;
            transform.DOMoveY(transform.position.y+100,1.4f).OnComplete(() => Reset(originalY));
            yield return new WaitForSeconds(timeBeforeFade);
            alertText.CrossFadeAlpha(0.0f, fadeOutSpeed, false);
            //alertText.DOFade(0f,fadeOutSpeed).OnComplete(() => transform.DOMoveY(originalY, 0f));
        }

        private void Reset(float y) {
            transform.DOMoveY(y, 0f);
            Close();
        }

        public void Close()
        {
            
            gameObject.SetActive(false);
        }
    }
}