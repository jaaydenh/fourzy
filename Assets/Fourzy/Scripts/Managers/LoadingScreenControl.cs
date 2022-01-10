//modded

using Fourzy._Updates._Tutorial;
using Fourzy._Updates.Audio;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Menu;
using Fourzy._Updates.UI.Menu.Screens;
using Hellmade.Net;
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
        [SerializeField]
        private Slider slider;
        [SerializeField]
        private GameplayBG bg;
        [SerializeField]
        private OnRatio landscape;
        [SerializeField]
        private OnRatio portrait;

        private PlayfabValuesLoaded[] valuesToWaitFor;

        protected void Start()
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
                case BuildIntent.MOBILE_REGULAR:
                case BuildIntent.DESKTOP_REGULAR:
                    StartCoroutine(MobileRegularLoadRoutine());

                    break;

                case BuildIntent.MOBILE_SKILLZ:
                    StartCoroutine(SkillzLoadRoutine());

                    break;

                case BuildIntent.MOBILE_INFINITY:
                    StartCoroutine(InfinityLoadRoutine());

                    break;
            }
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

        private IEnumerator MobileRegularLoadRoutine()
        {
            valuesToWaitFor = ((PlayfabValuesLoaded[])Enum.GetValues(typeof(PlayfabValuesLoaded))).Where(_value => _value != PlayfabValuesLoaded.NONE).ToArray();
            UserManager.onPlayfabValuesLoaded += OnPlayfabValueLoaded;

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
                onboardingScreen.OpenTutorial(HardcodedTutorials.GetByName("Onboarding"));
            }
        }

        private IEnumerator SkillzLoadRoutine()
        {
            while (EazyNetChecker.Status != NetStatus.Connected)
            {
                yield return null;
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

        private IEnumerator InfinityLoadRoutine()
        {
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
