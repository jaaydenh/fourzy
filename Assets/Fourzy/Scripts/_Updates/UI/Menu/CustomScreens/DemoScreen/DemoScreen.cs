//@vadym udod

using System;
using Fourzy._Updates.Managers;
using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Helpers;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class DemoScreen : MenuScreen
    {
        public CanvasGroup context;
        public ButtonExtended placementStyleButton;
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

        public void ResetPuzzles()
        {
            GameContentManager.Instance.ResetPuzzlePacks();
            GameContentManager.Instance.tokensDataHolder.ResetTokenInstructions();
        }

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

        public void OpenGrid()
        {
            menuController.OpenScreen<PuzzleSelectionScreen>(true);
            Toggle();
        }

        public void TogglePiecesPlacementStyle()
        {
            if (GameManager.Instance.placementStyle == GameManager.PlacementStyle.DEFAULT)
                GameManager.Instance.placementStyle = GameManager.PlacementStyle.DEMO_STYLE;
            else
                GameManager.Instance.placementStyle = GameManager.PlacementStyle.DEFAULT;
        }

        public void Toggle()
        {
            isContextShown = !isContextShown;

            context.alpha = isContextShown ? 1f : 0f;
            context.interactable = isContextShown;
            context.blocksRaycasts = isContextShown;
        }

        private void SetPlacementButtonState(bool state)
        {
            if (state)
            {
                if (GameManager.Instance.activeGame != null)
                {
                    switch (GameManager.Instance.activeGame._Type)
                    {
                        case GameType.REALTIME:
                        case GameType.TURN_BASED:

                            break;

                        default:
                            placementStyleButton.SetActive(true);

                            break;
                    }
                }
            }
            else
                placementStyleButton.SetActive(false);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            SettingsManager.onDemoMode += OnDemo;

            if (SettingsManager.Instance.Get(SettingsManager.KEY_DEMO_MODE)) OnDemo(true);

            GameManager.onSceneChanged += OnSceneChanged;
            SetPlacementButtonState(false);
        }

        private void OnDemo(bool state)
        {
            if (state) Open();
            else Close();
        }

        private void OnSceneChanged(string sceneName)
        {
            SetPlacementButtonState(sceneName == Constants.GAMEPLAY_SCENE_NAME);
        }
    }
}
