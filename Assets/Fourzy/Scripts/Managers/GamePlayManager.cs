//modded @vadym udod

#if !MOBILE_SKILLZ
using ExitGames.Client.Photon;
using Photon.Pun;
#endif

using Fourzy._Updates.Audio;
using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Managers;
using Fourzy._Updates.Mechanics._GamePiece;
using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tools;
using Fourzy._Updates.UI.Helpers;
using Fourzy._Updates.UI.Menu;
using Fourzy._Updates.UI.Menu.Screens;
using FourzyGameModel.Model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using static Fourzy._Updates.Serialized.AreasDataHolder;

namespace Fourzy._Updates.Mechanics.GameplayScene
{
    public class GamePlayManager : RoutinesBase
    {
        public static GamePlayManager Instance;

        public static Action<IClientFourzy> onBoardInitialized;
        public static Action<IClientFourzy> onGameStarted;
        public static Action<IClientFourzy> onGameFinished;
        public static Action<IClientFourzy> onDraw;
        public static Action<GamePieceView> onGamepiceSpawned;
        public static Action<ClientPlayerTurn, bool> onMoveStarted;
        public static Action<ClientPlayerTurn, PlayerTurnResult, bool> onMoveEnded;

        public static Action<string> OnGamePlayMessage;
        public static Action<long> OnTimerUpdate;

        public TouchZone touchZone;
        public FourzyGameMenuController menuController;
        public WinningParticleGenerator winningParticleGenerator;
        public Transform bgParent;
        public GameObject noNetworkOverlay;
        public RectTransform hintBlocksParent;
        public GameCameraManager gameCameraManager;
        public ButtonExtended backButton;

        [HideInInspector]
        public bool rematchRequested = false;

        private AudioHolder.BGAudio gameplayBGAudio;
        private PromptScreen realtimeWaitingOtherScreen;
        private PromptScreen playerLeftScreen;

        private bool ratingUpdated = true;
        /// <summary>
        /// for adventure map
        /// </summary>
        private float inPauseTime;
        private bool isSolved = false;
        private bool waitingForPlayerRejoinReady;
        private bool otherPlayerForfeit;
        private bool rejoinGame;
        private GameState previousGameState;
        private int gauntletRechargedViaGems = 0;
        private int gauntletRechargedViaAds = 0;
        private string hintRemovedToken;

        public BackgroundConfigurationData CurrentBGConfiguration { get; private set; }
        public GameboardView BoardView { get; private set; }
        public GameplayBG BG { get; private set; }
        public GameplayScreen GameplayScreen { get; private set; }
        public RandomPlayerPickScreen PlayerPickScreen { get; private set; }
        public GameWinLoseScreen GameWinLoseScreen { get; private set; }

        public IClientFourzy Game { get; private set; }
        public IClientFourzy PrevGame { get; private set; }

        public float realtimeTimerDelay
        {
            get
            {
#if !MOBILE_SKILLZ
                return Mathf.Min(PhotonNetwork.GetPing() * .002f, .5f);
#else
                return 0f;
#endif
            }
        }
        public bool IsBoardReady { get; private set; }
        public bool GameStarted { get; private set; }
        public GameState GameState { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            Instance = this;
        }

        protected void Start()
        {
            GameplayScreen = menuController.GetScreen<GameplayScreen>();
            PlayerPickScreen = menuController.GetScreen<RandomPlayerPickScreen>();
            GameWinLoseScreen = menuController.GetScreen<GameWinLoseScreen>();

            touchZone.onPointerDownData += OnPointerDown;
            touchZone.onPointerUpData += OnPointerRelease;

            GameManager.onNetworkAccess += OnNetwork;
            GameManager.ratingDataReceived += OnRatingDataAquired;
            LoginManager.OnDeviceLoginComplete += OnLogin;

            UserManager.onHintsUpdate += OnHintUpdate;

#if !MOBILE_SKILLZ
            FourzyPhotonManager.onRoomPropertiesUpdate += OnRoomPropertiesUpdate;
            FourzyPhotonManager.onPlayerEnteredRoom += OnPlayerEnteredRoom;
            FourzyPhotonManager.onPlayerLeftRoom += OnPlayerLeftRoom;
            FourzyPhotonManager.onEvent += OnEventCall;
#endif

            if (SettingsManager.Get(SettingsManager.KEY_DEMO_MODE))
            {
                PointerInputModuleExtended.noInput += OnNoInput;
            }

            HeaderScreen.Instance.Close();

            CheckGameMode();
        }

        protected void OnDestroy()
        {
            if (BoardView)
            {
                BoardView.onGameFinished -= OnGameFinished;
                BoardView.onDraw -= OnDraw;
                BoardView.onMoveStarted -= OnMoveStarted;
                BoardView.onMoveEnded -= OnMoveEnded;
                BoardView.onGamepieceSmashed -= OnGamePieceSmashed;
                BoardView.onPieceSpawned -= OnGamepieceSpawned;
            }

            GameManager.onNetworkAccess -= OnNetwork;
            GameManager.ratingDataReceived -= OnRatingDataAquired;
            LoginManager.OnDeviceLoginComplete -= OnLogin;

            UserManager.onHintsUpdate -= OnHintUpdate;

#if !MOBILE_SKILLZ
            FourzyPhotonManager.onRoomPropertiesUpdate -= OnRoomPropertiesUpdate;
            FourzyPhotonManager.onPlayerEnteredRoom -= OnPlayerEnteredRoom;
            FourzyPhotonManager.onPlayerLeftRoom -= OnPlayerLeftRoom;
            FourzyPhotonManager.onEvent -= OnEventCall;
#endif

            if (SettingsManager.Get(SettingsManager.KEY_DEMO_MODE))
            {
                PointerInputModuleExtended.noInput -= OnNoInput;
            }

            AudioHolder.instance.StopBGAudio(gameplayBGAudio, .5f);
        }

        private void OnApplicationPause(bool pause)
        {
            if (Game == null)
            {
                return;
            }

            switch (Game._Type)
            {
                case GameType.SKILLZ_ASYNC:
                    if (pause)
                    {
                        if (Game.IsOver)
                        {
                            if (SkillzGameController.Instance.HaveNextGame)
                            {
                                StartNextGame();
                            }
                        }

                        if (SkillzGameController.Instance.MatchPausesLeft == 0)
                        {
                            inPauseTime = Time.realtimeSinceStartup;
                        }
                    }
                    else
                    {
                        if (SkillzGameController.Instance.CurrentMatch == null) return;

                        //deduct wait time
                        if (SkillzGameController.Instance.MatchPausesLeft == 0)
                        {
                            inPauseTime = Time.realtimeSinceStartup - inPauseTime;
                            GameplayScreen.skillzGameScreen.DeductTimer(inPauseTime);
                        }

                        if (GameplayScreen.skillzGameScreen.Timer > 0f)
                        {
                            menuController.GetOrAddScreen<PauseMenuScreen>()._Open();
                        }
                    }

                    break;
            }

#if !MOBILE_SKILLZ
            switch (GameManager.Instance.ExpectedGameType)
            {
                case GameTypeLocal.REALTIME_LOBBY_GAME:
                case GameTypeLocal.REALTIME_QUICKMATCH:
                    if (Game != null && !Game.IsOver)
                    {
                        PlayerPrefsWrapper.SetAbandonedRealtimeRoomName(PhotonNetwork.CurrentRoom.Name);
                    }

                    break;
            }
#endif
        }

        protected void OnApplicationQuit()
        {
            switch (GameManager.Instance.ExpectedGameType)
            {
                case GameTypeLocal.LOCAL_GAME:
                    LogGameComplete(true);

                    break;

                //bot game to be reported by me, others will be reported by another client
                case GameTypeLocal.REALTIME_BOT_GAME:
                    switch (GameManager.Instance.botGameType)
                    {
                        case GameManager.BotGameType.FTUE_RATED:
                        case GameManager.BotGameType.REGULAR:
                            GameManager.Instance.ReportBotGameFinished(Game, true, true);

                            //will get logged, but not with up to date rating values
                            GameManager.Instance.LogGameComplete(Game);

                            break;

                        case GameManager.BotGameType.FTUE_NOT_RATED:
                            GameManager.Instance.ReportBotGameFinished(Game, false, true);

                            //will get logged, but not with up to date rating values
                            GameManager.Instance.LogGameComplete(Game);

                            break;
                    }

                    break;

#if !MOBILE_SKILLZ
                //for editor/pc
                case GameTypeLocal.REALTIME_LOBBY_GAME:
                case GameTypeLocal.REALTIME_QUICKMATCH:
                    if (Game != null && !Game.IsOver)
                    {
                        PlayerPrefsWrapper.SetAbandonedRealtimeRoomName(PhotonNetwork.CurrentRoom.Name);
                    }

                    break;
#endif
            }

#if !MOBILE_SKILLZ
            //for editor/pc
            switch (GameManager.Instance.ExpectedGameType)
            {
                case GameTypeLocal.REALTIME_BOT_GAME:
                case GameTypeLocal.REALTIME_LOBBY_GAME:
                case GameTypeLocal.REALTIME_QUICKMATCH:
                    PlayerPrefsWrapper.AddRealtimGamesAbandoned();
                    AnalyticsManager.Instance.AmplitudeSetUserProperty("totalRealtimeGamesAbandoned", PlayerPrefsWrapper.GetRealtimeGamesAbandoned());

                    break;
            }
#endif
        }

        public void BackButtonOnClick()
        {
            switch (GameManager.Instance.ExpectedGameType)
            {
                case GameTypeLocal.LOCAL_GAME:
                    LogGameComplete(true);

                    break;

#if !MOBILE_SKILLZ
                case GameTypeLocal.REALTIME_BOT_GAME:
                case GameTypeLocal.REALTIME_LOBBY_GAME:
                case GameTypeLocal.REALTIME_QUICKMATCH:
                    if (PhotonNetwork.CurrentRoom != null)
                    {
                        //let other player know we forfeit
                        var eventOptions = new Photon.Realtime.RaiseEventOptions();
                        eventOptions.Flags.HttpForward = true;
                        eventOptions.Flags.WebhookFlags = Photon.Realtime.WebFlags.HttpForwardConst;
                        PhotonNetwork.RaiseEvent(Constants.PLAYER_FORFEIT, UserManager.Instance.userId, eventOptions, SendOptions.SendReliable);

                        FourzyPhotonManager.TryLeaveRoom();
                    }

                    break;
#endif
            }

            GameManager.Instance.OpenMainMenu();
        }

