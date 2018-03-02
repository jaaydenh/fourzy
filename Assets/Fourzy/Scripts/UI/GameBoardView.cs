using UnityEngine;

namespace Fourzy
{
    public class GameBoardView : MonoBehaviour {

        public GameObject[,] gamePieces; //Collection of GamePiece Views
        public GameObject[,] tokens; //Collection of Token Views
        [Range(3, 8)]
        public int numRows = Constants.numRows;
        [Range(3, 8)]
        public int numColumns = Constants.numRows;

        void Start () {
            tokens = new GameObject[numRows, numColumns];
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

        public void Clear() {
            gamePieces = new GameObject[numRows, numColumns];
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