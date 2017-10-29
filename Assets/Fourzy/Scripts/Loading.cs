namespace Fourzy
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.SceneManagement;

    public class Loading : MonoBehaviour
    {

        public GameObject loadingScreenObj;
        public Slider slider;

        AsyncOperation async;

        void Start()
        {
            //StartCoroutine(LoadScene());
        }

        public void TransitionToGame()
        {
            StartCoroutine(LoadScene());
            //async.allowSceneActivation = true;
        }

        IEnumerator LoadScene()
        {
            while (!LocalizationManager.instance.GetIsReady())
            {
                yield return null;
            }

            SceneManager.LoadScene("game");

            //loadingScreenObj.SetActive(true);
            //async = SceneManager.LoadSceneAsync("game");
            //async.allowSceneActivation = false;

            //while (async.isDone == false)
            //{
            //    slider.value = async.progress;
            //    if (async.progress == 0.9f)
            //    {
            //        slider.value = 1f;
            //        //yield return new WaitForSeconds(3f);
            //        //async.allowSceneActivation = true;
            //    }
            //    yield return null;
            //}
        }
    }
}