        protected void SetGameIfNull(IClientFourzy _game)
        {
            //if active game is empty, load random pass&play board
            if (_game == null)
            {
                ClientFourzyGame game = null;
                switch (GameManager.Instance.ExpectedGameType)
                {
                    case GameTypeLocal.REALTIME_BOT_GAME:
                        int percent = UserManager.Instance.MyComplexityPercent();

                        Tuple<int, int> minMax = BoardFactory.NormalizeComplexity((Area)PlayerPrefsWrapper.GetCurrentArea(), percent);

                        BoardGenerationPreferences preferences = new BoardGenerationPreferences();
                        preferences.TargetComplexityLow = minMax.Item1;
                        preferences.TargetComplexityHigh = minMax.Item2;

                        //load realtime game
                        game = new ClientFourzyGame(
                            (Area)PlayerPrefsWrapper.GetCurrentArea(),
                            UserManager.Instance.meAsPlayer,
                            GameManager.Instance.Bot,
                            UnityEngine.Random.value > .5f ? 1 : 2,
                            null,
                            preferences)
                        {
                            _Type = GameType.REALTIME
                        };
                        GameManager.Instance.botGameType = GameManager.BotGameType.REGULAR;
                        if (Debug.isDebugBuild)
                        {
                            Debug.Log($"Starting {GameManager.Instance.botGameType} bot game: MinMax: {preferences.TargetComplexityLow}-{preferences.TargetComplexityHigh}, precent: {percent}");
                        }

                        break;

                    case GameTypeLocal.LOCAL_GAME:
                        game = new ClientFourzyGame(GameContentManager.Instance.GetInstructionBoard("ARROW"), UserManager.Instance.meAsPlayer, new Player(2, "Player Two Player Two Player Two Player Two"))
                        {
                            _Type = GameType.PASSANDPLAY
                        };
                        game.SetRandomActivePlayer();

                        break;
                }
                game.UpdateFirstState();

                GameManager.Instance.activeGame = game;
            }
            else if (_game != GameManager.Instance.activeGame)
            {
                GameManager.Instance.activeGame = _game;
            }

            Game = GameManager.Instance.activeGame;

            if (Game.puzzleData)
            {
                isSolved = PlayerPrefsWrapper.GetPuzzleChallengeComplete(Game.puzzleData.ID);
            }

            Debug.Log($"Type: {Game._Type} Mode: {Game._Mode} Expected Local Type: {GameManager.Instance.ExpectedGameType} Opponent: {Game.opponent.Profile}");
        }

        public void LoadGame(IClientFourzy _game)
        {
            CancelRoutine("gameInit");
            CancelRoutine("takeTurn");
            CancelRoutine("realtimeCountdownRoutine");
            CancelRoutine("postGameRoutine");
            CancelRoutine("tokensInstruction");

            if (GameState != GameState.PAUSED)
            {
                GameState = GameState.OPENNING_GAME;
            }
            else
            {
                previousGameState = GameState.OPENNING_GAME;
            }

            GameStarted = false;
            IsBoardReady = false;
            ratingUpdated = false;
            gauntletRechargedViaAds = 0;

            SetGameIfNull(_game);

            LogGameStart();
            GamepadCheck();
            PlayBGAudio();
            GameOpenedCheck();

            //close other screens
            menuController.BackToRoot();

            winningParticleGenerator.HideParticles();

            PlayerPickScreen.SetData(Game);

            LoadBG(Game._Area);
            LoadBoard();

            GameplayScreen.InitializeUI(this);

            StartRoutine("gameInit", GameInitRoutine());

            PrevGame = Game;
        }

        public void CreateRealtimeGame()
        {
#if !MOBILE_SKILLZ
            //only continue if master
            if (PhotonNetwork.IsMasterClient)
            {
                Area _area = FourzyPhotonManager.GetRoomProperty(Constants.REALTIME_ROOM_AREA, InternalSettings.Current.DEFAULT_AREA);

                //get percent
                int myPercent = UserManager.Instance.MyComplexityPercent();
                int opponentPercent = UserManager.GetComplexityPercent(
                    FourzyPhotonManager.GetOpponentTotalGames(),
                    FourzyPhotonManager.GetOpponentProperty(Constants.REALTIME_RATING_KEY, UserManager.Instance.lastCachedRating));

                //get complexity scores
                Tuple<int, int> myMinMax = BoardFactory.NormalizeComplexity(_area, myPercent);
                Tuple<int, int> oppMinMax = BoardFactory.NormalizeComplexity(_area, opponentPercent);

                (int, int) actualMinMax = (Mathf.Min(myMinMax.Item1, oppMinMax.Item1), Mathf.Min(myMinMax.Item2, oppMinMax.Item2));

                BoardGenerationPreferences preferences = new BoardGenerationPreferences();
                preferences.TargetComplexityLow = actualMinMax.Item1;
                preferences.TargetComplexityHigh = actualMinMax.Item2;

                //ready opponent
                Player opponent =
                    new Player(2, PhotonNetwork.PlayerListOthers[0].NickName)
                    {
                        HerdId = FourzyPhotonManager.GetOpponentProperty(Constants.REALTIME_ROOM_GAMEPIECE_KEY, Constants.REALTIME_DEFAULT_GAMEPIECE_KEY),
                        PlayerString = PhotonNetwork.PlayerListOthers[0].UserId
                    };

                Player me = UserManager.Instance.meAsPlayer;

                //load realtime game
                ClientFourzyGame _game = new ClientFourzyGame(
                    _area,
                    me,
                    opponent,
                    UnityEngine.Random.value > .5f ? 1 : 2,
                    null,
                    preferences)
                {
                    _Type = GameType.REALTIME
                };

                if (Application.isEditor || Debug.isDebugBuild)
                {
                    Debug.Log($"Realtime game created: min/max: {actualMinMax.Item1}/{actualMinMax.Item2}");
                }

                RealtimeGameStateData gameStateData = _game.toGameStateData;
                gameStateData.createdEpoch = Utils.EpochMilliseconds();
                gameStateData.player1Id = _game.player1.PlayerString;

                var eventOptions = new Photon.Realtime.RaiseEventOptions();
                eventOptions.Flags.HttpForward = true;
                eventOptions.Flags.WebhookFlags = Photon.Realtime.WebFlags.HttpForwardConst;
                string serializedGameState = JsonConvert.SerializeObject(gameStateData);
                var result = PhotonNetwork.RaiseEvent(Constants.GAME_DATA, serializedGameState, eventOptions, SendOptions.SendReliable);
                Debug.Log("Photon create game event result: " + result);

                //update lastest state
                FourzyPhotonManager.SetRoomProperty("latestState", serializedGameState);

                //set ttl
                PhotonNetwork.CurrentRoom.EmptyRoomTtl = Constants.REALTIME_TTL_SECONDS * 1000;
                PhotonNetwork.CurrentRoom.PlayerTtl = Constants.REALTIME_TTL_SECONDS * 1000;

                LoadGame(_game);
            }
#endif
        }

        public void OnPointerDown(Vector2 position, int pointerId)
        {
            switch (GameState)
            {
                case GameState.GAME:
                    if (CustomInputManager.GamepadCount == 2 && pointerId > -1 && Game._State.ActivePlayerId != (pointerId + 1))
                    {
                        GameplayScreen.OnWrongTurn();

                        return;
                    }

                    if (GameplayScreen != menuController.currentScreen) return;

                    BoardView.OnPointerDown(position);

                    break;

                case GameState.HELP_STATE:
                    BoardView.ShowHelpForTokenAtPosition(position);

                    break;
            }
        }

        public void OnPointerMove(Vector2 position)
        {
            if (GameState != GameState.GAME)
            {
                return;
            }

            if (GameplayScreen != menuController.currentScreen || GameState != GameState.GAME)
            {
                OnPointerRelease(position, -1);

                return;
            }

            BoardView.OnPointerMove(position);
        }

        public void OnPointerRelease(Vector2 position, int pointerId)
        {
            if (GameState != GameState.GAME)
            {
                return;
            }

            if (CustomInputManager.GamepadCount == 2 &&
                pointerId > -1 &&
                Game._State.ActivePlayerId != (pointerId + 1))
            {
                return;
            }

            BoardView.OnPointerRelease(position);
        }

