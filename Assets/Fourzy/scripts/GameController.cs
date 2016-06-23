using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ConnectFour
{
	public class GameController : MonoBehaviour 
	{
		enum Piece
		{
			Empty = 0,
			Blue = 1,
			Red = 2
		}

		enum Direction {Up, Down, Left, Right};

		[Range(3, 8)]
		public int numRows = 6;
		[Range(3, 8)]
		public int numColumns = 7;

		[Tooltip("How many pieces have to be connected to win.")]
		public int numPiecesToWin = 4;

		[Tooltip("Allow diagonally connected Pieces?")]
		public bool allowDiagonally = true;
		
		public float dropTime = 1.0f;

		// Gameobjects 
		public GameObject pieceRed;
		public GameObject pieceBlue;
		public GameObject pieceField;

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

		GameObject gameObjectField;

		// temporary gameobject, holds the piece at mouse position until the mouse has clicked
		GameObject gameObjectTurn;

		/// <summary>
		/// The Game field.
		/// 0 = Empty
		/// 1 = Blue
		/// 2 = Red
		/// </summary>
		int[,] field;

		bool isPlayersTurn = true;
		bool isPlayerOneTurn = true;
		bool isLoading = true;
		bool isDropping = false; 
		bool mouseButtonPressed = false;

		bool gameOver = false;
		bool isCheckingForWinner = false;

		// Use this for initialization
		void Start () 
		{
			int max = Mathf.Max (numRows, numColumns);

			if(numPiecesToWin > max)
				numPiecesToWin = max;

			CreateField ();

			//isPlayersTurn = System.Convert.ToBoolean(Random.Range (0, 1));

			btnPlayAgainOrigColor = btnPlayAgain.GetComponent<Renderer>().material.color;
		}

		/// <summary>
		/// Creates the field.
		/// </summary>
		void CreateField()
		{
			winningText.SetActive(false);
			btnPlayAgain.SetActive(false);

			isLoading = true;

			gameObjectField = GameObject.Find ("EmptySpot");
			if(gameObjectField != null)
			{
				DestroyImmediate(gameObjectField);
			}
			gameObjectField = new GameObject("EmptySpot");

			// create an empty field and instantiate the cells
			field = new int[numColumns, numRows];
			for(int x = 0; x < numColumns; x++)
			{
				for(int y = 0; y < numRows; y++)
				{
					field[x, y] = (int)Piece.Empty;
					GameObject g = Instantiate(pieceField, new Vector3(x, y * -1, 20), Quaternion.identity) as GameObject;
					g.transform.parent = gameObjectField.transform;
				}
			}

			isLoading = false;
			gameOver = false;

			// center camera 
			Camera.main.transform.position = new Vector3(
				(numColumns-1) / 2.0f, -((numRows-1) / 2.0f), Camera.main.transform.position.z);

			winningText.transform.position = new Vector3(
				(numColumns-1) / 2.0f, -((numRows-1) / 2.0f) + 1, winningText.transform.position.z);

			btnPlayAgain.transform.position = new Vector3(
				(numColumns-1) / 2.0f, -((numRows-1) / 2.0f) - 1, btnPlayAgain.transform.position.z);
		}

		/// <summary>
		/// Spawns a piece at mouse position above the first row
		/// </summary>
		/// <returns>The piece.</returns>
		GameObject SpawnPiece()
		{
			Vector3 spawnPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					
			if(!isPlayersTurn)
			{
				List<int> moves = GetPossibleMoves();

				if(moves.Count > 0)
				{
					int column = moves[Random.Range (0, moves.Count)];

					spawnPos = new Vector3(column, 0, 0);
				}
			}

			GameObject g = Instantiate(
				isPlayerOneTurn ? pieceBlue : pieceRed, // is players turn = spawn blue, else spawn red
				new Vector3(Mathf.FloorToInt(spawnPos.x + 0.5f), 
					Mathf.FloorToInt(spawnPos.y + 0.5f), 10), // spawn it above the first row
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
					Application.LoadLevel(0);
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

			if(isPlayersTurn)
			{
//				if(gameObjectTurn == null)
//				{
//					gameObjectTurn = SpawnPiece();
//				}
//				else
//				{
					// update the objects position
					//Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					
//					gameObjectTurn.transform.position = new Vector3(
//						Mathf.Clamp(pos.x, 0, numColumns-1), 
//						gameObjectField.transform.position.y + 1, 0);

					// click the left mouse button to drop the piece into the selected column
				if (Input.GetMouseButtonDown (0) && !mouseButtonPressed && !isDropping) {
					mouseButtonPressed = true;
					Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
					if (inTopRowBounds (pos.x, pos.y)) {
						gameObjectTurn = SpawnPiece ();
						StartCoroutine (movePiece (gameObjectTurn, Direction.Down));
					} else if (inBottomRowBounds(pos.x, pos.y)) {
						gameObjectTurn = SpawnPiece ();
						StartCoroutine (movePiece (gameObjectTurn, Direction.Up));
					} else if (inRightRowBounds(pos.x, pos.y)) {
						gameObjectTurn = SpawnPiece ();
						StartCoroutine (movePiece (gameObjectTurn, Direction.Left));
					} else if (inLeftRowBounds(pos.x, pos.y)) {
						gameObjectTurn = SpawnPiece ();
						StartCoroutine (movePiece (gameObjectTurn, Direction.Right));
					}
				} else {
					mouseButtonPressed = false;
				}
				//}
			}
			else
			{
				if(gameObjectTurn == null)
				{
					gameObjectTurn = SpawnPiece();
				}
				else
				{
					if(!isDropping)
						StartCoroutine(movePiece(gameObjectTurn, Direction.Down));
				}
			}
		}

		/// <summary>
		/// Gets all the possible moves.
		/// </summary>
		/// <returns>The possible moves.</returns>
		public List<int> GetPossibleMoves()
		{
			List<int> possibleMoves = new List<int>();
			for (int col = 0; col < numColumns; col++)
			{
				int row = nextEmptyRowInColumnDown (col);
				if(row != -1) {
					possibleMoves.Add(col);
				}
			}
			return possibleMoves;
		}

		/// <summary>
		/// This method searches for a empty cell and lets 
		/// the object fall down into this cell
		/// </summary>
		/// <param name="gObject">Game Object.</param>
		IEnumerator movePiece(GameObject gObject, Direction direction)
		{
			isDropping = true;

			Vector3 startPosition = gObject.transform.position;
			Vector3 endPosition = new Vector3();

			// round to a grid cell
			int column = Mathf.RoundToInt(startPosition.x);
			int row = Mathf.RoundToInt(startPosition.y) * -1;
			startPosition = new Vector3(column, startPosition.y, startPosition.z);

			// is there a free cell in the selected column?
			bool foundFreeCell = false;
			if (direction == Direction.Down) {
				int nextRow = nextEmptyRowInColumnDown (column);
				if (nextRow != -1) {
					foundFreeCell = true;
					field [column, nextRow] = isPlayerOneTurn ? (int)Piece.Blue : (int)Piece.Red;
					endPosition = new Vector3 (column, nextRow * -1, startPosition.z);
				}
			} else if (direction == Direction.Up) {
				int nextRow = nextEmptyRowInColumnUp (column);
				if (nextRow != -1) {
					foundFreeCell = true;
					field [column, nextRow] = isPlayerOneTurn ? (int)Piece.Blue : (int)Piece.Red;
					endPosition = new Vector3 (column, nextRow * -1, startPosition.z);
				}
			} else if (direction == Direction.Right) {
				int nextColumn = nextEmptySpaceInRowRight (row);
				if (nextColumn != -1) {
					foundFreeCell = true;
					field [nextColumn, row] = isPlayerOneTurn ? (int)Piece.Blue : (int)Piece.Red;
					endPosition = new Vector3 (nextColumn, row * -1, startPosition.z);
				}
			} else if (direction == Direction.Left) {
				//int nextColumn = nextEmptySpaceInRowLeft (row);
				int nextColumn = -1;
				for (int col = numColumns - 1; col >= 0; col--) {
					if (field [col, row] == 0) {
						nextColumn = col;
						if (nextColumn == 0) {
							break;
						}
					} else if (nextColumn != -1) {
						break;
					} else {
						break;
					}
				}

				if (nextColumn != -1) {
					
					field [nextColumn, row] = isPlayerOneTurn ? (int)Piece.Blue : (int)Piece.Red;
					endPosition = new Vector3 (nextColumn, row * -1, startPosition.z);
					foundFreeCell = true;
				}
			}

			if(foundFreeCell)
			{
				// Instantiate a new Piece, disable the temporary
				GameObject g = Instantiate (gObject) as GameObject;
				gameObjectTurn.GetComponent<Renderer>().enabled = false;

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

				g.transform.parent = gameObjectField.transform;

				// remove the temporary gameobject
				DestroyImmediate(gameObjectTurn);

				// run coroutine to check if someone has won
				StartCoroutine(Won());

				// wait until winning check is done
				while(isCheckingForWinner)
					yield return null;

				isPlayerOneTurn = !isPlayerOneTurn;
				//isPlayersTurn = !isPlayersTurn;
			}

			isDropping = false;

			yield return 0;
		}

		/// <summary>
		/// Check for Winner
		/// </summary>
		IEnumerator Won()
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
					if(field[x, y] != (isPlayerOneTurn ? (int)Piece.Blue : (int)Piece.Red))
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
					if(field[x, y] == (int)Piece.Empty)
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

		int nextEmptyRowInColumnUp(int column) {
			int nextEmptyRow = -1;
			for (int row = numRows - 1; row >= 0; row--) {
				if (field [column, row] == 0) {
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

		int nextEmptyRowInColumnDown(int column) {
			int nextEmptyRow = -1;
			for (int row = 0; row < numRows; row++) {
				if (field [column, row] == 0) {
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

		int nextEmptySpaceInRowRight(int row) {
			int nextEmptyCol = -1;
			for (int column = 0; column < numColumns; column++) {
				if (field [column, row] == 0) {
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

		int nextEmptySpaceInRowLeft(int row) {
			int nextEmptyCol = -1;
			for (int column = numColumns - 1; column >= 0; column--) {
				if (field [column, row] == 0) {
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
