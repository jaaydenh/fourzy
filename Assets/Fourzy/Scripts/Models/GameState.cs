using System.Collections.Generic;
using UnityEngine;

namespace Fourzy
{
    public class GameState {

        private int numRows;
        private int numColumns;
        private GameBoard previousGameBoard;
        private int[] isMoveableUp;
        private int[] isMoveableDown;
        private int[] isMoveableLeft;
        private int[] isMoveableRight;
        private string randomGuid;
        public GameType GameType { get; private set; }
        // TODO: isCurrentPlayerTurn should not be updated outside of GameState
        public bool isCurrentPlayerTurn;
        public bool IsPlayerOneTurn { get; private set; }
        public bool IsGameOver { get; private set; }
        public bool IsPuzzleChallengePassed { get; private set; }
        public PlayerEnum Winner { get; private set; }
        public GameBoard GameBoard { get; private set; }
        public TokenBoard PreviousTokenBoard { get; private set; }
        public TokenBoard TokenBoard { get; private set; }
        public List<Move> MoveList { get; private set; }
        public int Player1MoveCount { get; private set; }
        public int Player2MoveCount { get; private set; }

        public GameState (int numRows, int numColumns, GameType gameType, bool isPlayerOneTurn, bool isCurrentPlayerTurn, TokenBoard tokenBoard, int[] gameBoardData, bool isGameOver, List<Move> moveList) {
            // Debug.Log("GameState 1");
            this.numRows = numRows;
            this.numColumns = numColumns;
            this.GameType = gameType;
            this.IsPlayerOneTurn = isPlayerOneTurn;
            this.isCurrentPlayerTurn = isCurrentPlayerTurn;
            this.TokenBoard = tokenBoard.Clone();
            this.PreviousTokenBoard = tokenBoard.Clone();
            this.IsGameOver = isGameOver;
            this.MoveList = moveList;
            this.Winner = PlayerEnum.EMPTY;

            Player1MoveCount = 0;
            Player2MoveCount = 0;

            isMoveableUp = new int[numColumns * numRows];
            isMoveableDown = new int[numColumns * numRows];
            isMoveableLeft = new int[numColumns * numRows];
            isMoveableRight = new int[numColumns * numRows];

            InitGameState(gameBoardData);
        }

        public GameState(int numRows, int numColumns, GameType gameType, bool isPlayerOneTurn, bool isCurrentPlayerTurn, bool isGameOver, TokenBoard tokenBoard, PlayerEnum winner, List<Move> moveList, int[] previousGameboardData) {
            // Debug.Log("GameState 2");
            this.numRows = numRows;
            this.numColumns = numColumns;
            this.GameType = gameType;
            this.IsPlayerOneTurn = isPlayerOneTurn;
            this.isCurrentPlayerTurn = isCurrentPlayerTurn;
            this.TokenBoard = tokenBoard.Clone();
            this.PreviousTokenBoard = tokenBoard.Clone();
            this.IsGameOver = isGameOver;
            this.MoveList = moveList;
            this.Winner = winner;

            isMoveableUp = new int[numColumns * numRows];
            isMoveableDown = new int[numColumns * numRows];
            isMoveableLeft = new int[numColumns * numRows];
            isMoveableRight = new int[numColumns * numRows];

            InitGameState(previousGameboardData);
        }

        public GameState Clone()
        {
            return new GameState(numRows, numColumns, GameType, IsPlayerOneTurn, isCurrentPlayerTurn, TokenBoard, GameBoard.ToArray(), IsGameOver, MoveList);
        }

        private void InitGameState(int[] gameBoardData) {
            GameBoard.OnUpdateTokenBoard += SetTokenBoardCell;

            GameBoard = new GameBoard(this, Constants.numRows, Constants.numColumns, Constants.numPiecesToWin);

            InitIsMovable();

            if (gameBoardData != null) {
                if (gameBoardData.Length > 0) {
                    GameBoard.SetGameBoard(gameBoardData);
                }

                UpdateMoveablePieces();
                GameBoard.completedMovingPieces.Clear();
            }
            previousGameBoard = GameBoard.Clone();

            randomGuid = System.Guid.NewGuid().ToString();
        }

        private void InitMoveablePieces() {
            GameBoard.GetCompletedPieces();
            UpdateMoveablePieces();
            GameBoard.completedMovingPieces.Clear();
        }

        public void SetRandomGuid(string guid) {
            randomGuid = guid;
        }