        public void UpdatePlayerTurn()
        {
            if (Game == null) return;

            if (!GameStarted)
            {
                if (GameState != GameState.PAUSED)
                {
                    GameState = GameState.GAME;
                }
                else
                {
                    previousGameState = GameState.GAME;
                }
                GameStarted = true;
                OnGameStarted();
            }

            switch (Game._Type)
            {
                case GameType.AI:
                    if (!Game.IsOver && !Game.isMyTurn)
                    {
                        BoardView.TakeAITurn();
                    }

                    break;

                case GameType.SKILLZ_ASYNC:
                    if (!Game.IsOver && !Game.isMyTurn)
                    {
                        BoardView.TakeAITurn();
                    }

                    break;

                case GameType.PUZZLE:
                    if (!Game.IsOver && !Game.isMyTurn)
                    {
                        BoardView.TakeAITurn();
                    }

                    break;

                case GameType.TRY_TOKEN:
                    if (!Game.IsOver && !Game.isMyTurn)
                    {
                        BoardView.TakeAITurn();
                    }

                    break;

                case GameType.PRESENTATION:
                    if (!Game.IsOver)
                    {
                        BoardView.TakeAITurn();
                    }

                    break;

                case GameType.ONBOARDING:
                    switch (Game._Mode)
                    {
                        case GameMode.VERSUS:
                            if (!Game.IsOver && !Game.isMyTurn)
                            {
                                BoardView.TakeAITurn();
                            }

                            break;
                    }

                    break;

                case GameType.PASSANDPLAY:
                    StandaloneInputModuleExtended.GamepadID = Game._State.ActivePlayerId == 1 ? 0 : 1;

                    break;

                case GameType.REALTIME:
                    switch (GameManager.Instance.ExpectedGameType)
                    {
                        case GameTypeLocal.REALTIME_BOT_GAME:
                            if (!Game.IsOver && !Game.isMyTurn)
                            {
                                BoardView.TakeAITurn(InternalSettings.Current.BOT_SETTINGS.randomTurnDelay);
                            }

                            break;
                    }

                    break;
            }

            // drop first piece for two step placement types
            switch (GameManager.Instance.placementStyle)
            {
                case GameManager.PlacementStyle.TWO_STEP_SWIPE:
                case GameManager.PlacementStyle.TWO_STEP_TAP:
                    if (!Game.IsOver)
                    {
                        switch (Game._Type)
                        {
                            case GameType.PASSANDPLAY:
                                DropPiece(new BoardLocation(0, 4));

                                break;

                            case GameType.ONBOARDING:
                                switch (Game._Mode)
                                {
                                    case GameMode.VERSUS:
                                        if (Game.isMyTurn)
                                        {
                                            DropPiece(new BoardLocation(0, 4));
                                        }

                                        break;
                                }

                                break;

                            default:
                                if (Game.isMyTurn)
                                {
                                    DropPiece(new BoardLocation(0, 4));
                                }

                                break;
                        }
                    }

                    break;
            }

            //location is only used as initial location
            void DropPiece(BoardLocation location)
            {
                List<BoardLocation> possibleMoves = BoardView.GetPossibleMoves();

                if (possibleMoves.Count == 0)
                {
                    return;
                }

                PlayerTurn lastTurn = Game._allTurnRecord.FindLast(_move => _move.PlayerId == Game._State.ActivePlayerId);
                if (lastTurn != null)
                {
                    location = lastTurn.ToBoardLocation(Game);
                }

                if (!possibleMoves.Any(_location => _location.Equals(location)))
                {
                    //pick random one
                    location = possibleMoves.Random();
                }

                Vector3 worldPosition = BoardView.transform.position + (Vector3)BoardView.BoardLocationToVec2(location);
                Vector3 screenPos = menuController._camera.WorldToScreenPoint(worldPosition);

                BoardView.OnPointerDown(screenPos);
                BoardView.OnPointerMove(screenPos);
                BoardView.OnPointerRelease(screenPos);
            }
        }

        public void ReportRematchResult(bool result)
        {
            rematchRequested = false;

            AnalyticsManager.Instance.LogEvent(
                result ? "acceptRealtimeRematch" : "declineRealtimeRematch",
                AnalyticsManager.AnalyticsProvider.ALL,
                new KeyValuePair<string, object>("isWinner", Game.IsOver),
                new KeyValuePair<string, object>(
                    "isBotOpponent",
                    GameManager.Instance.ExpectedGameType == GameTypeLocal.REALTIME_BOT_GAME),
                new KeyValuePair<string, object>("isPrivate", false));
        }

        public void ShowTokensInstructions(IEnumerable<TokensDataHolder.TokenData> tokens, bool ribbon)
        {
            StartRoutine("tokensInstruction", ShowTokensInstructionsRoutine(tokens, ribbon));
        }

        /// <summary>
        /// Triggered after board/tokens fade id + optional delays
        /// </summary>
        private void OnGameStarted()
        {
            onGameStarted?.Invoke(Game);

            GameplayScreen.OnGameStarted();

            BoardView.interactable = true;
            BoardView.OnPlayManagerReady();
            Game.SetInitialTime(Time.time);

            switch (Game._Type)
            {
                case GameType.REALTIME:
                    FourzyPhotonManager.ResetRematchState();

                    break;
            }
        }

        private IEnumerator ShowTokensInstructionsRoutine(IEnumerable<TokensDataHolder.TokenData> tokens, bool ribbon)
        {
            foreach (TokensDataHolder.TokenData token in tokens)
            {
                TokenPrompt popupUI = menuController.GetOrAddScreen<TokenPrompt>(true);

                popupUI.Prompt(token, false, ribbon, true);

                yield return new WaitWhile(() => popupUI.isOpened);

                PlayerPrefsWrapper.SetInstructionPopupDisplayed((int)token.tokenType, true);

                if (!SettingsManager.Get(SettingsManager.KEY_TOKEN_INSTRUCTION))
                {
                    break;
                }
            }
        }

        private IEnumerator ShowTokenInstructionPopupRoutine()
        {
            if (Game == null || Game._Type == GameType.ONBOARDING) yield break;

            HashSet<TokensDataHolder.TokenData> tokens = new HashSet<TokensDataHolder.TokenData>();
            bool includePreviouslyDisplayed = false;

            switch (GameManager.Instance.buildIntent)
            {
                case BuildIntent.MOBILE_SKILLZ:
                    includePreviouslyDisplayed = true;

                    break;
            }

            foreach (BoardSpace boardSpace in Game.boardContent)
            {
                foreach (IToken token in boardSpace.Tokens.Values)
                {
                    if ((!PlayerPrefsWrapper.InstructionPopupWasDisplayed((int)token.Type) || includePreviouslyDisplayed) &&
                        !GameManager.Instance.excludeInstructionsFor.Contains(token.Type))
                    {
                        tokens.Add(GameContentManager.Instance.GetTokenData(token.Type));
                    }
                }
            }

            if (tokens.Count > 0)
            {
                GameState = GameState.PREGAME_DISPLAY_INSTRUCTION;
            }

            //"token unlock ribbon" condition
            bool showRibbon = false;
            switch (GameManager.Instance.buildIntent)
            {
                case BuildIntent.MOBILE_REGULAR:
                    showRibbon = true;

                    break;
            }

            yield return ShowTokensInstructionsRoutine(tokens, showRibbon);
        }

        private IEnumerator FadeGameScreen(float alpha, float fadeTime)
        {
            BoardView.Fade(alpha, fadeTime);
            yield return new WaitForSeconds(fadeTime - .2f);

            FadeTokens(alpha, fadeTime);
            yield return new WaitForSeconds(fadeTime - .2f);

            FadePieces(alpha, fadeTime);
            yield return new WaitForSeconds(fadeTime - .2f);
        }

        private void FadeTokens(float alpha, float fadeTime)
        {
            if (BoardView.tokens.Count > 0)
            {
                BoardView.FadeTokens(alpha, fadeTime);
            }
        }

        private void FadePieces(float alpha, float fadeTime)
        {
            if (BoardView.gamePieces.Count > 0)
            {
                BoardView.FadeGamepieces(alpha, fadeTime);
            }
        }

        public void CheckGameMode()
        {
            GameManager.Instance.botGameType = GameManager.BotGameType.NONE;
            string matchId = "";

            //auto load game if its not realtime mdoe
            switch (GameManager.Instance.ExpectedGameType)
            {
                case GameTypeLocal.REALTIME_LOBBY_GAME:
                case GameTypeLocal.REALTIME_QUICKMATCH:
                    //unload tutorial screen
                    OnboardingScreen.CloseTutorial();

                    UnloadBoard();
                    UnloadBG();

                    rejoinGame = GameManager.Instance.RejoinAbandonedGame;
                    GameManager.Instance.RejoinAbandonedGame = false;

                    if (rejoinGame)
                    {
                        RealtimeGameStateData lastState = JsonConvert.DeserializeObject<RealtimeGameStateData>(FourzyPhotonManager.GetRoomProperty("latestState", ""));
                        //load board from last saved state 
                        ClientFourzyGame _realtimeGame = new ClientFourzyGame(lastState);

                        LoadGame(_realtimeGame);
                        GameplayScreen.UpdateFromRealtimeState(lastState);
                    }

                    //notify that this client is ready
                    FourzyPhotonManager.SetClientReadyState(true);
                    GameplayScreen.realtimeScreen.SetMessage(LocalizationManager.Value("waiting_for_player"));

                    break;

                case GameTypeLocal.REALTIME_BOT_GAME:
                    //unload tutorial screen
                    OnboardingScreen.CloseTutorial();

                    ResourceItem botGameBoardFile = GameContentManager.Instance.GetRealtimeBotBoard(UserManager.Instance.realtimeGamesComplete);

                    if (GameManager.Instance.RealtimeOpponent == null)
                    {
                        GameManager.Instance.RealtimeOpponent =
                            new OpponentData(
                                "bot",
                                1200,
                                InternalSettings.Current.GAMES_BEFORE_RATING_USED);
                    }

                    if (GameManager.Instance.Bot == null)
                    {
                        GameManager.Instance.Bot = new Player(
                            2,
                            CharacterNameFactory.GenerateBotName(AIDifficulty.Easy),
                            AIProfile.EasyAI)
                        {
                            HerdId = GameContentManager.Instance.piecesDataHolder.random.Id,
                            PlayerString = "2",
                        };
                    }

                    if (botGameBoardFile != null)
                    {
                        FTUEGameBoardDefinition _board = JsonConvert.DeserializeObject<FTUEGameBoardDefinition>(
                            botGameBoardFile.Load<TextAsset>().text);

                        if (_board.aiProfile >= 0)
                        {
                            GameManager.Instance.Bot.Profile = _board.AIProfile;
                            GameManager.Instance.botGameType = GameManager.BotGameType.FTUE_NOT_RATED;
                        }
                        else
                        {
                            GameManager.Instance.botGameType = GameManager.BotGameType.FTUE_RATED;
                        }

                        ClientFourzyGame __game = new ClientFourzyGame(
                            _board,
                            UserManager.Instance.meAsPlayer,
                            GameManager.Instance.Bot)
                        {
                            _Type = GameType.REALTIME,
                        };
                        __game.SetRandomActivePlayer();
                        __game.UpdateFirstState();

                        if (Debug.isDebugBuild)
                        {
                            Debug.Log($"Starting {GameManager.Instance.botGameType} bot game, {_board.BoardName}");
                        }

                        LoadGame(__game);
                    }
                    else
                    {
                        LoadGame(null);
                    }

                    break;

                case GameTypeLocal.ASYNC_SKILLZ_GAME:
                    SkillzLevelParams levelParams = SkillzGameController.Instance.LevelsInfo[SkillzGameController.Instance.CurrentLevelIndex];
                    Area area = (Area)levelParams.areaId;
                    AIProfile aiProfile = (AIProfile)levelParams.aiProfile;
                    BoardGenerationPreferences preferences = new BoardGenerationPreferences(area);
                    int explicitSeed = SkillzGameController.Instance.ExplicitSeed;

                    string randomSeed;
                    if (explicitSeed > -1)
                    {
                        randomSeed = explicitSeed + "";
                    }
                    else
                    {
                        randomSeed = (SkillzGameController.Instance.CurrentMatch?.ID.Value.ToString() ?? "default") + SkillzGameController.Instance.CurrentLevelIndex;
                    }

                    matchId = randomSeed;

                    string myName = SkillzGameController.Instance.CurrentMatch?.Players.Find(_player => _player.IsCurrentPlayer)?.DisplayName ?? CharacterNameFactory.GeneratePlayerName();
                    //string opponentName = SkillzGameController.Instance.CurrentMatch?.Players.Find(_player => !_player.IsCurrentPlayer)?.DisplayName ?? CharacterNameFactory.GeneratePlayerName();
                    string opponentName = AIPlayerFactory.GetAIPlayerNameForSkillz(aiProfile);

                    preferences.RequestedRecipe = "";
                    preferences.RecipeSeed = randomSeed;
                    preferences.TargetComplexityLow = levelParams.complexityLow;
                    preferences.TargetComplexityHigh = levelParams.complexityHigh;

                    Player player = new Player(1, myName) { PlayerString = UserManager.Instance.userId, HerdId = UserManager.Instance.gamePieceId };
                    Player opponent = new Player(2, opponentName, aiProfile) { HerdId = levelParams.oppHerdId };

                    GameOptions options = new GameOptions() { PlayersUseSpells = false, MovesReduceHerd = true };
                    ClientFourzyGame game = new ClientFourzyGame(area, player, opponent, 1, options, preferences);
                    game._Type = GameType.SKILLZ_ASYNC;
                    game.State.InitializeHerd(1, SkillzGameController.Instance.MovesPerMatch);
                    game.State.InitializeHerd(2, 999);
                    game.UpdateFirstState();

                    LoadGame(game);

                    break;

                default:
                    LoadGame(GameManager.Instance.activeGame);

                    break;
            }

            GameplayScreen.SetMatchID(matchId);
        }

