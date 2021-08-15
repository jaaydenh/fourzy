//modded

using Fourzy._Updates._Tutorial;
using Fourzy._Updates.Audio;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Menu;
using Fourzy._Updates.UI.Menu.Screens;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Fourzy
{
    public class LoadingScreenControl : MonoBehaviour
    {
        public Slider slider;
        public GameplayBG bg;
        public OnRatio landscape;
        public OnRatio portrait;

        private PlayfabValuesLoaded[] valuesToWaitFor;

        protected void Start()
        {
            valuesToWaitFor = ((PlayfabValuesLoaded[])Enum.GetValues(typeof(PlayfabValuesLoaded))).Where(_value => _value != PlayfabValuesLoaded.NONE).ToArray();

            UserManager.onPlayfabValuesLoaded += OnPlayfabValueLoaded;

            if (GameManager.Instance.Landscape)
            {
                landscape.CheckOrientation();
            }
            else
            {
                portrait.CheckOrientation();
            }

            bg.Configure();

            StartCoroutine(LoadingRoutine());
        }

        protected void OnDestroy()
        {
            UserManager.onPlayfabValuesLoaded -= OnPlayfabValueLoaded;
        }

        private void OnPlayfabValueLoaded(PlayfabValuesLoaded value)
        {
            int values = 0;

            foreach (PlayfabValuesLoaded _value in valuesToWaitFor)
            {
                if (UserManager.Instance.IsPlayfabValueLoaded(_value))
                {
                    values++;
                }
            }

            slider.value = (float)values / valuesToWaitFor.Length;
        }

        private IEnumerator LoadingRoutine()
        {
            AudioHolder.instance.PlaySelfSfxOneShotTracked("game_greeting", 1f);

            OnboardingScreen onboardingScreen = PersistantMenuController.Instance.GetOrAddScreen<OnboardingScreen>();

            AsyncOperation async = null;
            bool displayTutorial = onboardingScreen.WillDisplayTutorial(HardcodedTutorials.GetByName("Onboarding"))/* && GameManager.Instance.Landscape*/;

            if (!displayTutorial)
            {
                async = SceneManager.LoadSceneAsync(GameManager.Instance.MainMenuSceneName, LoadSceneMode.Single);
                async.allowSceneActivation = false;
            }

            while (slider.value != 1f)
            {
                yield return null;
            }

            if (async != null)
            {
                async.allowSceneActivation = true;

                while (!async.isDone)
                {
                    slider.value = async.progress;
                    yield return null;
                }
            }

            if (displayTutorial)
            {
                onboardingScreen.OpenTutorial(
                    HardcodedTutorials.GetByName(GameManager.Instance.Landscape ? "OnboardingLandscape" : "Onboarding"));
            }
        }
    }
}
