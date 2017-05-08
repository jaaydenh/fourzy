using System.Collections.Generic;
using UnityEngine;

namespace Fourzy
{

    public class GameBoardView : MonoBehaviour {

        public GameObject[,] gamePieces; //Collection of Views
        public IToken[,] tokenBoard;
        [Range(3, 8)]
        public int numRows = Constants.numRows;
        [Range(3, 8)]
        public int numColumns = Constants.numRows;
        public List<MovingGamePiece> activeMovingPieces;
        public List<MovingGamePiece> completedMovingPieces;

    	void Start () {
            activeMovingPieces = new List<MovingGamePiece>();
            completedMovingPieces = new List<MovingGamePiece>();
            tokenBoard = new IToken[numRows, numColumns];
            gamePieces = new GameObject[numRows, numColumns];
    	}

        public void MakePieceMoveable(Position pos, bool moveable, Direction direction) {
            gamePieces[pos.row, pos.column].GetComponent<GamePiece>().MakeMoveable(moveable, direction);
        }

        public void SwapPiecePosition(Position oldPos, Position newPos) {
            GameObject oldPiece = gamePieces[oldPos.row, oldPos.column];
            GamePiece gamePiece = oldPiece.GetComponent<GamePiece>();
            
            gamePiece.column = newPos.column;
            gamePiece.row = newPos.row;
            gamePieces[oldPos.row, oldPos.column] = null;
            gamePieces[newPos.row, newPos.column] = oldPiece;
        }

        public void DisableNextMovingPiece() {
            if (activeMovingPieces.Count > 0) {
                completedMovingPieces.Add(activeMovingPieces[0]);
                activeMovingPieces.RemoveAt(0);
            }
        }

        public void PrintGameBoard() {
            string gameboard = "GameboardView: \n";

            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numColumns; col++)
                {
                    if (gamePieces[row, col]) {
                        gameboard += (int)gamePieces[row, col].GetComponent<GamePiece>().player;    
                    } else {
                        gameboard += "0";
                    }
                }
                gameboard += "\n";
            }
            Debug.Log(gameboard);
        }
    }
}