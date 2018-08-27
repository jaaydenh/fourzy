using UnityEngine;

namespace Fourzy
{
    public class GameBoardView : MonoBehaviour {

        public GamePiece[,] gamePieces; //Collection of GamePiece Views
        public GameObject[,] tokens; //Collection of Token Views
        [Range(3, 8)]
        public int numRows = Constants.numRows;
        [Range(3, 8)]
        public int numColumns = Constants.numRows;

        void Start () {
            tokens = new GameObject[numRows, numColumns];
            gamePieces = new GamePiece[numRows, numColumns];
        }

        public void MakePieceMoveable(Position pos, bool moveable, Direction direction) {
            gamePieces[pos.row, pos.column].MakeMoveable(moveable, direction);
        }

        public void SwapPiecePosition(Position oldPos, Position newPos) {
            GamePiece gamePiece = gamePieces[oldPos.row, oldPos.column];
            
            gamePiece.column = newPos.column;
            gamePiece.row = newPos.row;
            gamePieces[oldPos.row, oldPos.column] = null;
            gamePieces[newPos.row, newPos.column] = gamePiece;
        }

        public GamePiece GamePieceAt(Position position)
        {            
            return gamePieces[position.row, position.column];
        }

        public void Clear() {
            gamePieces = new GamePiece[numRows, numColumns];
        }

        public void PrintGameBoard() {
            string gameboard = "GameboardView: \n";

            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numColumns; col++)
                {
                    if (gamePieces[row, col]) {
                        gameboard += (int)gamePieces[row, col].player;
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