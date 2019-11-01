//modded

using Fourzy._Updates._Tutorial;
using Fourzy._Updates.Audio;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Menu;
using Fourzy._Updates.UI.Menu.Screens;
using Sirenix.Utilities;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Fourzy
{
    public class LoadingScreenControl : MonoBehaviour
    {
        public const float WAIT_DURATION = 1.7f;

        public Slider slider;
        public GameObject bg;

        protected void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        protected void Start()
        {
            StartCoroutine(LoadingRoutine());

            bg.GetComponents<OnRatio>().ForEach(o => o.CheckOrientation());
            bg.GetComponent<GameplayBG>().Configure();
        }

        protected void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            SceneManager.UnloadSceneAsync(Constants.LOGO_SCENE_NAME);
        }

        private IEnumerator LoadingRoutine()
        {
            AudioHolder.instance.PlaySelfSfxOneShotTracked(_Updates.Serialized.AudioTypes.GAME_LOGO, 1f);

            OnboardingScreen onboardingScreen = PersistantMenuController.instance.GetScreen<OnboardingScreen>();

            AsyncOperation async = null;
            bool displayTutorial = onboardingScreen.WillDisplayTutorial(HardcodedTutorials.tutorials[0]);

            if (!displayTutorial)
            {
                async = SceneManager.LoadSceneAsync(Constants.MAIN_MENU_SCENE_NAME);
                async.allowSceneActivation = false;
            }

            float duration = 0;
            while (duration < WAIT_DURATION)
            {
                duration += Time.deltaTime;
                slider.value = duration / WAIT_DURATION * 0.75f;

                yield return null;
            }

            slider.value = 1f;
            if (async != null)
            {
                async.allowSceneActivation = true;

                while (!async.isDone)
                {
                    slider.value = async.progress;
                    yield return null;
                }
            }

            if (displayTutorial) onboardingScreen.OpenTutorial(HardcodedTutorials.tutorials[0]);
        }
    }
}
