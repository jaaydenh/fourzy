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

        private float inPauseTime;
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

        private void OnApplicationPause(bool pause)
        {
            if (!CheckPausedState("timer"))
            {
                if (pause)
                {
                    inPauseTime = Time.realtimeSinceStartup;
                }
                else
                {
                    inPauseTime = Time.realtimeSinceStartup - inPauseTime;
                }
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

            gameTimer = SkillzGameController.Instance.GameInitialTimerValue;
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
            CancelRoutine("timer");
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

        private IEnumerator GameTimerRoutine()
        {
            while (Timer > 0f)
            {
                if (inPauseTime > 0f)
                {
                    Timer -= inPauseTime;
                    inPauseTime = 0f;
                }

                Timer -= Time.deltaTime;

                yield return null;
            }

            Timer = 0f;
            game.IsOver = true;

            GamePlayManager.Instance.board.Pause();
            GamePlayManager.Instance.OnGameFinished(game);
        }
    }
}