        public void StartNextGame(bool sendResetEvent = false)
        {
            if (Game == null) return;

            if (sendResetEvent)
            {
                switch (Game._Type)
                {
                    case GameType.ONBOARDING:
                    case GameType.TRY_TOKEN:
                    case GameType.PRESENTATION:

                        break;

                    default:
                        AnalyticsManager.Instance.LogGame(
                            Game.GameToAnalyticsEvent(false),
                            Game,
                            values: new KeyValuePair<string, object>(
                                AnalyticsManager.GAME_RESULT_KEY,
                                AnalyticsManager.GameResultType.reset));

                        break;
                }
            }

            bool switchActivePlayer = false;

            switch (GameManager.Instance.buildIntent)
            {
                case BuildIntent.MOBILE_INFINITY:
                    switchActivePlayer = true;

                    break;
            }

            if (switchActivePlayer)
            {
                Game._FirstState.ActivePlayerId = Game._FirstState.ActivePlayerId == 1 ? 2 : 1;
            }

            BoardView.StopAIThread();
            switch (Game._Type)
            {
                case GameType.AI:
                    switch (GameManager.Instance.buildIntent)
                    {
                        case BuildIntent.MOBILE_INFINITY:
                            NewBoard(GameType.AI);

                            break;

                        default:
                            ResetBoard();

                            break;
                    }

                    break;

                case GameType.PUZZLE:
                case GameType.ONBOARDING:
                case GameType.TRY_TOKEN:
                    ResetBoard();

                    break;

                case GameType.PASSANDPLAY:
                    NewBoard(GameType.PASSANDPLAY);

                    break;

                case GameType.REALTIME:
                    ResetBoard();

                    break;

                case GameType.SKILLZ_ASYNC:
                    if (SkillzGameController.Instance.HaveNextGame)
                    {
                        CheckGameMode();
                    }
                    else
                    {
                        SkillzGameController.Instance.CloseGameOnBack = true;

                        if (!SkillzGameController.Instance.ReturnToSkillzCalled)
                        {
                            if (SkillzCrossPlatform.ReturnToSkillz())
                            {
                                SkillzGameController.Instance.ReturnToSkillzCalled = true;
                            }
                        }
                    }

                    break;
            }

            void NewBoard(GameType type)
            {
                //create new game if random board
                if (Game.asFourzyGame.isBoardRandom)
                {
                    bool isAreaRandom = Game.asFourzyGame.isAreaRandom;

                    Area area = isAreaRandom ? GameContentManager.Instance.areasDataHolder.areas.Random().areaID : Game._Area;
                    Game = new ClientFourzyGame(area,
                        Game.asFourzyGame.player1,
                        Game.asFourzyGame.player2,
                        switchActivePlayer ? Game._FirstState.ActivePlayerId : UnityEngine.Random.value > .5f ? 1 : 2)
                    {
                        _Type = type
                    };
                    Game.UpdateFirstState();
                    Game.asFourzyGame.isAreaRandom = isAreaRandom;
                }
                else
                {
                    Game._Reset();
                }

                LoadGame(Game);
            }

            void ResetBoard()
            {
                Game._Reset();

                LoadGame(Game);
            }
        }

        public void RechargeByAd()
        {
            switch (Game._Mode)
            {
                case GameMode.GAUNTLET:
                    gauntletRechargedViaAds++;

                    break;
            }

            Recharge();
        }

        public void RechargeByGem()
        {
            switch (Game._Mode)
            {
                case GameMode.GAUNTLET:
                    gauntletRechargedViaGems++;

                    break;
            }

            Recharge();
        }

        public void Recharge()
        {
            Game.AddMembers(Constants.GAUNTLET_RECHARGE_AMOUNT);

            UpdatePlayerTurn();
            GameplayScreen.gauntletGameScreen.UpdateCounter();
        }

        public int GetGauntletRechargePrice()
        {
            return (int)Mathf.Pow(Constants.BASE_GAUNTLET_MOVES_COST, gauntletRechargedViaGems);
        }

        public void PlayHint()
        {
            if (Game == null ||
                !Game.isMyTurn ||
                Game.IsOver ||
                !Game.puzzleData ||
                Game.puzzleData.Solution.Count == 0) return;

            if (UserManager.Instance.hints <= 0)
            {
                //currently unavailable
                menuController
                    .GetOrAddScreen<PromptScreen>()
                    .Prompt(
                        LocalizationManager.Value("not_available"),
                        LocalizationManager.Value("not_supported_functionality"),
                        LocalizationManager.Value("back"),
                        "")
                    .CloseOnAccept();
                //PersistantMenuController.Instance
                //    .GetOrAddScreen<StorePromptScreen>()
                //    .Prompt(StorePromptScreen.StoreItemType.HINTS);

                return;
            }

            hintRemovedToken = Guid.NewGuid().ToString();
            UserManager.AddHints(-1, hintRemovedToken);
        }

        public void PauseGame()
        {
            if (GameState == GameState.PAUSED) return;

            previousGameState = GameState;
            GameState = GameState.PAUSED;
            GameplayScreen.OnGamePaused();
        }

        public void UnpauseGame()
        {
            GameState = previousGameState;
            GameplayScreen.OnGameUnpaused();
        }

        public void RecordLastBoardStateAsRoomProperty(bool bothPlayers)
        {
            RealtimeGameStateData gameStateData = Game.toGameStateData;
            gameStateData.UpdateWithGameplayData(this, bothPlayers);

            string serializedGameState = JsonConvert.SerializeObject(gameStateData);
            FourzyPhotonManager.SetRoomProperty("latestState", serializedGameState);

            Debug.Log("Recording state");
        }

        public void ToggleHelpState()
        {
            if (GameState != GameState.HELP_STATE)
            {
                previousGameState = GameState;
                GameState = GameState.HELP_STATE;

                BoardView.SetHelpState(true);
            }
            else
            {
                GameState = previousGameState;

                BoardView.SetHelpState(false);
            }

            GameplayScreen.UpdateHelpButton();
        }

        public void DisplayHint(PlayerTurn turn, string hintMessage = "")
        {
            BoardLocation location = turn.GetMove().ToBoardLocation(Game);
            BoardLocation next = location.Neighbor(turn.GetMove().Direction, 2);

            _Tutorial.OnboardingTask_PointAt pointAtTask =
                new _Tutorial.OnboardingTask_PointAt(new Dictionary<GameManager.PlacementStyle, Vector2[]>()
                {
                    [GameManager.PlacementStyle.EDGE_TAP] =
                    new Vector2[] { new Vector2(location.Column, location.Row) },
                    [GameManager.PlacementStyle.TAP_AND_DRAG] =
                    new Vector2[] { new Vector2(location.Column, location.Row), new Vector2(next.Column, next.Row) }
                },
            hintMessage);

            Rect swipeRect;
            switch (turn.GetMove().Direction)
            {
                case Direction.UP:
                    swipeRect = new Rect(location.Column, location.Row - 2, 1f, 3f);

                    break;

                case Direction.DOWN:
                    swipeRect = new Rect(location.Column, location.Row, 1f, 3f);

                    break;

                case Direction.LEFT:
                    swipeRect = new Rect(location.Column - 2, location.Row, 3f, 1f);

                    break;

                default:
                    swipeRect = new Rect(location.Column, location.Row, 3f, 1f);

                    break;
            }

            _Tutorial.OnboardingTask_ShowMaskedBoardCells maskTask =
                new _Tutorial.OnboardingTask_ShowMaskedBoardCells(new Dictionary<GameManager.PlacementStyle, Rect>()
                {
                    [GameManager.PlacementStyle.EDGE_TAP] =
                    new Rect(location.Column, location.Row, 1f, 1f),
                    [GameManager.PlacementStyle.TAP_AND_DRAG] = swipeRect
                }, UI.Widgets.OnboardingScreenMaskObject.MaskStyle.PX_0);

            PersistantMenuController.Instance
                .GetOrAddScreen<OnboardingScreen>()
                .OpenTutorial(new _Tutorial.Tutorial()
                {
                    name = "hint move",
                    onBack = _Tutorial.TutorialOnBack.IGNORE,
                    tasks = new _Tutorial.OnboardingTask[] {
                    pointAtTask,
                    maskTask,
                    new _Tutorial.OnboardingTask_LimitInput(new Rect(location.Column, location.Row, 1f, 1f)),
                    new _Tutorial.OnboardingTask() { action = _Tutorial.OnboardingActions.ON_MOVE_STARTED, },
                    new _Tutorial.OnboardingTask() { action = _Tutorial.OnboardingActions.RESET_BOARD_INPUT, }
                }
                });
        }

