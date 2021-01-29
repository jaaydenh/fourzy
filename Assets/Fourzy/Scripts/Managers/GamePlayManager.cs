//modded @vadym udod

using ExitGames.Client.Photon;
using Fourzy._Updates.Audio;
using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Managers;
using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tools;
using Fourzy._Updates.UI.Menu;
using Fourzy._Updates.UI.Menu.Screens;
using Fourzy._Updates.UI.Toasts;
using FourzyGameModel.Model;
using Newtonsoft.Json;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static Fourzy._Updates.Serialized.ThemesDataHolder;

namespace Fourzy._Updates.Mechanics.GameplayScene
{
    public class GamePlayManager : RoutinesBase
    {
        public static GamePlayManager Instance;

        public static Action<ClientPlayerTurn> onMoveStarted;
        public static Action<ClientPlayerTurn> onMoveEnded;
        public static Action<IClientFourzy> onGameFinished;

        public static Action<string> OnGamePlayMessage;
        public static Action<long> OnTimerUpdate;

        public TouchZone touchZone;
        public FourzyGameMenuController menuController;
        public WinningParticleGenerator winningParticleGenerator;
        public Transform bgParent;
        public GameObject noNetworkOverlay;
        public RectTransform hintBlocksParent;

        private AudioHolder.BGAudio gameplayBGAudio;
        private PromptScreen waitingScreen;

        //id of rematch challenge
        private string awaitingChallengeID = "";
        private long epochDelta;
        private bool logGameFinished;
        private bool ratingUpdated = true;
        private float startedAt = 0f;
        private GameState previousGameState;
        private int gauntletRechargedViaGems = 0;
        private int gauntletRechargedViaAds = 0;

        public BackgroundConfigurationData currentConfiguration { get; private set; }
        public GameboardView board { get; private set; }
        public GameplayBG bg { get; private set; }
        public GameplayScreen gameplayScreen { get; private set; }
        public RandomPlayerPickScreen playerPickScreen { get; private set; }
        public GameWinLoseScreen gameWinLoseScreen { get; private set; }
        public LoadingPromptScreen loadingPrompt { get; private set; }

        public IClientFourzy game { get; private set; }

        public bool isBoardReady { get; private set; }
        public bool replayingLastTurn { get; private set; }
        public bool gameStarted { get; private set; }
        public GameState gameState { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            Instance = this;
        }

        protected void Start()
        {
            gameplayScreen = menuController.GetOrAddScreen<GameplayScreen>();
            playerPickScreen = menuController.GetOrAddScreen<RandomPlayerPickScreen>();
            gameWinLoseScreen = menuController.GetOrAddScreen<GameWinLoseScreen>();

            touchZone.onPointerDownData += OnPointerDown;
            touchZone.onPointerUpData += OnPointerRelease;

            GameManager.onNetworkAccess += OnNetwork;
            LoginManager.OnDeviceLoginComplete += OnLogin;
            // ChallengeManager.OnChallengeUpdate += OnChallengeUpdate;
            // ChallengeManager.OnChallengesUpdate += OnChallengesUpdate;

            //listen to roomPropertyChanged event
            FourzyPhotonManager.onRoomPropertiesUpdate += OnRoomPropertiesUpdate;
            FourzyPhotonManager.onPlayerLeftRoom += OnPlayerLeftRoom;
            FourzyPhotonManager.onEvent += OnEventCall;

            if (SettingsManager.Get(SettingsManager.KEY_DEMO_MODE))
            {
                PointerInputModuleExtended.noInput += OnNoInput;
            }

            HeaderScreen.instance.Close();

            CheckGameMode();
        }

        protected void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.H))
            {
            }
