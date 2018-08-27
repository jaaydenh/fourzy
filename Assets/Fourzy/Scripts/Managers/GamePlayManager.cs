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
    public class GamePlayManager : UnitySingleton<GamePlayManager> {

        public Game game;
        private bool isNewGame;
        private bool isLoading = false;
        public bool isLoadingUI = false;
        private bool isDropping = false;
        public int numPiecesAnimating = 0;
        public bool disableInput = false;
        private bool replayedLastMove = false;
        private float gameScreenFadeInTime = 0.7f;
        private DateTime serverClock;
        private DateTime playerMoveCountdown;
        private bool clockStarted = false;
        private int playerMoveCountDown_LastTime;

        [Header("Game UI")]
        public GameObject gameScreen;
        public Rewards rewardScreen;
        public GameObject gameBoard;
        public GameObject gamePieces;
        private GameObject tokens;
        public GameObject[,] tokenViews;
        public GameObject gamePiecePrefab;
        public Image playerPieceImage;
        public Image opponentPieceImage;
        public GameObject playerPieceUI;
        // private Animator playerPieceUIAnimator;
        private PieceUIGameScreen playerPieceUIScript;
        public GameObject opponentPieceUI;
        // private Animator opponentPieceUIAnimator;
        private PieceUIGameScreen opponentPieceUIScript;
        public Sprite playerOneDefaultSprite;
        public Sprite playerTwoDefaultSprite;
        public Text playerNameText;
        public Text opponentNameText;
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
        public GameBoardView gameBoardView;
        public GameObject particleEffect;
        public TextMeshProUGUI timerText;


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

        void Start () {
            game = GameManager.instance.activeGame;

            //TODO: Create a game if activeGame is null
            
            //SoundManager.instance.Mute(true);
            UserInputHandler.inputEnabled = false;

            // timerText = GetComponent<TextMeshProUGUI>();

            Utility.SetSpriteAlpha(gameBoard, 0.0f);
            gameScreen.GetComponent<CanvasGroup>().alpha = 0.0f;

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

            // playerPieceUIAnimator = playerPieceUI.GetComponent<Animator>();
            // opponentPieceUIAnimator = opponentPieceUI.GetComponent<Animator>();

            playerPieceUIScript = playerPieceUI.GetComponent<PieceUIGameScreen>();
            opponentPieceUIScript = opponentPieceUI.GetComponent<PieceUIGameScreen>();

            FadeGameScreen(1.0f, gameScreenFadeInTime);
            StartCoroutine(WaitToEnableInput());

            if (tokens == null)
            {
                tokens = new GameObject("Tokens");
                tokens.transform.parent = gameScreen.transform;
                // tokens.transform.position.Set(-324f, 307f, 0f);
            }

            replayedLastMove = false;

            // ResetGamePiecesAndTokens();
            ResetUI();
            if (game.gameState != null) {
                CreateGamePieceViews(game.gameState.GetPreviousGameBoard());
            }

            CreateTokenViews();

            InitPlayerUI();
            UpdatePlayerUI();

            InitIntroUI();

            SetActionButton();

            ratingDeltaText.text = "0";
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
                StartCoroutine(SendTimeStamp());
            }

            playerMoveCountDown_LastTime = 30000;
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

        public void ResetUI()
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

        public void ResetGamePiecesAndTokens()
        {
            if (gamePieces.transform.childCount > 0)
            {
                for (int i = gamePieces.transform.childCount - 1; i >= 0; i--)
                {
                    Transform piece = gamePieces.transform.GetChild(i);
                    piece.localScale = new Vector3(1.0f, 1.0f, 1.0f);
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

        private void InitIntroUI() 
        {
            string title = game.title;
            string subtitle = game.subtitle;
            if (subtitle == "") {
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

            if (title == "") {
                title = game.gameState.TokenBoard.name;
            }

            if (game.displayIntroUI) {
                DisplayIntroUI(title, subtitle, true);
            }
        }

        public void DisplayIntroUI(string title, string subtitle, bool fade) {
            if (title != null && title != "" && subtitle != null && subtitle != "") {
                gameIntroUI.Open(title, subtitle, fade);
            }
        }

        public void UpdateOpponentUI(Opponent opponent) {
            // TODO: complete this
        }

        public void InitPlayerUI()
        {
            Debug.Log("game.gameState.GameType: " + game.gameState.GameType);
            if (game.gameState.GameType == GameType.REALTIME || game.gameState.GameType == GameType.RANDOM || game.gameState.GameType == GameType.FRIEND || game.gameState.GameType == GameType.LEADERBOARD) {
                int playerGamePieceId = 0;
                int opponentGamePieceId = 0;

                if (game.gameState.GameType == GameType.REALTIME) {
                    playerGamePieceId = UserManager.instance.gamePieceId;
                    opponentGamePieceId = game.opponent.gamePieceId;
                }

                if (game.isCurrentPlayer_PlayerOne)
                {
                    if (game.gameState.GameType != GameType.REALTIME) {
                        playerGamePieceId = game.challengerGamePieceId;
                        opponentGamePieceId = game.challengedGamePieceId;
                    }

                    if (playerGamePieceId > GamePieceSelectionManager.Instance.gamePieces.Count - 1)
                    {
                        playerGamePieceId = 0;
                    }

                    playerPieceImage.sprite = GamePieceSelectionManager.Instance.GetGamePieceSprite(playerGamePieceId);
                    GamePieceUI player = playerPieceImage.GetComponent<GamePieceUI>();
                    player.SetAlternateColor(false);

                    if (opponentGamePieceId > GamePieceSelectionManager.Instance.gamePieces.Count - 1) {
                        opponentGamePieceId = 0;
                    }

                    if (opponentGamePieceId != -1) {
                        opponentPieceImage.sprite = GamePieceSelectionManager.Instance.GetGamePieceSprite(opponentGamePieceId);
                    } else {
                        opponentPieceImage.sprite = GamePieceSelectionManager.Instance.gamePieces[0];
                    }

                    GamePieceUI opponent = opponentPieceImage.GetComponent<GamePieceUI>();
                    if (playerGamePieceId == opponentGamePieceId) {
                        opponent.SetAlternateColor(true);    
                    } else {
                        opponent.SetAlternateColor(false);
                    }
                }
                else
                {
                    playerPieceImage.sprite = playerTwoDefaultSprite;
                    opponentPieceImage.sprite = playerOneDefaultSprite;

                    if (game.gameState.GameType != GameType.REALTIME) {
                        playerGamePieceId = game.challengedGamePieceId;
                        opponentGamePieceId = game.challengerGamePieceId;
                    }

                    if (playerGamePieceId > GamePieceSelectionManager.Instance.gamePieces.Count - 1)
                    {
                        playerGamePieceId = 0;
                    }

                    playerPieceImage.sprite = GamePieceSelectionManager.Instance.GetGamePieceSprite(playerGamePieceId);
                    GamePieceUI player = playerPieceImage.GetComponent<GamePieceUI>();
                    if (playerGamePieceId == opponentGamePieceId) {
                        player.SetAlternateColor(true);
                    } else {
                        player.SetAlternateColor(false);
                    }

                    if (opponentGamePieceId > GamePieceSelectionManager.Instance.gamePieces.Count - 1)
                    {
                        opponentGamePieceId = 0;
                    }

                    opponentPieceImage.sprite = GamePieceSelectionManager.Instance.GetGamePieceSprite(opponentGamePieceId);
                    GamePieceUI opponent = opponentPieceImage.GetComponent<GamePieceUI>();
                    opponent.SetAlternateColor(false);
                }

                playerNameText.text = UserManager.instance.userName;

                // if (UserManager.instance.profilePicture)
                // {
                //     playerProfilePicture.sprite = UserManager.instance.profilePicture;
                // }
                // else
                // {
                //     playerProfilePicture.sprite = Sprite.Create(defaultProfilePicture,
                //         new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height),
                //         new Vector2(0.5f, 0.5f));
                // }
                
                if (game.opponent != null && game.opponent.opponentName != null && game.opponent.opponentName != "") {
                    Debug.Log("game.opponent.opponentName: " + game.opponent.opponentName);
                    opponentNameText.text = game.opponent.opponentName;
                } else {
                    opponentNameText.text = LocalizationManager.Instance.GetLocalizedValue("waiting_opponent_text");
                }

                // if (opponentProfileSprite != null)
                // {
                //     opponentProfilePicture.sprite = opponentProfileSprite;
                // }
                // else if (opponentFacebookId != "")
                // {
                //     StartCoroutine(UserManager.instance.GetFBPicture(opponentFacebookId, (sprite) =>
                //     {
                //         opponentProfilePicture.sprite = sprite;
                //     }));
                // } else {
                //     opponentProfilePicture.sprite = Sprite.Create(defaultProfilePicture,
                //         new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height),
                //         new Vector2(0.5f, 0.5f));
                // }
            } else {
                playerNameText.text = "Player 1";
                // playerProfilePicture.sprite = Sprite.Create(defaultProfilePicture,
                //     new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height),
                //     new Vector2(0.5f, 0.5f));
                playerPieceImage.sprite = GamePieceSelectionManager.Instance.GetGamePieceSprite(game.challengerGamePieceId);

                opponentNameText.text = "Player 2";
                // opponentProfilePicture.sprite = Sprite.Create(defaultProfilePicture,
                //     new Rect(0, 0, defaultProfilePicture.width, defaultProfilePicture.height),
                //     new Vector2(0.5f, 0.5f));
                opponentPieceImage.sprite = GamePieceSelectionManager.Instance.GetGamePieceSprite(game.challengedGamePieceId);

                GamePieceUI opponent = opponentPieceImage.GetComponent<GamePieceUI>();
                if (game.challengerGamePieceId == game.challengedGamePieceId)
                {
                    opponent.SetAlternateColor(true);
                }
                else
                {
                    opponent.SetAlternateColor(false);
                }
            }
        }

        public void SetupGame(string title, string subtitle)
        {
            SoundManager.instance.Mute(true);
            if (game.challengeId != null || game.challengeId != "") {
                challengeIdDebugText.text = "ChallengeId: " + game.challengeId;    
            } else {
                challengeIdDebugText.text = "Error: missing challenge id";
                OnGamePlayMessage("Error: missing challenge id");
            }

            CreateGamePieceViews(game.gameState.GetPreviousGameBoard());
            CreateTokenViews();

            DisplayIntroUI(title, subtitle, true);

            SetActionButton();

            ratingDeltaText.text = "0";
            if (game.isCurrentPlayer_PlayerOne)
            {
                ratingDeltaText.text = game.challengerRatingDelta.ToString();
            }
            else
            {
                ratingDeltaText.text = game.challengedRatingDelta.ToString();
            }
        }

        public void CreateGamePieceViews(int[,] board)
        {
            gameBoardView.gamePieces = new GamePiece[Constants.numRows, Constants.numColumns];

            for (int row = 0; row < Constants.numRows; row++)
            {
                for (int col = 0; col < Constants.numColumns; col++)
                {
                    int piece = board[row, col];

                    if (piece == (int)Piece.BLUE)
                    {
                        GamePiece pieceObject = SpawnPiece(col, row * -1, PlayerEnum.ONE, PieceAnimState.ASLEEP);
                        pieceObject.player = PlayerEnum.ONE;
                        gameBoardView.gamePieces[row, col] = pieceObject;
                    }
                    else if (piece == (int)Piece.RED)
                    {
                        GamePiece pieceObject = SpawnPiece(col, row * -1, PlayerEnum.TWO, PieceAnimState.ASLEEP);
                        pieceObject.player = PlayerEnum.TWO;
                        gameBoardView.gamePieces[row, col] = pieceObject;
                    }
                }
            }
        }

        public void CreateStickyToken(int row, int col) {
            float tokenScale = .96f;
            float xPos = (col + .15f) * tokenScale;
            float yPos = (row * -1 + .075f) * tokenScale;
            GameObject go = Instantiate(stickyToken, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
            Destroy(tokenViews[row, col]);
            tokenViews[row, col] = go;
        }

        public void CreateTokenViews()
        {
            tokenViews = new GameObject[Constants.numRows, Constants.numColumns];
            float tokenScale = .96f;
            
            for (int row = 0; row < Constants.numRows; row++)
            {
                for (int col = 0; col < Constants.numColumns; col++)
                {
                    float xPos = (col + .15f) * tokenScale;
                    float yPos = (row * -1 + .075f) * tokenScale;
                    // float gridSize = Screen.width - 20;
                    bool rotateRight = false;
                    bool rotateLeft = false;
                    GameObject go;
                    GameObject tokenPrefab = null;
                    GameState gameState = game.gameState;
                    TokenBoard previousTokenBoard = game.gameState.PreviousTokenBoard;
                    IToken[,] previousTokenBoardTokens = previousTokenBoard.tokens;
                    if (previousTokenBoardTokens[row, col] != null) {
                        Token token = previousTokenBoardTokens[row, col].tokenType;

                        switch (token)
                        {
                            case Token.UP_ARROW:
                                tokenPrefab = upArrowToken;
                                break;
                            case Token.DOWN_ARROW:
                                tokenPrefab = downArrowToken;
                                break;
                            case Token.LEFT_ARROW:
                                rotateLeft = true;
                                tokenPrefab = leftArrowToken;
                                break;
                            case Token.RIGHT_ARROW:
                                rotateRight = true;
                                tokenPrefab = rightArrowToken;
                                break;
                            case Token.STICKY:
                                tokenPrefab = stickyToken;
                                break;
                            case Token.BLOCKER:
                                tokenPrefab = blockerToken;
                                break;
                            case Token.GHOST:
                                tokenPrefab = ghostToken;
                                break;
                            case Token.ICE_SHEET:
                                tokenPrefab = iceSheetToken;
                                break;
                            case Token.PIT:
                                tokenPrefab = pitToken;
                                break;
                            case Token.NINETY_RIGHT_ARROW:
                                tokenPrefab = ninetyRightArrowToken;
                                break;
                            case Token.NINETY_LEFT_ARROW:
                                tokenPrefab = ninetyLeftArrowToken;
                                break;
                            case Token.BUMPER:
                                tokenPrefab = bumperToken;
                                break;
                            case Token.COIN:
                                tokenPrefab = coinToken;
                                break;
                            case Token.FRUIT:
                                tokenPrefab = fruitToken;
                                break;
                            case Token.FRUIT_TREE:
                                tokenPrefab = fruitTreeToken;
                                break;
                            case Token.WEB:
                                tokenPrefab = webToken;
                                break;
                            case Token.SPIDER:
                                tokenPrefab = spiderToken;
                                break;
                            case Token.SAND:
                                tokenPrefab = sandToken;
                                break;
                            case Token.WATER:
                                tokenPrefab = waterToken;
                                break;
                            default:
                                break;
                        }
                    } else {
                        Debug.Log("previousTokenBoardTokens is null");
                    }

                    if (tokenPrefab) {
                        go = Instantiate(tokenPrefab, new Vector3(xPos, yPos, 15), Quaternion.identity, tokens.transform);
                        if (rotateRight) {
                            go.transform.Rotate(0,0,90);
                        }
                        if (rotateLeft) {
                            go.transform.Rotate(0, 0, -90);
                        }
                        Utility.SetSpriteAlpha(go, 0.0f);
                        tokenViews[row, col] = go;
                        // go.transform.localScale = new Vector2(rectHeight/8, rectHeight/8);
                    }
                }
            }
        }

        private void FadeGameScreen(float alpha, float fadeTime)
        {
            gameBoard.GetComponent<SpriteRenderer>().DOFade(alpha, fadeTime);
            gameScreen.GetComponent<CanvasGroup>().DOFade(alpha, fadeTime).OnComplete(() => FadeTokens(alpha, fadeTime));
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
            if (game.gameState.MoveList != null)
            {
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

                StartCoroutine(MovePiece(lastMove, true, false));
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

            
            RealtimeManager.Instance.GetRTSession().Disconnect();
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
            gameScreen.GetComponent<CanvasGroup>().alpha = 0.0f;
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
        GamePiece SpawnPiece(float posX, float posY, PlayerEnum player, PieceAnimState startingState)
        {
            if (game.challengerGamePieceId > GamePieceSelectionManager.Instance.gamePieces.Count - 1)
            {
                game.challengerGamePieceId = 0;
            }
            if (game.challengedGamePieceId > GamePieceSelectionManager.Instance.gamePieces.Count - 1)
            {
                game.challengedGamePieceId = 0;
            }

            //Debug.Log("SpawnPiece: x: " + posX + " y: " + posY);
            float xPos = (posX + .1f) * .972f;
            float yPos = (posY + .05f) * .96f;

            int gamePieceID = game.challengerGamePieceId;
            if (game.gameState.GameType == GameType.REALTIME)
            {
                if (game.isCurrentPlayer_PlayerOne)
                {
                    if (player == PlayerEnum.ONE)
                    {
                        gamePieceID = UserManager.instance.gamePieceId;
                    }
                    else
                    {
                        gamePieceID = game.opponent.gamePieceId;
                    }
                }
                else
                {
                    if (player == PlayerEnum.ONE)
                    {
                        gamePieceID = game.opponent.gamePieceId;
                    }
                    else
                    {
                        gamePieceID = UserManager.instance.gamePieceId;
                    }
                }
            }

            GameObject gamePiecePrefab = GamePieceSelectionManager.Instance.GetGamePiecePrefab(gamePieceID);
            GameObject go = Instantiate(gamePiecePrefab, new Vector3(xPos, yPos, 10),
                                                Quaternion.identity, gamePieces.transform);
            GamePiece gamePiece = go.GetComponent<GamePiece>();
            
            gamePiece.transform.position = new Vector3(xPos, yPos, 10);
            gamePiece.SetupPlayer(player, startingState);

            return gamePiece;
        }

        public void UpdatePlayerUI()
        {
            AnimatePlayerPieceUI();
        }

		public void AnimatePlayerPieceUI() {
            float animationSpeed = 0.8f;
            if ((game.gameState.IsPlayerOneTurn && game.isCurrentPlayer_PlayerOne) || (!game.gameState.IsPlayerOneTurn && !game.isCurrentPlayer_PlayerOne)) {
                playerPieceUIScript.PlayAnimation();
                // Animation jump = playerPieceUI.GetComponent<Animation>();
                opponentPieceUIScript.StopAnimation();
            } else if ((game.gameState.IsPlayerOneTurn && !game.isCurrentPlayer_PlayerOne) || (!game.gameState.IsPlayerOneTurn && game.isCurrentPlayer_PlayerOne)) {
                opponentPieceUIScript.PlayAnimation();
                // Animation jump = opponentPieceUIAnimator.GetComponent<Animation>();
                playerPieceUIScript.StopAnimation();
            }
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

        IEnumerator WaitToEnableInput()
        {
            yield return new WaitForSeconds(1.5f);
            UserInputHandler.inputEnabled = true;
        }

        private void ProcessPlayerInput(Vector3 mousePosition)
        {
            Debug.Log("Gamemanager: disableInput: " + disableInput + ", isLoading: " + isLoading + ", isLoadingUI: " + isLoadingUI + ",isDropping: " + isDropping + ", numPiecesAnimating: " + numPiecesAnimating + ", isGameOver: "+ game.gameState.IsGameOver);

            Vector3 pos = Camera.main.ScreenToWorldPoint(mousePosition);
            //GameObject particle = SpawnParticle();
            //particle.transform.position = new Vector3(Mathf.RoundToInt(pos.x), Mathf.CeilToInt((pos.y * -1 - .3f))*-1);

            if (disableInput || isLoading || isLoadingUI || isDropping || game.gameState.IsGameOver || numPiecesAnimating > 0)
            {
                Debug.Log("returned in process player input");
                return;
            }

            // Debug.Log("Gamemanager: gameState.isCurrentPlayerTurn: " + gameState.isCurrentPlayerTurn);

            if (game.gameState.isCurrentPlayerTurn)
            {
                // round to a grid square
                // Debug.Log("Move Position: x: " + pos.x + " y: " + (pos.y * -1 - .3f));
                int column = Mathf.RoundToInt(pos.x);
                int row = Mathf.CeilToInt((pos.y * - 1 - .3f));

                Position position;
                PlayerEnum player = game.gameState.IsPlayerOneTurn ? PlayerEnum.ONE : PlayerEnum.TWO;
                //Debug.Log("ProcessPlayerInput: column: " + column + " row: " + row);
                if (Utility.inTopRowBounds(pos.x, pos.y))
                {
                    position = new Position(column, row - 1);
                    //Debug.Log("Move Position: col: " + position.column + " row: " + position.row);
                    Move move = new Move(position, Direction.DOWN, player);
                    StartCoroutine(ProcessMove(move));
                }
                else if (Utility.inBottomRowBounds(pos.x, pos.y))
                {
                    position = new Position(column, row + 1);
                    Move move = new Move(position, Direction.UP, player);
                    //Debug.Log("Move Position: col: " + position.column + " row: " + position.row);
                    StartCoroutine(ProcessMove(move));
                }
                else if (Utility.inRightRowBounds(pos.x, pos.y))
                {
                    position = new Position(column + 1, row);
                    Move move = new Move(position, Direction.LEFT, player);
                    //Debug.Log("Move Position: col: " + position.column + " row: " + position.row);
                    StartCoroutine(ProcessMove(move));
                }
                else if (Utility.inLeftRowBounds(pos.x, pos.y))
                {
                    position = new Position(column - 1, row);
                    Move move = new Move(position, Direction.RIGHT, player);
                    //Debug.Log("Move Position: col: " + position.column + " row: " + position.row);
                    StartCoroutine(ProcessMove(move));
                }
            }
            else
            {
                Debug.Log("Not isCurrentPlayerTurn: challengeInstanceId: " + game.challengeId);
                if (game.challengeId != null) {
                    if (Utility.inTopRowBounds(pos.x, pos.y) || Utility.inBottomRowBounds(pos.x, pos.y) || Utility.inRightRowBounds(pos.x, pos.y) || Utility.inLeftRowBounds(pos.x, pos.y))
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
                        while (isDropping && numPiecesAnimating > 0)
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
            // GameObject[] cornerArrows = GameObject.FindGameObjectsWithTag("Arrow");
            // foreach (GameObject cornerArrow in cornerArrows) {
            //     SpriteRenderer sr = cornerArrow.GetComponentInChildren<SpriteRenderer>();
            //     sr.enabled = false;
            // }

            // numPiecesAnimating = 0;
            isDropping = true;

            if (OnStartMove != null)
                OnStartMove();
            
            SoundManager.instance.PlayRandomizedSfx(clipMove);

            List<IToken> activeTokens;

            game.gameState.PrintGameState("BeforeMove");
            List<MovingGamePiece> movingPieces = game.gameState.MovePiece(move, replayMove, out activeTokens);
            game.gameState.PrintGameState("AfterMove");

            MoveGamePieceViews(move, movingPieces, activeTokens);

            yield return new WaitWhile(() => numPiecesAnimating > 0);

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
                        Debug.Log("test test: " + PlayerPrefs.GetInt("PuzzleChallengeID:" + game.puzzleChallengeInfo.ID, 0));
                        if (PlayerPrefs.GetInt("PuzzleChallengeID:" + game.puzzleChallengeInfo.ID, 0) == 0) {
                            Debug.Log("Succssfully submitted puzzle complted");
                            ChallengeManager.instance.SubmitPuzzleCompleted();
                        }
                        
                        PlayerPrefs.SetInt("PuzzleChallengeID:" + game.puzzleChallengeInfo.ID, 1);
                        // if (OnPuzzleCompleted != null)
                            // OnPuzzleCompleted(game.puzzleChallengeInfo);
                        // PlayerPrefs.SetInt("puzzleChallengeLevel", game.puzzleChallengeInfo.Level);
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
            if (game.gameState.IsGameOver || game.isExpired)
            {
                DisplayGameOverView();
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
                UpdatePlayerUI();
            }
        }



        private void MoveGamePieceViews(Move move, List<MovingGamePiece> movingPieces, List<IToken> activeTokens)
        {
            // animatingGamePieces = true;

            // Create Game Piece View
            GamePiece gamePiece = SpawnPiece(move.position.column, move.position.row * -1, move.player, PieceAnimState.MOVING);
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
            if (game.isExpired)
            {
                GameManager.instance.VisitedGameResults(game);
                gameInfo.Open(LocalizationManager.Instance.GetLocalizedValue("expired_text"), Color.white, false, !game.didViewResult);
                return;
            }

            Debug.Log("DisplayGameOverView gameState.winner: " +  game.gameState.Winner);

            this.ShowWinnerAnimation();
            this.ShowWinnerTextAndPlaySound();
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

        private void ShowWinnerAnimation()
        {
            for (int i = 0; i < game.gameState.GameBoard.player1WinningPositions.Count; i++)
            {
                Position position = game.gameState.GameBoard.player1WinningPositions[i];
                GamePiece gamePiece = gameBoardView.GamePieceAt(position);
                gamePiece.View.ShowWinOutline(bluePlayerColor);
            }

            for (int i = 0; i < game.gameState.GameBoard.player2WinningPositions.Count; i++)
            {
                Position position = game.gameState.GameBoard.player2WinningPositions[i];
                GamePiece gamePiece = gameBoardView.GamePieceAt(position);
                gamePiece.View.ShowWinOutline(redPlayerColor);
            }
        }

        private void ShowWinnerTextAndPlaySound()
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
                    //SoundManager.instance.PlayRandomizedSfx(clipWin);
                }
                else
                {
                    if (game.isCurrentPlayer_PlayerOne && game.gameState.Winner == PlayerEnum.ONE)
                    {
                        SoundManager.instance.PlayRandomizedSfx(clipWin);
                        gameInfo.Open(UserManager.instance.userName + LocalizationManager.Instance.GetLocalizedValue("won_suffix"), winnerTextColor, true, showRewardButton);
                    }
                    else if (!game.isCurrentPlayer_PlayerOne && game.gameState.Winner == PlayerEnum.TWO)
                    {
                        SoundManager.instance.PlayRandomizedSfx(clipWin);
                        gameInfo.Open(UserManager.instance.userName + LocalizationManager.Instance.GetLocalizedValue("won_suffix"), winnerTextColor, true, showRewardButton);
                    }
                    else
                    {
                        gameInfo.Open(opponentNameText.text + LocalizationManager.Instance.GetLocalizedValue("won_suffix"), winnerTextColor, true, showRewardButton);
                    }

                    //AnalyticsManager.LogGameOver("multiplayer", gameState.winner, gameState.tokenBoard);
                }
            }
            else
            {
                SoundManager.instance.PlayRandomizedSfx(clipWin);
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

		// public void AnimatePlayerPieceUI() {
        //     float animationSpeed = 0.8f;
        //     if ((gameState.IsPlayerOneTurn && isCurrentPlayer_PlayerOne) || (!gameState.IsPlayerOneTurn && !isCurrentPlayer_PlayerOne)) {
        //         //Animate current player piece to center
        //         Vector2 centerPos = new Vector2(439, -125);
        //         RectTransform pprt = playerPieceUI.GetComponent<RectTransform>();
        //         pprt.DOAnchorPos(centerPos, animationSpeed);
        //         playerPieceUI.transform.DOScale(new Vector3(1.3f, 1.3f), 1);

        //         //Animate opponent piece to upper right
        //         Vector3 startingPos = new Vector2(-130, -83);
        //         RectTransform oprt = opponentPieceUI.GetComponent<RectTransform>();
        //         oprt.DOAnchorPos(startingPos, animationSpeed);
        //         opponentPieceUI.transform.DOScale(new Vector3(1, 1), animationSpeed);
        //     } else if ((gameState.IsPlayerOneTurn && !isCurrentPlayer_PlayerOne) || (!gameState.IsPlayerOneTurn && isCurrentPlayer_PlayerOne)) {
        //         //Animate player piece to upper left
        //         Vector2 startingPos = new Vector2(175, -83);
        //         RectTransform pprt = playerPieceUI.GetComponent<RectTransform>();
        //         pprt.DOAnchorPos(startingPos, 1);
        //         playerPieceUI.transform.DOScale(new Vector3(1, 1), animationSpeed);

        //         // Animate opponent piece to center
        //         Vector2 centerPos = new Vector2(-370, -125);
        //         RectTransform oprt = opponentPieceUI.GetComponent<RectTransform>();
        //         oprt.DOAnchorPos(centerPos, 1);
        //         opponentPieceUI.transform.DOScale(new Vector3(1.3f, 1.3f), animationSpeed);
        //     }
        // }
	}
}

