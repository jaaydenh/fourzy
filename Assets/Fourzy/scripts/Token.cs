using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy {
    
    public interface IToken 
    {
        int Row { get; set; }
        int Column { get; set; }
        bool canPassThrough { get; set; }
        bool canStopOn { get; set; }
        bool isMoveable { get; set; }
        Token tokenType { get; set; }
        void UpdateBoard(GameBoard board, bool swapPiece);
    }

    public class UpArrowToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canPassThrough { get; set; }
        public bool canStopOn { get; set; }
        public bool isMoveable { get; set; }
        public Token tokenType { get; set; }

        public UpArrowToken() {
            canPassThrough = true;
            canStopOn = true;
            isMoveable = false;
            tokenType = Token.UP_ARROW;
    	}

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            // process next moving piece at gameBoard.activeMovingPieces[0]
            if (board.activeMovingPieces.Count > 0) {
                MovingGamePiece piece = board.activeMovingPieces[0];
                Position newPosition = piece.GetNextPosition();
                Position currentPosition = piece.GetCurrentPosition();
                board.activeMovingPieces[0].positions.Add(piece.GetNextPosition());
                if (piece.player != Player.NONE) {
                    board.SetCell(newPosition.column, newPosition.row, piece.player);
                    piece.player = 0;
                } else {
                    if (swapPiece) {
                        board.SwapPiecePosition(currentPosition, newPosition);
                    }
                }
                
                piece.currentDirection = Direction.UP;
            }
        }
    }

    public class DownArrowToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canPassThrough { get; set; }
        public bool canStopOn { get; set; }
        public bool isMoveable { get; set; }
        public Token tokenType { get; set; }

        public DownArrowToken() {
            canPassThrough = true;
            canStopOn = true;
            isMoveable = false;
            tokenType = Token.DOWN_ARROW;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            // process next moving piece at gameBoard.activeMovingPieces[0]
            if (board.activeMovingPieces.Count > 0) {
                MovingGamePiece piece = board.activeMovingPieces[0];
                Position newPosition = piece.GetNextPosition();
                Position currentPosition = piece.GetCurrentPosition();
                board.activeMovingPieces[0].positions.Add(piece.GetNextPosition());
                if (piece.player != Player.NONE) {
                    board.SetCell(newPosition.column, newPosition.row, piece.player);
                    piece.player = 0;
                } else {
                    if (swapPiece) {
                        board.SwapPiecePosition(currentPosition, newPosition);
                    }
                }
                
                piece.currentDirection = Direction.DOWN;
            }
        }
    }

    public class LeftArrowToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canPassThrough { get; set; }
        public bool canStopOn { get; set; }
        public bool isMoveable { get; set; }
        public Token tokenType { get; set; }

        public LeftArrowToken() {
            canPassThrough = true;
            canStopOn = true;
            isMoveable = false;
            tokenType = Token.LEFT_ARROW;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            // process next moving piece at gameBoard.activeMovingPieces[0]
            if (board.activeMovingPieces.Count > 0) {
                MovingGamePiece piece = board.activeMovingPieces[0];
                Position newPosition = piece.GetNextPosition();
                Position currentPosition = piece.GetCurrentPosition();
                board.activeMovingPieces[0].positions.Add(piece.GetNextPosition());
                if (piece.player != Player.NONE) {
                    board.SetCell(newPosition.column, newPosition.row, piece.player);
                    piece.player = 0;
                } else {
                    if (swapPiece) {
                        board.SwapPiecePosition(currentPosition, newPosition);
                    }
                }
                
                piece.currentDirection = Direction.LEFT;
            }
        }
    }

    public class RightArrowToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canPassThrough { get; set; }
        public bool canStopOn { get; set; }
        public bool isMoveable { get; set; }
        public Token tokenType { get; set; }

        public RightArrowToken() {
            canPassThrough = true;
            canStopOn = true;
            isMoveable = false;
            tokenType = Token.RIGHT_ARROW;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            // process next moving piece at gameBoard.activeMovingPieces[0]
            if (board.activeMovingPieces.Count > 0) {
                MovingGamePiece piece = board.activeMovingPieces[0];
                Position newPosition = piece.GetNextPosition();
                Position currentPosition = piece.GetCurrentPosition();
                board.activeMovingPieces[0].positions.Add(piece.GetNextPosition());
                if (piece.player != Player.NONE) {
                    board.SetCell(newPosition.column, newPosition.row, piece.player);
                    piece.player = 0;
                } else {
                    if (swapPiece) {
                        board.SwapPiecePosition(currentPosition, newPosition);
                    }
                }
                
                piece.currentDirection = Direction.RIGHT;
            }
        }
    }

    public class EmptyToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canPassThrough { get; set; }
        public bool canStopOn { get; set; }
        public bool isMoveable { get; set; }
        public Token tokenType { get; set; }

        public EmptyToken () {
            canPassThrough = true;
            canStopOn = true;
            isMoveable = false;
            tokenType = Token.EMPTY;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            // process next moving piece at gameBoard.activeMovingPieces[0]
            if (board.activeMovingPieces.Count > 0) {
                MovingGamePiece piece = board.activeMovingPieces[0];
                Position newPosition = piece.GetNextPosition();
                
                if (piece.player != Player.NONE) {
                    board.SetCell(newPosition.column, newPosition.row, piece.player);
                    piece.player = 0;
                } else {
                    if (swapPiece) {
                        board.SwapPiecePosition(piece.GetCurrentPosition(), newPosition);
                    }
                }

                board.activeMovingPieces[0].positions.Add(newPosition);
            }
        }
    }

    public class BlockerToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canPassThrough { get; set; }
        public bool canStopOn { get; set; }
        public bool isMoveable { get; set; }
        public Token tokenType { get; set; }

        public BlockerToken () {
            canPassThrough = false;
            canStopOn = false;
            isMoveable = false;
            tokenType = Token.BLOCKER;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            Debug.Log("BLOCKER TOKEN");
            //Do nothing as the blocker token prevents the piece from moving here
            //return board;
        }
    }

    public class GhostToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canPassThrough { get; set; }
        public bool canStopOn { get; set; }
        public bool isMoveable { get; set; }
        public Token tokenType { get; set; }

        public GhostToken () {
            canPassThrough = true;
            canStopOn = false;
            isMoveable = false;
            tokenType = Token.GHOST;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            // process next moving piece at gameBoard.activeMovingPieces[0]
            if (board.activeMovingPieces.Count > 0) {
                MovingGamePiece piece = board.activeMovingPieces[0];
                Position newPosition = piece.GetNextPosition();
                
                if (piece.player != Player.NONE) {
                    board.SetCell(newPosition.column, newPosition.row, piece.player);
                    piece.player = 0;
                } else {
                    if (swapPiece) {
                        board.SwapPiecePosition(piece.GetCurrentPosition(), newPosition);
                    }
                }

                board.activeMovingPieces[0].positions.Add(newPosition);
            }
        }
    }

    public class StickyToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canPassThrough { get; set; }
        public bool canStopOn { get; set; }
        public bool isMoveable { get; set; }
        public Token tokenType { get; set; }

        public StickyToken() {
            canPassThrough = true;
            canStopOn = true;
            isMoveable = true;
            tokenType = Token.STICKY;
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
                    piece.player = 0;
                } else {
                    if (swapPiece) {
                        board.SwapPiecePosition(piece.GetCurrentPosition(), nextPosition);
                    } 
                }

                board.activeMovingPieces[0].positions.Add(nextPosition);
                // Stop the active piece in the sticky token square
                board.DisableNextMovingPiece();
            }
        }
    }

    public class IceSheetToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canPassThrough { get; set; }
        public bool canStopOn { get; set; }
        public bool isMoveable { get; set; }
        public Token tokenType { get; set; }

        public IceSheetToken() {
            canPassThrough = true;
            canStopOn = true;
            isMoveable = true;
            tokenType = Token.ICE_SHEET;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            bool pieceInSquare = false;

            // process next moving piece at gameBoard.activeMovingPieces[0]
            if (board.activeMovingPieces.Count > 0) {
                MovingGamePiece piece = board.activeMovingPieces[0];
                Position nextPosition = piece.GetNextPosition();
                
                if (board.GetCell(nextPosition.column, nextPosition.row) != 0) {
                    pieceInSquare = true;
                    Move move = new Move(nextPosition, piece.currentDirection);
                    MovingGamePiece activeMovingPiece = new MovingGamePiece(move);
                    int player = board.GetCell(nextPosition.column, nextPosition.row);
                    activeMovingPiece.player = (Player)player;
                    board.activeMovingPieces.Add(activeMovingPiece);
                }

                if (piece.player != Player.NONE) {
                    board.SetCell(nextPosition.column, nextPosition.row, piece.player);
                    piece.player = 0;
                } else {
                    if (swapPiece) {
                        board.SwapPiecePosition(piece.GetCurrentPosition(), nextPosition);
                    }
                }

                board.activeMovingPieces[0].positions.Add(nextPosition);
                if (pieceInSquare) {
                    board.DisableNextMovingPiece();
                }
            }
        }
    }
}