        /// <summary>
        /// Game must be assigned prior to this call
        /// </summary>
        private void LoadBoard()
        {
            UnloadBoard();

            BoardView = Instantiate(CurrentBGConfiguration.gameboardPrefab, transform);
            BoardView.Initialize(Game);
            BoardView.gameplayManager = this;
            BoardView.transform.localPosition = CurrentBGConfiguration.gameboardPrefab.transform.position;
            BoardView.interactable = false;

            BoardView.onGameFinished += OnGameFinished;
            BoardView.onDraw += OnDraw;
            BoardView.onMoveStarted += OnMoveStarted;
            BoardView.onMoveEnded += OnMoveEnded;
            BoardView.onWrongTurn += () => GameplayScreen.OnWrongTurn();
            BoardView.onGamepieceSmashed += OnGamePieceSmashed;
            BoardView.onPieceSpawned += OnGamepieceSpawned;

            //hide tokens/gamepieces
            BoardView.FadeTokens(0f, 0f);
            BoardView.FadeGamepieces(0f, 0f);
        }

        private void OnGamepieceSpawned(GamePieceView gamepiece)
        {
            onGamepiceSpawned?.Invoke(gamepiece);
        }

        private void UnloadBoard()
        {
            if (BoardView)
            {
                Destroy(BoardView.gameObject);
            }
        }

        private void LoadBG(Area area)
        {
            UnloadBG();

            CurrentBGConfiguration = GameContentManager.Instance.areasDataHolder.GetAreaBGConfiguration(area, Camera.main);

            BG = Instantiate(CurrentBGConfiguration.backgroundPrefab, bgParent);
            BG.transform.localPosition = Vector3.zero;
        }

        private void UnloadBG()
        {
            if (BG)
            {
                Destroy(BG.gameObject);
            }
        }

        /// <summary>
        /// Game must be assigned prior to this call
        /// </summary>
        private void GamepadCheck()
        {
            switch (Game._Type)
            {
                case GameType.PASSANDPLAY:
                case GameType.REALTIME:
                    if (Input.GetJoystickNames().Length > 1)
                    {
                        StandaloneInputModuleExtended.GamepadFilter =
                            StandaloneInputModuleExtended.GamepadControlFilter.SPECIFIC_GAMEPAD;
                        StandaloneInputModuleExtended.GamepadID =
                            Game._State.ActivePlayerId == 1 ? 0 : 1;
                    }

                    break;
            }
        }

        private void PlayBGAudio()
        {
            switch (Game._Type)
            {
                default:
                    string gameBGAudio = GameContentManager
                        .Instance
                        .areasDataHolder[Game._Area].bgAudio;

                    if (gameplayBGAudio != null)
                    {
                        if (gameplayBGAudio.type != gameBGAudio)
                        {
                            AudioHolder.instance.StopBGAudio(gameplayBGAudio, .5f);
                            gameplayBGAudio = AudioHolder.instance.PlayBGAudio(gameBGAudio, true, 1f, 3f);
                        }
                    }
                    else
                    {
                        gameplayBGAudio = AudioHolder.instance.PlayBGAudio(gameBGAudio, true, 1f, 3f);
                    }

                    break;
            }
        }

        private void GameOpenedCheck()
        {
            switch (Game._Type)
            {
                // case GameType.TURN_BASED:
                //     //if game was over and havent been viewed yet, set it to viewed
                //     if (game.asFourzyGame.challengeData.lastTurnGame.isOver && !PlayerPrefsWrapper.GetGameViewed(game.BoardID))
                //     {
                //         PlayerPrefsWrapper.SetGameViewed(game.BoardID);

                //         ChallengeManager.OnChallengeUpdateLocal.Invoke(game.asFourzyGame.challengeData);
                //     }
                //     break;

                default:
                    //if current game have puzzle data assigned, set puzzlepack as opened
                    if (Game.puzzleData)
                    {
                        if (Game.puzzleData.pack)
                        {
                            PlayerPrefsWrapper.SetPuzzlePackOpened(Game.puzzleData.pack.packId, true);
                        }
                    }

                    break;
            }
        }

#region Turn Base Calls

        // private void OnChallengeUpdate(ChallengeData gameData)
        // {
        //     StartRoutine("takeTurn", PlayTurnBaseTurn(gameData));
        // }

        // private void OnChallengesUpdate(List<ChallengeData> challenges)
        // {
        //     //if game waits for rematch challenge
        //     if (!string.IsNullOrEmpty(awaitingChallengeID))
        //     {
        //         //find the event we ve been waiting for
        //         ChallengeData challenge = challenges.Find(_challenge => _challenge.challengeInstanceId == awaitingChallengeID);

        //         if (challenge == null) return;

        //         GameManager.Instance.StartGame(challenge.lastTurnGame);
        //     }
        // }

        // private void CreateTurnBasedGameSuccess(LogEventResponse response)
        // {
        //     awaitingChallengeID = response.ScriptData.GetString("challengeInstanceId");
        //     Debug.Log($"Game created {awaitingChallengeID}");
        // }

        // private void CreateTurnBasedGameError(LogEventResponse response)
        // {
        //     Debug.Log("***** Error Creating Turn based game: " + response.Errors.JSON);
        //     AnalyticsManager.Instance.LogError(response.Errors.JSON, AnalyticsManager.AnalyticsErrorType.create_turn_base_game);

        //     if (loadingPrompt && loadingPrompt.isOpened)
        //     {
        //         loadingPrompt.UpdateInfo("Failed to create new game...\n" + response.Errors.JSON);
        //         StartRoutine("closingLoadingPrompt", 5f, () => menuController.CloseCurrentScreen());
        //     }
        // }

#endregion

#if !MOBILE_SKILLZ
        private void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable values)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (values.ContainsKey(Constants.REALTIME_ROOM_PLAYER_1_READY) || values.ContainsKey(Constants.REALTIME_ROOM_PLAYER_2_READY))
                {
                    if (FourzyPhotonManager.CheckPlayersReady())
                    {
                        if (waitingForPlayerRejoinReady)
                        {
                            waitingForPlayerRejoinReady = false;

                            CloseRealtimeScreen();
                            UnpauseGame();
                            UpdatePlayerTurn();
                        }
                        else
                        {
                            CreateRealtimeGame();
                        }

                        FourzyPhotonManager.SetClientsReadyState(false);
                    }
                }
                else if (values.ContainsKey(Constants.REALTIME_ROOM_PLAYER_1_REMATCH) || values.ContainsKey(Constants.REALTIME_ROOM_PLAYER_2_REMATCH))
                {
                    if (FourzyPhotonManager.CheckPlayersRematchReady())
                    {
                        CreateRealtimeGame();
                        GameplayScreen.realtimeScreen.SetMessage(LocalizationManager.Value("waiting_for_game"));
                    }
                }
            }
            else
            {
                if (values.ContainsKey(Constants.REALTIME_ROOM_PLAYER_1_READY) || values.ContainsKey(Constants.REALTIME_ROOM_PLAYER_2_READY))
                {
                    if (FourzyPhotonManager.CheckPlayersReady())
                    {
                        CloseRealtimeScreen();
                        UpdatePlayerTurn();
                    }
                }
            }

            void CloseRealtimeScreen()
            {
                if (GameplayScreen.realtimeScreen.isOpened)
                {
                    GameplayScreen.realtimeScreen.CloseSelf();
                }
            }
        }

        private void OnEventCall(EventData data)
        {
            //will be called on other client
            switch (data.Code)
            {
                case Constants.GAME_DATA:
                    //always received by opponent
                    ClientFourzyGame _realtimeGame =
                        new ClientFourzyGame(
                            JsonConvert.DeserializeObject<RealtimeGameStateData>(data.CustomData.ToString()));
                    //assign proper user id
                    _realtimeGame.SetPlayer2ID(UserManager.Instance.userId);

                    LoadGame(_realtimeGame);

                    break;

                case Constants.TAKE_TURN:
                    StartRoutine("takeTurn", PlayRealtimeTurn(
                        JsonConvert.DeserializeObject<ClientPlayerTurn>(data.CustomData.ToString())));

                    break;

                case Constants.REMATCH_REQUEST:
                    realtimeWaitingOtherScreen = menuController.GetOrAddScreen<PromptScreen>()
                        .Prompt(string.Format(LocalizationManager.Value("rematch_request"), Game.opponent.DisplayName), LocalizationManager.Value("play"),
                        () =>
                        {
                            ReportRematchResult(true);

                            FourzyPhotonManager.SetClientRematchReady();
                            GameplayScreen.realtimeScreen.SetMessage(LocalizationManager.Value("waiting_for_game"));
                        },
                        () =>
                        {
                            ReportRematchResult(false);

                            BackButtonOnClick();
                        })
                        .CloseOnAccept()
                        .CloseOnDecline();

                    rematchRequested = true;

                    break;

                case Constants.RATING_GAME_OTHER_LOST:
                    Game._State.WinnerId = Game.me.PlayerId;
                    OnGameFinished(Game);

                    break;

                case Constants.PLAYER_FORFEIT:
                    otherPlayerForfeit = true;

                    break;
            }
        }

        private void OnPlayerLeftRoom(Photon.Realtime.Player other)
        {
            if (Game == null)
            {
                BackButtonOnClick();

                return;
            }

            if (otherPlayerForfeit)
            {
                playerLeftScreen = PersistantMenuController.Instance
                    .GetOrAddScreen<PromptScreen>()
                    .Prompt(
                        $"{other.NickName} {LocalizationManager.Value("left_game")}",
                        null,
                        null,
                        LocalizationManager.Value("leave"),
                        null,
                        BackButtonOnClick)
                    .CloseOnDecline();

                OnRealtimeOpponentAbandoned();

                otherPlayerForfeit = false;
            }
            else
            {
                switch (Game._Type)
                {
                    case GameType.REALTIME:
                        if (Game != null)
                        {
                            if (Game.IsOver)
                            {
                                playerLeftScreen = PersistantMenuController.Instance
                                    .GetOrAddScreen<PromptScreen>()
                                    .Prompt(
                                        $"{other.NickName} {LocalizationManager.Value("left_game")}",
                                        null,
                                        null,
                                        LocalizationManager.Value("leave"),
                                        null,
                                        BackButtonOnClick)
                                    .CloseOnDecline();
                            }
                            else
                            {
                                FourzyPhotonManager.SetClientsReadyState(false);

                                RecordLastBoardStateAsRoomProperty(true);
                                PauseGame();
                                StartRoutine("playerLeft", PlayerLeftRealtimeRoutine(other), () =>
                                {
                                    playerLeftScreen.CloseSelf();
                                    playerLeftScreen = null;
                                });
                            }
                        }
                        else
                        {
                            BackButtonOnClick();
                        }

                        break;
                }
            }
        }

        private void OnPlayerEnteredRoom(Photon.Realtime.Player other)
        {
            if (Game == null)
            {
                BackButtonOnClick();

                return;
            }

            switch (Game._Type)
            {
                case GameType.REALTIME:
                    CancelRoutine("playerLeft");

                    waitingForPlayerRejoinReady = true;
                    FourzyPhotonManager.SetClientReadyState(true);
                    GameplayScreen.realtimeScreen.SetMessage(LocalizationManager.Value("waiting_for_player"));

                    break;
            }
        }
