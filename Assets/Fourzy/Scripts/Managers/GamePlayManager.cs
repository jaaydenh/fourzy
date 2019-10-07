﻿//modded @vadym udod

using Fourzy._Updates.Audio;
using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Managers;
using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tools;
using Fourzy._Updates.UI.Menu;
using Fourzy._Updates.UI.Menu.Screens;
using FourzyGameModel.Model;
using GameSparks.Api.Responses;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static Fourzy._Updates.Serialized.ThemesDataHolder;

namespace Fourzy._Updates.Mechanics.GameplayScene
{
    public class GamePlayManager : RoutinesBase
    {
        public static GamePlayManager instance;

        public static Action<ClientPlayerTurn> onMoveStarted;
        public static Action<ClientPlayerTurn> onMoveEnded;
        public static Action<IClientFourzy> onGameFinished;

        public static Action<string> OnGamePlayMessage;
        public static Action<long> OnTimerUpdate;

        public FourzyGameMenuController menuController;
        public WinningParticleGenerator winningParticleGenerator;
        public Transform bgParent;
        public GameObject noNetworkOverlay;
        public RectTransform hintBlocksParent;

        private AudioHolder.BGAudio gameplayBGAudio;

        //id of rematch challenge
        private string awaitingChallengeID = "";
        private long epochDelta;

        public BackgroundConfigurationData currentConfiguration { get; private set; }
        public GameboardView board { get; private set; }
        public GameplayBG bg { get; private set; }
        public GameplayScreen gameplayScreen { get; private set; }
        public RandomPlayerPickScreen playerPickScreen { get; private set; }
        public LoadingPromptScreen loadingPrompt { get; private set; }

        public IClientFourzy game { get; private set; }

        public bool isBoardReady { get; private set; }
        public bool replayingLastTurn { get; private set; }
        public bool gameStarted { get; private set; }
        public bool isGamePaused { get; private set; }

        /// <summary>
        /// Used to check if postGame should log the game or not
        /// </summary>
        private bool logGameFinished;

        protected override void Awake()
        {
            base.Awake();

            instance = this;
        }

        protected void Start()
        {
            gameplayScreen = menuController.GetScreen<GameplayScreen>();
            playerPickScreen = menuController.GetScreen<RandomPlayerPickScreen>();

            GameManager.onNetworkAccess += OnNetwork;
            LoginManager.OnDeviceLoginComplete += OnLogin;
            ChallengeManager.OnChallengeUpdate += OnChallengeUpdate;
            ChallengeManager.OnChallengesUpdate += OnChallengesUpdate;

            //listen to roomPropertyChanged event
            FourzyPhotonManager.onRoomCustomPropertiesChanged += OnRoomCustomPropertiesChanged;
            FourzyPhotonManager.onPlayerDisconnected += OnPlayerDisconnected;
            PhotonNetwork.OnEventCall += OnEventCall;

            if (SettingsManager.Instance.Get(SettingsManager.KEY_DEMO_MODE)) PointerInputModuleExtended.noInput += OnNoInput;

            //auto load game if its not realtime mdoe
            if (GameManager.Instance.isRealtime)
            {
                //notify that this client is ready
                FourzyPhotonManager.SetClientReady();
                gameplayScreen.realtimeScreen.CheckWaitingForOtherPlayer();

                OnRoomCustomPropertiesChanged(PhotonNetwork.room.CustomProperties);
            }
            else LoadGame(GameManager.Instance.activeGame);
        }

        protected void Update()
        {
            //add gems
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.O))
                UserManager.Instance.hints += 3;
            else if (Input.GetKeyDown(KeyCode.I))
                UserManager.Instance.hints -= 3;
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
            ChallengeManager.OnChallengeUpdate -= OnChallengeUpdate;
            ChallengeManager.OnChallengesUpdate -= OnChallengesUpdate;

            FourzyPhotonManager.onRoomCustomPropertiesChanged -= OnRoomCustomPropertiesChanged;
            FourzyPhotonManager.onPlayerDisconnected -= OnPlayerDisconnected;
            PhotonNetwork.OnEventCall -= OnEventCall;

