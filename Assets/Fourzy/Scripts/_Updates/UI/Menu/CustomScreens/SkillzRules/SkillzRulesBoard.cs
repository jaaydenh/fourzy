using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.Tools;
using FourzyGameModel.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class SkillzRulesBoard : RoutinesBase, ISKillzRulesPageComponent
    {
        public Action<int> onMoveEnded;

        [SerializeField]
        private List<string> boardIds;
        [SerializeField]
        private bool animated;

        private GameboardView gameboard;
        private ClientFourzyGame game;
        private int movesCount;

        public void OnPageOpened()
        {
            CancelRoutine("playBoard");
            StartRoutine("playBoard", PlayBoard(), gameboard.StopBoardUpdates);
        }

        public void OnPageClosed()
        {
            CancelRoutine("playBoard");
        }

        protected override void Awake()
        {
            base.Awake();

            gameboard = GetComponentInChildren<GameboardView>();
            gameboard.onMoveEnded += OnMoveEnded;
        }

        private void OnMoveEnded(ClientPlayerTurn turn, PlayerTurnResult turnResult, bool startTurn)
        {
            if (turn != null && turn.PlayerId == 1)
            {
                movesCount++;
            }

            onMoveEnded?.Invoke(movesCount);
        }

        private IEnumerator PlayBoard()
        {
            yield return new WaitForSeconds(.4f);

            if (animated)
            {
                while (true)
                {
                    foreach (string boardId in boardIds)
                    {
                        movesCount = 0;

                        LoadBoard(boardId);

                        gameboard.FadeGamepieces(1f, .3f);
                        gameboard.FadeTokens(1f, .3f);

                        //wait a bit for screen to fade in
                        yield return new WaitForSeconds(.4f);

                        //play before actions
                        yield return gameboard.OnPlayManagerReady();

                        //wait a bit again
                        yield return new WaitForSeconds(.3f);

                        //play initial moves
                        yield return gameboard.PlayInitialMoves();

                        yield return new WaitForSeconds(.7f);

                        gameboard.FadeGamepieces(0f, .3f);
                        gameboard.FadeTokens(0f, .3f);

                        yield return new WaitForSeconds(.45f);
                    }
                }
            }
            else
            {
                LoadBoard(boardIds[0]);
            }

            void LoadBoard(string boardId)
            {
                game = new ClientFourzyGame(
                    GameContentManager.Instance.GetMiscBoard(boardId),
                    UserManager.Instance.meAsPlayer,
                    new Player(2, "Player Two")
                    {
                        HerdId = InternalSettings.Current.DEFAULT_GAME_PIECE
                    })
                {
                    _Type = GameType.ONBOARDING,
                    _Mode = GameMode.NONE,
                };

                gameboard.Initialize(game, false);
            }
        }
    }
}
