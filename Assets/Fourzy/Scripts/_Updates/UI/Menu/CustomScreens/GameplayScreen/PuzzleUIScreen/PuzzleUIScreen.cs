//@vadym udod

using Fourzy._Updates._Tutorial;
using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.Tools;
using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using System.Collections.Generic;
using TMPro;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class PuzzleUIScreen : MenuScreen
    {
        public MovesLeftWidget movesLeftWidget;
        public ButtonExtended nextButton;
        public HintButton hintButton;

        public TMP_Text rule;
        public TMP_Text level;
        public TweenBase completeIcon;
        public AlphaTween packInfoTween;

        private int prevHintsCount = -1;

        public IClientFourzy game { get; private set; }

        public void Open(IClientFourzy game)
        {
            if (game == null || !game.puzzleData)
            {
                if (isOpened) Close();

                return;
            }

            base.Open();

            this.game = game;

            //!hint button controls, should be uncommented when hints are brought back
            if (game.puzzleData.Solution.Count > 0)
            {
                hintButton.Show();
                SetHintButtonState(true);
            }
            else
            {
                hintButton.Hide();
            }

            if (game.puzzleData.pack)
            {
                if (game.puzzleData.pack.complete)
                {
                    nextButton.Show(0f);
                    nextButton.SetState(true);
                }

                switch (game.puzzleData.pack.packType)
                {
                    case PackType.BOSS_AI_PACK:
                    case PackType.AI_PACK:
                        packInfoTween.SetAlpha(0f);

                        break;

                    case PackType.PUZZLE_PACK:
                        packInfoTween.SetAlpha(1f);

                        movesLeftWidget.SetData(game.asFourzyPuzzle);

                        if (PlayerPrefsWrapper.GetPuzzleChallengeComplete(game.puzzleData.ID))
                        {
                            completeIcon.PlayForward(true);
                        }
                        else
                        {
                            completeIcon.AtProgress(0f);
                        }

                        rule.text = game.puzzleData.Instructions;
                        //level.text = "Level " + (game.puzzleData.pack.enabledPuzzlesData.IndexOf(game.puzzleData) + 1);

                        break;
                }
            }
            //for quick puzzles
            else
            {
                if (nextButton.alphaTween._value < 1f)
                {
                    nextButton.Show(.3f);
                    nextButton.SetState(true);
                }

                packInfoTween.SetAlpha(1f);

                if (PlayerPrefsWrapper.GetFastPuzzleComplete(game.puzzleData.ID))
                {
                    completeIcon.PlayForward(true);
                }
                else
                {
                    completeIcon.AtProgress(0f);
                }

                rule.text = game.puzzleData.Instructions;

                movesLeftWidget.SetData(game.asFourzyPuzzle);
            }
        }

        public void OnMoveStarted()
        {
            if (game == null || !game.puzzleData) return;

            SetHintButtonState(false);
        }

        public void OnGameStarted()
        {
            if (game == null || !game.puzzleData) return;

            if (game.puzzleData.Solution.Count == 0) return;

            int progress = PlayerPrefsWrapper.GetHintTutorialStage();

            if (game.isMyTurn)
            {
                switch (progress)
                {
                    case 0:
                        if (game.LoseStreak == 2)
                        {
                            UserManager.Instance.hints++;
                            PersistantMenuController.Instance
                                .GetOrAddScreen<OnboardingScreen>()
                                .OpenTutorial(HardcodedTutorials.GetByName("HintInstruction"));
                            PlayerPrefsWrapper.SetHintTutorialStage(progress + 1);

                            hintButton.SetOutline(1f);
                        }

                        break;

                    case 1:
                        if (game.LoseStreak == 6 && UserManager.Instance.hints > 0)
                        {
                            //PlayerPrefsWrapper.SetHintTutorialStage(progress + 1);

                            hintButton.SetMessage(LocalizationManager.Value("suggest_hint_use"));
                            hintButton.SetOutline(1f);
                        }

                        break;
                }
            }
        }

        public void UpdatePlayerTurn()
        {
            if (game == null || game.puzzleData == null) return;

            if (game._Type == GameType.PUZZLE)
            {
                movesLeftWidget.UpdateMovesLeft();
            }

            SetHintButtonState(true);
        }

        public void Next()
        {
            AnalyticsManager.Instance.LogGame(
                game.GameToAnalyticsEvent(false),
                game,
                values: new KeyValuePair<string, object>(
                    AnalyticsManager.GAME_RESULT_KEY,
                    AnalyticsManager.GameResultType.skip));

            GamePlayManager.Instance.LoadGame(game.Next());
        }

        public void GameComplete()
        {
            if (game.IsWinner()) completeIcon.PlayForward(true);

            if (movesLeftWidget.alphaTween._value > 0f) movesLeftWidget.Hide(.3f);

            if (nextButton.interactable)
            {
                nextButton.SetState(false);
                nextButton.Hide(.3f);
            }
        }

        public void TryUseHint()
        {
            GamePlayManager.Instance.PlayHint();

            if (GamePlayManager.Instance.IsRoutineActive("hintRoutine"))
            {
                SetHintButtonState(false);
            }

            //if check to prevent event spamming
            if (prevHintsCount != UserManager.Instance.hints)
            {
                AnalyticsManager.Instance.LogGame(
                    AnalyticsManager.AnalyticsEvents.hintButtonPressed,
                    game,
                    values: new KeyValuePair<string, object>(
                        AnalyticsManager.HINT_STORE_ITEMS_KEY,
                        StorePromptScreen.ProductsToString(StorePromptScreen.StoreItemType.HINTS)));
            }

            prevHintsCount = UserManager.Instance.hints;
        }

        private void SetHintButtonState(bool state) => hintButton.SetState(state && !game.isOver && game.isMyTurn);
    }
}
