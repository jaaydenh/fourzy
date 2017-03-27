using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy {
    
    public struct GameBoard {

        private int numRows;
        private int numColumns;

        public int[] board;
        public int[] isMoveableUp;
        public int[] isMoveableDown;
        public int[] isMoveableLeft;
        public int[] isMoveableRight;
        public List<MovingGamePiece> activeMovingPieces;
        public List<MovingGamePiece> completedMovingPieces;

        public GameBoard (bool test) {
            numRows = Constants.numRows;
            numColumns = Constants.numColumns;

            board = new int[numColumns * numRows];
            isMoveableUp = new int[numColumns * numRows];
            isMoveableDown = new int[numColumns * numRows];
            isMoveableLeft = new int[numColumns * numRows];
            isMoveableRight = new int[numColumns * numRows];
            activeMovingPieces = new List<MovingGamePiece>();
            completedMovingPieces = new List<MovingGamePiece>();

            InitGameBoard();
    	}
    	
        public void InitGameBoard() {
            for (int i = 0; i < numColumns * numRows; i++)
            {
                board[i] = 0;
                isMoveableUp[i] = 1;
                isMoveableDown[i] = 1;
                isMoveableLeft[i] = 1;
                isMoveableRight[i] = 1;
            }

            for (int i = 0; i < numColumns * numRows; i++)
            {
                board[i] = 0;
            }
        }

        public void PrintBoard() {
            string log = "";
            for (int i = 0; i < numColumns * numRows; i++)
            {
                log += board[i] + ",";
            }

            Debug.Log(log);
        }

        public void SetGameBoard(int[] boardData) {
            board = boardData;
        }

        public void SwapPiecePosition(Position oldPos, Position newPos) {
            //Debug.Log("oldPos col: " + oldPos.column);
            //Debug.Log("oldPos row: " + oldPos.row);

            int oldPiece = board[oldPos.column * numColumns + oldPos.row];
            //Debug.Log("OLDPIECE: " + oldPiece + " oldpos.col: " + oldPos.column + " oldpos row: " + oldPos.row);
            board[oldPos.column * numColumns + oldPos.row] = 0;
            board[newPos.column * numColumns + newPos.row] = oldPiece;
        }

        public void DisableNextMovingPiece() {
            if (activeMovingPieces.Count > 0) {
                completedMovingPieces.Add(activeMovingPieces[0]);
                activeMovingPieces.RemoveAt(0);
            }
        }

        public void MakePieceMoveable(Position pos, bool moveable, Direction direction) {

            switch (direction)
            {
                case Direction.UP:
                    isMoveableUp[pos.column * numColumns + pos.row] = moveable ? 1 : 0;
                    break;
                case Direction.DOWN:
                    isMoveableDown[pos.column * numColumns + pos.row] = moveable ? 1 : 0;
                    break;
                case Direction.LEFT:
                    isMoveableLeft[pos.column * numColumns + pos.row] = moveable ? 1 : 0;
                    break;
                case Direction.RIGHT:
                    isMoveableRight[pos.column * numColumns + pos.row] = moveable ? 1 : 0;
                    break;
                default:
                    break;
            }
        }

        public Position GetNextPosition(Move move) {
            Position nextPosition = new Position(0,0);

            switch (move.direction)
            {
                case Direction.UP:
                    nextPosition.column = move.position.column;
                    nextPosition.row = move.position.row - 1;
                    break;
                case Direction.DOWN:
                    nextPosition.column = move.position.column;
                    nextPosition.row = move.position.row + 1;
                    break;
                case Direction.LEFT:
                    nextPosition.column = move.position.column - 1;
                    nextPosition.row = move.position.row;
                    break;
                case Direction.RIGHT:
                    nextPosition.column = move.position.column + 1;
                    nextPosition.row = move.position.row;
                    break;
                default:
                    break;
            }

            return nextPosition;
        }
    }
}
