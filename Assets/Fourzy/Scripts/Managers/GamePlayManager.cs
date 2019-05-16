//modded @vadym udod

using Fourzy._Updates.Audio;
using Fourzy._Updates.ClientModel;
using Fourzy._Updates.Mechanics.Board;
using Fourzy._Updates.Serialized;
using Fourzy._Updates.Tools;
using Fourzy._Updates.Tools.Timing;
using Fourzy._Updates.UI.Menu;
using Fourzy._Updates.UI.Menu.Screens;
using FourzyGameModel.Model;
using GameSparks.Api.Responses;
using mixpanel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Fourzy._Updates.Serialized.ThemesDataHolder;

namespace Fourzy._Updates.Mechanics.GameplayScene
{
    public class GamePlayManager : RoutinesBase
    {
        public static GamePlayManager instance;

        public static Action<int> onMoveStarted;
        public static Action<int> onMoveEnded;
        public static Action<IClientFourzy> onGameFinished;

        public static Action<string> OnGamePlayMessage;
        public static Action<long> OnTimerUpdate;

        public MenuController menuController;
        public WinningParticleGenerator winningParticleGenerator;
        public Transform bgParent;
        public AdvancedTimingEventsSet notYourTurnLabel;
        public GameObject noNetworkOverlay;
        
        private AudioHolder.BGAudio gameplayBGAudio;

        //id of rematch challenge
        private string awaitingChallengeID = "";

        public BackgroundConfigurationData currentConfiguration { get; private set; }
        public GameboardView board { get; private set; }
        public GameplayBG bg { get; private set; }
        public GameplayScreen gameplayScreen { get; private set; }
        public GameInfoScreen gameInfoScreen { get; private set; }
        public RandomPlayerPickScreen playerPickScreen { get; private set; }
        public LoadingPromptScreen loadingPrompt { get; private set; }

        public IClientFourzy game { get; private set; }

        public bool isBoardReady { get; private set; }
        public bool replayingLastTurn { get; private set; }
        public bool gameStarted { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            instance = this;
            isBoardReady = false;
        }

        protected void Start()
        {
            gameInfoScreen = menuController.GetScreen<GameInfoScreen>();
            gameplayScreen = menuController.GetScreen<GameplayScreen>();
            playerPickScreen = menuController.GetScreen<RandomPlayerPickScreen>();

            NetworkAccess.onNetworkAccess += OnNetwork;
            LoginManager.OnDeviceLoginComplete += OnLogin;
            ChallengeManager.OnChallengeUpdate += OnChallengeUpdate;
            ChallengeManager.OnChallengesUpdate += OnChallengesUpdate;

            LoadGame(GameManager.Instance.activeGame);
        }

        protected void OnDestroy()
        {
            board.onGameFinished -= OnGameFinished;
            board.onDraw -= OnDraw;
            board.onMoveStarted -= OnMoveStarted;
            board.onMoveEnded -= OnMoveEnded;

            NetworkAccess.onNetworkAccess -= OnNetwork;
            LoginManager.OnDeviceLoginComplete -= OnLogin;
            ChallengeManager.OnChallengeUpdate -= OnChallengeUpdate;
            ChallengeManager.OnChallengesUpdate -= OnChallengesUpdate;

            if (GameManager.InstanceExists)
                GameManager.Instance.activeGame = null;
        }

