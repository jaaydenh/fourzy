using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GameAnalyticsSDK;

public class LoadingScreenControl : MonoBehaviour {

    public GameObject loadingScreenObj;
    public Slider slider;

    AsyncOperation async;

    void Start() {
        //StartCoroutine(Loading());
        Debug.Log("GameAnalytics.Initialize();");
        GameAnalytics.Initialize();
    }

    IEnumerator Loading() {
        loadingScreenObj.SetActive(true);
        async = SceneManager.LoadSceneAsync("tabbedUI");
        async.allowSceneActivation = false;
        yield return new WaitForSeconds(1f);
        while (async.isDone == false) {
            slider.value = async.progress;
            if (async.progress == 0.9f) {
                slider.value = 1f;
                yield return new WaitForSeconds(2f);
                async.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
