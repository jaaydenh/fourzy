﻿//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.UI.Widgets;
using FourzyGameModel.Model;
using StackableDecorator;
using System.Collections;
using System.Collections.Generic;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class GameplayScreen : MenuScreen
    {
        public PlayerUIWidget playerWidget;
        public PlayerUIWidget opponentWidget;
        //public PlayerUIMessagesWidget playerMessagesWidget;
        public PlayerUIMessagesWidget opponentMessagesWidget;
        public GameInfoWidget gameInfoWidget;

        public List<TimerSliderWidget> timerWidgets;

        public PuzzleWinLoseScreen puzzleWinLoseScreen;
        public GameWinLoseScreen gameWinLoseScreen;

        [HelpBox("Pass And Play mode overlay", messageType = MessageType.None)]
        public MenuScreen tapToStartOverlay;

        private IClientFourzy game;
        private GamePlayManager gameplayManager;
        private int onePlayerTurnCounter;

        public PuzzleUIScreen puzzleUI { get; private set; }
        public TurnBaseScreen turnbaseUI { get; private set; }
        public PassAndPlayScreen passAndPlayUI { get; private set; }
        public RealtimeScreen realtimeScreen { get; private set; }
        public DemoGameScreen demoGameScreen { get; private set; }

        private SpellsListUIWidget spellsListWidget;

        private bool timersEnabled => GameManager.Instance.passAndPlayTimer && (game._Type == GameType.PASSANDPLAY || game._Type == GameType.REALTIME);

        protected override void Awake()
        {
            base.Awake();

            puzzleUI = GetComponentInChildren<PuzzleUIScreen>();
            turnbaseUI = GetComponentInChildren<TurnBaseScreen>();
            passAndPlayUI = GetComponentInChildren<PassAndPlayScreen>();
            realtimeScreen = GetComponentInChildren<RealtimeScreen>();
            demoGameScreen = GetComponentInChildren<DemoGameScreen>();

            spellsListWidget = GetComponentInChildren<SpellsListUIWidget>();

            timerWidgets.ForEach(widget => widget.onValueEmpty += OnTimerEmpty);
        }

        public override void OnBack()
        {
            base.OnBack();

            if (game == null)
            {
                GamePlayManager.instance.BackButtonOnClick();
            }
            else
            {
                switch (game._Type)
                {
                    case GameType.PUZZLE:
                    case GameType.TURN_BASED:
                    case GameType.PRESENTATION:
                        GamePlayManager.instance.BackButtonOnClick();

                        break;

                    default:
                        if (game.isOver)
                            GamePlayManager.instance.BackButtonOnClick();
                        else
                            menuController.GetScreen<PromptScreen>().Prompt("Leave Game?", "", "Yes", "No", () => GamePlayManager.instance.BackButtonOnClick());

                        break;
                }
            }
        }

        public void InitUI(GamePlayManager gameplayManager)
        {
            this.gameplayManager = gameplayManager;
            game = gameplayManager.game;

            playerWidget.SetGame(game);
            opponentWidget.SetGame(game);

            playerWidget.Initialize();
            opponentWidget.Initialize();

            playerWidget.SetPlayerName(game.me.DisplayName);
            opponentWidget.SetPlayerName(game.opponent.DisplayName);

            playerWidget.StopPlayerTurnAnimation();
            opponentWidget.StopPlayerTurnAnimation();

            playerWidget.SetPlayerIcon(game.me);
            opponentWidget.SetPlayerIcon(game.opponent);

            if (game.hideOpponent) opponentWidget.Hide(.1f);
            else opponentWidget.Show(.1f);

            //initialize timer
            if (timersEnabled)
            {
                timerWidgets[0].AssignPlayer(game.me);
                timerWidgets[1].AssignPlayer(game.opponent);
            }
            //disable timer widgets if not needed
            else
            {
                //close tap overlay of opened
                CancelRoutine("waitFotTap");
                timerWidgets.ForEach(widget => widget.Hide(0f));
            }

            puzzleUI.Open(game);
            turnbaseUI.Open(game);
            passAndPlayUI.Open(game);
            realtimeScreen.Open(game);
            demoGameScreen.Open(game);

            //close game win/lose screen
            if (gameWinLoseScreen.isCurrent) menuController.CloseCurrentScreen(true);
            //close puzzle win/lose screen
            if (puzzleWinLoseScreen.isCurrent) menuController.CloseCurrentScreen(true);

            #region Check AI move

            //switch (game._Type)
            //{
            //    case GameType.AI:
            //    case GameType.PRESENTATION:
            //    case GameType.PUZZLE:
            //        gameInfoWidget.Hide(.3f);

            //        break;
            //}

            #endregion

            spellsListWidget.Open(game, gameplayManager.board, game.me);
        }

        public void ShowOpponentMessage(string message, float duration)
        {
            opponentMessagesWidget.AddMessage(message, duration);
        }

        public void OnWrongTurn()
        {
            gameInfoWidget.NotYourTurn();
        }

        public void OnMoveStarted(ClientPlayerTurn turn)
        {
            puzzleUI.OnMoveStarted();

            if (turn == null || turn.PlayerId < 1) return;

            #region Checking AI turn

            switch (game._Type)
            {
                case GameType.AI:
                //case GameType.PRESENTATION:
                case GameType.PUZZLE:
                    if (game.isMyTurn) gameInfoWidget.Hide(.3f);

                    break;
            }

            #endregion

            #region Timers

            if (timersEnabled)
            {
                if (game._FirstState.ActivePlayerId == turn.PlayerId) onePlayerTurnCounter++;

                //deactivate timers, add timer value
                timerWidgets.ForEach(widget =>
                {
                    if (widget.player.PlayerId == turn.PlayerId)
                    {
                        widget.Deactivate();

                        if (!widget.isEmpty)
                        {
                            if (onePlayerTurnCounter % Constants.addTimerBarEveryXTurn == 0)
                                widget.AddTimerValue(Constants.barsToAdd, true, true);
                            else
                                widget.AddSmallTimerValue(Constants.circularTimerValue);
                        }
                    }
                });
            }

            #endregion
        }

        public void OnMoveEnded(ClientPlayerTurn turn)
        {
            if (game == null) return;

            puzzleUI.UpdatePlayerTurn();
            passAndPlayUI.UpdatePlayerTurn();

            if (game.isOver) return;

            UpdatePlayerTurnGraphics();

            if (turn == null || turn.PlayerId < 1) return;

            #region Checking AI turn

            switch (game._Type)
            {
                case GameType.AI:
                //case GameType.PRESENTATION:
                case GameType.PUZZLE:
                    //if waiting more than (time), show "thinking..."
                    //if (!game.isMyTurn) gameInfoWidget.SetText("Thinking...").ShowDelayed(time: .6f);
                    if (!game.isMyTurn) ShowOpponentMessage("Thinking...", 1f);

                    break;
            }

            #endregion

            #region Timers

            if (timersEnabled)
            {
                //activate timers
                ActivatePlayerTimer(game._State.ActivePlayerId);

                //reset timers
                timerWidgets.ForEach(widget => { if (widget.isEmpty) widget.AddTimerValue(Constants.aiTurnTimerResetValue, true, true); });
            }

            #endregion

            spellsListWidget.UpdateSpells(game._State.ActivePlayerId);
        }

        public void UpdatePlayerTurnGraphics()
        {
            if (game.isMyTurn)
            {
                playerWidget.ShowPlayerTurnAnimation();

                if (!game.hideOpponent) opponentWidget.StopPlayerTurnAnimation();
            }
            else
            {
                playerWidget.StopPlayerTurnAnimation();

                if (!game.hideOpponent) opponentWidget.ShowPlayerTurnAnimation();
            }
        }

        public void OnGameStarted()
        {
            UpdatePlayerTurnGraphics();

            switch (game._Type)
            {
                case GameType.PASSANDPLAY:
                    if (GameManager.Instance.tapToStartGame)
                    {
                        menuController.OpenScreen(tapToStartOverlay);

                        //wait for tap screen to be closed
                        StartRoutine("waitFotTap", WaitForTapRoutine(), () => { if (timersEnabled) ActivatePlayerTimer(game._State.ActivePlayerId); }, () => tapToStartOverlay.CloseSelf());
                    }
                    else
                        if (timersEnabled) ActivatePlayerTimer(game._State.ActivePlayerId);

                    break;

                case GameType.REALTIME:
                    //start coutdown

                    if (timersEnabled) ActivatePlayerTimer(game._State.ActivePlayerId);

                    break;
            }
        }

        public void OnGameFinished()
        {
            if (!game.draw)
            {
                if (game.IsWinner())
                    playerWidget.StartWinJumps();
                else
                    opponentWidget.StartWinJumps();
            }

            if (game.puzzleData != null)
            {
                puzzleUI.GameComplete();

                //open puzzle win/lose screen
                puzzleWinLoseScreen.Open(game);
            }
            else
            {
                switch (game._Type)
                {
                    case GameType.PASSANDPLAY:
                        passAndPlayUI.GameComplete();

                        break;

                    case GameType.TURN_BASED:
                        turnbaseUI.GameComplete();

                        break;

                    case GameType.REALTIME:
                        realtimeScreen.GameComplete();

                        break;

                    case GameType.PRESENTATION:
                        demoGameScreen.GameComplete();

                        break;
                }

                //win/lose screen
                switch (game._Type)
                {
                    case GameType.ONBOARDING:
                        //open nothing

                        break;

                    default:
                        //open game win/lose screen
                        gameWinLoseScreen.Open(game);

                        break;
                }
            }

            #region Check AI move

            //switch (game._Type)
            //{
            //    case GameType.AI:
            //    case GameType.PRESENTATION:
            //    case GameType.PUZZLE:
            //        gameInfoWidget.Hide(.3f);

            //        break;
            //}

            #endregion
        }

        public void OnGamePaused()
        {
            //pause timers
            timerWidgets.ForEach(widget => widget.Pause());
        }

        public void OnGameUnpaused()
        {
            //unpause timers
            timerWidgets.ForEach(widget => widget.Unpause());
        }

        private void OnTimerEmpty(Player player)
        {
            switch (game._Type)
            {
                case GameType.PASSANDPLAY:
                    gameplayManager.board.TakeAITurn();

                    break;

                case GameType.REALTIME:
                    //only take turn if its my turn
                    if (game.isMyTurn) gameplayManager.board.TakeAITurn();

                    break;
            }
        }

        private void ActivatePlayerTimer(int playerID) => timerWidgets.ForEach(widget => { if (widget.player.PlayerId == playerID) { widget.Activate(); } });

        private IEnumerator WaitForTapRoutine()
        {
            while (!isCurrent) yield return null;
        }
    }
}
