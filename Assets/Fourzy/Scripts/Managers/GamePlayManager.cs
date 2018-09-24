using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using GameSparks.Api.Requests;
using UnityEngine.SceneManagement;
using TMPro;

namespace Fourzy
{
    [UnitySingleton(UnitySingletonAttribute.Type.ExistsInScene, true)]
    public class GamePlayManager : UnitySingleton<GamePlayManager> 
    {
        public Game game;

        private bool isLoading = false;
        public bool isLoadingUI = false;
        private bool isDropping = false;
        public bool disableInput = false;
        private bool replayedLastMove = false;
        private float gameScreenFadeInTime = 0.7f;
        private DateTime serverClock;
        private DateTime playerMoveCountdown;
        private bool clockStarted = false;
        private int playerMoveCountDown_LastTime;

        private GameObject playerGamePiecePrefab;
        private GameObject opponentGamePiecePrefab;

        [Header("Game UI")]
        public Image backgroundImage;
        public CanvasGroup fadeUICanvasGroup;
        public Rewards rewardScreen;
        public GameBoardView gameBoardView;
        public PlayerUIPanel playerUIPanel;
        public PlayerUIPanel opponentUIPanel;
        public WinningParticleGenerator winningParticleGenerator;

        public Text ratingDeltaText;
        public GameInfo gameInfo;
        public AlertUI alertUI;
        public Button rematchButton;
        public Button nextGameButton;
        public Button createGameButton;
        public Button retryPuzzleChallengeButton;
        public Button nextPuzzleChallengeButton;
        public Button rewardsButton;
        public Button backButton;
        public GameObject backButtonObject;
        public Button resignButton;
        public GameObject moveHintAreaObject;
        public GameIntroUI gameIntroUI;
        public Text challengeIdDebugText;
        public TextMeshProUGUI timerText;
        public GameObject playerTimer;

        public delegate void Resign(string challengeInstanceId);
        public static event Resign OnResign;
        public delegate void StartMove();
        public static event StartMove OnStartMove;
        public delegate void EndMove();
        public static event EndMove OnEndMove;
        public delegate void GameOver();
        public static event GameOver OnGameOver;
        public delegate void PuzzleCompleted(PuzzleChallengeLevel puzzleChallengeLevel);
        public static event PuzzleCompleted OnPuzzleCompleted;
        public delegate void GamePlayMessage(string message);
        public static event GamePlayMessage OnGamePlayMessage;
        public AudioClip clipMove;
        public AudioClip clipWin;
        public Color bluePlayerColor = new Color(0f / 255f, 176.0f / 255f, 255.0f / 255.0f);
        public Color redPlayerColor = new Color(254.0f / 255.0f, 40.0f / 255.0f, 81.0f / 255.0f);
        private string playerOneWonText = "Player 1 Wins!";
        private string playerTwoWonText = "Player 2 Wins!";

        IEnumerator Start () 
        {
            game = GameManager.instance.activeGame;

            //TODO: Create a game if activeGame is null

            //SoundManager.instance.Mute(true);
            UserInputHandler.inputEnabled = false;
            replayedLastMove = false;
            // timerText = GetComponent<TextMeshProUGUI>();

            InitButtonListeners();
            SetActionButton();
            InitPlayerPrefabs();
            ResetUI();
            CreateGamePieceViews();
            CreateTokenViews();
            InitPlayerUI();
            InitIntroUI();

            if (game.isCurrentPlayer_PlayerOne)
            {
                ratingDeltaText.text = game.challengerRatingDelta.ToString();
            }
            else
            {
                ratingDeltaText.text = game.challengedRatingDelta.ToString();
            }

            challengeIdDebugText.text = "ChallengeId: " + game.challengeId;

            if (GameManager.instance.shouldLoadOnboarding) {
                GameManager.instance.shouldLoadOnboarding = false;
                GameManager.instance.onboardingScreen.StartOnboarding();
            }

            if (game.gameState.GameType == GameType.REALTIME) {
                playerTimer.SetActive(true);
                StartCoroutine(SendTimeStamp());
            }

            playerMoveCountDown_LastTime = 30000;

            playerUIPanel.StopPlayerTurnAnimation();
            opponentUIPanel.StopPlayerTurnAnimation();

            yield return StartCoroutine(FadeGameScreen(0.0f, 1.0f, gameScreenFadeInTime));
            yield return StartCoroutine(ReplayLastMove());
            yield return StartCoroutine(ShowTokenInstructionPopupRoutine());
            yield return StartCoroutine(ShowPlayTurnWithDelay(0.3f));

            StartCoroutine(RandomGamePiecesBlinkingRoutine());

            UserInputHandler.inputEnabled = true;
        }

        private void OnEnable()
        {
            UserInputHandler.OnTap += ProcessPlayerInput;
        }

        private void OnDisable()
        {
            UserInputHandler.OnTap -= ProcessPlayerInput;
        }

        /// <summary>
        /// Sends a Unix timestamp in milliseconds to the server
        /// </summary>
        private IEnumerator SendTimeStamp() {
            RealtimeManager.Instance.SendTimeStamp();
            yield return new WaitForSeconds(5f);
            StartCoroutine(SendTimeStamp());
        }

        /// <summary>
        /// Syncs the local clock to server-time
        /// </summary>
        /// <param name="_packet">Packet.</param>
        public void SyncClock(long milliseconds){
            DateTime dateNow = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc); // get the current time
            serverClock = dateNow.AddMilliseconds(milliseconds + RealtimeManager.Instance.timeDelta).ToLocalTime(); // adjust current time to match clock from server

