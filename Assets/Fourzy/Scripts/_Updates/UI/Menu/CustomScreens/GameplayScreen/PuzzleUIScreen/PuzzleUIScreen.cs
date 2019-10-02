//@vadym udod

using System;
using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.Tween;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class PuzzleUIScreen : MenuScreen
    {
        public MovesLeftWidget movesLeftWidget;
        public ButtonExtended rematchButton;
        public ButtonExtended nextButton;
        public ButtonExtended hintButton;

        public TMP_Text rule;
        public TweenBase completeIcon;
        public AlphaTween packInfoTween;

        //private bool rematchButtonState = false;

        public IClientFourzy game { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            UserManager.onCurrencyUpdate += OnCurrencyUpdate;
        }

        protected void OnDestroy()
        {
            UserManager.onCurrencyUpdate -= OnCurrencyUpdate;
        }

        public void Open(IClientFourzy game)
        {
            if (game == null || !game.puzzleData)
            {
                if (isOpened) Close();

                return;
            }

            base.Open();

            this.game = game;

            OnCurrencyUpdate(CurrencyType.HINTS);

            if (game.puzzleData.Solution.Count > 0)
            {
                hintButton.SetActive(true);

                SetHintButtonState(true);
            }
            else
                hintButton.SetActive(false);

            if (game.puzzleData.pack)
            {
                if (game.isFourzyPuzzle)
                {
                    if (game.puzzleData.pack.enabledPuzzlesData.Count > 1)
                    {
                        if (nextButton.alphaTween._value < 1f)
                        {
                            nextButton.Show(.3f);
                            nextButton.SetState(true);
                        }
                    }
                    else
                    {
                        nextButton.Hide(0f);
                        nextButton.SetState(false);
                    }
                }
                else
                    nextButton.SetState(false);

                switch (game.puzzleData.pack.packType)
                {
                    case PackType.BOSS_AI_PACK:
                    case PackType.AI_PACK:
                        packInfoTween.SetAlpha(0f);

                        break;

                    case PackType.PUZZLE_PACK:
                        packInfoTween.SetAlpha(1f);

                        movesLeftWidget.SetData(game.asFourzyPuzzle);

                        if (PlayerPrefsWrapper.GetPuzzleChallengeComplete(game.GameID))
                            completeIcon.PlayForward(true);
                        else
                            completeIcon.AtProgress(0f);

                        rule.text = (game.puzzleData.pack.enabledPuzzlesData.IndexOf(game.puzzleData) + 1) + ": " + game.puzzleData.Instructions;

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

                if (PlayerPrefsWrapper.GetFastPuzzleComplete(game.GameID))
                    completeIcon.PlayForward(true);
                else
                    completeIcon.AtProgress(0f);

                rule.text = game.puzzleData.Instructions;

                movesLeftWidget.SetData(game.asFourzyPuzzle);
            }

            //if (rematchButtonState)
            if (rematchButton.interactable)
            {
                //rematchButton.scaleTween.PlayBackward(true);
                rematchButton.alphaTween.PlayBackward(true);
                rematchButton.SetState(false);

                //rematchButton.interactable = false;
                //rematchButtonState = false;
            }
        }

        public void OnMoveStarted()
        {
            if (game == null || !game.puzzleData) return;

            SetHintButtonState(false);
            //hintButton.SetState(false);
        }

        public void UpdatePlayerTurn()
        {
            if (game == null || game.puzzleData == null) return;

            if (game._Type == GameType.PUZZLE) movesLeftWidget.UpdateMovesLeft();

            //if (game.isMyTurn && !game.isOver) hintButton.SetState(true);
            SetHintButtonState(true);

            if (game._allTurnRecord.Count == 1 && !game.isOver && game.isFourzyPuzzle)
            {
                rematchButton.SetState(true);
                rematchButton.alphaTween.PlayForward(true);

                //rematchButton.interactable = true;
                //rematchButtonState = true;
            }
        }

        public void Next() => GamePlayManager.instance.LoadGame(game.Next());

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

        public void TryUseHint() => GamePlayManager.instance.PlayHint();

        private void SetHintButtonState(bool state) => hintButton.SetState(state && !game.isOver && game.isMyTurn);

        private void OnCurrencyUpdate(CurrencyType type)
        {
            if (type != CurrencyType.HINTS) return;

            if (game == null || game.puzzleData == null) return;

            hintButton.GetBadge().badge.SetValue(UserManager.Instance.hints);
        }
    }
}
