using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using GameSparks.Api.Requests;
using GameSparks.Api.Messages;
using GameSparks.Core;
using System.Linq;
using DG.Tweening;
using Lean.Pool;
//using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

namespace Fourzy
{
    public class GameManager : Singleton<GameManager>
    {
        public delegate void StartMove();
        public static event StartMove OnStartMove;
        public delegate void EndMove();
        public static event EndMove OnEndMove;
        public delegate void GameOver();
        public static event GameOver OnGameOver;
        public delegate void Resign(string challengeInstanceId);
        public static event Resign OnResign;
        public float moveSpeed = 8f;
        public float gameScreenFadeInTime = 0.6f;
        public string challengeInstanceId = null; // GameSparks Challenge Instance Id
        public string opponentUserId = "";

        // ---------- Token Views ----------
        public GameObject upArrowToken;
        public GameObject downArrowToken;
        public GameObject leftArrowToken;
        public GameObject rightArrowToken;
        public GameObject stickyToken;
        public GameObject blockerToken;
        public GameObject ghostToken;
        public GameObject iceSheetToken;
        public GameObject pitToken;
        public GameObject ninetyRightArrowToken;
        public GameObject ninetyLeftArrowToken;
        public GameObject bumperToken;
        public GameObject coinToken;
        public GameObject fruitToken;
        public GameObject fruitTreeToken;
        public GameObject webToken;
        public GameObject spiderToken;
        public GameObject sandToken;
        public GameObject waterToken;
        // ---------- End Token Views ----------

        public PuzzleChallengeInfo puzzleChallengeInfo;
        public GameBoardView gameBoardView;
        public GameState gameState { get; set; }
        public GameObject[,] tokenViews;
        public List<Game> games = new List<Game>();
        public GameObject gamePiecePrefab;
        public Sprite playerOneSpriteMoving;
        public Sprite playerOneSpriteAsleep;
        public Sprite playerTwoSpriteMoving;
        public Sprite playerTwoSpriteAsleep;
        private string playerOneWonText = "Player 1 Wins!";
        private string playerTwoWonText = "Player 2 Wins!";
        public string winner;
        public string opponentFacebookId;

        public Color bluePlayerColor = new Color(0f / 255f, 176.0f / 255f, 255.0f / 255.0f);
        public Color redPlayerColor = new Color(254.0f / 255.0f, 40.0f / 255.0f, 81.0f / 255.0f);
        public Shader lineShader = null;

        [Header("Game UI")]
        public GameObject gamePieces;
        private GameObject tokens;
        public GameObject gameScreen;
        public GameObject gameScreenSprites;
        public GameObject ErrorPanel;
        public Onboarding onboardingScreen;
        public Text playerNameLabel;
        public Image playerProfilePicture;
        public Image playerPiece;
        public Text opponentNameLabel;
        public Image opponentProfilePicture;
        public Image opponentPiece;
        public Button rematchButton;
        public Button nextGameButton;
        public Button createGameButton;
        public Button retryPuzzleChallengeButton;
        public Button nextPuzzleChallengeButton;
        public Button rewardsButton;
        public Button backButton;
        public Button resignButton;
        public GameObject moveHintArea;
        public GameIntroUI gameIntroUI;
        public AlertUI alertUI;
        public GameObject HeaderUI;
        public GameObject playerPieceUI;
        public GameObject opponentPieceUI;
        public Rewards rewardScreen;
        public GameInfo gameInfo;
        public Texture2D defaultProfilePicture;
        public Badge homeScreenPlayBadge;
        public Text challengeIdDebugText;
        public GameObject tokenPopupUI;
        public Text ratingDelta;

        public GameObject particleEffect;

        [Header("Game State")]
        public Game activeGame;
        // public GameType gameType;
        public bool isMultiplayer = false;
        public bool isNewChallenge = false;
        public bool isNewRandomChallenge = false;
        public bool isPuzzleChallenge = false;
        public bool isAiActive = false;
        public bool isCurrentPlayer_PlayerOne;
        public bool isLoading = true;
        public bool disableInput = false;
        public bool isLoadingUI;
        public bool isOnboardingActive = false;
        private bool isDropping = false;
        public bool isAnimating = false;
        public bool animatingGamePieces = false;
        private bool replayedLastMove = false;
        public bool isExpired = false;
        public bool didViewResult = false;
        public string opponentLeaderboardRank = "";
        public Screens activeScreen = Screens.NONE;
        public int challengerGamePieceId = 0;
        public int challengedGamePieceId = 0;
        public List<MoveInfo> initialMoves;

        //int spacing = 1; //100
        //int offset = 0; //4
        public AudioClip clipMove;
        public AudioClip clipWin;
        public AudioClip clipChest;

        public GameObject moveArrowLeft;
        public GameObject moveArrowRight;
        public GameObject moveArrowDown;
        public GameObject moveArrowUp;
        public GameObject cornerSpot;
        public MoveHintTouchArea moveHintTouchArea;
        Camera mainCamera;

        private void OnEnable()
        {
            UserInputHandler.OnTap += ProcessPlayerInput;
            GameUI.OnActiveGame += TransitionToGameScreen;
            FriendEntry.OnActiveGame += TransitionToGameScreen;
            LeaderboardPlayer.OnActiveGame += TransitionToGameScreen;
            ChallengeManager.OnActiveGame += TransitionToGameScreen;
            ChallengeManager.OnReceivedOpponentGamePiece += SetOpponentGamePiece;
            LoginManager.OnLoginError += DisplayLoginError;
            Game.OnActiveGame += TransitionToGameScreen;
            ChallengeManager.OnOpponentTurnTakenDelegate += OpponentTurnHandler;
            ChallengeManager.OnChallengeJoinedDelegate += ChallengeJoinedHandler;
            ChallengeManager.OnChallengeWonDelegate += ChallengeWonHandler;
            ChallengeManager.OnChallengeLostDelegate += ChallengeLostHandler;
            ChallengeManager.OnChallengeDrawnDelegate += ChallengeDrawnHandler;
            ChallengeManager.OnChallengeIssuedDelegate += ChallengeIssuedHandler;
            //SceneManager.sceneLoaded += OnGameFinishedLoading;
        }

        private void OnDisable()
        {
            UserInputHandler.OnTap -= ProcessPlayerInput;
            GameUI.OnActiveGame -= TransitionToGameScreen;
            FriendEntry.OnActiveGame -= TransitionToGameScreen;
            LeaderboardPlayer.OnActiveGame -= TransitionToGameScreen;
            ChallengeManager.OnActiveGame -= TransitionToGameScreen;
            ChallengeManager.OnReceivedOpponentGamePiece -= SetOpponentGamePiece;
            LoginManager.OnLoginError -= DisplayLoginError;
            Game.OnActiveGame -= TransitionToGameScreen;
            ChallengeManager.OnOpponentTurnTakenDelegate -= OpponentTurnHandler;
            ChallengeManager.OnChallengeJoinedDelegate -= ChallengeJoinedHandler;
            ChallengeManager.OnChallengeWonDelegate -= ChallengeWonHandler;
            ChallengeManager.OnChallengeLostDelegate -= ChallengeLostHandler;
            ChallengeManager.OnChallengeDrawnDelegate -= ChallengeDrawnHandler;
            ChallengeManager.OnChallengeIssuedDelegate -= ChallengeIssuedHandler;
            //SceneManager.sceneLoaded -= OnGameFinishedLoading;
        }

        void Start()
        {
            ChallengeAcceptedMessage.Listener = (message) =>
            {
                var gsChallenge = message.Challenge;
                Debug.Log("ChallengeAcceptedMessage");
                // Debug.Log("gsChallenge.Challenger.Id: " + gsChallenge.Challenger.Id);
                // Debug.Log("gsChallenge.Challenged: " + gsChallenge.Challenged);
                if (UserManager.instance.userId == gsChallenge.NextPlayer)
                {
                    Debug.Log("UserManager.instance.userId == gsChallenge.NextPlayer");
                }
            };

            ChallengeStartedMessage.Listener = (message) =>
            {
                var gsChallenge = message.Challenge;
                Debug.Log("ChallengeStartedMessage");
                // Debug.Log("gsChallenge.Challenger.Id: " + gsChallenge.Challenger.Id);
                // Debug.Log("gsChallenge.Challenged: " + gsChallenge.Challenged);
                if (UserManager.instance.userId == gsChallenge.NextPlayer)
                {
                    Debug.Log("UserManager.instance.userId == gsChallenge.NextPlayer");
                }
            };

            mainCamera = Camera.main;

            rematchButton.gameObject.SetActive(false);

            if (tokens == null)
            {
                tokens = new GameObject("Tokens");
                tokens.transform.parent = gameScreen.transform;
                tokens.transform.position.Set(-324f, 307f, 0f);
            }

            activeScreen = Screens.GAMES_LIST;

#if UNITY_IOS
            UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
            UnityEngine.iOS.NotificationServices.ClearRemoteNotifications();
#endif
        }

        new void Awake()
        {
            base.Awake();
            GS.GameSparksAvailable += CheckConnectionStatus;
            DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
            replayedLastMove = false;
        }

