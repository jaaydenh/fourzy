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
        public List<MovingGamePiece> activeMovingPieces;
        public List<MovingGamePiece> completedMovingPieces;
        public List<Position> player1WinningPositions;
        public List<Position> player2WinningPositions;

        public GameBoard (int numRows, int numColumns, int numPiecesToWin) {
            this.numRows = numRows;
            this.numColumns = numColumns;
            this.numPiecesToWin = numPiecesToWin;

            board = new int[numColumns, numRows];

            activeMovingPieces = new List<MovingGamePiece>();
            completedMovingPieces = new List<MovingGamePiece>();
            player1WinningPositions = new List<Position>();
            player2WinningPositions = new List<Position>(); 

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

        public void SetGameBoard(int[] boardData) {
            for (int row = 0; row < numRows; row++) {
                for (int col = 0; col < numColumns; col++) {
                    board[row, col] = boardData[row * numRows + col];
                    if (board[row, col] != (int)Piece.EMPTY) {
                        Position pos = new Position(col, row);
                        MovingGamePiece mgp = new MovingGamePiece(new Move(pos, Direction.UP, Player.NONE));
                        completedMovingPieces.Add(mgp);
                    }
                }
            }
        }

        public int[,] GetGameBoard() {
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

        public void ProcessBoardUpdate(IToken token, bool swapPiece) {
            bool pieceInSquare = false;
            // process next moving piece at gameBoard.activeMovingPieces[0]
            if (token.canEnter == true) {
                if (activeMovingPieces.Count > 0) {
                    MovingGamePiece piece = activeMovingPieces[0];
                    Position nextPosition = piece.GetNextPosition();
                    Position currentPosition = piece.GetCurrentPosition();

                    if (token.changePieceDirection) {
                        activeMovingPieces[0].positions.Add(piece.GetNextPosition());
                    }
                    
                    if (token.isMoveable) {
                        if (GetCell(nextPosition.column, nextPosition.row) != 0) {
                            pieceInSquare = true;
                            Player player = (Player)GetCell(nextPosition.column, nextPosition.row);
                            Move move = new Move(nextPosition, piece.currentDirection, player);
                            MovingGamePiece activeMovingPiece = new MovingGamePiece(move);

                            activeMovingPieces.Add(activeMovingPiece);
                        }
                    }

                    if (piece.player != Player.NONE) {
                        SetCell(nextPosition.column, nextPosition.row, piece.player);
                        piece.player = 0;
                    } else {
                        if (swapPiece) {
                            SwapPiecePosition(currentPosition, nextPosition);
                        }
                    }

                    if (!token.changePieceDirection) {
                        activeMovingPieces[0].positions.Add(nextPosition);
                    }

                    if (token.newPieceDirection != Direction.NONE) {
                        piece.currentDirection = token.newPieceDirection;
                    }

                    if (pieceInSquare || token.mustStop) {
                        DisableNextMovingPiece();
                    }
                }
            } 
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

        public bool didPlayerWin(int player) {
            for(var row = 0; row < numRows; row++)
            {
                for(var col = 0; col < numColumns; col++)
                {
                    if (squaresMatchPiece(player, col, row, 1, 0)) { // Check for win in a column
                        return true;
                    } else if (squaresMatchPiece(player, col, row, 0, 1)) { //Check for win in a row
                        return true;
                    } else if (squaresMatchPiece(player, col, row, 1, 1)) { // Check for win in diagonal from upper left to lower right
                        return true;
                    } else if (squaresMatchPiece(player, col, row, 1, -1)) { // Check for win in diagonal from upper right to lower left
                        return true;
                    }
                }
            }
            return false;
        }

        public bool squaresMatchPiece(int player, int col, int row, int rowShift, int colShift) {
            // exit if we can't win from here
            if (row + (rowShift * 3) < 0) { return false; }
            if (row + (rowShift * 3) >= numRows) { return false; }
            if (col + (colShift * 3) < 0) { return false; }
            if (col + (colShift * 3) >= numColumns) { return false; }

            // still here? Check every square
            //var boardPosition = row * numRows + col;
            if (board[row, col] != player) { return false; }
            if (board[row + rowShift, col + colShift] != player) { return false; }
            if (board[row + (rowShift * 2), col + (colShift * 2)] != player) { return false; }
            if (board[row + (rowShift * 3), col + (colShift * 3)] != player) { return false; }

            if (player == 1) {
                player1WinningPositions.Clear();
                player1WinningPositions.Add(new Position(col, row));
                player1WinningPositions.Add(new Position(col + colShift, row + rowShift));
                player1WinningPositions.Add(new Position(col + (colShift * 2), row + (rowShift * 2)));
                player1WinningPositions.Add(new Position(col + (colShift * 3), row + (rowShift * 3)));
                if (row + (rowShift * 4) < numRows && col + (colShift * 4) < numColumns && col + (colShift * 4) >= 0) {
                    if (board[row + (rowShift * 4), col + (colShift * 4)] == player) {
                        player1WinningPositions.Add(new Position(col + (colShift * 4), row + (rowShift * 4)));

                        if (row + (rowShift * 5) < numRows && col + (colShift * 5) < numColumns && col + (colShift * 5) >= 0) {
                            if (board[row + (rowShift * 5), col + (colShift * 5)] == player) {
                                player1WinningPositions.Add(new Position(col + (colShift * 5), row + (rowShift * 5)));

                                if (row + (rowShift * 6) < numRows && col + (colShift * 6) < numColumns && col + (colShift * 6) >= 0) {
                                    if (board[row + (rowShift * 6), col + (colShift * 6)] == player) {
                                        player1WinningPositions.Add(new Position(col + (colShift * 6), row + (rowShift * 6)));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (player == 2) {
                player2WinningPositions.Clear();
                player2WinningPositions.Add(new Position(col, row));
                player2WinningPositions.Add(new Position(col + colShift, row + rowShift));
                player2WinningPositions.Add(new Position(col + (colShift * 2), row + (rowShift * 2)));
                player2WinningPositions.Add(new Position(col + (colShift * 3), row + (rowShift * 3)));
                if (row + (rowShift * 4) < numRows && col + (colShift * 4) < numColumns && col + (colShift * 4) >= 0) {
                    if (board[row + (rowShift * 4), col + (colShift * 4)] == player) {
                        player2WinningPositions.Add(new Position(col + (colShift * 4), row + (rowShift * 4)));

                        if (row + (rowShift * 5) < numRows && col + (colShift * 5) < numColumns && col + (colShift * 5) >= 0) {
                            if (board[row + (rowShift * 5), col + (colShift * 5)] == player) {
                                player2WinningPositions.Add(new Position(col + (colShift * 5), row + (rowShift * 5)));

                                if (row + (rowShift * 6) < numRows && col + (colShift * 6) < numColumns && col + (colShift * 6) >= 0) {
                                    if (board[row + (rowShift * 6), col + (colShift * 6)] == player) {
                                        player2WinningPositions.Add(new Position(col + (colShift * 6), row + (rowShift * 6)));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        // TODO: Need to move this to GameState to also check the tokenboard incase there are tokens on the edge of the board
        public bool hasValidMove() {
            // check the left edge for empty spaces
            for(var row = 1; row < numRows - 1; row++)
            {
                if (board[row, 0] == 0) { return true; }
            }
            // check the right edge for empty spaces
            for(var row = 1; row < numRows - 1; row++)
            {
                if (board[row, numColumns - 1] == 0) { return true; }
            }
            // check the top edge for empty spaces
            for(var col = 1; col < numColumns - 1; col++)
            {
                if (board[0, col] == 0) { return true; }
            }
            // check the bottom edge for empty spaces
            for(var col = 1; col < numColumns - 1; col++)
            {
                if (board[numRows - 1, col] == 0) { return true; }
            }

            return false;
        }

        public GameBoard Clone ()
        {
            return new GameBoard (numRows, numColumns, numPiecesToWin, piecesCount, board);
        }

        public string PrintBoard() {
            string log = "GameBoard" + "\n";

            for (int row = 0; row < numRows; row++) {
                for (int col = 0; col < numColumns; col++) {
                    log += board[row, col];
                }
                log += "\n";
            }
            log += "\n";

            return log;
        }
    }
}
