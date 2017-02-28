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
        GameBoard UpdateBoard(GameBoard board, bool swapPiece);
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
    	
        public GameBoard UpdateBoard(GameBoard board, bool swapPiece)
        {
            Debug.Log("UpToken:UpdateBoard");
            // process next moving piece at gameBoard.activeMovingPieces[0]
            if (board.activeMovingPieces.Count > 0) {
                MovingGamePiece piece = board.activeMovingPieces[0];
                Position currentPosition = piece.GetCurrentPosition();
                Position newPosition = piece.GetNextPosition();
                if (piece.gamePieceObject) {
                    GamePiece gamePiece = piece.gamePieceObject.GetComponent<GamePiece>();
                    gamePiece.column = newPosition.column;
                    gamePiece.row = newPosition.row;
                    board.gamePieces[newPosition.column, newPosition.row] = piece.gamePieceObject;
                    piece.gamePieceObject = null;
                } else {
                    if (swapPiece) {
                        board.SwapPiecePosition(currentPosition, newPosition);    
                    }
                }
//                if (swapPiece) {
//                    board.SwapPiecePosition(piece.GetCurrentPosition(), piece.GetNextPosition());
//                }
                board.activeMovingPieces[0].positions.Add(piece.GetNextPosition());
                piece.currentDirection = Direction.UP;
            } else {
                Debug.LogWarning("Attempting to update gameboard when there is " +
                    "no active moving piece to update", board);
            }

            return board;
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

        public GameBoard UpdateBoard(GameBoard board, bool swapPiece)
        {
            Debug.Log("DownToken:UpdateBoard");
            // process next moving piece at gameBoard.activeMovingPieces[0]
            if (board.activeMovingPieces.Count > 0) {
                MovingGamePiece piece = board.activeMovingPieces[0];
                Position currentPosition = piece.GetCurrentPosition();
                Position newPosition = piece.GetNextPosition();
                if (piece.gamePieceObject) {
                    GamePiece gamePiece = piece.gamePieceObject.GetComponent<GamePiece>();
                    gamePiece.column = newPosition.column;
                    gamePiece.row = newPosition.row;
                    board.gamePieces[newPosition.column, newPosition.row] = piece.gamePieceObject;
                    piece.gamePieceObject = null;
                } else {
                    if (swapPiece) {
                        board.SwapPiecePosition(currentPosition, newPosition);    
                    }
                }
//                if (swapPiece) {
//                    board.SwapPiecePosition(piece.GetCurrentPosition(), piece.GetNextPosition());
//                }
                board.activeMovingPieces[0].positions.Add(piece.GetNextPosition());
                piece.currentDirection = Direction.DOWN;
            } else {
                Debug.LogWarning("Attempting to update gameboard when there is " +
                    "no active moving piece to update", board);
            }

            return board;
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

        public GameBoard UpdateBoard(GameBoard board, bool swapPiece)
        {
            Debug.Log("LeftToken:UpdateBoard");
            // process next moving piece at gameBoard.activeMovingPieces[0]
            if (board.activeMovingPieces.Count > 0) {
                MovingGamePiece piece = board.activeMovingPieces[0];
                Position currentPosition = piece.GetCurrentPosition();
                Position newPosition = piece.GetNextPosition();
                if (piece.gamePieceObject) {
                    GamePiece gamePiece = piece.gamePieceObject.GetComponent<GamePiece>();
                    gamePiece.column = newPosition.column;
                    gamePiece.row = newPosition.row;
                    board.gamePieces[newPosition.column, newPosition.row] = piece.gamePieceObject;
                    piece.gamePieceObject = null;
                } else {
                    if (swapPiece) {
                        board.SwapPiecePosition(currentPosition, newPosition);    
                    }
                }
//                if (swapPiece) {
//                    board.SwapPiecePosition(piece.GetCurrentPosition(), piece.GetNextPosition());    
//                }
                board.activeMovingPieces[0].positions.Add(piece.GetNextPosition());
                piece.currentDirection = Direction.LEFT;
            } else {
                Debug.LogWarning("Attempting to update gameboard when there is " +
                    "no active moving piece to update", board);
            }

            return board;
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

        public GameBoard UpdateBoard(GameBoard board, bool swapPiece)
        {
            Debug.Log("RightToken:UpdateBoard");
            // process next moving piece at gameBoard.activeMovingPieces[0]
            if (board.activeMovingPieces.Count > 0) {
                MovingGamePiece piece = board.activeMovingPieces[0];
                Position currentPosition = piece.GetCurrentPosition();
                Position newPosition = piece.GetNextPosition();
                if (piece.gamePieceObject) {
                    GamePiece gamePiece = piece.gamePieceObject.GetComponent<GamePiece>();
                    gamePiece.column = newPosition.column;
                    gamePiece.row = newPosition.row;
                    board.gamePieces[newPosition.column, newPosition.row] = piece.gamePieceObject;
                    piece.gamePieceObject = null;
                } else {
                    if (swapPiece) {
                        board.SwapPiecePosition(currentPosition, newPosition);    
                    }
                }
//                if (swapPiece) {
//                    board.SwapPiecePosition(piece.GetCurrentPosition(), piece.GetNextPosition());    
//                }
                board.activeMovingPieces[0].positions.Add(piece.GetNextPosition());
                piece.currentDirection = Direction.RIGHT;
            } else {
                Debug.LogWarning("Attempting to update gameboard when there is " +
                    "no active moving piece to update", board);
            }

            return board;
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

        public GameBoard UpdateBoard(GameBoard board, bool swapPiece)
        {
            // process next moving piece at gameBoard.activeMovingPieces[0]
            if (board.activeMovingPieces.Count > 0) {
                MovingGamePiece piece = board.activeMovingPieces[0];
                Position currentPosition = piece.GetCurrentPosition();
                Position newPosition = piece.GetNextPosition();

                board.activeMovingPieces[0].positions.Add(newPosition);
                if (piece.gamePieceObject) {
                    GamePiece gamePiece = piece.gamePieceObject.GetComponent<GamePiece>();
                    gamePiece.column = newPosition.column;
                    gamePiece.row = newPosition.row;
                    board.gamePieces[newPosition.column, newPosition.row] = piece.gamePieceObject;
                    piece.gamePieceObject = null;
                } else {
                    if (swapPiece) {
                        board.SwapPiecePosition(currentPosition, newPosition);    
                    }
                }
//                if (swapPiece) {
//                    board.SwapPiecePosition(currentPosition, newPosition);   
//                }
            } else {
                Debug.LogWarning("Updating gameboard when there is no active moving piece to update", board);
            }

            return board;
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

        public GameBoard UpdateBoard(GameBoard board, bool swapPiece)
        {
            //Debug.Log("StickyToken:UpdateBoard");
            // process next moving piece at gameBoard.activeMovingPieces[0]
            if (board.activeMovingPieces.Count > 0) {
                MovingGamePiece piece = board.activeMovingPieces[0];
                Position nextPosition = piece.GetNextPosition();

                //Debug.Log("tag: " + board.gamePieces[nextPosition.column, nextPosition.row].GetType());
                if (board.gamePieces[nextPosition.column, nextPosition.row]) {
                    //Debug.Log("Add active piece at sticky column: " + nextPosition.column + " row: " + nextPosition.row);
                    MovingGamePiece activeMovingPiece = new MovingGamePiece(nextPosition, piece.currentDirection);

                    GameObject pieceObject = board.gamePieces[nextPosition.column, nextPosition.row];

                    activeMovingPiece.gamePieceObject = pieceObject;
                    board.activeMovingPieces.Add(activeMovingPiece);
                } 

                if (piece.gamePieceObject) {
                    GamePiece gamePiece = piece.gamePieceObject.GetComponent<GamePiece>();
                    gamePiece.column = nextPosition.column;
                    gamePiece.row = nextPosition.row;
                    board.gamePieces[nextPosition.column, nextPosition.row] = piece.gamePieceObject;

                    piece.gamePieceObject = null;
                } else {
                    if (swapPiece) {
                        board.SwapPiecePosition(piece.GetCurrentPosition(), nextPosition);
                        //board.MakePieceMoveable(nextPosition, true);
                    }
                }
                    
                board.activeMovingPieces[0].positions.Add(nextPosition);
                board.DisableNextMovingPiece();
            } else {
                Debug.LogWarning("Attempting to update gameboard when there is " +
                    "no active moving piece to update", board);
            }

            return board;
        }
    }
}