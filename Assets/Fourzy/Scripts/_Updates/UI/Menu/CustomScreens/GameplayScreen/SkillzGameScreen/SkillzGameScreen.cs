//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Managers;
using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Mechanics.GameplayScene;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class SkillzGameScreen : MenuScreen
    {
        [SerializeField]
        private TMP_Text timerLabel;
        [SerializeField]
        private TMP_Text scoreLabel;
        [SerializeField]
        private TMP_Text movesLeftLabel;
        [SerializeField]
        private RectTransform gamepieceParent;

        private float gameTimer;
        private GamePieceView currentGamepiece;

        public IClientFourzy game { get; private set; }
        public float Timer
        {
            get => gameTimer;
            set
            {
                gameTimer = value;
                TimeSpan time = TimeSpan.FromSeconds(Mathf.CeilToInt(gameTimer));
                timerLabel.text = time.ToString("mm':'ss");
            }
        }

        public void Open(IClientFourzy game)
        {
            if (game == null)
            {
                if (isOpened)
                {
                    Close();
                }

                return;
            }

            if (game._Type != GameType.SKILLZ_ASYNC)
            {
                return;
            }

            base.Open();

            this.game = game;

            //only reset timer when it's first game
            if (SkillzGameController.Instance.GamesPlayed.Count == 0)
            {
                gameTimer = SkillzGameController.Instance.GameInitialTimerValue;
            }

            SetMovesLeft(game.myMembers.Count);
            SetPoints(SkillzGameController.Instance.Points);

            if (currentGamepiece)
            {
                Destroy(currentGamepiece.gameboard);
            }
            currentGamepiece = Instantiate(GameContentManager.Instance.piecesDataHolder.GetGamePieceData(game.me.HerdId).player1Prefab, gamepieceParent);
            currentGamepiece.StartBlinking();

            CancelRoutine("timer");
            StartRoutine("timer", GameTimerRoutine());
        }

        public void GameComplete()
        {
            SetPausedState("timer", true);
        }

        public void OnMoveStarted()
        {
            if (!isOpened || game._Type != GameType.SKILLZ_ASYNC) return;

            SetMovesLeft(game.myMembers.Count);

            if (!game.isMyTurn)
            {
                Pause();
            }
        }

        public void UpdatePlayerTurn()
        {
            if (game == null || game._Type != GameType.SKILLZ_ASYNC) return;

            if (game.isMyTurn)
            {
                Unpause();
            }
        }

        public void SetPoints(int points)
        {
            scoreLabel.text = $"{LocalizationManager.Value("points")}: {points}";
        }

        public void SetMovesLeft(int movesLeft)
        {
            movesLeftLabel.text = movesLeft + "";
        }

        public void Pause()
        {
            SetPausedState("timer", true);
        }

        public void Unpause()
        {
            SetPausedState("timer", false);
        }

        public void DeductTimer(float value)
        {
            Timer = Mathf.Max(0f, Timer - Mathf.Abs(value));
        }

        private IEnumerator GameTimerRoutine()
        {
            while (Timer > 0f)
            {
                Timer -= Time.deltaTime;

                yield return null;
            }

            Timer = 0f;
            game.IsOver = true;

            GamePlayManager.Instance.BoardView.Pause();
            GamePlayManager.Instance.OnGameFinished(game);
        }
    }
}
