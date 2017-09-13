using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy
{
    public class GameState {

        private int numRows;
        private int numColumns;
        public bool isCurrentPlayerTurn;
        public bool isPlayerOneTurn;
        public bool isGameOver;
        public Player winner;
        public GameBoard gameBoard;
        public TokenBoard tokenBoard;
        public int[] isMoveableUp;
        public int[] isMoveableDown;
        public int[] isMoveableLeft;
        public int[] isMoveableRight;

        public GameState (int numRows, int numColumns, bool isPlayerOneTurn, bool isCurrentPlayerTurn, TokenBoard tokenBoard, int[] gameBoardData, bool isGameOver) {
            this.numRows = numRows;
            this.numColumns = numColumns;
            this.isPlayerOneTurn = isPlayerOneTurn;
            this.isCurrentPlayerTurn = isCurrentPlayerTurn;
            this.tokenBoard = tokenBoard;
            this.isGameOver = isGameOver;

            isMoveableUp = new int[numColumns * numRows];
            isMoveableDown = new int[numColumns * numRows];
            isMoveableLeft = new int[numColumns * numRows];
            isMoveableRight = new int[numColumns * numRows];

            InitGameState(gameBoardData);
        }

        public void InitGameState(int[] gameBoardData) {

            gameBoard = new GameBoard(Constants.numRows, Constants.numColumns, Constants.numPiecesToWin);
            winner = Player.NONE;

            for (int i = 0; i < numColumns * numRows; i++)
            {
                isMoveableUp[i] = 1;
                isMoveableDown[i] = 1;
                isMoveableLeft[i] = 1;
                isMoveableRight[i] = 1;
            }

            if (gameBoardData != null) {
                if (gameBoardData.Length > 0) {
                    gameBoard.SetGameBoard(gameBoardData);
                }

                UpdateMoveablePieces();
                gameBoard.completedMovingPieces.Clear();
            }
        }

        public List<MovingGamePiece> MovePiece(Move move, bool isReplay) {
            gameBoard.completedMovingPieces.Clear();

            MovingGamePiece activeMovingPiece = new MovingGamePiece(move);
            gameBoard.activeMovingPieces.Add(activeMovingPiece);

            tokenBoard.tokens[activeMovingPiece.GetNextPosition().row, activeMovingPiece.GetNextPosition().column].UpdateBoard(gameBoard, false);

            while (gameBoard.activeMovingPieces.Count > 0) {
                MovingGamePiece activePiece = gameBoard.activeMovingPieces[0];
                Position nextPosition = activePiece.GetNextPosition();
                Direction activeDirection = activePiece.currentDirection;

                if (CanMove(new Move(nextPosition, activeDirection, move.player), tokenBoard.tokens)) {
                    tokenBoard.tokens[nextPosition.row, nextPosition.column].UpdateBoard(gameBoard, true);
                } else {
                    gameBoard.DisableNextMovingPiece();
                }
            }

            if (!isReplay) {
                isPlayerOneTurn = !isPlayerOneTurn;
            }

            UpdateMoveablePieces();

            if (gameBoard.didPlayerWin(1)) {
                winner = Player.ONE;
                isGameOver = true;
            }
            
            if (gameBoard.didPlayerWin(2)) {
                isGameOver = true;
                if (winner == Player.ONE) {
                    winner = Player.ALL;
                } else {
                    winner = Player.TWO;
                }
            }

            if (!gameBoard.hasValidMove()) {
                isGameOver = true;
                winner = Player.NONE;
            }

            return gameBoard.completedMovingPieces;
        }

        public int[,] GetGameBoard() {
            return gameBoard.GetGameBoard();
        }

        public List<long> GetGameBoardData() {
            return gameBoard.GetGameBoardData();
        }
        public void UpdateMoveablePieces() {
            foreach (var piece in gameBoard.completedMovingPieces)
            {
                if (!piece.isDestroyed) {
                    Position currentPosition = piece.GetCurrentPosition();
                    //Debug.Log("UpdateMoveablePieces col: " + currentPosition.column + " row: " + currentPosition.row);
                    if (tokenBoard.tokens[currentPosition.row, currentPosition.column].isMoveable) {

                        if (CanMove(new Move(piece.GetNextPositionWithDirection(Direction.UP), Direction.UP, Player.NONE), tokenBoard.tokens)) {
                            MakePieceMoveable(currentPosition, true, Direction.UP);
                        } else {
                            MakePieceMoveable(currentPosition, false, Direction.UP);
                        } 
                        if (CanMove(new Move(piece.GetNextPositionWithDirection(Direction.DOWN), Direction.DOWN, Player.NONE), tokenBoard.tokens)) {
                            MakePieceMoveable(currentPosition, true, Direction.DOWN);
                        } else {
                            MakePieceMoveable(currentPosition, false, Direction.DOWN);
                        }
                        if (CanMove(new Move(piece.GetNextPositionWithDirection(Direction.LEFT), Direction.LEFT, Player.NONE), tokenBoard.tokens)) {
                            MakePieceMoveable(currentPosition, true, Direction.LEFT);
                        } else {
                            MakePieceMoveable(currentPosition, false, Direction.LEFT);
                        }
                        if (CanMove(new Move(piece.GetNextPositionWithDirection(Direction.RIGHT), Direction.RIGHT, Player.NONE), tokenBoard.tokens)) {
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
        }

        public void MakePieceMoveable(Position pos, bool moveable, Direction direction) {
            //Debug.Log("MakePieceMoveable: " + moveable);
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
            if (gameBoard.GetCell(endPosition.column, endPosition.row) != 0) {
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

                return CanMove(new Move(Utility.GetNextPosition(move), move.direction, move.player), tokens);
            }

            // if there is a token at the end position and canPassThrough is false then return false
            // MUST CHECK FOR canPassThrough before checking canStopOn
            if (!tokens[endPosition.row, endPosition.column].canEnter) {
                return false;
            }

            // if there is a token at the end position and canStopOn is false then check if the piece can move
            // to the next position, if not then return false
            if (!tokens[endPosition.row, endPosition.column].canEndMove) {
                return CanMove(new Move(Utility.GetNextPosition(move), move.direction, move.player), tokens);
            }

            return true;
        }

        // public void ClearMovingPieces() {
        //     gameBoard.completedMovingPieces.Clear();
        // }

        public void PrintGameState(string name) {
            string log = name + "\n";

            log += gameBoard.PrintBoard();

            log += "IsMoveableUp\n";
            for (int row = 0; row < numRows; row++) {
                for (int col = 0; col < numColumns; col++) {
                    log += isMoveableUp[row * numRows + col];
                }
                log += "\n";
            }
            log += "\n";

            log += "IsMoveableDown\n";
            for (int row = 0; row < numRows; row++) {
                for (int col = 0; col < numColumns; col++) {
                    log += isMoveableDown[row * numRows + col];
                }
                log += "\n";
            }
            log += "\n";

            log += "IsMoveableLeft\n";
            for (int row = 0; row < numRows; row++) {
                for (int col = 0; col < numColumns; col++) {
                    log += isMoveableLeft[row * numRows + col];
                }
                log += "\n";
            }
            log += "\n";

            log += "IsMoveableRight\n";
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