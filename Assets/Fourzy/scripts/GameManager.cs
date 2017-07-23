using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GameSparks.Api.Requests;
using GameSparks.Api.Messages;
using GameSparks.Core;
using System.Linq;
using DG.Tweening;
using Lean;
using UnityEngine.Analytics.Experimental;

namespace Fourzy
{
    public class GameManager : MonoBehaviour
    {
        [Range(3, 8)]
        public int numRows = Constants.numRows;
        [Range(3, 8)]
        public int numColumns = Constants.numColumns;

        [Tooltip("How many pieces have to be connected to win.")]
        public int numPiecesToWin = Constants.numPiecesToWin;

        [Tooltip("Allow diagonally connected Pieces?")]
        public bool allowDiagonally = true;

        public float dropTime = 1.0f;

        // GameSparks
        public string challengeInstanceId;

        public GameObject gamePiecePrefab;
        public GameObject cornerSpot;
        public GameObject upArrowToken;
        public GameObject downArrowToken;
        public GameObject leftArrowToken;
        public GameObject rightArrowToken;
        public GameObject stickyToken;
        public GameObject blockerToken;
        public GameObject ghostToken;
        public GameObject moveArrowLeft;
        public GameObject moveArrowRight;
        public GameObject moveArrowDown;
        public GameObject moveArrowUp;
        public GameBoardView gameBoardView;
        public TokenBoard tokenBoard;
        public GameBoard gameBoard;
        public List<GameObject> tokenViews;
        public Button rematchButton;
        public Button nextGameButton;
        public GameObject CreateGameScreen;
        public Text gameStatusText;
        public Sprite playerOneSprite;
        public Sprite playerTwoSprite;
        public string bluePlayerWonText = "Blue Player Won!";
        public string redPlayerWonText = "Red Player Won!";
        public string bluePlayerMoveText = "Blue Player's Move";
        public string redPlayerMoveText = "Red Player's Move";
        public string playerWonText = "You Won!";
        public string playerLoseText = "You Lose!";
        public string drawText = "Draw!";

        public Color bluePlayerColor = new Color(0f/255f, 176.0f/255f, 255.0f/255.0f);
        public Color redPlayerColor = new Color(254.0f/255.0f, 40.0f/255.0f, 81.0f/255.0f);

        public Shader lineShader = null;

        public GameObject gamePieces;
        public GameObject tokens;
        public GameObject gameScreen;
        public GameObject ErrorPanel;

        private GameObject UIScreen;

        public delegate void MoveAction();
        public static event MoveAction OnMoved;

		public bool isMultiplayer = false;
        public bool isNewChallenge = false;
        public bool isNewRandomChallenge = false;
        public bool isCurrentPlayerTurn = false;
		public bool isPlayerOneTurn = true;
        public bool isAiActive;
        public string challengedUserId;
        public int currentPlayerId;
        public string winner;
        public string opponentFacebookId;
        
		bool isLoading = true;
		bool isDropping = false; 
		public bool gameOver = false;
        bool isGameDrawn = false;
        bool didPlayer1Win = false;
        bool replayedLastMove = false;
		public bool isCheckingForWinner = false;
        public bool isAnimating = false;
        public float gameScreenFadeInTime = 0.6f;
        //int spacing = 1; //100
        //int offset = 0; //4

        public AudioClip clipMove;
        public AudioClip clipWin;
        private AudioSource audioMove;
        private AudioSource audioWin;

        public Text playerNameLabel;
        public Image playerProfilePicture;
        public Text opponentNameLabel;
        public Image opponentProfilePicture;
        public CreateGame createGameScript;

        public AiPlayer aiPlayer;

        //Singleton
        private static GameManager _instance;
        public static GameManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<GameManager>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }

        void OnApplicationPause(bool paused) {
            if (!paused)
            {
                if (ChallengeManager.instance)
                {
                    ChallengeManager.instance.GetChallenges();
                }
            } else {
                //createGameScript.ResetFindMatchButton();
                new EndSessionRequest()
                    .Send((response) => {
                        if (response.HasErrors) {
                            Debug.Log("***** EndSessionRequest:Error: " + response.Errors.JSON);
                        }
                    });
            }
        }

        private void TransitionToGameScreen() {
            UserInputHandler.inputEnabled = false;
            //FadeGamesListScreen(0.0f, false, 0.1f);
            UIScreen.SetActive(false);
            CreateGameScreen.SetActive(false);
            gameScreen.SetActive(true);
            FadeGameScreen(1.0f, gameScreenFadeInTime);
            StartCoroutine(WaitToEnableInput());
        }

        public void TransitionToCreateGameScreen() {
            BoardSelectionManager.instance.LoadMiniBoards();
            UIScreen.SetActive(false);
            CreateGameScreen.SetActive(true);
        }

        private void FadeGameScreen(float alpha, float fadeTime) {
            gameScreen.GetComponent<CanvasGroup>().DOFade(alpha, 0.5f).OnComplete(()=>FadeTokens(alpha, fadeTime));
        }

        private void WinLineSetActive(bool active) {
            LineRenderer[] winLines = gameScreen.GetComponentsInChildren<LineRenderer>(true);

            foreach (var line in winLines)
            {
                if (line) {
                    line.gameObject.SetActive(active);
                }
            }
        }
        private void FadePieces(float alpha, float fadeTime) {
        //    SpriteRenderer[] sprites = gameScreen.GetComponentsInChildren<SpriteRenderer>();
           
        //     for (int i=0; i < sprites.Length; i++)
        //     { 
        //         if (i < sprites.Length-1) {
        //             sprites[i].DOFade(alpha, fadeTime);
        //         } else {
        //             sprites[i].DOFade(alpha, fadeTime).OnComplete(()=>WinLineSetActive(true));
        //         }
        //     }

            GameObject[] pieces = GameObject.FindGameObjectsWithTag("GamePiece");
            for (int i=0; i < pieces.Length; i++)
            {
                if (i < pieces.Length-1) {
                    pieces[i].GetComponent<SpriteRenderer>().DOFade(alpha, fadeTime);
                } else {
                    pieces[i].GetComponent<SpriteRenderer>().DOFade(alpha, fadeTime).OnComplete(()=>WinLineSetActive(true));
                }
            }
        }