        public int GetRandomNumber(int min, int max, int move = 0, int random_id=0)
        {
            int number = 0;
            int number_space = max - min;
            int window_length = 5;
            int window_start = move % (randomGuid.Length - window_length);

            string window = randomGuid.Substring(window_start, window_length);
            foreach (char c in window)
            {
                number += (c % number_space);
            }

            number += min;
            return number;
        }

        public void SetTokenBoardCell(int row, int col, IToken token) {
            TokenBoard.SetTokenBoardCell(row, col, token);
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
            PlayerEnum curPlayer = IsPlayerOneTurn ? PlayerEnum.ONE : PlayerEnum.TWO;

            List<Move> moves = new List<Move>();
            for (int r = 1; r < numRows - 1; r++)
            {
                //from left to right
                Move m1 = new Move(new Position(-1, r), Direction.RIGHT, curPlayer);
                if (CanMove(m1.GetNextPosition(), TokenBoard.tokens)) { moves.Add(m1); }
                //from right to left
                Move m2 = new Move(new Position(numRows, r), Direction.LEFT, curPlayer);
                if (CanMove(m2.GetNextPosition(), TokenBoard.tokens)) { moves.Add(m2); }
            }
            for (int c = 1; c < numColumns - 1; c++)
            {
                //from up going down
                Move m1 = new Move(new Position(c, -1), Direction.DOWN, curPlayer);
                if (CanMove(m1.GetNextPosition(), TokenBoard.tokens)) { moves.Add(m1); }
                //from down going up
                Move m2 = new Move(new Position(c, numRows), Direction.UP, curPlayer);
                if (CanMove(m2.GetNextPosition(), TokenBoard.tokens)) { moves.Add(m2); }
            }
            return moves;
        }

        // For AI Player
        public List<MovingGamePiece> MovePiece(Move move, bool isReplay)
        {
            List<IToken> activeTokens;

            return MovePiece(move, isReplay, out activeTokens);
        }

