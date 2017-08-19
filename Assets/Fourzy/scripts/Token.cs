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
        bool isSticky { get; set; }
        bool changePieceDirection { get; set; }
        Direction newPieceDirection { get; set;}
        Token tokenType { get; set; }
        void UpdateBoard(GameBoard board, bool swapPiece);
    }

    public class UpArrowToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canPassThrough { get; set; }
        public bool canStopOn { get; set; }
        public bool isMoveable { get; set; }
        public bool isSticky { get; set; }
        public bool changePieceDirection { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public UpArrowToken() {
            canPassThrough = true;
            canStopOn = true;
            isMoveable = false;
            isSticky = false;
            changePieceDirection = true;
            newPieceDirection = Direction.UP;
            tokenType = Token.UP_ARROW;
    	}

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    public class DownArrowToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canPassThrough { get; set; }
        public bool canStopOn { get; set; }
        public bool isMoveable { get; set; }
        public bool isSticky { get; set; }
        public bool changePieceDirection { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public DownArrowToken() {
            canPassThrough = true;
            canStopOn = true;
            isMoveable = false;
            isSticky = false;
            changePieceDirection = true;
            newPieceDirection = Direction.DOWN;
            tokenType = Token.DOWN_ARROW;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    public class LeftArrowToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canPassThrough { get; set; }
        public bool canStopOn { get; set; }
        public bool isMoveable { get; set; }
        public bool isSticky { get; set; }
        public bool changePieceDirection { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public LeftArrowToken() {
            canPassThrough = true;
            canStopOn = true;
            isMoveable = false;
            isSticky = false;
            changePieceDirection = true;
            newPieceDirection = Direction.LEFT;
            tokenType = Token.LEFT_ARROW;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    public class RightArrowToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canPassThrough { get; set; }
        public bool canStopOn { get; set; }
        public bool isMoveable { get; set; }
        public bool isSticky { get; set; }
        public bool changePieceDirection { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public RightArrowToken() {
            canPassThrough = true;
            canStopOn = true;
            isMoveable = false;
            isSticky = false;
            changePieceDirection = true;
            newPieceDirection = Direction.RIGHT;
            tokenType = Token.RIGHT_ARROW;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    public class EmptyToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canPassThrough { get; set; }
        public bool canStopOn { get; set; }
        public bool isMoveable { get; set; }
        public bool isSticky { get; set; }
        public bool changePieceDirection { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public EmptyToken () {
            canPassThrough = true;
            canStopOn = true;
            isMoveable = false;
            isSticky = false;
            changePieceDirection = false;
            newPieceDirection = Direction.NONE;
            tokenType = Token.EMPTY;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    public class BlockerToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canPassThrough { get; set; }
        public bool canStopOn { get; set; }
        public bool isMoveable { get; set; }
        public bool isSticky { get; set; }
        public bool changePieceDirection { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public BlockerToken () {
            canPassThrough = false;
            canStopOn = false;
            isMoveable = false;
            isSticky = false;
            changePieceDirection = false;
            newPieceDirection = Direction.NONE;
            tokenType = Token.BLOCKER;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            Debug.Log("BLOCKER TOKEN");
            //Do nothing as the blocker token prevents the piece from moving here
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    public class GhostToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canPassThrough { get; set; }
        public bool canStopOn { get; set; }
        public bool isMoveable { get; set; }
        public bool isSticky { get; set; }
        public bool changePieceDirection { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public GhostToken () {
            canPassThrough = true;
            canStopOn = false;
            isMoveable = false;
            isSticky = false;
            changePieceDirection = false;
            newPieceDirection = Direction.NONE;
            tokenType = Token.GHOST;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
        }
    }

    public class StickyToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canPassThrough { get; set; }
        public bool canStopOn { get; set; }
        public bool isMoveable { get; set; }
        public bool isSticky { get; set; }
        public bool changePieceDirection { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public StickyToken() {
            canPassThrough = true;
            canStopOn = true;
            isMoveable = true;
            isSticky = true;
            changePieceDirection = false;
            newPieceDirection = Direction.NONE;
            tokenType = Token.STICKY;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
            // // process next moving piece at gameBoard.activeMovingPieces[0]
            // if (board.activeMovingPieces.Count > 0) {
            //     MovingGamePiece piece = board.activeMovingPieces[0];
            //     Position nextPosition = piece.GetNextPosition();

            //     if (board.GetCell(nextPosition.column, nextPosition.row) != 0) {
            //         Move move = new Move(nextPosition, piece.currentDirection);
            //         MovingGamePiece activeMovingPiece = new MovingGamePiece(move);
            //         int player = board.GetCell(nextPosition.column, nextPosition.row);
            //         activeMovingPiece.player = (Player)player;
            //         board.activeMovingPieces.Add(activeMovingPiece);
            //     } 

            //     if (piece.player != Player.NONE) {
            //         board.SetCell(nextPosition.column, nextPosition.row, piece.player);
            //         piece.player = 0;
            //     } else {
            //         if (swapPiece) {
            //             board.SwapPiecePosition(piece.GetCurrentPosition(), nextPosition);
            //         } 
            //     }

            //     board.activeMovingPieces[0].positions.Add(nextPosition);
            //     // Stop the active piece in the sticky token square
            //     board.DisableNextMovingPiece();
            // }
        }
    }

    public class IceSheetToken : IToken {

        public int Row { get; set; }
        public int Column { get; set; }
        public bool canPassThrough { get; set; }
        public bool canStopOn { get; set; }
        public bool isMoveable { get; set; }
        public bool isSticky { get; set; }
        public bool changePieceDirection { get; set; }
        public Direction newPieceDirection { get; set;}
        public Token tokenType { get; set; }

        public IceSheetToken() {
            canPassThrough = true;
            canStopOn = true;
            isMoveable = true;
            isSticky = false;
            changePieceDirection = false;
            newPieceDirection = Direction.NONE;
            tokenType = Token.ICE_SHEET;
        }

        public void UpdateBoard(GameBoard board, bool swapPiece)
        {
            board.ProcessBoardUpdate(this, swapPiece);
            // bool pieceInSquare = false;

            // // process next moving piece at gameBoard.activeMovingPieces[0]
            // if (board.activeMovingPieces.Count > 0) {
            //     MovingGamePiece piece = board.activeMovingPieces[0];
            //     Position nextPosition = piece.GetNextPosition();
                
            //     if (board.GetCell(nextPosition.column, nextPosition.row) != 0) {
            //         pieceInSquare = true;
            //         Move move = new Move(nextPosition, piece.currentDirection);
            //         MovingGamePiece activeMovingPiece = new MovingGamePiece(move);
            //         int player = board.GetCell(nextPosition.column, nextPosition.row);
            //         activeMovingPiece.player = (Player)player;
            //         board.activeMovingPieces.Add(activeMovingPiece);
            //     }

            //     if (piece.player != Player.NONE) {
            //         board.SetCell(nextPosition.column, nextPosition.row, piece.player);
            //         piece.player = 0;
            //     } else {
            //         if (swapPiece) {
            //             board.SwapPiecePosition(piece.GetCurrentPosition(), nextPosition);
            //         }
            //     }

            //     board.activeMovingPieces[0].positions.Add(nextPosition);
            //     if (pieceInSquare) {
            //         board.DisableNextMovingPiece();
            //     }
            // }
        }
    }
}