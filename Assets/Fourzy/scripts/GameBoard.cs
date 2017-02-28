using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy {
    
    public class GameBoard : MonoBehaviour {

        GameObject gamePiecesView;
        GameObject tempPiece;
        public GameObject gameScreenCanvas;
        public GameObject pieceRed; // View
        public GameObject pieceBlue; // View
        public GameObject[,] gamePieces; //Collection of Views
        public IToken[,] tokenBoard;
        [Range(3, 8)]
        public int numRows = Constants.numRows;
        [Range(3, 8)]
        public int numColumns = Constants.numRows;
        bool isLoading = true;

        public List<MovingGamePiece> activeMovingPieces;
        public List<MovingGamePiece> completedMovingPieces;

    	void Start () {
            activeMovingPieces = new List<MovingGamePiece>();
            completedMovingPieces = new List<MovingGamePiece>();
            tokenBoard = new IToken[numColumns, numRows];
            gamePieces = new GameObject[numColumns, numRows];
    	}

        public void MakePieceMoveable(Position pos, bool moveable, Direction direction) {
            gamePieces[pos.column, pos.row].GetComponent<GamePiece>().MakeMoveable(moveable, direction);
        }

        public void SwapPiecePosition(Position oldPos, Position newPos) {
            GameObject oldPiece = gamePieces[oldPos.column, oldPos.row];
            GamePiece gamePiece = oldPiece.GetComponent<GamePiece>();
            //gamePiece.position = newPos;
            gamePiece.column = newPos.column;
            gamePiece.row = newPos.row;
            gamePieces[oldPos.column, oldPos.row] = null;
            gamePieces[newPos.column, newPos.row] = oldPiece;
        }

        public void SwapStickyPiecePosition(Position oldPos, Position newPos) {
            GameObject oldPiece = gamePieces[oldPos.column, oldPos.row];
            gamePieces[oldPos.column, oldPos.row] = null;
            gamePieces[newPos.column, newPos.row] = oldPiece;
        }

        public void DisableNextMovingPiece() {
            if (activeMovingPieces.Count > 0) {
                completedMovingPieces.Add(activeMovingPieces[0]);
                activeMovingPieces.RemoveAt(0);
            }
        }

        public Player PlayerAtPosition(Position position) {
            if (gamePieces[position.column, position.row]) {
                Player player = gamePieces[position.column, position.row].GetComponent<GamePiece>().player;
                return player;
            }
            return Player.NONE;
        }

        public List<long> GetGameBoardData() {
            List<long> gameBoardList = new List<long>();
            for(int col = 0; col < numColumns; col++)
            {
                for(int row = 0; row < numRows; row++)
                {
                    if (gamePieces[col, row]) {
                        gameBoardList.Add((int)gamePieces[col, row].GetComponent<GamePiece>().player);    
                    } else {
                        gameBoardList.Add(0);
                    }
                }
            }
            return gameBoardList;
        }

        public void PrintGameBoard() {
            string gameboard = "Gameboard: ";

            for (int col = 0; col < numColumns; col++)
            {
                for (int row = 0; row < numRows; row++)
                {
                    if (gamePieces[col, row]) {
                        gameboard += (int)gamePieces[col, row].GetComponent<GamePiece>().player + ",";    
                    } else {
                        gameboard += "0,";
                    }
                }
            }
            Debug.Log(gameboard);
        }

        public void ResetGameBoard() {

            isLoading = true;

            if(gamePiecesView != null)
            {
                DestroyImmediate(gamePiecesView);
            }
            gamePiecesView = new GameObject("GamePieces");
            gamePiecesView.transform.parent = gameScreenCanvas.transform;

            // create an empty gameboard and instantiate the cells
            //gameBoard = new int[numColumns, numRows];
            //tokenBoard = new int[numColumns, numRows];
            gamePieces = new GameObject[numColumns, numRows];

            for (int col = 0; col < numColumns; col++)
            {
                for (int row = 0; row < numRows; row++)
                {
                    //gameBoard[col, row] = (int)Piece.Empty;
                    //tokenBoard[col, row] = (int)Token.Empty;
                }
            }
                

            isLoading = false;
            //gameOver = false;
        }

        public IEnumerator SetGameBoard(int[] boardData) {
            //isLoading = true;

            for(int col = 0; col < numColumns; col++)
            {
                for(int row = 0; row < numRows; row++)
                {
                    int piece = boardData[col * numColumns + row];
                    //gameBoard[col, row] = piece;
                    if (piece == (int)Piece.BLUE)
                    {
                        gamePieces[col,row] = Instantiate(pieceBlue, new Vector3(col, row * -1, 10), Quaternion.identity, gamePiecesView.transform);
                    }
                    else if (piece == (int)Piece.RED)
                    {
                        gamePieces[col,row] = Instantiate(pieceRed, new Vector3(col, row * -1, 10), Quaternion.identity, gamePiecesView.transform);
                    }
                }
            }

            //isLoading = false;

            yield return 0;
        }
    }
}