            if (game.gameState.isCurrentPlayerTurn) {
                if (!clockStarted) { // make sure that we only calculate the endtime once
                    // playerMoveCountdown = serverClock.AddMilliseconds(playerMoveCountDown_LastTime + RealtimeManager.Instance.timeDelta); // endtime is 60seconds plus the time-offset
                    playerMoveCountdown = serverClock.AddMilliseconds(playerMoveCountDown_LastTime + RealtimeManager.Instance.timeDelta); // endtime is 60seconds plus the time-offset
                    clockStarted = true;
                }

                // set the timer each time a new update from the server comes in
                Debug.Log("Total miliseconds: " + (playerMoveCountdown-serverClock).TotalMilliseconds);
                TimeSpan timeDifference = playerMoveCountdown-serverClock;
                timerText.text = new System.DateTime(timeDifference.Ticks).ToString("m:ss");
                // .Minutes.ToString() + ":" + (playerMoveCountdown-serverClock).Seconds.ToString();
                if((playerMoveCountdown-serverClock).TotalMilliseconds <= 0){ // if the time is out, return to the lobby
                    Debug.Log ("player ran out of time to make a move!");
                    // RealtimeManager.Instance.GetRTSession().Disconnect();
                }
            } else {
                // playerMoveCountdown = serverClock;
            }
        }

        private void InitButtonListeners()
        {
            Button backBtn = backButton.GetComponent<Button>();
            backBtn.onClick.AddListener(BackButtonOnClick);

            Button rematchBtn = rematchButton.GetComponent<Button>();
            rematchBtn.onClick.AddListener(RematchPassAndPlayGameButtonOnClick);

            Button nextGameBtn = nextGameButton.GetComponent<Button>();
            nextGameBtn.onClick.AddListener(NextGameButtonOnClick);

            Button createGameBtn = createGameButton.GetComponent<Button>();
            createGameBtn.onClick.AddListener(CreateGameButtonOnClick);

            Button nextPuzzleChallengeBtn = nextPuzzleChallengeButton.GetComponent<Button>();
            nextPuzzleChallengeBtn.onClick.AddListener(NextPuzzleChallengeButtonOnClick);

            Button retryPuzzleChallengeBtn = retryPuzzleChallengeButton.GetComponent<Button>();
            retryPuzzleChallengeBtn.onClick.AddListener(RetryPuzzleChallengeButtonOnClick);

            Button rewardsBtn = rewardsButton.GetComponent<Button>();
            rewardsButton.onClick.AddListener(RewardsButtonOnClick);
        }

        private void ResetUI()
        {
            gameInfo.Close();
            rematchButton.gameObject.SetActive(false);
            nextGameButton.gameObject.SetActive(false);
            createGameButton.gameObject.SetActive(false);
            nextPuzzleChallengeButton.gameObject.SetActive(false);
            retryPuzzleChallengeButton.gameObject.SetActive(false);
            backButton.gameObject.SetActive(true);
            moveHintAreaObject.SetActive(false);
            gameIntroUI.Close();
        }

        private void InitIntroUI() 
        {
            if (!game.displayIntroUI)
            {
                return;
            }

            string title = game.title;
            string subtitle = game.subtitle;
            if (subtitle == "") 
            {
                switch (game.gameState.GameType)
                {
                    case GameType.PASSANDPLAY:
                        subtitle = LocalizationManager.Instance.GetLocalizedValue("pnp_button");
                        break;
                    case GameType.FRIEND:
                        subtitle = LocalizationManager.Instance.GetLocalizedValue("friend_challenge_text");
                        break;
                    case GameType.LEADERBOARD:
                        subtitle = LocalizationManager.Instance.GetLocalizedValue("leaderboard_challenge_text");
                        break;
                    case GameType.RANDOM:
                        subtitle = LocalizationManager.Instance.GetLocalizedValue("random_opponent_button");
                        break;
                    case GameType.AI:
                        subtitle = LocalizationManager.Instance.GetLocalizedValue("ai_challenge_text");
                        break;
                    default:
                        break;
                }
            }

            if (title == "") 
            {
                title = game.gameState.TokenBoard.name;
            }

            DisplayIntroUI(title, subtitle, true);
        }

        private void DisplayIntroUI(string title, string subtitle, bool fade) 
        {
            if(string.IsNullOrEmpty(title) || string.IsNullOrEmpty(subtitle))
            {
                return;
            }

            gameIntroUI.Open(title, subtitle, fade);
        }

        private IEnumerator ShowTokenInstructionPopupRoutine()
        {
            if (!game.displayIntroUI)
            {
                yield break;
            }

            yield return new WaitForSeconds(0.5f);

            TokenBoard previousTokenBoard = game.gameState.PreviousTokenBoard;
            IToken[,] previousTokenBoardTokens = previousTokenBoard.tokens;

            HashSet<TokenData> tokens = new HashSet<TokenData>();

            for (int i = 0; i < Constants.numRows; i++)
            {
                for (int j = 0; j < Constants.numColumns; j++)
                {
                    if (previousTokenBoardTokens[i, j] == null || previousTokenBoardTokens[i, j].tokenType == Token.EMPTY)
                    {
                        continue;
                    }

                    TokenData token = GameContentManager.Instance.GetTokenDataWithType(previousTokenBoardTokens[i, j].tokenType);
                    if (!PlayerPrefsWrapper.InstructionPopupWasDisplayed(token.ID))
                    {
                        tokens.Add(token);
                    }
                }
            }

            TokenPopupUI popupUI = GameManager.instance.tokenPopupUI;

            foreach(TokenData token in tokens)
            {
                popupUI.Open(token);

                yield return new WaitWhile(() => popupUI.IsOpen());

                PlayerPrefsWrapper.SetInstructionPopupDisplayed(token.ID, true);

                yield return new WaitForSeconds(0.5f);
            }
        }

        public void UpdateOpponentUI(Opponent opponent) {
            // TODO: complete this
        }