        public void LoadGame(IClientFourzy _game)
        {
            CancelRoutine("gameInit");
            CancelRoutine("turnBaseTurn");

            awaitingChallengeID = "";
            gameStarted = false;

            //if active game is empty, load random pass&play board
            if (_game == null)
            {
                ClientFourzyGame newGame = new ClientFourzyGame(GameContentManager.Instance.passAndPlayDataHolder.random, UserManager.Instance.meAsPlayer, new Player(2, "Player Two"));
                newGame._Type = GameType.PASSANDPLAY;

                GameManager.Instance.activeGame = newGame;
            }
            else if (_game != GameManager.Instance.activeGame)
                GameManager.Instance.activeGame = _game;

            game = GameManager.Instance.activeGame;

            //manage bg audio
            switch (game._Type)
            {
                case GameType.REALTIME:
                    gameplayBGAudio = AudioHolder.instance.PlayBGAudio(AudioTypes.BG_GARDEN_REALTIME, true, .9f, 3f);
                    break;

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

            switch (game._Type)
            {
                case GameType.TURN_BASED:
                    //if game was over and havent viewed yet, set it to viewed
                    if (game.asFourzyGame.challengeData.lastTurnGame.isOver && !PlayerPrefsWrapper.GetGameViewed(game.GameID))
                    {
                        PlayerPrefsWrapper.SetGameViewed(game.GameID);

                        ChallengeManager.OnChallengeUpdateLocal.Invoke(game.asFourzyGame.challengeData);
                    }
                    break;

                case GameType.PUZZLE:
                    //set this puzzlepack as opened
                    PlayerPrefsWrapper.SetPuzzlePackOpened(game.asFourzyPuzzle.puzzlePack.packID);

                    break;
            }

            //close loading prompt if opened
            if (loadingPrompt && loadingPrompt.isOpened) menuController.CloseCurrentScreen();

            //close info screen if opened
            if (gameInfoScreen.isOpened) gameInfoScreen.Close();
            winningParticleGenerator.HideParticles();

            //unload old bg
            if (bg) Destroy(bg.gameObject);

            playerPickScreen.SetData(game);

            currentConfiguration = GameContentManager.Instance.themesDataHolder.GetThemeBGConfiguration(game._Area, Camera.main);
            bg = Instantiate(currentConfiguration.backgroundPrefab, bgParent);
            bg.transform.localPosition = Vector3.zero;

            if (board) Destroy(board.gameObject);

            board = Instantiate(currentConfiguration.gameboardPrefab, transform);
            board.Initialize(game);
            board.transform.localPosition = currentConfiguration.gameboardPrefab.transform.position;
            board.interactable = false;

            board.onGameFinished += OnGameFinished;
            board.onDraw += OnDraw;
            board.onMoveStarted += OnMoveStarted;
            board.onMoveEnded += OnMoveEnded;
            board.onWrongTurn += () => notYourTurnLabel.StartTimer();

            //hide tokens/gamepieces
            board.FadeTokens(0f, 0f);
            board.FadeGamepieces(0f, 0f);

            gameplayScreen.InitUI(this);
            OnNetwork(NetworkAccess.ACCESS);

            StartRoutine("gameInit", GameInitRoutine());
        }

        public void OnPointerDown(Vector2 position)
        {
            //only continue if current opened screen is GameplayScreen
            if (gameplayScreen != menuController.currentScreen)
                return;

            board.OnPointerDown(position);
        }

        public void OnPointerMove(Vector2 position)
        {
            //release controls if current screen isnt GameplayScreen
            if (gameplayScreen != menuController.currentScreen)
            {
                OnPointerRelease(position);
                return;
            }

            board.OnPointerMove(position);
        }

        public void OnPointerRelease(Vector2 position)
        {
            board.OnPointerRelease(position);
        }

        /// <summary>
        /// Triggered after board/tokens fade id + optional delays
        /// </summary>
        public void OnGameStarted()
        {
            board.interactable = true;
            game.SetInitialTime(Time.time);
        }

        public void UpdatePlayerTurn()
        {
            if (game == null) return;

            if (!gameStarted)
            {
                gameStarted = true;
                OnGameStarted();
            }

            //extra
            switch (game._Type)
            {
                case GameType.AI:
                    //AI turn
                    if (!game.isOver && !game.isMyTurn) board.TakeAITurn();

                    break;

                case GameType.PUZZLE:
                    if (!game.isOver)
                    {
                        //AI turn
                        if (!game.isMyTurn) board.TakeAITurn();
                    }
                    else
                    {
                        //player lost
                        if (game._State.WinningLocations == null)
                            OnGameFinished(game);
                    }

                    break;
            }

            gameplayScreen.UpdatePlayerTurn();
        }

        private IEnumerator ShowTokenInstructionPopupRoutine()
        {
            if (game == null || game._Type == GameType.ONBOARDING)
                yield break;

            HashSet<TokensDataHolder.TokenData> tokens = new HashSet<TokensDataHolder.TokenData>();

            foreach (BoardSpace boardSpace in game.boardContent)
                foreach (IToken token in boardSpace.Tokens.Values)
                    if (!PlayerPrefsWrapper.InstructionPopupWasDisplayed((int)token.Type))
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
            AudioHolder.instance.StopBGAudio(gameplayBGAudio, .5f);

            GameManager.Instance.OpenMainMenu();

            //disconnect
            if (game._Type == GameType.REALTIME)
                RealtimeManager.Instance.GetRTSession().Disconnect();
        }

        public void UnloadGamePlayScreen()
        {
            SceneManager.UnloadSceneAsync(Constants.GAMEPLAY_SCENE_NAME);
        }

        public void Rematch()
        {
            switch (game._Type)
            {
                case GameType.TURN_BASED:
                    if (game == null) return;

                    loadingPrompt = menuController.GetScreen<LoadingPromptScreen>();

                    loadingPrompt.Prompt("", "Loading new game...", null, null, null, null);

                    ChallengeManager.Instance.CreateTurnBasedGame(game.opponent.PlayerString, game._Area, CreateTurnBasedGameSuccess, CreateTurnBasedGameError);
                    
                    break;

                case GameType.AI:
                case GameType.PASSANDPLAY:
                case GameType.PUZZLE:
                    if (game == null) return;

                    game.Reset();
                    LoadGame(game);

                    break;
            }
        }

        #region Turn Base Calls

        private void OnChallengeUpdate(ChallengeData gameData)
        {
            StartRoutine("turnBaseTurn", PlayTurnBaseTurn(gameData));
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
            AnalyticsManager.LogError("create_turn_based_error", response.Errors.JSON);

            if (loadingPrompt && loadingPrompt.isOpened)
            {
                loadingPrompt.promptText.text = "Failed to create new game...\n" + response.Errors.JSON;
                StartRoutine("closingLoadingPrompt", 5f, () => menuController.CloseCurrentScreen());
            }
        }

        #endregion

        private void OnGameFinished(IClientFourzy game)
        {
            onGameFinished?.Invoke(game);

            AnalyticsManager.LogGameOver(game);

            gameplayScreen.OnGameFinished();
            gameInfoScreen.SetData(game);

            StartCoroutine(PostGameFinished(game));
        }

        private void OnDraw(IClientFourzy game)
        {
            gameInfoScreen.Open(LocalizationManager.Instance.GetLocalizedValue("draw_text"), "");
        }

        private void OnMoveStarted(int playerID)
        {
            onMoveStarted?.Invoke(playerID);
        }

        private void OnMoveEnded(int playerID)
        {
            if (replayingLastTurn) replayingLastTurn = false;

            onMoveEnded?.Invoke(playerID);

            UpdatePlayerTurn();
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
            if (state)
                noNetworkOverlay.SetActive(false);
        }

        private IEnumerator GameInitRoutine()
        {
            if (game == null) yield break;

            yield return StartCoroutine(FadeGameScreen(1f, .5f));
            yield return StartCoroutine(ShowTokenInstructionPopupRoutine());

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

                default:
                    UpdatePlayerTurn();
                    break;
            }

            isBoardReady = true;
        }

