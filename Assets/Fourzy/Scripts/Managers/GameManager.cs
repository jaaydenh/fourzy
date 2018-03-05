﻿using UnityEngine;
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
        public delegate void MoveAction();
        public static event MoveAction OnMoved;
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
        // ---------- End Token Views ----------

        public PuzzleChallengeInfo puzzleChallengeInfo;
        public GameBoardView gameBoardView;
        public GameState gameState { get; set; }
        //public List<GameObject> tokenViews;
        public GameObject[,] tokenViews;
        public List<Game> games = new List<Game>();
        public GameObject gamePiecePrefab;
        public Sprite playerOneSpriteMoving;
        public Sprite playerOneSpriteAsleep;
        public Sprite playerTwoSpriteMoving;
        public Sprite playerTwoSpriteAsleep;
        private string bluePlayerWonText = "Blue Player Won!";
        private string redPlayerWonText = "Red Player Won!";
        public string winner;
        public string opponentFacebookId;

        public Color bluePlayerColor = new Color(0f / 255f, 176.0f / 255f, 255.0f / 255.0f);
        public Color redPlayerColor = new Color(254.0f / 255.0f, 40.0f / 255.0f, 81.0f / 255.0f);
        public Shader lineShader = null;

        [Header("Game UI")]
        public GameObject gamePieces;
        public GameObject tokens;
        public GameObject gameScreen;
        public GameObject CreateGameScreen;
        public GameObject GameOptionsScreen;
        public GameObject ErrorPanel;
        public GameObject UIScreen;
        public Text playerNameLabel;
        public Image playerProfilePicture;
        public Image playerPiece;
        public Text opponentNameLabel;
        public Image opponentProfilePicture;
        public Image opponentPiece;
        public Text gameStatusText;
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

        [Header("Game State")]
        public GameType gameType;
        public bool isMultiplayer = false;
        public bool isNewChallenge = false;
        public bool isNewRandomChallenge = false;
        public bool isPuzzleChallenge = false;
        public bool isAiActive = false;
        public bool isCurrentPlayer_PlayerOne;
        public bool isLoading = true;
        public bool isLoadingUI;
        private bool isDropping = false;
        public bool isAnimating = false;
        public bool animatingGamePieces = false;
        private bool replayedLastMove = false;
        private bool isPuzzleChallengeCompleted = false;
        private bool isPuzzleChallengePassed = false;
        public bool isExpired = false;
        public bool didViewResult = false;
        public string opponentLeaderboardRank = "";
        public Screens activeScreen = Screens.NONE;
        public int challengerGamePieceId;
        public int challengedGamePieceId;

        //int spacing = 1; //100
        //int offset = 0; //4
        public AudioClip clipMove;
        public AudioClip clipWin;
        public AudioClip clipChest;
        private AudioSource audioMove;
        private AudioSource audioWin;
        private AudioSource audioChest;
        //public CreateGame createGameScript;
        public GameObject moveArrowLeft;
        public GameObject moveArrowRight;
        public GameObject moveArrowDown;
        public GameObject moveArrowUp;
        public GameObject cornerSpot;
        public MoveHintTouchArea moveHintTouchArea;

        //Static instance of GameManager which allows it to be accessed by any other script.
        //public static GameManager instance = null;

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
            ChallengeManager.OnChallengeIssuedDelegate -= ChallengeIssuedHandler;
            //SceneManager.sceneLoaded -= OnGameFinishedLoading;
        }

        void Start()
        {
            ChallengeAcceptedMessage.Listener = (message) =>
            {
                var gsChallenge = message.Challenge;
                Debug.Log("ChallengeAcceptedMessage");
                Debug.Log("gsChallenge.Challenger.Id: " + gsChallenge.Challenger.Id);
                Debug.Log("gsChallenge.Challenged: " + gsChallenge.Challenged);
                if (UserManager.instance.userId == gsChallenge.NextPlayer)
                {
                    Debug.Log("UserManager.instance.userId == gsChallenge.NextPlayer");
                }
            };

            ChallengeStartedMessage.Listener = (message) =>
            {
                var gsChallenge = message.Challenge;
                Debug.Log("ChallengeStartedMessage");
                Debug.Log("gsChallenge.Challenger.Id: " + gsChallenge.Challenger.Id);
                Debug.Log("gsChallenge.Challenged: " + gsChallenge.Challenged);
                if (UserManager.instance.userId == gsChallenge.NextPlayer)
                {
                    Debug.Log("UserManager.instance.userId == gsChallenge.NextPlayer");
                }
            };

            rematchButton.gameObject.SetActive(false);
            gameScreen.SetActive(false);

            if (tokens == null)
            {
                tokens = new GameObject("Tokens");
                tokens.transform.parent = gameScreen.transform;
            }

            activeScreen = Screens.GAMES_LIST;

#if UNITY_IOS
            UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
            UnityEngine.iOS.NotificationServices.ClearRemoteNotifications();
#endif

            //GamePieceSelectionManager.instance.LoadGamePieces();
            // center camera
            //Camera.main.transform.position = new Vector3((Constants.numColumns - 1) / 2.0f, -((Constants.numRows - 1) / 2.0f) + .15f, Camera.main.transform.position.z);
        }

        new void Awake()
        {
            base.Awake();

            GS.GameSparksAvailable += CheckConnectionStatus;

            DOTween.Init(false, true, LogBehaviour.ErrorsOnly);

            audioMove = AddAudio(clipMove, false, false, 1);
            audioWin = AddAudio(clipWin, false, false, 1);
            audioChest = AddAudio(clipChest, false, false, 1);

            replayedLastMove = false;
        }

        private void OpponentTurnHandler(ChallengeTurnTakenMessage message) {
            //Debug.Log("ChallengeTurnTakenMessage active screen: " + activeScreen.ToString());

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
                        GameState newGameState = new GameState(Constants.numRows, Constants.numColumns, challenge, true, PlayerEnum.EMPTY);
                        currentGame.gameState = newGameState;
                    }

                    if (activeScreen == Screens.GAME)
                    {
                        SetActionButton();
                    }

                    if (activeScreen == Screens.GAMES_LIST)
                    {
                        //ChallengeManager.instance.GetChallenges();
                        ChallengeManager.instance.ReloadGames();
                    }
                }
            }
        }

        private void ChallengeJoinedHandler(ChallengeJoinedMessage message) {
            var gsChallenge = message.Challenge;
            string opponentName = gsChallenge.Challenged.FirstOrDefault().Name;
            string opponentFBId = gsChallenge.Challenged.FirstOrDefault().ExternalIds.GetString("FB");
            //Debug.Log("JSON: " + message.Challenge.JSONString);
            //Debug.Log("gsChallenge.Challenged.FirstOrDefault().Id: " + gsChallenge.Challenged.FirstOrDefault().Id);
            //challengedGamePieceId = int.Parse(gsChallenge.Challenged.FirstOrDefault().Id);
            ChallengeManager.instance.GetOpponentGamePiece(gsChallenge.Challenged.FirstOrDefault().Id, gsChallenge.ChallengeId);

            var currentGame = games
                .Where(t => t.challengeId == gsChallenge.ChallengeId)
                .FirstOrDefault();
            if (currentGame != null)
            {
                //Debug.Log("ChallengeJoinedMessage set current games playerdata");
                Opponent opponent = new Opponent(opponentName, opponentFBId);
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

        private void ChallengeWonHandler(ChallengeWonMessage message) {
            var gsChallenge = message.Challenge;
            if (UserManager.instance.userId != gsChallenge.NextPlayer)
            {
                int currentPlayerMove = gsChallenge.ScriptData.GetInt("currentPlayerMove").GetValueOrDefault();
                PlayerEnum player = currentPlayerMove == 1 ? PlayerEnum.ONE : PlayerEnum.TWO;

                // Only Replay the last move if the player is viewing the game screen for that game
                if (gsChallenge.ChallengeId == challengeInstanceId)
                {
                    List<GSData> moveList = gsChallenge.ScriptData.GetGSDataList("moveList");
                    GSData lastMove = moveList.Last();

                    StartCoroutine(ReplayIncomingOpponentMove(lastMove, player));
                    //ChallengeManager.instance.SetViewedCompletedGame(challenge.ChallengeId);
                    SetActionButton();
                }
                else
                {
                    var currentGame = games
                        .Where(t => t.challengeId == gsChallenge.ChallengeId)
                        .FirstOrDefault();
                    if (currentGame != null)
                    {
                        GameSparksChallenge challenge = new GameSparksChallenge(gsChallenge);
                        GameState newGameState = new GameState(Constants.numRows, Constants.numColumns, challenge, true, player);
                        currentGame.gameState = newGameState;
                    }

                    if (activeScreen == Screens.GAME)
                    {
                        SetActionButton();
                    }

                    if (activeScreen == Screens.GAMES_LIST)
                    {
                        //ChallengeManager.instance.GetChallenges();
                        ChallengeManager.instance.ReloadGames();
                    }
                }
            }
        }

        private void ChallengeLostHandler(ChallengeLostMessage message) {
            var gsChallenge = message.Challenge;
            if (UserManager.instance.userId != gsChallenge.NextPlayer)
            {
                int currentPlayerMove = gsChallenge.ScriptData.GetInt("currentPlayerMove").GetValueOrDefault();
                PlayerEnum player = currentPlayerMove == 1 ? PlayerEnum.ONE : PlayerEnum.TWO;

                // Only Replay the last move if the player is viewing the game screen for that game
                if (gsChallenge.ChallengeId == challengeInstanceId)
                {
                    List<GSData> moveList = gsChallenge.ScriptData.GetGSDataList("moveList");
                    GSData lastMove = moveList.Last();

                    StartCoroutine(ReplayIncomingOpponentMove(lastMove, player));
                    //ChallengeManager.instance.SetViewedCompletedGame(challenge.ChallengeId);
                    SetActionButton();
                }
                else
                {
                    var currentGame = games
                        .Where(t => t.challengeId == gsChallenge.ChallengeId)
                        .FirstOrDefault();
                    if (currentGame != null)
                    {
                        GameSparksChallenge challenge = new GameSparksChallenge(gsChallenge);
                        GameState newGameState = new GameState(Constants.numRows, Constants.numColumns, challenge, true, player);
                        currentGame.gameState = newGameState;
                    }

                    if (activeScreen == Screens.GAME)
                    {
                        SetActionButton();
                    }

                    if (activeScreen == Screens.GAMES_LIST)
                    {
                        //ChallengeManager.instance.GetChallenges();
                        ChallengeManager.instance.ReloadGames();
                    }
                }
            }
        }

        private void ChallengeIssuedHandler(ChallengeIssuedMessage message) {
            var gsChallenge = message.Challenge;
            //Debug.Log("ChallengeIssuedMessage");
            //Debug.Log("gsChallenge.Challenger.Id: " + gsChallenge.Challenger.Id);

            //bool isplayerOne = false;
            //if (UserManager.instance.userId == gsChallenge.NextPlayer)
            //{
            //    Debug.Log("UserManager.instance.userId == gsChallenge.NextPlayer");
            //}

            GameSparksChallenge challenge = new GameSparksChallenge(gsChallenge);
            GameState newGameState = new GameState(Constants.numRows, Constants.numColumns, challenge, true, PlayerEnum.EMPTY);
            Opponent opponent = new Opponent(gsChallenge.Challenger.Name, gsChallenge.Challenger.Id);

            Game game = new Game(gsChallenge.ChallengeId, newGameState, false, false, false, opponent, ChallengeState.RUNNING, ChallengeType.STANDARD, challenge.gameType, challenge.challengerGamePieceId, challenge.challengedGamePieceId);
            games.Add(game);

            if (challengeInstanceId != null)
            {
                SetActionButton();
            }
        }

        public void UpdateBadgeCounts() {
            int activeGamesCount = 0;

            for (int i = 0; i < games.Count(); i++)
            {
                if (games[i].gameState.isCurrentPlayerTurn == true || (games[i].didViewResult == false && games[i].gameState.isGameOver == true))
                {
                    activeGamesCount++;
                }
            }

            homeScreenPlayBadge.SetGameCount(activeGamesCount);
            if (activeGamesCount > 0) {
                homeScreenPlayBadge.gameObject.SetActive(true);
            } else {
                homeScreenPlayBadge.gameObject.SetActive(false);    
            }
        }

        void Update()
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    if (challengeInstanceId != null)
                    {
                        TransitionToGamesListScreen();
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
            isPuzzleChallengeCompleted = false;
            isPuzzleChallengePassed = false;
            puzzleChallengeInfo = null;
            gameType = GameType.NONE;
        }

        public void PlayButton() {
            int activeGamesCount = 0;
            Game game = null;
            for (int i = 0; i < games.Count(); i++)
            {
                if (games[i].gameState.isCurrentPlayerTurn == true || (games[i].didViewResult == false && games[i].gameState.isGameOver == true))
                {
                    activeGamesCount++;
                    if (game == null) {
                        game = games[i];
                    }
                }
            }

            if (game != null) {
                game.OpenGame();
            } else {
                ChallengeManager.instance.FindRandomChallenge();
            }
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

            //RewardsScreen.SetActive(true);
            bool isCurrentPlayerWinner = false;
            PlayerEnum currentPlayer = PlayerEnum.NONE;
            Debug.Log("gameState.winner: " + gameState.winner);
            Debug.Log("isCurrentPlayer_PlayerOne: " + isCurrentPlayer_PlayerOne);

            if (isCurrentPlayer_PlayerOne && gameState.winner == PlayerEnum.ONE)
            {
                isCurrentPlayerWinner = true;
            }
            else if (!isCurrentPlayer_PlayerOne && gameState.winner == PlayerEnum.TWO)
            {
                isCurrentPlayerWinner = true;
            }
            else if (isCurrentPlayer_PlayerOne && gameState.winner == PlayerEnum.TWO)
            {
                isCurrentPlayerWinner = false;
            }
            else if (!isCurrentPlayer_PlayerOne && gameState.winner == PlayerEnum.ONE)
            {
                isCurrentPlayerWinner = false;
            }
            else if (gameState.winner == PlayerEnum.ALL || gameState.winner == PlayerEnum.NONE)
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

            //audioChest.Play();
            //Debug.Log("RewardsButton gameState.PlayerPieceCount(currentPlayer): " + gameState.PlayerPieceCount(currentPlayer));
            rewardScreen.Open(isCurrentPlayerWinner, gameState.PlayerPieceCount(currentPlayer));
            gameInfo.Close();
        }

        public void RewardsScreenOkButton()
        {
            //RewardsScreen.SetActive(false);
            //rewardScreen.Close();
            rewardScreen.gameObject.SetActive(false);
            //TransitionToGamesListScreen();
        }

        private void TransitionToGameScreen()
        {
            UserInputHandler.inputEnabled = false;
            UIScreen.SetActive(false);
            CreateGameScreen.SetActive(false);
            GameOptionsScreen.SetActive(false);
            gameScreen.SetActive(true);
            FadeGameScreen(1.0f, gameScreenFadeInTime);
            StartCoroutine(WaitToEnableInput());
            HeaderUI.SetActive(false);
            activeScreen = Screens.GAME;
        }

        public void TransitionToGamesListScreen()
        {
            UserInputHandler.inputEnabled = false;
            UIScreen.SetActive(true);
            gameScreen.GetComponent<CanvasGroup>().alpha = 0.0f;
            gameScreen.SetActive(false);
            challengeInstanceId = null;
            winner = null;

            ResetGamePiecesAndTokens();
            ResetGameManagerState();
            ResetUIGameScreen();
            HeaderUI.SetActive(true);
            activeScreen = Screens.GAMES_LIST;
            ChallengeManager.instance.ReloadGames();
        }

        public void TransitionToCreateGameScreen()
        {
            ResetGameManagerState();
            UIScreen.SetActive(false);
            CreateGameScreen.SetActive(true);
            gameScreen.GetComponent<CanvasGroup>().alpha = 0.0f;
            gameScreen.SetActive(false);
            activeScreen = Screens.CREATE_GAME;
        }

        public void TransitionToGameOptionsScreen(GameType gameType, string opponentUserId = "", string opponentName = "", Image opponentProfilePicture = null, string opponentLeaderboardRank = "")
        {
            Debug.Log("TransitionToGameOptionsScreen: opponentUserId: " + opponentUserId);
            challengerGamePieceId = UserManager.instance.gamePieceId;
            if (opponentUserId != "") {
                ChallengeManager.instance.GetOpponentGamePiece(opponentUserId);    
            }

            ResetUIGameScreen();
            challengeInstanceId = null;
            this.gameType = gameType;
            this.opponentUserId = opponentUserId;
            this.opponentNameLabel.text = opponentName;
            if (opponentProfilePicture != null)
            {
                this.opponentProfilePicture.sprite = opponentProfilePicture.sprite;
            }
            this.opponentLeaderboardRank = opponentLeaderboardRank;

            BoardSelectionManager.instance.LoadMiniBoards();
            UIScreen.SetActive(false);
            CreateGameScreen.SetActive(false);
            GameOptionsScreen.SetActive(true);
            gameScreen.GetComponent<CanvasGroup>().alpha = 0.0f;
            gameScreen.SetActive(false);
            activeScreen = Screens.GAME_OPTIONS;
        }

        public void CreateGameScreenBackButton()
        {
            ResetGameManagerState();
            CreateGameScreen.SetActive(false);
            UIScreen.SetActive(true);
            activeScreen = Screens.GAMES_LIST;
            ChallengeManager.instance.ReloadGames();
        }

        public void GameOptionsScreenBackButton()
        {
            ResetGameManagerState();
            GameOptionsScreen.SetActive(false);
            UIScreen.SetActive(true);
            activeScreen = Screens.GAMES_LIST;
            ChallengeManager.instance.ReloadGames();
        }

        public void NextGame()
        {
            //challengeInstanceId = null;
            gameScreen.GetComponent<CanvasGroup>().alpha = 0.0f;
            // foreach (var game in activeGames)
            // {
            //     Debug.Log("viewedResult: " + game.viewedResult);
            //     Debug.Log("gameState.isGameOver: " + game.gameState.isGameOver);
            //     Debug.Log("gameState.isCurrentPlayerTurn: " + game.gameState.isCurrentPlayerTurn);
            // }

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
                if ((games[i].gameState.isCurrentPlayerTurn == true || (games[i].didViewResult == false && games[i].gameState.isGameOver == true)) && games[i].challengeId != challengeInstanceId)
                {
                    if (games[i] != null)
                    {
                        Debug.Log("NextGame: current game challengeid: "+ challengeInstanceId);
                        Debug.Log("NextGame:    open game challengeid: " + games[i].challengeId);
                        games[i].OpenGame();
                        AnalyticsManager.LogCustom("next_game");
                        break;
                    }
                }
            }

            //var nextGame = games
            //    .Where(t => (t.gameState.isCurrentPlayerTurn == true || (t.didViewResult == false && t.gameState.isGameOver == true)) && t.challengeId != challengeInstanceId)
            //    .FirstOrDefault();
            //if (nextGame != null)
            //{
            //    nextGame.OpenGame();
            //    AnalyticsManager.LogCustom("next_game");
            //}
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

            opponentPiece.sprite = GamePieceSelectionManager.instance.gamePieces[challengedGamePieceId];

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

        private void FadePieces(float alpha, float fadeTime)
        {

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

        private void FadeGamesListScreen(float alpha, bool isActive, float fadeTime)
        {
            UIScreen.GetComponent<CanvasGroup>().DOFade(alpha, fadeTime).OnComplete(() => UIScreenSetActive(isActive));
        }

        private void GameScreenSetActive(bool isActive)
        {
            gameScreen.SetActive(isActive);
        }

        private void UIScreenSetActive(bool isActive)
        {
            UIScreen.SetActive(isActive);
        }

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

        public AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol)
        {
            AudioSource newAudio = gameObject.AddComponent<AudioSource>();
            newAudio.clip = clip;
            newAudio.loop = loop;
            newAudio.playOnAwake = playAwake;
            newAudio.volume = vol;
            return newAudio;
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

        private void ReplayLastMove()
        {
            if (gameState.moveList != null)
            {
                //Debug.Log("ReplayLastMove: gameState.moveList.count: " + gameState.moveList.Count());
                GSData lastMove = gameState.moveList.Last();
                // Debug.Log("lastmove: " + lastMove.JSON.ToString());
                int position = lastMove.GetInt("position").GetValueOrDefault();
                Direction direction = (Direction)lastMove.GetInt("direction").GetValueOrDefault();

                PlayerEnum player = PlayerEnum.NONE;
                if (!gameState.isGameOver)
                {
                    player = gameState.isPlayerOneTurn ? PlayerEnum.TWO : PlayerEnum.ONE;
                }
                else
                {
                    player = gameState.isPlayerOneTurn ? PlayerEnum.ONE : PlayerEnum.TWO;
                }
                //Debug.Log("ReplayLastMove lastmove: position: " + position + " direction: " + direction + " player: " + player.ToString());
                Move move = new Move(position, direction, player);

                StartCoroutine(MovePiece(move, true, false));
            }
            else
            {
                isLoading = false;
            }
        }

        public void OpenNewGame() 
        {
            TokenBoard tokenBoard = ChallengeManager.instance.tokenBoard;

            if (tokenBoard == null) {
                tokenBoard = TokenBoardLoader.instance.GetTokenBoard();    
            }

            //If we initiated the challenge, we get to be player 1
            GameState newGameState = new GameState(Constants.numRows, Constants.numColumns, true, true, tokenBoard, null, false, null);
            gameState = newGameState;

            ResetGamePiecesAndTokens();
            ResetUIGameScreen();
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
                    introUISubtitle = LocalizationManager.instance.GetLocalizedValue("pnp_button");
                    break;
                case GameType.FRIEND:
                    isMultiplayer = true;
                    isNewRandomChallenge = false;
                    isNewChallenge = true;
                    isAiActive = false;
                    introUISubtitle = LocalizationManager.instance.GetLocalizedValue("friend_challenge_text");
                    break;
                case GameType.LEADERBOARD:
                    isMultiplayer = true;
                    isNewRandomChallenge = false;
                    isNewChallenge = true;
                    isAiActive = false;
                    introUISubtitle = LocalizationManager.instance.GetLocalizedValue("leaderboard_challenge_text");
                    break;
                case GameType.RANDOM:
                    isMultiplayer = true;
                    isNewRandomChallenge = true;
                    isNewChallenge = false;
                    isAiActive = false;
                    introUISubtitle = LocalizationManager.instance.GetLocalizedValue("random_opponent_button");
                    break;
                case GameType.AI:
                    isMultiplayer = false;
                    isNewRandomChallenge = false;
                    isNewChallenge = false;
                    isAiActive = true;
                    introUISubtitle = LocalizationManager.instance.GetLocalizedValue("ai_challenge_text");
                    break;
                default:
                    break;
            }

            InitPlayerUI(opponentNameLabel.text, opponentProfilePicture.sprite);
            UpdatePlayerUI();
            DisplayIntroUI(tokenBoard.name, introUISubtitle, true);
            UIScreen.SetActive(false);
            TransitionToGameScreen();

            //GameManager.instance.EnableTokenAudio();

            if (gameType == GameType.FRIEND) {
                Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                customAttributes.Add("TokenBoardId", tokenBoard.id);
                customAttributes.Add("TokenBoardName", tokenBoard.name);
                AnalyticsManager.LogCustom("open_new_friend_challenge", customAttributes);
            } else if (gameType == GameType.LEADERBOARD) {
                Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                customAttributes.Add("TokenBoardId", tokenBoard.id);
                customAttributes.Add("TokenBoardName", tokenBoard.name);
                customAttributes.Add("Rank", opponentLeaderboardRank);
                AnalyticsManager.LogCustom("open_new_leaderboard_challenge", customAttributes);
            } else if (gameType == GameType.PASSANDPLAY) {
                Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                customAttributes.Add("TokenBoardId", tokenBoard.id);
                customAttributes.Add("TokenBoardName", tokenBoard.name);
                AnalyticsManager.LogCustom("open_pnp_game", customAttributes);
            } else if (gameType == GameType.RANDOM) {
                Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                customAttributes.Add("PlayerName", UserManager.instance.userName);
                customAttributes.Add("TokenBoardId", tokenBoard.id);
                customAttributes.Add("TokenBoardName", tokenBoard.name);
                AnalyticsManager.LogCustom("open_new_multiplayer_game", customAttributes);
            } else if (gameType == GameType.AI) {
                Dictionary<String, object> customAttributes = new Dictionary<String, object>();
                customAttributes.Add("TokenBoardId", tokenBoard.id);
                customAttributes.Add("TokenBoardName", tokenBoard.name);
                AnalyticsManager.LogCustom("open_new_ai_challenge", customAttributes);
            }
        }

        public void SetupGame(string title, string subtitle)
        {
            //UpdatePlayersStatusView();
            if (challengeInstanceId != null || challengeInstanceId != "") {
                challengeIdDebugText.text = "ChallengeId: " + challengeInstanceId;    
            } else {
                challengeIdDebugText.text = "Error: missing challenge id";
            }

            SetGameBoardView(gameState.GetPreviousGameBoard());
            CreateTokenViews();
            DisplayIntroUI(title, subtitle, true);

            SetActionButton();

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

        public void SetGameBoardView(int[,] board)
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
                        //TODO: Use player chosen sprite for gamepiece
                        SpriteRenderer pieceSprite = pieceObject.GetComponent<SpriteRenderer>();
                        Color c = pieceSprite.color;
                        c.a = 0.0f;
                        pieceSprite.color = c;
                        //pieceSprite.sprite = new Sprite().texture.

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
                        //pieceSprite.sprite = new Sprite().texture.
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
            //tokenViews = new List<GameObject>();
            tokenViews = new GameObject[Constants.numRows, Constants.numColumns];

            for (int row = 0; row < Constants.numRows; row++)
            {
                for (int col = 0; col < Constants.numColumns; col++)
                {
                    float xPos = (col + .1f) * .972f;
                    float yPos = (row * -1 + .09f) * .965f;

                    Token token = gameState.previousTokenBoard.tokens[row, col].tokenType;
                    GameObject go;
                    switch (token)
                    {
                        case Token.UP_ARROW:
                            go = Instantiate(upArrowToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            //tokenViews.Add(go);
                            tokenViews[row, col] = go;
                            break;
                        case Token.DOWN_ARROW:
                            go = Instantiate(downArrowToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            //tokenViews.Add(go);
                            tokenViews[row, col] = go;
                            break;
                        case Token.LEFT_ARROW:
                            go = Instantiate(leftArrowToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            go.transform.Rotate(0, 0, -90);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            //tokenViews.Add(go);
                            tokenViews[row, col] = go;
                            break;
                        case Token.RIGHT_ARROW:
                            go = Instantiate(rightArrowToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            go.transform.Rotate(0,0,90);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            //tokenViews.Add(go);
                            tokenViews[row, col] = go;
                            break;
                        case Token.STICKY:
                            go = Instantiate(stickyToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            //tokenViews.Add(go);
                            tokenViews[row, col] = go;
                            break;
                        case Token.BLOCKER:
                            go = Instantiate(blockerToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            //tokenViews.Add(go);
                            tokenViews[row, col] = go;
                            break;
                        case Token.GHOST:
                            go = Instantiate(ghostToken, new Vector3(xPos, yPos, 5), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            //tokenViews.Add(go);
                            tokenViews[row, col] = go;
                            break;
                        case Token.ICE_SHEET:
                            go = Instantiate(iceSheetToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            //tokenViews.Add(go);
                            tokenViews[row, col] = go;
                            break;
                        case Token.PIT:
                            go = Instantiate(pitToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            //tokenViews.Add(go);
                            tokenViews[row, col] = go;
                            break;
                        case Token.NINETY_RIGHT_ARROW:
                            go = Instantiate(ninetyRightArrowToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            //tokenViews.Add(go);
                            tokenViews[row, col] = go;
                            break;
                        case Token.NINETY_LEFT_ARROW:
                            go = Instantiate(ninetyLeftArrowToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            //tokenViews.Add(go);
                            tokenViews[row, col] = go;
                            break;
                        case Token.BUMPER:
                            go = Instantiate(bumperToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            //tokenViews.Add(go);
                            tokenViews[row, col] = go;
                            break;
                        case Token.COIN:
                            go = Instantiate(coinToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            //tokenViews.Add(go);
                            tokenViews[row, col] = go;
                            break;
                        case Token.FRUIT:
                            go = Instantiate(fruitToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            //tokenViews.Add(go);
                            tokenViews[row, col] = go;
                            break;
                        case Token.FRUIT_TREE:
                            go = Instantiate(fruitTreeToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            //tokenViews.Add(go);
                            tokenViews[row, col] = go;
                            break;
                        case Token.WEB:
                            go = Instantiate(webToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            //tokenViews.Add(go);
                            tokenViews[row, col] = go;
                            break;
                        case Token.SPIDER:
                            go = Instantiate(spiderToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            //tokenViews.Add(go);
                            tokenViews[row, col] = go;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void EnableTokenAudio()
        {
            foreach (var item in tokenViews)
            {
                if (item != null && item.GetComponent<AudioSource>())
                {
                    item.GetComponent<AudioSource>().mute = false;
                }
            }
        }

        public void InitPlayerUI(string opponentName = "", Sprite opponentProfileSprite = null, string opponentFacebookId = "")
        {
            //Debug.Log("isMultiplayer: "+ isMultiplayer);
            if (isMultiplayer) {
                //Debug.Log("isCurrentPlayer_PlayerOne: " + isCurrentPlayer_PlayerOne);
                if (isCurrentPlayer_PlayerOne)
                {
                    playerNameLabel.color = bluePlayerColor;
                    opponentNameLabel.color = redPlayerColor;
                    //playerPiece.sprite = playerOneSpriteMoving;
                    //opponentPiece.sprite = playerTwoSpriteMoving;
                    int playerGamePieceId = challengerGamePieceId;
                    int opponentGamePieceId = challengedGamePieceId;
                    playerPiece.sprite = GamePieceSelectionManager.instance.gamePieces[playerGamePieceId];
                    GamePieceUI player = playerPiece.GetComponent<GamePieceUI>();
                    player.SetAlternateColor(false);

                    if (opponentGamePieceId != -1) {
                        opponentPiece.sprite = GamePieceSelectionManager.instance.gamePieces[opponentGamePieceId];
                    } else {
                        opponentPiece.sprite = GamePieceSelectionManager.instance.gamePieces[0];
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
                    playerNameLabel.color = redPlayerColor;
                    opponentNameLabel.color = bluePlayerColor;
                    playerPiece.sprite = playerTwoSpriteMoving;
                    opponentPiece.sprite = playerOneSpriteMoving;
                    int playerGamePieceId = challengedGamePieceId;
                    int opponentGamePieceId = challengerGamePieceId;
                    playerPiece.sprite = GamePieceSelectionManager.instance.gamePieces[playerGamePieceId];
                    GamePieceUI player = playerPiece.GetComponent<GamePieceUI>();
                    if (playerGamePieceId == opponentGamePieceId) {
                        player.SetAlternateColor(true);
                    } else {
                        player.SetAlternateColor(false);
                    }
                    opponentPiece.sprite = GamePieceSelectionManager.instance.gamePieces[opponentGamePieceId];
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
                    opponentNameLabel.text = LocalizationManager.instance.GetLocalizedValue("waiting_opponent_text");
                }

                if (opponentProfileSprite != null)
                {
                    //Debug.Log("setting opponentProfilePicture is not null");
                    opponentProfilePicture.sprite = opponentProfileSprite;
                }
                else if (opponentFacebookId != "")
                {
                    Debug.Log("has opponent facebook id");
                    StartCoroutine(UserManager.instance.GetFBPicture(opponentFacebookId, (sprite) =>
                    {
                        Debug.Log("setting opponentProfilePicture.sprite");
                        opponentProfilePicture.sprite = sprite;
                    }));
                } else {
                    Debug.Log("setting default opponentProfilePicture.sprite");
                    opponentProfilePicture.sprite = Sprite.Create(defaultProfilePicture,
                        new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height),
                        new Vector2(0.5f, 0.5f));
                }
            } else {
                playerNameLabel.text = "Player 1";
                playerProfilePicture.sprite = Sprite.Create(defaultProfilePicture,
                    new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height),
                    new Vector2(0.5f, 0.5f));
                playerPiece.sprite = GamePieceSelectionManager.instance.gamePieces[challengerGamePieceId];

                opponentNameLabel.text = "Player 2";
                opponentProfilePicture.sprite = Sprite.Create(defaultProfilePicture,
                    new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height),
                    new Vector2(0.5f, 0.5f));
                opponentPiece.sprite = GamePieceSelectionManager.instance.gamePieces[challengedGamePieceId];

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
            AnimatePlayerPieceUI();
        }

        public void AnimatePlayerPieceUI() {
            //Debug.Log("isCurrentPlayer_PlayerOne: " + isCurrentPlayer_PlayerOne);
            //Debug.Log("isPlayerOneTurn: " + gameState.isPlayerOneTurn);
            float animationSpeed = 0.8f;
            if ((gameState.isPlayerOneTurn && isCurrentPlayer_PlayerOne) || (!gameState.isPlayerOneTurn && !isCurrentPlayer_PlayerOne)) {
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
            } else if ((gameState.isPlayerOneTurn && !isCurrentPlayer_PlayerOne) || (!gameState.isPlayerOneTurn && isCurrentPlayer_PlayerOne)) {
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
            RectTransform pprt = playerPieceUI.GetComponent<RectTransform>();
            pprt.anchoredPosition = new Vector2(175, -83);
            pprt.localScale = new Vector3(1, 1, 1);
            RectTransform oprt = opponentPieceUI.GetComponent<RectTransform>();
            oprt.anchoredPosition = new Vector2(-130, -83);
            oprt.localScale = new Vector3(1, 1, 1);

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
                        if ((games[i].gameState.isCurrentPlayerTurn == true || (games[i].didViewResult == false && games[i].gameState.isGameOver == true && games[i].gameState.isCurrentPlayerTurn == false)) && games[i].challengeId != challengeInstanceId)
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
                    if (isPuzzleChallengeCompleted) {
                        if (isPuzzleChallengePassed) {
                            nextPuzzleChallengeButton.gameObject.SetActive(true);
                        } else {
                            retryPuzzleChallengeButton.gameObject.SetActive(true);
                        }
                    }
                } else {
                    if (gameState.isGameOver) {
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
            //Debug.Log("SpawnPiece: challengerGamePieceId: " + challengerGamePieceId);
            //Debug.Log("SpawnPiece: challengedGamePieceId: " + challengedGamePieceId);
            if (player == PlayerEnum.ONE) {
                if (startingState == PieceAnimState.MOVING) {
                    gamePiece.GetComponent<SpriteRenderer>().sprite = GamePieceSelectionManager.instance.gamePieces[challengerGamePieceId];
                } else {
                    gamePiece.GetComponent<SpriteRenderer>().sprite = GamePieceSelectionManager.instance.gamePieces[challengerGamePieceId];
                }
            } else {
                if (startingState == PieceAnimState.MOVING) {
                    gamePiece.GetComponent<SpriteRenderer>().sprite = GamePieceSelectionManager.instance.gamePieces[challengedGamePieceId];
                } else {
                    gamePiece.GetComponent<SpriteRenderer>().sprite = GamePieceSelectionManager.instance.gamePieces[challengedGamePieceId];
                }
            }
            gamePiece.GetComponent<SpriteRenderer>().enabled = true;

            return gamePiece;
        }

        public void RematchPassAndPlayGame()
        {
            GameManager.instance.TransitionToGameOptionsScreen(GameType.PASSANDPLAY);

            AnalyticsManager.LogCustom("rematch_pnp_game");
        }

        public void RetryPuzzleChallenge() {
            PuzzleChallengeInfo puzzleChallenge = PuzzleChallengeLoader.instance.GetChallenge();
            if (puzzleChallenge ==  null) {
                alertUI.Open(LocalizationManager.instance.GetLocalizedValue("all_challenges_completed"));
            } else {
                ResetGameManagerState();
                puzzleChallengeInfo = puzzleChallenge;
                ResetUIGameScreen();
                gameScreen.GetComponent<CanvasGroup>().alpha = 0.0f;
                ResetGamePiecesAndTokens();

                TokenBoard initialTokenBoard = new TokenBoard(puzzleChallenge.InitialTokenBoard.ToArray(), "", "", true);

                gameState = new GameState(Constants.numRows, Constants.numColumns, true, true, initialTokenBoard, puzzleChallenge.InitialGameBoard.ToArray(), false, null);

                isCurrentPlayer_PlayerOne = true;
                isPuzzleChallenge = true;
                UpdatePlayerUI();

                string subtitle = "";
                if (puzzleChallenge.MoveGoal > 1)
                {
                    subtitle = LocalizationManager.instance.GetLocalizedValue("puzzle_challenge_win_instructions_plural");
                }
                else
                {
                    subtitle = LocalizationManager.instance.GetLocalizedValue("puzzle_challenge_win_instructions_singular");
                }

                SetupGame(puzzleChallenge.Name, subtitle.Replace("%1", puzzleChallenge.MoveGoal.ToString()));

                retryPuzzleChallengeButton.gameObject.SetActive(false);

                EnableTokenAudio();
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
                alertUI.Open(LocalizationManager.instance.GetLocalizedValue("all_challenges_completed"));
            } else {
                ResetGameManagerState();

                puzzleChallengeInfo = puzzleChallenge;
                ResetUIGameScreen();
                gameScreen.GetComponent<CanvasGroup>().alpha = 0.0f;
                ResetGamePiecesAndTokens();

                TokenBoard initialTokenBoard = new TokenBoard(puzzleChallenge.InitialTokenBoard.ToArray(), "", "", true);

                gameState = new GameState(Constants.numRows, Constants.numColumns, true, true, initialTokenBoard, puzzleChallenge.InitialGameBoard.ToArray(), false, null);

                isCurrentPlayer_PlayerOne = true;
                isPuzzleChallenge = true;
                UpdatePlayerUI();

                string subtitle = "";
                if (puzzleChallenge.MoveGoal > 1) {
                    subtitle = LocalizationManager.instance.GetLocalizedValue("puzzle_challenge_win_instructions_plural");
                } else {
                    subtitle = LocalizationManager.instance.GetLocalizedValue("puzzle_challenge_win_instructions_singular");
                }

                SetupGame(puzzleChallenge.Name, subtitle.Replace("%1", puzzleChallenge.MoveGoal.ToString()));

                nextPuzzleChallengeButton.gameObject.SetActive(false);

                EnableTokenAudio();
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

        private void ProcessPlayerInput(Vector3 mousePosition)
        {
            //Debug.Log("ProcessPlayerInput");
            if (isLoading || isLoadingUI || isDropping || gameState.isGameOver || animatingGamePieces)
            {
                //Debug.Log("returned in process player input");
                return;

            }

            Vector3 pos = Camera.main.ScreenToWorldPoint(mousePosition);

            if (gameState.isCurrentPlayerTurn)
            {
                // round to a grid square
                //Debug.Log("Move Position: x: " + pos.x + " y: " + (pos.y * -1 - .3f));
                int column = Mathf.RoundToInt(pos.x);
                int row = Mathf.CeilToInt((pos.y * - 1 - .3f));

                Position position;
                PlayerEnum player = gameState.isPlayerOneTurn ? PlayerEnum.ONE : PlayerEnum.TWO;
                //Debug.Log("ProcessPlayerInput: column: " + column + " row: " + row);
                if (inTopRowBounds(pos.x, pos.y))
                {
                    position = new Position(column, row - 1);
                    //Debug.Log("Move Position: col: " + position.column + " row: " + position.row);
                    Move move = new Move(position, Direction.DOWN, player);
                    StartCoroutine(ProcessMove(move, true));
                }
                else if (inBottomRowBounds(pos.x, pos.y))
                {
                    position = new Position(column, row + 1);
                    Move move = new Move(position, Direction.UP, player);
                    //Debug.Log("Move Position: col: " + position.column + " row: " + position.row);
                    StartCoroutine(ProcessMove(move, true));
                }
                else if (inRightRowBounds(pos.x, pos.y))
                {
                    position = new Position(column + 1, row);
                    Move move = new Move(position, Direction.LEFT, player);
                    //Debug.Log("Move Position: col: " + position.column + " row: " + position.row);
                    StartCoroutine(ProcessMove(move, true));
                }
                else if (inLeftRowBounds(pos.x, pos.y))
                {
                    position = new Position(column - 1, row);
                    Move move = new Move(position, Direction.RIGHT, player);
                    //Debug.Log("Move Position: col: " + position.column + " row: " + position.row);
                    StartCoroutine(ProcessMove(move, true));
                }
            }
            else
            {
                //Debug.Log("Not isCurrentPlayerTurn: challengeInstanceId: " + challengeInstanceId);
                if (challengeInstanceId != null) {
                    if (inTopRowBounds(pos.x, pos.y) || inBottomRowBounds(pos.x, pos.y) || inRightRowBounds(pos.x, pos.y) || inLeftRowBounds(pos.x, pos.y))
                    {
                        alertUI.Open(LocalizationManager.instance.GetLocalizedValue("not_your_turn"));
                    }
                }
            }
        }

        public IEnumerator ProcessMove(Move move, bool updatePlayer)
        {
            isDropping = true;

            if (gameState.CanMove(move.GetNextPosition(), gameState.tokenBoard.tokens))
            {
                moveHintArea.SetActive(false);

                if (isMultiplayer && !isNewChallenge && !isNewRandomChallenge)
                {
                    replayedLastMove = false;
                    StartCoroutine(MovePiece(move, false, updatePlayer));
                    Debug.Log("LogChallengeEventRequest: challengeInstanceId: " + challengeInstanceId);
                    new LogChallengeEventRequest().SetChallengeInstanceId(challengeInstanceId)
                        .SetEventKey("takeTurn") //The event we are calling is "takeTurn", we set this up on the GameSparks Portal
                        .SetEventAttribute("pos", Utility.GetMoveLocation(move)) // pos is the row or column the piece was placed at depending on the direction
                        .SetEventAttribute("direction", move.direction.GetHashCode()) // direction can be up, down, left, right
                        .SetEventAttribute("player", gameState.isPlayerOneTurn ? (int)Piece.BLUE : (int)Piece.RED)
                        .SetDurable(true)
                        .Send((response) =>
                            {
                                if (response.HasErrors)
                                {
                                    Debug.Log("***** ChallengeEventRequest failed: " + response.Errors.JSON);
                                    //gameInfo.Open("There was a problem making your move.Please try again.", Color.red, true);
                                    gameStatusText.text = "There was a problem making your move. Please try again.";
                                }
                                else
                                {
                                    // If our ChallengeEventRequest was successful we inform the player
                                    Debug.Log("ChallengeEventRequest was successful");
                                }
                            });

                    while (isDropping)
                        yield return null;
                    //var currentGame = games
                    //    .Where(t => t.challengeId == challengeInstanceId)
                    //    .FirstOrDefault();
                    //if (currentGame != null)
                    //{
                    //    currentGame.gameState = gameState;
                    //}
                }
                else if (isMultiplayer && isNewChallenge)
                {
                    isNewChallenge = false;
                    StartCoroutine(MovePiece(move, false, updatePlayer));
                    ChallengeManager.instance.ChallengeUser(opponentUserId, gameState, Utility.GetMoveLocation(move), move.direction, gameType);

                    // TODO: Create an Game and add it to the Games list
                    //PlayerData playerData = new PlayerData(opponentNameLabel.text, opponentUserId);
                    //Game game = new Game(challengeInstanceId, gameState, isCurrentPlayer_PlayerOne, isExpired, didViewResult, playerData, ChallengeState.RUNNING, ChallengeType.STANDARD, gameType);
                    //games.Add(game);
                }
                else if (isMultiplayer && isNewRandomChallenge)
                {
                    isNewRandomChallenge = false;
                    StartCoroutine(MovePiece(move, false, updatePlayer));
                    ChallengeManager.instance.ChallengeRandomUser(gameState, Utility.GetMoveLocation(move), move.direction, gameType);

                    // TODO: Create an ActiveGame and add it to the activeGames list
                    //PlayerData playerData = new PlayerData(opponentNameLabel.text, opponentUserId);
                    //Game game = new Game(challengeInstanceId, gameState, isCurrentPlayer_PlayerOne, isExpired, didViewResult, playerData, ChallengeState.ISSUED, ChallengeType.STANDARD, gameType);
                    //games.Add(game);
                }
                else if (isAiActive)
                {
                    Debug.Log("I'm making a move for AI Improved.");

                    StartCoroutine(MovePiece(move, false, updatePlayer));

                    //check for end of game.
                    if (gameState.isGameOver)
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
                    if (isPuzzleChallenge && !gameState.isGameOver && !isPuzzleChallengeCompleted) {
                        while (isDropping)
                            yield return null;
                        isDropping = true;
                        yield return new WaitForSeconds(1f);

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
                                    if (gameState.CanMove(newMove.GetNextPosition(), gameState.tokenBoard.tokens)) {
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

            yield return 0;
        }

        public void CreateGame(string challengeId) {
            Opponent opponent = new Opponent(opponentNameLabel.text, opponentFacebookId);
            Game game = new Game(challengeId, gameState, isCurrentPlayer_PlayerOne, isExpired, didViewResult, opponent, ChallengeState.RUNNING, ChallengeType.STANDARD, gameType, challengerGamePieceId.ToString(), challengedGamePieceId.ToString());
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

            isDropping = true;
            audioMove.Play();

            List<IToken> activeTokens;

            gameState.PrintGameState("BeforeMove");
            List<MovingGamePiece> movingPieces = gameState.MovePiece(move, replayMove, out activeTokens);
            gameState.PrintGameState("AfterMove");
            if (movingPieces.Count == 0) {
                yield return false;
                yield break;
            }

            if (isPuzzleChallenge) {
                if (gameState.player1MoveCount >= puzzleChallengeInfo.MoveGoal) {
                    isPuzzleChallengeCompleted = true;
                    if (gameState.isGameOver && gameState.winner == PlayerEnum.ONE) {
                        // Puzzle Challenge Completed
                        isPuzzleChallengePassed = true;
                        PlayerPrefs.SetInt("puzzleChallengeLevel", puzzleChallengeInfo.Level);

                        AnalyticsManager.LogPuzzleChallenge(puzzleChallengeInfo, true);
                    } else {
                        // Puzzle Challenge Failed
                        isPuzzleChallengePassed = false;
                        gameState.isGameOver = true;

                        AnalyticsManager.LogPuzzleChallenge(puzzleChallengeInfo, false);
                    }
                } else if (gameState.isGameOver) {
                    isPuzzleChallengeCompleted = true;
                    //Debug.Log("isPuzzleChallengeCompleted: " + isPuzzleChallengeCompleted);
                    //Debug.Log("gameState.player1MoveCount: " + gameState.player1MoveCount);
                    if (gameState.winner == PlayerEnum.ONE) {
                        isPuzzleChallengePassed = true;
                        PlayerPrefs.SetInt("puzzleChallengeLevel", puzzleChallengeInfo.Level);

                        AnalyticsManager.LogPuzzleChallenge(puzzleChallengeInfo, true);
                    } else {
                        isPuzzleChallengePassed = false;
                        gameState.isGameOver = true;

                        AnalyticsManager.LogPuzzleChallenge(puzzleChallengeInfo, false);
                    }
                }
            }

            MoveGamePieceViews(move, movingPieces, activeTokens);

            while (animatingGamePieces)
                yield return null;

            UpdateGameStatus(updatePlayer);

            isDropping = false;
            if (replayMove)
            {
                isLoading = false;
            }

            if (isPuzzleChallengeCompleted) {
                if (isPuzzleChallengePassed) {
                    string title = LocalizationManager.instance.GetLocalizedValue("challenge_completed_title");
                    string subtitle = LocalizationManager.instance.GetLocalizedValue("challenge_completed_subtitle");
                    DisplayIntroUI(title, subtitle, false);
                } else {
                    string title = LocalizationManager.instance.GetLocalizedValue("challenge_failed_title");
                    string subtitle = LocalizationManager.instance.GetLocalizedValue("challenge_failed_subtitle");
                    DisplayIntroUI(title, subtitle, false);
                }
            }

            if (!replayMove || gameState.isGameOver || isPuzzleChallengeCompleted)
            {
                SetActionButton();
            }

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

            if (OnMoved != null)
                OnMoved();

            yield return true;
        }

        private void UpdateGameStatus(bool updatePlayer)
        {
            if (gameState.isGameOver || isExpired)
            {
                DisplayGameOverView();
            }
            else
            {
                if (updatePlayer)
                {
                    if (isMultiplayer || isAiActive)
                    {
                        gameState.isCurrentPlayerTurn = !gameState.isCurrentPlayerTurn;
                    }
                    //UpdatePlayersStatusView();
                }
                UpdatePlayerUI();
            }
        }

        private void VisitedGameResults() {
            var currentGame = games
                .Where(t => t.challengeId == challengeInstanceId).FirstOrDefault();
            if (currentGame != null)
            {
                if (!currentGame.didViewResult && gameState.isGameOver)
                {
                    ChallengeManager.instance.SetViewedCompletedGame(challengeInstanceId);
                    // Debug.Log("activeGame.challengeId: " + activeGame.challengeId);
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

        public void DisplayGameOverView()
        {
            bool showRewardButton = false;

            if (isExpired)
            {
                VisitedGameResults();
                gameInfo.Open(LocalizationManager.instance.GetLocalizedValue("expired_text"), Color.white, false, showRewardButton);
                return;
            }

            if (!didViewResult) {
                showRewardButton = true;
            }
            //Debug.Log("viewedResult: " + didViewResult);
            //Debug.Log("showRewardButton: " + showRewardButton);
            Color winnerTextColor = gameState.winner == PlayerEnum.ONE ? bluePlayerColor : redPlayerColor;
            Debug.Log("DisplayGameOverView gameState.winner: " +  gameState.winner);
            if (gameState.winner == PlayerEnum.ONE)
            {
                int size = gameState.gameBoard.player1WinningPositions.Count;
                Vector3 startPos = new Vector3((gameState.gameBoard.player1WinningPositions[0].column + .1f) *.972f, (gameState.gameBoard.player1WinningPositions[0].row * -1 + .05f) * .96f, 12);
                Vector3 endPos = new Vector3((gameState.gameBoard.player1WinningPositions[size - 1].column+ .1f) * .972f, (gameState.gameBoard.player1WinningPositions[size - 1].row * -1 + .05f) * .96f, 12);
                DrawLine(startPos, endPos, bluePlayerColor);

                if (gameState.isCurrentPlayerTurn)
                {
#if UNITY_IOS || UNITY_ANDROID
                    Handheld.Vibrate();
#endif
                }
            }

            if (gameState.winner == PlayerEnum.TWO)
            {
                int size = gameState.gameBoard.player2WinningPositions.Count;
                Vector3 startPos = new Vector3((gameState.gameBoard.player2WinningPositions[0].column + .1f) * .972f, (gameState.gameBoard.player2WinningPositions[0].row * -1 + .05f) * .96f, 12);
                Vector3 endPos = new Vector3((gameState.gameBoard.player2WinningPositions[size - 1].column + .1f) * .972f, (gameState.gameBoard.player2WinningPositions[size - 1].row * -1 + .05f) *.96f, 12);
                DrawLine(startPos, endPos, redPlayerColor);

                if (gameState.isCurrentPlayerTurn)
                {
#if UNITY_IOS || UNITY_ANDROID
                    Handheld.Vibrate();
#endif
                }
            }
            WinLineSetActive(true);

            if (gameState.winner == PlayerEnum.NONE || gameState.winner == PlayerEnum.ALL)
            {
                if (isMultiplayer) {
                    gameInfo.Open(LocalizationManager.instance.GetLocalizedValue("draw_text"), Color.white, false, showRewardButton);    
                } else {
                    if (!isPuzzleChallenge)
                    {
                        gameInfo.Open(LocalizationManager.instance.GetLocalizedValue("draw_text"), Color.white, false, false);
                    }
                }
            }
            else if (isMultiplayer)
            {
                if (winner != null && winner.Length > 0)
                {
                    //gameStatusText.text = winner + LocalizationManager.instance.GetLocalizedValue("won_suffix");
                    gameInfo.Open(winner + LocalizationManager.instance.GetLocalizedValue("won_suffix"), winnerTextColor, true, showRewardButton);
                }
                else
                {                    
                    if (isCurrentPlayer_PlayerOne && gameState.winner == PlayerEnum.ONE)
                    {
                        audioWin.Play();
                        gameInfo.Open(UserManager.instance.userName + LocalizationManager.instance.GetLocalizedValue("won_suffix"), winnerTextColor, true, showRewardButton);
                    }
                    else if (!isCurrentPlayer_PlayerOne && gameState.winner == PlayerEnum.TWO)
                    {
                        audioWin.Play();
                        gameInfo.Open(UserManager.instance.userName + LocalizationManager.instance.GetLocalizedValue("won_suffix"), winnerTextColor, true, showRewardButton);
                    }
                    else
                    {
                        gameInfo.Open(opponentNameLabel.text + LocalizationManager.instance.GetLocalizedValue("won_suffix"), winnerTextColor, true, showRewardButton);
                    }

                    //gameStatusText.color = gameState.winner == PlayerEnum.ONE ? bluePlayerColor : redPlayerColor;

                    //AnalyticsManager.LogGameOver("multiplayer", gameState.winner, gameState.tokenBoard);
                }
            }
            else
            {
                audioWin.Play();
                if (!isPuzzleChallenge) {
                    //AnalyticsManager.LogGameOver("pnp", gameState.winner, gameState.tokenBoard);
                    string winnerText = gameState.winner == PlayerEnum.ONE ? bluePlayerWonText : redPlayerWonText;
                    gameInfo.Open(winnerText, winnerTextColor, true, false);
                }
            }

            switch (gameType)
            {
                case GameType.FRIEND:
                    AnalyticsManager.LogGameOver("friend", gameState.winner, gameState.tokenBoard);
                    break;
                case GameType.LEADERBOARD:
                    AnalyticsManager.LogGameOver("leaderboard", gameState.winner, gameState.tokenBoard);
                    break;
                case GameType.AI:
                    AnalyticsManager.LogGameOver("AI", gameState.winner, gameState.tokenBoard);
                    break;
                case GameType.PASSANDPLAY:
                    AnalyticsManager.LogGameOver("pnp", gameState.winner, gameState.tokenBoard);
                    break;
                case GameType.PUZZLE:
                    AnalyticsManager.LogGameOver("puzzle", gameState.winner, gameState.tokenBoard);
                    break;
                default:
                    AnalyticsManager.LogGameOver("random", gameState.winner, gameState.tokenBoard);
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