#endif


        internal void OnGameFinished(IClientFourzy game)
        {
            onGameFinished?.Invoke(game);
            bool winner = game.IsWinner();

#region Skillz Game Check

            if (game._Type == GameType.SKILLZ_ASYNC)
            {
                int myMovesLeft = game.myMembers.Count;
                int timerLeft = (int)GameplayScreen.MyTimerLeft;
                List<PointsEntry> points = new List<PointsEntry>();
                if (winner)
                {
                    points.Add(new PointsEntry(LocalizationManager.Value("skillz_win_points_key"), SkillzGameController.Instance.WinPoints));

                    if (myMovesLeft > 0)
                    {
                        points.Add(new PointsEntry(
                            $"{LocalizationManager.Value("skillz_moves_left_points_key")}", 
                            myMovesLeft * SkillzGameController.Instance.PointsPerMoveLeftWin));
                    }
                }
                else if (game.draw)
                {
                    if (myMovesLeft > 0)
                    {
                        points.Add(new PointsEntry(
                            $"{LocalizationManager.Value("skillz_moves_left_points_key")}",
                            myMovesLeft * SkillzGameController.Instance.PointsPerMoveLeftDraw));
                    }
                }
                else
                {
                    points.Add(new PointsEntry(
                        $"{LocalizationManager.Value("skillz_survival_moves_left_key")}",
                        myMovesLeft * SkillzGameController.Instance.PointsPerMoveLeftLose));
                }

                if (SkillzGameController.Instance.GamesPlayed.Count == SkillzGameController.Instance.GamesToPlay - 1)
                {
                    //add timer bonus
                    if (timerLeft > 0)
                    {
                        points.Add(new PointsEntry(
                            $"{LocalizationManager.Value("skillz_time_left_points_key")}",
                            timerLeft * SkillzGameController.Instance.PointsPerSecond));
                    }

                    // only if this match is more than 1 games
                    if (SkillzGameController.Instance.GamesToPlay > 1)
                    {
                        //big win bonus
                        if (SkillzGameController.Instance.GamesPlayed.TrueForAll(_game => _game.state) && winner)
                        {
                            points.Add(new PointsEntry(LocalizationManager.Value("skillz_big_win_key"), 2000));
                        }
                    }
                }

                SkillzGameController.Instance.FinishGame(winner, points.ToArray());

                //force end game due to empty timer
                if (GameplayScreen.skillzGameScreen.Timer == 0)
                {
                    while (SkillzGameController.Instance.HaveNextGame)
                    {
                        SkillzGameController.Instance.FinishGame(false);
                    }
                }

                //report score
                if (!SkillzGameController.Instance.HaveNextGame)
                {
                    SkillzGameController.Instance.OnMatchFinished();
                    TrySubmitScore();
                }
            }

#endregion

            GameplayScreen.OnGameFinished();

#if !MOBILE_SKILLZ

#region Amplitude user properties update

            switch (GameManager.Instance.ExpectedGameType)
            {
                case GameTypeLocal.REALTIME_QUICKMATCH:
                case GameTypeLocal.REALTIME_BOT_GAME:
                    if (game.draw)
                    {
                        PlayerPrefsWrapper.AddRealtimeGamesDraw();

                        AnalyticsManager.Instance.AmplitudeSetUserProperty("totalRealtimeGamesDraw", PlayerPrefsWrapper.GetRealtimeGamesDraw());
                    }
                    else
                    {
                        if (game.IsWinner())
                        {
                            PlayerPrefsWrapper.AddRealtimeGamesWon();

                            AnalyticsManager.Instance.AmplitudeSetUserProperty("totalRealtimeGamesWon", PlayerPrefsWrapper.GetRealtimeGamesWon());
                        }
                        else
                        {
                            PlayerPrefsWrapper.AddRealtimeGamesLost();

                            AnalyticsManager.Instance.AmplitudeSetUserProperty("totalRealtimeGamesLost", PlayerPrefsWrapper.GetRealtimeGamesLost());
                        }
                    }

                    GameManager.Instance.ReportAreaProgression((Area)PlayerPrefsWrapper.GetCurrentArea());

                    break;
                case GameTypeLocal.REALTIME_LOBBY_GAME:
                    if (game.draw)
                    {
                        PlayerPrefsWrapper.AddPrivateGamesDraw();

                        AnalyticsManager.Instance.AmplitudeSetUserProperty("totalPrivateGamesDraw", PlayerPrefsWrapper.GetPrivateGamesDraw());
                    }
                    else
                    {
                        if (game.IsWinner())
                        {
                            PlayerPrefsWrapper.AddPrivateGamesWon();

                            AnalyticsManager.Instance.AmplitudeSetUserProperty("totalPrivateGamesWon", PlayerPrefsWrapper.GetPrivateGamesWon());
                        }
                        else
                        {
                            PlayerPrefsWrapper.AddPrivateGamesLost();

                            AnalyticsManager.Instance.AmplitudeSetUserProperty("totalPrivateGamesLost", PlayerPrefsWrapper.GetPrivateGamesLost());
                        }
                    }
                    break;
            }

            if (GameManager.Instance.ExpectedGameType == GameTypeLocal.LOCAL_GAME)
            {
                switch (game._Type)
                {
                    case GameType.TRY_TOKEN:
                    case GameType.ONBOARDING:
                    case GameType.PRESENTATION:

                        break;

                    default:
                        LogGameComplete(false);

                        break;
                }

                switch (game._Mode)
                {
                    case GameMode.PUZZLE_PACK:
                    case GameMode.AI_PACK:
                    case GameMode.BOSS_AI_PACK:
                        if (winner)
                        {
                            AnalyticsManager.Instance.AmplitudeSetUserProperty("totalAdventurePuzzlesCompleted", GameManager.Instance.currentMap.totalGamesComplete);
                        }
                        else
                        {
                            PlayerPrefsWrapper.AddAdventurePuzzlesFailedTimes();

                            AnalyticsManager.Instance.AmplitudeSetUserProperty("totalAdventurePuzzleFailures", PlayerPrefsWrapper.GetAdventurePuzzleFailedTimes());
                        }

                        break;
                }
            }

#endregion

#region Realtime game complete (both vs and bot)

            if (game._Type == GameType.REALTIME)
            {
                switch (GameManager.Instance.ExpectedGameType)
                {
                    case GameTypeLocal.REALTIME_LOBBY_GAME:
                    case GameTypeLocal.REALTIME_QUICKMATCH:
                        if (PhotonNetwork.IsMasterClient)
                        {
                            if (winner)
                            {
                                GameManager.Instance.ReportRealtimeGameFinished(
                                    game,
                                    LoginManager.playfabId,
                                    GameManager.Instance.RealtimeOpponent.Id,
                                    false);
                            }
                            else
                            {
                                GameManager.Instance.ReportRealtimeGameFinished(
                                    game,
                                    GameManager.Instance.RealtimeOpponent.Id,
                                    LoginManager.playfabId,
                                    false);
                            }

                            //reset game ttl
                            PhotonNetwork.CurrentRoom.EmptyRoomTtl = 0;
                            PhotonNetwork.CurrentRoom.PlayerTtl = 0;
                        }
                        else
                        {
                            //for other player update value manually 
                            UserManager.Instance.realtimeGamesComplete += 1;
                        }
                        rejoinGame = false;
                        PlayerPrefsWrapper.SetAbandonedRealtimeRoomName("");

                        break;

                    case GameTypeLocal.REALTIME_BOT_GAME:
                        switch (GameManager.Instance.botGameType)
                        {
                            case GameManager.BotGameType.FTUE_RATED:
                            case GameManager.BotGameType.REGULAR:
                                GameManager.Instance.ReportBotGameFinished(game, true, false);

                                break;

                            case GameManager.BotGameType.FTUE_NOT_RATED:
                                GameManager.Instance.ReportBotGameFinished(game, false, false);

                                break;
                        }

                        break;
                }

                ratingUpdated = true;
            }

#endregion

#endif

#region Rewards

            switch (game._Type)
            {
                case GameType.TURN_BASED:
                case GameType.REALTIME:
                case GameType.PASSANDPLAY:
                    PersistantMenuController.Instance.GetOrAddScreen<RewardsScreen>().SetData(game);

                    break;
            }

#endregion

            //reset controller filter
            switch (game._Type)
            {
                case GameType.PASSANDPLAY:
                    //change gamepad mode
                    StandaloneInputModuleExtended.GamepadFilter =
                        StandaloneInputModuleExtended.GamepadControlFilter.ANY_GAMEPAD;

                    break;
            }
            //

            StartRoutine("postGameRoutine", PostGameFinished());
        }

        private void TrySubmitScore()
        {
            SkillzCrossPlatform.SubmitScore(SkillzGameController.Instance.Points, OnSkillzScoreReported, OnSkillzScoreReportedError);
        }

        private void OnSkillzScoreReported()
        {
            Debug.Log("Skillz Score submited. All good.");
        }

        private void OnSkillzScoreReportedError(string error)
        {
            Debug.Log($"Failed to report Skillz score: {error}");

            StartCoroutine(SkillzScoreReportHelper());
        }

        /// <summary>
        /// When failed to report score
        /// </summary>
        /// <returns></returns>
        private IEnumerator SkillzScoreReportHelper()
        {
            if (SkillzGameController.Instance.SubmitRetries > 0)
            {
                //wait for 3 seconds for the next try
                yield return new WaitForSeconds(3f);

                SkillzGameController.Instance.SubmitRetries--;
                TrySubmitScore();
            }
            else
            {
                Debug.Log("Failed to report score");
                Debug.Log("Last effort, using DisplayTournamentResultsWithScore");

                if (!SkillzGameController.Instance.ReturnToSkillzCalled)
                {
                    SkillzGameController.Instance.ReturnToSkillzCalled = true;
                    SkillzCrossPlatform.DisplayTournamentResultsWithScore(SkillzGameController.Instance.Points);
                }
            }
        }

        private void OnDraw(IClientFourzy game)
        {
            //channel into gamefinished pipeline
            OnGameFinished(game);
        }

        private void OnMoveStarted(ClientPlayerTurn turn, bool startTurn)
        {
            onMoveStarted?.Invoke(turn, startTurn);
        }

        private void OnMoveEnded(ClientPlayerTurn turn, PlayerTurnResult turnResult, bool startTurn)
        {
            onMoveEnded?.Invoke(turn, turnResult, startTurn);

            //game lost due to "no_pieces" needs to be triggered manually, as it is not by model code
            if (turn != null && turn.PlayerId == Game.me.PlayerId && (Game._State.Options.MovesReduceHerd && Game.myMembers.Count == 0) && Game._State.WinningLocations == null)
            {
                switch (Game._Mode)
                {
                    case GameMode.GAUNTLET:
                        //SetHintAreaColliderState(false);
                        OnGameFinished(Game);

                        break;

                    case GameMode.VERSUS:
                        switch (Game._Type)
                        {
                            case GameType.SKILLZ_ASYNC:
                                OnGameFinished(Game);

                                break;
                        }

                        break;
                }
            }

            if (startTurn) return;

            UpdatePlayerTurn();

            ////pause opponents timer
            //if (turn.PlayerId != Game._State.ActivePlayerId)
            //{
            //    GameplayScreen.timerWidgets[1].Pause(realtimeTimerDelay);
            //}
        }

        private void OnNetwork(bool state)
        {
            if (Game == null) return;

            if (!state)
            {
                if (Game._Type == GameType.REALTIME)
                {
                    PauseGame();

                    //open prompt screen
                    PersistantMenuController.Instance.GetOrAddScreen<PromptScreen>().Prompt(
                        LocalizationManager.Value("no_connection"),
                        "", null,
                        LocalizationManager.Value("back"), null, () =>
                        {
                            PersistantMenuController.Instance.CloseCurrentScreen();
                            BackButtonOnClick();
                        });
                }

                switch (Game._Type)
                {
                    case GameType.FRIEND:
                    case GameType.LEADERBOARD:
                    case GameType.TURN_BASED:
                        noNetworkOverlay.SetActive(true);

                        break;
                }
            }
        }

        private void OnRatingDataAquired(RatingGameCompleteResult data)
        {
            foreach (RatingGamePlayer player in data.players)
            {
                if (player.playfabID == LoginManager.playfabId)
                {
                    string ratingText = "Rating ";
                    if (player.winner)
                    {
                        ratingText += "+";
                    }

                    int diff =
                        InternalSettings.Current.GAMES_BEFORE_RATING_USED -
                        UserManager.Instance.totalRatedGames;
                    string message;
                    if (diff > 0)
                    {
                        message = $"Rating revealed in {diff} more game{(diff == 1 ? "" : "s")}.";
                    }
                    else
                    {
                        message = ratingText + player.ratingChange;
                    }

                    if (playerLeftScreen)
                    {
                        playerLeftScreen.promptText.text += $"\n{message}";
                        playerLeftScreen = null;
                    }
                    else if (GameWinLoseScreen.isOpened)
                    {
                        if (player.ratingChange != 0)
                        {
                            GameWinLoseScreen.SetInfoLabel(message);
                        }
                    }
                }
            }
        }

        private void LogGameStart()
        {
            //skip for next game types
            switch (Game._Type)
            {
                case GameType.ONBOARDING:
                case GameType.TRY_TOKEN:
                case GameType.PRESENTATION:
                case GameType.SKILLZ_ASYNC:

                    return;
            }

            if (PrevGame == null || (PrevGame.BoardID != Game.BoardID))
            {
                AnalyticsManager.Instance.LogGame(Game.GameToAnalyticsEvent(true), Game);
            }
        }

        private void LogGameComplete(bool abandoned)
        {
            if (Game._Type == GameType.ONBOARDING) return;

            AnalyticsManager.AnalyticsEvents _event = Game.GameToAnalyticsEvent(false);
            AnalyticsManager.GameResultType gameResult = AnalyticsManager.GameResultType.none;
            Dictionary<string, object> extraParams = new Dictionary<string, object>();

            bool isPlayer1 = Game.me == Game.player1;

            if (GameplayScreen.timersEnabled)
            {
                float player1TimeLeft = isPlayer1 ? GameplayScreen.MyTimerLeft : GameplayScreen.OpponentTimerLeft;
                float player2TimeLeft = isPlayer1 ? GameplayScreen.OpponentTimerLeft : GameplayScreen.MyTimerLeft;

                extraParams.Add("player1TimeRemaining", player1TimeLeft);
                extraParams.Add("player2TimeRemaining", player2TimeLeft);

                if (!abandoned)
                {
                    if (player1TimeLeft <= 0f)
                    {
                        gameResult = AnalyticsManager.GameResultType.player1TimeExpired;
                    }
                    else if (player2TimeLeft <= 0f)
                    {
                        gameResult = AnalyticsManager.GameResultType.player2TimeExpired;
                    }
                }
            }

            if (abandoned)
            {
                gameResult = AnalyticsManager.GameResultType.abandoned;
            }
            else
            {
                if (gameResult == AnalyticsManager.GameResultType.none)
                {
                    if (!Game.turnEvaluator.IsAvailableSimpleMove())
                    {
                        gameResult = AnalyticsManager.GameResultType.noPossibleMoves;
                    }
                    else if (Game.draw)
                    {
                        gameResult = AnalyticsManager.GameResultType.draw;
                    }
                    else
                    {
                        if (Game._Mode == GameMode.VERSUS)
                        {
                            gameResult = Game.IsWinner(Game.player1) ?
                                AnalyticsManager.GameResultType.player1Win :
                                AnalyticsManager.GameResultType.player2Win;
                        }
                        else
                        {
                            gameResult = Game.IsWinner() ?
                                AnalyticsManager.GameResultType.win :
                                AnalyticsManager.GameResultType.lose;
                        }
                    }
                }
            }

            extraParams.Add(AnalyticsManager.GAME_RESULT_KEY, gameResult.ToString());

            switch (_event)
            {
                case AnalyticsManager.AnalyticsEvents.aiLevelEnd:
                case AnalyticsManager.AnalyticsEvents.bossAILevelEnd:
                case AnalyticsManager.AnalyticsEvents.puzzleLevelEnd:
                    extraParams.Add("isAlreadySolved", isSolved);

                    break;
            }

            if (gameResult != AnalyticsManager.GameResultType.none)
            {
                AnalyticsManager.Instance.LogGame(_event, Game, extraParams);
            }
        }

        private void OnLogin(bool state)
        {
            if (state)
            {
                noNetworkOverlay.SetActive(false);
            }
        }

        private void OnHintUpdate(int amount, string token)
        {
            if (token == hintRemovedToken)
            {
                StartRoutine("hintRoutine", PlayHintRoutine());
            }
        }

        private void OnNoInput(KeyValuePair<string, float> noInputFilter)
        {
            if (noInputFilter.Key != "highlightMoves" || !GameplayScreen.isCurrent) return;

            //if its NOT my turn, exit
            switch (Game._Type)
            {
                case GameType.PASSANDPLAY:

                    break;

                default:
                    if (!Game.isMyTurn) return;

                    break;
            }

            switch (BoardView.actionState)
            {
                case GameboardView.BoardActionState.MOVE:
                    switch (GameManager.Instance.placementStyle)
                    {
                        case GameManager.PlacementStyle.EDGE_TAP:
                            BoardView.ShowHintArea(
                                GameboardView.HintAreaStyle.ANIMATION_LOOP,
                                GameboardView.HintAreaAnimationPattern.NONE);
                            BoardView.SetHintAreaSelectableState(false);

                            break;
                    }

                    break;
            }
        }

        private void OnGamePieceSmashed(GamePieceView gamepiece)
        {
            gameCameraManager.Wiggle();
        }