        public List<MovingGamePiece> MovePiece(Move move, bool isReplay, out List<IToken> activeTokens) {
            // Debug.Log("MovePiece");
            if (isReplay) {
                GameBoard = previousGameBoard.Clone();
                TokenBoard = PreviousTokenBoard.Clone();
                InitMoveablePieces();
            } else {
                previousGameBoard = GameBoard.Clone();
                PreviousTokenBoard = TokenBoard.Clone();
            }

            GameBoard.completedMovingPieces.Clear();

            MovingGamePiece activeMovingPiece = new MovingGamePiece(move);
            GameBoard.activeMovingPieces.Add(activeMovingPiece);

            TokenBoard.tokens[activeMovingPiece.GetNextPosition().row, activeMovingPiece.GetNextPosition().column].UpdateBoard(GameBoard, false);

            List<IToken> activeTokenList = new List<IToken>();
            while (GameBoard.activeMovingPieces.Count > 0)
            {
                MovingGamePiece activePiece = GameBoard.activeMovingPieces[0];
                Position nextPosition = activePiece.GetNextPosition();
                Direction activeDirection = activePiece.currentDirection;

                if (CanMove(new Move(nextPosition, activeDirection, move.player), TokenBoard.tokens))
                {
                    if (TokenBoard.tokens[nextPosition.row, nextPosition.column].hasEffect)
                    {
                        activeTokenList.Add(TokenBoard.tokens[nextPosition.row, nextPosition.column]);
                    }
                    TokenBoard.tokens[nextPosition.row, nextPosition.column].UpdateBoard(GameBoard, true);
                }
                else
                {
                    bool isPieceDestroyed = false;

                    IToken t = TokenBoard.tokens[activePiece.GetCurrentPosition().row, activePiece.GetCurrentPosition().column];
                    if (t.destroyTokenOnEnd)
                    {
                        isPieceDestroyed = GameBoard.CalculatePieceChanceOfDestruction(t.chanceDestroyPieceOnEnd);
                    }

                    if (isPieceDestroyed)
                    {
                        activePiece.isDestroyed = true;
                        activePiece.animationState = PieceAnimState.FALLING;

                        if (GameBoard.InBoardBounds(nextPosition))
                        {
                            GameBoard.SetCell(activePiece.GetCurrentPosition().column, activePiece.GetCurrentPosition().row, PlayerEnum.NONE);
                        }
                    }

                    if ((t.tokenType == Token.PIT) && isPieceDestroyed)
                    {
                        activeTokenList.Add(TokenBoard.tokens[activePiece.GetCurrentPosition().row, activePiece.GetCurrentPosition().column]);
                        SetTokenBoardCell(activePiece.GetCurrentPosition().row, activePiece.GetCurrentPosition().column, new EmptyToken(activePiece.GetCurrentPosition().row, activePiece.GetCurrentPosition().column));
                    }

                    if ((t.tokenType == Token.CIRCLE_BOMB) && isPieceDestroyed)
                    {
                        if (GameBoard.InBoardBounds(new Position(activePiece.GetCurrentPosition().column - 1, activePiece.GetCurrentPosition().row - 1)))
                        {
                            GameBoard.SetCell(activePiece.GetCurrentPosition().column - 1, activePiece.GetCurrentPosition().row - 1, PlayerEnum.NONE);
                        }
                        if (GameBoard.InBoardBounds(new Position(activePiece.GetCurrentPosition().column, activePiece.GetCurrentPosition().row - 1)))
                        {
                            GameBoard.SetCell(activePiece.GetCurrentPosition().column, activePiece.GetCurrentPosition().row - 1, PlayerEnum.NONE);
                        }
                        if (GameBoard.InBoardBounds(new Position(activePiece.GetCurrentPosition().column + 1, activePiece.GetCurrentPosition().row - 1)))
                        {
                            GameBoard.SetCell(activePiece.GetCurrentPosition().column + 1, activePiece.GetCurrentPosition().row - 1, PlayerEnum.NONE);
                        }
                        if (GameBoard.InBoardBounds(new Position(activePiece.GetCurrentPosition().column - 1, activePiece.GetCurrentPosition().row)))
                        {
                            GameBoard.SetCell(activePiece.GetCurrentPosition().column - 1, activePiece.GetCurrentPosition().row, PlayerEnum.NONE);
                        }
                        if (GameBoard.InBoardBounds(new Position(activePiece.GetCurrentPosition().column + 1, activePiece.GetCurrentPosition().row)))
                        {
                            GameBoard.SetCell(activePiece.GetCurrentPosition().column + 1, activePiece.GetCurrentPosition().row, PlayerEnum.NONE);
                        }
                        if (GameBoard.InBoardBounds(new Position(activePiece.GetCurrentPosition().column - 1, activePiece.GetCurrentPosition().row + 1)))
                        {
                            GameBoard.SetCell(activePiece.GetCurrentPosition().column - 1, activePiece.GetCurrentPosition().row + 1, PlayerEnum.NONE);
                        }
                        if (GameBoard.InBoardBounds(new Position(activePiece.GetCurrentPosition().column, activePiece.GetCurrentPosition().row + 1)))
                        {
                            GameBoard.SetCell(activePiece.GetCurrentPosition().column, activePiece.GetCurrentPosition().row + 1, PlayerEnum.NONE);
                        }
                        if (GameBoard.InBoardBounds(new Position(activePiece.GetCurrentPosition().column + 1, activePiece.GetCurrentPosition().row + 1)))
                        {
                            GameBoard.SetCell(activePiece.GetCurrentPosition().column + 1, activePiece.GetCurrentPosition().row + 1, PlayerEnum.NONE);
                        }

                        activeTokenList.Add(TokenBoard.tokens[activePiece.GetCurrentPosition().row, activePiece.GetCurrentPosition().column]);
                        SetTokenBoardCell(activePiece.GetCurrentPosition().row, activePiece.GetCurrentPosition().column, new EmptyToken(activePiece.GetCurrentPosition().row, activePiece.GetCurrentPosition().column));
                    }

                    GameBoard.SetActivePieceAsComplete();

                }
            }
            activeTokens = activeTokenList;

            if (!isReplay) {
                // System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
                // int currentTime = (int)(System.DateTime.UtcNow - epochStart).TotalMilliseconds;
                move.timeStamp = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
                // move.timeStamp = currentTime;
                // Debug.Log("timstamp: " + move.timeStamp.ToString());
                if (MoveList ==  null) {
                    MoveList = new List<Move>();
                }
                MoveList.Add(move);

                if (IsPlayerOneTurn) {
                    Player1MoveCount++;
                } else {
                    Player2MoveCount++;
                }
            }

            UpdateMoveablePieces();

            if (GameBoard.didPlayerWin(1)) {
                Winner = PlayerEnum.ONE;
                IsGameOver = true;
                isCurrentPlayerTurn = false;
            }
            
            if (GameBoard.didPlayerWin(2)) {
                IsGameOver = true;
                isCurrentPlayerTurn = false;
                if (Winner == PlayerEnum.ONE) {
                    Winner = PlayerEnum.ALL;
                } else {
                    Winner = PlayerEnum.TWO;
                }
            }

            if (IsGameOver == false && GetPossibleMoves().Count <= 0) {
                IsGameOver = true;
                isCurrentPlayerTurn = false;
                Winner = PlayerEnum.NONE;
            }

            if (!isReplay && Winner == PlayerEnum.EMPTY)
            {
                IsPlayerOneTurn = !IsPlayerOneTurn;
            }

            if (GameType == GameType.PUZZLE)
            {
                if (Player1MoveCount >= GameManager.Instance.activeGame.puzzleChallengeInfo.MoveGoal)
                {
                    IsGameOver = true;
                }

                if (IsGameOver)
                {
                    if (Winner == PlayerEnum.ONE)
                    {
                        IsPuzzleChallengePassed = true;
                    }
                    else
                    {
                        IsPuzzleChallengePassed = false;
                    }
                }
            }

            return GameBoard.completedMovingPieces;
        }

