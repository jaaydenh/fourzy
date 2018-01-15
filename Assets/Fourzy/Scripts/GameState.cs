using System.Collections.Generic;
using UnityEngine;
using GameSparks.Core;

namespace Fourzy
{
    public class GameState {

        private int numRows;
        private int numColumns;
        public int player1MoveCount;
        public int player2MoveCount;
        public bool isCurrentPlayerTurn;
        public bool isPlayerOneTurn;
        public bool isGameOver;
        public PlayerEnum winner = PlayerEnum.EMPTY;
        public GameBoard previousGameBoard;
        public GameBoard gameBoard;
        public TokenBoard tokenBoard;
        public int[] isMoveableUp;
        public int[] isMoveableDown;
        public int[] isMoveableLeft;
        public int[] isMoveableRight;
        public List<GSData> moveList;

        public GameState (int numRows, int numColumns, bool isPlayerOneTurn, bool isCurrentPlayerTurn, TokenBoard tokenBoard, int[] gameBoardData, bool isGameOver, List<GSData> moveList) {
            this.numRows = numRows;
            this.numColumns = numColumns;
            this.isPlayerOneTurn = isPlayerOneTurn;
            this.isCurrentPlayerTurn = isCurrentPlayerTurn;
            this.tokenBoard = tokenBoard;
            this.isGameOver = isGameOver;
            this.moveList = moveList;

            player1MoveCount = 0;
            player2MoveCount = 0;

            isMoveableUp = new int[numColumns * numRows];
            isMoveableDown = new int[numColumns * numRows];
            isMoveableLeft = new int[numColumns * numRows];
            isMoveableRight = new int[numColumns * numRows];

            InitGameState(gameBoardData);
        }

        public GameState(int numRows, int numColumns, GameSparksChallenge challenge, bool isCurrentPlayerTurn, PlayerEnum winner) {
            this.numRows = numRows;
            this.numColumns = numColumns;
            this.isPlayerOneTurn = challenge.isPlayerOneTurn;
            this.isCurrentPlayerTurn = isCurrentPlayerTurn;
            this.tokenBoard = challenge.tokenBoard;
            this.isGameOver = challenge.isGameOver;
            this.moveList = challenge.moveList;
            this.winner = winner;

            isMoveableUp = new int[numColumns * numRows];
            isMoveableDown = new int[numColumns * numRows];
            isMoveableLeft = new int[numColumns * numRows];
            isMoveableRight = new int[numColumns * numRows];

            InitGameState(challenge.previousGameboardData);
        }

        private void InitGameState(int[] gameBoardData) {
            gameBoard = new GameBoard(Constants.numRows, Constants.numColumns, Constants.numPiecesToWin);

            InitIsMovable();

            if (gameBoardData != null) {
                if (gameBoardData.Length > 0) {
                    gameBoard.SetGameBoard(gameBoardData);
                }

                UpdateMoveablePieces();
                gameBoard.completedMovingPieces.Clear();
            }
            previousGameBoard = gameBoard.Clone();
        }

        private void InitMoveablePieces() {
            gameBoard.GetCompletedPieces();
            UpdateMoveablePieces();
            gameBoard.completedMovingPieces.Clear();
        }

        public GameState Clone()
        {
            return new GameState(numRows, numColumns, isPlayerOneTurn, isCurrentPlayerTurn, tokenBoard, gameBoard.ToArray(), isGameOver, moveList);
        }

        private void InitIsMovable() {
            for (int i = 0; i < numColumns * numRows; i++)
            {
                isMoveableUp[i] = 1;
                isMoveableDown[i] = 1;
                isMoveableLeft[i] = 1;
                isMoveableRight[i] = 1;
            }
        }

        public List<Move> GetPossibleMoves()
        {
            PlayerEnum curPlayer = isPlayerOneTurn ? PlayerEnum.ONE : PlayerEnum.TWO;

            List<Move> moves = new List<Move>();
            for (int r = 1; r < numRows - 1; r++)
            {
                //from left to right
                Move m1 = new Move(new Position(-1, r), Direction.RIGHT, curPlayer);
                if (CanMove(m1.GetNextPosition(), tokenBoard.tokens)) { moves.Add(m1); }
                //from right to left
                Move m2 = new Move(new Position(numRows, r), Direction.LEFT, curPlayer);
                if (CanMove(m2.GetNextPosition(), tokenBoard.tokens)) { moves.Add(m2); }
            }
            for (int c = 1; c < numColumns - 1; c++)
            {
                //from up going down
                Move m1 = new Move(new Position(c, -1), Direction.DOWN, curPlayer);
                if (CanMove(m1.GetNextPosition(), tokenBoard.tokens)) { moves.Add(m1); }
                //from down going up
                Move m2 = new Move(new Position(c, numRows), Direction.UP, curPlayer);
                if (CanMove(m2.GetNextPosition(), tokenBoard.tokens)) { moves.Add(m2); }
            }
            return moves;
        }

