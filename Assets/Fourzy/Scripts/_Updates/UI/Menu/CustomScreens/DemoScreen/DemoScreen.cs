//@vadym udod

using Fourzy._Updates.Managers;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tween;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class DemoScreen : MenuScreen
    {
        public CanvasGroup context;
        public PositionTween buttonPositionTween;

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

        public void AddHints() => UserManager.Instance.hints += 5;

        public void RemoveHints() => UserManager.Instance.hints -= 5;

        public void AddPortalPoints() => UserManager.Instance.portalPoints += 110;

        public void RemovePortalPoints() => UserManager.Instance.portalPoints -= 110;

        public void AddRarePortalPoints() => UserManager.Instance.rarePortalPoints += 9;

        public void RemoveRarePortalPoints() => UserManager.Instance.rarePortalPoints -= 9;

        public void AddGems() => UserManager.Instance.gems += 5;

        public void RemoveGems() => UserManager.Instance.gems -= 5;

        public void SetLanguageEng() => LocalizationManager.Instance.SetCurrentLanguage(SystemLanguage.English);

        public void SetLanguageSpanish() => LocalizationManager.Instance.SetCurrentLanguage(SystemLanguage.Spanish);

        public void SetLanguageThai() => LocalizationManager.Instance.SetCurrentLanguage(SystemLanguage.Thai);

        public void OpenGrid()
        {
            menuController.OpenScreen<PuzzleSelectionScreen>(true);
            Toggle();
        }

        public void UnlockModes()
        {
            PlayerPrefsWrapper.SetRewardRewarded(Constants.GAME_MODE_FAST_PUZZLES, true);
            PlayerPrefsWrapper.SetRewardRewarded(Constants.GAME_MODE_GAUNTLET_GAME, true);
        }

        public void CompleteProgress() => GameContentManager.Instance.progressionMaps[0].CompleteAll();

        public void SetPlacementStyle(int value) => GameManager.Instance.placementStyle = (GameManager.PlacementStyle)value;

        public void UnlockGamepieces()
        {
            GameContentManager.Instance.piecesDataHolder.UnlockAll();
        }

        public void ResetGamepieces()
        {
            GamePiecesDataHolder.ClearGamepiecesData();
            GameContentManager.Instance.piecesDataHolder.Initialize();
        }

        public void Toggle()
        {
            isContextShown = !isContextShown;

            context.alpha = isContextShown ? 1f : 0f;
            context.interactable = isContextShown;
            context.blocksRaycasts = isContextShown;

            if (!isContextShown && FourzyMainMenuController.instance) FourzyMainMenuController.instance.currentScreen.Open();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            SettingsManager.onDemoMode += OnDemo;

            GameManager.onSceneChanged += OnSceneChanged;

            if (SettingsManager.Instance.Get(SettingsManager.KEY_DEMO_MODE)) OnDemo(true);
        }

        private void OnDemo(bool state)
        {
            if (state) Open();
            else Close();
        }

        private void OnSceneChanged(string sceneName)
        {

        }
    }
}