        private void FadeTokens(float alpha, float fadeTime) {
            GameObject[] tokens = GameObject.FindGameObjectsWithTag("Token");
            for (int i=0; i < tokens.Length; i++)
            {
                if (i < tokens.Length-1) {
                    tokens[i].GetComponent<SpriteRenderer>().DOFade(alpha, fadeTime);
                } else {
                    tokens[i].GetComponent<SpriteRenderer>().DOFade(alpha, fadeTime).OnComplete(()=>FadePieces(alpha, fadeTime));
                }
            }
        }

        private void FadeGamesListScreen(float alpha, bool enabled, float fadeTime) {
            UIScreen.GetComponent<CanvasGroup>().DOFade(alpha, fadeTime).OnComplete(()=>UIScreenSetActive(enabled));
        }

        private void GameScreenSetActive(bool setActive) {
            gameScreen.SetActive(setActive);
        }

        private void UIScreenSetActive(bool setActive) {
            UIScreen.SetActive(setActive);
        }

        public void TransitionToGamesListScreen() {
            UserInputHandler.inputEnabled = false;
            UIScreen.SetActive(true);
            gameScreen.GetComponent<CanvasGroup>().alpha = 0.0f;
            gameScreen.SetActive(false);
            // FadeGameScreen(0.0f, false, 0.4f);
            //FadeGamesListScreen(1.0f, true, 0.1f);
            challengeInstanceId = null;
            winner = null;
            isCurrentPlayerTurn = false;
        }

        IEnumerator WaitToEnableInput() {
            yield return new WaitForSeconds(1.5f);
            UserInputHandler.inputEnabled = true;
        }

        private void OnEnable()
        {
            UserInputHandler.OnTap += ProcessPlayerInput;
            ActiveGame.OnActiveGame += TransitionToGameScreen;
            FriendEntry.OnActiveGame += TransitionToGameScreen;
            ChallengeManager.OnActiveGame += TransitionToGameScreen;
            LoginManager.OnLoginError += DisplayLoginError;
        }

        private void OnDisable()
        {
            UserInputHandler.OnTap -= ProcessPlayerInput;
            ActiveGame.OnActiveGame -= TransitionToGameScreen;
            FriendEntry.OnActiveGame -= TransitionToGameScreen;
            ChallengeManager.OnActiveGame -= TransitionToGameScreen;
            LoginManager.OnLoginError -= DisplayLoginError;
        }

        private void CheckConnectionStatus(bool connected) {
            Debug.Log("CheckConnectionStatus: " + connected);
        }

		void Start() 
		{
            GS.GameSparksAvailable += CheckConnectionStatus;

            DOTween.Init(false, true, LogBehaviour.ErrorsOnly);

            audioMove = AddAudio(clipMove, false, false, 1);
            audioWin = AddAudio(clipWin, false, false, 1);

            MatchFoundMessage.Listener = (message) => {
                createGameScript.SetButtonStateWrapper(false);
                ChallengeManager.instance.GetChallenges();
            };

            ChallengeWonMessage.Listener = (message) => {
                var challenge = message.Challenge;
                if (UserManager.instance.userId != challenge.NextPlayer) {
                    // Only Replay the last move if the player is viewing the game screen for that game
                    if (challenge.ChallengeId == challengeInstanceId) {
                        List<GSData> moveList = challenge.ScriptData.GetGSDataList("moveList");
                        ReplayLastMove(moveList);
                        //gameStatusText.text = challenge.ScriptData.GetString("winnerName") + " Won!";
                        //gameStatusText.text = challenge.Challenger.Name + " Won!";
                        ChallengeManager.instance.SetViewedCompletedGame(challenge.ChallengeId);
                    }
                    ChallengeManager.instance.GetChallenges();
                }
            };

            ChallengeLostMessage.Listener = (message) => {
                var challenge = message.Challenge;
                if (UserManager.instance.userId != challenge.NextPlayer) {
                    // Only Replay the last move if the player is viewing the game screen for that game
                    if (challenge.ChallengeId == challengeInstanceId) {
                        List<GSData> moveList = challenge.ScriptData.GetGSDataList("moveList");
                        ReplayLastMove(moveList);
                        //gameStatusText.text = challenge.ScriptData.GetString("winnerName") + " Won!";
                        //gameStatusText.text = challenge.Challenged.First().Name + " Won!";
                        ChallengeManager.instance.SetViewedCompletedGame(challenge.ChallengeId);
                    }
                    ChallengeManager.instance.GetChallenges();
                }
            };
                
            ChallengeTurnTakenMessage.Listener = (message) => {
                var challenge = message.Challenge;
                
                if (UserManager.instance.userId == challenge.NextPlayer && !replayedLastMove) {
                    // Only Replay the last move if the player is viewing the game screen for that game
                    if (challenge.ChallengeId == challengeInstanceId) {
                        replayedLastMove = true;
                        List<GSData> moveList = challenge.ScriptData.GetGSDataList("moveList");
                        ReplayLastMove(moveList);
                        //AnimateEmptyEdgeSpots(true);
                    }
                    ChallengeManager.instance.GetChallenges();
                }
            };

            int max = Mathf.Max(numRows, numColumns);
            
            if(numPiecesToWin > max)
                numPiecesToWin = max;

            //gamePieces = new GameObject("GamePieces");
            //gamePieces.transform.parent = gameScreen.transform;

            tokens = new GameObject("Tokens");
            tokens.transform.parent = gameScreen.transform;

            gameBoard = new GameBoard(Constants.numRows, Constants.numColumns, Constants.numPiecesToWin);

            UIScreen = GameObject.Find("UI Screen");

            rematchButton.gameObject.SetActive(false);
            gameScreen.SetActive(false);

            // center camera
            Camera.main.transform.position = new Vector3((numColumns-1) / 2.0f, -((numRows-1) / 2.0f)+.2f, Camera.main.transform.position.z);
        }