#if !MOBILE_SKILLZ
        private void OnRealtimeOpponentAbandoned()
        {
            //reset game ttl
            PhotonNetwork.CurrentRoom.EmptyRoomTtl = 0;
            PhotonNetwork.CurrentRoom.PlayerTtl = 0;

            //report score
            if (!ratingUpdated)
            {
                GameManager.Instance.ReportRealtimeGameFinished(
                    Game,
                    LoginManager.playfabId,
                    GameManager.Instance.RealtimeOpponent.Id,
                    true);
            }

            PlayerPrefsWrapper.AddRealtimeGamesOpponentAbandoned();
            AnalyticsManager.Instance.AmplitudeSetUserProperty("totalRealtimeGamesOpponentAbandoned", PlayerPrefsWrapper.GetRealtimeGamesOpponentAbandoned());
        }
#endif

        private IEnumerator GameInitRoutine()
        {
            if (Game == null) yield break;

            yield return StartCoroutine(FadeGameScreen(1f, .5f));

            MenuScreen _screen = menuController.GetScreen<PrePackPrompt>();
            if (!_screen)
            {
                _screen = menuController.GetScreen<VSGamePrompt>();
            }

            while (_screen && _screen.isOpened)
            {
                yield return null;
            }

            //instruction boards
            switch (Game._Type)
            {
                // Do not show token instructions popup for these types
                case GameType.REALTIME:
                case GameType.ONBOARDING:
                case GameType.PRESENTATION:
                    break;

                default:
                    if (SettingsManager.Get(SettingsManager.KEY_TOKEN_INSTRUCTION))
                    {
                        yield return StartCoroutine(ShowTokenInstructionPopupRoutine());
                    }

                    break;
            }

            //first update player turn
            switch (Game._Type)
            {
                case GameType.REALTIME:
                    if (!rejoinGame)
                    {
                        StartRoutine("realtimeCountdownRoutine", StartRealtimeCountdown());
                    }

                    break;

                default:
                    UpdatePlayerTurn();

                    break;
            }

            IsBoardReady = true;
        }

        private IEnumerator PlayHintRoutine()
        {
            int lastHintIndex = PlayerPrefsWrapper.GetPuzzleHintProgress(Game.BoardID);

            bool resetBoard = Game._playerTurnRecord.Count != lastHintIndex;

            if (!resetBoard)
            {
                for (int turnIndex = 0; turnIndex < Game._playerTurnRecord.Count; turnIndex++)
                {
                    if (Game._playerTurnRecord[turnIndex].Notation != Game.puzzleData.Solution[turnIndex].Notation)
                    {
                        resetBoard = true;
                        break;
                    }
                }
            }

            if (resetBoard)
            {
                Game._Reset();

                for (int turnIndex = 0; turnIndex < lastHintIndex; turnIndex++)
                {
                    Game.TakeTurn(Game.puzzleData.Solution[turnIndex], false);
                    Game.TakeAITurn(false);
                }

                LoadGame(Game);
            }

            while (IsRoutineActive("gameInit")) yield return null;

            //play hint
            //board.TakeTurn(game.puzzleData.Solution[lastHintIndex]);

            //display hint
            DisplayHint(Game.puzzleData.Solution[lastHintIndex]);

            PlayerPrefsWrapper.SetPuzzleHintProgress(Game.BoardID, lastHintIndex + 1 == Game.puzzleData.Solution.Count ? 0 : lastHintIndex + 1);
        }

        private IEnumerator StartRealtimeCountdown()
        {
            //start timer
            yield return GameplayScreen.realtimeScreen.StartCountdown(InternalSettings.Current.REALTIME_COUNTDOWN_SECONDS);

            UpdatePlayerTurn();

            ////adjust opponent timer value
            //if (!PhotonNetwork.IsMasterClient)
            //{
            //    GameplayScreen.timerWidgets[1].SmallTimerValue -= realtimeTimerDelay;
            //}
        }

        //private IEnumerator PlayTurnBaseTurn(ChallengeData gameData)
        //{
        //    if (gameData.lastTurnGame._Type != GameType.TURN_BASED) yield break;

        //    yield return new WaitUntil(() => !board.isAnimating && isBoardReady);

        //    PlayerTurn lastTurn = gameData.lastTurn;

        //    if (game.asFourzyGame.playerTurnRecord.Count > 0 &&
        //        game.asFourzyGame.playerTurnRecord[game.asFourzyGame.playerTurnRecord.Count - 1].PlayerId == lastTurn.PlayerId) yield break;

        //    //compare challenges
        //    if (gameData.challengeInstanceId != game.asFourzyGame.challengeData.challengeInstanceId) yield break;

        //    board.TakeTurn(lastTurn);
        //    UpdatePlayerTurn();
        //}

        private IEnumerator PlayRealtimeTurn(ClientPlayerTurn turn)
        {
            if (Game._Type != GameType.REALTIME) yield break;

            yield return new WaitUntil(() => !BoardView.isAnimating && IsBoardReady);

            GameplayScreen.OnRealtimeTurnRecieved(turn);
            //since spell are not created as a result of TakeTurn, need to create them manually
            turn.Moves.ForEach(imove =>
            {
                if (imove.MoveType == MoveType.SPELL)
                {
                    ISpell spell = imove as ISpell;

                    switch (spell.SpellId)
                    {
                        case SpellId.HEX:
                            BoardView.CastSpell((spell as HexSpell).Location, spell.SpellId);

                            break;

                        case SpellId.PLACE_LURE:
                            BoardView.CastSpell((spell as LureSpell).Location, spell.SpellId);

                            break;

                        case SpellId.DARKNESS:
                            BoardView.CastSpell((spell as DarknessSpell).Location, spell.SpellId);

                            break;

                    }
                }
            });

            yield return BoardView.TakeTurn(turn);
        }

        private IEnumerator PostGameFinished()
        {
            yield return new WaitForSeconds(.5f);

            //visuals + haptic
            switch (Game._Type)
            {
                case GameType.ONBOARDING:
                    break;

                case GameType.FRIEND:
                case GameType.LEADERBOARD:
                case GameType.PUZZLE:
                case GameType.AI:
                case GameType.TURN_BASED:
                case GameType.REALTIME:
                case GameType.TRY_TOKEN:
                case GameType.SKILLZ_ASYNC:
                    if (Game.IsWinner())
                    {
                        winningParticleGenerator.ShowParticles();

                        GameManager.Vibrate();
                    }

                    break;

                default:
                    if (!Game.draw)
                    {
                        winningParticleGenerator.ShowParticles();
                    }

                    GameManager.Vibrate();

                    break;
            }

            //sound
            switch (Game._Type)
            {
                case GameType.ONBOARDING:
                    break;

                case GameType.PUZZLE:
                case GameType.REALTIME:
                    if (Game.IsWinner())
                    {
                        AudioHolder.instance.PlaySelfSfxOneShotTracked("game_won");
                    }
                    else
                    {
                        AudioHolder.instance.PlaySelfSfxOneShotTracked("game_lost");
                    }

                    break;

                case GameType.FRIEND:
                case GameType.LEADERBOARD:
                case GameType.AI:
                case GameType.TRY_TOKEN:
                case GameType.TURN_BASED:
                    if (Game.IsWinner())
                    {
                        AudioHolder.instance.PlaySelfSfxOneShotTracked("game_turnbased_won");
                    }
                    else
                    {
                        AudioHolder.instance.PlaySelfSfxOneShotTracked("game_turnbased_lost");
                    }

                    break;

                default:
                    AudioHolder.instance.PlaySelfSfxOneShotTracked("game_won");

                    break;
            }

            //if demo, restart
            switch (Game._Type)
            {
                case GameType.PRESENTATION:
                    yield return new WaitForSeconds(3f);

                    ClientFourzyGame _game = new ClientFourzyGame(GameContentManager.Instance.areasDataHolder.areas.Random().areaID,
                        new Player(1, "AI Player 1") { PlayerString = "1" },
                        new Player(2, "AI Player 2") { PlayerString = "2" }, 1)
                    {
                        _Type = GameType.PRESENTATION,
                    };
                    _game.UpdateFirstState();

                    LoadGame(_game);

                    break;
            }
        }