        public float GetGameResult(bool isCurrentPlayerPlayerOne) {
            float result = -1;
            switch (Winner)
            {
                case PlayerEnum.ONE:
                    if (isCurrentPlayerPlayerOne) {
                        result = 1;
                    } else {
                        result = 0;
                    }
                    break;
                case PlayerEnum.TWO:
                    if (isCurrentPlayerPlayerOne) {
                        result = 0;
                    } else {
                        result = 1;    
                    }
                    break;
                case PlayerEnum.NONE:
                    result = 0.5f;
                    break;
                case PlayerEnum.ALL:
                    result = 0.5f;
                    break;
                case PlayerEnum.EMPTY:
                    result = -1;
                    break;
                default:
                    break;
            }

            return result;
        }

        public int[,] GetPreviousGameBoard() {
            return  previousGameBoard.GetGameBoard();
        }

        public int[,] GetGameBoard() {
            return GameBoard.GetGameBoard();
        }

        public int[] GetGameBoardArray() {
            return GameBoard.ToArray();
        }

        public List<long> GetGameBoardData() {
            return GameBoard.GetGameBoardData();
        }

        private void UpdateMoveablePieces() {
            foreach (var piece in GameBoard.completedMovingPieces)
            {
                if (!piece.isDestroyed) {
                    Position currentPosition = piece.GetCurrentPosition();
                    if (currentPosition.row > 7 || currentPosition.column < 0 || currentPosition.column > 7 || currentPosition.column < 0) {
                        // Debug.Log("currentPosition.row: " + currentPosition.row);
                        // Debug.Log("currentPosition.column: " + currentPosition.column);
                    }
                    if (TokenBoard.tokens == null) {
                        // Debug.Log("Tokenboard tokens is null");
                    }
                    if (TokenBoard.tokens[currentPosition.row, currentPosition.column].isMoveable) {

                        if (CanMove(new Move(piece.GetNextPositionWithDirection(Direction.UP), Direction.UP, PlayerEnum.NONE), TokenBoard.tokens)) {
                            MakePieceMoveable(currentPosition, true, Direction.UP);
                        } else {
                            MakePieceMoveable(currentPosition, false, Direction.UP);
                        } 
                        if (CanMove(new Move(piece.GetNextPositionWithDirection(Direction.DOWN), Direction.DOWN, PlayerEnum.NONE), TokenBoard.tokens)) {
                            MakePieceMoveable(currentPosition, true, Direction.DOWN);
                        } else {
                            MakePieceMoveable(currentPosition, false, Direction.DOWN);
                        }
                        if (CanMove(new Move(piece.GetNextPositionWithDirection(Direction.LEFT), Direction.LEFT, PlayerEnum.NONE), TokenBoard.tokens)) {
                            MakePieceMoveable(currentPosition, true, Direction.LEFT);
                        } else {
                            MakePieceMoveable(currentPosition, false, Direction.LEFT);
                        }
                        if (CanMove(new Move(piece.GetNextPositionWithDirection(Direction.RIGHT), Direction.RIGHT, PlayerEnum.NONE), TokenBoard.tokens)) {
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
            if (GameBoard.GetCell(endPosition.column, endPosition.row) != 0) {
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
            if (!tokens[endPosition.row, endPosition.column].pieceCanEnter) {
                return false;
            }

            // if there is a token at the end position and canStopOn is false then check if the piece can move
            // to the next position, if not then return false
            if (!tokens[endPosition.row, endPosition.column].pieceCanEndMoveOn) {
                return CanMove(new Move(Utility.GetNextPosition(move), move.direction, move.player), tokens);
            }

            return true;
        }

        public int PlayerPieceCount(PlayerEnum player)
        {
            return GameBoard.PlayerPieceCount(player);
        }

        public void PrintMoveList() {
            int i = 0;

            foreach (var move in MoveList)
            {
                i++;
                int position = move.location;
                Direction direction = move.direction;
                // Debug.Log("Move " + i + ": position: " + position + " direction: " + direction.ToString());
            }
        }

        public int CountFours(int player = -1)
        {
            if (player < 0) player = IsPlayerOneTurn ? 1 : 2;
            int opponent = (player == 1) ? 1 : 2;
            int four_count_for = 0;

            for (var row = 0; row < numRows - 4; row++)
            {
                for (var col = 0; col < numColumns - 4; col++)
                {
                    //right
                    for (var i = 0; i < 4; i++)
                    {
                        if (GameBoard.GetCell(row, col + i) != 0 || GameBoard.GetCell(row, col + i) == opponent)
                        {
                            break;
                        }
                        if (!TokenBoard.tokens[row, col].pieceCanEndMoveOn) break;

                        four_count_for++;
                    }

                    //down
                    for (var i = 0; i < 4; i++)
                    {
                        if (GameBoard.GetCell(row + i, col) != 0 || GameBoard.GetCell(row, col + i) == opponent) break;
                        if (!TokenBoard.tokens[row + i, col].pieceCanEndMoveOn) break;
                        four_count_for++;
                    }

                    //diagonal
                    for (var i = 0; i < 4; i++)
                    {
                        if (GameBoard.GetCell(row + i, col + i) != 0 || GameBoard.GetCell(row, col + i) == opponent) break;
                        if (!TokenBoard.tokens[row + i, col + i].pieceCanEndMoveOn) break;
                        four_count_for++;
                    }
                }
            }

            return four_count_for;
        }

        public int EstimateScore(int player = -1)
        {
            if (player < 0) player = IsPlayerOneTurn ? 1 : 2;
            int opponent = (player == 1) ? 1 : 2;
            int four_count_for = 0;
            int four_count_against = 0;

            for (var row = 0; row < numRows - 4; row++)
            {
                for (var col = 0; col < numColumns - 4; col++)
                {
                    //right
                    for (var i = 0; i < 4; i++)
                    {
                        if (GameBoard.GetCell(row, col + i) != 0 || GameBoard.GetCell(row, col + i) == opponent)
                        {
                            break;
                        }
                        if (!TokenBoard.tokens[row, col].pieceCanEndMoveOn) break;

                        four_count_for++;
                    }

                    //down
                    for (var i = 0; i < 4; i++)
                    {
                        if (GameBoard.GetCell(row + i, col) != 0 || GameBoard.GetCell(row, col + i) == opponent) break;
                        if (!TokenBoard.tokens[row + i, col].pieceCanEndMoveOn) break;
                        four_count_for++;
                    }

                    //diagonal
                    for (var i = 0; i < 4; i++)
                    {
                        if (GameBoard.GetCell(row + i, col + i) != 0 || GameBoard.GetCell(row, col + i) == opponent) break;
                        if (!TokenBoard.tokens[row + i, col + i].pieceCanEndMoveOn) break;
                        four_count_for++;
                    }
                }
            }

            for (var row = 0; row < numRows - 4; row++)
            {
                for (var col = 0; col < numColumns - 4; col++)
                {
                    //right
                    for (var i = 0; i < 4; i++)
                    {
                        if (GameBoard.GetCell(row, col + i) != 0 || GameBoard.GetCell(row, col + i) == opponent)
                        {
                            break;
                        }
                        if (!TokenBoard.tokens[row, col].pieceCanEndMoveOn) break;

                        four_count_against++;
                    }

                    //down
                    for (var i = 0; i < 4; i++)
                    {
                        if (GameBoard.GetCell(row + i, col) != 0 || GameBoard.GetCell(row, col + i) == opponent) break;
                        if (!TokenBoard.tokens[row + i, col].pieceCanEndMoveOn) break;
                        four_count_against++;
                    }

                    //diagonal
                    for (var i = 0; i < 4; i++)
                    {
                        if (GameBoard.GetCell(row + i, col + i) != 0 || GameBoard.GetCell(row, col + i) == opponent) break;
                        if (!TokenBoard.tokens[row + i, col + i].pieceCanEndMoveOn) break;
                        four_count_against++;
                    }
                }
            }

            return ((four_count_for * 5) - (four_count_against * 4));
        }

        public void PrintGameState(string name) {
            string log = name + "\n";

            log += GameBoard.PrintBoard("Gameboard");
            log += previousGameBoard.PrintBoard("PreviousGameboard");
            log += TokenBoard.PrintBoard("TokenBoard");

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