using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy {
    
    public interface IToken 
    {
        int Row { get; set; }
        int Column { get; set; }
        bool canPassThrough { get; set; }
        Token tokenType { get; set; }
        void UpdateBoard(GameBoardView boardView, bool swapPiece);
        void UpdateBoard(GameBoard board, bool swapPiece);
    }
           
    // x o o o o o
    // o s o o o o 
    // x > s o o o
    // o o o o o o
    public class UpArrowToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canPassThrough { get; set; }
        public Token tokenType { get; set; }

        public UpArrowToken() {
            canPassThrough = true;
            tokenType = Token.UP_ARROW;
    	}
    	
        public void UpdateBoard(GameBoardView boardView, bool swapPiece)
        {
            //Debug.Log("UpToken:UpdateBoard");

            // process next moving piece at gameBoard.activeMovingPieces[0]
            if (boardView.activeMovingPieces.Count > 0) {
                MovingGamePiece piece = boardView.activeMovingPieces[0];
                Position currentPosition = piece.GetCurrentPosition();
                Position newPosition = piece.GetNextPosition();
                if (piece.gamePieceObject) {
                    GamePiece gamePiece = piece.gamePieceObject.GetComponent<GamePiece>();
                    gamePiece.column = newPosition.column;
                    gamePiece.row = newPosition.row;
                    boardView.gamePieces[newPosition.row, newPosition.column] = piece.gamePieceObject;
                    piece.gamePieceObject = null;
                } else {
                    if (swapPiece) {
                        boardView.SwapPiecePosition(currentPosition, newPosition);
                    }
                }
                    
                boardView.activeMovingPieces[0].positions.Add(piece.GetNextPosition());

                piece.currentDirection = Direction.UP;
            } else {
                Debug.LogWarning("Attempting to update gameboard when there is " +
                    "no active moving piece to update", boardView);
            }

            //return board;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            //Debug.Log("UpToken:UpdateBoard");

            // process next moving piece at gameBoard.activeMovingPieces[0]
            if (board.activeMovingPieces.Count > 0) {
                MovingGamePiece piece = board.activeMovingPieces[0];
                Position currentPosition = piece.GetCurrentPosition();
                Position newPosition = piece.GetNextPosition();
                if (piece.player != Player.NONE) {
                    board.SetCell(newPosition.column, newPosition.row, piece.player);
                    //board.board[newPosition.column * Constants.numColumns + newPosition.row] = (int)piece.player;
                    piece.player = 0;
                } else {
                    if (swapPiece) {
                        board.SwapPiecePosition(currentPosition, newPosition);
                    }
                }
                
                board.activeMovingPieces[0].positions.Add(piece.GetNextPosition());
                piece.currentDirection = Direction.UP;
            }

            //return board;
        }
    }

    public class DownArrowToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canPassThrough { get; set; }
        public Token tokenType { get; set; }

        public DownArrowToken() {
            canPassThrough = true;
            tokenType = Token.DOWN_ARROW;
        }

        public void UpdateBoard(GameBoardView boardView, bool swapPiece)
        {
            //Debug.Log("DownToken:UpdateBoard");
            // process next moving piece at gameBoard.activeMovingPieces[0]
            if (boardView.activeMovingPieces.Count > 0) {
                MovingGamePiece piece = boardView.activeMovingPieces[0];
                Position currentPosition = piece.GetCurrentPosition();
                Position newPosition = piece.GetNextPosition();
                if (piece.gamePieceObject) {
                    GamePiece gamePiece = piece.gamePieceObject.GetComponent<GamePiece>();
                    gamePiece.column = newPosition.column;
                    gamePiece.row = newPosition.row;
                    boardView.gamePieces[newPosition.row, newPosition.column] = piece.gamePieceObject;
                    piece.gamePieceObject = null;
                } else {
                    if (swapPiece) {
                        boardView.SwapPiecePosition(currentPosition, newPosition);    
                    }
                }

                boardView.activeMovingPieces[0].positions.Add(piece.GetNextPosition());
                piece.currentDirection = Direction.DOWN;
            } else {
                Debug.LogWarning("Attempting to update gameboard when there is " +
                    "no active moving piece to update", boardView);
            }

            //return board;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            // process next moving piece at gameBoard.activeMovingPieces[0]
            if (board.activeMovingPieces.Count > 0) {
                MovingGamePiece piece = board.activeMovingPieces[0];
                Position currentPosition = piece.GetCurrentPosition();
                Position newPosition = piece.GetNextPosition();
                if (piece.player != Player.NONE) {
                    board.SetCell(newPosition.column, newPosition.row, piece.player);
                    //board.board[newPosition.column * Constants.numColumns + newPosition.row] = (int)piece.player;
                    piece.player = 0;
                } else {
                    if (swapPiece) {
                        board.SwapPiecePosition(currentPosition, newPosition);
                    }
                }

                board.activeMovingPieces[0].positions.Add(piece.GetNextPosition());
                piece.currentDirection = Direction.DOWN;
            }

            //return board;
        }
    }

    public class LeftArrowToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canPassThrough { get; set; }
        public Token tokenType { get; set; }

        public LeftArrowToken() {
            canPassThrough = true;
            tokenType = Token.LEFT_ARROW;
        }

        public void UpdateBoard(GameBoardView boardView, bool swapPiece)
        {
            //Debug.Log("LeftToken:UpdateBoard");
            // process next moving piece at gameBoard.activeMovingPieces[0]
            if (boardView.activeMovingPieces.Count > 0) {
                MovingGamePiece piece = boardView.activeMovingPieces[0];
                Position currentPosition = piece.GetCurrentPosition();
                Position newPosition = piece.GetNextPosition();
                if (piece.gamePieceObject) {
                    GamePiece gamePiece = piece.gamePieceObject.GetComponent<GamePiece>();
                    gamePiece.column = newPosition.column;
                    gamePiece.row = newPosition.row;
                    boardView.gamePieces[newPosition.row, newPosition.column] = piece.gamePieceObject;
                    piece.gamePieceObject = null;
                } else {
                    if (swapPiece) {
                        boardView.SwapPiecePosition(currentPosition, newPosition);    
                    }
                }

                boardView.activeMovingPieces[0].positions.Add(piece.GetNextPosition());
                piece.currentDirection = Direction.LEFT;
            } else {
                Debug.LogWarning("Attempting to update gameboard when there is " +
                    "no active moving piece to update", boardView);
            }

            //return board;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            // process next moving piece at gameBoard.activeMovingPieces[0]
            if (board.activeMovingPieces.Count > 0) {
                MovingGamePiece piece = board.activeMovingPieces[0];
                Position currentPosition = piece.GetCurrentPosition();
                Position newPosition = piece.GetNextPosition();
                if (piece.player != Player.NONE) {
                    board.SetCell(newPosition.column, newPosition.row, piece.player);
                    //board.board[newPosition.column * Constants.numColumns + newPosition.row] = (int)piece.player;
                    piece.player = 0;
                } else {
                    if (swapPiece) {
                        board.SwapPiecePosition(currentPosition, newPosition);
                    }
                }

                board.activeMovingPieces[0].positions.Add(piece.GetNextPosition());
                piece.currentDirection = Direction.LEFT;
            }

            //return board;
        }
    }

    public class RightArrowToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canPassThrough { get; set; }
        public Token tokenType { get; set; }

        public RightArrowToken() {
            canPassThrough = true;
            tokenType = Token.RIGHT_ARROW;
        }

        public void UpdateBoard(GameBoardView boardView, bool swapPiece)
        {
            //Debug.Log("RightToken:UpdateBoard");
            // process next moving piece at gameBoard.activeMovingPieces[0]
            if (boardView.activeMovingPieces.Count > 0) {
                MovingGamePiece piece = boardView.activeMovingPieces[0];
                Position currentPosition = piece.GetCurrentPosition();
                Position newPosition = piece.GetNextPosition();
                if (piece.gamePieceObject) {
                    GamePiece gamePiece = piece.gamePieceObject.GetComponent<GamePiece>();
                    gamePiece.column = newPosition.column;
                    gamePiece.row = newPosition.row;
                    boardView.gamePieces[newPosition.row, newPosition.column] = piece.gamePieceObject;
                    piece.gamePieceObject = null;
                } else {
                    if (swapPiece) {
                        boardView.SwapPiecePosition(currentPosition, newPosition);    
                    }
                }

                boardView.activeMovingPieces[0].positions.Add(piece.GetNextPosition());
                piece.currentDirection = Direction.RIGHT;
            } else {
                Debug.LogWarning("Attempting to update gameboard when there is " +
                    "no active moving piece to update", boardView);
            }

            //return board;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            Debug.Log("RIGHT ARROW TOKEN");
            // process next moving piece at gameBoard.activeMovingPieces[0]
            if (board.activeMovingPieces.Count > 0) {
                MovingGamePiece piece = board.activeMovingPieces[0];
                Position currentPosition = piece.GetCurrentPosition();
                Position newPosition = piece.GetNextPosition();
                if (piece.player != Player.NONE) {
                    board.SetCell(newPosition.column, newPosition.row, piece.player);
                    //board.board[newPosition.column * Constants.numColumns + newPosition.row] = (int)piece.player;
                    piece.player = 0;
                } else {
                    if (swapPiece) {
                        board.SwapPiecePosition(currentPosition, newPosition);
                    }
                }

                board.activeMovingPieces[0].positions.Add(piece.GetNextPosition());
                piece.currentDirection = Direction.RIGHT;
            }

            //return board;
        }
    }

    public class EmptyToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canPassThrough { get; set; }
        public Token tokenType { get; set; }

        public EmptyToken () {
            canPassThrough = true;
            tokenType = Token.EMPTY;
        }

        public void UpdateBoard(GameBoardView boardView, bool swapPiece)
        {
            // process next moving piece at gameBoard.activeMovingPieces[0]
            if (boardView.activeMovingPieces.Count > 0) {
                MovingGamePiece piece = boardView.activeMovingPieces[0];
                Position currentPosition = piece.GetCurrentPosition();
                //Debug.Log("view currentposition col: " + currentPosition.column + " row: " + currentPosition.row);
                Position newPosition = piece.GetNextPosition();

                boardView.activeMovingPieces[0].positions.Add(newPosition);
                if (piece.gamePieceObject) {
                    GamePiece gamePiece = piece.gamePieceObject.GetComponent<GamePiece>();
                    gamePiece.column = newPosition.column;
                    gamePiece.row = newPosition.row;
                    boardView.gamePieces[newPosition.row, newPosition.column] = piece.gamePieceObject;
                    piece.gamePieceObject = null;
                } else {
                    if (swapPiece) {
                        boardView.SwapPiecePosition(currentPosition, newPosition);    
                    }
                }
            } else {
                Debug.LogWarning("Updating gameboard when there is no active moving piece to update", boardView);
            }
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            //Debug.Log("EMPTY TOKEN");
            // process next moving piece at gameBoard.activeMovingPieces[0]
            if (board.activeMovingPieces.Count > 0) {
                MovingGamePiece piece = board.activeMovingPieces[0];
                Position currentPosition = piece.GetCurrentPosition();
                Position newPosition = piece.GetNextPosition();
                //Debug.Log("CURRENT PIECE DIRECTION: " + piece.currentDirection);
                //Debug.Log("model currentposition col: " + currentPosition.column + " row: " + currentPosition.row);
                board.activeMovingPieces[0].positions.Add(newPosition);
                if (piece.player != Player.NONE) {
                    board.SetCell(newPosition.column, newPosition.row, piece.player);
                    //board.board[newPosition.column * Constants.numColumns + newPosition.row] = (int)piece.player;
                    piece.player = 0;
                } else {
                    if (swapPiece) {
                        board.SwapPiecePosition(currentPosition, newPosition);
                    }
                }
            }

            //return board;
        }
    }

    public class BlockerToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canPassThrough { get; set; }
        public Token tokenType { get; set; }

        public BlockerToken () {
            canPassThrough = false;
            tokenType = Token.BLOCKER;
        }

        public void UpdateBoard(GameBoardView boardView, bool swapPiece)
        {
            //Do nothing as the blocker token prevents the piece from moving here
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            Debug.Log("BLOCKER TOKEN");
            //Do nothing as the blocker token prevents the piece from moving here
            //return board;
        }
    }

    public class StickyToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canPassThrough { get; set; }
        public Token tokenType { get; set; }

        public StickyToken() {
            canPassThrough = true;
            tokenType = Token.STICKY;
        }

        public void UpdateBoard(GameBoardView boardView, bool swapPiece)
        {
            // process next moving piece at gameBoard.activeMovingPieces[0]
            if (boardView.activeMovingPieces.Count > 0) {
                MovingGamePiece piece = boardView.activeMovingPieces[0];
                Position nextPosition = piece.GetNextPosition();

                if (boardView.gamePieces[nextPosition.row, nextPosition.column]) {
                    //Debug.Log("Add active piece at sticky column: " + nextPosition.column + " row: " + nextPosition.row);
                    Move move = new Move(nextPosition, piece.currentDirection);
                    MovingGamePiece activeMovingPiece = new MovingGamePiece(move);
                    GameObject pieceObject = boardView.gamePieces[nextPosition.row, nextPosition.column];
                    activeMovingPiece.gamePieceObject = pieceObject;
                    boardView.activeMovingPieces.Add(activeMovingPiece);
                } 

                if (piece.gamePieceObject) {
                    GamePiece gamePiece = piece.gamePieceObject.GetComponent<GamePiece>();
                    gamePiece.column = nextPosition.column;
                    gamePiece.row = nextPosition.row;
                    boardView.gamePieces[nextPosition.row, nextPosition.column] = piece.gamePieceObject;

                    piece.gamePieceObject = null;
                } else {
                    if (swapPiece) {
                        boardView.SwapPiecePosition(piece.GetCurrentPosition(), nextPosition);
                    }
                }
                    
                boardView.activeMovingPieces[0].positions.Add(nextPosition);
                boardView.DisableNextMovingPiece();
            } else {
                Debug.LogWarning("Attempting to update gameboard when there is " +
                    "no active moving piece to update", boardView);
            }

            //return board;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            // process next moving piece at gameBoard.activeMovingPieces[0]
            if (board.activeMovingPieces.Count > 0) {
                MovingGamePiece piece = board.activeMovingPieces[0];
                Position nextPosition = piece.GetNextPosition();

                if (board.GetCell(nextPosition.column, nextPosition.row) != 0) {
                    Move move = new Move(nextPosition, piece.currentDirection);
                    MovingGamePiece activeMovingPiece = new MovingGamePiece(move);
                    int player = board.GetCell(nextPosition.column, nextPosition.row);
                    activeMovingPiece.player = (Player)player;
                    board.activeMovingPieces.Add(activeMovingPiece);
                } 

                if (piece.player != Player.NONE) {
                    board.SetCell(nextPosition.column, nextPosition.row, piece.player);
                    //board.board[nextPosition.column * Constants.numColumns + nextPosition.row] = (int)piece.player;
                    piece.player = 0;
                } else {
                    if (swapPiece) {
                        board.SwapPiecePosition(piece.GetCurrentPosition(), nextPosition);
                    }
                }

                board.activeMovingPieces[0].positions.Add(nextPosition);
                board.DisableNextMovingPiece();
            }

            //return board;
        }
    }
}