using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy {
    public class TokenBoard {

        public IToken[,] tokens;

        public TokenBoard(int[] tokenData) {
            InitTokenBoard();

            SetTokenBoard(tokenData);
        }

        public TokenBoard (string type) {
            InitTokenBoard();
            //Debug.Log("TOKEN BOARD TYPE: " + type);
            if (type == "ALL") {
                SetTokenBoard(TokenBoardLoader.Instance.FindTokenBoardAll());
            } else if (type == "NOSTICKY") {
                SetTokenBoard(TokenBoardLoader.Instance.FindTokenBoardNoSticky());
            } else if (type == "EMPTY") {
                SetTokenBoard(new int[64]);
            }
    	}
    	
        public void InitTokenBoard() {
            tokens = new IToken[Constants.numColumns, Constants.numRows];

            for(int col = 0; col < Constants.numColumns; col++)
            {
                for(int row = 0; row < Constants.numRows; row++)
                {
                    tokens[col, row] = new EmptyToken();
                }
            }
        }

        public void SetTokenBoard(int[] tokenData) {
            
            for(int col = 0; col < Constants.numColumns; col++)
            {
                for(int row = 0; row < Constants.numRows; row++)
                {
                    int token = tokenData[col * Constants.numColumns + row];
                    //Debug.Log("TOKEN:" + token.ToString());
                    if (token == (int)Token.UP_ARROW)
                    {
                        tokens[col, row] = new UpArrowToken();
                    }
                    else if (token == (int)Token.DOWN_ARROW)
                    {
                        tokens[col, row] = new DownArrowToken();
                    }
                    else if (token == (int)Token.LEFT_ARROW)
                    {
                        tokens[col, row] = new LeftArrowToken();
                    }
                    else if (token == (int)Token.RIGHT_ARROW)
                    {
                        tokens[col, row] = new RightArrowToken();
                    }
                    else if (token == (int)Token.STICKY)
                    {
                        tokens[col, row] = new StickyToken();
                    }
                    else if (token == (int)Token.BLOCKER)
                    {
                        tokens[col, row] = new BlockerToken();
                    }
                }
            }
        }

        public bool CanMove(GameBoardView gameBoard, Move move)
        {
            Position endPosition = move.position;
            //Debug.Log("startPosition:col: " + startPosition.column + " row: " + startPosition.row);
            //Debug.Log("endPosition:col: " + endPosition.column + " row: " + endPosition.row);
            // if the next end position is outside of the board then return false;
            if (endPosition.column >= Constants.numColumns || endPosition.column < 0 || endPosition.row >= Constants.numRows || endPosition.row < 0) {
                Debug.Log("OUTSIDE OF BOARD");
                return false;
            }

            // check for piece at end position if there is a piece and the piece is not moveable then return false
            if (gameBoard.gamePieces[endPosition.column, endPosition.row]) {
                GameObject pieceObject = gameBoard.gamePieces[endPosition.column, endPosition.row];
                GamePiece gamePiece = pieceObject.GetComponent<GamePiece>();

                switch (move.direction)
                {
                    case Direction.UP:
                        if (!gamePiece.isMoveableUp) {
                            return false;
                        }
                        break;
                    case Direction.DOWN:
                        if (!gamePiece.isMoveableDown) {
                            return false;
                        }
                        break;
                    case Direction.LEFT:
                        if (!gamePiece.isMoveableLeft) {
                            return false;
                        }
                        break;
                    case Direction.RIGHT:
                        if (!gamePiece.isMoveableRight) {
                            return false;
                        }
                        break;
                    default:
                        break;
                }
                //Debug.Log("endPosition:col: " + endPosition.column + " row: " + endPosition.row);
                //Debug.Log("next Position:col: " + gamePiece.GetNextPosition(move.direction).column + " row: " + gamePiece.GetNextPosition(move.direction).row);
                return CanMove(gameBoard, new Move(gamePiece.GetNextPosition(move.direction), move.direction));
            }

            // if there is a token at the end position and canPassThrough is true then true
            if (!tokens[endPosition.column, endPosition.row].canPassThrough) {
                Debug.Log("CANT PASS THROUGH");
                return false;
            }

            return true;
        }

        public bool CanMove(GameBoard gameBoard, Move move)
        {
            Position endPosition = move.position;
            //Debug.Log("startPosition:col: " + startPosition.column + " row: " + startPosition.row);
            //Debug.Log("endPosition:col: " + endPosition.column + " row: " + endPosition.row);
            // if the next end position is outside of the board then return false;
            if (endPosition.column >= Constants.numColumns || endPosition.column < 0 || endPosition.row >= Constants.numRows || endPosition.row < 0) {
                Debug.Log("OUTSIDE OF BOARD Model");
                return false;
            }

            // check for piece at end position, if there is a piece and the piece is not moveable then return false
            if (gameBoard.GetCell(endPosition.column, endPosition.row) != 0) {
                //int piece = gameBoard[endPosition.column * Constants.numColumns + endPosition.row];

                switch (move.direction)
                {
                    case Direction.UP:
                        int isMoveableUp = gameBoard.isMoveableUp[endPosition.column * Constants.numColumns + endPosition.row];
                        if (isMoveableUp == 0) {
                            return false;
                        }
                        break;
                    case Direction.DOWN:
                        int isMoveableDown = gameBoard.isMoveableUp[endPosition.column * Constants.numColumns + endPosition.row];
                        if (isMoveableDown == 0) {
                            return false;
                        }
                        break;
                    case Direction.LEFT:
                        int isMoveableLeft = gameBoard.isMoveableUp[endPosition.column * Constants.numColumns + endPosition.row];
                        if (isMoveableLeft == 0) {
                            return false;
                        }
                        break;
                    case Direction.RIGHT:
                        int isMoveableRight = gameBoard.isMoveableUp[endPosition.column * Constants.numColumns + endPosition.row];
                        if (isMoveableRight == 0) {
                            return false;
                        }
                        break;
                    default:
                        break;
                }
                        
                return CanMove(gameBoard, new Move(gameBoard.GetNextPosition(move), move.direction));
            }

            // if there is a token at the end position and canPassThrough is true then true
            if (!tokens[endPosition.column, endPosition.row].canPassThrough) {
                Debug.Log("CANT PASS THROUGH");
                return false;
            }

            return true;
        }

        public void UpdateMoveablePieces(GameBoardView gameBoard) {
            foreach (var piece in gameBoard.completedMovingPieces)
            {
                Position currentPosition = piece.GetCurrentPosition();

                if (tokens[currentPosition.column, currentPosition.row].tokenType == Token.STICKY) {

                    if (CanMove(gameBoard, new Move(piece.GetNextPositionWithDirection(Direction.UP), Direction.UP))) {
                        gameBoard.MakePieceMoveable(currentPosition, true, Direction.UP);
                    } else {
                        gameBoard.MakePieceMoveable(currentPosition, false, Direction.UP);
                    } 
                    if (CanMove(gameBoard, new Move(piece.GetNextPositionWithDirection(Direction.DOWN), Direction.DOWN))) {
                        gameBoard.MakePieceMoveable(currentPosition, true, Direction.DOWN);
                    } else {
                        gameBoard.MakePieceMoveable(currentPosition, false, Direction.DOWN);
                    }
                    if (CanMove(gameBoard, new Move(piece.GetNextPositionWithDirection(Direction.LEFT), Direction.LEFT))) {
                        gameBoard.MakePieceMoveable(currentPosition, true, Direction.LEFT);
                    } else {
                        gameBoard.MakePieceMoveable(currentPosition, false, Direction.LEFT);
                    }
                    if (CanMove(gameBoard, new Move(piece.GetNextPositionWithDirection(Direction.RIGHT), Direction.RIGHT))) {
                        gameBoard.MakePieceMoveable(currentPosition, true, Direction.RIGHT);
                    } else {
                        gameBoard.MakePieceMoveable(currentPosition, false, Direction.RIGHT);
                    }
                } else {
                    gameBoard.MakePieceMoveable(currentPosition, false, Direction.UP);
                    gameBoard.MakePieceMoveable(currentPosition, false, Direction.DOWN);
                    gameBoard.MakePieceMoveable(currentPosition, false, Direction.LEFT);
                    gameBoard.MakePieceMoveable(currentPosition, false, Direction.RIGHT);
                }
            }
        }

        public void UpdateMoveablePieces(GameBoard gameBoard) {
            foreach (var piece in gameBoard.completedMovingPieces)
            {
                Position currentPosition = piece.GetCurrentPosition();
                Debug.Log("UpdateMoveablePieces col: " + currentPosition.column + " row: " + currentPosition.row);
                if (tokens[currentPosition.column, currentPosition.row].tokenType == Token.STICKY) {

                    if (CanMove(gameBoard, new Move(piece.GetNextPositionWithDirection(Direction.UP), Direction.UP))) {
                        gameBoard.MakePieceMoveable(currentPosition, true, Direction.UP);
                    } else {
                        gameBoard.MakePieceMoveable(currentPosition, false, Direction.UP);
                    } 
                    if (CanMove(gameBoard, new Move(piece.GetNextPositionWithDirection(Direction.DOWN), Direction.DOWN))) {
                        gameBoard.MakePieceMoveable(currentPosition, true, Direction.DOWN);
                    } else {
                        gameBoard.MakePieceMoveable(currentPosition, false, Direction.DOWN);
                    }
                    if (CanMove(gameBoard, new Move(piece.GetNextPositionWithDirection(Direction.LEFT), Direction.LEFT))) {
                        gameBoard.MakePieceMoveable(currentPosition, true, Direction.LEFT);
                    } else {
                        gameBoard.MakePieceMoveable(currentPosition, false, Direction.LEFT);
                    }
                    if (CanMove(gameBoard, new Move(piece.GetNextPositionWithDirection(Direction.RIGHT), Direction.RIGHT))) {
                        gameBoard.MakePieceMoveable(currentPosition, true, Direction.RIGHT);
                    } else {
                        gameBoard.MakePieceMoveable(currentPosition, false, Direction.RIGHT);
                    }
                } else {
                    gameBoard.MakePieceMoveable(currentPosition, false, Direction.UP);
                    gameBoard.MakePieceMoveable(currentPosition, false, Direction.DOWN);
                    gameBoard.MakePieceMoveable(currentPosition, false, Direction.LEFT);
                    gameBoard.MakePieceMoveable(currentPosition, false, Direction.RIGHT);
                }
            }
        }
    }
}