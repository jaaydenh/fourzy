//@vadym udod

using ExitGames.Client.Photon;
using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Managers;
using Fourzy._Updates.Mechanics.GameplayScene;
using Fourzy._Updates.Tools;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Widgets;
using FourzyGameModel.Model;
using Photon.Pun;
using StackableDecorator;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
        [SerializeField]
        private TMP_Text matchId;

        public List<TimerSliderWidget> timerWidgets;

        public PuzzleWinLoseScreen puzzleWinLoseScreen;
        public GameWinLoseScreen gameWinLoseScreen;

        [HelpBox("Pass And Play mode overlay", messageType = MessageType.None)]
        public MenuScreen tapToStartOverlay;

        private IClientFourzy game;
        private GamePlayManager gameplayManager;
        private int gameTurnCounter;
        private UIOutline helpButtonOutline;
        private MagicState magicState = MagicState.DISABLED;

        public bool timersEnabled { get; private set; }
        public bool magicEnabled => magicState == MagicState.BOTH || magicState == MagicState.ONLY_PLAYER_1;
        public PuzzleUIScreen puzzleUI { get; private set; }
        public TurnBaseScreen turnbaseUI { get; private set; }
        public PassAndPlayScreen passAndPlayUI { get; private set; }
        public RealtimeScreen realtimeScreen { get; private set; }
        public DemoGameScreen demoGameScreen { get; private set; }
        public GauntletGameScreen gauntletGameScreen { get; private set; }
        public SkillzGameScreen skillzGameScreen { get; private set; }
        public float MyTimerLeft
        {
            get
            {
                if (game == null)
                {
                    return -1f;
                }
                else
                {
                    switch (game._Type)
                    {
                        case GameType.SKILLZ_ASYNC:
                            return skillzGameScreen.Timer;

                        default:
                            return timersEnabled ? timerWidgets[0].TotalTimeLeft : -1f;
                    }
                }
            }
        }
        public float OpponentTimerLeft => timersEnabled ? timerWidgets[1].TotalTimeLeft : -1f;
        public int MyMagicLeft => magicEnabled ? player1Widget.magic : -1;
        public float Player1TimeLeft
        {
            get
            {
                if (player1Widget.assignedPlayer == game.player1)
                {
                    return timerWidgets[0].TotalTimeLeft;
                }
                else
                {
                    return timerWidgets[1].TotalTimeLeft;
                }
            }
            set
            {
                if (player1Widget.assignedPlayer == game.player1)
                {
                    timerWidgets[0].TotalTimeLeft = value;
                }
                else
                {
                    timerWidgets[1].TotalTimeLeft = value;
                }
            }
        }
        public float Player2TimeLeft
        {
            get
            {
                if (player2Widget.assignedPlayer == game.player2)
                {
                    return timerWidgets[1].TotalTimeLeft;
                }
                else
                {
                    return timerWidgets[0].TotalTimeLeft;
                }
            }
            set
            {
                if (player2Widget.assignedPlayer == game.player2)
                {
                    timerWidgets[1].TotalTimeLeft = value;
                }
                else
                {
                    timerWidgets[0].TotalTimeLeft = value;
                }
            }
        }
        public int Player1Magic
        {
            get
            {
                if (player1Widget.assignedPlayer == game.player1)
                {
                    return player1Widget.magic;
                }
                else
                {
                    return player2Widget.magic;
                }
            }
            set
            {
                if (player1Widget.assignedPlayer == game.player1)
                {
                    player1Widget.SetMagic(value);
                }
                else
                {
                    player2Widget.SetMagic(value);
                }
            }
        }
        public int Player2Magic
        {
            get
            {
                if (player2Widget.assignedPlayer == game.player2)
                {
                    return player2Widget.magic;
                }
                else
                {
                    return player1Widget.magic;
                }
            }
            set
            {
                if (player2Widget.assignedPlayer == game.player2)
                {
                    player2Widget.SetMagic(value);
                }
                else
                {
                    player1Widget.SetMagic(value);
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();

            puzzleUI = GetComponentInChildren<PuzzleUIScreen>();
            turnbaseUI = GetComponentInChildren<TurnBaseScreen>();
            passAndPlayUI = GetComponentInChildren<PassAndPlayScreen>();
            realtimeScreen = GetComponentInChildren<RealtimeScreen>();
            demoGameScreen = GetComponentInChildren<DemoGameScreen>();
            gauntletGameScreen = GetComponentInChildren<GauntletGameScreen>();
            skillzGameScreen = GetComponentInChildren<SkillzGameScreen>();

            helpButtonOutline = helpButton.GetComponent<UIOutline>();

            timerWidgets.ForEach(widget => widget.onValueEmpty += OnTimerEmpty);

            GamePlayManager.onMoveStarted += OnMoveStarted;
            GamePlayManager.onMoveEnded += OnMoveEnded;
        }

        protected void OnDestroy()
        {
            GamePlayManager.onMoveStarted -= OnMoveStarted;
            GamePlayManager.onMoveEnded -= OnMoveEnded;
        }

        public override void OnBack()
        {
            base.OnBack();

            if (game == null)
            {
                GamePlayManager.Instance.BackButtonOnClick();
            }
            else
            {
                switch (game._Type)
                {
                    case GameType.PUZZLE:
                    case GameType.TURN_BASED:
                    case GameType.PRESENTATION:
                        GamePlayManager.Instance.BackButtonOnClick();

                        break;

                    case GameType.REALTIME:
                        //cant leave realtime game anymore

                        //if (game.isOver)
                        //{
                        //    GamePlayManager.Instance.BackButtonOnClick();
                        //}
                        //else
                        //{
                        //    menuController.GetOrAddScreen<PromptScreen>()
                        //        .Prompt(
                        //            LocalizationManager.Value("are_you_sure"),
                        //            LocalizationManager.Value("leave_realtime_game_message"),
                        //            LocalizationManager.Value("yes"),
                        //            LocalizationManager.Value("no"),
                        //            () => GamePlayManager.Instance.BackButtonOnClick());
                        //}
                        if (game.IsOver)
                        {
                            GamePlayManager.Instance.BackButtonOnClick();
                        }

                        break;

                    case GameType.SKILLZ_ASYNC:
                        menuController.GetOrAddScreen<SkillzPauseMenuScreen>()._Open();

                        break;

                    default:
                        if (game.IsOver)
                        {
                            GamePlayManager.Instance.BackButtonOnClick();
                        }
                        else
                        {
                            menuController.GetOrAddScreen<PromptScreen>()
                                .Prompt(
                                    LocalizationManager.Value("leave_game"),
                                    "",
                                    LocalizationManager.Value("yes"),
                                    LocalizationManager.Value("no"),
                                    () => GamePlayManager.Instance.BackButtonOnClick());
                        }

                        break;
                }
            }
        }

        public void InitializeUI(GamePlayManager gameplayManager)
        {
            this.gameplayManager = gameplayManager;
            game = gameplayManager.Game;

            #region Timer usage

            timersEnabled = Tools.Utils.GetTimerState(game);

            #endregion

            #region Magic usage

            magicState = MagicState.DISABLED;
            switch (game._Type)
            {
                //case GameType.AI:
                case GameType.PASSANDPLAY:
                    if (SettingsManager.Get(SettingsManager.KEY_REALTIME_MAGIC))
                    {
                        magicState = MagicState.BOTH;
                    }

                    break;

                case GameType.REALTIME:
                    switch (GameManager.Instance.ExpectedGameType)
                    {
                        case GameTypeLocal.REALTIME_LOBBY_GAME:
                        case GameTypeLocal.REALTIME_QUICKMATCH:
                            magicState =
                                (MagicState)FourzyPhotonManager.GetRoomProperty(Constants.REALTIME_ROOM_MAGIC_KEY, 0);

                            break;

                        case GameTypeLocal.REALTIME_BOT_GAME:
                            magicState = SettingsManager.Get(SettingsManager.KEY_REALTIME_MAGIC) ? 
                                MagicState.BOTH : 
                                MagicState.DISABLED;

                            break;
                    }

                    break;

                default:
                    magicState = MagicState.DISABLED;

                    break;
            }

            switch (game._Mode)
            {
                case GameMode.GAUNTLET:
                    magicState = MagicState.ONLY_PLAYER_1;

                    break;
            }

            #endregion

            Player me = game.me;

            if (magicEnabled)
            {
                player1Widget.spellsHolder.SetData(game, gameplayManager.BoardView, me);
                player1Widget.SetMagicWidget(true);
            }
            else
            {
                player1Widget.SetMagicWidget(false);
            }

            player1Widget.SetGame(game);
            player2Widget.SetGame(game);

            player1Widget.Initialize();
            player2Widget.Initialize();

            player1Widget.SetPlayer(me);
            player1Widget.StopPlayerTurnAnimation();

            #region Set P1 Rating

            switch (game._Type)
            {
                case GameType.REALTIME:
                    player1Widget.SetRating(
                        UserManager.Instance.lastCachedRating,
                        UserManager.Instance.totalRatedGames);

                    break;

                default:
                    player1Widget.SetExtraData("Wizard");

                    break;
            }

            #endregion

            Player opponent = game.opponent;
            if (opponent != null)
            {
                player2Widget.SetPlayer(opponent);
                player2Widget.StopPlayerTurnAnimation();

                #region Set P2 Rating

                switch (game._Type)
                {
                    case GameType.REALTIME:
                        switch (GameManager.Instance.ExpectedGameType)
                        {
                            case GameTypeLocal.REALTIME_LOBBY_GAME:
                            case GameTypeLocal.REALTIME_QUICKMATCH:
                                player2Widget.SetRating(
                                    FourzyPhotonManager.GetOpponentProperty(
                                        Constants.REALTIME_RATING_KEY, -1),
                                    FourzyPhotonManager.GetOpponentTotalGames());

                                break;

                            case GameTypeLocal.REALTIME_BOT_GAME:
                                player2Widget.SetRating(
                                    GameManager.Instance.RealtimeOpponent.Rating,
                                    GameManager.Instance.RealtimeOpponent.TotalGames);

                                break;
                        }

                        break;

                    default:
                        player2Widget.SetExtraData("Wizard");

                        break;
                }

                #endregion

                if (game.hideOpponent)
                {
                    player2Widget.Hide(.1f);
                }
                else
                {
                    if (game.puzzleData)
                    {
                        if (game.puzzleData.hasAIOpponent)
                        {
                            ShowPlayer2Magic();
                        }
                        else
                        {
                            player2Widget.Hide(.1f);
                        }
                    }
                    else
                    {
                        ShowPlayer2Magic();
                    }

                    void ShowPlayer2Magic()
                    {
                        player2Widget.Show(.1f);

                        if (magicState == MagicState.BOTH)
                        {
                            player2Widget.spellsHolder.SetData(game, gameplayManager.BoardView, opponent);
                            player2Widget.SetMagicWidget(true);
                        }
                        else
                        {
                            player2Widget.SetMagicWidget(false);
                        }
                    }
                }
            }
            else
            {
                player2Widget.Hide(.1f);
            }

            //initialize timer
            if (timersEnabled)
            {
                timerWidgets[0].AssignPlayer(me);
                timerWidgets[1].AssignPlayer(opponent);
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

            //position help button
            Vector2 viewportPoint = Camera.main.WorldToViewportPoint(
                gameplayManager.BoardView.BoardLocationToVec2(
                    new BoardLocation(0, 0)) + (Vector2)GamePlayManager.Instance.BoardView.transform.position);
            helpButton.rectTransform.anchorMin = helpButton.rectTransform.anchorMax = viewportPoint;
            if (GameManager.Instance.Landscape)
            {
                helpButton.rectTransform.anchoredPosition = new Vector2(-160f, 0f);
            }
            else
            {
                helpButton.rectTransform.anchoredPosition = new Vector2(0f, 110f);
            }

            puzzleUI.Open(game);
            turnbaseUI.Open(game);
            passAndPlayUI.Open(game);
            realtimeScreen.Open(game);
            demoGameScreen.Open(game);
            gauntletGameScreen.Open(game);
            skillzGameScreen.Open(game);

            gameWinLoseScreen.CloseIfOpened();
            puzzleWinLoseScreen.CloseIfOpened();

            SetResetButtonState(false);
            UpdateHelpButton();
            HideGameInfoWidget(false);
        }

        public void SetMatchID(string matchID)
        {
            matchId.text = matchID;
        }

        public void OnWrongTurn()
        {
            gameInfoWidget.NotYourTurn();
        }

        public void OnRealtimeTurnRecieved(ClientPlayerTurn turn)
        {
            if (timersEnabled)
            {
                timerWidgets[1].TotalTimeLeft = turn.playerTimerLeft;
            }

            //adjust magic
            player2Widget.SetMagic(turn.magicLeft);
        }

        public void UpdateHelpButton()
        {
            helpButtonOutline.intensity = gameplayManager.GameState == Mechanics.GameplayScene.GameState.HELP_STATE ? 1f : 0f;
        }

        public void UpdatePlayerTurnGraphics()
        {
            if (game.activePlayer.PlayerId == player1Widget.assignedPlayer.PlayerId)
            {
                player1Widget.ShowPlayerTurnAnimation();

                if (!game.hideOpponent)
                {
                    player2Widget.StopPlayerTurnAnimation();
                }
            }
            else
            {
                player2Widget.ShowPlayerTurnAnimation();

                if (!game.hideOpponent)
                {
                    player1Widget.StopPlayerTurnAnimation();
                }
            }
        }

        public void OnGameStarted()
        {
            UpdatePlayerTurnGraphics();

            puzzleUI.OnGameStarted();
            gameWinLoseScreen.OnGameStarted();

            switch (game._Type)
            {
                case GameType.PASSANDPLAY:
                    if (GameManager.Instance.tapToStartGame)
                    {
                        menuController.OpenScreen(tapToStartOverlay);

                        //wait for tap screen to be closed
                        StartRoutine("waitFotTap", WaitForTapRoutine(), () =>
                        {
                            if (timersEnabled)
                            {
                                ActivatePlayerTimer(game._State.ActivePlayerId);
                            }
                        },
                        () => tapToStartOverlay.CloseSelf());
                    }
                    else
                    {
                        if (timersEnabled)
                        {
                            ActivatePlayerTimer(game._State.ActivePlayerId);
                        }
                    }

                    break;

                case GameType.REALTIME:
                    if (timersEnabled)
                    {
                        ActivatePlayerTimer(game._State.ActivePlayerId);
                    }

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
                {
                    player1Widget.StartWinJumps();
                }
                else
                {
                    player2Widget.StartWinJumps();
                }
            }

            if (game.puzzleData)
            {
                puzzleUI.GameComplete();

                //open puzzle win/lose screen
                if (game.isFourzyPuzzle)
                {
                    puzzleWinLoseScreen.Open(game);
                }
                else
                {
                    gameWinLoseScreen.Open(game);
                }
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

                    case GameType.SKILLZ_ASYNC:
                        SkillzPauseMenuScreen pauseMenu = menuController.GetScreen<SkillzPauseMenuScreen>();
                        if (pauseMenu && pauseMenu.isOpened)
                        {
                            pauseMenu._Close();
                        }

                        skillzGameScreen.GameComplete();

                        break;
                }

                //win/lose screen
                switch (game._Type)
                {
                    case GameType.ONBOARDING:
                        switch (game._Mode)
                        {
                            case GameMode.VERSUS:
                                //open game win/lose screen
                                gameWinLoseScreen.Open(game);

                                break;
                        }

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
            switch (game._Type)
            {
                case GameType.SKILLZ_ASYNC:
                    skillzGameScreen.Pause();

                    break;

                default:
                    //pause timers
                    timerWidgets.ForEach(widget => widget.Pause());

                    break;
            }
        }

        public void OnGameUnpaused()
        {
            switch (game._Type)
            {
                case GameType.SKILLZ_ASYNC:
                    skillzGameScreen.Unpause();

                    break;

                default:
                    //unpause timers
                    timerWidgets.ForEach(widget => widget.Unpause());

                    break;
            }
        }

        private void OnMoveStarted(ClientPlayerTurn turn, bool startTurn)
        {
            if (startTurn) return;

            puzzleUI.OnMoveStarted();
            gauntletGameScreen.OnMoveStarted();
            skillzGameScreen.OnMoveStarted();

            if (turn == null || turn.PlayerId < 1) return;

            HideGameInfoWidget();

            #region Timers

            if (timersEnabled)
            {
                if (game._FirstState.ActivePlayerId == turn.PlayerId)
                {
                    gameTurnCounter++;
                }

                //deactivate timers, add timer value
                timerWidgets.ForEach(widget =>
                {
                    if (widget.player.PlayerId == turn.PlayerId)
                    {
                        widget.Deactivate();

                        if (!widget.IsEmpty)
                        {
                            if (InternalSettings.Current.ADD_TIMER_BAR_EVERY_X_TURN > 0 &&
                                gameTurnCounter % InternalSettings.Current.ADD_TIMER_BAR_EVERY_X_TURN == 0)
                            {
                                widget.AddTimerValue(InternalSettings.Current.BARS_TO_ADD, true, true);
                            }
                            else
                            {
                                widget.AddSmallTimerValue(InternalSettings.Current.CIRCULAR_TIMER_SECONDS);
                            }
                        }
                    }
                });
            }

            #endregion

            SetResetButtonState(true);
        }

        private void OnMoveEnded(ClientPlayerTurn turn, PlayerTurnResult turnResult, bool startTurn)
        {
            if (startTurn) return;

            puzzleUI.UpdatePlayerTurn();
            passAndPlayUI.UpdatePlayerTurn();
            gauntletGameScreen.UpdatePlayerTurn();
            skillzGameScreen.UpdatePlayerTurn();

            if (game.IsOver) return;

            UpdatePlayerTurnGraphics();

            if (turn == null || turn.PlayerId < 1) return;

            switch (game._Type)
            {
                case GameType.PUZZLE:
                case GameType.AI:
                    if (!game.isMyTurn)
                    {
                        gameInfoWidget.SetText(LocalizationManager.Value("thinking")).ShowDelayed(time: .6f);
                    }

                    break;
            }

            #region Timers

            if (timersEnabled)
            {
                //activate timer
                ActivatePlayerTimer(game._State.ActivePlayerId);

                //reset timers
                timerWidgets.ForEach(widget =>
                {
                    if (widget.IsEmpty)
                    {
                        widget.AddTimerValue(InternalSettings.Current.RESET_TIMER_SECTIONS, true, true);
                    }
                });
            }

            #endregion
        }

        private void OnTimerEmpty(Player player)
        {
            switch (game._Type)
            {
                case GameType.PASSANDPLAY:
                    if (InternalSettings.Current.LOSE_ON_EMPTY_TIMER)
                    {
                        timerWidgets.ForEach(_widget => _widget.Deactivate());
                        gameplayManager.OnGameFinished(game);
                    }
                    else
                    {
                        gameplayManager.BoardView.TakeAITurn();
                    }

                    break;

                case GameType.REALTIME:
                    if (InternalSettings.Current.LOSE_ON_EMPTY_TIMER)
                    {
                        timerWidgets.ForEach(_widget => _widget.Deactivate());

                        switch (GameManager.Instance.ExpectedGameType)
                        {
                            case GameTypeLocal.REALTIME_BOT_GAME:
                                //set other as winner
                                game._State.WinnerId = game.isMyTurn ? game.opponent.PlayerId : game.me.PlayerId;
                                gameplayManager.OnGameFinished(game);

                                break;

                            case GameTypeLocal.REALTIME_LOBBY_GAME:
                            case GameTypeLocal.REALTIME_QUICKMATCH:
                                if (game.isMyTurn)
                                {
                                    //set other as winner
                                    game._State.WinnerId = game.opponent.PlayerId;
                                    gameplayManager.OnGameFinished(game);

                                    Debug.Log("Tell other client that game's over");
                                    if (PhotonNetwork.CurrentRoom != null)
                                    {
                                        var eventOptions = new Photon.Realtime.RaiseEventOptions();
                                        eventOptions.Flags.HttpForward = true;
                                        eventOptions.Flags.WebhookFlags = Photon.Realtime.WebFlags.HttpForwardConst;

                                        var photonEventResult = PhotonNetwork.RaiseEvent(
                                            Constants.RATING_GAME_OTHER_LOST,
                                            null,
                                            eventOptions,
                                            SendOptions.SendReliable);
                                    }
                                }

                                break;
                        }
                    }
                    else
                    {
                        if (game.isMyTurn)
                        {
                            gameplayManager.BoardView.TakeAITurn();
                        }
                        else if (game.opponent.Profile != AIProfile.Player)
                        {
                            game._State.WinnerId = game.me.PlayerId;
                            gameplayManager.OnGameFinished(game);
                        }
                    }

                    break;
            }
        }

        private void ActivatePlayerTimer(int playerId)
        {
            if (playerId < 1) return;

            timerWidgets.Find(widget => widget.player.PlayerId == playerId).Activate();
        }

        private void HideGameInfoWidget(bool checkType = true)
        {
            if (checkType)
            {
                switch (game._Type)
                {
                    case GameType.AI:
                    case GameType.PUZZLE:
                        if (game.isMyTurn) gameInfoWidget.Hide(.3f);

                        break;
                }
            }
            else
            {
                gameInfoWidget.Hide(.3f);
            }
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
                        if (game._allTurnRecord.Count == 1 && !game.IsOver)
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

        private enum MagicState
        {
            DISABLED,
            ONLY_PLAYER_1,
            BOTH,
        }
    }
}
