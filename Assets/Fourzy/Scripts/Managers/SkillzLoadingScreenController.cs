//@vadym udod

using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.UI.Helpers;
using Hellmade.Net;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Fourzy._Updates.Managers
{
    public class SkillzLoadingScreenController : MonoBehaviour
    {
        [SerializeField]
        private Slider slider;
        [SerializeField]
        private GameplayBG bg;
        [SerializeField]
        private OnRatio landscape;
        [SerializeField]
        private OnRatio portrait;

        private IEnumerator Start()
        {
            if (GameManager.Instance.Landscape)
            {
                landscape.CheckOrientation();
            }
            else
            {
                portrait.CheckOrientation();
            }

            bg.Configure();

            switch (GameManager.Instance.buildIntent)
            {
                case BuildIntent.MOBILE_SKILLZ:
                    while (EazyNetChecker.Status != NetStatus.Connected)
                    {
                        yield return null;
                    }

                    break;
            }

            AsyncOperation async = SceneManager.LoadSceneAsync(GameManager.Instance.MainMenuSceneName, LoadSceneMode.Single);

            if (async != null)
            {
                async.allowSceneActivation = true;

                while (!async.isDone)
                {
                    slider.value = async.progress;
                    yield return null;
                }
            }
        }
    }
}