        private IEnumerator PlayTurnBaseTurn(ChallengeData gameData)
        {
            if (gameData.lastTurnGame._Type != GameType.TURN_BASED) yield break;

            yield return new WaitUntil(() => !board.isAnimating && isBoardReady);

            PlayerTurn lastTurn = gameData.lastTurn;

            if (game.asFourzyGame.playerTurnRecord.Count > 0 && game.asFourzyGame.playerTurnRecord[game.asFourzyGame.playerTurnRecord.Count - 1].PlayerId == lastTurn.PlayerId) yield break;

            //compare challenges
            if (gameData.challengeInstanceId != game.asFourzyGame.challengeData.challengeInstanceId) yield break;

            board.TakeTurn(lastTurn.GetMove(), true);
            UpdatePlayerTurn();
        }

        private IEnumerator PostGameFinished(IClientFourzy game)
        {
            //rewards screen
            switch (game._Type)
            {
                case GameType.PASSANDPLAY:
                    break;

                //show rewards screen
                case GameType.PUZZLE:
                case GameType.TURN_BASED:
                    //if (!PlayerPrefsWrapper.GetGameRewarded(game.GameID))
                    //    PersistantMenuController.instance.GetScreen<RewardsScreen>().SetData(game.asSubject);
                    break;
            }

            yield return new WaitForSeconds(0.5f);

            //visuals + haptic
            switch (game._Type)
            {
                case GameType.ONBOARDING:
                    break;

                case GameType.FRIEND:
                case GameType.LEADERBOARD:
                case GameType.PUZZLE:
                case GameType.TURN_BASED:
                    if (game.IsWinner())
                    {
                        winningParticleGenerator.ShowParticles();
                        AudioHolder.instance.PlaySelfSfxOneShotTracked(AudioTypes.GAME_TURNBASED_WON);

                        GameManager.Vibrate();
                    }
                    else
                        AudioHolder.instance.PlaySelfSfxOneShotTracked(AudioTypes.GAME_TURNBASED_LOST);
                    break;

                case GameType.REALTIME:
                    if (game.IsWinner())
                    {
                        winningParticleGenerator.ShowParticles();
                        AudioHolder.instance.PlaySelfSfxOneShotTracked(AudioTypes.GAME_WON);

                        GameManager.Vibrate();
                    }
                    break;

                default:
                    winningParticleGenerator.ShowParticles();
                    AudioHolder.instance.PlaySelfSfxOneShotTracked(AudioTypes.GAME_WON);

                    GameManager.Vibrate();
                    break;
            }

            //sound
            switch (game._Type)
            {
                case GameType.ONBOARDING:
                case GameType.PUZZLE:
                case GameType.PASSANDPLAY:
                    AudioHolder.instance.PlaySelfSfxOneShotTracked(AudioTypes.GAME_WON);
                    break;

                case GameType.FRIEND:
                case GameType.LEADERBOARD:
                case GameType.TURN_BASED:
                    if (game.IsWinner())
                        AudioHolder.instance.PlaySelfSfxOneShotTracked(AudioTypes.GAME_TURNBASED_WON);
                    else
                        AudioHolder.instance.PlaySelfSfxOneShotTracked(AudioTypes.GAME_TURNBASED_LOST);
                    break;

                case GameType.REALTIME:
                    if (game.IsWinner())
                        AudioHolder.instance.PlaySelfSfxOneShotTracked(AudioTypes.GAME_WON);
                    else
                        AudioHolder.instance.PlaySelfSfxOneShotTracked(AudioTypes.GAME_LOST);
                    break;
            }
        }
    }
}

