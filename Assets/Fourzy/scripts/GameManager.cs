﻿using UnityEngine;
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
		enum Piece
		{
			Empty = 0,
			Blue = 1,
			Red = 2
		}

		public enum Direction {Up, Down, Left, Right};

		[Range(3, 8)]
		public int numRows = 8;
		[Range(3, 8)]
		public int numColumns = 8;

		[Tooltip("How many pieces have to be connected to win.")]
		public int numPiecesToWin = 4;

		[Tooltip("Allow diagonally connected Pieces?")]
		public bool allowDiagonally = true;
		
		public float dropTime = 1.0f;

		// GameSparks
        public string challengeInstanceId;

		// Gameobjects 
		public GameObject pieceRed;
		public GameObject pieceBlue;
		public GameObject pieceEmpty;
        public GameObject cornerSpot;
        public Button rematchButton;

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

		/// <summary>
		/// The Gameboard.
		/// 0 = Empty
		/// 1 = Blue
		/// 2 = Red
		/// </summary>
		public int[,] gameBoard;

		public bool isMultiplayer = false;
        public bool isNewChallenge = false;
        public bool isCurrentPlayerTurn = true;
		public bool isPlayerOneTurn = true;
        public string challengedUserId;
        public int currentPlayerId;

		bool isLoading = true;
		bool isDropping = false; 
		bool gameOver = false;
		bool isCheckingForWinner = false;

        int spacing = 1; //100
        int offset = 0; //4

        private GameObject gameScreen;

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

        private void enableGameScreen(bool enabled) {
            UserInputHandler.inputEnabled = false;
            Debug.Log("UserInputHandler.inputEnabled = false;");
            gameScreen.SetActive(enabled);
            if (enabled)
            {
                StartCoroutine(WaitToEnableInput());
            }
        }

        IEnumerator WaitToEnableInput() {
            yield return new WaitForSeconds(2);
            Debug.Log("UserInputHandler.inputEnabled = true;");
            UserInputHandler.inputEnabled = true;
        }

        private void OnEnable()
        {
            UserInputHandler.OnTap += processPlayerInput;
            ActiveGame.OnActiveGame += enableGameScreen;
            FriendEntry.OnActiveGame += enableGameScreen;
        }

        private void OnDisable()
        {
            UserInputHandler.OnTap -= processPlayerInput;
            ActiveGame.OnActiveGame -= enableGameScreen;
            FriendEntry.OnActiveGame -= enableGameScreen;
        }

		void Start() 
		{
//            ChallengeStartedMessage.Listener = (message) => {
//                var challenge = message.Challenge; 
//                string messageId = message.MessageId; 
//                bool? notification = message.Notification; 
//                GSData scriptData = message.ScriptData; 
//                string subTitle = message.SubTitle; 
//                string summary = message.Summary;
//                string title = message.Title;
//
//                Debug.Log(message);
//                Debug.Log("ChallengeStartedMessage: " + message.Title);
//            };
                
            ChallengeTurnTakenMessage.Listener = (message) => {
                var challenge = message.Challenge;
                var nextPlayerId = challenge.NextPlayer;

                Debug.Log("message: " + message.JSONString.ToString());
//                List<int> boardDataInt = challenge.ScriptData.GetIntList("gameBoard");
//
//                foreach (int x in boardDataInt) {
//                    Debug.Log("IntPiece: " + x);
//                }

                //string messageId = message.MessageId;
                //bool? notification = message.Notification;
                //GSData scriptData = message.ScriptData;

                Debug.Log("UserId: " + UserManager.instance.userId);
                Debug.Log("Message nextplayerId: " + nextPlayerId);
                if (UserManager.instance.userId == nextPlayerId) { 
                    List<GSData> moveList = challenge.ScriptData.GetGSDataList("moveList");

                    foreach (GSData x in moveList) {
                        Debug.Log("position: " + x.GetInt("position"));
                        Debug.Log("direction: " + x.GetInt("direction"));
                        Debug.Log("player: " + x.GetInt("player"));
                    }
                    GSData lastMove = moveList.Last();
                    int position = lastMove.GetInt("position").GetValueOrDefault();
                    int directionInt = lastMove.GetInt("direction").GetValueOrDefault();
                    int player = lastMove.GetInt("player").GetValueOrDefault();
                    if (player == (int)Piece.Blue) {
                        isPlayerOneTurn = true;
                    } else if (player == (int)Piece.Red) {
                        isPlayerOneTurn = false;
                    }
                    Direction direction = (Direction)directionInt;
                    StartCoroutine(movePiece(position, direction, true));
                }
            };

            int max = Mathf.Max(numRows, numColumns);

			if(numPiecesToWin > max)
				numPiecesToWin = max;

			//CreateGameBoard();

            UIScreen = GameObject.Find("UI Screen");

            if (!isMultiplayer)
            {   
                gameStatusText.text = isPlayerOneTurn ? bluePlayerMoveText : redPlayerMoveText;
            }

            rematchButton.gameObject.SetActive(false);

            gameScreen = GameObject.Find("Game Screen");
            gameScreen.SetActive(false);
		}

        public void TransitionToGamesList() {
            enableGameScreen(false);
            UIScreen.SetActive(true);
            //ChallengeManager.instance.GetActiveChallenges();
        }

        public void SetGameBoard(int[] boardData) {
            isLoading = true;

            for(int x = 0; x < numColumns; x++)
            {
                for(int y = 0; y < numRows; y++)
                {
                    int piece = boardData[x * numColumns + y];
                    gameBoard[x, y] = piece;
                    if (piece == (int)Piece.Blue)
                    {
                        GameObject g = Instantiate(pieceBlue, new Vector3(x, y * -1, 10), Quaternion.identity) as GameObject;
                        g.transform.parent = gamePieces.transform;
                    }
                    else if (piece == (int)Piece.Red)
                    {
                        GameObject g = Instantiate(pieceRed, new Vector3(x, y * -1, 10), Quaternion.identity) as GameObject;
                        g.transform.parent = gamePieces.transform;
                    }
                }
            }

            isLoading = false;
        }
       
        public void SetMultiplayerGameStatusText() {
            if (isCurrentPlayerTurn)
            {
                gameStatusText.text = "Your Move";
            }
            else
            {
                gameStatusText.text = "Their Move";
            }
        }

        private List<long> GetGameBoard() {
//            List<GSData> gameBoardList = new List<GSData>();
            List<long> gameBoardList = new List<long>();
            for(int col = 0; col < numColumns; col++)
            {
                for(int row = 0; row < numRows; row++)
                {
                    //GSData TEST = new GSData(gameBoard[col, row].ToString());
                    //gameBoardList.Add(gameBoard[col, row]);
                    gameBoardList.Add(gameBoard[col, row]);
                }
            }
            return gameBoardList;
        }

        public void CreateGameBoard() {

			isLoading = true;

            if(gamePieces != null)
			{
                DestroyImmediate(gamePieces);
			}
            gamePieces = new GameObject("GamePieces");
            gamePieces.transform.parent = gameScreenCanvas.transform;

			// create an empty gameboard and instantiate the cells
			gameBoard = new int[numColumns, numRows];
			for(int col = 0; col < numColumns; col++)
			{
				for(int row = 0; row < numRows; row++)
				{
					gameBoard[col, row] = (int)Piece.Empty;
                    if (col + row == 0)
                    {
                        GameObject g = Instantiate(cornerSpot, new Vector3((col - offset) * spacing, ((row - offset) * spacing) * -1, 20), Quaternion.identity) as GameObject;
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
                        GameObject g = Instantiate(cornerSpot, new Vector3((col - offset) * spacing, ((row - offset) * spacing) * -1, 20), Quaternion.identity) as GameObject;
                        g.transform.parent = gamePieces.transform;
                        CornerSpot spot = g.GetComponent<CornerSpot>();
                        spot.rightArrowActive = true;
                        spot.upArrowActive = true;
                        spot.row = numRows - 1;
                        spot.column = 0;
                    }
                    else if (col * row == (numColumns - 1) * (numRows - 1))
                    { 
                        GameObject g = Instantiate(cornerSpot, new Vector3((col - offset) * spacing, ((row - offset) * spacing) * -1, 20), Quaternion.identity) as GameObject;
                        g.transform.parent = gamePieces.transform;
                        CornerSpot spot = g.GetComponent<CornerSpot>();
                        spot.upArrowActive = true;
                        spot.leftArrowActive = true;
                        spot.row = numRows - 1;
                        spot.column = numColumns - 1;
                    }
                    else if (col == numColumns - 1 && row == 0)
                    {
                        GameObject g = Instantiate(cornerSpot, new Vector3((col - offset) * spacing, ((row - offset) * spacing) * -1, 20), Quaternion.identity) as GameObject;
                        g.transform.parent = gamePieces.transform;
                        CornerSpot spot = g.GetComponent<CornerSpot>();
                        spot.downArrowActive = true;
                        spot.leftArrowActive = true;
                        spot.row = 0;
                        spot.column = numColumns - 1;
                    }
                    else
                    {   
                        GameObject g = Instantiate(pieceEmpty, new Vector3((col - offset) * spacing, ((row - offset) * spacing) * -1, 20), Quaternion.identity) as GameObject;
                        g.transform.parent = gamePieces.transform;
                    }
				}
			}

			isLoading = false;
			gameOver = false;

			// center camera 
			Camera.main.transform.position = new Vector3((numColumns-1) / 2.0f, -((numRows-1) / 2.0f), Camera.main.transform.position.z);
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
					Quaternion.identity) as GameObject;

            return gamePiece;
		}

        public void RematchGame() {
            CreateGameBoard();
            gameStatusText.text = isPlayerOneTurn ? bluePlayerMoveText : redPlayerMoveText;
            gameStatusText.color = isPlayerOneTurn ? bluePlayerColor : redPlayerColor;
            rematchButton.gameObject.SetActive(false);
        }
            
		void Update () {

			if(gameOver)
			{
                rematchButton.gameObject.SetActive(true);
			}
		}

        private void processPlayerInput(Vector3 mousePosition) {

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
                    Debug.Log("column: " + column);
                    Debug.Log("row: " + row);
                    if (inTopRowBounds (pos.x, pos.y)) {
                        StartCoroutine(movePiece(column, Direction.Down, false));
                    } else if (inBottomRowBounds(pos.x, pos.y)) {
                        StartCoroutine(movePiece(column, Direction.Up, false));
                    } else if (inRightRowBounds(pos.x, pos.y)) {
                        StartCoroutine(movePiece(row, Direction.Left, false));
                    } else if (inLeftRowBounds(pos.x, pos.y)) {
                        StartCoroutine(movePiece(row, Direction.Right, false));
                    }
                }
            } else { 
                // TODO: inform the player it is not their turn
            }
        }

		/// <summary>
		/// This method searches for a row or column with an empty spot 
		/// </summary>
        /// <param name="posX">x position</param>
        /// <param name="posY">y position</param>
        /// <param name="direction">direction</param>
        public IEnumerator movePiece(int position, Direction direction, bool replayMove)
		{
			isDropping = true;
            int movePosition = -1;
            int row = -1, column = -1;

            Vector3 endPosition = new Vector3();

			bool foundFreeSpot = false;
			if (direction == Direction.Down) {
                int nextRow = nextEmptySpotInColumnDown(position);
				if (nextRow != -1) {
                    foundFreeSpot = true;
                    movePosition = position;
                    gameBoard [position, nextRow] = isPlayerOneTurn ? (int)Piece.Blue : (int)Piece.Red;
                    endPosition = new Vector3(position, nextRow * -1, 10);
                    row = 0;
                    column = position;
				}
			} else if (direction == Direction.Up) {
                int nextRow = nextEmptySpotInColumnUp(position);
				if (nextRow != -1) {
                    foundFreeSpot = true;
                    movePosition = position;
                    gameBoard [position, nextRow] = isPlayerOneTurn ? (int)Piece.Blue : (int)Piece.Red;
                    endPosition = new Vector3(position, nextRow * -1, 10);
                    row = numRows - 1;
                    column = position;
				}
			} else if (direction == Direction.Right) {
                int nextColumn = nextEmptySpotInRowRight(position);
				if (nextColumn != -1) {
                    foundFreeSpot = true;
                    movePosition = position;
                    gameBoard [nextColumn, position] = isPlayerOneTurn ? (int)Piece.Blue : (int)Piece.Red;
                    endPosition = new Vector3(nextColumn, position * -1, 10);
                    row = position;
                    column = 0;
				}
			} else if (direction == Direction.Left) {
                int nextColumn = nextEmptySpotInRowLeft(position);
				if (nextColumn != -1) {
                    foundFreeSpot = true;
                    movePosition = position;
                    gameBoard [nextColumn, position] = isPlayerOneTurn ? (int)Piece.Blue : (int)Piece.Red;
                    endPosition = new Vector3(nextColumn, position * -1, 10);
                    row = position;
                    column = numColumns - 1;
				}
			}

            Vector3 startPosition = new Vector3(column, row * -1, 10);

            GameObject[] gos;
            gos = GameObject.FindGameObjectsWithTag("Arrow");

            foreach (GameObject go in gos) {
                SpriteRenderer sr = go.GetComponentInChildren<SpriteRenderer>();
                sr.enabled = false;
            }
            Debug.Log("foundFreeSpot: " + foundFreeSpot);
            if(foundFreeSpot)
			{
//                Debug.Log("Direction: " + direction.GetHashCode());
//                Debug.Log("Direction: " + direction);
//                Debug.Log("Move Position: " + movePosition);
//                Debug.Log("challengeInstanceId: " + challengeInstanceId);
                if (!replayMove && isMultiplayer && !isNewChallenge)
                {
                    new LogChallengeEventRequest().SetChallengeInstanceId(challengeInstanceId)
                    .SetEventKey("takeTurn") //The event we are calling is "takeTurn", we set this up on the GameSparks Portal
                    .SetEventAttribute("pos", movePosition) // pos is the row or column the piece was placed at depending on the direction
                    .SetEventAttribute("direction", direction.GetHashCode()) // direction can be up, down, left, right
                    .SetEventAttribute("player", isPlayerOneTurn ? (int)Piece.Blue : (int)Piece.Red)
                    .Send((response) =>
                        {
                            if (response.HasErrors)
                            {

                            }
                            else
                            {
                                // If our ChallengeEventRequest was successful we inform the player
                                Debug.Log("ChallengeEventRequest was successful");
                            }
                        });
                }
                else if (isMultiplayer && isNewChallenge) {
                    Debug.Log("challenge user");
                    //Debug.Log(GetGameBoard());
                    ChallengeManager.instance.ChallengeUser(challengedUserId, GetGameBoard(), position, direction);
                }
                    
                GameObject g = SpawnPiece(column, row * -1);

				float distance = Vector3.Distance(startPosition, endPosition);

				float t = 0;
				while(t < 1)
				{
					t += Time.deltaTime * dropTime;
					if (numRows - distance > 0) {
						t += (numRows - distance) / 100;
					}
                    g.transform.position = Vector3.Lerp (startPosition, endPosition, t);
					yield return null;
				}

                g.transform.parent = gamePieces.transform;

				StartCoroutine(CheckForWinner());

				// wait until winning check is done
				while(isCheckingForWinner)
					yield return null;

				isPlayerOneTurn = !isPlayerOneTurn;
                isCurrentPlayerTurn = !isCurrentPlayerTurn;

                if (gameOver == false) {
                    if (isMultiplayer)
                    {
                        SetMultiplayerGameStatusText();
                    } else {
                        gameStatusText.text = isPlayerOneTurn ? bluePlayerMoveText : redPlayerMoveText;
                        gameStatusText.color = isPlayerOneTurn ? bluePlayerColor : redPlayerColor;
                    }
                }
			}
               
            foreach (GameObject go in gos) {
                go.SetActive(false);
                SpriteRenderer sr = go.GetComponentInChildren<SpriteRenderer>();
                sr.enabled = true;
            }

			isDropping = false;

            if(OnMoved != null)
                OnMoved();
			yield return 0;
		}

		/// <summary>
		/// Check for Winner
		/// </summary>
		IEnumerator CheckForWinner()
		{
			isCheckingForWinner = true;

			for(int x = 0; x < numColumns; x++)
			{
				for(int y = 0; y < numRows; y++)
				{
					// Get the Laymask to Raycast against, if its Players turn only include
					// Layermask Blue otherwise Layermask Red
					int layermask = isPlayerOneTurn ? (1 << 8) : (1 << 9);

					// If its Players turn ignore red as Starting piece and wise versa
					if(gameBoard[x, y] != (isPlayerOneTurn ? (int)Piece.Blue : (int)Piece.Red))
					{
						continue;
					}

					// shoot a ray of length 'numPiecesToWin - 1' to the right to test horizontally
					RaycastHit2D[] hitsHorz = Physics2D.RaycastAll(
						new Vector2(x, y * -1), 
						Vector2.right, 
						numPiecesToWin - 1, 
						layermask);

					// return true (won) if enough hits
					if(hitsHorz.Length == numPiecesToWin)
					{
                        DrawLine(hitsHorz[0].transform.position, hitsHorz[3].transform.position, isPlayerOneTurn ? bluePlayerColor : redPlayerColor);
                        gameOver = true;
						break;
					}

					// shoot a ray up to test vertically
					RaycastHit2D[] hitsVert = Physics2D.RaycastAll(
						new Vector2(x, y * -1), 
						Vector2.up, 
						numPiecesToWin - 1,
						layermask);
                    
					if(hitsVert.Length == numPiecesToWin)
					{
                        //SpriteRenderer glow =  hitsVert[3].transform.gameObject.GetComponentInChildren<SpriteRenderer>();
                        //glow.enabled = false;
                        DrawLine(hitsVert[0].transform.position, hitsVert[3].transform.position, isPlayerOneTurn ? bluePlayerColor : redPlayerColor);

                        gameOver = true;
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
                            DrawLine(hitsDiaLeft[0].transform.position, hitsDiaLeft[3].transform.position, isPlayerOneTurn ? bluePlayerColor : redPlayerColor);
                            gameOver = true;
							break;
						}

						RaycastHit2D[] hitsDiaRight = Physics2D.RaycastAll(
							new Vector3(x, y * -1), 
							new Vector3(1 , 1), 
							length, 
							layermask);

						if(hitsDiaRight.Length == numPiecesToWin)
						{
                            DrawLine(hitsDiaRight[0].transform.position, hitsDiaRight[3].transform.position, isPlayerOneTurn ? bluePlayerColor : redPlayerColor);
                            gameOver = true;
							break;
						}
					}

					yield return null;
				}

				yield return null;
			}

			// if Game Over update the winning text to show who has won
			if(gameOver == true)
			{
                gameStatusText.text = isPlayerOneTurn ? bluePlayerWonText : redPlayerWonText;
                gameStatusText.color = isPlayerOneTurn ? bluePlayerColor : redPlayerColor;
			}
			else 
			{
				// check if there are any empty cells left, if not set game over and update text to show a draw
				if(!FieldContainsEmptyCell())
				{
					gameOver = true;
                    gameStatusText.text = drawText;
				}
            } 

			isCheckingForWinner = false;

			yield return 0;
		}

        void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
        {
            GameObject myLine = new GameObject();
            myLine.transform.parent = gamePieces.transform;
            myLine.transform.position = start;
            myLine.AddComponent<LineRenderer>();
            LineRenderer lr = myLine.GetComponent<LineRenderer>();
            lr.material = new Material(lineShader);
            lr.SetColors(color, color);
            lr.SetWidth(0.15f, 0.15f);
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
			for(int x = 0; x < numColumns; x++)
			{
				for(int y = 0; y < numRows; y++)
				{
					if(gameBoard[x, y] == (int)Piece.Empty)
						return true;
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

		int nextEmptySpotInColumnUp(int column) {
			int nextEmptyRow = -1;
			for (int row = numRows - 1; row >= 0; row--) {
				if (gameBoard [column, row] == 0) {
					nextEmptyRow = row;
					if (nextEmptyRow == 0) {
						return nextEmptyRow;
					}
				} else if (nextEmptyRow != -1) {
					return nextEmptyRow;
				} else {
					return nextEmptyRow;
				}
			}
			return nextEmptyRow;
		}

		int nextEmptySpotInColumnDown(int column) {
			int nextEmptyRow = -1;
			for (int row = 0; row < numRows; row++) {
				if (gameBoard [column, row] == 0) {
					nextEmptyRow = row;
					if (nextEmptyRow == numRows - 1) {
						return nextEmptyRow;
					}
				} else if (nextEmptyRow != -1) {
					return nextEmptyRow;
				} else {
					return nextEmptyRow;
				}
			}
			return nextEmptyRow;
		}

		int nextEmptySpotInRowRight(int row) {
			int nextEmptyCol = -1;
			for (int column = 0; column < numColumns; column++) {
				if (gameBoard [column, row] == 0) {
					nextEmptyCol = column;
					if (nextEmptyCol == numColumns - 1) {
						return nextEmptyCol;
					}
				} else if (nextEmptyCol != -1) {
					return nextEmptyCol;
				} else {
					return nextEmptyCol;
				}
			}
			return nextEmptyCol;
		}

		int nextEmptySpotInRowLeft(int row) {
			int nextEmptyCol = -1;
			for (int column = numColumns - 1; column >= 0; column--) {
				if (gameBoard [column, row] == 0) {
					nextEmptyCol = column;
					if (nextEmptyCol == 0) {
						return nextEmptyCol;
					}
				} else if (nextEmptyCol != -1) {
					return nextEmptyCol;
				} else {
					return nextEmptyCol;
				}
			}
			return nextEmptyCol;
		}

        //When this is called we reset the game instance to a blank state
        public void ClearInstance()
        {
            for (int i = 0; i < gameBoard.Length; i++)
            {

            }

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

		//		bool squaresMatchPiece(int piece, int row, int col, int moveX, int moveY) {
		//			// bail out early if we can't win from here
		//			if (row + (moveY * 3) < 0) { return false; }
		//			if (row + (moveY * 3) >= numRows) { return false; }
		//			if (col + (moveX * 3) < 0) { return false; }
		//			if (col + (moveX * 3) >= numColumns) { return false; }
		//
		//			// still here? Check every square
		//			if (field[col, row] != piece) { return false; }
		//			if (field[col + moveX,row + moveY] != piece) { return false; }
		//			if (field[col + (moveX * 2), row + (moveY * 2)] != piece) { return false; }
		//			if (field[col + (moveX * 3), row + (moveY * 3)] != piece) { return false; }
		//
		//			return true;
		//		}

		//		/// <summary>
		//		/// Check for Winner
		//		/// </summary>
		//		IEnumerator Won()
		//		{
		//			int currentPlayer = 1;
		//
		//			isCheckingForWinner = true;
		//
		//			for(int col = 0; col < numColumns; col++)
		//			{
		//				for(int row = 0; row < numRows; row++)
		//				{
		//					if (squaresMatchPiece(currentPlayer, row, col, moveX: 1, moveY: 0)) {
		//						gameOver = true;
		//						break;
		//					} else if (squaresMatchPiece(currentPlayer, row, col, moveX: 0, moveY: 1)) {
		//						gameOver = true;
		//						break;
		//					} else if (squaresMatchPiece(currentPlayer, row, col, moveX: 1, moveY: 1)) {
		//						gameOver = true;
		//						break;
		//					} else if (squaresMatchPiece(currentPlayer, row, col, moveX: 1, moveY: -1)) {
		//						gameOver = true;
		//						break;
		//					}
		//
		//					yield return null;
		//				}
		//
		//				yield return null;
		//			}
		//
		//			// if Game Over update the winning text to show who has won
		//			if(gameOver == true)
		//			{
		//				winningText.GetComponent<TextMesh>().text = isPlayersTurn ? playerWonText : playerLoseText;
		//			}
		//			else 
		//			{
		//				// check if there are any empty cells left, if not set game over and update text to show a draw
		//				if(!FieldContainsEmptyCell())
		//				{
		//					gameOver = true;
		//					winningText.GetComponent<TextMesh>().text = drawText;
		//				}
		//			}
		//
		//			isCheckingForWinner = false;
		//
		//			yield return 0;
		//		}
	}
}
