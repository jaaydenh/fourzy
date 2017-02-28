using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GameSparks.Api.Requests;
using GameSparks.Api.Messages;
using GameSparks.Core;
using System.Linq;

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

		public GameObject pieceRed;
		public GameObject pieceBlue;
		public GameObject pieceEmpty;
        public GameObject cornerSpot;
        public GameObject upArrowToken;
        public GameObject downArrowToken;
        public GameObject leftArrowToken;
        public GameObject rightArrowToken;
        public GameObject stickyToken;

        public GameBoard gameBoard;
        public IToken[,] tokenBoard;

        public Button rematchButton;
        public Button nextGameButton;

		public Text gameStatusText;
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

		GameObject gamePieces;

        public GameObject gameScreenCanvas;
        private GameObject UIScreen;

        public delegate void MoveAction();
        public static event MoveAction OnMoved;

		public bool isMultiplayer = false;
        public bool isNewChallenge = false;
        public bool isNewRandomChallenge = false;
        public bool isCurrentPlayerTurn = false;
		public bool isPlayerOneTurn = true;
        public string challengedUserId;
        public int currentPlayerId;
        public string winner;
        public string opponentFacebookId;

		bool isLoading = true;
		bool isDropping = false; 
		public bool gameOver = false;
        bool isGameDrawn = false;
        bool didPlayer1Win = false;
		public bool isCheckingForWinner = false;
        public bool isAnimating = false;

        int spacing = 1; //100
        int offset = 0; //4

        public GameObject gameScreen;

        public Text playerNameLabel;
        public Image playerProfilePicture;
        public Text opponentNameLabel;
        public Image opponentProfilePicture;
        public CreateGame createGameScript;

        //int loop = 0;

        //Singleton
        private static GameManager _instance;
        public static GameManager instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<GameManager>();
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
                createGameScript.ResetFindMatchButton();
            }
        }

        private void enableGameScreen(bool enabled) {
            UserInputHandler.inputEnabled = false;
            gameScreen.SetActive(enabled);
            if (enabled)
            {
                StartCoroutine(WaitToEnableInput());
            }
        }

        IEnumerator WaitToEnableInput() {
            yield return new WaitForSeconds(2);
            UserInputHandler.inputEnabled = true;
        }

        private void OnEnable()
        {
            UserInputHandler.OnTap += ProcessPlayerInput;
            ActiveGame.OnActiveGame += enableGameScreen;
            FriendEntry.OnActiveGame += enableGameScreen;
            ChallengeManager.OnActiveGame += enableGameScreen;
        }

        private void OnDisable()
        {
            UserInputHandler.OnTap -= ProcessPlayerInput;
            ActiveGame.OnActiveGame -= enableGameScreen;
            FriendEntry.OnActiveGame -= enableGameScreen;
            ChallengeManager.OnActiveGame -= enableGameScreen;
        }

		void Start() 
		{
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
                        gameStatusText.text = challenge.Challenger.Name + " Won!";
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
                        gameStatusText.text = challenge.Challenged.First().Name + " Won!";
                    }
                    ChallengeManager.instance.GetChallenges();
                }
            };

            ChallengeTurnTakenMessage.Listener = (message) => {
                var challenge = message.Challenge;
                if (UserManager.instance.userId == challenge.NextPlayer) {
                    // Only Replay the last move if the player is viewing the game screen for that game
                    if (challenge.ChallengeId == challengeInstanceId) {
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

            InitTokenBoard();

            UIScreen = GameObject.Find("UI Screen");

            rematchButton.gameObject.SetActive(false);
            gameScreen.SetActive(false);

            // center camera
            Camera.main.transform.position = new Vector3((numColumns-1) / 2.0f, -((numRows-1) / 2.0f), Camera.main.transform.position.z);
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
            Position pos = new Position(0,0);
            switch (direction)
            {
                case Direction.UP:
                    pos.column = position;
                    pos.row = numRows;
                    break;
                case Direction.DOWN:
                    pos.column = position;
                    pos.row = -1;
                    break;
                case Direction.LEFT:
                    pos.column = numColumns;
                    pos.row = position;
                    break;
                case Direction.RIGHT:
                    pos.column = -1;
                    pos.row = position;
                    break;
                default:
                    break;
            }

            StartCoroutine(MovePiece(pos, direction, true));
        }

        public void TransitionToGamesList() {
            enableGameScreen(false);
            UIScreen.SetActive(true);
            challengeInstanceId = null;
            isCurrentPlayerTurn = false;
        }

        public void SetupGameWrapper(int[] boardData, int[] tokenData) {
            StartCoroutine(SetupGame(boardData, tokenData));
        }

        public IEnumerator SetupGame(int[] boardData, int[] tokenData) {
            StartCoroutine(SetGameBoard(boardData));
            StartCoroutine(SetTokenBoard(tokenData));
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

            UpdateGameStatusText();
        }

        public IEnumerator SetGameBoard(int[] boardData) {
            isLoading = true;

            for(int col = 0; col < numColumns; col++)
            {
                for(int row = 0; row < numRows; row++)
                {
                    int piece = boardData[col * numColumns + row];

                    if (piece == (int)Piece.BLUE)
                    {
                        GameObject pieceObject = Instantiate(pieceBlue, new Vector3(col, row * -1, 10), Quaternion.identity, gamePieces.transform);
                        SpriteRenderer pieceSprite = pieceObject.GetComponent<SpriteRenderer>();
                        //pieceSprite.sprite = new Sprite().texture.
                        GamePiece pieceModel = pieceObject.GetComponent<GamePiece>();
                        pieceModel.player = Player.ONE;
                        gameBoard.gamePieces[col, row] = pieceObject;
                    }
                    else if (piece == (int)Piece.RED)
                    {
                        GameObject pieceObject = Instantiate(pieceRed, new Vector3(col, row * -1, 10), Quaternion.identity, gamePieces.transform);
                        SpriteRenderer pieceSprite = pieceObject.GetComponent<SpriteRenderer>();
                        //pieceSprite.sprite = new Sprite().texture.
                        GamePiece pieceModel = pieceObject.GetComponent<GamePiece>();
                        pieceModel.player = Player.TWO;
                        gameBoard.gamePieces[col, row] = pieceObject;
                    }
                }
            }
                
            gameBoard.PrintGameBoard();
            isLoading = false;
            yield return 0;
        }
       
        public void InitTokenBoard() {
            tokenBoard = new IToken[numColumns, numRows];

            for(int col = 0; col < numColumns; col++)
            {
                for(int row = 0; row < numRows; row++)
                {
                    tokenBoard[col, row] = new EmptyToken();
                }
            }
        }

        public IEnumerator SetTokenBoard(int[] tokenData) {
            isLoading = true;

            for(int col = 0; col < numColumns; col++)
            {
                for(int row = 0; row < numRows; row++)
                {
                    int token = tokenData[col * numColumns + row];

                    if (token == (int)Token.UP_ARROW)
                    {
                        Instantiate(upArrowToken, new Vector3(col, row * -1, 15), Quaternion.identity, gamePieces.transform);
                        tokenBoard[col, row] = new UpArrowToken();
                        //Debug.Log("UpArrowToken");
                    }
                    else if (token == (int)Token.DOWN_ARROW)
                    {
                        Instantiate(downArrowToken, new Vector3(col, row * -1, 15), Quaternion.identity, gamePieces.transform);
                        tokenBoard[col, row] = new DownArrowToken();
                        //Debug.Log("DownArrowToken");
                    }
                    else if (token == (int)Token.LEFT_ARROW)
                    {
                        Instantiate(leftArrowToken, new Vector3(col, row * -1, 15), Quaternion.identity, gamePieces.transform);
                        tokenBoard[col, row] = new LeftArrowToken();
                    }
                    else if (token == (int)Token.RIGHT_ARROW)
                    {
                        Instantiate(rightArrowToken, new Vector3(col, row * -1, 15), Quaternion.identity, gamePieces.transform);
                        tokenBoard[col, row] = new RightArrowToken();
                    }
                    else if (token == (int)Token.STICKY)
                    {
                        Instantiate(stickyToken, new Vector3(col, row * -1, 15), Quaternion.identity, gamePieces.transform);
                        tokenBoard[col, row] = new StickyToken();
                    }
                }
            }

            isLoading = false;
            yield return 0;
        }

//        public void SetSampleTokenBoard()
//        {
//            tokenBoard[2, 2] = new RightArrowToken();
//            tokenBoard[5, 2] = new DownArrowToken();
//            tokenBoard[5, 5] = new LeftArrowToken();
//            tokenBoard[1, 5] = new UpArrowToken();
//            tokenBoard[1, 1] = new RightArrowToken();
//            tokenBoard[6, 1] = new DownArrowToken();
//            tokenBoard[6, 6] = new LeftArrowToken();
//
//            Instantiate(rightArrowToken, new Vector3(2, 2 * -1, 8), Quaternion.identity, gamePieces.transform);
//            Instantiate(downArrowToken, new Vector3(5, 2 * -1, 8), Quaternion.identity, gamePieces.transform);
//            Instantiate(leftArrowToken, new Vector3(5, 5 * -1, 8), Quaternion.identity, gamePieces.transform);
//            Instantiate(upArrowToken, new Vector3(1, 5 * -1, 8), Quaternion.identity, gamePieces.transform);
//            Instantiate(rightArrowToken, new Vector3(1, 1 * -1, 8), Quaternion.identity, gamePieces.transform);
//            Instantiate(downArrowToken, new Vector3(6, 1 * -1, 8), Quaternion.identity, gamePieces.transform);
//            Instantiate(leftArrowToken, new Vector3(6, 6 * -1, 8), Quaternion.identity, gamePieces.transform);
//        }

        public void UpdateGameStatusText() {
//            print("isMultiplayer: " + isMultiplayer);
//            print("gameOver: " + gameOver);
//            print("isCurrentPlayerTurn: " + isCurrentPlayerTurn);
//            print("isPlayerOneTurn: " + isPlayerOneTurn);
//            print("didPlayer1Win: " + didPlayer1Win);

            if (gameOver == true)
            {
                if (isGameDrawn) {
                    gameStatusText.text = drawText;
                } else if (isMultiplayer) {
                    if (winner != null)
                    {
                        gameStatusText.text = winner + " Won!";
                    }
                    else
                    {
                        if (isPlayerOneTurn && didPlayer1Win)
                        {
                            gameStatusText.text = UserManager.instance.userName + " Won!";
                        }
                        if (isPlayerOneTurn && !didPlayer1Win)
                        {
                            gameStatusText.text = UserManager.instance.userName + " Lost!";
                        }
                        if (!isPlayerOneTurn && !didPlayer1Win)
                        {
                            gameStatusText.text = UserManager.instance.userName + " Won!";
                        }
                        if (!isPlayerOneTurn && didPlayer1Win)
                        {
                            gameStatusText.text = UserManager.instance.userName + " Lost!";
                        }
                        gameStatusText.color = didPlayer1Win ? bluePlayerColor : redPlayerColor;
                    }
                } else {
                    gameStatusText.text = didPlayer1Win ? bluePlayerWonText : redPlayerWonText;
                    gameStatusText.color = didPlayer1Win ? bluePlayerColor : redPlayerColor;
                }
            } else {
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
                } else {
                    gameStatusText.text = isPlayerOneTurn ? bluePlayerMoveText : redPlayerMoveText;
                    gameStatusText.color = isPlayerOneTurn ? bluePlayerColor : redPlayerColor;
                }
            }
        }

        private List<long> GetTokenBoardData() {
            List<long> tokenBoardList = new List<long>();
            for(int col = 0; col < numColumns; col++)
            {
                for(int row = 0; row < numRows; row++)
                {
                    tokenBoardList.Add((int)tokenBoard[col, row].tokenType);
                }
            }
            return tokenBoardList;
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

            if(gamePieces != null)
			{
                DestroyImmediate(gamePieces);
			}
            gamePieces = new GameObject("GamePieces");
            gamePieces.transform.parent = gameScreenCanvas.transform;
            gamePieces.transform.localPosition = new Vector3(-375f, -501f);

            for (int col = 0; col < numColumns; col++)
            {
                for (int row = 0; row < numRows; row++)
                {
                    tokenBoard[col, row] = new EmptyToken();
                }
            }

			isLoading = false;
			gameOver = false;
		}

        public void PopulateEmptySpots() {
            for(int col = 0; col < numColumns; col++)
            {
                for(int row = 0; row < numRows; row++)
                {
                    GameObject g;
                    if (col + row == 0)
                    {
                        g = Instantiate(pieceEmpty, new Vector3((col - offset) * spacing, ((row - offset) * spacing) * -1, 20), Quaternion.identity) as GameObject;
                        g.transform.parent = gamePieces.transform;
                        CornerSpot spot = g.GetComponent<CornerSpot>();
                        spot.downArrowActive = true;
                        spot.downArrowActive = true;
                        spot.rightArrowActive = true;
                        spot.row = 0;
                        spot.column = 0;
                    }
                    else if (col == 0 && row == numRows - 1)
                    {
                        g = Instantiate(pieceEmpty, new Vector3((col - offset) * spacing, ((row - offset) * spacing) * -1, 20), Quaternion.identity) as GameObject;
                        g.transform.parent = gamePieces.transform;
                        CornerSpot spot = g.GetComponent<CornerSpot>();
                        spot.rightArrowActive = true;
                        spot.upArrowActive = true;
                        spot.row = numRows - 1;
                        spot.column = 0;
                    }
                    else if (col * row == (numColumns - 1) * (numRows - 1))
                    { 
                        g = Instantiate(pieceEmpty, new Vector3((col - offset) * spacing, ((row - offset) * spacing) * -1, 20), Quaternion.identity) as GameObject;
                        g.transform.parent = gamePieces.transform;
                        CornerSpot spot = g.GetComponent<CornerSpot>();
                        spot.upArrowActive = true;
                        spot.leftArrowActive = true;
                        spot.row = numRows - 1;
                        spot.column = numColumns - 1;
                    }
                    else if (col == numColumns - 1 && row == 0)
                    {
                        g = Instantiate(pieceEmpty, new Vector3((col - offset) * spacing, ((row - offset) * spacing) * -1, 20), Quaternion.identity) as GameObject;
                        g.transform.parent = gamePieces.transform;
                        CornerSpot spot = g.GetComponent<CornerSpot>();
                        spot.downArrowActive = true;
                        spot.leftArrowActive = true;
                        spot.row = 0;
                        spot.column = numColumns - 1;
                    }
                    else
                    {
                        g = Instantiate(pieceEmpty, new Vector3((col - offset) * spacing, ((row - offset) * spacing) * -1, 20), Quaternion.identity) as GameObject;
                        g.transform.parent = gamePieces.transform;
                    }

                    EmptySpot emptySpot = g.GetComponent<EmptySpot>();

                    if (gameBoard.PlayerAtPosition(new Position(col, row)) != Player.NONE) {
                        emptySpot.hasPiece = true;
                    }
                    if (col == 0 || row == 0 || col == numColumns - 1 || row == numRows - 1) {
                        emptySpot.isEdgeSpot = true;
                    }
                }
            }
        }

        public void AnimateEmptyEdgeSpots(bool animate) {
            foreach (EmptySpot spot in gamePieces.GetComponentsInChildren<EmptySpot>())
            {
                StartCoroutine(spot.AnimateSpot(animate));
            }
        } 

		/// <summary>
		/// Spawns a gamepiece at the given column and row
		/// </summary>
		/// <returns>The piece.</returns>
        GameObject SpawnPiece(float posX, float posY)
		{          
			GameObject gamePiece = Instantiate(
				isPlayerOneTurn ? pieceBlue : pieceRed,
				new Vector3(Mathf.FloorToInt(posX + 0.5f), 
					Mathf.FloorToInt(posY + 0.5f), 10), // spawn it above the first row
                Quaternion.identity, gamePieces.transform) as GameObject;

            return gamePiece;
		}

        public void RematchPassAndPlayGame() {
            ResetGameBoard();
            PopulateEmptySpots();
            int[] tokenData = TokenBoard.Instance.FindTokenBoardAll();
            StartCoroutine(GameManager.instance.SetTokenBoard(tokenData));
            gameOver = false;
            UpdateGameStatusText();
            rematchButton.gameObject.SetActive(false);
        }
            
		void Update () {
            if(!isMultiplayer && gameOver)
			{
                rematchButton.gameObject.SetActive(true);
			}
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
                        //StartCoroutine(MovePiece(column, Direction.Down, false));
                        position = new Position(column, row - 1);
                        Debug.Log("Move Position: col: " + position.column + " row: " + position.row);
                        StartCoroutine(MovePiece(position, Direction.DOWN, false));
                    } else if (inBottomRowBounds(pos.x, pos.y)) {
                        //StartCoroutine(MovePiece(column, Direction.Up, false));
                        position = new Position(column, row + 1);
                        Debug.Log("Move Position: col: " + position.column + " row: " + position.row);
                        StartCoroutine(MovePiece(position, Direction.UP, false));
                    } else if (inRightRowBounds(pos.x, pos.y)) {
                        //StartCoroutine(MovePiece(row, Direction.Left, false));
                        position = new Position(column + 1, row);
                        Debug.Log("Move Position: col: " + position.column + " row: " + position.row);
                        StartCoroutine(MovePiece(position, Direction.LEFT, false));
                    } else if (inLeftRowBounds(pos.x, pos.y)) {
                        //StartCoroutine(MovePiece(row, Direction.Right, false));
                        position = new Position(column - 1, row);
                        Debug.Log("Move Position: col: " + position.column + " row: " + position.row);
                        StartCoroutine(MovePiece(position, Direction.RIGHT, false));
                    }
                }
            } else { 
                // TODO: inform the player it is not their turn
            }
        }

        public int GetMoveLocation(Position position, Direction direction) {
            int movePosition = -1;
            Debug.Log("direction: " + direction.ToString());
            Debug.Log("columm: " + position.column + " row: " + position.row);
            switch (direction)
            {
                case Direction.UP:
                    movePosition = position.column;
                    break;
                case Direction.DOWN:
                    movePosition = position.column;
                    break;
                case Direction.LEFT:
                    movePosition = position.row;
                    break;
                case Direction.RIGHT:
                    movePosition = position.row;
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
        public IEnumerator MovePiece(Position position, Direction direction, bool replayMove)
        {
            isDropping = true;

            MovingGamePiece activeMovingPiece = new MovingGamePiece(position, direction);
            gameBoard.activeMovingPieces.Add(activeMovingPiece);
            if (CanMoveInPosition(position, activeMovingPiece.GetNextPosition(), direction))
            {
                GameObject[] cornerArrows = GameObject.FindGameObjectsWithTag("Arrow");

                foreach (GameObject cornerArrow in cornerArrows) {
                    SpriteRenderer sr = cornerArrow.GetComponentInChildren<SpriteRenderer>();
                    sr.enabled = false;
                }

                Position movePosition = activeMovingPiece.GetNextPosition();
                GameObject g = SpawnPiece(movePosition.column, movePosition.row * -1);
                GamePiece gamePiece = g.GetComponent<GamePiece>();
                gamePiece.player = isPlayerOneTurn ? Player.ONE : Player.TWO;
                gamePiece.column = movePosition.column;
                gamePiece.row = movePosition.row;

                gameBoard.gamePieces[movePosition.column, movePosition.row] = g;
                gameBoard = tokenBoard[movePosition.column, movePosition.row].UpdateBoard(gameBoard, false);    

                while (gameBoard.activeMovingPieces.Count > 0) {
                    //loop = 0;
                    Debug.Log("Checking if a move is possible");
                    Position startPosition = gameBoard.activeMovingPieces[0].GetCurrentPosition();
                    Position endPosition = gameBoard.activeMovingPieces[0].GetNextPosition();

                    if (CanMoveInPosition(startPosition, endPosition, direction)) {
                        //Debug.Log("CanMoveInPosition: direction: " + direction.ToString());
                        gameBoard = tokenBoard[endPosition.column, endPosition.row].UpdateBoard(gameBoard, true);    
                    } else {
                        //Debug.Log("Disable next moving piece");
                        gameBoard.DisableNextMovingPiece();
                    }
                }

                UpdateMoveablePieces();

                //Debug.Log("replayMove: " + replayMove + " ismultiplayer: " + isMultiplayer + " isNewChallenge: " + isNewChallenge + " isNewRandomChallenge: " + isNewRandomChallenge);

                if (!replayMove && isMultiplayer && !isNewChallenge && !isNewRandomChallenge)
                {
                    new LogChallengeEventRequest().SetChallengeInstanceId(challengeInstanceId)
                        .SetEventKey("takeTurn") //The event we are calling is "takeTurn", we set this up on the GameSparks Portal
                        .SetEventAttribute("pos", GetMoveLocation(position, direction)) // pos is the row or column the piece was placed at depending on the direction
                        .SetEventAttribute("direction", direction.GetHashCode()) // direction can be up, down, left, right
                        .SetEventAttribute("player", isPlayerOneTurn ? (int)Piece.BLUE : (int)Piece.RED)
                        .Send((response) =>
                            {
                                if (response.HasErrors)
                                {
                                    Debug.Log("ChallengeEventRequest was not successful");
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
                    ChallengeManager.instance.ChallengeUser(challengedUserId, gameBoard.GetGameBoardData(), GetTokenBoardData(), GetMoveLocation(position, direction), direction);
                }
                else if (isMultiplayer && isNewRandomChallenge)
                {
                    isNewRandomChallenge = false;
                    ChallengeManager.instance.ChallengeRandomUser(gameBoard.GetGameBoardData(), GetTokenBoardData(), GetMoveLocation(position, direction), direction);
                }

                // process animations for completed moving pieces
                foreach (var piece in gameBoard.completedMovingPieces)
                {
                    StartCoroutine(AnimatePiece(piece.positions));
                    while(this.isAnimating)
                        yield return null;
                }

                gameBoard.completedMovingPieces.Clear();

                //gameBoard.PrintGameBoard();

                // Check if Player one is the winner
                StartCoroutine(CheckForWinner(true));
                // Check if Player two is the winner
                StartCoroutine(CheckForWinner(false));

                // wait until winning check is done
                while(this.isCheckingForWinner)
                    yield return null;

                isPlayerOneTurn = !isPlayerOneTurn;
                if (isMultiplayer) {
                    isCurrentPlayerTurn = !isCurrentPlayerTurn;    
                }
                UpdateGameStatusText();
                AnimateEmptyEdgeSpots(false);

                foreach (GameObject cornerArrow in cornerArrows) {
                    cornerArrow.SetActive(false);
                    SpriteRenderer sr = cornerArrow.GetComponentInChildren<SpriteRenderer>();
                    sr.enabled = true;
                }

                isDropping = false;

                if(OnMoved != null)
                    OnMoved();
            } else {
                gameBoard.activeMovingPieces.Clear();
                isDropping = false;
            }

            yield return 0;
        }

        private void UpdateMoveablePieces() {
            foreach (var piece in gameBoard.completedMovingPieces)
            {
                Position currentPosition = piece.GetCurrentPosition();
                if (tokenBoard[currentPosition.column, currentPosition.row].tokenType == Token.STICKY) {
                    if (CanMoveInPosition(piece.GetCurrentPosition(), piece.GetNextPositionWithDirection(Direction.UP), Direction.UP)) {
                        gameBoard.MakePieceMoveable(currentPosition, true, Direction.UP);
                    } else {
                        gameBoard.MakePieceMoveable(currentPosition, false, Direction.UP);
                    } 
                    if (CanMoveInPosition(piece.GetCurrentPosition(), piece.GetNextPositionWithDirection(Direction.DOWN), Direction.DOWN)) {
                        gameBoard.MakePieceMoveable(currentPosition, true, Direction.DOWN);
                    } else {
                        gameBoard.MakePieceMoveable(currentPosition, false, Direction.DOWN);
                    }
                    if (CanMoveInPosition(piece.GetCurrentPosition(), piece.GetNextPositionWithDirection(Direction.LEFT), Direction.LEFT)) {
                        gameBoard.MakePieceMoveable(currentPosition, true, Direction.LEFT);
                    } else {
                        gameBoard.MakePieceMoveable(currentPosition, false, Direction.LEFT);
                    }
                    if (CanMoveInPosition(piece.GetCurrentPosition(), piece.GetNextPositionWithDirection(Direction.RIGHT), Direction.RIGHT)) {
                        gameBoard.MakePieceMoveable(currentPosition, true, Direction.RIGHT);
                    } else {
                        gameBoard.MakePieceMoveable(currentPosition, false, Direction.RIGHT);
                    }
                } else {
                    gameBoard.MakePieceMoveable(currentPosition, false, Direction.UP);
                    gameBoard.MakePieceMoveable(currentPosition, false, Direction.DOWN);
                    gameBoard.MakePieceMoveable(currentPosition, false, Direction.LEFT);
                    gameBoard.MakePieceMoveable(currentPosition, false, Direction.RIGHT);
                }
            }
        }

        private IEnumerator AnimatePiece(List<Position> positions) {
            isAnimating = true;
            GameObject g = gameBoard.gamePieces[positions[positions.Count - 1].column, positions[positions.Count - 1].row];

            Vector3 start = new Vector3(positions[1].column, positions[1].row * -1);
            for (int i = 1; i < positions.Count; i++)
            {
                Vector3 end = new Vector3(positions[i].column, positions[i].row * -1);
                float distance = Vector3.Distance(start, end);

                float t = 0;
                while(t < 1)
                {
                    t += Time.deltaTime * dropTime;
                    if (numRows - distance > 0) {
                        t += (numRows - distance) / 500;
                    }
                    g.transform.position = Vector3.Lerp (start, end, t);
                    yield return null;
                }

                start = end;
            }
            isAnimating = false;
        }

        public bool CanMoveInPosition(Position startPosition, Position endPosition, Direction direction)
        {
//            loop++;
//            if (loop> 10) {
//                return false;
//            }
            //Debug.Log("startPosition:col: " + startPosition.column + " row: " + startPosition.row);
            //Debug.Log("endPosition:col: " + endPosition.column + " row: " + endPosition.row);
            // if the next end position is outside of the board then return false;
            if (endPosition.column >= numColumns || endPosition.column < 0 || endPosition.row >= numRows || endPosition.row < 0) {
                Debug.Log("OUTSIDE OF BOARD");
                return false;
            }

            // check for piece at end position if there is a piece and the piece is not moveable then return false
            if (gameBoard.gamePieces[endPosition.column, endPosition.row]) {
                GameObject pieceObject = gameBoard.gamePieces[endPosition.column, endPosition.row];
                GamePiece gamePiece = pieceObject.GetComponent<GamePiece>();

                switch (direction)
                {
                    case Direction.UP:
                        if (!gamePiece.isMoveableUp) {
                            return false;
                        }
                        break;
                    case Direction.DOWN:
                        if (!gamePiece.isMoveableDown) {
                            return false;
                        }
                        break;
                    case Direction.LEFT:
                        if (!gamePiece.isMoveableLeft) {
                            return false;
                        }
                        break;
                    case Direction.RIGHT:
                        if (!gamePiece.isMoveableRight) {
                            return false;
                        }
                        break;
                    default:
                        break;
                }
                Debug.Log("IsMoveAble");
                Debug.Log("endPosition:col: " + endPosition.column + " row: " + endPosition.row);
                Debug.Log("next Position:col: " + gamePiece.GetNextPosition(direction).column + " row: " + gamePiece.GetNextPosition(direction).row);
                return CanMoveInPosition(endPosition, gamePiece.GetNextPosition(direction), direction);
            }
                
            // if there is a token at the end position and canPassThrough is true then true
            if (!tokenBoard[endPosition.column, endPosition.row].canPassThrough) {
                Debug.Log("CANT PASS THROUGH");
                return false;
            }

            return true;
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
                        DrawLine(hitsHorz[0].transform.position, hitsHorz[3].transform.position, playerOne ? bluePlayerColor : redPlayerColor);
                        if (isCurrentPlayerTurn) {
                            Handheld.Vibrate();    
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
                        DrawLine(hitsVert[0].transform.position, hitsVert[3].transform.position, playerOne ? bluePlayerColor : redPlayerColor);
                        if (isCurrentPlayerTurn) {
                            Handheld.Vibrate();    
                        }
                        gameOver = true;
                        didPlayer1Win = playerOne;
						break;
					}

					// test diagonally
					if(allowDiagonally)
					{
						// calculate the length of the ray to shoot diagonally
						float length = Vector2.Distance(new Vector2(0, 0), new Vector2(numPiecesToWin - 1, numPiecesToWin - 1));

						RaycastHit2D[] hitsDiaLeft = Physics2D.RaycastAll(
							new Vector3(x, y * -1),
							new Vector3(-1 , 1),
							length,
							layermask);
						if(hitsDiaLeft.Length == numPiecesToWin)
						{
                            DrawLine(hitsDiaLeft[0].transform.position, hitsDiaLeft[3].transform.position, playerOne ? bluePlayerColor : redPlayerColor);
                            if (isCurrentPlayerTurn) {
                                Handheld.Vibrate();    
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
                            DrawLine(hitsDiaRight[0].transform.position, hitsDiaRight[3].transform.position, playerOne ? bluePlayerColor : redPlayerColor);
                            if (isCurrentPlayerTurn) {
                                Handheld.Vibrate();    
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

            if (!FieldContainsEmptyCell())
            {
                gameOver = true;
                isGameDrawn = true;
            }
                
			this.isCheckingForWinner = false;

			yield return 0;
		}

        void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
        {
            //Debug.Log("DRAWLINE");
            GameObject myLine = new GameObject();
            myLine.transform.parent = gamePieces.transform;
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

            //GameObject.Destroy(myLine, duration);
        }

		/// <summary>
		/// check if the field contains an empty cell
		/// </summary>
		/// <returns><c>true</c>, if it contains empty cell, <c>false</c> otherwise.</returns>
		bool FieldContainsEmptyCell()
		{
			for(int col = 0; col < numColumns; col++)
			{
				for(int row = 0; row < numRows; row++)
				{
//					if(gameBoard[x, y] == (int)Piece.EMPTY)
//						return true;
                    if(!gameBoard.gamePieces[col, row]) {
                        return true;
                    }
				}
			}
			return false;
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
