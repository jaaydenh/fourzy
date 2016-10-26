using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameSparks.Api.Requests;

namespace ConnectFour
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

		public GameObject winningText;
		public string bluePlayerWonText = "Blue Player Won!";
		public string redPlayerWonText = "Red Player Won!";
		public string playerWonText = "You Won!";
		public string playerLoseText = "You Lose!";
		public string drawText = "Draw!";

		public GameObject btnPlayAgain;
		bool btnPlayAgainTouching = false;
		Color btnPlayAgainOrigColor;
		Color btnPlayAgainHoverColor = new Color(255, 143,4);

		GameObject gamePieces;

		/// <summary>
		/// The Gameboard.
		/// 0 = Empty
		/// 1 = Blue
		/// 2 = Red
		/// </summary>
		public int[,] gameBoard;

		public bool isMultiplayer = false;
        public bool isCurrentPlayerTurn = true;
		public bool isPlayerOneTurn = true;
		bool isLoading = true;
		bool isDropping = false; 
		bool mouseButtonPressed = false;

		bool gameOver = false;
		bool isCheckingForWinner = false;

        int spacing = 1; //100
        int offset = 0; //4

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
            
		void Start() 
		{
			int max = Mathf.Max(numRows, numColumns);

			if(numPiecesToWin > max)
				numPiecesToWin = max;

			CreateGameBoard();

			btnPlayAgainOrigColor = btnPlayAgain.GetComponent<Renderer>().material.color;
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
                        GameObject g = Instantiate(pieceBlue, new Vector3(x, y * -1, 20), Quaternion.identity) as GameObject;
                        g.transform.parent = gamePieces.transform;
                    }
                    else if (piece == (int)Piece.Red)
                    {
                        GameObject g = Instantiate(pieceRed, new Vector3(x, y * -1, 20), Quaternion.identity) as GameObject;
                        g.transform.parent = gamePieces.transform;
                    }
                }
            }

            isLoading = false;
        }

        void CreateGameBoard() {
			winningText.SetActive(false);
			btnPlayAgain.SetActive(false);

			isLoading = true;

            gamePieces = GameObject.Find ("EmptySpot");
            if(gamePieces != null)
			{
                DestroyImmediate(gamePieces);
			}
            gamePieces = new GameObject("EmptySpot");

			// create an empty gameboard and instantiate the cells
			gameBoard = new int[numColumns, numRows];
			for(int col = 0; col < numColumns; col++)
			{
				for(int row = 0; row < numRows; row++)
				{
					gameBoard[col, row] = (int)Piece.Empty;
                    if (col + row == 0)
                    {
                        GameObject ula = Instantiate(cornerSpot, new Vector3((col - offset) * spacing, ((row - offset) * spacing) * -1, 20), Quaternion.identity) as GameObject;
                        ula.transform.parent = gamePieces.transform;
                        ula.GetComponent<CornerSpot>().downArrowActive = true;
                        ula.GetComponent<CornerSpot>().rightArrowActive = true;
                        ula.GetComponent<CornerSpot>().row = 0;
                        ula.GetComponent<CornerSpot>().column = 0;
                    }
                    else if (col == 0 && row == numRows - 1)
                    {
                        GameObject lla = Instantiate(cornerSpot, new Vector3((col - offset) * spacing, ((row - offset) * spacing) * -1, 20), Quaternion.identity) as GameObject;
                        lla.transform.parent = gamePieces.transform;
                        lla.GetComponent<CornerSpot>().rightArrowActive = true;
                        lla.GetComponent<CornerSpot>().upArrowActive = true;
                        lla.GetComponent<CornerSpot>().row = numRows - 1;
                        lla.GetComponent<CornerSpot>().column = 0;
                    }
                    else if (col * row == (numColumns - 1) * (numRows - 1))
                    { 
                        GameObject bra = Instantiate(cornerSpot, new Vector3((col - offset) * spacing, ((row - offset) * spacing) * -1, 20), Quaternion.identity) as GameObject;
                        bra.transform.parent = gamePieces.transform;
                        bra.GetComponent<CornerSpot>().upArrowActive = true;
                        bra.GetComponent<CornerSpot>().leftArrowActive = true;
                        bra.GetComponent<CornerSpot>().row = numRows - 1;
                        bra.GetComponent<CornerSpot>().column = numColumns - 1;
                    }
                    else if (col == numColumns - 1 && row == 0)
                    {
                        GameObject ura = Instantiate(cornerSpot, new Vector3((col - offset) * spacing, ((row - offset) * spacing) * -1, 20), Quaternion.identity) as GameObject;
                        ura.transform.parent = gamePieces.transform;
                        ura.GetComponent<CornerSpot>().downArrowActive = true;
                        ura.GetComponent<CornerSpot>().leftArrowActive = true;
                        ura.GetComponent<CornerSpot>().row = 0;
                        ura.GetComponent<CornerSpot>().column = numColumns - 1;
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

			winningText.transform.position = new Vector3(
				(numColumns-1) / 2.0f, -((numRows-1) / 2.0f) + 1, winningText.transform.position.z);

			btnPlayAgain.transform.position = new Vector3(
				(numColumns-1) / 2.0f, -((numRows-1) / 2.0f) - 1, btnPlayAgain.transform.position.z);
		}

		/// <summary>
		/// Spawns a piece at mouse position above the first row
		/// </summary>
		/// <returns>The piece.</returns>
        GameObject SpawnPiece(float posX, float posY)
		{          
			GameObject g = Instantiate(
				isPlayerOneTurn ? pieceBlue : pieceRed,
				new Vector3(Mathf.FloorToInt(posX + 0.5f), 
					Mathf.FloorToInt(posY + 0.5f), 10), // spawn it above the first row
					Quaternion.identity) as GameObject;

			return g;
		}

		void UpdatePlayAgainButton()
		{
			RaycastHit hit;
			//ray shooting out of the camera from where the mouse is
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			if (Physics.Raycast(ray, out hit) && hit.collider.name == btnPlayAgain.name)
			{
				btnPlayAgain.GetComponent<Renderer>().material.color = btnPlayAgainHoverColor;
				//check if the left mouse has been pressed down this frame
				if (Input.GetMouseButtonDown(0) || Input.touchCount > 0 && btnPlayAgainTouching == false)
				{
					btnPlayAgainTouching = true;
					
					//CreateField();

					//Application.LoadLevel(0);
				}
			}
			else
			{
				btnPlayAgain.GetComponent<Renderer>().material.color = btnPlayAgainOrigColor;
			}
			
			if(Input.touchCount == 0)
			{
				btnPlayAgainTouching = false;
			}
		}

		// Update is called once per frame
		void Update () 
		{
			if(isLoading)
				return;

			if(isCheckingForWinner)
				return;

			if(gameOver)
			{
				winningText.SetActive(true);
				btnPlayAgain.SetActive(true);

				UpdatePlayAgainButton();

				return;
			}

            if(isCurrentPlayerTurn)
			{
				if (Input.GetMouseButtonDown(0) && !mouseButtonPressed && !isDropping) {
					mouseButtonPressed = true;
					Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    // round to a grid cell
                    int column = Mathf.RoundToInt(pos.x);
                    int row = Mathf.RoundToInt(pos.y * -1);

					if (inTopRowBounds (pos.x, pos.y)) {
                        StartCoroutine(movePiece(column, row, Direction.Down));
					} else if (inBottomRowBounds(pos.x, pos.y)) {
                        StartCoroutine(movePiece(column, row, Direction.Up));
					} else if (inRightRowBounds(pos.x, pos.y)) {
                        StartCoroutine(movePiece(column, row, Direction.Left));
					} else if (inLeftRowBounds(pos.x, pos.y)) {
                        StartCoroutine(movePiece(column, row, Direction.Right));
					}
				} else {
					mouseButtonPressed = false;
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
        public IEnumerator movePiece(int column, int row, Direction direction)
		{
			isDropping = true;
            int movePosition = -1;
            Vector3 startPosition = new Vector3(column, row * -1, 10);
            Vector3 endPosition = new Vector3();

			bool foundFreeSpot = false;
			if (direction == Direction.Down) {
				int nextRow = nextEmptySpotInColumnDown(column);
				if (nextRow != -1) {
                    foundFreeSpot = true;
                    movePosition = column;
					gameBoard [column, nextRow] = isPlayerOneTurn ? (int)Piece.Blue : (int)Piece.Red;
                    endPosition = new Vector3(column, nextRow * -1, startPosition.z);
				}
			} else if (direction == Direction.Up) {
				int nextRow = nextEmptySpotInColumnUp(column);
				if (nextRow != -1) {
                    foundFreeSpot = true;
                    movePosition = column;
					gameBoard [column, nextRow] = isPlayerOneTurn ? (int)Piece.Blue : (int)Piece.Red;
                    endPosition = new Vector3(column, nextRow * -1, startPosition.z);
				}
			} else if (direction == Direction.Right) {
				int nextColumn = nextEmptySpotInRowRight(row);
				if (nextColumn != -1) {
                    foundFreeSpot = true;
                    movePosition = row;
					gameBoard [nextColumn, row] = isPlayerOneTurn ? (int)Piece.Blue : (int)Piece.Red;
                    endPosition = new Vector3(nextColumn, row * -1, startPosition.z);
				}
			} else if (direction == Direction.Left) {
				int nextColumn = nextEmptySpotInRowLeft(row);
				if (nextColumn != -1) {
                    foundFreeSpot = true;
                    movePosition = row;
					gameBoard [nextColumn, row] = isPlayerOneTurn ? (int)Piece.Blue : (int)Piece.Red;
					endPosition = new Vector3(nextColumn, row * -1, startPosition.z);
				}
			}

            GameObject[] gos;
            gos = GameObject.FindGameObjectsWithTag("Arrow");

            foreach (GameObject go in gos) {
                SpriteRenderer sr = go.GetComponentInChildren<SpriteRenderer>();
                sr.enabled = false;
            }

            if(foundFreeSpot)
			{
                Debug.Log("Direction: " + direction.GetHashCode());
                Debug.Log("Direction: " + direction);
                Debug.Log("Move Position: " + movePosition);
                Debug.Log("challengeInstanceId: " + challengeInstanceId);
                if (isMultiplayer)
                {
                    new LogChallengeEventRequest().SetChallengeInstanceId(challengeInstanceId)
                    .SetEventKey("takeTurn") //The event we are calling is "takeTurn", we set this up on the GameSparks Portal
                    .SetEventAttribute("pos", movePosition) // pos is the row or column the piece was placed at depending on the direction
                    .SetEventAttribute("direction", direction.GetHashCode()) // direction can be up, down, left, right
                    .SetEventAttribute("player", isPlayerOneTurn ? (int)Piece.Blue : (int)Piece.Red) //takeTurn also has an attribute called "playerIcon, we set to our X or O
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
				//isPlayersTurn = !isPlayersTurn;
			}
               
            foreach (GameObject go in gos) {
                go.SetActive(false);
                SpriteRenderer sr = go.GetComponentInChildren<SpriteRenderer>();
                sr.enabled = true;
            }

			isDropping = false;

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
				winningText.GetComponent<TextMesh>().text = isPlayerOneTurn ? bluePlayerWonText : redPlayerWonText;
			}
			else 
			{
				// check if there are any empty cells left, if not set game over and update text to show a draw
				if(!FieldContainsEmptyCell())
				{
					gameOver = true;
					winningText.GetComponent<TextMesh>().text = drawText;
				}
			}

			isCheckingForWinner = false;

			yield return 0;
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