        private void OpponentTurnHandler(ChallengeTurnTakenMessage message) {
            var gsChallenge = message.Challenge;
            // replayedLastMove is a workaround to avoid having the opponents move being replayed more than once
            if (UserManager.instance.userId == gsChallenge.NextPlayer)
            {
                // Only Replay the last move if the player is viewing the game screen for that game
                if (gsChallenge.ChallengeId == challengeInstanceId && !replayedLastMove)
                {
                    replayedLastMove = true;
                    List<GSData> moveList = gsChallenge.ScriptData.GetGSDataList("moveList");
                    GSData lastMove = moveList.Last();
                    int currentPlayerMove = gsChallenge.ScriptData.GetInt("currentPlayerMove").GetValueOrDefault();
                    PlayerEnum opponent = currentPlayerMove == 1 ? PlayerEnum.TWO : PlayerEnum.ONE;
                    StartCoroutine(ReplayIncomingOpponentMove(lastMove, opponent));
                }
                else
                {
                    var currentGame = games
                        .Where(t => t.challengeId == gsChallenge.ChallengeId)
                        .FirstOrDefault();
                    if (currentGame != null)
                    {
                        GameSparksChallenge challenge = new GameSparksChallenge(gsChallenge);
                        GameState newGameState = new GameState(Constants.numRows, Constants.numColumns, challenge.gameType, challenge.isPlayerOneTurn, true, challenge.isGameOver, challenge.tokenBoard, PlayerEnum.EMPTY, ChallengeManager.instance.ConvertMoveList(challenge.moveList), challenge.previousGameboardData);
                        currentGame.gameState = newGameState;
                    }

                    if (activeScreen == Screens.GAME)
                    {
                        SetActionButton();
                    }

                    if (activeScreen == Screens.GAMES_LIST)
                    {
                        ChallengeManager.instance.ReloadGames();
                    }
                }
            }
        }

        private void ChallengeJoinedHandler(ChallengeJoinedMessage message) {
            var gsChallenge = message.Challenge;
            string opponentId = gsChallenge.Challenged.FirstOrDefault().Id;
            string opponentName = gsChallenge.Challenged.FirstOrDefault().Name;
            string opponentFBId = gsChallenge.Challenged.FirstOrDefault().ExternalIds.GetString("FB");

            ChallengeManager.instance.GetOpponentGamePiece(opponentId, gsChallenge.ChallengeId);

            var currentGame = games
                .Where(t => t.challengeId == gsChallenge.ChallengeId)
                .FirstOrDefault();
            if (currentGame != null)
            {
                Opponent opponent = new Opponent(opponentId, opponentName, opponentFBId);
                currentGame.opponent = opponent;
                currentGame.InitGame();
            }

            if (gsChallenge.ChallengeId == challengeInstanceId)
            {
                InitPlayerUI(opponentName, null, opponentFBId);
            }
            else
            {
                if (activeScreen == Screens.GAMES_LIST)
                {
                    ChallengeManager.instance.ReloadGames();
                }
            }
        }

        private void ChallengeDrawnHandler(ChallengeDrawnMessage message) {
            GameSparksChallenge challenge = new GameSparksChallenge(message.Challenge);
            GameCompletedHandler(challenge);
        }

        private void ChallengeWonHandler(ChallengeWonMessage message) {
            GameSparksChallenge challenge = new GameSparksChallenge(message.Challenge);
            GameCompletedHandler(challenge);
        }

        private void ChallengeLostHandler(ChallengeLostMessage message) {
            GameSparksChallenge challenge = new GameSparksChallenge(message.Challenge);
            GameCompletedHandler(challenge);
        }

        private void GameCompletedHandler(GameSparksChallenge challenge) {
            PlayerEnum player = challenge.currentPlayerMove == 1 ? PlayerEnum.ONE : PlayerEnum.TWO;

            var currentGame = games
                .Where(t => t.challengeId == challenge.challengeId)
                .FirstOrDefault();
            if (currentGame != null)
            {
                GameState newGameState = new GameState(Constants.numRows, Constants.numColumns, challenge.gameType, challenge.isPlayerOneTurn, true, challenge.isGameOver, challenge.tokenBoard, player, ChallengeManager.instance.ConvertMoveList(challenge.moveList), challenge.previousGameboardData);
                currentGame.gameState = newGameState;
                currentGame.challengerRatingDelta = challenge.challengerRatingDelta;
                currentGame.challengedRatingDelta = challenge.challengedRatingDelta;
            }

            if (challenge.challengeId == challengeInstanceId)
            {
                ChallengeManager.instance.GetRatingDelta(challengeInstanceId);
            } 

            // Only for the player who didn't make the last move
            if (UserManager.instance.userId != challenge.nextPlayer)
            {
                // Only Replay the last move if the player is viewing the game screen for that game
                if (challenge.challengeId == challengeInstanceId)
                {
                    GSData lastMove = challenge.moveList.Last();
                    StartCoroutine(ReplayIncomingOpponentMove(lastMove, player));
                    SetActionButton();
                }
                else
                {
                    if (activeScreen == Screens.GAME) {
                        SetActionButton();
                    }

                    if (activeScreen == Screens.GAMES_LIST) {
                        ChallengeManager.instance.ReloadGames();
                    }
                }
            }

            // Update players coins and rating when a game is completed
            UserManager.instance.UpdateInformation();
        }

        private void ChallengeIssuedHandler(ChallengeIssuedMessage message) {
            var gsChallenge = message.Challenge;
            Debug.Log("ChallengeIssuedHandler");
            GameSparksChallenge challenge = new GameSparksChallenge(gsChallenge);
            // public GameState(int numRows, int numColumns, bool isPlayerOneTurn, bool isCurrentPlayerTurn, bool isGameOver, TokenBoard tokenBoard, PlayerEnum winner, List<Move> moveList, int[] previousGameboardData) {
            GameState newGameState = new GameState(Constants.numRows, Constants.numColumns, challenge.gameType, challenge.isPlayerOneTurn, true, challenge.isGameOver, challenge.tokenBoard, PlayerEnum.EMPTY, ChallengeManager.instance.ConvertMoveList(challenge.moveList), challenge.previousGameboardData);
            string opponentFBId = gsChallenge.Challenger.ExternalIds.GetString("FB");
            Opponent opponent = new Opponent(gsChallenge.Challenger.Id, gsChallenge.Challenger.Name, opponentFBId);

            Game game = new Game(gsChallenge.ChallengeId, newGameState, false, false, false, opponent, ChallengeState.RUNNING, ChallengeType.STANDARD, challenge.challengerGamePieceId, challenge.challengedGamePieceId);
            game.gameState.SetRandomGuid(gsChallenge.ChallengeId);
            games.Add(game);

            if (activeScreen == Screens.GAME) {
                SetActionButton();
            }

            if (activeScreen == Screens.GAMES_LIST) {
                ChallengeManager.instance.ReloadGames();
            }
        }

        public void UpdateBadgeCounts() {
            int activeGamesCount = 0;

            for (int i = 0; i < games.Count(); i++)
            {
                if (games[i].gameState.isCurrentPlayerTurn == true || (games[i].didViewResult == false && games[i].gameState.IsGameOver == true))
                {
                    activeGamesCount++;
                }
            }

            if (activeGamesCount > 0) {
                homeScreenPlayBadge.gameObject.SetActive(true);
            } else {
                homeScreenPlayBadge.gameObject.SetActive(false);    
            }

            homeScreenPlayBadge.SetGameCount(activeGamesCount);
        }