        public List<MovingGamePiece> MovePiece(Move move, bool isReplay) {
            if (isReplay) {
                gameBoard = previousGameBoard.Clone();
                InitMoveablePieces();
            } else {
                previousGameBoard = gameBoard.Clone();
            }

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
                Dictionary<string, object> moveData = new Dictionary<string, object>();
                moveData.Add("position", move.location);
                moveData.Add("direction", (int)move.direction);
                moveData.Add("player", (int)move.player);
                System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
                int currentTime = (int)(System.DateTime.UtcNow - epochStart).TotalMilliseconds;
                moveData.Add("timestamp", currentTime);
                GSData data = new GSData(moveData);
                if (moveList ==  null) {
                    moveList = new List<GSData>();
                }
                moveList.Add(data);

                if (isPlayerOneTurn) {
                    player1MoveCount++;
                } else {
                    player2MoveCount++;
                }

                //isPlayerOneTurn = !isPlayerOneTurn;
            }

            //PrintMoveList();

            UpdateMoveablePieces();

            if (gameBoard.didPlayerWin(1)) {
                winner = PlayerEnum.ONE;
                isGameOver = true;
                isCurrentPlayerTurn = false;
            }
            
            if (gameBoard.didPlayerWin(2)) {
                isGameOver = true;
                isCurrentPlayerTurn = false;
                if (winner == PlayerEnum.ONE) {
                    winner = PlayerEnum.ALL;
                } else {
                    winner = PlayerEnum.TWO;
                }
            }

            if (!gameBoard.hasValidMove()) {
                isGameOver = true;
                isCurrentPlayerTurn = false;
                winner = PlayerEnum.NONE;
            }
            Debug.Log("GameState MovePiece Winner: " + winner);
            Debug.Log("gameBoard.player1WinningPositions.Count: " + gameBoard.player1WinningPositions.Count);
            Debug.Log("gameBoard.player2WinningPositions.Count: " + gameBoard.player2WinningPositions.Count);

            if (!isReplay && winner == PlayerEnum.EMPTY)
            {
                isPlayerOneTurn = !isPlayerOneTurn;
            }

            return gameBoard.completedMovingPieces;
        }

        public int[,] GetPreviousGameBoard() {
            return  previousGameBoard.GetGameBoard();
        }

        public int[,] GetGameBoard() {
            return gameBoard.GetGameBoard();
        }

        public int[] GetGameBoardArray() {
            return gameBoard.ToArray();
        }

        public List<long> GetGameBoardData() {
            return gameBoard.GetGameBoardData();
        }

        private void UpdateMoveablePieces() {
            foreach (var piece in gameBoard.completedMovingPieces)
            {
                if (!piece.isDestroyed) {
                    Position currentPosition = piece.GetCurrentPosition();
                    if (currentPosition.row > 7 || currentPosition.column < 0 || currentPosition.column > 7 || currentPosition.column < 0) {
                        Debug.Log("currentPosition.row: " + currentPosition.row);
                        Debug.Log("currentPosition.column: " + currentPosition.column);
                    }
                    if (tokenBoard.tokens == null) {
                        Debug.Log("Tokenboard tokens is null");
                    }
                    if (tokenBoard.tokens[currentPosition.row, currentPosition.column].isMoveable) {

                        if (CanMove(new Move(piece.GetNextPositionWithDirection(Direction.UP), Direction.UP, PlayerEnum.NONE), tokenBoard.tokens)) {
                            MakePieceMoveable(currentPosition, true, Direction.UP);
                        } else {
                            MakePieceMoveable(currentPosition, false, Direction.UP);
                        } 
                        if (CanMove(new Move(piece.GetNextPositionWithDirection(Direction.DOWN), Direction.DOWN, PlayerEnum.NONE), tokenBoard.tokens)) {
                            MakePieceMoveable(currentPosition, true, Direction.DOWN);
                        } else {
                            MakePieceMoveable(currentPosition, false, Direction.DOWN);
                        }
                        if (CanMove(new Move(piece.GetNextPositionWithDirection(Direction.LEFT), Direction.LEFT, PlayerEnum.NONE), tokenBoard.tokens)) {
                            MakePieceMoveable(currentPosition, true, Direction.LEFT);
                        } else {
                            MakePieceMoveable(currentPosition, false, Direction.LEFT);
                        }
                        if (CanMove(new Move(piece.GetNextPositionWithDirection(Direction.RIGHT), Direction.RIGHT, PlayerEnum.NONE), tokenBoard.tokens)) {
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

        private void MakePieceMoveable(Position pos, bool moveable, Direction direction) {
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

            if (tokens[endPosition.row, endPosition.column].canEvaluateWithoutEntering)
            {
                return true;
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

        public int PlayerPieceCount(PlayerEnum player)
        {
            return gameBoard.PlayerPieceCount(player);
        }

        public void PrintMoveList() {
            int i = 0;
            Debug.Log("MoveList");
            foreach (var move in moveList)
            {
                i++;
                int position = move.GetInt("position").GetValueOrDefault();
                Direction direction = (Direction)move.GetInt("direction").GetValueOrDefault();
                Debug.Log("Move " + i + ": position: " + position + " direction: " + direction.ToString());
            }
        }

        public void PrintGameState(string name) {
            string log = name + "\n";

            log += gameBoard.PrintBoard("Gameboard");
            log += previousGameBoard.PrintBoard("PreviousGameboard");

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