        void Awake() {
            replayedLastMove = false;
        }

        public AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol) { 
            AudioSource newAudio = gameObject.AddComponent<AudioSource>();
            newAudio.clip = clip; 
            newAudio.loop = loop;
            newAudio.playOnAwake = playAwake;
            newAudio.volume = vol; 
            return newAudio; 
        }

        private void ReplayLastMove(List<GSData> moveList) {
            GSData lastMove = moveList.Last();
            int position = lastMove.GetInt("position").GetValueOrDefault();
            Direction direction = (Direction)lastMove.GetInt("direction").GetValueOrDefault();
            int player = lastMove.GetInt("player").GetValueOrDefault();

            if (player == (int)Piece.BLUE) {
                isPlayerOneTurn = true;
            } else if (player == (int)Piece.RED) {
                isPlayerOneTurn = false;
            }

            Move move = new Move(position, direction);
            StartCoroutine(MovePiece(move, true));
        }

        public void SetupGameWrapper(int[] boardData, TokenBoard tokenBoard) {
            this.tokenBoard = tokenBoard;
            StartCoroutine(SetupGame(boardData));
        }

        public IEnumerator SetupGame(int[] boardData) {
            gameBoard.SetGameBoard(boardData, tokenBoard.tokens);
            
            StartCoroutine(SetGameBoardView(gameBoard.GetBoard()));
            while (isLoading)
                yield return null;
            StartCoroutine(CreateTokens());
            while (isLoading)
              yield return null;
            
            StartCoroutine(CheckWinners());
        }

        public IEnumerator CheckWinners() {
            // Check if Player one is the winner
            StartCoroutine(CheckForWinner(true));

            if (gameOver == false)
            {
                // Check if Player two is the winner
                StartCoroutine(CheckForWinner(false));
            }

            // wait until winning check is done
            while (this.isCheckingForWinner == true)
            {
                yield return null;
            }

            DisplayGameOverView();
            UpdateGameStatusText();
        }

        public IEnumerator SetGameBoardView(int[,] board) {
            isLoading = true;
            gameBoardView.gamePieces = new GameObject[numRows, numColumns];

            for(int row = 0; row < numRows; row++)
            {
                for(int col = 0; col < numColumns; col++)
                {
                    int piece = board[row, col];

                    if (piece == (int)Piece.BLUE)
                    {
                        GameObject pieceObject = SpawnPiece(col, row * -1, Player.ONE);
                        //TODO: Use player chosen sprite for gamepiece
                        SpriteRenderer pieceSprite = pieceObject.GetComponent<SpriteRenderer>();
                        Color c = pieceSprite.color;
                        c.a = 0.0f;
                        pieceSprite.color = c;
                        //pieceSprite.sprite = new Sprite().texture.
                        
                        GamePiece pieceModel = pieceObject.GetComponent<GamePiece>();
                        pieceModel.player = Player.ONE;
                        gameBoardView.gamePieces[row, col] = pieceObject;
                    }
                    else if (piece == (int)Piece.RED)
                    {
                        GameObject pieceObject = SpawnPiece(col, row * -1, Player.TWO);
                        SpriteRenderer pieceSprite = pieceObject.GetComponent<SpriteRenderer>();
                        Color c = pieceSprite.color;
                        c.a = 0.0f;
                        pieceSprite.color = c;
                        //pieceSprite.sprite = new Sprite().texture.
                        GamePiece pieceModel = pieceObject.GetComponent<GamePiece>();
                        pieceModel.player = Player.TWO;
                        gameBoardView.gamePieces[row, col] = pieceObject;
                    }
                }
            }
                
            isLoading = false;
            yield return 0;
        }

        public IEnumerator CreateTokens() {
            isLoading = true;
            tokenViews = new List<GameObject>();

            for(int row = 0; row < numRows; row++)
            {
                for(int col = 0; col < numColumns; col++)
                {
                    Token token = tokenBoard.tokens[row, col].tokenType;
                    GameObject go;
                    switch (token)
                    {
                        case Token.UP_ARROW:
                            go = Instantiate(upArrowToken, new Vector3(col, row * -1, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews.Add(go);
                            break;
                        case Token.DOWN_ARROW:
                            go = Instantiate(downArrowToken, new Vector3(col, row * -1, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews.Add(go);
                            break;
                        case Token.LEFT_ARROW:
                            go = Instantiate(leftArrowToken, new Vector3(col, row * -1, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews.Add(go);
                            break;
                        case Token.RIGHT_ARROW:
                            go = Instantiate(rightArrowToken, new Vector3(col, row * -1, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews.Add(go);
                            break;
                        case Token.STICKY:
                            go = Instantiate(stickyToken, new Vector3(col, row * -1, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews.Add(go);
                            break;
                        case Token.BLOCKER:
                            go = Instantiate(blockerToken, new Vector3(col, row * -1, 15), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews.Add(go);
                            break;
                        case Token.GHOST:
                            go = Instantiate(ghostToken, new Vector3(col, row * -1, 5), Quaternion.identity, tokens.transform);
                            Utility.SetSpriteAlpha(go, 0.0f);
                            tokenViews.Add(go);
                            break;
                        default:
                            break;
                    }
                }
            }

            isLoading = false;
            yield return 0;
        }

        public void EnableTokenAudio() {
            foreach (var item in tokenViews)
            {
                if (item.GetComponent<AudioSource>()) {
                    item.GetComponent<AudioSource>().mute = false;
                }
            }
        }

        public void DisplayGameOverView() {
            if (gameOver == true)
            {
                if (isGameDrawn) {
                    gameStatusText.text = drawText;
                } else if (isMultiplayer) {
                    if (winner != null && winner.Length > 0)
                    {
                        gameStatusText.text = winner + " Won!";
                    }
                    else
                    {
                        if (isCurrentPlayerTurn && (isPlayerOneTurn == didPlayer1Win)) {
                            audioWin.Play();
                            gameStatusText.text = UserManager.instance.userName + " Won!";
                        } else {
                            gameStatusText.text = opponentNameLabel.text + " Won!";
                        }

                        gameStatusText.color = didPlayer1Win ? bluePlayerColor : redPlayerColor;
                    }
                } else {
                    AnalyticsEvent.GameOver("local_game");
                    if (isPlayerOneTurn == didPlayer1Win) {
                        audioWin.Play();
                    }
                    gameStatusText.text = didPlayer1Win ? bluePlayerWonText : redPlayerWonText;
                    gameStatusText.color = didPlayer1Win ? bluePlayerColor : redPlayerColor;
                }
            }
        }

        public void UpdateGameStatusText() {
            if (!gameOver)
            {
                if (isMultiplayer)
                {
                    if (isCurrentPlayerTurn)
                    {
                        gameStatusText.text = "Your Move";
                        gameStatusText.color = isPlayerOneTurn ? bluePlayerColor : redPlayerColor;
                    }
                    else
                    {
                        gameStatusText.text = "Their Move";
                        gameStatusText.color = isPlayerOneTurn ? bluePlayerColor : redPlayerColor;
                    }
                }
                else
                {
                    gameStatusText.text = isPlayerOneTurn ? bluePlayerMoveText : redPlayerMoveText;
                    gameStatusText.color = isPlayerOneTurn ? bluePlayerColor : redPlayerColor;
                }
            }
        }

        public void ResetUI() {
            if (isCurrentPlayerTurn) {
                playerNameLabel.color = isPlayerOneTurn ? bluePlayerColor : redPlayerColor;
                opponentNameLabel.color = isPlayerOneTurn ? redPlayerColor : bluePlayerColor;
                AnimateEmptyEdgeSpots(true);
            } else {
                playerNameLabel.color = isPlayerOneTurn ? redPlayerColor : bluePlayerColor;
                opponentNameLabel.color = isPlayerOneTurn ? bluePlayerColor : redPlayerColor;
            }

            rematchButton.gameObject.SetActive(false);
        }

        public void ResetGameBoard() {

            isLoading = true;
            //int count = 0;
            //Debug.Log("GAMEPIECES COUNT: " + gamePieces.transform.childCount);
            if (gamePieces.transform.childCount > 0) {
                for (int i = gamePieces.transform.childCount-1; i >= 0; i--)
                {
                    Transform piece = gamePieces.transform.GetChild(i);
                    Lean.LeanPool.Despawn(piece.gameObject);
                }
            }

            // foreach (Transform piece in gamePieces.transform)
            // {
            //     count++;
            //     Debug.Log("count: " + count);
            //     Lean.LeanPool.Despawn(piece.gameObject);
            // }

            if (tokens.transform.childCount > 0) {
                for (int i = tokens.transform.childCount-1; i >= 0; i--)
                {
                    Transform token = tokens.transform.GetChild(i);
                    DestroyImmediate(token.gameObject);
                }
            }

            // if(tokens != null)
            // {
            //     DestroyImmediate(tokens);
            // }
            // tokens = new GameObject("Tokens");
            // tokens.transform.parent = gameScreen.transform;
            // tokens.transform.localPosition = new Vector3(-375f, -501f);

            gameBoard = new GameBoard(Constants.numRows, Constants.numColumns, Constants.numPiecesToWin);

            isLoading = false;
            gameOver = false;
        }

        public void PopulateMoveArrows() {
            // for(int col = 0; col < numColumns; col++)
            // {
            //     for(int row = 0; row < numRows; row++)
            //     {
            //         GameObject g;

            //         if (col == numColumns - 1 && row > 0 && row < numRows - 1) {
            //             g = Instantiate(moveArrowLeft, new Vector3((col - offset) * spacing, ((row - offset) * spacing) * -1, 10), Quaternion.identity) as GameObject;
            //             g.transform.parent = gamePieces.transform;
            //         }
            //         if (col == 0 && row > 0 && row < numRows - 1) {
            //             g = Instantiate(moveArrowRight, new Vector3((col - offset) * spacing, ((row - offset) * spacing) * -1, 10), Quaternion.identity) as GameObject;
            //             g.transform.parent = gamePieces.transform;
            //         }
            //         if (row == 0 && col > 0 && col < numColumns - 1) {
            //             if (gameBoard.GetCell(col, row) == 0) {
            //                 g = Instantiate(moveArrowDown, new Vector3((col - offset) * spacing, ((row - offset) * spacing) * -1, 10), Quaternion.identity) as GameObject;
            //                 g.transform.parent = gamePieces.transform;
            //             }
            //         }
            //         if (row == numRows - 1 && col > 0 && col < numColumns - 1) {
            //             if (gameBoard.GetCell(col, row) == 0) {
            //                 g = Instantiate(moveArrowUp, new Vector3((col - offset) * spacing, ((row - offset) * spacing) * -1, 10), Quaternion.identity) as GameObject;
            //                 g.transform.parent = gamePieces.transform;
            //             }
            //         }
            //     }
            // }
        }            

        public void AnimateEmptyEdgeSpots(bool animate) {
            foreach (EmptySpot spot in gameScreen.GetComponentsInChildren<EmptySpot>())
            {
                StartCoroutine(spot.AnimateSpot(animate));
            }
        } 

        /// <summary>
        /// Spawns a gamepiece at the given column and row
        /// </summary>
        /// <returns>The piece.</returns>
        GameObject SpawnPiece(float posX, float posY, Player player)
        {
            GameObject gamePiece = Lean.LeanPool.Spawn(gamePiecePrefab,
            new Vector3(Mathf.FloorToInt(posX + 0.5f), 
                Mathf.FloorToInt(posY + 0.5f), 10),
                Quaternion.identity, gamePieces.transform) as GameObject;
            
            gamePiece.transform.position = new Vector3(Mathf.FloorToInt(posX + 0.5f), 
                Mathf.FloorToInt(posY + 0.5f), 10);
            // GameObject gamePiece = Instantiate(
            //     isPlayerOneTurn ? pieceBlue : pieceRed,
            //     new Vector3(Mathf.FloorToInt(posX + 0.5f),
            //     Mathf.FloorToInt(posY + 0.5f), 10), // spawn it above the first row
            //     Quaternion.identity, gamePieces.transform) as GameObject;
            
            if (player == Player.ONE) {
                gamePiece.GetComponent<SpriteRenderer>().sprite = playerOneSprite;
                // Layer for player 1 pieces
                gamePiece.layer = 8;
            } else {
                gamePiece.GetComponent<SpriteRenderer>().sprite = playerTwoSprite;
                // Layer for player 2 pieces
                gamePiece.layer = 9;
            }
            gamePiece.GetComponent<SpriteRenderer>().enabled = true;
            
            return gamePiece;
		}

        public void RematchPassAndPlayGame() {
            gameScreen.GetComponent<CanvasGroup>().alpha = 0.0f;
            ResetGameBoard();
            PopulateMoveArrows();
            
            TokenBoard tokenBoard = TokenBoardLoader.instance.GetTokenBoard();
            GameManager.instance.tokenBoard = tokenBoard;
            StartCoroutine(GameManager.instance.CreateTokens());
            gameOver = false;
            UpdateGameStatusText();
            rematchButton.gameObject.SetActive(false);
            GameManager.instance.EnableTokenAudio();
            
            FadeGameScreen(1.0f, gameScreenFadeInTime);
        }

        void Update () {
            if(!isMultiplayer && gameOver)
            {
                rematchButton.gameObject.SetActive(true);
            }
        }

        private void DisplayLoginError() {
            ErrorPanel.SetActive(true);
        }

        private void ProcessPlayerInput(Vector3 mousePosition) {

            if(isLoading)
                return;

            if(isCheckingForWinner)
                return;
            
            if(gameOver)
            {
                return;
            }

            if(isCurrentPlayerTurn)
            {
                if (!isDropping) {
                    Vector3 pos = Camera.main.ScreenToWorldPoint(mousePosition);

                    // round to a grid cell
                    int column = Mathf.RoundToInt(pos.x);
                    int row = Mathf.RoundToInt(pos.y * -1);
                    Position position;

                    if (inTopRowBounds (pos.x, pos.y)) {
                        position = new Position(column, row - 1);
                        //Debug.Log("Move Position: col: " + position.column + " row: " + position.row);
                        Move move = new Move(position, Direction.DOWN);
                        StartCoroutine(MovePiece(move, false));
                    } else if (inBottomRowBounds(pos.x, pos.y)) {
                        position = new Position(column, row + 1);
                        Move move = new Move(position, Direction.UP);
                        //Debug.Log("Move Position: col: " + position.column + " row: " + position.row);
                        StartCoroutine(MovePiece(move, false));
                    } else if (inRightRowBounds(pos.x, pos.y)) {
                        position = new Position(column + 1, row);
                        Move move = new Move(position, Direction.LEFT);
                        //Debug.Log("Move Position: col: " + position.column + " row: " + position.row);
                        StartCoroutine(MovePiece(move, false));
                    } else if (inLeftRowBounds(pos.x, pos.y)) {
                        position = new Position(column - 1, row);
                        Move move = new Move(position, Direction.RIGHT);
                        //Debug.Log("Move Position: col: " + position.column + " row: " + position.row);
                        StartCoroutine(MovePiece(move, false));
                    }
                }
            } else { 
                // TODO: inform the player it is not their turn
            }
        }

        public int GetMoveLocation(Move move) {
            int movePosition = -1;

            switch (move.direction)
            {
                case Direction.UP:
                    movePosition = move.position.column;
                    break;
                case Direction.DOWN:
                    movePosition = move.position.column;
                    break;
                case Direction.LEFT:
                    movePosition = move.position.row;
                    break;
                case Direction.RIGHT:
                    movePosition = move.position.row;
                    break;
                default:
                    break;
            }

            return movePosition;
        }

        // MovePiece
        // 1. Add the piece to moving pieces
        //   While board.movingpiece contains an active moving piece
        // in case of undo feature, save the initial board state
        // 2. Evalute at Token at row 0 col 3, empty token moves the piece to row 1 col 3, no new board state
        // 3. Evaluate token at row 1 col 3 left arrow, return a new board state, update movingpiece with a new position,
        //    and update moving piece to row 1 col 2
        // Final result
        // 1. Final board state
        // 2. List of completed moving pieces and list of positions for those pieces to be used for piece animation
        public IEnumerator MovePiece(Move move, bool replayMove)
        {
            Debug.Log("BEGIN MOVE PIECE");
            isDropping = true;

            MovingGamePiece activeMovingPiece = new MovingGamePiece(move);

            if (gameBoard.CanMove(new Move(activeMovingPiece.GetNextPosition(), move.direction), tokenBoard.tokens))
            {
                //Debug.Log("replayMove: " + replayMove + " ismultiplayer: " + isMultiplayer + " isNewChallenge: " + isNewChallenge + " isNewRandomChallenge: " + isNewRandomChallenge);

                if (!replayMove && isMultiplayer && !isNewChallenge && !isNewRandomChallenge)
                {
                    replayedLastMove = false;
                    StartCoroutine(ProcessMove(move.position, move.direction, replayMove));
                    Debug.Log("LogChallengeEventRequest: challengeInstanceId: " + challengeInstanceId);
                    new LogChallengeEventRequest().SetChallengeInstanceId(challengeInstanceId)
                        .SetEventKey("takeTurn") //The event we are calling is "takeTurn", we set this up on the GameSparks Portal
                        .SetEventAttribute("pos", GetMoveLocation(move)) // pos is the row or column the piece was placed at depending on the direction
                        .SetEventAttribute("direction", move.direction.GetHashCode()) // direction can be up, down, left, right
                        .SetEventAttribute("player", isPlayerOneTurn ? (int)Piece.BLUE : (int)Piece.RED)
                        .SetDurable(true)
                        .Send((response) =>
                            {
                                if (response.HasErrors)
                                {
                                    Debug.Log("***** ChallengeEventRequest failed: " + response.Errors.JSON);
                                    gameStatusText.text = "There was a problem making your move. Please try again.";
                                }
                                else
                                {
                                    // If our ChallengeEventRequest was successful we inform the player
                                    Debug.Log("ChallengeEventRequest was successful");
                                }
                            });
                }
                else if (isMultiplayer && isNewChallenge)
                {
                    isNewChallenge = false;
                    StartCoroutine(ProcessMove(move.position, move.direction, replayMove));
                    ChallengeManager.instance.ChallengeUser(challengedUserId, gameBoard.GetGameBoardData(), tokenBoard, GetMoveLocation(move), move.direction);
                }
                else if (isMultiplayer && isNewRandomChallenge)
                {
                    isNewRandomChallenge = false;
                    StartCoroutine(ProcessMove(move.position, move.direction, replayMove));
                    ChallengeManager.instance.ChallengeRandomUser(gameBoard.GetGameBoardData(), tokenBoard, GetMoveLocation(move), move.direction);
                } else {
                    StartCoroutine(ProcessMove(move.position, move.direction, replayMove));
                }
            } else {
                // TODO: inform the player that the move is not possible
                gameBoard.activeMovingPieces.Clear();
                isDropping = false;
            }

            yield return 0;
        }

        private IEnumerator ProcessMove(Position position, Direction direction, bool replayMove) {
            gameBoard.PrintBoard("BeforeMove");

            Dictionary<GameObject, List<Position>> GameBoardViewUpdates = new Dictionary<GameObject, List<Position>>();

            audioMove.Play();
            GameObject[] cornerArrows = GameObject.FindGameObjectsWithTag("Arrow");
            foreach (GameObject cornerArrow in cornerArrows) {
                SpriteRenderer sr = cornerArrow.GetComponentInChildren<SpriteRenderer>();
                sr.enabled = false;
            }

            MovingGamePiece activeMovingPiece = new MovingGamePiece(new Move(position, direction));
            gameBoard.activeMovingPieces.Add(activeMovingPiece);

            Position movePosition = activeMovingPiece.GetNextPosition();
            GameObject g = SpawnPiece(movePosition.column, movePosition.row * -1, isPlayerOneTurn ? Player.ONE : Player.TWO);
            GamePiece gamePiece = g.GetComponent<GamePiece>();
            gamePiece.player = isPlayerOneTurn ? Player.ONE : Player.TWO;
            gamePiece.column = movePosition.column;
            gamePiece.row = movePosition.row;

            gameBoardView.gamePieces[movePosition.row, movePosition.column] = g;
            Debug.Log("Move position row: " + movePosition.row + " column: " + movePosition.column);

            gameBoard.SetCell(movePosition.column, movePosition.row, isPlayerOneTurn ? Player.ONE : Player.TWO);

            tokenBoard.tokens[movePosition.row, movePosition.column].UpdateBoard(gameBoard, false);

            while (gameBoard.activeMovingPieces.Count > 0) {
                //Position startPosition = gameBoard.activeMovingPieces[0].GetCurrentPosition();
                MovingGamePiece activePiece = gameBoard.activeMovingPieces[0];
                Position endPosition = activePiece.GetNextPosition();
                Direction activeDirection = activePiece.currentDirection;

                if (gameBoard.CanMove(new Move(endPosition, activeDirection), tokenBoard.tokens)) {
                    tokenBoard.tokens[endPosition.row, endPosition.column].UpdateBoard(gameBoard, true);
                } else {
                    gameBoard.DisableNextMovingPiece();
                }
            }

            gameBoard.UpdateMoveablePieces(tokenBoard.tokens);

            // process animations for completed moving pieces
            for (int i = 0; i < gameBoard.completedMovingPieces.Count; i++)
            {   
                var piece = gameBoard.completedMovingPieces[i];
                if (i==0) {
                    piece.positions.RemoveAt(0);
                }

                GameObject pieceView = gameBoardView.gamePieces[piece.positions[0].row, piece.positions[0].column];
                
                GameBoardViewUpdates.Add(pieceView, piece.positions);

                StartCoroutine(AnimatePiece(piece.positions));
                while(this.isAnimating)
                    yield return null;
            }

            foreach (var update in GameBoardViewUpdates.Reverse())
            {
                GameObject piece = update.Key;
                List<Position> positions = update.Value;
                
                Position startPosition = positions[0];
                Position endPosition = positions[positions.Count - 1];

                gameBoardView.gamePieces[endPosition.row, endPosition.column] = gameBoardView.gamePieces[startPosition.row, startPosition.column];
                gameBoardView.gamePieces[startPosition.row, startPosition.column] = null;
            }

            // foreach (var piece in gameBoard.completedMovingPieces)
            // {
            //     StartCoroutine(AnimatePiece(piece.positions));
            //     // gameBoardView.gamePieces[piece.positions[piece.positions.Count - 1].row, piece.positions[piece.positions.Count - 1].column] = gameBoardView.gamePieces[piece.positions[index].row, piece.positions[index].column];
            //     // if (piece.positions.Count > 2) {
            //     //     gameBoardView.gamePieces[piece.positions[index].row, piece.positions[index].column] = null;
            //     // }
            //     while(this.isAnimating)
            //         yield return null;
            // }

            gameBoard.PrintBoard("AfterMove");
            gameBoardView.PrintGameBoard();

            gameBoard.completedMovingPieces.Clear();

            // Check if Player one is the winner
            StartCoroutine(CheckForWinner(true));
            // Check if Player two is the winner
            StartCoroutine(CheckForWinner(false));

            // wait until winning check is done
            while(this.isCheckingForWinner)
                yield return null;

            if (gameOver) {
                DisplayGameOverView();
            }

            if (isCurrentPlayerTurn && gameOver) {
                ChallengeManager.instance.SetViewedCompletedGame(challengeInstanceId);
            }

            isPlayerOneTurn = !isPlayerOneTurn;
            if (isMultiplayer || isAiActive) {
                isCurrentPlayerTurn = !isCurrentPlayerTurn;    
            }
            UpdateGameStatusText();
            //AnimateEmptyEdgeSpots(false);

            foreach (GameObject cornerArrow in cornerArrows) {
                cornerArrow.SetActive(false);
                SpriteRenderer sr = cornerArrow.GetComponentInChildren<SpriteRenderer>();
                sr.enabled = true;
            }

            if (isAiActive && !gameOver && aiPlayer != null && !isCurrentPlayerTurn) {
                Move move = aiPlayer.GetMove(gameBoard, tokenBoard, isPlayerOneTurn ? 1 : 2);
                StartCoroutine(ProcessMove(move.position, move.direction, replayMove));
            }

            WinLineSetActive(true);

            isDropping = false;

            if(OnMoved != null)
                OnMoved();
        }

        private IEnumerator AnimatePiece(List<Position> positions) {
            isAnimating = true;
            // Debug.Log("POSITIONS: ");
            // foreach (var item in positions)
            // {
            //     Debug.Log("row: " + item.row + ", column: " + item.column);
            // }
            //Debug.Log("AnimatePiece: ROW: " + positions[positions.Count - 1].row + "COLUMN: " + positions[positions.Count - 1].column);
            //Debug.Log("AnimatePiece: ROW: " + positions[0].row + "COLUMN: " + positions[0].column);
            
            //GameObject g = gameBoardView.gamePieces[positions[positions.Count - 1].row, positions[positions.Count - 1].column];
            GameObject g = gameBoardView.gamePieces[positions[0].row, positions[0].column];
            
            //Vector3 start = new Vector3(positions[1].column, positions[1].row * -1);
            Sequence mySequence = DOTween.Sequence();
            for (int i = 0; i < positions.Count; i++)
            {
                Vector3 end = new Vector3(positions[i].column, positions[i].row * -1);
                //float distance = Vector3.Distance(start, end);

                // if (i < positions.Count) {
                //     mySequence.Append(g.transform.DOMove(end, dropTime, false).SetEase(Ease.Linear));
                // } else {
                //     mySequence.Append(g.transform.DOMove(end, dropTime, false).SetEase(Ease.Linear));
                // }

                mySequence.Append(g.transform.DOMove(end, dropTime, false).SetEase(Ease.Linear));

//                float t = 0;
//                while(t < 1)
//                {
//                    t += Time.deltaTime * dropTime;
//                    if (numRows - distance > 0) {
//                        t += (numRows - distance) / 500;
//                    }
//                    g.transform.position = Vector3.Lerp (start, end, t);
//
//                    yield return null;
//                }

                //start = end;
            }
            
            yield return mySequence.WaitForCompletion();
            isAnimating = false;
        }

		/// <summary>
		/// Check for Winner
		/// </summary>
		public IEnumerator CheckForWinner(bool playerOne)
		{
            isCheckingForWinner = true;
            //Debug.Log("Checking for winner");
			for(int x = 0; x < numColumns; x++)
			{
				for(int y = 0; y < numRows; y++)
				{
					// Get the Layermask to Raycast against, if its Players turn only include
					// Layermask Blue otherwise Layermask Red
                    int layermask = playerOne ? (1 << 8) : (1 << 9);
                    Position position = new Position(x, y);
                    if(gameBoard.PlayerAtPosition(position) != (playerOne ? Player.ONE : Player.TWO)) {
                        continue;
                    }

					// shoot a ray of length 'numPiecesToWin - 1' to the right to test horizontally
                    RaycastHit2D[] hitsHorz = Physics2D.RaycastAll(
						new Vector2(x, y * -1), 
                        Vector2.left, 
						numPiecesToWin - 1, 
						layermask);
                    //Debug.Log("hitsHorz: " + hitsHorz.Length);
					// return true (won) if enough hits
					if(hitsHorz.Length == numPiecesToWin)
					{
                        //Debug.Log("HORIZONTAL WIN");
                        Vector3 pos = new Vector3(hitsHorz[0].transform.position.x, hitsHorz[0].transform.position.y, 12);
                        DrawLine(pos, hitsHorz[3].transform.position, playerOne ? bluePlayerColor : redPlayerColor);
                        if (isCurrentPlayerTurn) {
                            #if UNITY_IOS || UNITY_ANDROID
                                Handheld.Vibrate();
                            #endif
                        }
                        gameOver = true;
                        didPlayer1Win = playerOne;
						break;
					}

					// shoot a ray up to test vertically
					RaycastHit2D[] hitsVert = Physics2D.RaycastAll(
						new Vector2(x, y * -1), 
                        Vector2.up, 
						numPiecesToWin - 1,
						layermask);
                    //Debug.Log("hitsVert: " + hitsVert.Length + "x: " + x + " y: " + y);
					if(hitsVert.Length == numPiecesToWin)
					{
                        //Debug.Log("VERTICAL WIN");
                        //SpriteRenderer glow =  hitsVert[3].transform.gameObject.GetComponentInChildren<SpriteRenderer>();
                        //glow.enabled = false;
                        Vector3 pos = new Vector3(hitsVert[0].transform.position.x, hitsVert[0].transform.position.y, 12);
                        DrawLine(pos, hitsVert[3].transform.position, playerOne ? bluePlayerColor : redPlayerColor);
                        if (isCurrentPlayerTurn) {
                            #if UNITY_IOS || UNITY_ANDROID
                                Handheld.Vibrate();
                            #endif
                        }
                        gameOver = true;
                        didPlayer1Win = playerOne;
						break;
					}

					// test diagonally
					if(allowDiagonally)
					{
						// calculate the length of the ray to shoot diagonally
						float length = Vector2.Distance(new Vector2(0, 0), new Vector2(Constants.numPiecesToWin - 1, numPiecesToWin - 1));

						RaycastHit2D[] hitsDiaLeft = Physics2D.RaycastAll(
							new Vector3(x, y * -1),
							new Vector3(-1 , 1),
							length,
							layermask);
						if(hitsDiaLeft.Length == numPiecesToWin)
						{
                            Vector3 pos = new Vector3(hitsDiaLeft[0].transform.position.x, hitsDiaLeft[0].transform.position.y, 12);
                            DrawLine(pos, hitsDiaLeft[3].transform.position, playerOne ? bluePlayerColor : redPlayerColor);
                            if (isCurrentPlayerTurn) {
                                #if UNITY_IOS || UNITY_ANDROID
                                    Handheld.Vibrate();
                                #endif
                            }
                            gameOver = true;
                            didPlayer1Win = playerOne;
							break;
						}

						RaycastHit2D[] hitsDiaRight = Physics2D.RaycastAll(
							new Vector3(x, y * -1),
							new Vector3(1 , 1),
							length,
							layermask);
						if(hitsDiaRight.Length == numPiecesToWin)
						{
                            Vector3 pos = new Vector3(hitsDiaRight[0].transform.position.x, hitsDiaRight[0].transform.position.y, 12);
                            DrawLine(pos, hitsDiaRight[3].transform.position, playerOne ? bluePlayerColor : redPlayerColor);
                            if (isCurrentPlayerTurn) {
                                #if UNITY_IOS || UNITY_ANDROID
                                    Handheld.Vibrate();
                                #endif  
                            }
                            gameOver = true;
                            didPlayer1Win = playerOne;
							break;
						}
					}

					yield return null;
				}

				yield return null;
			}

            //TODO: Check for a draw because their are no moves left
            // if (!FieldContainsEmptyCell())
            // {
            //     gameOver = true;
            //     isGameDrawn = true;
            // }
                
			this.isCheckingForWinner = false;

			yield return 0;
		}

        void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
        {
            Debug.Log("DRAWLINE");
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

        bool inTopRowBounds(float x, float y) {
            return x > 0.5 && x < numColumns - 1.5 && y > -0.5 && y < 0.5;
        }

        bool inBottomRowBounds(float x, float y) {
            return x > 0.5 && x < numColumns - 1.5 && y > -numColumns && y < -numColumns + 1.5;
        }

        bool inLeftRowBounds(float x, float y) {
            return x > - 0.5 && x < 0.5 && y > -numColumns + 1.5 && y < -0.5;
        }

		bool inRightRowBounds(float x, float y) {
			return x > numColumns - 1.5 && x < numColumns - 0.5 && y > -numColumns + 1.5 && y < -0.5;
		}

        public static void shineObject (GameObject obj, float width, float duration)
        {
            var mono = obj.GetComponent<MonoBehaviour>();
            if( mono != null) {
                Debug.Log("MonoBehaviour");
                // change material
                Material mat = Resources.Load ("Materials/ShineMaterial", typeof(Material)) as Material;
                var render = obj.GetComponent<SpriteRenderer> ();
                if (render != null) {
                    Debug.Log("render");
                    render.material = mat;
                } else {
                    var img = obj.GetComponent<Image> ();
                    if (img != null) {
                        img.material = mat;
                    } else {
                        Debug.LogWarning ("cannot get the render or image compoent!");
                    }
                }
                mat.SetFloat("_ShineWidth", width);
                // start a coroutine
                mono.StopAllCoroutines ();
                mono.StartCoroutine (shineRoutine (mat, duration));
            } else {
                Debug.LogWarning ("cannot find MonoBehaviour component!");
            }
        }

        static IEnumerator shineRoutine (Material mat, float duration)
        {
            if( mat != null ) {
                float location = 0f;
                float interval = 0.04f;
                float offsetVal = interval / duration;
                while(true) {
                    yield return new WaitForSeconds (interval);
                    mat.SetFloat("_ShineLocation", location);
                    location += offsetVal;
                    if (location > 1f) {
                        location = 0f;
                    }
                }
            } else {
                Debug.LogWarning ("there is no material parameter!");
            }
        }
	}
}
