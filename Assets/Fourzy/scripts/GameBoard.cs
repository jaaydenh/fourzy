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
        protected int[,] board;
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

            board = new int[numColumns, numRows];

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

            this.board = new int[numColumns, numRows];

            for (int row = 0; row < numRows; row++) {
                for (int col = 0; col < numColumns; col++) {
                    this.board [row, col] = board [row, col];
                }
            }
        }

        public void InitGameBoard() {
            for (int i = 0; i < numColumns * numRows; i++)
            {
                isMoveableUp[i] = 1;
                isMoveableDown[i] = 1;
                isMoveableLeft[i] = 1;
                isMoveableRight[i] = 1;
            }

            for (int row = 0; row < numRows; row++) {
                for (int col = 0; col < numColumns; col++) {
                    board[row, col] = (int)Piece.EMPTY;
                }
            }
        }

        public void SetCell(int col, int row, Player player) {
            board[row, col] = (int)player;
        }

        public int GetCell(int col, int row) {
            return board[row, col]; 
        }

        public void SetGameBoard(int[] boardData, IToken[,] tokens) {
            for (int row = 0; row < numRows; row++) {
                for (int col = 0; col < numColumns; col++) {
                    board[row, col] = boardData[row * numRows + col];
                    if (board[row, col] != (int)Piece.EMPTY) {
                        Position pos = new Position(col, row);
                        MovingGamePiece mgp = new MovingGamePiece(new Move(pos, Direction.UP));
                        completedMovingPieces.Add(mgp);
                    }
                }
            }
            UpdateMoveablePieces(tokens);
            completedMovingPieces.Clear();
        }

        public int[,] GetBoard() {
            return board;
        }

        public void SwapPiecePosition(Position oldPos, Position newPos) {
            //Debug.Log("OLDPIECE: " + oldPiece + " oldpos.col: " + oldPos.column + " oldpos row: " + oldPos.row);
            //Debug.Log("NEWPIECE: " + oldPiece + " newPos.col: " + newPos.column + " newPos row: " + newPos.row);
            int oldPiece = board[oldPos.row, oldPos.column];
            board[oldPos.row, oldPos.column] = 0;
            board[newPos.row, newPos.column] = oldPiece;
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

        public Player PlayerAtPosition(Position position) {
            return (Player)board[position.row, position.column];
        }

        public List<long> GetGameBoardData() {
            List<long> data = new List<long>();
            for(int row = 0; row < numRows; row++)
            {
                for(int col = 0; col < numColumns; col++)
                {
                    data.Add(board[row, col]);
                }
            }
            return data;
        }

        public void UpdateMoveablePieces(IToken[,] tokens) {
            foreach (var piece in completedMovingPieces)
            {
                Position currentPosition = piece.GetCurrentPosition();
                //Debug.Log("UpdateMoveablePieces col: " + currentPosition.column + " row: " + currentPosition.row);
                if (tokens[currentPosition.row, currentPosition.column].tokenType == Token.STICKY) {

                    if (CanMove(new Move(piece.GetNextPositionWithDirection(Direction.UP), Direction.UP), tokens)) {
                        MakePieceMoveable(currentPosition, true, Direction.UP);
                    } else {
                        MakePieceMoveable(currentPosition, false, Direction.UP);
                    } 
                    if (CanMove(new Move(piece.GetNextPositionWithDirection(Direction.DOWN), Direction.DOWN), tokens)) {
                        MakePieceMoveable(currentPosition, true, Direction.DOWN);
                    } else {
                        MakePieceMoveable(currentPosition, false, Direction.DOWN);
                    }
                    if (CanMove(new Move(piece.GetNextPositionWithDirection(Direction.LEFT), Direction.LEFT), tokens)) {
                        MakePieceMoveable(currentPosition, true, Direction.LEFT);
                    } else {
                        MakePieceMoveable(currentPosition, false, Direction.LEFT);
                    }
                    if (CanMove(new Move(piece.GetNextPositionWithDirection(Direction.RIGHT), Direction.RIGHT), tokens)) {
                        MakePieceMoveable(currentPosition, true, Direction.RIGHT);
                    } else {
                        MakePieceMoveable(currentPosition, false, Direction.RIGHT);
                    }
                } else {
                    MakePieceMoveable(currentPosition, false, Direction.UP);
                    MakePieceMoveable(currentPosition, false, Direction.DOWN);
                    MakePieceMoveable(currentPosition, false, Direction.LEFT);
                    MakePieceMoveable(currentPosition, false, Direction.RIGHT);
                }
            }
        }

        public bool CanMove(Move move, IToken[,] tokens)
        {
            Position endPosition = move.position;
            //Debug.Log("startPosition:col: " + startPosition.column + " row: " + startPosition.row);
            //Debug.Log("endPosition:col: " + endPosition.column + " row: " + endPosition.row);
            // if the next end position is outside of the board then return false;
            if (endPosition.column >= Constants.numColumns || endPosition.column < 0 || endPosition.row >= Constants.numRows || endPosition.row < 0) {
                //Debug.Log("OUTSIDE OF BOARD Model");
                return false;
            }

            // check for piece at end position, if there is a piece and the piece is not moveable then return false
            if (GetCell(endPosition.column, endPosition.row) != 0) {
                //Debug.Log("Check can move: row: " + endPosition.row + " col: " + endPosition.column + " direction: " + move.direction);                
                switch (move.direction)
                {
                    case Direction.UP:
                        int isMoveableUp = this.isMoveableUp[endPosition.row * Constants.numRows + endPosition.column];
                        //Debug.Log("isMoveableUp: " + isMoveableUp);
                        if (isMoveableUp == 0) {
                            return false;
                        }
                        break;
                    case Direction.DOWN:
                        int isMoveableDown = this.isMoveableDown[endPosition.row * Constants.numRows + endPosition.column];
                        //Debug.Log("isMoveableDown: " + isMoveableDown);
                        if (isMoveableDown == 0) {
                            return false;
                        }
                        break;
                    case Direction.LEFT:
                        int isMoveableLeft = this.isMoveableLeft[endPosition.row * Constants.numRows + endPosition.column];
                        //Debug.Log("isMoveableLeft: " + isMoveableLeft);
                        if (isMoveableLeft == 0) {
                            return false;
                        }
                        break;
                    case Direction.RIGHT:
                        int isMoveableRight = this.isMoveableRight[endPosition.row * Constants.numRows + endPosition.column];
                        //Debug.Log("isMoveableRight: " + isMoveableRight);
                        if (isMoveableRight == 0) {
                            return false;
                        }
                        break;
                    default:
                        break;
                }
                        
                return CanMove(new Move(GetNextPosition(move), move.direction), tokens);
            }

            // if there is a token at the end position and canPassThrough is false then return false
            // MUST CHECK FOR canPassThrough before checking canStopOn
            if (!tokens[endPosition.row, endPosition.column].canPassThrough) {
                Debug.Log("CANT PASS THROUGH");
                return false;
            }
            
            // if there is a token at the end position and canStopOn is false then check if the piece can move
            // to the next position, if not then return false
            if (!tokens[endPosition.row, endPosition.column].canStopOn) {
                Debug.Log("CANT STOP ON");
                return CanMove(new Move(GetNextPosition(move), move.direction), tokens);
            }

            return true;
        }

        public GameBoard Clone ()
        {
            return new GameBoard (numRows, numColumns, numPiecesToWin, piecesCount, board);
        }

        public void PrintBoard(string name) {
            string log = name + "\n";

            for (int row = 0; row < numRows; row++) {
                for (int col = 0; col < numColumns; col++) {
                    log += board[row, col];
                }
                log += "\n";
            }

            for (int row = 0; row < numRows; row++) {
                for (int col = 0; col < numColumns; col++) {
                    log += isMoveableUp[row * numRows + col];
                }
                log += "\n";
            }
            log += "\n";

            for (int row = 0; row < numRows; row++) {
                for (int col = 0; col < numColumns; col++) {
                    log += isMoveableDown[row * numRows + col];
                }
                log += "\n";
            }
            log += "\n";

            for (int row = 0; row < numRows; row++) {
                for (int col = 0; col < numColumns; col++) {
                    log += isMoveableLeft[row * numRows + col];
                }
                log += "\n";
            }
            log += "\n";

            for (int row = 0; row < numRows; row++) {
                for (int col = 0; col < numColumns; col++) {
                    log += isMoveableRight[row * numRows + col];
                }
                log += "\n";
            }
            
            Debug.Log(log);
        }
    }
}