        private void InitPlayerPrefabs()
        {
            int playerGamePieceId = 0;
            int opponentGamePieceId = 0;

            PlayerEnum player = PlayerEnum.ONE;
            PlayerEnum opponent = PlayerEnum.TWO;

            if (game.gameState.GameType == GameType.REALTIME
                || game.gameState.GameType == GameType.RANDOM
                || game.gameState.GameType == GameType.FRIEND
                || game.gameState.GameType == GameType.LEADERBOARD)
            {
                if (game.gameState.GameType == GameType.REALTIME)
                {
                    playerGamePieceId = UserManager.instance.gamePieceId;
                    opponentGamePieceId = game.opponent.gamePieceId;
                }
                else if (game.isCurrentPlayer_PlayerOne)
                {
                    playerGamePieceId = game.challengerGamePieceId;
                    opponentGamePieceId = game.challengedGamePieceId;
                }
                else
                {
                    playerGamePieceId = game.challengedGamePieceId;
                    opponentGamePieceId = game.challengerGamePieceId;

                    player = PlayerEnum.TWO;
                    opponent = PlayerEnum.ONE;
                }
            }

            // For test
            int numPieces = GameContentManager.Instance.GetGamePieceCount();
            playerGamePieceId = UnityEngine.Random.Range(0, numPieces);
            opponentGamePieceId = UnityEngine.Random.Range(0, numPieces);

            GameObject prefab = GameContentManager.Instance.GetGamePiecePrefab(playerGamePieceId);
            playerGamePiecePrefab = Instantiate(prefab);
            GamePiece gamePiece = playerGamePiecePrefab.GetComponent<GamePiece>();
            gamePiece.player = player;
            gamePiece.gameBoardView = gameBoardView;
            gamePiece.View.UseSecondaryColor(false);
            playerGamePiecePrefab.SetActive(false);

            prefab = GameContentManager.Instance.GetGamePiecePrefab(opponentGamePieceId);
            opponentGamePiecePrefab = Instantiate(prefab);
            gamePiece = opponentGamePiecePrefab.GetComponent<GamePiece>();
            gamePiece.player = player;
            gamePiece.gameBoardView = gameBoardView;
            gamePiece.View.UseSecondaryColor(playerGamePieceId == opponentGamePieceId);
            opponentGamePiecePrefab.SetActive(false);
        }

        private void InitPlayerUI()
        {
            Debug.Log("game.gameState.GameType: " + game.gameState.GameType);
            if (game.gameState.GameType == GameType.REALTIME 
                || game.gameState.GameType == GameType.RANDOM 
                || game.gameState.GameType == GameType.FRIEND 
                || game.gameState.GameType == GameType.LEADERBOARD) 
            {
                string opponentName;
                if (game.opponent != null && game.opponent.opponentName != null && game.opponent.opponentName != "")
                {
                    Debug.Log("game.opponent.opponentName: " + game.opponent.opponentName);
                    opponentName = game.opponent.opponentName;
                }
                else
                {
                    opponentName = LocalizationManager.Instance.GetLocalizedValue("waiting_opponent_text");
                }

                playerUIPanel.SetupPlayerName(UserManager.instance.userName);
                opponentUIPanel.SetupPlayerName(opponentName);
            } 
            else 
            {
                playerUIPanel.SetupPlayerName("Player 1");
                opponentUIPanel.SetupPlayerName("Player 2");
            }

            playerUIPanel.InitPlayerIcon(playerGamePiecePrefab);
            opponentUIPanel.InitPlayerIcon(opponentGamePiecePrefab);
        }

        private void CreateGamePieceViews()
        {
            int[,] board = game.gameState.GetPreviousGameBoard();
            gameBoardView.gamePieces = new GamePiece[Constants.numRows, Constants.numColumns];

            for (int row = 0; row < Constants.numRows; row++)
            {
                for (int col = 0; col < Constants.numColumns; col++)
                {
                    int piece = board[row, col];
                    if (piece == (int)Piece.EMPTY)
                    {
                        continue;
                    }

                    PlayerEnum player = (piece == (int)Piece.BLUE) ? PlayerEnum.ONE : PlayerEnum.TWO;

                    GamePiece pieceObject = SpawnPiece(col, row, player);
                    pieceObject.player = player;
                    gameBoardView.gamePieces[row, col] = pieceObject;
                    pieceObject.View.SetAlpha(0.0f);
                }
            }
        }

        public void CreateStickyToken(int row, int col) 
        {
            GameObject stickyToken = GameContentManager.Instance.GetTokenPrefab(Token.STICKY);
            gameBoardView.CreateToken(row, col, stickyToken);
        }

        private void CreateTokenViews()
        {
            TokenBoard previousTokenBoard = game.gameState.PreviousTokenBoard;
            IToken[,] previousTokenBoardTokens = previousTokenBoard.tokens;
            
            for (int row = 0; row < Constants.numRows; row++)
            {
                for (int col = 0; col < Constants.numColumns; col++)
                {
                    if (previousTokenBoardTokens[row, col] == null || previousTokenBoardTokens[row, col].tokenType == Token.EMPTY)
                    {
                        continue;
                    }

                    Token token = previousTokenBoardTokens[row, col].tokenType;

                    GameObject tokenPrefab = GameContentManager.Instance.GetTokenPrefab(token);

                    if (tokenPrefab) 
                    {
                        gameBoardView.CreateToken(row, col, tokenPrefab);
                        gameBoardView.TokenAt(row, col).GetComponent<SpriteRenderer>().SetAlpha(0.0f);
                    }
                }
            }
        }

        private IEnumerator FadeGameScreen(float startAlpha, float alpha, float fadeTime)
        {
            gameBoardView.SetAlpha(startAlpha);
            backgroundImage.SetAlpha(startAlpha);
            fadeUICanvasGroup.alpha = startAlpha;

            gameBoardView.Fade(alpha, fadeTime);
            backgroundImage.DOFade(alpha, fadeTime);
            fadeUICanvasGroup.DOFade(alpha, fadeTime);

            yield return new WaitForSeconds(fadeTime);

            yield return StartCoroutine(FadeTokens(alpha, fadeTime));
            yield return StartCoroutine(FadePieces(alpha, fadeTime));
        }

        private IEnumerator FadeTokens(float alpha, float fadeTime)
        {
            GameObject[] tokenArray = GameObject.FindGameObjectsWithTag("Token");
            for (int i = 0; i < tokenArray.Length; i++)
            {
                tokenArray[i].GetComponent<SpriteRenderer>().DOFade(alpha, fadeTime);
            }

            yield return new WaitForSeconds(fadeTime);
        }