#if !MOBILE_SKILLZ
        private IEnumerator PlayerLeftRealtimeRoutine(Photon.Realtime.Player otherPlayer)
        {
            int waitTime = Constants.REALTIME_TTL_SECONDS;

            playerLeftScreen = PersistantMenuController.Instance
                .GetOrAddScreen<PromptScreen>()
                .Prompt(
                    $"{otherPlayer.NickName} {LocalizationManager.Value("left_game")}",
                    waitTime.ToString("mm':'ss") + "\n" + LocalizationManager.Value("realtime_player_left_message"),
                    null,
                    LocalizationManager.Value("leave"),
                    null,
                    BackButtonOnClick)
                .CloseOnDecline();

            int timer = waitTime;
            while ((timer -= 1) > 0f)
            {
                TimeSpan time = TimeSpan.FromSeconds(timer);
                playerLeftScreen.promptText.text = time.ToString("mm':'ss");

                yield return new WaitForSeconds(1f);
            }

            OnRealtimeOpponentAbandoned();

            //and close gameplay scene
            BackButtonOnClick();
        }
#endif
    }

    public enum GameState
    {
        OPENNING_GAME,
        PREGAME_DISPLAY_INSTRUCTION,
        GAME,
        PAUSED,
        HELP_STATE,
    }

    [Serializable]
    public class RatingGameCompleteResult
    {
        public RatingGamePlayer[] players;
        public bool draw;

        public RatingGameCompleteResult()
        {
        }
    }

    [Serializable]
    public class RatingGamePlayer
    {
        public string playfabID;
        public int rating;
        public int ratingChange;
        public bool winner;

        public RatingGamePlayer()
        {
        }
    }
}

