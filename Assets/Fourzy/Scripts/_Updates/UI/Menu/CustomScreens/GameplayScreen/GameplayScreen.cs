﻿//@vadym udod

using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.Tools;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using FourzyGameModel.Model;
using StackableDecorator;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fourzy._Updates.UI.Menu.Screens
{
    public class GameplayScreen : MenuScreen
    {
        public PlayerUIWidget player1Widget;
        public PlayerUIWidget player2Widget;
        public PlayerUIMessagesWidget opponentMessagesWidget;
        public GameInfoWidget gameInfoWidget;
        public ButtonExtended rematchButton;
        public ButtonExtended helpButton;

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
        public GauntletGameScreen gauntletGameScreen { get; private set; }

        private SpellsListUIWidget spellsListWidget;
        private UIOutline helpButtonOutline;

        private bool timersEnabled => GameManager.Instance.passAndPlayTimer && (game._Type == GameType.PASSANDPLAY || game._Type == GameType.REALTIME);

        protected override void Awake()
        {
            base.Awake();

            puzzleUI = GetComponentInChildren<PuzzleUIScreen>();
            turnbaseUI = GetComponentInChildren<TurnBaseScreen>();
            passAndPlayUI = GetComponentInChildren<PassAndPlayScreen>();
            realtimeScreen = GetComponentInChildren<RealtimeScreen>();
            demoGameScreen = GetComponentInChildren<DemoGameScreen>();
            gauntletGameScreen = GetComponentInChildren<GauntletGameScreen>();

            spellsListWidget = GetComponentInChildren<SpellsListUIWidget>();
            helpButtonOutline = helpButton.GetComponent<UIOutline>();

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
                            menuController.GetOrAddScreen<PromptScreen>().Prompt(LocalizationManager.Value("leave_game"), "", LocalizationManager.Value("yes"), LocalizationManager.Value("no"), () => GamePlayManager.instance.BackButtonOnClick());

                        break;
                }
            }
        }

        public void InitUI(GamePlayManager gameplayManager)
        {
            this.gameplayManager = gameplayManager;
            game = gameplayManager.game;

            player1Widget.SetGame(game);
            player2Widget.SetGame(game);

            player1Widget.Initialize();
            player2Widget.Initialize();

            player1Widget.SetPlayerName(game.player1.DisplayName);
            player1Widget.SetPlayerIcon(game.player1);
            player1Widget.StopPlayerTurnAnimation();

            if (game.opponent != null)
            {
                player2Widget.SetPlayerName(game.player2.DisplayName);
                player2Widget.SetPlayerIcon(game.player2);
                player2Widget.StopPlayerTurnAnimation();

                if (game.hideOpponent)
                {
                    player2Widget.Hide(.1f);
                }
                else
                {
                    if (game.puzzleData)
                    {
                        if (game.puzzleData.hasAIOpponent)
                            player2Widget.Show(.1f);
                        else
                            player2Widget.Hide(.1f);
                    }
                    else
                        player2Widget.Show(.1f);
                }
            }
            else
                player2Widget.Hide(.1f);

            //initialize timer
            if (timersEnabled)
            {
                timerWidgets[0].AssignPlayer(game.player1);
                timerWidgets[1].AssignPlayer(game.player2);
            }
            //disable timer widgets if not needed
            else
            {
                //close tap overlay of opened
                CancelRoutine("waitFotTap");
                timerWidgets.ForEach(widget => widget.Hide(0f));
            }

            //check if help button available
            switch (game._Type)
            {
                case GameType.PRESENTATION:
                case GameType.ONBOARDING:
                    helpButton.SetActive(false);

                    break;

                default:
                    helpButton.SetActive(true);

                    break;
            }

            puzzleUI.Open(game);
            turnbaseUI.Open(game);
            passAndPlayUI.Open(game);
            realtimeScreen.Open(game);
            demoGameScreen.Open(game);
            gauntletGameScreen.Open(game);

            gameWinLoseScreen.CloseIfOpened();
            puzzleWinLoseScreen.CloseIfOpened();

            spellsListWidget.Open(game, gameplayManager.board, game.me);

            SetResetButtonState(false);
            UpdateHelpButton();
            HideGameInfoWidget(false);
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
            gauntletGameScreen.OnMoveStarted();

            if (turn == null || turn.PlayerId < 1) return;

            HideGameInfoWidget();

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
                            if (onePlayerTurnCounter % Constants.ADD_TIMER_BAR_EVERY_X_TURN == 0)
                                widget.AddTimerValue(Constants.BARS_TO_ADD, true, true);
                            else
                                widget.AddSmallTimerValue(Constants.CIRCULAR_TIMER_VALUE);
                        }
                    }
                });
            }

            #endregion

            SetResetButtonState(true);
        }

        public void OnMoveEnded(ClientPlayerTurn turn, PlayerTurnResult turnResult)
        {
            if (game == null) return;

            puzzleUI.UpdatePlayerTurn();
            passAndPlayUI.UpdatePlayerTurn();
            gauntletGameScreen.UpdatePlayerTurn();

            if (game.isOver) return;

            UpdatePlayerTurnGraphics();

            if (turn == null || turn.PlayerId < 1) return;

            switch (game._Type)
            {
                case GameType.PUZZLE:
                case GameType.AI:
                    if (!game.isMyTurn) gameInfoWidget.SetText(LocalizationManager.Value("thinking")).ShowDelayed(time: .6f);

                    break;
            }

            #region Timers

            if (timersEnabled)
            {
                //activate timers
                ActivatePlayerTimer(game._State.ActivePlayerId);

                //reset timers
                timerWidgets.ForEach(widget => { if (widget.isEmpty) widget.AddTimerValue(Constants.AI_TURN_TIMER_RESET_VALUE, true, true); });
            }

            #endregion

            spellsListWidget.UpdateSpells(game._State.ActivePlayerId);
        }

        public void UpdateHelpButton()
        {
            helpButtonOutline.intensity = gameplayManager.gameState == Mechanics.GameplayScene.GameState.HELP_STATE ? 1f : 0f;
        }

        public void UpdatePlayerTurnGraphics()
        {
            if (game.activePlayer.PlayerId == player1Widget.assignedPlayer.PlayerId)
            {
                player1Widget.ShowPlayerTurnAnimation();
                if (!game.hideOpponent) player2Widget.StopPlayerTurnAnimation();
            }
            else
            {
                player2Widget.ShowPlayerTurnAnimation();
                if (!game.hideOpponent) player1Widget.StopPlayerTurnAnimation();
            }
        }

        public void OnGameStarted()
        {
            UpdatePlayerTurnGraphics();

            puzzleUI.OnGameStarted();

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
            gameInfoWidget.Hide(.3f);

            SetResetButtonState(false);

            if (!game.draw)
            {
                if (game.IsWinner(player1Widget.assignedPlayer))
                    player1Widget.StartWinJumps();
                else
                    player2Widget.StartWinJumps();
            }

            if (game.puzzleData)
            {
                puzzleUI.GameComplete();

                //open puzzle win/lose screen
                if (game.isFourzyPuzzle)
                    puzzleWinLoseScreen.Open(game);
                else
                    gameWinLoseScreen.Open(game);
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

        private void ActivatePlayerTimer(int playerID) => timerWidgets.Find(widget => widget.player.PlayerId == playerID).Activate();

        private void HideGameInfoWidget(bool checkType = true)
        {
            if (checkType)
                switch (game._Type)
                {
                    case GameType.AI:
                    case GameType.PUZZLE:
                        if (game.isMyTurn) gameInfoWidget.Hide(.3f);

                        break;
                }
            else
                gameInfoWidget.Hide(.3f);
        }

        private void SetResetButtonState(bool state)
        {
            if (state)
            {
                switch (game._Mode)
                {
                    case GameMode.AI_PACK:
                    case GameMode.BOSS_AI_PACK:
                    case GameMode.GAUNTLET:
                    case GameMode.PUZZLE_FAST:
                    case GameMode.PUZZLE_PACK:
                        if (game._allTurnRecord.Count == 1 && !game.isOver)
                        {
                            rematchButton.SetState(true);
                            rematchButton.Show(.3f);
                        }

                        break;
                }
            }
            else
            {
                if (rematchButton.interactable)
                {
                    rematchButton.Hide(.3f);
                    rematchButton.SetState(false);
                }
            }
        }

        private IEnumerator WaitForTapRoutine()
        {
            while (!isCurrent) yield return null;
        }
    }
}