        void Update()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (challengeInstanceId != null)
                    {
                        GameScreenBackButton();
                    }
                    else
                    {
                        Application.Quit();
                    }
                }
            }
        }

        void OnApplicationPause(bool paused)
        {
            if (!paused)
            {
                if (ChallengeManager.instance)
                {
                    ChallengeManager.instance.GetChallenges();
                }
            }
            else
            {
                new EndSessionRequest()
                    .Send((response) =>
                    {
                        if (response.HasErrors)
                        {
                            Debug.Log("***** EndSessionRequest:Error: " + response.Errors.JSON);
                        }
                    });
            }
        }

        private void ResetGameManagerState()
        {
            challengeInstanceId = null;
            isMultiplayer = false;
            isNewChallenge = false;
            isNewRandomChallenge = false;
            isPuzzleChallenge = false;
            isAiActive = false;
            isLoading = true;
            isLoadingUI = false;
            isDropping = false;
            isAnimating = false;
            didViewResult = false;
            isExpired = false;
            animatingGamePieces = false;
            replayedLastMove = false;
            puzzleChallengeInfo = null;
            // gameType = GameType.NONE;
        }

        public void GameScreenBackButton() {
            UserInputHandler.inputEnabled = false;
            gameScreen.GetComponent<CanvasGroup>().alpha = 0.0f;
            gameScreen.SetActive(false);
            challengeInstanceId = null;
            winner = null;

            ResetGamePiecesAndTokens();
            ResetGameManagerState();
            ResetUIGameScreen();
            HeaderUI.SetActive(true);
            activeScreen = Screens.GAMES_LIST;

            if (ViewController.instance.GetCurrentView() != null) {
                ViewController.instance.ChangeView(ViewController.instance.GetCurrentView());
                if (ViewController.instance.GetCurrentView() != ViewController.instance.viewTraining)
                {
                    ViewController.instance.ShowTabView();
                }
            } else if (ViewController.instance.GetPreviousView() != null) {
                ViewController.instance.ChangeView(ViewController.instance.GetPreviousView());
            }

            ChallengeManager.instance.ReloadGames();
        }

        public Game GetActiveGame() {
            int activeGamesCount = 0;
            Game game = null;
            for (int i = 0; i < games.Count(); i++)
            {
                if (games[i].gameState.isCurrentPlayerTurn == true || (games[i].didViewResult == false && games[i].gameState.IsGameOver == true))
                {
                    activeGamesCount++;
                    if (game == null)
                    {
                        game = games[i];
                    }
                }
            }

            return game;
        }

        public void ResignButton() {
            if (OnResign != null)
            {
                OnResign(challengeInstanceId);
                //resignButton.gameObject.SetActive(false);
            }
        }

        public void RewardsButton()
        {
            VisitedGameResults();
            UserInputHandler.inputEnabled = false;

            bool isCurrentPlayerWinner = false;
            PlayerEnum currentPlayer = PlayerEnum.NONE;
            Debug.Log("gameState.winner: " + gameState.Winner);
            Debug.Log("isCurrentPlayer_PlayerOne: " + isCurrentPlayer_PlayerOne);

            if (isCurrentPlayer_PlayerOne && gameState.Winner == PlayerEnum.ONE)
            {
                isCurrentPlayerWinner = true;
            }
            else if (!isCurrentPlayer_PlayerOne && gameState.Winner == PlayerEnum.TWO)
            {
                isCurrentPlayerWinner = true;
            }
            else if (isCurrentPlayer_PlayerOne && gameState.Winner == PlayerEnum.TWO)
            {
                isCurrentPlayerWinner = false;
            }
            else if (!isCurrentPlayer_PlayerOne && gameState.Winner == PlayerEnum.ONE)
            {
                isCurrentPlayerWinner = false;
            }
            else if (gameState.Winner == PlayerEnum.ALL || gameState.Winner == PlayerEnum.NONE)
            {
                isCurrentPlayerWinner = false;
            }

            if (isCurrentPlayer_PlayerOne)
            {
                currentPlayer = PlayerEnum.ONE;
            }
            else
            {
                currentPlayer = PlayerEnum.TWO;
            }

            //Debug.Log("RewardsButton gameState.PlayerPieceCount(currentPlayer): " + gameState.PlayerPieceCount(currentPlayer));
            rewardScreen.Open(isCurrentPlayerWinner, gameState.PlayerPieceCount(currentPlayer));
            gameInfo.Close();
        }

        public void CreateGameButton()
        {
            Game game = GetActiveGame();
            if (game != null)
            {
                game.OpenGame();
            }
            else
            {
                ViewController.instance.ChangeView(ViewController.instance.viewMatchMaking);
            }
        }

        public void RewardsScreenOkButton()
        {
            rewardScreen.gameObject.SetActive(false);
        }

        private void TransitionToGameScreen()
        {
            UserInputHandler.inputEnabled = false;
            gameScreen.SetActive(true);

            FadeGameScreen(1.0f, gameScreenFadeInTime);
            StartCoroutine(WaitToEnableInput());
            HeaderUI.SetActive(false);
            activeScreen = Screens.GAME;
        }

        public void OpenNewGame(GameType gameType, bool displayIntroUI = true, string tokenBoardId = null)
        {
            Debug.Log("GameManager OpenNewGame tokenboardId: "+ tokenBoardId);

            TokenBoard tokenBoard = ChallengeManager.instance.tokenBoard;

            if (tokenBoard == null)
            {
                tokenBoard = TokenBoardLoader.instance.GetRandomTokenBoard();
            }
            if (tokenBoardId != null) {
                tokenBoard = TokenBoardLoader.instance.GetTokenBoard(tokenBoardId);
            }
            if (tokenBoard.initialMoves != null) {
                initialMoves = tokenBoard.initialMoves;    
            }
            //Debug.Log("GameManager OpenNewGame tokenBoard.initialGameBoard: " + tokenBoard.initialGameBoard);
            //for (int i = 0; i < 64; i++)
            //{
            //    Debug.Log(tokenBoard.initialGameBoard[i]);
            //}
            //If we initiated the challenge, we get to be player 1
            GameState newGameState = new GameState(Constants.numRows, Constants.numColumns, gameType, true, true, tokenBoard, tokenBoard.initialGameBoard, false, null);
            gameState = newGameState;

            ResetGamePiecesAndTokens();
            ResetUIGameScreen();

            CreateGamePieceViews(gameState.GetGameBoard());
            CreateTokenViews();

            challengeInstanceId = null;
            isPuzzleChallenge = false;
            isCurrentPlayer_PlayerOne = true;

            string introUISubtitle = "";
            switch (gameType)
            {
                case GameType.PASSANDPLAY:
                    isMultiplayer = false;
                    isNewRandomChallenge = false;
                    isNewChallenge = false;
                    isAiActive = false;
                    introUISubtitle = LocalizationManager.Instance.GetLocalizedValue("pnp_button");
                    break;
                case GameType.FRIEND:
                    isMultiplayer = true;
                    isNewRandomChallenge = false;
                    isNewChallenge = true;
                    isAiActive = false;
                    introUISubtitle = LocalizationManager.Instance.GetLocalizedValue("friend_challenge_text");
                    break;
                case GameType.LEADERBOARD:
                    isMultiplayer = true;
                    isNewRandomChallenge = false;
                    isNewChallenge = true;
                    isAiActive = false;
                    introUISubtitle = LocalizationManager.Instance.GetLocalizedValue("leaderboard_challenge_text");
                    break;
                case GameType.RANDOM:
                    isMultiplayer = true;
                    isNewRandomChallenge = true;
                    isNewChallenge = false;
                    isAiActive = false;
                    introUISubtitle = LocalizationManager.Instance.GetLocalizedValue("random_opponent_button");
                    break;
                case GameType.AI:
                    isMultiplayer = false;
                    isNewRandomChallenge = false;
                    isNewChallenge = false;
                    isAiActive = true;
                    introUISubtitle = LocalizationManager.Instance.GetLocalizedValue("ai_challenge_text");
                    break;
                default:
                    break;
            }

            InitPlayerUI(opponentNameLabel.text, opponentProfilePicture.sprite);
            UpdatePlayerUI();

            if (displayIntroUI) {
                DisplayIntroUI(tokenBoard.name, introUISubtitle, true);
            }

            TransitionToGameScreen();

            if (gameType == GameType.FRIEND)
            {
                Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                customAttributes.Add("TokenBoardId", tokenBoard.id);
                customAttributes.Add("TokenBoardName", tokenBoard.name);
                AnalyticsManager.LogCustom("open_new_friend_challenge", customAttributes);
            }
            else if (gameType == GameType.LEADERBOARD)
            {
                Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                customAttributes.Add("TokenBoardId", tokenBoard.id);
                customAttributes.Add("TokenBoardName", tokenBoard.name);
                customAttributes.Add("Rank", opponentLeaderboardRank);
                AnalyticsManager.LogCustom("open_new_leaderboard_challenge", customAttributes);
            }
            else if (gameType == GameType.PASSANDPLAY)
            {
                Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                customAttributes.Add("TokenBoardId", tokenBoard.id);
                customAttributes.Add("TokenBoardName", tokenBoard.name);
                AnalyticsManager.LogCustom("open_pnp_game", customAttributes);
            }
            else if (gameType == GameType.RANDOM)
            {
                Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                customAttributes.Add("PlayerName", UserManager.instance.userName);
                customAttributes.Add("TokenBoardId", tokenBoard.id);
                customAttributes.Add("TokenBoardName", tokenBoard.name);
                AnalyticsManager.LogCustom("open_new_multiplayer_game", customAttributes);
            }
            else if (gameType == GameType.AI)
            {
                Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                customAttributes.Add("TokenBoardId", tokenBoard.id);
                customAttributes.Add("TokenBoardName", tokenBoard.name);
                AnalyticsManager.LogCustom("open_new_ai_challenge", customAttributes);
            }
        }

        public void NextGame()
        {
            gameScreen.GetComponent<CanvasGroup>().alpha = 0.0f;

            for (int i = 0; i < games.Count; i++)
            {
                if (games[i].challengeId == challengeInstanceId) {
                    var game = games[i];
                    games.RemoveAt(i);
                    games.Add(game);
                    break;
                }
            }

            for (int i = 0; i < games.Count; i++)
            {
                if ((games[i].gameState.isCurrentPlayerTurn == true || (games[i].didViewResult == false && games[i].gameState.IsGameOver == true)) && games[i].challengeId != challengeInstanceId)
                {
                    if (games[i] != null)
                    {
                        Debug.Log("NextGame: current game challengeid: "+ challengeInstanceId);
                        Debug.Log("NextGame: open game challengeid: " + games[i].challengeId);
                        games[i].OpenGame();
                        AnalyticsManager.LogCustom("next_game");
                        break;
                    }
                }
            }
        }

        private void SetOpponentGamePiece(string gamePieceId, string challengeId) {
            Debug.Log("GameManager: SetOpponentGamePiece: gamepieceid: " + gamePieceId);
            challengedGamePieceId = int.Parse(gamePieceId);
            if (challengeId != "") {
                var game = games
                    .Where(t => t.challengeId == challengeId)
                    .FirstOrDefault();

                if (game != null)
                {
                    game.challengedGamePieceId = int.Parse(gamePieceId);
                }
            }

            if (challengedGamePieceId > GamePieceSelectionManager.Instance.gamePieces.Count - 1) {
                challengedGamePieceId = 0;
            }

            opponentPiece.sprite = GamePieceSelectionManager.Instance.GetGamePieceSprite(challengedGamePieceId);

            GamePieceUI opponent = opponentPiece.GetComponent<GamePieceUI>();
            if (challengerGamePieceId == challengedGamePieceId)
            {
                opponent.SetAlternateColor(true);
            }
            else
            {
                opponent.SetAlternateColor(false);
            }
        }

        private void FadeGameScreen(float alpha, float fadeTime)
        {
            gameScreen.GetComponent<CanvasGroup>().DOFade(alpha, 0.5f).OnComplete(() => FadeTokens(alpha, fadeTime));
        }

        private void FadeTokens(float alpha, float fadeTime)
        {
            GameObject[] tokenArray = GameObject.FindGameObjectsWithTag("Token");
            for (int i = 0; i < tokenArray.Length; i++)
            {
                if (i < tokenArray.Length - 1)
                {
                    tokenArray[i].GetComponent<SpriteRenderer>().DOFade(alpha, fadeTime);
                }
                else
                {
                    tokenArray[i].GetComponent<SpriteRenderer>().DOFade(alpha, fadeTime).OnComplete(() => FadePieces(alpha, fadeTime));
                }
            }
        }

        private void FadePieces(float alpha, float fadeTime)
        {
            SoundManager.instance.Mute(false);
            GameObject[] pieces = GameObject.FindGameObjectsWithTag("GamePiece");
            if (pieces.Length > 0)
            {
                for (int i = 0; i < pieces.Length; i++)
                {
                    if (i < pieces.Length - 1)
                    {
                        pieces[i].GetComponent<SpriteRenderer>().DOFade(alpha, fadeTime);
                    }
                    else
                    {
                        pieces[i].GetComponent<SpriteRenderer>().DOFade(alpha, fadeTime).OnComplete(() => ReplayLastMove());
                    }
                }
            }
            else
            {
                ReplayLastMove();
            }
        }

        private void ReplayLastMove()
        {
            //Debug.Log("ReplayLastMove: initialMoves = " + initialMoves.Count);
            //Debug.Log("gameState.moveList = " + gameState.moveList.Count);
            if (gameState.MoveList != null)
            {
                // GSData lastMove = gameState.moveList.Last();
                Move lastMove = gameState.MoveList.Last();

                // int position = lastMove.GetInt("position").GetValueOrDefault();
                // Direction direction = (Direction)lastMove.GetInt("direction").GetValueOrDefault();
                
                // int position = lastMove.location; 
                // Direction direction = lastMove.direction;

                PlayerEnum player = PlayerEnum.NONE;
                if (!gameState.IsGameOver)
                {
                    player = gameState.IsPlayerOneTurn ? PlayerEnum.TWO : PlayerEnum.ONE;
                }
                else
                {
                    player = gameState.IsPlayerOneTurn ? PlayerEnum.ONE : PlayerEnum.TWO;
                }

                // Move move = new Move(position, direction, player);
                lastMove.player = player;

                StartCoroutine(MovePiece(lastMove, true, false));
            }
            else if (initialMoves.Count > 0)
            {
                //StartCoroutine(PlayInitialMoves());
            } else {
                //Debug.Log("ReplayLastMove: isloading is false");
                isLoading = false;
            }
        }

        public IEnumerator PlayInitialMoves() {
            Debug.Log("PlayInitialMoves");
            for (int i = 0; i < initialMoves.Count; i++)
            {
                Move move = new Move(initialMoves[i].Location, (Direction)initialMoves[i].Direction, i % 2 == 0 ? PlayerEnum.ONE : PlayerEnum.TWO);
                StartCoroutine(MovePiece(move, false, false));
                yield return new WaitWhile(() => isDropping == true);
            }

            isLoading = false;
        }

        //private void FadeGamesListScreen(float alpha, bool isActive, float fadeTime)
        //{
        //    UIScreen.GetComponent<CanvasGroup>().DOFade(alpha, fadeTime).OnComplete(() => UIScreenSetActive(isActive));
        //}

        //private void GameScreenSetActive(bool isActive)
        //{
        //    gameScreen.SetActive(isActive);
        //}

        IEnumerator WaitToEnableInput()
        {
            yield return new WaitForSeconds(1.5f);
            UserInputHandler.inputEnabled = true;
        }

        private void CheckConnectionStatus(bool connected)
        {
            // TODO: inform the player they dont have a connection when connected is false
            //Debug.Log("CheckConnectionStatus: " + connected);
        }

        private IEnumerator ReplayIncomingOpponentMove(GSData lastMove, PlayerEnum player)
        {
            int position = lastMove.GetInt("position").GetValueOrDefault();
            Direction direction = (Direction)lastMove.GetInt("direction").GetValueOrDefault();
            Move move = new Move(position, direction, player);

            while (isDropping || isLoading)
                yield return null;
            StartCoroutine(MovePiece(move, false, true));
        }

        public void SetupGame(string title, string subtitle)
        {
            //UpdatePlayersStatusView();
            SoundManager.instance.Mute(true);
            if (challengeInstanceId != null || challengeInstanceId != "") {
                challengeIdDebugText.text = "ChallengeId: " + challengeInstanceId;    
            } else {
                challengeIdDebugText.text = "Error: missing challenge id";
            }

            CreateGamePieceViews(gameState.GetPreviousGameBoard());
            CreateTokenViews();

            DisplayIntroUI(title, subtitle, true);

            SetActionButton();
            // Debug.Log("SetupGame:activeGame.challengerRatingDelta: " + activeGame.challengerRatingDelta);
            // Debug.Log("SetupGame:activeGame.challengedRatingDelta: " + activeGame.challengedRatingDelta);
            ratingDelta.text = "0";
            if (isCurrentPlayer_PlayerOne)
            {
                if (activeGame != null)
                {
                    ratingDelta.text = activeGame.challengerRatingDelta.ToString();
                }
            }
            else
            {
                if (activeGame != null)
                {
                    ratingDelta.text = activeGame.challengedRatingDelta.ToString();
                }
            }

            //if (isPuzzleChallenge && puzzleChallengeInfo.Level <= 2) {
            //    //moveHintArea.SetActive(true);
            //    moveHintTouchArea.FadeInAndOutSprite();
            //}
        }

        public void DisplayIntroUI(string title, string subtitle, bool fade) {
            if (title != null && title != "" && subtitle != null && subtitle != "") {
                gameIntroUI.Open(title, subtitle, fade);
            }
        }

        public void CreateGamePieceViews(int[,] board)
        {
            gameBoardView.gamePieces = new GameObject[Constants.numRows, Constants.numColumns];

            for (int row = 0; row < Constants.numRows; row++)
            {
                for (int col = 0; col < Constants.numColumns; col++)
                {
                    int piece = board[row, col];

                    if (piece == (int)Piece.BLUE)
                    {
                        GameObject pieceObject = SpawnPiece(col, row * -1, PlayerEnum.ONE, PieceAnimState.ASLEEP);
                        SpriteRenderer pieceSprite = pieceObject.GetComponent<SpriteRenderer>();
                        Color c = pieceSprite.color;
                        c.a = 0.0f;
                        pieceSprite.color = c;

                        GamePiece pieceModel = pieceObject.GetComponent<GamePiece>();
                        pieceModel.player = PlayerEnum.ONE;
                        gameBoardView.gamePieces[row, col] = pieceObject;
                    }
                    else if (piece == (int)Piece.RED)
                    {
                        GameObject pieceObject = SpawnPiece(col, row * -1, PlayerEnum.TWO, PieceAnimState.ASLEEP);
                        SpriteRenderer pieceSprite = pieceObject.GetComponent<SpriteRenderer>();
                        Color c = pieceSprite.color;
                        c.a = 0.0f;
                        pieceSprite.color = c;

                        GamePiece pieceModel = pieceObject.GetComponent<GamePiece>();
                        pieceModel.player = PlayerEnum.TWO;
                        gameBoardView.gamePieces[row, col] = pieceObject;
                    }
                }
            }
        }

        public void CreateStickyToken(int row, int col) {
            float xPos = (col + .1f) * .972f;
            float yPos = (row * -1 + .09f) * .965f;
            GameObject go = Instantiate(stickyToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
            Destroy(tokenViews[row, col]);
            tokenViews[row, col] = go;
        }

        public void CreateTokenViews()
        {
            tokenViews = new GameObject[Constants.numRows, Constants.numColumns];

            for (int row = 0; row < Constants.numRows; row++)
            {
                for (int col = 0; col < Constants.numColumns; col++)
                {
                    float xPos = (col + .1f) * .972f;
                    float yPos = (row * -1 + .09f) * .965f;

                    Token token = gameState.PreviousTokenBoard.tokens[row, col].tokenType;
                    GameObject go;
                    switch (token)
                    {
                        case Token.UP_ARROW:
                            go = Instantiate(upArrowToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews[row, col] = go;
                            break;
                        case Token.DOWN_ARROW:
                            go = Instantiate(downArrowToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews[row, col] = go;
                            break;
                        case Token.LEFT_ARROW:
                            go = Instantiate(leftArrowToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            go.transform.Rotate(0, 0, -90);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews[row, col] = go;
                            break;
                        case Token.RIGHT_ARROW:
                            go = Instantiate(rightArrowToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            go.transform.Rotate(0,0,90);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews[row, col] = go;
                            break;
                        case Token.STICKY:
                            go = Instantiate(stickyToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews[row, col] = go;
                            break;
                        case Token.BLOCKER:
                            go = Instantiate(blockerToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews[row, col] = go;
                            break;
                        case Token.GHOST:
                            go = Instantiate(ghostToken, new Vector3(xPos, yPos, 5), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews[row, col] = go;
                            break;
                        case Token.ICE_SHEET:
                            go = Instantiate(iceSheetToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews[row, col] = go;
                            break;
                        case Token.PIT:
                            go = Instantiate(pitToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews[row, col] = go;
                            break;
                        case Token.NINETY_RIGHT_ARROW:
                            go = Instantiate(ninetyRightArrowToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews[row, col] = go;
                            break;
                        case Token.NINETY_LEFT_ARROW:
                            go = Instantiate(ninetyLeftArrowToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews[row, col] = go;
                            break;
                        case Token.BUMPER:
                            go = Instantiate(bumperToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews[row, col] = go;
                            break;
                        case Token.COIN:
                            go = Instantiate(coinToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews[row, col] = go;
                            break;
                        case Token.FRUIT:
                            go = Instantiate(fruitToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews[row, col] = go;
                            break;
                        case Token.FRUIT_TREE:
                            go = Instantiate(fruitTreeToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews[row, col] = go;
                            break;
                        case Token.WEB:
                            go = Instantiate(webToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews[row, col] = go;
                            break;
                        case Token.SPIDER:
                            go = Instantiate(spiderToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews[row, col] = go;
                            break;
                        case Token.SAND:
                            go = Instantiate(sandToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews[row, col] = go;
                            break;
                        case Token.WATER:
                            go = Instantiate(waterToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews[row, col] = go;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void InitPlayerUI(string opponentName = "", Sprite opponentProfileSprite = null, string opponentFacebookId = "")
        {
            if (isMultiplayer) {
                if (isCurrentPlayer_PlayerOne)
                {
                    // playerNameLabel.color = bluePlayerColor;
                    // opponentNameLabel.color = redPlayerColor;
                    int playerGamePieceId = challengerGamePieceId;
                    int opponentGamePieceId = challengedGamePieceId;

                    if (playerGamePieceId > GamePieceSelectionManager.Instance.gamePieces.Count - 1)
                    {
                        playerGamePieceId = 0;
                    }

                    playerPiece.sprite = GamePieceSelectionManager.Instance.GetGamePieceSprite(playerGamePieceId);
                    GamePieceUI player = playerPiece.GetComponent<GamePieceUI>();
                    player.SetAlternateColor(false);

                    if (opponentGamePieceId > GamePieceSelectionManager.Instance.gamePieces.Count - 1) {
                        opponentGamePieceId = 0;
                    }

                    if (opponentGamePieceId != -1) {
                        opponentPiece.sprite = GamePieceSelectionManager.Instance.GetGamePieceSprite(opponentGamePieceId);
                    } else {
                        opponentPiece.sprite = GamePieceSelectionManager.Instance.gamePieces[0];
                    }

                    GamePieceUI opponent = opponentPiece.GetComponent<GamePieceUI>();
                    if (playerGamePieceId == opponentGamePieceId) {
                        opponent.SetAlternateColor(true);    
                    } else {
                        opponent.SetAlternateColor(false);
                    }
                }
                else
                {
                    // playerNameLabel.color = redPlayerColor;
                    // opponentNameLabel.color = bluePlayerColor;
                    playerPiece.sprite = playerTwoSpriteMoving;
                    opponentPiece.sprite = playerOneSpriteMoving;
                    int playerGamePieceId = challengedGamePieceId;
                    int opponentGamePieceId = challengerGamePieceId;

                    if (playerGamePieceId > GamePieceSelectionManager.Instance.gamePieces.Count - 1)
                    {
                        playerGamePieceId = 0;
                    }

                    playerPiece.sprite = GamePieceSelectionManager.Instance.GetGamePieceSprite(playerGamePieceId);
                    GamePieceUI player = playerPiece.GetComponent<GamePieceUI>();
                    if (playerGamePieceId == opponentGamePieceId) {
                        player.SetAlternateColor(true);
                    } else {
                        player.SetAlternateColor(false);
                    }

                    if (opponentGamePieceId > GamePieceSelectionManager.Instance.gamePieces.Count - 1)
                    {
                        opponentGamePieceId = 0;
                    }

                    opponentPiece.sprite = GamePieceSelectionManager.Instance.GetGamePieceSprite(opponentGamePieceId);
                    GamePieceUI opponent = opponentPiece.GetComponent<GamePieceUI>();
                    opponent.SetAlternateColor(false);
                }

                playerNameLabel.text = UserManager.instance.userName;
                if (UserManager.instance.profilePicture)
                {
                    playerProfilePicture.sprite = UserManager.instance.profilePicture;
                }
                else
                {
                    playerProfilePicture.sprite = Sprite.Create(defaultProfilePicture,
                        new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height),
                        new Vector2(0.5f, 0.5f));
                }

                if (opponentName != "") {
                    opponentNameLabel.text = opponentName;
                } else {
                    opponentNameLabel.text = LocalizationManager.Instance.GetLocalizedValue("waiting_opponent_text");
                }

                if (opponentProfileSprite != null)
                {
                    //Debug.Log("setting opponentProfilePicture is not null");
                    opponentProfilePicture.sprite = opponentProfileSprite;
                }
                else if (opponentFacebookId != "")
                {
                    StartCoroutine(UserManager.instance.GetFBPicture(opponentFacebookId, (sprite) =>
                    {
                        opponentProfilePicture.sprite = sprite;
                    }));
                } else {
                    opponentProfilePicture.sprite = Sprite.Create(defaultProfilePicture,
                        new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height),
                        new Vector2(0.5f, 0.5f));
                }
            } else {
                playerNameLabel.text = "Player 1";
                playerProfilePicture.sprite = Sprite.Create(defaultProfilePicture,
                    new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height),
                    new Vector2(0.5f, 0.5f));
                playerPiece.sprite = GamePieceSelectionManager.Instance.GetGamePieceSprite(challengerGamePieceId);

                opponentNameLabel.text = "Player 2";
                opponentProfilePicture.sprite = Sprite.Create(defaultProfilePicture,
                    new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height),
                    new Vector2(0.5f, 0.5f));
                opponentPiece.sprite = GamePieceSelectionManager.Instance.GetGamePieceSprite(challengedGamePieceId);

                GamePieceUI opponent = opponentPiece.GetComponent<GamePieceUI>();
                if (challengerGamePieceId == challengedGamePieceId)
                {
                    opponent.SetAlternateColor(true);
                }
                else
                {
                    opponent.SetAlternateColor(false);
                }
            }
        }

        public void UpdatePlayerUI()
        {
            //AnimatePlayerPieceUI();
        }

        public void AnimatePlayerPieceUI() {
            //Debug.Log("isCurrentPlayer_PlayerOne: " + isCurrentPlayer_PlayerOne);
            //Debug.Log("isPlayerOneTurn: " + gameState.isPlayerOneTurn);
            float animationSpeed = 0.8f;
            if ((gameState.IsPlayerOneTurn && isCurrentPlayer_PlayerOne) || (!gameState.IsPlayerOneTurn && !isCurrentPlayer_PlayerOne)) {
                //Animate current player piece to center
                Vector2 centerPos = new Vector2(439, -125);
                RectTransform pprt = playerPieceUI.GetComponent<RectTransform>();
                pprt.DOAnchorPos(centerPos, animationSpeed);
                playerPieceUI.transform.DOScale(new Vector3(1.3f, 1.3f), 1);

                //Animate opponent piece to upper right
                Vector3 startingPos = new Vector2(-130, -83);
                RectTransform oprt = opponentPieceUI.GetComponent<RectTransform>();
                oprt.DOAnchorPos(startingPos, animationSpeed);
                opponentPieceUI.transform.DOScale(new Vector3(1, 1), animationSpeed);
            } else if ((gameState.IsPlayerOneTurn && !isCurrentPlayer_PlayerOne) || (!gameState.IsPlayerOneTurn && isCurrentPlayer_PlayerOne)) {
                //Animate player piece to upper left
                Vector2 startingPos = new Vector2(175, -83);
                RectTransform pprt = playerPieceUI.GetComponent<RectTransform>();
                pprt.DOAnchorPos(startingPos, 1);
                playerPieceUI.transform.DOScale(new Vector3(1, 1), animationSpeed);

                // Animate opponent piece to center
                Vector2 centerPos = new Vector2(-370, -125);
                RectTransform oprt = opponentPieceUI.GetComponent<RectTransform>();
                oprt.DOAnchorPos(centerPos, 1);
                opponentPieceUI.transform.DOScale(new Vector3(1.3f, 1.3f), animationSpeed);
            }
        }

        public void ResetUIGameScreen()
        {
            gameInfo.Close();
            // RectTransform pprt = playerPieceUI.GetComponent<RectTransform>();
            // pprt.anchoredPosition = new Vector2(175, -83);
            // pprt.localScale = new Vector3(1, 1, 1);
            // RectTransform oprt = opponentPieceUI.GetComponent<RectTransform>();
            // oprt.anchoredPosition = new Vector2(-130, -83);
            // oprt.localScale = new Vector3(1, 1, 1);

            rematchButton.gameObject.SetActive(false);
            nextGameButton.gameObject.SetActive(false);
            createGameButton.gameObject.SetActive(false);
            nextPuzzleChallengeButton.gameObject.SetActive(false);
            retryPuzzleChallengeButton.gameObject.SetActive(false);
            backButton.gameObject.SetActive(true);
            moveHintArea.SetActive(false);
            gameIntroUI.Close();
        }

        public void SetActionButton()
        {
            if (isMultiplayer)
            {
                bool hasNextGame = false;

                for (int i = 0; i < games.Count(); i++)
                {
                    if (games[i].gameState != null) {
                        if ((games[i].gameState.isCurrentPlayerTurn == true || (games[i].didViewResult == false && games[i].gameState.IsGameOver == true && games[i].gameState.isCurrentPlayerTurn == false)) && games[i].challengeId != challengeInstanceId)
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
                if (isPuzzleChallenge) {
                    if (gameState.IsPuzzleChallengeCompleted) {
                        if (gameState.IsPuzzleChallengePassed) {
                            nextPuzzleChallengeButton.gameObject.SetActive(true);
                        } else {
                            retryPuzzleChallengeButton.gameObject.SetActive(true);
                        }
                    }
                } else {
                    if (gameState.IsGameOver && !isOnboardingActive) {
                        // Debug.Log("set rematch button active");
                        rematchButton.gameObject.SetActive(true);
                    }
                }
            }
        }

        public void ResetGamePiecesAndTokens()
        {
            if (gamePieces.transform.childCount > 0)
            {
                for (int i = gamePieces.transform.childCount - 1; i >= 0; i--)
                {
                    Transform piece = gamePieces.transform.GetChild(i);
                    piece.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                    //GamePiece p = piece.GetComponent<GamePiece>();
                    //Debug.Log("ResetGamePiecesAndTokens: animating: " + p.animating);
                    //p.Reset();
                    Destroy(piece.gameObject);
                    //LeanPool.Despawn(piece.gameObject);
                }
            }

            if (tokens.transform.childCount > 0)
            {
                for (int i = tokens.transform.childCount - 1; i >= 0; i--)
                {
                    Transform token = tokens.transform.GetChild(i);
                    DestroyImmediate(token.gameObject);
                }
            }

            gameBoardView.Clear();
        }

        /// <summary>
        /// Spawns a gamepiece at the given column and row
        /// </summary>
        /// <returns>The piece.</returns>
        GameObject SpawnPiece(float posX, float posY, PlayerEnum player, PieceAnimState startingState)
        {
            //Debug.Log("SpawnPiece: x: " + posX + " y: " + posY);
            float xPos = (posX + .1f) * .972f;
            float yPos = (posY + .05f) * .96f;

            //GameObject gamePiece = LeanPool.Spawn(gamePiecePrefab, new Vector3(xPos, yPos, 10),
                //Quaternion.identity, gamePieces.transform) as GameObject;
            GameObject gamePiece = Instantiate(gamePiecePrefab, new Vector3(xPos, yPos, 10),
                Quaternion.identity, gamePieces.transform) as GameObject;
            
            gamePiece.transform.position = new Vector3(xPos, yPos, 10);

            if (challengerGamePieceId == challengedGamePieceId && player == PlayerEnum.TWO) {
                gamePiece.GetComponent<GamePiece>().SetAlternateColor(true);
            } else {
                gamePiece.GetComponent<GamePiece>().SetAlternateColor(false);
            }

            if (challengerGamePieceId > GamePieceSelectionManager.Instance.gamePieces.Count - 1) {
                challengerGamePieceId = 0;
            }
            if (challengedGamePieceId > GamePieceSelectionManager.Instance.gamePieces.Count - 1) {
                challengedGamePieceId = 0;
            }

            if (player == PlayerEnum.ONE) {
                if (startingState == PieceAnimState.MOVING) {
                    gamePiece.GetComponent<SpriteRenderer>().sprite = GamePieceSelectionManager.Instance.GetGamePieceSprite(challengerGamePieceId);
                } else {
                    gamePiece.GetComponent<SpriteRenderer>().sprite = GamePieceSelectionManager.Instance.GetGamePieceSprite(challengerGamePieceId);
                }
            } else {
                if (startingState == PieceAnimState.MOVING) {
                    gamePiece.GetComponent<SpriteRenderer>().sprite = GamePieceSelectionManager.Instance.GetGamePieceSprite(challengedGamePieceId);
                } else {
                    gamePiece.GetComponent<SpriteRenderer>().sprite = GamePieceSelectionManager.Instance.GetGamePieceSprite(challengedGamePieceId);
                }
            }
            gamePiece.GetComponent<SpriteRenderer>().enabled = true;

            return gamePiece;
        }

        public void RematchPassAndPlayGame()
        {
            ViewController.instance.ChangeView(ViewController.instance.viewGameboardSelection);
            ViewGameBoardSelection.instance.TransitionToViewGameBoardSelection(GameType.PASSANDPLAY);
            // TransitionToGameOptionsScreen(GameType.PASSANDPLAY);
            AnalyticsManager.LogCustom("rematch_pnp_game");
        }

        public void RetryPuzzleChallenge() {
            PuzzleChallengeInfo puzzleChallenge = PuzzleChallengeLoader.instance.GetChallenge();
            if (puzzleChallenge ==  null) {
                alertUI.Open(LocalizationManager.Instance.GetLocalizedValue("all_challenges_completed"));
            } else {
                ResetGameManagerState();
                puzzleChallengeInfo = puzzleChallenge;
                ResetUIGameScreen();
                gameScreen.GetComponent<CanvasGroup>().alpha = 0.0f;
                ResetGamePiecesAndTokens();

                TokenBoard initialTokenBoard = new TokenBoard(puzzleChallenge.InitialTokenBoard.ToArray(), "", "", null, null, true);

                gameState = new GameState(Constants.numRows, Constants.numColumns, GameType.PUZZLE, true, true, initialTokenBoard, puzzleChallenge.InitialGameBoard.ToArray(), false, null);

                isCurrentPlayer_PlayerOne = true;
                isPuzzleChallenge = true;
                // gameType = GameType.PUZZLE;

                UpdatePlayerUI();

                string subtitle = "";
                if (puzzleChallenge.MoveGoal > 1)
                {
                    subtitle = LocalizationManager.Instance.GetLocalizedValue("puzzle_challenge_win_instructions_plural");
                }
                else
                {
                    subtitle = LocalizationManager.Instance.GetLocalizedValue("puzzle_challenge_win_instructions_singular");
                }

                SetupGame(puzzleChallenge.Name, subtitle.Replace("%1", puzzleChallenge.MoveGoal.ToString()));

                retryPuzzleChallengeButton.gameObject.SetActive(false);

                FadeGameScreen(1.0f, gameScreenFadeInTime);
                isLoading = false;

                Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                customAttributes.Add("id", puzzleChallenge.ID);
                customAttributes.Add("level", puzzleChallenge.Level);
                AnalyticsManager.LogCustom("retry_puzzle_challenge", customAttributes);
            }
        }

        public void OpenNextPuzzleChallenge() {
            PuzzleChallengeInfo puzzleChallenge = PuzzleChallengeLoader.instance.GetChallenge();
            if (puzzleChallenge ==  null) {
                alertUI.Open(LocalizationManager.Instance.GetLocalizedValue("all_challenges_completed"));
            } else {
                ResetGameManagerState();

                puzzleChallengeInfo = puzzleChallenge;
                ResetUIGameScreen();
                gameScreen.GetComponent<CanvasGroup>().alpha = 0.0f;
                ResetGamePiecesAndTokens();

                TokenBoard initialTokenBoard = new TokenBoard(puzzleChallenge.InitialTokenBoard.ToArray(), "", "", null, null, true);

                gameState = new GameState(Constants.numRows, Constants.numColumns, GameType.PUZZLE, true, true, initialTokenBoard, puzzleChallenge.InitialGameBoard.ToArray(), false, null);

                isCurrentPlayer_PlayerOne = true;
                isPuzzleChallenge = true;
                UpdatePlayerUI();

                string subtitle = "";
                if (puzzleChallenge.MoveGoal > 1) {
                    subtitle = LocalizationManager.Instance.GetLocalizedValue("puzzle_challenge_win_instructions_plural");
                } else {
                    subtitle = LocalizationManager.Instance.GetLocalizedValue("puzzle_challenge_win_instructions_singular");
                }

                SetupGame(puzzleChallenge.Name, subtitle.Replace("%1", puzzleChallenge.MoveGoal.ToString()));

                nextPuzzleChallengeButton.gameObject.SetActive(false);

                FadeGameScreen(1.0f, gameScreenFadeInTime);
                isLoading = false;
                Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                customAttributes.Add("id", puzzleChallenge.ID);
                customAttributes.Add("level", puzzleChallenge.Level);
                AnalyticsManager.LogCustom("retry_puzzle_challenge", customAttributes);
            }
        }

        private void DisplayLoginError()
        {
            ErrorPanel.SetActive(true);
        }

        public Position GetPositonFromTransform(Vector3 pos) {
            int column = Mathf.RoundToInt(pos.x);
            int row = Mathf.CeilToInt((pos.y * -1 - .3f));

            return new Position(column, row);
        }

        private GameObject SpawnParticle()
        {
            GameObject particles = (GameObject)Instantiate(particleEffect);
            particles.transform.position = new Vector3(particles.transform.position.x, particles.transform.position.y, 0);
//#if UNITY_3_5
//            particles.SetActiveRecursively(true);
//#else
            particles.SetActive(true);
            //          for(int i = 0; i < particles.transform.childCount; i++)
            //              particles.transform.GetChild(i).gameObject.SetActive(true);
//#endif

            ParticleSystem ps = particles.GetComponent<ParticleSystem>();

//#if UNITY_5_5_OR_NEWER
//            if (ps != null)
//            {
//                var main = ps.main;
//                if (main.loop)
//                {
//                    ps.gameObject.AddComponent<CFX_AutoStopLoopedEffect>();
//                    ps.gameObject.AddComponent<CFX_AutoDestructShuriken>();
//                }
//            }
//#endif

            //onScreenParticles.Add(particles);

            return particles;
        }

        private void ProcessPlayerInput(Vector3 mousePosition)
        {
            //Debug.Log("Gamemanager: disableInput: " + disableInput);
            //Debug.Log("Gamemanager: isLoading: " + isLoading);
            //Debug.Log("Gamemanager: isLoadingUI: " + isLoadingUI);
            //Debug.Log("Gamemanager: isDropping: " + isDropping);
            //Debug.Log("Gamemanager: gameState.isGameOver: " + gameState.isGameOver);
            //Debug.Log("Gamemanager: animatingGamePieces: " + animatingGamePieces);

            Vector3 pos = mainCamera.ScreenToWorldPoint(mousePosition);
            //GameObject particle = SpawnParticle();
            //particle.transform.position = new Vector3(Mathf.RoundToInt(pos.x), Mathf.CeilToInt((pos.y * -1 - .3f))*-1);

            if (disableInput || isLoading || isLoadingUI || isDropping || gameState.IsGameOver || animatingGamePieces)
            {
                Debug.Log("returned in process player input");
                return;
            }

            // Debug.Log("Gamemanager: gameState.isCurrentPlayerTurn: " + gameState.isCurrentPlayerTurn);

            if (gameState.isCurrentPlayerTurn)
            {
                // round to a grid square
                //Debug.Log("Move Position: x: " + pos.x + " y: " + (pos.y * -1 - .3f));
                int column = Mathf.RoundToInt(pos.x);
                int row = Mathf.CeilToInt((pos.y * - 1 - .3f));

                Position position;
                PlayerEnum player = gameState.IsPlayerOneTurn ? PlayerEnum.ONE : PlayerEnum.TWO;
                //Debug.Log("ProcessPlayerInput: column: " + column + " row: " + row);
                if (inTopRowBounds(pos.x, pos.y))
                {
                    position = new Position(column, row - 1);
                    //Debug.Log("Move Position: col: " + position.column + " row: " + position.row);
                    Move move = new Move(position, Direction.DOWN, player);
                    StartCoroutine(ProcessMove(move));
                }
                else if (inBottomRowBounds(pos.x, pos.y))
                {
                    position = new Position(column, row + 1);
                    Move move = new Move(position, Direction.UP, player);
                    //Debug.Log("Move Position: col: " + position.column + " row: " + position.row);
                    StartCoroutine(ProcessMove(move));
                }
                else if (inRightRowBounds(pos.x, pos.y))
                {
                    position = new Position(column + 1, row);
                    Move move = new Move(position, Direction.LEFT, player);
                    //Debug.Log("Move Position: col: " + position.column + " row: " + position.row);
                    StartCoroutine(ProcessMove(move));
                }
                else if (inLeftRowBounds(pos.x, pos.y))
                {
                    position = new Position(column - 1, row);
                    Move move = new Move(position, Direction.RIGHT, player);
                    //Debug.Log("Move Position: col: " + position.column + " row: " + position.row);
                    StartCoroutine(ProcessMove(move));
                }
            }
            else
            {
                Debug.Log("Not isCurrentPlayerTurn: challengeInstanceId: " + challengeInstanceId);
                if (challengeInstanceId != null) {
                    if (inTopRowBounds(pos.x, pos.y) || inBottomRowBounds(pos.x, pos.y) || inRightRowBounds(pos.x, pos.y) || inLeftRowBounds(pos.x, pos.y))
                    {
                        alertUI.Open(LocalizationManager.Instance.GetLocalizedValue("not_your_turn"));
                    }
                }
            }
        }

        public IEnumerator ProcessMove(Move move)
        {
            isDropping = true;
            bool updatePlayer = true;

            if (gameState.CanMove(move.GetNextPosition(), gameState.TokenBoard.tokens))
            {
                moveHintArea.SetActive(false);
                Console.Log("isMultiplayer: ", isMultiplayer, "isNewChallenge", isNewChallenge, "isNewRandomChallenge", isNewRandomChallenge);
                if (isMultiplayer && !isNewChallenge && !isNewRandomChallenge)
                {
                    replayedLastMove = false;
                    StartCoroutine(MovePiece(move, false, updatePlayer));
                    Debug.Log("LogChallengeEventRequest: challengeInstanceId: " + challengeInstanceId);
                    new LogChallengeEventRequest().SetChallengeInstanceId(challengeInstanceId)
                        .SetEventKey("takeTurn") //The event we are calling is "takeTurn", we set this up on the GameSparks Portal
                        .SetEventAttribute("pos", Utility.GetMoveLocation(move)) // pos is the row or column the piece was placed at depending on the direction
                        .SetEventAttribute("direction", move.direction.GetHashCode()) // direction can be up, down, left, right
                        .SetEventAttribute("player", gameState.IsPlayerOneTurn ? (int)Piece.BLUE : (int)Piece.RED)
                        .SetDurable(true)
                        .Send((response) =>
                            {
                                if (response.HasErrors)
                                {
                                    Debug.Log("***** ChallengeEventRequest failed: " + response.Errors.JSON);
                                    alertUI.Open("There was a problem making your move. Please try again.");
                                }
                                else
                                {
                                    Debug.Log("ChallengeEventRequest was successful");
                                }
                            });

                    while (isDropping)
                        yield return null;
                }
                else if (isMultiplayer && isNewChallenge)
                {
                    isNewChallenge = false;
                    StartCoroutine(MovePiece(move, false, updatePlayer));
                    ChallengeManager.instance.ChallengeUser(opponentUserId, gameState, Utility.GetMoveLocation(move), move.direction);
                }
                else if (isMultiplayer && isNewRandomChallenge)
                {
                    isNewRandomChallenge = false;
                    StartCoroutine(MovePiece(move, false, updatePlayer));
                    ChallengeManager.instance.ChallengeRandomUser(gameState, Utility.GetMoveLocation(move), move.direction);
                }
                else if (isAiActive)
                {
                    StartCoroutine(MovePiece(move, false, updatePlayer));

                    if (gameState.IsGameOver)
                    {
                        yield break;
                    }

                    while (isDropping)
                        yield return null;
                    isDropping = true;
                    yield return new WaitForSeconds(1f);

                    AiPlayer aiPlayer = new AiPlayer(AIPlayerSkill.LEVEL1);

                    StartCoroutine(aiPlayer.MakeMove(move));
                }
                else
                {
                    StartCoroutine(MovePiece(move, false, updatePlayer));
                    if (isPuzzleChallenge && !gameState.IsGameOver && !gameState.IsPuzzleChallengeCompleted) {

                        yield return new WaitWhile(() => isDropping == true);

                        bool foundMove = false;

                        // Check if the current board state matches a board state in the list of boards states for the puzzle challenge
                        foreach (var puzzleMove in puzzleChallengeInfo.Moves)
                        {
                            string gameBoardString = string.Join("", gameState.GetGameBoardArray().Select(item => item.ToString()).ToArray());
                            string puzzleMoveBoardString = string.Join("", puzzleMove.BoardState.Select(item => item.ToString()).ToArray());

                            puzzleMoveBoardString = Regex.Replace(puzzleMoveBoardString, "9", "[0-2]{1}");
                            puzzleMoveBoardString = Regex.Replace(puzzleMoveBoardString, "3", "[1-2]{1}");

                            if (Regex.IsMatch(gameBoardString, puzzleMoveBoardString)) {
                                //Debug.Log("FOUND MOVE");
                                Move newMove = new Move(puzzleMove.Location, (Direction)puzzleMove.Direction, PlayerEnum.TWO);
                                //Debug.Log("move direction: " + puzzleMove.Direction);
                                //Debug.Log("move location: " + puzzleMove.Location);
                                StartCoroutine(MovePiece(newMove, false, true));
                                foundMove = true;
                                break;
                            }
                        }
                        if (!foundMove) {
                            //Debug.Log("!foundMove");
                            // Make Random Move
                            float random = UnityEngine.Random.Range(0.0f, 1.0f);
                            //Debug.Log("random number: " + random);
                            float cumulative = 0.0f;
                            foreach (var randomMove in puzzleChallengeInfo.RandomMoves)
                            {
                                cumulative += randomMove.Weight;
                                //Debug.Log("randomMove.weight: " + randomMove.Weight);
                                //Debug.Log("cumulative: " + cumulative);
                                if (random < cumulative)
                                {
                                    Move newMove = new Move(randomMove.Location, (Direction)randomMove.Direction, PlayerEnum.TWO);
                                    //Debug.Log("RandomMove");
                                    if (gameState.CanMove(newMove.GetNextPosition(), gameState.TokenBoard.tokens)) {
                                        StartCoroutine(MovePiece(newMove, false, true));
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            } else {
                alertUI.Open("Nope, not possible");
                isDropping = false;
            }

            yield return null;
        }

        public void CreateGame(string challengeId, string opponentId) {
            Opponent opponent = new Opponent(opponentId, opponentNameLabel.text, opponentFacebookId);
            Game game = new Game(challengeId, gameState, isCurrentPlayer_PlayerOne, isExpired, didViewResult, opponent, ChallengeState.RUNNING, ChallengeType.STANDARD, challengerGamePieceId.ToString(), challengedGamePieceId.ToString());
            game.gameState.SetRandomGuid(challengeId);
            games.Add(game);
        }

        public void CallMovePiece(Move move, bool replayMove, bool updatePlayer)
        {
            StartCoroutine(MovePiece(move, replayMove, updatePlayer));
        }

        private IEnumerator MovePiece(Move move, bool replayMove, bool updatePlayer)
        {
            // GameObject[] cornerArrows = GameObject.FindGameObjectsWithTag("Arrow");
            // foreach (GameObject cornerArrow in cornerArrows) {
            //     SpriteRenderer sr = cornerArrow.GetComponentInChildren<SpriteRenderer>();
            //     sr.enabled = false;
            // }

            if (OnStartMove != null)
                OnStartMove();

            isDropping = true;
            SoundManager.instance.PlayRandomizedSfx(clipMove);

            List<IToken> activeTokens;

            gameState.PrintGameState("BeforeMove");
            List<MovingGamePiece> movingPieces = gameState.MovePiece(move, replayMove, out activeTokens);
            gameState.PrintGameState("AfterMove");

            MoveGamePieceViews(move, movingPieces, activeTokens);

            yield return new WaitWhile(() => animatingGamePieces == true);

            if (!replayMove || gameState.IsGameOver || gameState.IsPuzzleChallengeCompleted)
            {
                SetActionButton();
            }

            if (gameState.IsPuzzleChallengeCompleted)
            {
                if (gameState.IsPuzzleChallengePassed)
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

            if (isPuzzleChallenge)
            {
                if (gameState.IsPuzzleChallengeCompleted)
                {
                    if (gameState.IsPuzzleChallengePassed)
                    {
                        PlayerPrefs.SetInt("puzzleChallengeLevel", puzzleChallengeInfo.Level);
                        AnalyticsManager.LogPuzzleChallenge(puzzleChallengeInfo, true, gameState.Player1MoveCount);
                    }
                    else
                    {
                        AnalyticsManager.LogPuzzleChallenge(puzzleChallengeInfo, false, gameState.Player1MoveCount);
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
            if (isMultiplayer) {
                var currentGame = games
                    .Where(t => t.challengeId == challengeInstanceId)
                    .FirstOrDefault();
                if (currentGame != null)
                {
                    //Debug.Log("Updated Game State");
                    currentGame.gameState = gameState;
                }
            }

            if (gameState.IsGameOver) {
                if (OnGameOver != null)
                    OnGameOver();
            }

            if (OnEndMove != null)
                OnEndMove();
            
            yield return true;
        }

        private void UpdateGameStatus(bool updatePlayer)
        {
            if (gameState.IsGameOver || isExpired)
            {
                DisplayGameOverView();
            }
            else
            {
                Debug.Log("UpdateGameStatus: updatePlayer: " + updatePlayer + ", gameState.isCurrentPlayerTurn: "+ gameState.isCurrentPlayerTurn);
                if (updatePlayer)
                {
                    if (isMultiplayer || isAiActive)
                    {
                        gameState.isCurrentPlayerTurn = !gameState.isCurrentPlayerTurn;
                    }
                }
                UpdatePlayerUI();
            }
        }

        private void VisitedGameResults() {
            var currentGame = games
                .Where(t => t.challengeId == challengeInstanceId).FirstOrDefault();
            if (currentGame != null)
            {
                if (!currentGame.didViewResult && gameState.IsGameOver)
                {
                    ChallengeManager.instance.SetViewedCompletedGame(challengeInstanceId);
                    currentGame.didViewResult = true;
                }
            }
        }

        private void MoveGamePieceViews(Move move, List<MovingGamePiece> movingPieces, List<IToken> activeTokens)
        {
            animatingGamePieces = true;

            // Create Game Piece View
            GameObject g = SpawnPiece(move.position.column, move.position.row * -1, move.player, PieceAnimState.MOVING);
            GamePiece gamePiece = g.GetComponent<GamePiece>();
            gamePiece.player = move.player;
            gamePiece.column = move.position.column;
            gamePiece.row = move.position.row;

            //var x = new List<MovingGamePiece>(0);
            //CoroutineWithData cd = new CoroutineWithData(this, gamePiece.MoveGamePiece(x));
            //yield return cd.coroutine;
            //Debug.Log("MoveGamePiece success: " + cd.result.ToString());

            StartCoroutine(gamePiece.MoveGamePiece(movingPieces, activeTokens));

            gameBoardView.PrintGameBoard();
        }

        private void WinLineSetActive(bool active)
        {
            LineRenderer[] winLines = gameScreen.GetComponentsInChildren<LineRenderer>(true);

            foreach (var line in winLines)
            {
                if (line)
                {
                    line.gameObject.SetActive(active);
                }
            }
        }

        public void DisplayGameOverView()
        {
            bool showRewardButton = false;

            if (isExpired)
            {
                VisitedGameResults();
                gameInfo.Open(LocalizationManager.Instance.GetLocalizedValue("expired_text"), Color.white, false, showRewardButton);
                return;
            }

            if (!didViewResult) {
                showRewardButton = true;
            }

            // Color winnerTextColor = gameState.Winner == PlayerEnum.ONE ? bluePlayerColor : redPlayerColor;
            Color winnerTextColor = Color.white;
            Debug.Log("DisplayGameOverView gameState.winner: " +  gameState.Winner);
            if (gameState.Winner == PlayerEnum.ONE)
            {
                int size = gameState.GameBoard.player1WinningPositions.Count;
                Vector3 startPos = new Vector3((gameState.GameBoard.player1WinningPositions[0].column + .1f) *.972f, (gameState.GameBoard.player1WinningPositions[0].row * -1 + .05f) * .96f, 12);
                Vector3 endPos = new Vector3((gameState.GameBoard.player1WinningPositions[size - 1].column+ .1f) * .972f, (gameState.GameBoard.player1WinningPositions[size - 1].row * -1 + .05f) * .96f, 12);
                DrawLine(startPos, endPos, bluePlayerColor);

                if (gameState.isCurrentPlayerTurn)
                {
#if UNITY_IOS || UNITY_ANDROID
                    Handheld.Vibrate();
#endif
                }
            }

            if (gameState.Winner == PlayerEnum.TWO)
            {
                int size = gameState.GameBoard.player2WinningPositions.Count;
                Vector3 startPos = new Vector3((gameState.GameBoard.player2WinningPositions[0].column + .1f) * .972f, (gameState.GameBoard.player2WinningPositions[0].row * -1 + .05f) * .96f, 12);
                Vector3 endPos = new Vector3((gameState.GameBoard.player2WinningPositions[size - 1].column + .1f) * .972f, (gameState.GameBoard.player2WinningPositions[size - 1].row * -1 + .05f) *.96f, 12);
                DrawLine(startPos, endPos, redPlayerColor);

                if (gameState.isCurrentPlayerTurn)
                {
#if UNITY_IOS || UNITY_ANDROID
                    Handheld.Vibrate();
#endif
                }
            }

            if (gameState.Winner == PlayerEnum.NONE || gameState.Winner == PlayerEnum.ALL)
            {
                if (isMultiplayer) {
                    gameInfo.Open(LocalizationManager.Instance.GetLocalizedValue("draw_text"), Color.white, false, showRewardButton);    
                } else {
                    if (!isPuzzleChallenge)
                    {
                        gameInfo.Open(LocalizationManager.Instance.GetLocalizedValue("draw_text"), Color.white, false, false);
                    }
                }

                if (gameState.Winner == PlayerEnum.ALL) {
                    int size = gameState.GameBoard.player1WinningPositions.Count;
                    Debug.Log("player1WinningPositions.Count: " + gameState.GameBoard.player1WinningPositions.Count);
                    Debug.Log("player2WinningPositions.Count: " + gameState.GameBoard.player2WinningPositions.Count);
                    Vector3 startPos = new Vector3((gameState.GameBoard.player1WinningPositions[0].column + .1f) *.972f, (gameState.GameBoard.player1WinningPositions[0].row * -1 + .05f) * .96f, 12);
                    Vector3 endPos = new Vector3((gameState.GameBoard.player1WinningPositions[size - 1].column+ .1f) * .972f, (gameState.GameBoard.player1WinningPositions[size - 1].row * -1 + .05f) * .96f, 12);
                    DrawLine(startPos, endPos, bluePlayerColor);

                    int size2 = gameState.GameBoard.player2WinningPositions.Count;
                    Vector3 startPos2 = new Vector3((gameState.GameBoard.player2WinningPositions[0].column + .1f) * .972f, (gameState.GameBoard.player2WinningPositions[0].row * -1 + .05f) * .96f, 12);
                    Vector3 endPos2 = new Vector3((gameState.GameBoard.player2WinningPositions[size2 - 1].column + .1f) * .972f, (gameState.GameBoard.player2WinningPositions[size2 - 1].row * -1 + .05f) *.96f, 12);
                    DrawLine(startPos2, endPos2, redPlayerColor); 
                }
            }
            else if (isMultiplayer)
            {
                if (winner != null && winner.Length > 0)
                {
                    gameInfo.Open(winner + LocalizationManager.Instance.GetLocalizedValue("won_suffix"), winnerTextColor, true, showRewardButton);
                    //SoundManager.instance.PlayRandomizedSfx(clipWin);
                }
                else
                {                    
                    if (isCurrentPlayer_PlayerOne && gameState.Winner == PlayerEnum.ONE)
                    {
                        SoundManager.instance.PlayRandomizedSfx(clipWin);
                        gameInfo.Open(UserManager.instance.userName + LocalizationManager.Instance.GetLocalizedValue("won_suffix"), winnerTextColor, true, showRewardButton);
                    }
                    else if (!isCurrentPlayer_PlayerOne && gameState.Winner == PlayerEnum.TWO)
                    {
                        SoundManager.instance.PlayRandomizedSfx(clipWin);
                        gameInfo.Open(UserManager.instance.userName + LocalizationManager.Instance.GetLocalizedValue("won_suffix"), winnerTextColor, true, showRewardButton);
                    }
                    else
                    {
                        gameInfo.Open(opponentNameLabel.text + LocalizationManager.Instance.GetLocalizedValue("won_suffix"), winnerTextColor, true, showRewardButton);
                    }

                    //AnalyticsManager.LogGameOver("multiplayer", gameState.winner, gameState.tokenBoard);
                }
            }
            else
            {
                SoundManager.instance.PlayRandomizedSfx(clipWin);
                if (!isPuzzleChallenge) {
                    //AnalyticsManager.LogGameOver("pnp", gameState.winner, gameState.tokenBoard);
                    string winnerText = gameState.Winner == PlayerEnum.ONE ? playerOneWonText : playerTwoWonText;
                    gameInfo.Open(winnerText, winnerTextColor, true, false);
                }
            }

            WinLineSetActive(true);

            switch (gameState.GameType)
            {
                case GameType.FRIEND:
                    AnalyticsManager.LogGameOver("friend", gameState.Winner, gameState.TokenBoard);
                    break;
                case GameType.LEADERBOARD:
                    AnalyticsManager.LogGameOver("leaderboard", gameState.Winner, gameState.TokenBoard);
                    break;
                case GameType.AI:
                    AnalyticsManager.LogGameOver("AI", gameState.Winner, gameState.TokenBoard);
                    break;
                case GameType.PASSANDPLAY:
                    AnalyticsManager.LogGameOver("pnp", gameState.Winner, gameState.TokenBoard);
                    break;
                case GameType.PUZZLE:
                    //AnalyticsManager.LogGameOver("puzzle", gameState.winner, gameState.tokenBoard);
                    break;
                default:
                    AnalyticsManager.LogGameOver("random", gameState.Winner, gameState.TokenBoard);
                    break;
            }
        }

        void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            //Debug.Log("START X: " + start.x + " START Y: " + start.y);
            //Debug.Log("END X: " + end.x + " END Y: " + end.y);
            //Debug.Log("DRAWLINE");
            GameObject myLine = new GameObject("WinLine");
            myLine.tag = "WinLine";
            myLine.transform.parent = tokens.transform;
            myLine.transform.position = start;
            myLine.AddComponent<LineRenderer>();
            LineRenderer lr = myLine.GetComponent<LineRenderer>();
            lr.material = new Material(lineShader);
            lr.startColor = color;
            lr.endColor = color;
            lr.startWidth = 0.15f;
            lr.endWidth = 0.15f;
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            myLine.SetActive(false);
            //GameObject.Destroy(myLine, duration);
        }

        bool inTopRowBounds(float x, float y)
        {
            return x > 0.5 && x < Constants.numColumns - 1.5 && y > -0.46 && y < 1.2;
        }

        bool inBottomRowBounds(float x, float y)
        {
            return x > 0.5 && x < Constants.numColumns - 1.5 && y > -Constants.numColumns && y < -Constants.numColumns + 2;
        }

        bool inLeftRowBounds(float x, float y)
        {
            return x > -0.8 && x < 0.66 && y > -Constants.numColumns + 1.3 && y < -0.5;
        }

        bool inRightRowBounds(float x, float y)
        {
            return x > Constants.numColumns - 1.7 && x < Constants.numColumns - 0.5 && y > -Constants.numColumns + 1.5 && y < -0.5;
        }
    }
}
