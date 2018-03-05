﻿using UnityEngine;
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
        private float originalY;

        void Start() {
            originalY = transform.position.y;
        }

        // TODO: spawn new alert UI each time open is called
        public void Open(string text)
        {
            alertText.text = text;
            alertText.CrossFadeAlpha(1f,0f, true);
            gameObject.SetActive(true);

            StartCoroutine(FadeOut());
        }

        private IEnumerator FadeOut()    {
            transform.DOMoveY(transform.position.y+100,1.4f).OnComplete(() => Reset());
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