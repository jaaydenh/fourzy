//@vadym udod

using Fourzy._Updates.Managers;
using Fourzy._Updates.Tween;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using static Fourzy.GameManager;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class DemoScreen : MenuScreen
    {
        [SerializeField]
        private CanvasGroup context;
        [SerializeField]
        private PositionTween buttonPositionTween;
        [SerializeField]
        private TMP_Dropdown inputMethodDropdown;

        private bool isContextShown = false;

        public override void Open()
        {
            base.Open();

            buttonPositionTween.PlayForward(true);
        }

        public override void Close(bool animate = true)
        {
            base.Close(animate);

            buttonPositionTween.PlayBackward(false);
        }

        public void ResetPuzzles() => GameManager.Instance.ResetGames();

        public void ForceAIPresentation()
        {
            GameManager.Instance.StartPresentataionGame();
            Toggle();
        }

        public void AddHints() => UserManager.AddHints(5);

        public void RemoveHints() => UserManager.AddHints(-5);

        public void AddPortalPoints() => UserManager.Instance.portalPoints += 110;

        public void RemovePortalPoints() => UserManager.Instance.portalPoints -= 110;

        public void AddRarePortalPoints() => UserManager.Instance.rarePortalPoints += 9;

        public void RemoveRarePortalPoints() => UserManager.Instance.rarePortalPoints -= 9;

        public void AddGems() => UserManager.Instance.gems += 5;

        public void RemoveGems() => UserManager.Instance.gems -= 5;

        public void SetLanguageEng() => LocalizationManager.Instance.SetCurrentLanguage(SystemLanguage.English);

        public void SetLanguageSpanish() => LocalizationManager.Instance.SetCurrentLanguage(SystemLanguage.Spanish);

        public void SetLanguageThai() => LocalizationManager.Instance.SetCurrentLanguage(SystemLanguage.Thai);

        public void ToggleMagic() => SettingsManager.Toggle(SettingsManager.KEY_REALTIME_MAGIC);

        public void UnlockModes()
        {
            PlayerPrefsWrapper.SetRewardRewarded(Constants.GAME_MODE_FAST_PUZZLES, true);
            PlayerPrefsWrapper.SetRewardRewarded(Constants.GAME_MODE_GAUNTLET_GAME, true);
        }

        public void CompleteProgress() =>
            GameContentManager.Instance.progressionMaps[0].CompleteAll(0);

        public void SetPlacementStyle(int value) =>
            GameManager.Instance.placementStyle = (GameManager.PlacementStyle)value;

        public void OpenGeneratorTester()
        {
            Toggle();

            menuController.OpenScreen(menuController.GetOrAddScreen<GeneratorTesterPrompt>());
        }

        public void Toggle()
        {
            isContextShown = !isContextShown;

            context.alpha = isContextShown ? 1f : 0f;
            context.interactable = isContextShown;
            context.blocksRaycasts = isContextShown;

            if (!isContextShown &&
                FourzyMainMenuController.instance &&
                FourzyMainMenuController.instance == MenuController.activeMenu)
            {
                FourzyMainMenuController.instance.currentScreen.Open();
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            SettingsManager.onDemoMode += OnDemo;

            inputMethodDropdown.ClearOptions();
            inputMethodDropdown.AddOptions(((PlacementStyle[])Enum.GetValues(typeof(PlacementStyle))).Select(_style => _style.ToString()).ToList());
            inputMethodDropdown.SetValueWithoutNotify((int)GameManager.Instance.placementStyle);

            if (SettingsManager.Get(SettingsManager.KEY_DEMO_MODE))
            {
                OnDemo(true);
            }
        }

        private void OnDemo(bool state)
        {
            if (state)
            {
                Open();
            }
            else
            {
                Close();
            }
        }
    }
}