#endif
        }

        protected void OnDestroy()
        {
            if (board)
            {
                board.onGameFinished -= OnGameFinished;
                board.onDraw -= OnDraw;
                board.onMoveStarted -= OnMoveStarted;
                board.onMoveEnded -= OnMoveEnded;
            }

            GameManager.onNetworkAccess -= OnNetwork;
            LoginManager.OnDeviceLoginComplete -= OnLogin;
            // ChallengeManager.OnChallengeUpdate -= OnChallengeUpdate;
            // ChallengeManager.OnChallengesUpdate -= OnChallengesUpdate;

            FourzyPhotonManager.onRoomPropertiesUpdate -= OnRoomPropertiesUpdate;
            FourzyPhotonManager.onPlayerLeftRoom -= OnPlayerLeftRoom;
            FourzyPhotonManager.onEvent -= OnEventCall;

            if (SettingsManager.Get(SettingsManager.KEY_DEMO_MODE)) PointerInputModuleExtended.noInput -= OnNoInput;

            AudioHolder.instance.StopBGAudio(gameplayBGAudio, .5f);
        }

        protected void OnApplicationQuit()
        {
            //analytics
            if (game != null && !game.isOver)
                AnalyticsManager.Instance.LogGame(
                    game._Mode.GameModeToAnalyticsEvent(false),
                    game,
                    extraParams: new KeyValuePair<string, object>(
                        AnalyticsManager.GAME_RESULT_KEY,
                        AnalyticsManager.GameResultType.Abandoned));
        }

        public void BackButtonOnClick()
        {
            //analytics
            if (game != null && !game.isOver)
            {
                AnalyticsManager.Instance.LogGame(
                    game._Mode.GameModeToAnalyticsEvent(false), 
                    game,
                    extraParams: new KeyValuePair<string, object>(
                        AnalyticsManager.GAME_RESULT_KEY, 
                        AnalyticsManager.GameResultType.Abandoned));
            }

            GameManager.Instance.OpenMainMenu();

            switch (GameManager.Instance.ExpectedGameType)
            {
                case GameTypeLocal.REALTIME_LOBBY_GAME:
                case GameTypeLocal.REALTIME_QUICKMATCH:
                    FourzyPhotonManager.TryLeaveRoom();
                    FourzyPhotonManager.Instance.JoinLobby();

                    GameManager.Instance.currentOpponent = "";

                    break;
            }
        }

        protected void SetGameIfNull(IClientFourzy _game)
        {
            //if active game is empty, load random pass&play board
            if (_game == null)
            {
                //ClientFourzyGame newGame = new ClientFourzyGame(GameContentManager.Instance.passAndPlayDataHolder.random, UserManager.Instance.meAsPlayer, new Player(2, "Player Two"));
                //ClientFourzyGame newGame = new ClientFourzyGame(GameContentManager.Instance.GetPassAndPlayBoardByName("Sand Dunes"), UserManager.Instance.meAsPlayer, new Player(2, "Player Two"));
                ClientFourzyGame newGame = new ClientFourzyGame(GameContentManager.Instance.GetMiscBoard("20"), UserManager.Instance.meAsPlayer, new FourzyGameModel.Model.Player(2, "Player Two"));
                newGame._Type = GameType.PASSANDPLAY;

                GameManager.Instance.activeGame = newGame;
            }
            else if (_game != GameManager.Instance.activeGame)
            {
                GameManager.Instance.activeGame = _game;
            }

            game = GameManager.Instance.activeGame;

            Debug.Log($"Type: {game._Type} Mode: {game._Mode} Expected Local Type: {GameManager.Instance.ExpectedGameType}");
        }

        public void LoadGame(IClientFourzy _game)
        {
            CancelRoutine("gameInit");
            CancelRoutine("takeTurn");
            CancelRoutine("realtimeCoutdownRoutine");
            CancelRoutine("postGameRoutine");

            awaitingChallengeID = "";
            gameState = GameState.OPENNING_GAME;
            gameStarted = false;
            isBoardReady = false;
            ratingUpdated = false;
            gauntletRechargedViaAds = 0;

            SetGameIfNull(_game);

            GamepadCheck();
            LogGameCheck();
            PlayBGAudio();
            GameOpenedCheck();

            //close other screens
            menuController.BackToRoot();

            winningParticleGenerator.HideParticles();

            playerPickScreen.SetData(game);

            LoadBG(game._Area);
            LoadBoard();

            gameplayScreen.InitializeUI(this);
            OnNetwork(GameManager.NetworkAccess);

            StartRoutine("gameInit", GameInitRoutine());
        }

        public void CreateRealtimeGame()
        {
            //only continue if master
            if (PhotonNetwork.IsMasterClient)
            {
                //ready opponent
                FourzyGameModel.Model.Player opponen =
                    new FourzyGameModel.Model.Player(2, PhotonNetwork.PlayerListOthers[0].NickName)
                    {
                        HerdId = FourzyPhotonManager.GetOpponentProperty(
                            Constants.REALTIME_GAMEPIECE_KEY, 
                            Constants.REALTIME_DEFAULT_GAMEPIECE_KEY),
                        PlayerString = "2"
                    };

                //load realtime game
                ClientFourzyGame _game = new ClientFourzyGame(
                    GameContentManager.Instance.enabledThemes.Random().themeID,
                    UserManager.Instance.meAsPlayer,
                    opponen,
                    UnityEngine.Random.value > .5f ? 1 : 2)
                { _Type = GameType.REALTIME };

                GameStateDataEpoch gameStateData = _game.toGameStateData;
                //add realtime data
                gameStateData.realtimeData = new RealtimeData() { createdEpoch = Utils.EpochMilliseconds(), };

                var eventOptions = new Photon.Realtime.RaiseEventOptions();
                eventOptions.Flags.HttpForward = true;
                eventOptions.Flags.WebhookFlags = Photon.Realtime.WebFlags.HttpForwardConst;
                var result = PhotonNetwork.RaiseEvent(
                    Constants.GAME_DATA, 
                    JsonConvert.SerializeObject(gameStateData), 
                    eventOptions, 
                    SendOptions.SendReliable);
                Debug.Log("Photon create game event result: " + result);

                LoadGame(_game);
            }
        }

        public void OnPointerDown(Vector2 position, int pointerId)
        {
            if (CustomInputManager.GamepadCount == 2 && pointerId > -1 && game._State.ActivePlayerId != (pointerId + 1))
            {
                gameplayScreen.OnWrongTurn();
                return;
            }

            if (gameplayScreen != menuController.currentScreen) return;

            board.OnPointerDown(position);
        }

        public void OnPointerMove(Vector2 position)
        {
            if (gameplayScreen != menuController.currentScreen || gameState != GameState.GAME)
            {
                OnPointerRelease(position, -1);

                return;
            }

            board.OnPointerMove(position);
        }

        public void OnPointerRelease(Vector2 position, int pointerId)
        {
            if (CustomInputManager.GamepadCount == 2 && 
                pointerId > -1 && 
                game._State.ActivePlayerId != (pointerId + 1))
            {
                return;
            }
            if (gameState != GameState.GAME)
            {
                return;
            }

            board.OnPointerRelease(position);
        }

        public void UpdatePlayerTurn()
        {
            if (game == null) return;

            if (!gameStarted)
            {
                gameState = GameState.GAME;
                gameStarted = true;
                OnGameStarted();
            }

            switch (game._Type)
            {
                case GameType.AI:
                    if (!game.isOver && !game.isMyTurn) board.TakeAITurn();

                    break;

                case GameType.PUZZLE:
                    if (!game.isOver && !game.isMyTurn) board.TakeAITurn();

                    break;

                case GameType.PRESENTATION:
                    if (!game.isOver) board.TakeAITurn();

                    break;

                case GameType.PASSANDPLAY:
                    StandaloneInputModuleExtended.GamepadID = game._State.ActivePlayerId == 1 ? 0 : 1;

                    break;
            }
        }

        /// <summary>
        /// Triggered after board/tokens fade id + optional delays
        /// </summary>
        private void OnGameStarted()
        {
            gameplayScreen.OnGameStarted();

            startedAt = Time.time;
            board.interactable = true;
            board.OnPlayManagerReady();
            game.SetInitialTime(Time.time);

            switch (game._Type)
            {
                case GameType.REALTIME:
                    FourzyPhotonManager.ResetRematchState();

                    break;
            }
        }

        private IEnumerator ShowTokenInstructionPopupRoutine()
        {
            if (game == null || game._Type == GameType.ONBOARDING) yield break;

            HashSet<TokensDataHolder.TokenData> tokens = new HashSet<TokensDataHolder.TokenData>();

            foreach (BoardSpace boardSpace in game.boardContent)
            {
                foreach (IToken token in boardSpace.Tokens.Values)
                {
                    if (!PlayerPrefsWrapper.InstructionPopupWasDisplayed((int)token.Type) &&
                        !GameManager.Instance.excludeInstructionsFor.Contains(token.Type))
                    {
                        tokens.Add(GameContentManager.Instance.GetTokenData(token.Type));
                    }
                }
            }

            if (tokens.Count > 0)
            {
                gameState = GameState.PREGAME_DISPLAY_INSTRUCTION;
            }

            foreach (TokensDataHolder.TokenData token in tokens)
            {
                TokenPrompt popupUI = menuController.GetOrAddScreen<TokenPrompt>(true);
                popupUI.Prompt(token);

                yield return new WaitWhile(() => popupUI.isOpened);

                PlayerPrefsWrapper.SetInstructionPopupDisplayed((int)token.tokenType, true);

                //yield return new WaitForSeconds(.5f);
            }
        }

        private IEnumerator FadeGameScreen(float alpha, float fadeTime)
        {
            board.Fade(alpha, fadeTime);
            yield return new WaitForSeconds(fadeTime - .2f);

            FadeTokens(alpha, fadeTime);
            yield return new WaitForSeconds(fadeTime - .2f);

            FadePieces(alpha, fadeTime);
            yield return new WaitForSeconds(fadeTime - .2f);
        }

        private void FadeTokens(float alpha, float fadeTime)
        {
            if (board.tokens.Count > 0)
            {
                board.FadeTokens(alpha, fadeTime);
            }
        }

        private void FadePieces(float alpha, float fadeTime)
        {
            if (board.gamePieces.Count > 0)
            {
                board.FadeGamepieces(alpha, fadeTime);
            }
        }

        public void UnloadGamePlaySceene()
        {
            SceneManager.UnloadSceneAsync(Constants.GAMEPLAY_SCENE_NAME);
        }

        public void CheckGameMode()
        {
            //auto load game if its not realtime mdoe
            switch (GameManager.Instance.ExpectedGameType)
            {
                case GameTypeLocal.REALTIME_LOBBY_GAME:
                case GameTypeLocal.REALTIME_QUICKMATCH:
                    UnloadBoard();
                    UnloadBG();

                    //notify that this client is ready
                    FourzyPhotonManager.SetClientReady();
                    gameplayScreen.realtimeScreen.CheckWaitingForOtherPlayer("Waiting for other player...");

                    //unload tutorial screen
                    OnboardingScreen.CloseTutorial();

                    break;

                default:
                    LoadGame(GameManager.Instance.activeGame);

                    break;
            }
        }

        public void Rematch(bool resetMembers = false)
        {
            if (game == null) return;

            board.StopAIThread();
            switch (game._Type)
            {
                case GameType.TURN_BASED:
                    loadingPrompt = PersistantMenuController.instance.GetOrAddScreen<LoadingPromptScreen>();
                    loadingPrompt._Prompt(LoadingPromptScreen.LoadingPromptType.BASIC, LocalizationManager.Value("loading"));

                    //loadingPrompt.Prompt("", "Loading new game...", null, null, null, null);

                    // ChallengeManager.Instance.CreateTurnBasedGame(game.opponent.PlayerString, game._Area, CreateTurnBasedGameSuccess, CreateTurnBasedGameError);

                    break;

                case GameType.AI:
                case GameType.PUZZLE:
                    game._Reset(resetMembers);

                    LoadGame(game);

                    break;

                case GameType.PASSANDPLAY:
                    //create new game if random board
                    if (game.asFourzyGame.isBoardRandom)
                    {
                        game = new ClientFourzyGame(
                            game._Area,
                            UserManager.Instance.meAsPlayer, new FourzyGameModel.Model.Player(2, "Player Two"),
                            UserManager.Instance.meAsPlayer.PlayerId)
                        { _Type = GameType.PASSANDPLAY };
                    }
                    else
                    {
                        game._Reset(resetMembers);
                    }

                    LoadGame(game);

                    break;

                case GameType.REALTIME:
                    game._Reset(resetMembers);

                    LoadGame(game);

                    break;
            }
        }

        public void TryContinue()
        {
            if (game == null) return;

            gameplayScreen.OnMoveEnded(null, null);
            UpdatePlayerTurn();
        }

        public void RechargeByAd()
        {
            switch (game._Mode)
            {
                case GameMode.GAUNTLET:
                    gauntletRechargedViaAds++;

                    break;
            }

            Recharge();
        }

        public void RechargeByGem()
        {
            switch (game._Mode)
            {
                case GameMode.GAUNTLET:
                    gauntletRechargedViaGems++;

                    break;
            }

            Recharge();
        }

        public void Recharge()
        {
            game.AddMembers(Constants.GAUNTLET_RECHARGE_AMOUNT);

            TryContinue();
            gameplayScreen.gauntletGameScreen.UpdateCounter();
        }

        public int GetGauntletRechargePrice()
        {
            return (int)Mathf.Pow(Constants.BASE_GAUNTLET_MOVES_COST, gauntletRechargedViaGems);
        }

        public void PlayHint()
        {
            if (game == null || 
                !game.isMyTurn || 
                game.isOver || 
                !game.puzzleData || 
                game.puzzleData.Solution.Count == 0) return;

            AnalyticsManager.Instance.LogGame(
                AnalyticsManager.AnalyticsGameEvents.PUZZLE_LEVEL_HINT_BUTTON_PRESS, 
                game,
                extraParams: new KeyValuePair<string, object>(
                    AnalyticsManager.HINT_STORE_ITEMS_KEY, 
                    StorePromptScreen.ProductsToString(StorePromptScreen.StoreItemType.HINTS)));

            if (UserManager.Instance.hints <= 0)
            {
                PersistantMenuController.instance
                    .GetOrAddScreen<StorePromptScreen>()
                    .Prompt(StorePromptScreen.StoreItemType.HINTS);

                return;
            }

            UserManager.Instance.hints--;

            StartRoutine("hintRoutine", PlayHintRoutine());
        }

        public void PauseGame()
        {
            if (gameState == GameState.PAUSED) return;

            previousGameState = gameState;
            gameState = GameState.PAUSED;
            gameplayScreen.OnGamePaused();
        }

        public void UnpauseGame()
        {
            if (gameState != GameState.PAUSED) return;

            gameState = previousGameState;
            gameplayScreen.OnGameUnpaused();
        }

        public void ToggleHelpState()
        {
            if (gameState != GameState.HELP_STATE)
            {
                previousGameState = gameState;
                gameState = GameState.HELP_STATE;

                board.SetHelpState(true);
            }
            else
            {
                gameState = previousGameState;

                board.SetHelpState(false);
            }

            gameplayScreen.UpdateHelpButton();
        }

        public void DisplayHint(PlayerTurn turn, string hintMessage = "")
        {
            BoardLocation location = turn.GetMove().ToBoardLocation(game);
            BoardLocation next = location.Neighbor(turn.GetMove().Direction, 2);

            _Tutorial.OnboardingTask_PointAt pointAtTask = 
                new _Tutorial.OnboardingTask_PointAt(new Dictionary<GameManager.PlacementStyle, Vector2[]>()
            {
                [GameManager.PlacementStyle.EDGE_TAP] = 
                    new Vector2[] { new Vector2(location.Column, location.Row) },
                [GameManager.PlacementStyle.SWIPE_STYLE_2] = 
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

            _Tutorial.OnboardingTask_ShowMaskedArea maskTask = 
                new _Tutorial.OnboardingTask_ShowMaskedArea(new Dictionary<GameManager.PlacementStyle, Rect>()
            {
                [GameManager.PlacementStyle.EDGE_TAP] = 
                    new Rect(location.Column, location.Row, 1f, 1f),
                [GameManager.PlacementStyle.SWIPE_STYLE_2] = swipeRect
            }, UI.Widgets.OnboardingScreenMaskObject.MaskStyle.PX_0);

            PersistantMenuController.instance
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

            board = Instantiate(currentConfiguration.gameboardPrefab, transform);
            board.Initialize(game);
            board.gameplayManager = this;
            board.transform.localPosition = currentConfiguration.gameboardPrefab.transform.position;
            board.interactable = false;

            board.onGameFinished += OnGameFinished;
            board.onDraw += OnDraw;
            board.onMoveStarted += OnMoveStarted;
            board.onMoveEnded += OnMoveEnded;
            board.onWrongTurn += () => gameplayScreen.OnWrongTurn();

            //hide tokens/gamepieces
            board.FadeTokens(0f, 0f);
            board.FadeGamepieces(0f, 0f);
        }

        private void UnloadBoard()
        {
            if (board) Destroy(board.gameObject);
        }

        private void LoadBG(Area area)
        {
            UnloadBG();

            currentConfiguration = GameContentManager
                .Instance
                .themesDataHolder
                .GetThemeBGConfiguration(area, Camera.main);
            bg = Instantiate(currentConfiguration.backgroundPrefab, bgParent);
            bg.transform.localPosition = Vector3.zero;
        }

        private void UnloadBG()
        {
            if (bg)
            {
                Destroy(bg.gameObject);
            }
        }

        /// <summary>
        /// Game must be assigned prior to this call
        /// </summary>
        private void GamepadCheck()
        {
            switch (game._Type)
            {
                case GameType.PASSANDPLAY:
                case GameType.REALTIME:
                    if (Input.GetJoystickNames().Length > 1)
                    {
                        StandaloneInputModuleExtended.GamepadFilter = 
                            StandaloneInputModuleExtended.GamepadControlFilter.SPECIFIC_GAMEPAD;
                        StandaloneInputModuleExtended.GamepadID = 
                            game._State.ActivePlayerId == 1 ? 0 : 1;
                    }

                    break;
            }
        }

        /// <summary>
        /// Game must be assigned prior to this call
        /// </summary>
        private void LogGameCheck()
        {
            logGameFinished = false;

            //log game finished
            switch (game._Type)
            {
                case GameType.TURN_BASED:
                    logGameFinished = PlayerPrefsWrapper.GetGameViewed(game.BoardID);

                    break;

                case GameType.PUZZLE:
                    logGameFinished = PlayerPrefsWrapper.GetPuzzleChallengeComplete(game.BoardID);

                    break;

                case GameType.AI:
                case GameType.PASSANDPLAY:
                    logGameFinished = true;

                    break;
            }

            //analytics
            AnalyticsManager.Instance.LogGame(game._Mode.GameModeToAnalyticsEvent(true), game);
        }

        private void PlayBGAudio()
        {
            switch (game._Type)
            {
                //case GameType.REALTIME:
                //    gameplayBGAudio = AudioHolder.instance.PlayBGAudio(AudioTypes.BG_GARDEN_REALTIME, true, 1f, 3f);
                //    break;

                default:
                    AudioTypes gameBGAudio = GameContentManager
                        .Instance
                        .themesDataHolder
                        .GetTheme(game._Area).bgAudio;

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
            switch (game._Type)
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
                    if (game.puzzleData)
                    {
                        if (game.puzzleData.pack)
                        {
                            PlayerPrefsWrapper.SetPuzzlePackOpened(game.puzzleData.pack.packID, true);
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

        #region Photon Callbacks

        private void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable values)
        {
            if (values.ContainsKey(Constants.REALTIME_PLAYER_1_READY) || 
                values.ContainsKey(Constants.REALTIME_PLAYER_2_READY))
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    if (FourzyPhotonManager.CheckPlayersReady())
                    {
                        CreateRealtimeGame();
                    }
                }
            }

            if (values.ContainsKey(Constants.EPOCH_KEY))
            {
                //update epoch delta
                if (!PhotonNetwork.IsMasterClient)
                {
                    epochDelta = 
                        values.ContainsKey(Constants.EPOCH_KEY) ? 
                        FourzyPhotonManager.GetRoomProperty(Constants.EPOCH_KEY, 0L) - Utils.EpochMilliseconds() : 0L;
                }
            }

            if (FourzyPhotonManager.CheckPlayersRematchReady())
            {
                if (waitingScreen && waitingScreen.isOpened)
                {
                    waitingScreen.CloseSelf();
                }
                waitingScreen = null;

                gameplayScreen.realtimeScreen.CheckWaitingForOtherPlayer("Waiting for game...");
                CreateRealtimeGame();
            }
        }

        private void OnEventCall(EventData data)
        {
            //will be called on other client
            switch (data.Code)
            {
                case Constants.GAME_DATA:
                    ClientFourzyGame _realtimeGame = 
                        new ClientFourzyGame(
                            JsonConvert.DeserializeObject<GameStateDataEpoch>(data.CustomData.ToString()));
                    //assign proper user id
                    _realtimeGame.SetPlayer2ID(UserManager.Instance.userId);

                    LoadGame(_realtimeGame);

                    break;

                case Constants.TAKE_TURN:
                    StartRoutine("takeTurn", PlayRealtimeTurn(
                        JsonConvert.DeserializeObject<PlayerTurn>(data.CustomData.ToString())));

                    break;

                case Constants.REMATCH_REQUEST:
                    waitingScreen = menuController.GetOrAddScreen<PromptScreen>()
                        .Prompt($"{game.opponent.DisplayName} wants to play again with you!", "Accept?", () =>
                        {
                            FourzyPhotonManager.SetClientRematchReady();
                            gameplayScreen.realtimeScreen.CheckWaitingForOtherPlayer("Waiting for game...");
                        }, BackButtonOnClick)
                        .CloseOnAccept();

                    break;

                case Constants.RATING_GAME_DATA:
                    OnRatingDataAquired(
                        JsonConvert.DeserializeObject<RatingGameCompleteResult>(data.CustomData.ToString()));

                    break;

                case Constants.RATING_GAME_OTHER_LOST:
                    game._State.WinnerId = game.me.PlayerId;
                    OnGameFinished(game);

                    break;
            }
        }

        private void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            if (game == null)
            {
                BackButtonOnClick();
                GamesToastsController.ShowTopToast($"Opponent left the room");

                return;
            }

            switch (game._Type)
            {
                case GameType.REALTIME:
                    //if more than X seconds passed, player who left the game loses
                    if (!game.isOver && Time.time - startedAt > 
                        Constants.REALTIME_GAME_VALID_AFTER_X_SECONDS)
                    {
                        OnRealtimeGameFinished(LoginManager.playfabID, GameManager.Instance.currentOpponent);
                    }

                    //pause game
                    PauseGame();

                    FourzyPhotonManager.TryLeaveRoom();

                    //display prompt
                    PersistantMenuController.instance.GetOrAddScreen<PromptScreen>().Prompt(
                        $"{otherPlayer.NickName} disconnected...",
                        "Other player disconnected.",
                        null,
                        "Back",
                        null,
                        BackButtonOnClick).CloseOnDecline();

                    break;
            }
        }

        #endregion

        internal void OnGameFinished(IClientFourzy game)
        {
            onGameFinished?.Invoke(game);

            gameplayScreen.OnGameFinished();

            //realtime game finished
            if (game._Type == GameType.REALTIME)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    if (game.IsWinner())
                    {
                        OnRealtimeGameFinished(LoginManager.playfabID, GameManager.Instance.currentOpponent);
                    }
                    else
                    {
                        OnRealtimeGameFinished(GameManager.Instance.currentOpponent, LoginManager.playfabID);
                    }
                }

                ratingUpdated = true;
            }

            //analytics event
            AnalyticsManager.GameResultType gameResult = AnalyticsManager.GameResultType.None;
            switch (game._Mode)
            {
                case GameMode.NONE:
                    //case GameMode.LOCAL_VERSUS:
                    gameResult = game.draw ? 
                        AnalyticsManager.GameResultType.Draw : 
                        AnalyticsManager.GameResultType.Win;

                    break;

                default:
                    if (!logGameFinished)
                    {
                        if (game.draw)
                            gameResult = AnalyticsManager.GameResultType.Draw;
                        else
                            gameResult = game.IsWinner() ?
                                AnalyticsManager.GameResultType.Win :
                                AnalyticsManager.GameResultType.Lose;
                    }

                    break;
            }

            if (gameResult != AnalyticsManager.GameResultType.None)
            {
                AnalyticsManager.Instance.LogGame(
                    game._Mode.GameModeToAnalyticsEvent(false),
                    game,
                    extraParams: new KeyValuePair<string, object>(AnalyticsManager.GAME_RESULT_KEY, gameResult));
            }
            //

            //rewards screen
            switch (game._Type)
            {
                case GameType.TURN_BASED:
                case GameType.REALTIME:
                case GameType.PASSANDPLAY:
                    PersistantMenuController.instance.GetOrAddScreen<RewardsScreen>().SetData(game);

                    break;
            }
            //

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

        private void OnDraw(IClientFourzy game)
        {
            //channel into gamefinished pipeline
            OnGameFinished(game);
        }

        private void OnMoveStarted(ClientPlayerTurn turn, bool startTurn)
        {
            if (startTurn) return;

            gameplayScreen.OnMoveStarted(turn);

            onMoveStarted?.Invoke(turn);
        }

        private void OnMoveEnded(ClientPlayerTurn turn, PlayerTurnResult turnResult, bool startTurn)
        {
            if (startTurn) return;

            if (replayingLastTurn) replayingLastTurn = false;

            onMoveEnded?.Invoke(turn);

            gameplayScreen.OnMoveEnded(turn, turnResult);

            UpdatePlayerTurn();
        }

        private void OnNetwork(bool state)
        {
            if (game == null) return;

            if (!state)
            {
                if (game._Type == GameType.REALTIME)
                {
                    PauseGame();

                    //open prompt screen
                    PersistantMenuController.instance.GetOrAddScreen<PromptScreen>().Prompt(
                        "Connection failed.",
                        "Failed to connect to multiplayer server :(", null,
                        LocalizationManager.Value("back"), null, () =>
                        {
                            PersistantMenuController.instance.CloseCurrentScreen();
                            BackButtonOnClick();
                        });
                }

                switch (game._Type)
                {
                    case GameType.FRIEND:
                    case GameType.LEADERBOARD:
                    case GameType.TURN_BASED:
                        noNetworkOverlay.SetActive(true);

                        break;
                }
            }
        }

        private void OnRealtimeGameFinished(string winnerID, string opponentID)
        {
            if (game == null) return;
            if (string.IsNullOrEmpty(GameManager.Instance.currentOpponent)) return;
            if (ratingUpdated) return;

            UserManager.Instance.playfabWinsCount += 1;

            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
            {
                FunctionName = "reportRatingGameComplete",
                FunctionParameter = new
                {
                    winnerID,
                    opponentID,
                    game.draw,
                },
                GeneratePlayStreamEvent = true,
            },
            (result) =>
            {
                OnRatingDataAquired(
                    JsonConvert.DeserializeObject<RatingGameCompleteResult>(result.FunctionResult.ToString()));

                //try send rating update to other client 
                if (PhotonNetwork.CurrentRoom != null)
                {
                    var eventOptions = new Photon.Realtime.RaiseEventOptions();
                    eventOptions.Flags.HttpForward = true;
                    eventOptions.Flags.WebhookFlags = Photon.Realtime.WebFlags.HttpForwardConst;

                    var photonEventResult = PhotonNetwork.RaiseEvent(
                        Constants.RATING_GAME_DATA,
                        result.FunctionResult.ToString(),
                        eventOptions,
                        SendOptions.SendReliable);
                }
            },
            (error) => { Debug.LogError(error.ErrorMessage); });
        }

        private void OnRatingDataAquired(RatingGameCompleteResult data)
        {
            foreach (RatingGamePlayer player in data.players)
            {
                if (player.playfabID == LoginManager.playfabID)
                {
                    UserManager.Instance.lastCachedRating = player.rating;

                    if (player.winner)
                    {
                        UserManager.Instance.playfabWinsCount += 1;

                        gameWinLoseScreen.SetInfoLabel($"+{player.ratingChange} rating points!");
                    }
                    else
                    {
                        UserManager.Instance.playfabLosesCount += 1;

                        gameWinLoseScreen.SetInfoLabel($"{player.ratingChange} rating points!");
                    }
                }
            }
        }

        private void OnLogin(bool state)
        {
            if (state)
            {
                noNetworkOverlay.SetActive(false);
            }
        }

        private void OnNoInput(KeyValuePair<string, float> noInputFilter)
        {
            if (noInputFilter.Key != "highlightMoves" || !gameplayScreen.isCurrent) return;

            //if its NOT my turn, exit
            switch (game._Type)
            {
                case GameType.PASSANDPLAY:

                    break;

                default:
                    if (!game.isMyTurn) return;

                    break;
            }

            switch (board.actionState)
            {
                case GameboardView.BoardActionState.MOVE:
                    switch (GameManager.Instance.placementStyle)
                    {
                        case GameManager.PlacementStyle.EDGE_TAP:
                            board.ShowHintArea(
                                GameboardView.HintAreaStyle.ANIMATION_LOOP, 
                                GameboardView.HintAreaAnimationPattern.NONE);
                            board.SetHintAreaSelectableState(false);

                            break;
                    }

                    break;
            }
        }

        private IEnumerator GameInitRoutine()
        {
            if (game == null) yield break;

            yield return StartCoroutine(FadeGameScreen(1f, .5f));

            MenuScreen _screen = menuController.GetScreen<PrePackPrompt>();
            if (!_screen)
            {
                _screen = menuController.GetScreen<VSGamePrompt>();
            }

            while (_screen && _screen.isOpened) yield return null;

            //instruction boards
            switch (game._Type)
            {
                //no token instructions for these modes
                case GameType.REALTIME:
                case GameType.ONBOARDING:
                case GameType.PRESENTATION:

                    break;

                default:
                    yield return StartCoroutine(ShowTokenInstructionPopupRoutine());

                    break;
            }

            switch (game._Type)
            {
                case GameType.TURN_BASED:
                    if (!game.asFourzyGame.challengeData.haveMoves)
                    {
                        playerPickScreen._Open();
                    }
                    else
                    {
                        PlayerTurn lastTurn = game.asFourzyGame.challengeData.lastTurn;

                        replayingLastTurn = true;

                        UpdatePlayerTurn();

                        board.TakeTurn(lastTurn);
                    }

                    break;

                //start timer
                case GameType.REALTIME:
                    StartRoutine("realtimeCoutdownRoutine", StartRealtimeCountdown());

                    break;

                default:
                    UpdatePlayerTurn();

                    break;
            }

            isBoardReady = true;
        }

        private IEnumerator PlayHintRoutine()
        {
            int lastHintIndex = PlayerPrefsWrapper.GetPuzzleHintProgress(game.BoardID);

            bool resetBoard = game._playerTurnRecord.Count != lastHintIndex;

            if (!resetBoard)
            {
                for (int turnIndex = 0; turnIndex < game._playerTurnRecord.Count; turnIndex++)
                {
                    if (game._playerTurnRecord[turnIndex].Notation != game.puzzleData.Solution[turnIndex].Notation)
                    {
                        resetBoard = true;
                        break;
                    }
                }
            }

            if (resetBoard)
            {
                game._Reset();

                for (int turnIndex = 0; turnIndex < lastHintIndex; turnIndex++)
                {
                    game.TakeTurn(game.puzzleData.Solution[turnIndex], true, false);
                    game.TakeAITurn(false);
                }

                LoadGame(game);
            }

            while (IsRoutineActive("gameInit")) yield return null;

            //play hint
            //board.TakeTurn(game.puzzleData.Solution[lastHintIndex]);

            //display hint
            DisplayHint(game.puzzleData.Solution[lastHintIndex]);

            PlayerPrefsWrapper.SetPuzzleHintProgress(game.BoardID, lastHintIndex + 1 == game.puzzleData.Solution.Count ? 0 : lastHintIndex + 1);
        }

        private IEnumerator StartRealtimeCountdown()
        {
            //start timer
            yield return gameplayScreen.realtimeScreen
                .StartCountdown(InternalSettings.Current.REALTIME_COUNTDOWN_SECONDS);

            UpdatePlayerTurn();
        }

        private IEnumerator PlayTurnBaseTurn(ChallengeData gameData)
        {
            if (gameData.lastTurnGame._Type != GameType.TURN_BASED) yield break;

            yield return new WaitUntil(() => !board.isAnimating && isBoardReady);

            PlayerTurn lastTurn = gameData.lastTurn;

            if (game.asFourzyGame.playerTurnRecord.Count > 0 &&
                game.asFourzyGame.playerTurnRecord[game.asFourzyGame.playerTurnRecord.Count - 1].PlayerId == lastTurn.PlayerId) yield break;

            //compare challenges
            if (gameData.challengeInstanceId != game.asFourzyGame.challengeData.challengeInstanceId) yield break;

            board.TakeTurn(lastTurn);
            UpdatePlayerTurn();
        }

        private IEnumerator PlayRealtimeTurn(PlayerTurn turn)
        {
            if (game._Type != GameType.REALTIME) yield break;

            yield return new WaitUntil(() => !board.isAnimating && isBoardReady);

            board.TakeTurn(turn);
            UpdatePlayerTurn();
        }

        private IEnumerator PostGameFinished()
        {
            yield return new WaitForSeconds(.5f);

            //visuals + haptic
            switch (game._Type)
            {
                case GameType.ONBOARDING:
                    break;

                case GameType.FRIEND:
                case GameType.LEADERBOARD:
                case GameType.PUZZLE:
                case GameType.AI:
                case GameType.TURN_BASED:
                case GameType.REALTIME:
                    if (game.IsWinner())
                    {
                        winningParticleGenerator.ShowParticles();

                        GameManager.Vibrate();
                    }
                    break;

                default:
                    if (!game.draw) winningParticleGenerator.ShowParticles();

                    GameManager.Vibrate();
                    break;
            }

            //sound
            switch (game._Type)
            {
                case GameType.ONBOARDING:
                    break;

                case GameType.PUZZLE:
                case GameType.REALTIME:
                    if (game.IsWinner())
                        AudioHolder.instance.PlaySelfSfxOneShotTracked(AudioTypes.GAME_WON);
                    else
                        AudioHolder.instance.PlaySelfSfxOneShotTracked(AudioTypes.GAME_LOST);

                    break;

                case GameType.FRIEND:
                case GameType.LEADERBOARD:
                case GameType.AI:
                case GameType.TURN_BASED:
                    if (game.IsWinner())
                        AudioHolder.instance.PlaySelfSfxOneShotTracked(AudioTypes.GAME_TURNBASED_WON);
                    else
                        AudioHolder.instance.PlaySelfSfxOneShotTracked(AudioTypes.GAME_TURNBASED_LOST);

                    break;

                default:
                    AudioHolder.instance.PlaySelfSfxOneShotTracked(AudioTypes.GAME_WON);

                    break;
            }

            //if demo, restart
            switch (game._Type)
            {
                case GameType.PRESENTATION:
                    yield return new WaitForSeconds(3f);

                    LoadGame(new ClientFourzyGame(GameContentManager.Instance.themesDataHolder.GetRandomTheme(Area.NONE),
                        new FourzyGameModel.Model.Player(1, "AI Player 1") { PlayerString = "1" },
                        new FourzyGameModel.Model.Player(2, "AI Player 2") { PlayerString = "2" }, 1)
                    { _Type = GameType.PRESENTATION, });

                    break;
            }
        }
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

