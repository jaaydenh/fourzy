//@vadym udod

using System;
using UnityEngine;
using UnityEngine.UI;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class LoadingPromptScreen : PromptScreen
    {
        public GameObject circular;
        public Image progress;

        public LoadingPromptScreen SetType(LoadingPromptType type)
        {
            switch (type)
            {
                case LoadingPromptType.BASIC:
                    circular.SetActive(true);
                    progress.transform.parent.gameObject.SetActive(false);

                    break;

                case LoadingPromptType.PROGRESS:
                    circular.SetActive(false);
                    progress.transform.parent.gameObject.SetActive(true);

                    progress.fillAmount = 0f;

                    break;
            }

            return this;
        }

        public void _Prompt(LoadingPromptType type, string message, Action onDeny = null)
        {
            switch (type)
            {
                case LoadingPromptType.BASIC:
                    circular.SetActive(true);
                    progress.transform.parent.gameObject.SetActive(false);

                    break;

                case LoadingPromptType.PROGRESS:
                    circular.SetActive(false);
                    progress.transform.parent.gameObject.SetActive(true);

                    progress.fillAmount = 0f;

                    break;
            }

            Prompt(message, null, null, null, null, onDeny);
        }

        public void SetProgress(float value) => progress.fillAmount = value;

        public void UpdateInfo(string text) => promptTitle.text = text;

        public void DelayedClose(float delay) => StartRoutine("close", delay, () => CloseSelf());

        public enum LoadingPromptType
        {
            BASIC,
            PROGRESS,
        }
    }
}