        private IEnumerator FadePieces(float alpha, float fadeTime)
        {
            SoundManager.instance.Mute(false);

            var pieces = gameBoardView.GetGamePiecesList();
            for (int i = 0; i < pieces.Count; i++)
            {
                pieces[i].View.Fade(alpha, fadeTime);
            }

            if (pieces.Count > 0)
            {
                yield return new WaitForSeconds(fadeTime);
            }
        }

        private IEnumerator ReplayLastMove()
        {
            Debug.Log("ReplayLastMove");
            if (game.gameState.MoveList != null)
            {
                Debug.Log("ReplayLastMove: has move");
                Move lastMove = game.gameState.MoveList.Last();

                PlayerEnum player = PlayerEnum.NONE;
                if (!game.gameState.IsGameOver)
                {
                    player = game.gameState.IsPlayerOneTurn ? PlayerEnum.TWO : PlayerEnum.ONE;
                }
                else
                {
                    player = game.gameState.IsPlayerOneTurn ? PlayerEnum.ONE : PlayerEnum.TWO;
                }

                lastMove.player = player;

                yield return StartCoroutine(MovePiece(lastMove, true, false));
            }
            else if (game.gameState.TokenBoard.initialMoves.Count > 0)
            {
                //StartCoroutine(PlayInitialMoves());
            } else {
                isLoading = false;
            }
        }

        public IEnumerator ReplayIncomingOpponentMove(Move move)
        {
            // int position = lastMove.GetInt("position").GetValueOrDefault();
            // Direction direction = (Direction)lastMove.GetInt("direction").GetValueOrDefault();
            // Move move = new Move(position, direction, player);

            while (isDropping || isLoading)
                yield return null;
            StartCoroutine(MovePiece(move, false, true));
        }

        public void BackButtonOnClick() {
            // SceneManager.LoadScene("tabbedUI");

            if (ViewController.instance.GetCurrentView() != null) {
                ViewController.instance.ChangeView(ViewController.instance.GetCurrentView());
                // Debug.Log("BackButtonOnClick: " + ViewController.instance.GetCurrentView().name);
                if (ViewController.instance.GetCurrentView() != ViewController.instance.viewTraining && ViewController.instance.GetCurrentView() != ViewController.instance.viewPuzzleSelection)
                {
                    ViewController.instance.ShowTabView();
                }
            } else if (ViewController.instance.GetPreviousView() != null) {
                ViewController.instance.ChangeView(ViewController.instance.GetPreviousView());
            }

            ChallengeManager.instance.ReloadGames();

            Scene uiScene = SceneManager.GetSceneByName("tabbedUI");
            SceneManager.SetActiveScene(uiScene);
            SceneManager.UnloadSceneAsync("gamePlay");

            if (game.gameState.GameType == GameType.REALTIME) {
                RealtimeManager.Instance.GetRTSession().Disconnect();
            }
        }

        public void UnloadGamePlayScreen() {
            SceneManager.UnloadSceneAsync("gamePlay");
        }

        public void ResignButtonOnClick() {
            if (OnResign != null)
            {
                OnResign(game.challengeId);
                //resignButton.gameObject.SetActive(false);
            }
        }

        public void RewardsButtonOnClick()
        {
            GameManager.instance.VisitedGameResults(game);
            UserInputHandler.inputEnabled = false;

            bool isCurrentPlayerWinner = false;
            PlayerEnum currentPlayer = PlayerEnum.NONE;

            if (game.isCurrentPlayer_PlayerOne && game.gameState.Winner == PlayerEnum.ONE)
            {
                isCurrentPlayerWinner = true;
            }
            else if (!game.isCurrentPlayer_PlayerOne && game.gameState.Winner == PlayerEnum.TWO)
            {
                isCurrentPlayerWinner = true;
            }
            else if (game.isCurrentPlayer_PlayerOne && game.gameState.Winner == PlayerEnum.TWO)
            {
                isCurrentPlayerWinner = false;
            }
            else if (!game.isCurrentPlayer_PlayerOne && game.gameState.Winner == PlayerEnum.ONE)
            {
                isCurrentPlayerWinner = false;
            }
            else if (game.gameState.Winner == PlayerEnum.ALL || game.gameState.Winner == PlayerEnum.NONE)
            {
                isCurrentPlayerWinner = false;
            }

            if (game.isCurrentPlayer_PlayerOne)
            {
                currentPlayer = PlayerEnum.ONE;
            }
            else
            {
                currentPlayer = PlayerEnum.TWO;
            }

            //Debug.Log("RewardsButton gameState.PlayerPieceCount(currentPlayer): " + gameState.PlayerPieceCount(currentPlayer));
            rewardScreen.Open(isCurrentPlayerWinner, game.gameState.PlayerPieceCount(currentPlayer));
            gameInfo.Close();
        }

        public void NextGameButtonOnClick() {
            backgroundImage.SetAlpha(0.0f);
            fadeUICanvasGroup.alpha = 0.0f;
            GameManager.instance.OpenNextGame();
        }


        public void CreateGameButtonOnClick()
        {
            Scene uiScene = SceneManager.GetSceneByName("tabbedUI");
            SceneManager.SetActiveScene(uiScene);
            SceneManager.UnloadSceneAsync("gamePlay");

            // Game game = GameManager.instance.GetNextActiveGame();
            // if (game != null)
            // {
            //     game.OpenGame();
            // }
            // else
            // {
                ViewController.instance.ChangeView(ViewController.instance.viewMatchMaking);
            // }
        }

        public void RewardsScreenOkButtonOnClick()
        {
            rewardScreen.gameObject.SetActive(false);
        }

        public void RematchPassAndPlayGameButtonOnClick()
        {
            Scene uiScene = SceneManager.GetSceneByName("tabbedUI");
            SceneManager.SetActiveScene(uiScene);
            SceneManager.UnloadSceneAsync("gamePlay");
            ViewController.instance.ChangeView(ViewController.instance.viewGameboardSelection);
            ViewGameBoardSelection.instance.TransitionToViewGameBoardSelection(game.gameState.GameType);
            AnalyticsManager.LogCustom("rematch_pnp_game");
        }

