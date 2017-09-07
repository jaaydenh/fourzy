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
        public float dropTime = 1.0f;
        // GameSparks Challenge Instance Id
        public string challengeInstanceId;
        public GameObject gamePiecePrefab;
        public GameObject cornerSpot;

        // ---------- Token Views ----------
        public GameObject upArrowToken;
        public GameObject downArrowToken;
        public GameObject leftArrowToken;
        public GameObject rightArrowToken;
        public GameObject stickyToken;
        public GameObject blockerToken;
        public GameObject ghostToken;
        public GameObject iceSheetToken;
        // ---------- Token Views ----------

        public GameObject moveArrowLeft;
        public GameObject moveArrowRight;
        public GameObject moveArrowDown;
        public GameObject moveArrowUp;
        public GameBoardView gameBoardView;
        public GameState gameState { get; set; }
        public List<GSData> moveList;
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
        public string drawText = "Draw!";

        public Color bluePlayerColor = new Color(0f/255f, 176.0f/255f, 255.0f/255.0f);
        public Color redPlayerColor = new Color(254.0f/255.0f, 40.0f/255.0f, 81.0f/255.0f);

        public Shader lineShader = null;

        // ---------- UI References ----------
        public GameObject gamePieces;
        public GameObject tokens;
        public GameObject gameScreen;
        public GameObject ErrorPanel;
        private GameObject UIScreen;
        public Text playerNameLabel;
        public Image playerProfilePicture;
        public Text opponentNameLabel;
        public Image opponentProfilePicture;
        // ---------- UI References ----------

        public delegate void MoveAction();
        public static event MoveAction OnMoved;

        public bool isMultiplayer = false;
        public bool isNewChallenge = false;
        public bool isNewRandomChallenge = false;
        public bool isAiActive;
        public string challengedUserId;
        public string winner;
        public string opponentFacebookId;
        bool isLoading = true;
        bool isDropping = false;
        bool updatingViews = false;
        //bool updatingModel = false;
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

            GameObject[] pieces = GameObject.FindGameObjectsWithTag("GamePiece");
            if (pieces.Length > 0) {
                for (int i=0; i < pieces.Length; i++)
                {
                    if (i < pieces.Length-1) {
                        pieces[i].GetComponent<SpriteRenderer>().DOFade(alpha, fadeTime);
                    } else {
                        pieces[i].GetComponent<SpriteRenderer>().DOFade(alpha, fadeTime).OnComplete(()=>ReplayLastMove());
                    }
                }
            } else {
                ReplayLastMove();
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
            gameState.isCurrentPlayerTurn = false;
            moveList = null;
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
                        int currentPlayerMove = challenge.ScriptData.GetInt("currentPlayerMove").GetValueOrDefault();
                        Player player = currentPlayerMove == 1 ? Player.ONE : Player.TWO;

                        ReplayIncomingOpponentMove(currentPlayerMove, moveList, player);
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
                        int currentPlayerMove = challenge.ScriptData.GetInt("currentPlayerMove").GetValueOrDefault();
                        Player player = currentPlayerMove == 1 ? Player.ONE : Player.TWO;

                        ReplayIncomingOpponentMove(currentPlayerMove, moveList, player);
                        ChallengeManager.instance.SetViewedCompletedGame(challenge.ChallengeId);
                    }
                    ChallengeManager.instance.GetChallenges();
                }
            };
                
            ChallengeTurnTakenMessage.Listener = (message) => {
                var challenge = message.Challenge;
                // replayedLastMove is a workaround to avoid having the opponents move being replayed more than once
                if (UserManager.instance.userId == challenge.NextPlayer && !replayedLastMove) {
                    // Only Replay the last move if the player is viewing the game screen for that game
                    if (challenge.ChallengeId == challengeInstanceId) {
                        replayedLastMove = true;
                        List<GSData> moveList = challenge.ScriptData.GetGSDataList("moveList");
                        int currentPlayerMove = challenge.ScriptData.GetInt("currentPlayerMove").GetValueOrDefault();
                        Player opponent = currentPlayerMove == 1 ? Player.TWO : Player.ONE;

                        ReplayIncomingOpponentMove(currentPlayerMove, moveList, opponent);
                    }
                    ChallengeManager.instance.GetChallenges();
                }
            };

            //gamePieces = new GameObject("GamePieces");
            //gamePieces.transform.parent = gameScreen.transform;

            tokens = new GameObject("Tokens");
            tokens.transform.parent = gameScreen.transform;

            UIScreen = GameObject.Find("UI Screen");

            rematchButton.gameObject.SetActive(false);
            gameScreen.SetActive(false);

            // center camera
            Camera.main.transform.position = new Vector3((Constants.numColumns-1) / 2.0f, -((Constants.numRows-1) / 2.0f)+.2f, Camera.main.transform.position.z);
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

        private void ReplayIncomingOpponentMove(int currentPlayerMove, List<GSData> moveList, Player player) {

            GSData lastMove = moveList.Last();
            int position = lastMove.GetInt("position").GetValueOrDefault();
            Direction direction = (Direction)lastMove.GetInt("direction").GetValueOrDefault();
            Move move = new Move(position, direction, player);

            StartCoroutine(MovePieceNew(move));
        }

        private void ReplayLastMove() {
            if (moveList != null) {
                GSData lastMove = moveList.Last();
                int position = lastMove.GetInt("position").GetValueOrDefault();
                Direction direction = (Direction)lastMove.GetInt("direction").GetValueOrDefault();
                //int player = lastMove.GetInt("player").GetValueOrDefault();
                Move move = new Move(position, direction);

                StartCoroutine(ProcessMove(move, true, false));
            }
        }

        public void SetupGameWrapper(List<GSData> moveList) {
            this.moveList = moveList;
            StartCoroutine(SetupGame());
        }

        public IEnumerator SetupGame() {
            UpdatePlayersStatusView();

            StartCoroutine(SetGameBoardView(gameState.GetGameBoard()));

            while (isLoading)
                yield return null;
            StartCoroutine(CreateTokenViews());
            while (isLoading)
                yield return null;
        }

        public IEnumerator SetGameBoardView(int[,] board) {
            isLoading = true;
            gameBoardView.gamePieces = new GameObject[Constants.numRows, Constants.numColumns];

            for(int row = 0; row < Constants.numRows; row++)
            {
                for(int col = 0; col < Constants.numColumns; col++)
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

        public IEnumerator CreateTokenViews() {
            isLoading = true;
            tokenViews = new List<GameObject>();

            for(int row = 0; row < Constants.numRows; row++)
            {
                for(int col = 0; col < Constants.numColumns; col++)
                {
                    Token token = gameState.tokenBoard.tokens[row, col].tokenType;
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
                        case Token.ICE_SHEET:
                            go = Instantiate(iceSheetToken, new Vector3(col, row * -1, 15), Quaternion.identity, tokens.transform);
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

        public void UpdatePlayersStatusView() {
            //Debug.Log("UpdatePlayersStatusView:isPlayerOneTurn: " + isPlayerOneTurn);
            //Debug.Log("UpdatePlayersStatusView:isCurrentPlayerTurn: " + isCurrentPlayerTurn);
            if (isMultiplayer)
            {
                if (gameState.isCurrentPlayerTurn)
                {
                    gameStatusText.text = "Your Move";
                    gameStatusText.color = gameState.isPlayerOneTurn ? bluePlayerColor : redPlayerColor;
                } else {
                    gameStatusText.text = "Their Move";
                    gameStatusText.color = gameState.isPlayerOneTurn ? bluePlayerColor : redPlayerColor;
                }
            }
            else
            {
                gameStatusText.text = gameState.isPlayerOneTurn ? bluePlayerMoveText : redPlayerMoveText;
                gameStatusText.color = gameState.isPlayerOneTurn ? bluePlayerColor : redPlayerColor;
            }
        }

        public void ResetUI() {
            if (gameState.isCurrentPlayerTurn) {
                playerNameLabel.color = gameState.isPlayerOneTurn ? bluePlayerColor : redPlayerColor;
                opponentNameLabel.color = gameState.isPlayerOneTurn ? redPlayerColor : bluePlayerColor;
                //AnimateEmptyEdgeSpots(true);
            } else {
                playerNameLabel.color = gameState.isPlayerOneTurn ? redPlayerColor : bluePlayerColor;
                opponentNameLabel.color = gameState.isPlayerOneTurn ? bluePlayerColor : redPlayerColor;
            }

            rematchButton.gameObject.SetActive(false);
        }

        public void ResetGamePiecesAndTokens() {

            isLoading = true;

            if (gamePieces.transform.childCount > 0) {
                for (int i = gamePieces.transform.childCount-1; i >= 0; i--)
                {
                    Transform piece = gamePieces.transform.GetChild(i);
                    Lean.LeanPool.Despawn(piece.gameObject);
                }
            }

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

            isLoading = false;
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

        // public void AnimateEmptyEdgeSpots(bool animate) {
        //     foreach (EmptySpot spot in gameScreen.GetComponentsInChildren<EmptySpot>())
        //     {
        //         StartCoroutine(spot.AnimateSpot(animate));
        //     }
        // }

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
            
            gamePiece.GetComponent<GamePiece>().gameManager = this;

            return gamePiece;
        }

        public void RematchPassAndPlayGame() {
            gameScreen.GetComponent<CanvasGroup>().alpha = 0.0f;
            ResetGamePiecesAndTokens();

            TokenBoard tokenBoard = TokenBoardLoader.instance.GetTokenBoard();
            gameState = new GameState(Constants.numRows, Constants.numColumns, true, true, tokenBoard, null, false);
            //PopulateMoveArrows();

            StartCoroutine(GameManager.instance.CreateTokenViews());
            UpdatePlayersStatusView();
            rematchButton.gameObject.SetActive(false);
            GameManager.instance.EnableTokenAudio();
            FadeGameScreen(1.0f, gameScreenFadeInTime);
        }

        void Update () {
            if(!isMultiplayer && gameState != null && gameState.isGameOver)
            {
                rematchButton.gameObject.SetActive(true);
            }
        }

        private void DisplayLoginError() {
            ErrorPanel.SetActive(true);
        }

        private void ProcessPlayerInput(Vector3 mousePosition) {

            if (isLoading || isCheckingForWinner || gameState.isGameOver) {
                return;
            }

            if(gameState.isCurrentPlayerTurn)
            {
                if (!isDropping) {
                    Vector3 pos = Camera.main.ScreenToWorldPoint(mousePosition);

                    // round to a grid square
                    int column = Mathf.RoundToInt(pos.x);
                    int row = Mathf.RoundToInt(pos.y * -1);
                    Position position;

                    if (inTopRowBounds (pos.x, pos.y)) {
                        position = new Position(column, row - 1);
                        //Debug.Log("Move Position: col: " + position.column + " row: " + position.row);
                        Move move = new Move(position, Direction.DOWN);
                        StartCoroutine(MovePiece(move, true));
                    } else if (inBottomRowBounds(pos.x, pos.y)) {
                        position = new Position(column, row + 1);
                        Move move = new Move(position, Direction.UP);
                        //Debug.Log("Move Position: col: " + position.column + " row: " + position.row);
                        StartCoroutine(MovePiece(move, true));
                    } else if (inRightRowBounds(pos.x, pos.y)) {
                        position = new Position(column + 1, row);
                        Move move = new Move(position, Direction.LEFT);
                        //Debug.Log("Move Position: col: " + position.column + " row: " + position.row);
                        StartCoroutine(MovePiece(move, true));
                    } else if (inLeftRowBounds(pos.x, pos.y)) {
                        position = new Position(column - 1, row);
                        Move move = new Move(position, Direction.RIGHT);
                        //Debug.Log("Move Position: col: " + position.column + " row: " + position.row);
                        StartCoroutine(MovePiece(move, true));
                    }
                }
            } else { 
                // TODO: inform the player it is not their turn
            }
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
        public IEnumerator MovePiece(Move move, bool updatePlayer)
        {
            //Debug.Log("BEGIN MOVE PIECE");
            isDropping = true;

            if (gameState.CanMove(move.GetNextPosition(), gameState.tokenBoard.tokens))
            {
                if (isMultiplayer && !isNewChallenge && !isNewRandomChallenge)
                {
                    replayedLastMove = false;
                    StartCoroutine(ProcessMove(move, false, updatePlayer));
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
                    StartCoroutine(ProcessMove(move, false, updatePlayer));
                    ChallengeManager.instance.ChallengeUser(challengedUserId, gameState, Utility.GetMoveLocation(move), move.direction);
                }
                else if (isMultiplayer && isNewRandomChallenge)
                {
                    isNewRandomChallenge = false;
                    StartCoroutine(ProcessMove(move, false, updatePlayer));
                    ChallengeManager.instance.ChallengeRandomUser(gameState, Utility.GetMoveLocation(move), move.direction);
                } else {
                    StartCoroutine(ProcessMove(move, false, updatePlayer));
                }
            } else {
                // TODO: inform the player that the move is not possible
                //gameState.gameBoard.activeMovingPieces.Clear();
                isDropping = false;
            }

            yield return 0;
        }

        private IEnumerator ProcessMove(Move move, bool replayMove, bool updatePlayer) {
            audioMove.Play();
            // GameObject[] cornerArrows = GameObject.FindGameObjectsWithTag("Arrow");
            // foreach (GameObject cornerArrow in cornerArrows) {
            //     SpriteRenderer sr = cornerArrow.GetComponentInChildren<SpriteRenderer>();
            //     sr.enabled = false;
            // }

            Fourzy.Player player = Player.NONE;
            if (replayMove && !gameState.isGameOver) {
                player = gameState.isPlayerOneTurn ? Player.TWO : Player.ONE;
            } else {
                player = gameState.isPlayerOneTurn ? Player.ONE : Player.TWO;
            }

            move.player = player;

            Debug.Log("Move position row: " + move.position.row + " column: " + move.position.column);

            gameState.PrintGameState("BeforeMove");
            gameState.MovePiece(move, replayMove);
            gameState.PrintGameState("AfterMove");

            StartCoroutine(MovePieceGameView(move, gameState.gameBoard.completedMovingPieces));

            while(updatingViews)
                yield return null;

            if (gameState.isCurrentPlayerTurn && gameState.isGameOver) {
                ChallengeManager.instance.SetViewedCompletedGame(challengeInstanceId);
            }

            if (gameState.isGameOver) {
                DisplayGameOverView();
            } else {
                if (updatePlayer) {
                    //gameState.isPlayerOneTurn = !gameState.isPlayerOneTurn;

                    if (isMultiplayer || isAiActive) {
                        gameState.isCurrentPlayerTurn = !gameState.isCurrentPlayerTurn;
                    }
                    UpdatePlayersStatusView();
                }
            }

            isDropping = false;

            if(OnMoved != null)
                OnMoved();
        }

        private IEnumerator MovePieceNew(Move move) {
            isDropping = true;

            audioMove.Play();

            gameState.MovePiece(move, false);

            StartCoroutine(MovePieceGameView(move, gameState.gameBoard.completedMovingPieces));

            while(updatingViews)
                yield return null;

            UpdateGameStatus();

            isDropping = false;
        }

        private IEnumerator MovePieceGameView(Move move, List<MovingGamePiece> completedMovingPieces) {
            updatingViews = true;

            // Create Game Piece View
            GameObject g = SpawnPiece(move.position.column, move.position.row * -1, move.player);
            GamePiece gamePiece = g.GetComponent<GamePiece>();
            gamePiece.isMoving = true;
            gamePiece.player = move.player;
            gamePiece.column = move.position.column;
            gamePiece.row = move.position.row;

            GameObject nextPieceView = new GameObject();
            // process animations for completed moving pieces
            for (int i = 0; i < completedMovingPieces.Count; i++)
            {   
                var piece = completedMovingPieces[i];
                Position startPosition = piece.positions[0];
                Position endPosition = piece.positions[piece.positions.Count - 1];
                if (i==0) {
                    piece.positions.RemoveAt(0);
                }

                GameObject pieceView;
                if (i == 0) {
                    pieceView = g;
                    nextPieceView = gameBoardView.gamePieces[endPosition.row,endPosition.column];
                } else {
                    pieceView = nextPieceView;
                    nextPieceView = gameBoardView.gamePieces[endPosition.row,endPosition.column];
                }

                pieceView.GetComponent<GamePiece>().positions = piece.positions;
                StartCoroutine(AnimatePiece(pieceView, piece.positions));

                // Update the state of the game board views
                gameBoardView.gamePieces[endPosition.row, endPosition.column] = pieceView;

                while(this.isAnimating)
                    yield return null;
            }

            gameState.ClearMovingPieces();

            gameBoardView.PrintGameBoard();

            updatingViews = false;
        }

        private void UpdateGameStatus() {
            Debug.Log("UpdateGameStatus");
            if (gameState.isCurrentPlayerTurn && gameState.isGameOver) {
                ChallengeManager.instance.SetViewedCompletedGame(challengeInstanceId);
            }

            if (gameState.isGameOver) {
                DisplayGameOverView();
            } else {
                //gameState.isPlayerOneTurn = !gameState.isPlayerOneTurn;

                if (isMultiplayer || isAiActive) {
                    gameState.isCurrentPlayerTurn = !gameState.isCurrentPlayerTurn;
                }
                UpdatePlayersStatusView();
            }
        }

        private IEnumerator AnimatePiece(GameObject g, List<Position> positions) {
            isAnimating = true;
            // Debug.Log("POSITIONS: ");
            // foreach (var item in positions)
            // {
            //     Debug.Log("row: " + item.row + ", column: " + item.column);
            // }
            //Debug.Log("AnimatePiece: ROW: " + positions[positions.Count - 1].row + "COLUMN: " + positions[positions.Count - 1].column);
            //Debug.Log("AnimatePiece: ROW: " + positions[0].row + "COLUMN: " + positions[0].column);
            
            //GameObject g = gameBoardView.gamePieces[positions[0].row, positions[0].column];
            
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
                //mySequence.Append(g.GetComponent<Rigidbody2D>().DOMove(end, dropTime, false).SetEase(Ease.Linear));

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
            GamePiece gamePiece = g.GetComponent<GamePiece>();
            gamePiece.isMoving = false;
            isAnimating = false;
        }

        public void DisplayGameOverView() {

            if (gameState.winner == Player.ONE) {
                int size = gameState.gameBoard.player1WinningPositions.Count;
                Vector3 startPos = new Vector3(gameState.gameBoard.player1WinningPositions[0].column, gameState.gameBoard.player1WinningPositions[0].row * -1, 12);
                Vector3 endPos = new Vector3(gameState.gameBoard.player1WinningPositions[size-1].column, gameState.gameBoard.player1WinningPositions[size-1].row * -1, 12);
                DrawLine(startPos, endPos, bluePlayerColor);

                if (gameState.isCurrentPlayerTurn) {
                    #if UNITY_IOS || UNITY_ANDROID
                        Handheld.Vibrate();
                    #endif
                }
            }

            if (gameState.winner == Player.TWO) {
                int size = gameState.gameBoard.player2WinningPositions.Count;
                Vector3 startPos = new Vector3(gameState.gameBoard.player2WinningPositions[0].column, gameState.gameBoard.player2WinningPositions[0].row * -1, 12);
                Vector3 endPos = new Vector3(gameState.gameBoard.player2WinningPositions[size-1].column, gameState.gameBoard.player2WinningPositions[size-1].row * -1, 12);
                DrawLine(startPos, endPos, redPlayerColor);

                if (gameState.isCurrentPlayerTurn) {
                    #if UNITY_IOS || UNITY_ANDROID
                        Handheld.Vibrate();
                    #endif
                }
            }
            WinLineSetActive(true);

            if (gameState.winner == Player.NONE || gameState.winner == Player.ALL) {
                gameStatusText.text = drawText;
            } else if (isMultiplayer) {
                if (winner != null && winner.Length > 0)
                {
                    gameStatusText.text = winner + " Won!";
                }
                else
                {
                    if (gameState.isCurrentPlayerTurn && gameState.isPlayerOneTurn && gameState.winner == Player.ONE) {
                        audioWin.Play();
                        gameStatusText.text = UserManager.instance.userName + " Won!";
                    } else {
                        gameStatusText.text = opponentNameLabel.text + " Won!";
                    }

                    gameStatusText.color = gameState.winner == Player.ONE ? bluePlayerColor : redPlayerColor;
                }
            } else {
                AnalyticsEvent.GameOver("local_game");
                audioWin.Play();
                gameStatusText.text = gameState.winner == Player.ONE ? bluePlayerWonText : redPlayerWonText;
                gameStatusText.color = gameState.winner == Player.ONE ? bluePlayerColor : redPlayerColor;
            }
        }

        void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
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

        bool inTopRowBounds(float x, float y) {
            return x > 0.5 && x < Constants.numColumns - 1.5 && y > -0.5 && y < 0.5;
        }

        bool inBottomRowBounds(float x, float y) {
            return x > 0.5 && x < Constants.numColumns - 1.5 && y > -Constants.numColumns && y < -Constants.numColumns + 1.5;
        }

        bool inLeftRowBounds(float x, float y) {
            return x > - 0.5 && x < 0.5 && y > -Constants.numColumns + 1.5 && y < -0.5;
        }

        bool inRightRowBounds(float x, float y) {
            return x > Constants.numColumns - 1.5 && x < Constants.numColumns - 0.5 && y > -Constants.numColumns + 1.5 && y < -0.5;
        }
    }
}
