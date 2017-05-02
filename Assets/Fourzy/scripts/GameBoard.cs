using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Fourzy {
    
    public class GameBoard {

        private int numRows;
        private int numColumns;
        private int numPiecesToWin;
        private int piecesCount = 0;

        //protected int[] board;
        protected int[,] boardnew;

        public int[] isMoveableUp;
        public int[] isMoveableDown;
        public int[] isMoveableLeft;
        public int[] isMoveableRight;
        public List<MovingGamePiece> activeMovingPieces;
        public List<MovingGamePiece> completedMovingPieces;

        public GameBoard (int numRows, int numColumns, int numPiecesToWin) {
            this.numRows = numRows;
            this.numColumns = numColumns;
            this.numPiecesToWin = numPiecesToWin;

            //board = new int[numColumns * numRows];
            boardnew = new int[numColumns, numRows];

            isMoveableUp = new int[numColumns * numRows];
            isMoveableDown = new int[numColumns * numRows];
            isMoveableLeft = new int[numColumns * numRows];
            isMoveableRight = new int[numColumns * numRows];
            activeMovingPieces = new List<MovingGamePiece>();
            completedMovingPieces = new List<MovingGamePiece>();

		    InitGameBoard();
    	}
    	
        public GameBoard (int numRows, int numColumns, int numPiecesToWin, int piecesCount, int[,] board)
        {
            this.numRows = numRows;
            this.numColumns = numColumns;
            this.numPiecesToWin = numPiecesToWin;
            this.piecesCount = piecesCount;

            this.boardnew = new int[numColumns, numRows];

            for (int row = 0; row < numRows; row++) {
                for (int col = 0; col < numColumns; col++) {
                    this.boardnew [row, col] = board [row, col];
                }
            }
        }

        public void InitGameBoard() {
            for (int i = 0; i < numColumns * numRows; i++)
            {
                //board[i] = (int)Piece.EMPTY;
                isMoveableUp[i] = 1;
                isMoveableDown[i] = 1;
                isMoveableLeft[i] = 1;
                isMoveableRight[i] = 1;
            }

            for (int row = 0; row < numRows; row++) {
                for (int col = 0; col < numColumns; col++) {
                    boardnew[row, col] = (int)Piece.EMPTY;
                }
            }
        }

        public void SampleBoard() {
            boardnew = new int[8, 8] { 
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 }
            };
        }

        public void SetCell(int col, int row, Player player) {
            //board[col * numColumns + row] = (int)player;
            boardnew[row, col] = (int)player;
        }

        public int GetCell(int col, int row) {
            //return board[col * numColumns + row];
            return boardnew[row, col]; 
        }

        public void SetGameBoard(int[] boardData) {
            for (int row = 0; row < numRows; row++) {
                for (int col = 0; col < numColumns; col++) {
                    boardnew[row, col] = boardData[row * numRows + col];
                }
            }
            //board = boardData;
        }

        public void SwapPiecePosition(Position oldPos, Position newPos) {
            //Debug.Log("oldPos col: " + oldPos.column);
            //Debug.Log("oldPos row: " + oldPos.row);

            // int oldPiece = board[oldPos.column * numColumns + oldPos.row];
            // Debug.Log("OLDPIECE: " + oldPiece + " oldpos.col: " + oldPos.column + " oldpos row: " + oldPos.row);
            // Debug.Log("NEWPIECE: " + oldPiece + " newPos.col: " + newPos.column + " newPos row: " + newPos.row);
            // board[oldPos.column * numColumns + oldPos.row] = 0;
            // board[newPos.column * numColumns + newPos.row] = oldPiece;

            int oldPiece = boardnew[oldPos.row, oldPos.column];
            //Debug.Log("OLDPIECE: " + oldPiece + " oldpos.col: " + oldPos.column + " oldpos row: " + oldPos.row);
            //Debug.Log("NEWPIECE: " + oldPiece + " newPos.col: " + newPos.column + " newPos row: " + newPos.row);
            boardnew[oldPos.row, oldPos.column] = 0;
            boardnew[newPos.row, newPos.column] = oldPiece;
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
                    isMoveableUp[pos.row * numRows + pos.column] = moveable ? 1 : 0;
                    break;
                case Direction.DOWN:
                    isMoveableDown[pos.row * numRows + pos.column] = moveable ? 1 : 0;
                    break;
                case Direction.LEFT:
                    isMoveableLeft[pos.row * numRows + pos.column] = moveable ? 1 : 0;
                    break;
                case Direction.RIGHT:
                    isMoveableRight[pos.row * numRows + pos.column] = moveable ? 1 : 0;
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

        public bool ContainsEmptyCell ()
        {
            return (piecesCount < numRows * numColumns);
        }

        public GameBoard Clone ()
        {
            return new GameBoard (numRows, numColumns, numPiecesToWin, piecesCount, boardnew);
        }

        public void PrintBoard(string name) {
            string log = name + "\n";

            for (int row = 0; row < numRows; row++) {
                for (int col = 0; col < numColumns; col++) {
                    log += boardnew[row, col];
                }
                log += "\n";
            }

            Debug.Log(log);
        }
    }
}