        public void RetryPuzzleChallengeButtonOnClick() {
            Scene uiScene = SceneManager.GetSceneByName("tabbedUI");
            SceneManager.SetActiveScene(uiScene);
            SceneManager.UnloadSceneAsync("gamePlay");
            GameManager.instance.OpenPuzzleChallengeGame("retry");
        }

        public void NextPuzzleChallengeButtonOnClick() {
            Scene uiScene = SceneManager.GetSceneByName("tabbedUI");
            SceneManager.SetActiveScene(uiScene);
            SceneManager.UnloadSceneAsync("gamePlay");
            GameManager.instance.OpenPuzzleChallengeGame("next");
        }

        /// <summary>
        /// Spawns a gamepiece at the given column and row
        /// </summary>
        /// <returns>The piece.</returns>
        GamePiece SpawnPiece(int col, int row, PlayerEnum player)
        {
            GameObject gamePiecePrefab;
            if (player == PlayerEnum.ONE && game.isCurrentPlayer_PlayerOne)
            {
                gamePiecePrefab = playerGamePiecePrefab;
            }
            else 
            {
                gamePiecePrefab = opponentGamePiecePrefab;
            }

            return gameBoardView.SpawnPiece(row, col, gamePiecePrefab);
        }

        private IEnumerator ShowPlayTurnWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            UpdatePlayerTurn();
        }

        private void UpdatePlayerTurn() 
        {
            if (game.gameState.IsPlayerOneTurn == game.isCurrentPlayer_PlayerOne)
            {
                playerUIPanel.ShowPlayerTurnAnimation();
                opponentUIPanel.StopPlayerTurnAnimation();
            } 
            else if (game.gameState.IsPlayerOneTurn != game.isCurrentPlayer_PlayerOne)
            {
                opponentUIPanel.ShowPlayerTurnAnimation();
                playerUIPanel.StopPlayerTurnAnimation();
            }
        }

        public void SetActionButton()
        {
            var games = GameManager.instance.games;

            if (game.gameState.GameType == GameType.RANDOM || game.gameState.GameType == GameType.FRIEND || game.gameState.GameType == GameType.LEADERBOARD)
            {
                bool hasNextGame = false;

                for (int i = 0; i < games.Count(); i++)
                {
                    if (games[i].gameState != null) {
                        if ((games[i].gameState.isCurrentPlayerTurn == true || (games[i].didViewResult == false && games[i].gameState.IsGameOver == true && games[i].gameState.isCurrentPlayerTurn == false)) && games[i].challengeId != game.challengeId)
                        {
                            hasNextGame = true;
                        }
                    } else {
                        AnalyticsManager.LogError("set_action_button_error", "GameState is null for challengeId: " + games[i].challengeId);
                    }
                }

                if (hasNextGame) {
                    nextGameButton.gameObject.SetActive(true);
                    createGameButton.gameObject.SetActive(false);
                } else {
                    createGameButton.gameObject.SetActive(true);
                    nextGameButton.gameObject.SetActive(false);
                }
            }
            else
            {
                if (game.gameState.GameType == GameType.PUZZLE) {
                    if (game.gameState.IsPuzzleChallengeCompleted) {
                        if (game.gameState.IsPuzzleChallengePassed) {
                            nextPuzzleChallengeButton.gameObject.SetActive(true);
                        } else {
                            retryPuzzleChallengeButton.gameObject.SetActive(true);
                        }
                    }
                } else {
                    if (game.gameState.IsGameOver && !GameManager.instance.isOnboardingActive) {
                        // Debug.Log("set rematch button active");
                        rematchButton.gameObject.SetActive(true);
                    }
                }
            }
        }

        public IEnumerator PlayInitialMoves() {
            // Debug.Log("PlayInitialMoves");
            List<MoveInfo> initialMoves = game.gameState.TokenBoard.initialMoves;

            for (int i = 0; i < initialMoves.Count; i++)
            {
                Move move = new Move(initialMoves[i].Location, (Direction)initialMoves[i].Direction, i % 2 == 0 ? PlayerEnum.ONE : PlayerEnum.TWO);
                StartCoroutine(MovePiece(move, false, false));
                yield return new WaitWhile(() => isDropping == true);
            }

            isLoading = false;
        }

        private void ProcessPlayerInput(Vector3 mousePosition)
        {
            Debug.Log("Gamemanager: disableInput: " + disableInput + ", isLoading: " + isLoading + ", isLoadingUI: " + isLoadingUI + ",isDropping: " + isDropping + ", numPiecesAnimating: " + gameBoardView.NumPiecesAnimating + ", isGameOver: "+ game.gameState.IsGameOver);

            Vector3 pos = Camera.main.ScreenToWorldPoint(mousePosition);
            //GameObject particle = SpawnParticle();
            //particle.transform.position = new Vector3(Mathf.RoundToInt(pos.x), Mathf.CeilToInt((pos.y * -1 - .3f))*-1);

            if (disableInput || isLoading || isLoadingUI || isDropping || game.gameState.IsGameOver || gameBoardView.NumPiecesAnimating > 0)
            {
                Debug.Log("returned in process player input");
                return;
            }

            Position position = gameBoardView.Vec3ToPosition(pos);

            if (game.gameState.isCurrentPlayerTurn)
            {                
                PlayerEnum player = game.gameState.IsPlayerOneTurn ? PlayerEnum.ONE : PlayerEnum.TWO;
                Debug.Log("ProcessPlayerInput: column: " + position.column + "   row = " + position.row);
                if (position.IsTopRow())
                {
                    position.row -= 1;
                    Move move = new Move(position, Direction.DOWN, player);
                    StartCoroutine(ProcessMove(move));
                }
                else if (position.IsBottomRow())
                {
                    position.row += 1;
                    Move move = new Move(position, Direction.UP, player);
                    StartCoroutine(ProcessMove(move));
                }
                else if (position.IsRightColumn())
                {
                    position.column += 1;
                    Move move = new Move(position, Direction.LEFT, player);
                    StartCoroutine(ProcessMove(move));
                }
                else if (position.IsLeftColumn())
                {
                    position.column -= 1;
                    Move move = new Move(position, Direction.RIGHT, player);
                    StartCoroutine(ProcessMove(move));
                }
            }
            else
            {
                Debug.Log("Not isCurrentPlayerTurn: challengeInstanceId: " + game.challengeId);
                if (game.challengeId != null) 
                {
                    if (position.IsTopRow() || position.IsBottomRow() || position.IsLeftColumn() || position.IsRightColumn())
                    {
                        alertUI.Open(LocalizationManager.Instance.GetLocalizedValue("not_your_turn"));
                    }
                }
            }
        }

        public IEnumerator ProcessMove(Move move)
        {
            Debug.Log("ProcessMove");
            isDropping = true;
            bool updatePlayer = true;

            if (game.gameState.CanMove(move.GetNextPosition(), game.gameState.TokenBoard.tokens))
            {
                moveHintAreaObject.SetActive(false);
                Debug.Log("game.challengeId: " + game.challengeId);
                // Console.Log("isMultiplayer: ", isMultiplayer, "isNewChallenge", isNewChallenge, "isNewRandomChallenge", isNewRandomChallenge);
                // if (isMultiplayer && !isNewChallenge && !isNewRandomChallenge)
                if (game.challengeId != null && (game.gameState.GameType == GameType.RANDOM || game.gameState.GameType == GameType.FRIEND || game.gameState.GameType == GameType.LEADERBOARD))
                {
                    replayedLastMove = false;
                    StartCoroutine(MovePiece(move, false, updatePlayer));
                    Debug.Log("LogChallengeEventRequest: challengeInstanceId: " + game.challengeId);
                    new LogChallengeEventRequest().SetChallengeInstanceId(game.challengeId)
                        .SetEventKey("takeTurn") //The event we are calling is "takeTurn", we set this up on the GameSparks Portal
                        .SetEventAttribute("pos", Utility.GetMoveLocation(move)) // pos is the row or column the piece was placed at depending on the direction
                        .SetEventAttribute("direction", move.direction.GetHashCode()) // direction can be up, down, left, right
                        .SetEventAttribute("player", game.gameState.IsPlayerOneTurn ? (int)Piece.BLUE : (int)Piece.RED)
                        .SetDurable(true)
                        .Send((response) =>
                            {
                                if (response.HasErrors)
                                {
                                    Debug.Log("***** ChallengeEventRequest failed: " + response.Errors.JSON);
                                    // alertUI.Open("Server Error: " + response.Errors.JSON);
                                    OnGamePlayMessage("Error: " + response.Errors.JSON);
                                }
                                else
                                {
                                    Debug.Log("ChallengeEventRequest was successful");
                                    OnGamePlayMessage("Move was successful");
                                }
                            });

                    while (isDropping)
                        yield return null;
                }
                else if (game.gameState.GameType == GameType.REALTIME)
                {
                    replayedLastMove = false;
                    StartCoroutine(MovePiece(move, false, updatePlayer));
                    move.player = game.isCurrentPlayer_PlayerOne ? PlayerEnum.ONE : PlayerEnum.TWO;
                    Debug.Log("Send Realtime move");
                    RealtimeManager.Instance.SendRealTimeMove(move);
                    while (isDropping) {
                        yield return null;
                    }

                    playerMoveCountDown_LastTime = (int)(playerMoveCountdown - serverClock).TotalMilliseconds + 15000;
                    Debug.Log("playerMoveCountDown_LastTime millisecond: "+ playerMoveCountDown_LastTime);
                    
                    TimeSpan ts = playerMoveCountdown - serverClock;
                    TimeSpan ts2 = ts.Add(TimeSpan.FromMilliseconds(15000));
                    timerText.text = new System.DateTime(ts2.Ticks).ToString("m:ss");

                    // timerText.text = (playerMoveCountDown_LastTime/1000).ToString();
                    clockStarted = false;
                }
                // else if (isMultiplayer && isNewChallenge)
                else if (game.gameState.GameType == GameType.FRIEND || game.gameState.GameType == GameType.LEADERBOARD)
                {
                    // isNewChallenge = false;
                    StartCoroutine(MovePiece(move, false, updatePlayer));
                    ChallengeManager.instance.ChallengeUser(game, Utility.GetMoveLocation(move), move.direction);
                }
                // else if (isMultiplayer && isNewRandomChallenge)
                else if (game.gameState.GameType == GameType.RANDOM)
                {
                    // isNewRandomChallenge = false;
                    StartCoroutine(MovePiece(move, false, updatePlayer));
                    ChallengeManager.instance.ChallengeRandomUser(game, Utility.GetMoveLocation(move), move.direction);
                }
                else if (game.gameState.GameType == GameType.AI)
                {
                    StartCoroutine(MovePiece(move, false, updatePlayer));

                    if (game.gameState.IsGameOver)
                    {
                        yield break;
                    }

                    while (isDropping)
                        yield return null;
                    isDropping = true;
                    // yield return new WaitForSeconds(0.5f);

                    AiPlayer aiPlayer = new AiPlayer(AIPlayerSkill.LEVEL1);

                    StartCoroutine(aiPlayer.MakeMove(move));
                }
                else
                {
                    StartCoroutine(MovePiece(move, false, updatePlayer));
                    if (game.gameState.GameType == GameType.PUZZLE && !game.gameState.IsGameOver && !game.gameState.IsPuzzleChallengeCompleted) {

                        // yield return new WaitWhile(() => isDropping == true);
                        while (isDropping && gameBoardView.NumPiecesAnimating > 0)
                            yield return null;

                        isDropping = true;
                        // yield return new WaitForSeconds(0.5f);
                        PuzzlePlayer puzzlePlayer = new PuzzlePlayer();

                        StartCoroutine(puzzlePlayer.MakeMove(game));
                    }
                }
            } else {
                alertUI.Open("Nope, not possible");
                isDropping = false;
            }

            yield return null;
        }

        public IEnumerator MovePiece(Move move, bool replayMove, bool updatePlayer)
        {
            isDropping = true;

            if (OnStartMove != null)
                OnStartMove();
            
            SoundManager.instance.PlayRandomizedSfx(clipMove);

            List<IToken> activeTokens;

            game.gameState.PrintGameState("BeforeMove");
            List<MovingGamePiece> movingPieces = game.gameState.MovePiece(move, replayMove, out activeTokens);
            game.gameState.PrintGameState("AfterMove");

            MoveGamePieceViews(move, movingPieces, activeTokens);

            yield return new WaitWhile(() => gameBoardView.NumPiecesAnimating > 0);

            if (!replayMove || game.gameState.IsGameOver || game.gameState.IsPuzzleChallengeCompleted)
            {
                SetActionButton();
            }

            if (game.gameState.IsPuzzleChallengeCompleted)
            {
                if (game.gameState.IsPuzzleChallengePassed)
                {
                    string title = LocalizationManager.Instance.GetLocalizedValue("challenge_completed_title");
                    string subtitle = LocalizationManager.Instance.GetLocalizedValue("challenge_completed_subtitle");
                    DisplayIntroUI(title, subtitle, false);
                }
                else
                {
                    string title = LocalizationManager.Instance.GetLocalizedValue("challenge_failed_title");
                    string subtitle = LocalizationManager.Instance.GetLocalizedValue("challenge_failed_subtitle");
                    DisplayIntroUI(title, subtitle, false);
                }
            }

            if (game.gameState.GameType == GameType.PUZZLE)
            {
                if (game.gameState.IsPuzzleChallengeCompleted)
                {
                    if (game.gameState.IsPuzzleChallengePassed)
                    {
                        Debug.Log("test test: " + PlayerPrefsWrapper.IsPuzzleChallengeCompleted(game.puzzleChallengeInfo.ID));
                        if (PlayerPrefsWrapper.IsPuzzleChallengeCompleted(game.puzzleChallengeInfo.ID))
                        {
                            Debug.Log("Succssfully submitted puzzle complted");
                            ChallengeManager.instance.SubmitPuzzleCompleted();
                        }

                        PlayerPrefsWrapper.SetPuzzleChallengeCompleted(game.puzzleChallengeInfo.ID, true);

                        GameManager.instance.SetNextActivePuzzleLevel();
                        AnalyticsManager.LogPuzzleChallenge(game.puzzleChallengeInfo, true, game.gameState.Player1MoveCount);
                    }
                    else
                    {
                        AnalyticsManager.LogPuzzleChallenge(game.puzzleChallengeInfo, false, game.gameState.Player1MoveCount);
                    }
                }
            }

            UpdateGameStatus(updatePlayer);

            isDropping = false;
            if (replayMove)
            {
                isLoading = false;
            }

            // Retrieve the current game from the list of games and update its state with the changes from the current move
            if (game.gameState.GameType == GameType.RANDOM || game.gameState.GameType == GameType.FRIEND || game.gameState.GameType == GameType.LEADERBOARD) {
                GameManager.instance.UpdateGame(game);
            }

            if (game.gameState.IsGameOver) {
                if (OnGameOver != null)
                    OnGameOver();
            }

            if (OnEndMove != null)
                OnEndMove();
            
            yield return true;
        }

        private void UpdateGameStatus(bool updatePlayer)
        {
            Debug.Log("UpdateGameStatus:isExpired: " + game.isExpired);
            if (game.gameState.IsGameOver || game.isExpired)
            {
                this.StartCoroutine(DisplayGameOverView());
            }
            else
            {
                //Debug.Log("UpdateGameStatus: updatePlayer: " + updatePlayer + ", gameState.isCurrentPlayerTurn: "+ game.gameState.isCurrentPlayerTurn);
                if (updatePlayer)
                {
                    if (game.gameState.GameType == GameType.REALTIME || game.gameState.GameType == GameType.RANDOM || game.gameState.GameType == GameType.FRIEND || game.gameState.GameType == GameType.LEADERBOARD || game.gameState.GameType == GameType.AI)
                    {
                        game.gameState.isCurrentPlayerTurn = !game.gameState.isCurrentPlayerTurn;
                    }
                }
                UpdatePlayerTurn();
            }
        }

        private void MoveGamePieceViews(Move move, List<MovingGamePiece> movingPieces, List<IToken> activeTokens)
        {
            GamePiece gamePiece = SpawnPiece(move.position.column, move.position.row, move.player);
            gamePiece.player = move.player;
            gamePiece.column = move.position.column;
            gamePiece.row = move.position.row;

            gamePiece.Move(movingPieces, activeTokens);

            gameBoardView.PrintGameBoard();
        }

        private IEnumerator RandomGamePiecesBlinkingRoutine()
        {
            float blinkTime = 2.5f;
            float t = 0.0f;
            while (true)
            {
                t += Time.deltaTime;
                if (t > blinkTime)
                {
                    var piecesForBlink = gameBoardView.GetWaitingGamePiecesList();
                    if (piecesForBlink.Count > 0)
                    {
                        int randomIndex = UnityEngine.Random.Range(0, piecesForBlink.Count);
                        GamePiece gamePiece = piecesForBlink[randomIndex];
                        gamePiece.View.Blink();     
                    }
                    t = 0;
                }
                yield return null;
            }
        }

        private IEnumerator DisplayGameOverView()
        {
            if (game.isExpired)
            {
                GameManager.instance.VisitedGameResults(game);
                gameInfo.Open(LocalizationManager.Instance.GetLocalizedValue("expired_text"), Color.white, false, !game.didViewResult);
                yield break;
            }

            Debug.Log("DisplayGameOverView gameState.winner: " +  game.gameState.Winner);

            this.PlayWinnerSound();
            this.ShowWinnerAnimation();

            yield return new WaitForSeconds(1.5f);

            winningParticleGenerator.ShowParticles();

            this.ShowWinnerText();
            this.LogGameWinner();

#if UNITY_IOS || UNITY_ANDROID
            if (game.gameState.Winner == PlayerEnum.ONE || game.gameState.Winner == PlayerEnum.TWO)
            {
                if (game.gameState.isCurrentPlayerTurn)
                {
                    Handheld.Vibrate();
                }
            }
#endif

        }

        private void PlayWinnerSound()
        {
            if (game.gameState.Winner == PlayerEnum.NONE || game.gameState.Winner == PlayerEnum.ALL)
            {
                return;
            }

            if (game.gameState.GameType == GameType.RANDOM || game.gameState.GameType == GameType.FRIEND || game.gameState.GameType == GameType.LEADERBOARD)
            {
                if (game.isCurrentPlayer_PlayerOne && game.gameState.Winner == PlayerEnum.ONE)
                {
                    SoundManager.instance.PlayRandomizedSfx(clipWin);
                }
                else if (!game.isCurrentPlayer_PlayerOne && game.gameState.Winner == PlayerEnum.TWO)
                {
                    SoundManager.instance.PlayRandomizedSfx(clipWin);
                }
            }
            else
            {
                SoundManager.instance.PlayRandomizedSfx(clipWin);
            }
        }


        private void ShowWinnerAnimation()
        {
            float delay = 0.3f;
            for (int i = 0; i < game.gameState.GameBoard.player1WinningPositions.Count; i++)
            {
                Position position = game.gameState.GameBoard.player1WinningPositions[i];
                GamePiece gamePiece = gameBoardView.GamePieceAt(position);
                gamePiece.View.PlayWinAnimation(delay);
                delay += 0.12f;
            }

            delay = 0.3f;
            for (int i = 0; i < game.gameState.GameBoard.player2WinningPositions.Count; i++)
            {
                Position position = game.gameState.GameBoard.player2WinningPositions[i];
                GamePiece gamePiece = gameBoardView.GamePieceAt(position);
                gamePiece.View.PlayWinAnimation(delay);
                delay += 0.12f;
            }
        }

        private void ShowWinnerText()
        {
            bool showRewardButton = !game.didViewResult;

            // Color winnerTextColor = gameState.Winner == PlayerEnum.ONE ? bluePlayerColor : redPlayerColor;
            Color winnerTextColor = Color.white;

            if (game.gameState.Winner == PlayerEnum.NONE || game.gameState.Winner == PlayerEnum.ALL)
            {
                if (game.gameState.GameType == GameType.RANDOM || game.gameState.GameType == GameType.FRIEND || game.gameState.GameType == GameType.LEADERBOARD)
                {
                    gameInfo.Open(LocalizationManager.Instance.GetLocalizedValue("draw_text"), Color.white, false, showRewardButton);
                }
                else
                {
                    if (game.gameState.GameType != GameType.PUZZLE)
                    {
                        gameInfo.Open(LocalizationManager.Instance.GetLocalizedValue("draw_text"), Color.white, false, false);
                    }
                }
            }
            else if (game.gameState.GameType == GameType.RANDOM || game.gameState.GameType == GameType.FRIEND || game.gameState.GameType == GameType.LEADERBOARD)
            {
                if (!string.IsNullOrEmpty(game.winnerName))
                {
                    gameInfo.Open(game.winnerName + LocalizationManager.Instance.GetLocalizedValue("won_suffix"), winnerTextColor, true, showRewardButton);
                }
                else
                {
                    if (game.isCurrentPlayer_PlayerOne && game.gameState.Winner == PlayerEnum.ONE)
                    {
                        gameInfo.Open(UserManager.instance.userName + LocalizationManager.Instance.GetLocalizedValue("won_suffix"), winnerTextColor, true, showRewardButton);
                    }
                    else if (!game.isCurrentPlayer_PlayerOne && game.gameState.Winner == PlayerEnum.TWO)
                    {
                        gameInfo.Open(UserManager.instance.userName + LocalizationManager.Instance.GetLocalizedValue("won_suffix"), winnerTextColor, true, showRewardButton);
                    }
                    else
                    {
                        gameInfo.Open(opponentUIPanel.GetPlayerName() + LocalizationManager.Instance.GetLocalizedValue("won_suffix"), winnerTextColor, true, showRewardButton);
                    }

                    //AnalyticsManager.LogGameOver("multiplayer", gameState.winner, gameState.tokenBoard);
                }
            }
            else
            {
                if (game.gameState.GameType != GameType.PUZZLE)
                {
                    //AnalyticsManager.LogGameOver("pnp", gameState.winner, gameState.tokenBoard);
                    string winnerText = game.gameState.Winner == PlayerEnum.ONE ? playerOneWonText : playerTwoWonText;
                    gameInfo.Open(winnerText, winnerTextColor, true, false);
                }
            }
        }

        private void LogGameWinner()
        {
            switch (game.gameState.GameType)
            {
                case GameType.FRIEND:
                    AnalyticsManager.LogGameOver("friend", game.gameState.Winner, game.gameState.TokenBoard);
                    break;
                case GameType.LEADERBOARD:
                    AnalyticsManager.LogGameOver("leaderboard", game.gameState.Winner, game.gameState.TokenBoard);
                    break;
                case GameType.AI:
                    AnalyticsManager.LogGameOver("AI", game.gameState.Winner, game.gameState.TokenBoard);
                    break;
                case GameType.PASSANDPLAY:
                    AnalyticsManager.LogGameOver("pnp", game.gameState.Winner, game.gameState.TokenBoard);
                    break;
                case GameType.PUZZLE:
                    //AnalyticsManager.LogGameOver("puzzle", gameState.winner, gameState.tokenBoard);
                    break;
                default:
                    AnalyticsManager.LogGameOver("random", game.gameState.Winner, game.gameState.TokenBoard);
                    break;
            }
        }
	}
}