            if (SettingsManager.Instance.Get(SettingsManager.KEY_DEMO_MODE)) PointerInputModuleExtended.noInput -= OnNoInput;

            AudioHolder.instance.StopBGAudio(gameplayBGAudio, .5f);
        }

        protected void SetGameIfNull(IClientFourzy _game)
        {
            //if active game is empty, load random pass&play board
            if (_game == null)
            {
                //ClientFourzyGame newGame = new ClientFourzyGame(GameContentManager.Instance.passAndPlayDataHolder.random, UserManager.Instance.meAsPlayer, new Player(2, "Player Two"));
                ClientFourzyGame newGame = new ClientFourzyGame(GameContentManager.Instance.GetMiscBoardByName("DrawBoardTester"), UserManager.Instance.meAsPlayer, new Player(2, "Player Two"));
                //ClientFourzyGame newGame = new ClientFourzyGame(GameContentManager.Instance.GetMiscBoard("23"), UserManager.Instance.meAsPlayer, new Player(2, "Player Two"));
                newGame._Type = GameType.PASSANDPLAY;

                GameManager.Instance.activeGame = newGame;
            }
            else
            if (_game != GameManager.Instance.activeGame)
                GameManager.Instance.activeGame = _game;

            game = GameManager.Instance.activeGame;
        }

        public void LoadGame(IClientFourzy _game)
        {
            CancelRoutine("gameInit");
            CancelRoutine("takeTurn");
            CancelRoutine("realtimeCoutdownRoutine");
            CancelRoutine("postGameRoutine");

            awaitingChallengeID = "";
            gameStarted = false;
            isGamePaused = false;
            isBoardReady = false;

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

            gameplayScreen.InitUI(this);
            OnNetwork(GameManager.NetworkAccess);

            StartRoutine("gameInit", GameInitRoutine());
        }

        public void CreateRealtimeGame()
        {
            //only continue if master
            if (PhotonNetwork.isMasterClient)
            {
                //ready me
                Player me = new Player(1, UserManager.Instance.userName);
                me.PlayerString = "1";
                me.HerdId = UserManager.Instance.gamePieceID + "";
                //ready opponent
                Player opponen = new Player(2, PhotonNetwork.otherPlayers[0].NickName);
                opponen.HerdId = PhotonNetwork.otherPlayers[0].CustomProperties.ContainsKey("gp") ? PhotonNetwork.otherPlayers[0].CustomProperties["gp"].ToString() : "1";
                opponen.PlayerString = "2";

                //load realtime game
                ClientFourzyGame _game = new ClientFourzyGame(GameContentManager.Instance.currentTheme.themeID, me, opponen, 1)
                { _Type = GameType.REALTIME, _Area = GameContentManager.Instance.currentTheme.themeID };

                GameStateDataEpoch gameStateData = _game.toGameStateData;
                //add realtime data
                gameStateData.realtimeData = new RealtimeData() { createdEpoch = Utils.EpochMilliseconds(), };

                //send this game data to other player
                PhotonNetwork.RaiseEvent(Constants.GAME_DATA, JsonConvert.SerializeObject(gameStateData), true, new RaiseEventOptions() { Receivers = ReceiverGroup.Others, });

                LoadGame(_game);
            }
        }

        public void OnPointerDown(Vector2 position)
        {
            //only continue if current opened screen is GameplayScreen
            if (gameplayScreen != menuController.currentScreen || isGamePaused) return;

            board.OnPointerDown(position);
        }

        public void OnPointerMove(Vector2 position)
        {
            //release controls if current screen isnt GameplayScreen
            if (gameplayScreen != menuController.currentScreen || isGamePaused)
            {
                OnPointerRelease(position);

                return;
            }

            board.OnPointerMove(position);
        }

        public void OnPointerRelease(Vector2 position)
        {
            if (isGamePaused) return;

            board.OnPointerRelease(position);
        }

        public void UpdatePlayerTurn()
        {
            if (game == null) return;

            if (!gameStarted)
            {
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
            }
        }

        /// <summary>
        /// Triggered after board/tokens fade id + optional delays
        /// </summary>
        private void OnGameStarted()
        {
            gameplayScreen.OnGameStarted();

            board.interactable = true;
            board.OnPlayManagerReady();
            game.SetInitialTime(Time.time);
        }

        private IEnumerator ShowTokenInstructionPopupRoutine()
        {
            if (game == null || game._Type == GameType.ONBOARDING) yield break;

            HashSet<TokensDataHolder.TokenData> tokens = new HashSet<TokensDataHolder.TokenData>();

            foreach (BoardSpace boardSpace in game.boardContent)
                foreach (IToken token in boardSpace.Tokens.Values)
                    if (!PlayerPrefsWrapper.InstructionPopupWasDisplayed((int)token.Type) && !GameManager.Instance.excludeInstructionsFor.Contains(token.Type))
                        tokens.Add(GameContentManager.Instance.GetTokenData(token.Type));

            if (tokens.Count > 0)
                yield return new WaitForSeconds(.5f);

            foreach (TokensDataHolder.TokenData token in tokens)
            {
                TokenPrompt popupUI = menuController.GetScreen<TokenPrompt>(true);
                popupUI.Prompt(token);

                yield return new WaitWhile(() => popupUI.isOpened);

                PlayerPrefsWrapper.SetInstructionPopupDisplayed((int)token.tokenType, true);

                yield return new WaitForSeconds(.5f);
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
            if (board.tokens.Count > 0) board.FadeTokens(alpha, fadeTime);
        }

        private void FadePieces(float alpha, float fadeTime)
        {
            if (board.gamePieces.Count > 0) board.FadeGamepieces(alpha, fadeTime);
        }

        public void BackButtonOnClick()
        {
            GameManager.Instance.OpenMainMenu();

            //disconnect if realtime
            switch (game._Type)
            {
                case GameType.REALTIME:
                    PhotonNetwork.LeaveRoom();

                    break;
            }
        }

        public void UnloadGamePlayScreen()
        {
            SceneManager.UnloadSceneAsync(Constants.GAMEPLAY_SCENE_NAME);
        }

        public void Rematch()
        {
            if (game == null) return;

            switch (game._Type)
            {
                case GameType.TURN_BASED:
                    loadingPrompt = menuController.GetScreen<LoadingPromptScreen>();

                    loadingPrompt.Prompt("", "Loading new game...", null, null, null, null);

                    ChallengeManager.Instance.CreateTurnBasedGame(game.opponent.PlayerString, game._Area, CreateTurnBasedGameSuccess, CreateTurnBasedGameError);

                    break;

                case GameType.AI:
                case GameType.PUZZLE:
                    game.Reset();

                    LoadGame(game);

                    break;

                case GameType.PASSANDPLAY:
                    //create new game if random board
                    if (game.asFourzyGame.isBoardRandom)
                        game = new ClientFourzyGame(
                            GameContentManager.Instance.currentTheme.themeID,
                            UserManager.Instance.meAsPlayer, new Player(2, "Player Two"),
                            UserManager.Instance.meAsPlayer.PlayerId)
                        { _Type = GameType.PASSANDPLAY };
                    else
                        game.Reset();

                    LoadGame(game);

                    break;
            }
        }

        public void PlayHint()
        {
            if (!game.isMyTurn || game.isOver) return;

            if (UserManager.Instance.hints <= 0)
            {
                menuController.GetScreen<StorePromptScreen>().Prompt(StorePromptScreen.StoreItemType.HINTS);

                return;
            }

            if (game == null || !game.puzzleData || game.puzzleData.Solution.Count == 0) return;

            UserManager.Instance.hints--;
            AnalyticsManager.Instance.LogGameEvent(AnalyticsManager.AnalyticsGameEvents.USE_HINT, game);

            StartRoutine("hintRoutine", PlayHintRoutine());
        }

        public void PauseGame()
        {
            if (isGamePaused) return;

            isGamePaused = true;
            gameplayScreen.OnGamePaused();
        }

        public void UnpauseGame()
        {
            if (!isGamePaused) return;

            isGamePaused = false;
            gameplayScreen.OnGameUnpaused();
        }

        /// <summary>
        /// Game must be assigned prior to this call
        /// </summary>
        private void LoadBoard()
        {
            if (board) Destroy(board.gameObject);

            board = Instantiate(currentConfiguration.gameboardPrefab, transform);
            board.Initialize(game);
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

        private void LoadBG(Area area)
        {
            //unload old bg
            if (bg) Destroy(bg.gameObject);

            currentConfiguration = GameContentManager.Instance.themesDataHolder.GetThemeBGConfiguration(area, Camera.main);
            bg = Instantiate(currentConfiguration.backgroundPrefab, bgParent);
            bg.transform.localPosition = Vector3.zero;
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
                        StandaloneInputModuleExtended.GamepadFilter = StandaloneInputModuleExtended.GamepadControlFilter.SPECIFIC_GAMEPAD;
                        StandaloneInputModuleExtended.GamepadID = game._State.ActivePlayerId == 1 ? 0 : 1;
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
                    logGameFinished = PlayerPrefsWrapper.GetGameViewed(game.GameID);

                    break;

                case GameType.PUZZLE:
                    logGameFinished = PlayerPrefsWrapper.GetPuzzleChallengeComplete(game.GameID);

                    break;

                case GameType.AI:
                case GameType.PASSANDPLAY:
                    logGameFinished = true;

                    break;
            }

            //log game open
            switch (game._Type)
            {
                case GameType.TURN_BASED:
                case GameType.PUZZLE:
                    AnalyticsManager.Instance.LogGameEvent(AnalyticsManager.AnalyticsGameEvents.GAME_OPEN, game);

                    break;
            }
        }

        private void PlayBGAudio()
        {
            switch (game._Type)
            {
                //case GameType.REALTIME:
                //    gameplayBGAudio = AudioHolder.instance.PlayBGAudio(AudioTypes.BG_GARDEN_REALTIME, true, .9f, 3f);
                //    break;

                default:
                    AudioTypes gameBGAudio = GameContentManager.Instance.themesDataHolder.GetTheme(game._Area).bgAudio;

                    if (gameplayBGAudio != null)
                    {
                        if (gameplayBGAudio.type != gameBGAudio)
                        {
                            AudioHolder.instance.StopBGAudio(gameplayBGAudio, .5f);
                            gameplayBGAudio = AudioHolder.instance.PlayBGAudio(gameBGAudio, true, .9f, 3f);
                        }
                    }
                    else
                        gameplayBGAudio = AudioHolder.instance.PlayBGAudio(gameBGAudio, true, .9f, 3f);
                    break;
            }
        }

        private void GameOpenedCheck()
        {
            switch (game._Type)
            {
                case GameType.TURN_BASED:
                    //if game was over and havent been viewed yet, set it to viewed
                    if (game.asFourzyGame.challengeData.lastTurnGame.isOver && !PlayerPrefsWrapper.GetGameViewed(game.GameID))
                    {
                        PlayerPrefsWrapper.SetGameViewed(game.GameID);

                        ChallengeManager.OnChallengeUpdateLocal.Invoke(game.asFourzyGame.challengeData);
                    }
                    break;

                default:
                    //if current game have puzzle data assigned, set puzzlepack as opened
                    if (game.puzzleData)
                    {
                        if (game.puzzleData.pack)
                            PlayerPrefsWrapper.SetPuzzlePackOpened(game.puzzleData.pack.packID, true);
                    }

                    break;
            }
        }

        #region Turn Base Calls

        private void OnChallengeUpdate(ChallengeData gameData)
        {
            StartRoutine("takeTurn", PlayTurnBaseTurn(gameData));
        }

        private void OnChallengesUpdate(List<ChallengeData> challenges)
        {
            //if game waits for rematch challenge
            if (!string.IsNullOrEmpty(awaitingChallengeID))
            {
                //find the event we ve been waiting for
                ChallengeData challenge = challenges.Find(_challenge => _challenge.challengeInstanceId == awaitingChallengeID);

                if (challenge == null) return;

                GameManager.Instance.StartGame(challenge.lastTurnGame);
            }
        }

        private void CreateTurnBasedGameSuccess(LogEventResponse response)
        {
            awaitingChallengeID = response.ScriptData.GetString("challengeInstanceId");
            Debug.Log($"Game created {awaitingChallengeID}");
        }

        private void CreateTurnBasedGameError(LogEventResponse response)
        {
            Debug.Log("***** Error Creating Turn based game: " + response.Errors.JSON);
            AnalyticsManager.Instance.LogError(response.Errors.JSON, AnalyticsManager.AnalyticsErrorType.create_turn_base_game);

            if (loadingPrompt && loadingPrompt.isOpened)
            {
                loadingPrompt.promptText.text = "Failed to create new game...\n" + response.Errors.JSON;
                StartRoutine("closingLoadingPrompt", 5f, () => menuController.CloseCurrentScreen());
            }
        }

        #endregion

        #region Photon Callbacks

        private void OnRoomCustomPropertiesChanged(ExitGames.Client.Photon.Hashtable values)
        {
            if (values.ContainsKey(Constants.PLAYER_1_READY) || values.ContainsKey(Constants.PLAYER_2_READY))
            {
                if (PhotonNetwork.isMasterClient)
                {
                    if (FourzyPhotonManager.CheckPlayersReady()) CreateRealtimeGame();
                }
            }

            if (values.ContainsKey(Constants.EPOCH_KEY))
            {
                //update epoch delta
                if (!PhotonNetwork.isMasterClient) epochDelta = values.ContainsKey(Constants.EPOCH_KEY) ? FourzyPhotonManager.GetRoomProperty(Constants.EPOCH_KEY, 0L) - Utils.EpochMilliseconds() : 0L;
            }
        }

        private void OnEventCall(byte eventCode, object content, int senderId)
        {
            //will be called on other client
            switch (eventCode)
            {
                case Constants.GAME_DATA:
                    LoadGame(new ClientFourzyGame(JsonConvert.DeserializeObject<GameStateDataEpoch>(content.ToString())));

                    break;

                case Constants.TAKE_TURN:
                    StartRoutine("takeTurn", PlayRealtimeTurn(JsonConvert.DeserializeObject<PlayerTurn>(content.ToString())));

                    break;
            }
        }

        private void OnPlayerDisconnected(PhotonPlayer otherPlayer)
        {
            switch (game._Type)
            {
                case GameType.REALTIME:
                    //only if game is not finished
                    if (game.isOver) return;

                    //pause game
                    PauseGame();

                    //display prompt
                    menuController.GetScreen<PromptScreen>().Prompt($"{otherPlayer.NickName} disconnected...", "Other player disconnected.", null, "Back", null, () =>
                    {
                        BackButtonOnClick();
                    });

                    break;
            }
        }

        #endregion

        private void OnGameFinished(IClientFourzy game)
        {
            onGameFinished?.Invoke(game);

            gameplayScreen.OnGameFinished();

            if (!logGameFinished)
            {
                AnalyticsManager.Instance.LogGameEvent(game.IsWinner() ? AnalyticsManager.AnalyticsGameEvents.GAME_FINISHED : AnalyticsManager.AnalyticsGameEvents.GAME_FAILED, game);
            }

            switch (game._Type)
            {
                case GameType.TURN_BASED:
                case GameType.REALTIME:
                case GameType.PASSANDPLAY:
                    PersistantMenuController.instance.GetScreen<RewardsScreen>().SetData(game);

                    break;
            }

            //reset controller filter
            switch (game._Type)
            {
                case GameType.PASSANDPLAY:
                    //change gamepad mode
                    StandaloneInputModuleExtended.GamepadFilter = StandaloneInputModuleExtended.GamepadControlFilter.ANY_GAMEPAD;

                    break;
            }

            StartRoutine("postGameRoutine", PostGameFinished());
        }

        private void OnDraw(IClientFourzy game)
        {
            //channel into gamefinished pipeline
            OnGameFinished(game);
        }

        private void OnMoveStarted(ClientPlayerTurn turn)
        {
            gameplayScreen.OnMoveStarted(turn);

            onMoveStarted?.Invoke(turn);
        }

        private void OnMoveEnded(ClientPlayerTurn turn, PlayerTurnResult turnResult)
        {
            if (replayingLastTurn) replayingLastTurn = false;

            onMoveEnded?.Invoke(turn);

            gameplayScreen.OnMoveEnded(turn, turnResult);

            UpdatePlayerTurn();

            //change active gamepad id
            StandaloneInputModuleExtended.GamepadID = game._State.ActivePlayerId == 1 ? 0 : 1;
        }

        private void OnNetwork(bool state)
        {
            if (game == null) return;

            if (!state)
            {
                switch (game._Type)
                {
                    case GameType.FRIEND:
                    case GameType.LEADERBOARD:
                    case GameType.REALTIME:
                    case GameType.TURN_BASED:
                        noNetworkOverlay.SetActive(true);
                        break;
                }
            }
        }

        private void OnLogin(bool state)
        {
            if (state) noNetworkOverlay.SetActive(false);
        }

        private void OnNoInput(KeyValuePair<string, float> noInputFilter)
        {
            if (noInputFilter.Key != "highlightMoves" || !gameplayScreen.isCurrent) return;

            switch (board.actionState)
            {
                case GameboardView.BoardActionState.SIMPLE_MOVE:
                    board.ShowHintArea(GameboardView.HintAreaStyle.ANIMATION_LOOP, GameboardView.HintAreaAnimationPattern.NONE);
                    board.SetHintAreaSelectableState(false);

                    break;
            }
        }

        private IEnumerator GameInitRoutine()
        {
            if (game == null) yield break;

            yield return StartCoroutine(FadeGameScreen(1f, .5f));

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
                        playerPickScreen._Open();
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
            int lastHintIndex = PlayerPrefsWrapper.GetPuzzleHintProgress(game.GameID);

            bool resetBoard = game._playerTurnRecord.Count != lastHintIndex;

            if (!resetBoard)
                for (int turnIndex = 0; turnIndex < game._playerTurnRecord.Count; turnIndex++)
                    if (game._playerTurnRecord[turnIndex].Notation != game.puzzleData.Solution[turnIndex].Notation)
                    {
                        resetBoard = true;
                        break;
                    }

            if (resetBoard)
            {
                game.Reset();

                for (int turnIndex = 0; turnIndex < lastHintIndex; turnIndex++)
                {
                    game.TakeTurn(game.puzzleData.Solution[turnIndex], true, false);
                    game.TakeAITurn(false);
                }

                LoadGame(game);
            }

            while (IsRoutineActive("gameInit")) yield return null;

            board.TakeTurn(game.puzzleData.Solution[lastHintIndex]);
            PlayerPrefsWrapper.SetPuzzleHintProgress(game.GameID, lastHintIndex + 1 == game.puzzleData.Solution.Count ? 0 : lastHintIndex + 1);
        }

        private IEnumerator StartRealtimeCountdown()
        {
            //start timer
            yield return gameplayScreen.realtimeScreen.StartCountdown(Constants.REALTIME_COUNTDOWN_SECONDS);

            UpdatePlayerTurn();
        }

        private IEnumerator PlayTurnBaseTurn(ChallengeData gameData)
        {
            if (gameData.lastTurnGame._Type != GameType.TURN_BASED) yield break;

            yield return new WaitUntil(() => !board.isAnimating && isBoardReady);

            PlayerTurn lastTurn = gameData.lastTurn;

            if (game.asFourzyGame.playerTurnRecord.Count > 0 && game.asFourzyGame.playerTurnRecord[game.asFourzyGame.playerTurnRecord.Count - 1].PlayerId == lastTurn.PlayerId) yield break;

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
                        new Player(1, "AI Player 1") { PlayerString = "1" },
                        new Player(2, "AI Player 2") { PlayerString = "2" }, 1)
                    { _Type = GameType.PRESENTATION, });

                    break;
            }
        }
    }
}

