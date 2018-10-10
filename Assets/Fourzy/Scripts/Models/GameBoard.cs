using System.Collections.Generic;
using UnityEngine;

namespace Fourzy {
    public class GameBoard {

        public delegate void UpdateTokenBoard(int row, int col, IToken token);
        public static event UpdateTokenBoard OnUpdateTokenBoard;

        private int numRows;
        private int numColumns;
        private int numPiecesToWin;
        private int piecesCount = 0;
        protected int[,] board;
        public List<MovingGamePiece> activeMovingPieces;
        public List<MovingGamePiece> completedMovingPieces;
        public List<Position> player1WinningPositions;
        public List<Position> player2WinningPositions;
        private GameState gameState;

        public GameBoard (GameState gameState, int numRows, int numColumns, int numPiecesToWin) {
            this.gameState = gameState;
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

        public GameBoard (GameState gameState, int numRows, int numColumns, int numPiecesToWin, int piecesCount, int[,] board)
        {
            this.gameState = gameState;
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

            activeMovingPieces = new List<MovingGamePiece>();
            completedMovingPieces = new List<MovingGamePiece>();
            player1WinningPositions = new List<Position>();
            player2WinningPositions = new List<Position>(); 
        }

        public GameBoard(GameBoard ObjectToCopy)
        {
            if (ObjectToCopy == null) return;

            this.gameState = ObjectToCopy.gameState;
            this.numRows = ObjectToCopy.numRows;
            this.numColumns = ObjectToCopy.numColumns;
            this.numPiecesToWin = ObjectToCopy.numPiecesToWin;

            board = new int[numColumns, numRows];
            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numColumns; col++)
                {
                    this.board[row, col] = ObjectToCopy.board[row, col];
                }
            }

            activeMovingPieces = new List<MovingGamePiece>();
            completedMovingPieces = new List<MovingGamePiece>();
            player1WinningPositions = new List<Position>();
            player2WinningPositions = new List<Position>();
        }

        public GameBoard Clone ()
        {
            return new GameBoard (this);
        }

        public void InitGameBoard() {
            for (int row = 0; row < numRows; row++) {
                for (int col = 0; col < numColumns; col++) {
                    board[row, col] = (int)Piece.EMPTY;
                }
            }
        }

        public void SetCell(int col, int row, PlayerEnum player) {
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
                        MovingGamePiece mgp = new MovingGamePiece(new Move(pos, Direction.UP, PlayerEnum.NONE));
                        completedMovingPieces.Add(mgp);
                    }
                }
            }
        }

        public void GetCompletedPieces() {
            for (int row = 0; row < numRows; row++) {
                for (int col = 0; col < numColumns; col++) {
                    if (board[row, col] != (int)Piece.EMPTY) {
                        Position pos = new Position(col, row);
                        MovingGamePiece mgp = new MovingGamePiece(new Move(pos, Direction.UP, PlayerEnum.NONE));
                        completedMovingPieces.Add(mgp);
                    }
                }
            }
        }

        public int[,] GetGameBoard() {
            return board;
        }

        public int[] ToArray() {
            int[] boardArray = new int[numRows * numColumns];
            for (int row = 0; row < numRows; row++) {
                for (int col = 0; col < numColumns; col++) {
                    boardArray[row * numRows + col] = board[row, col];
                }
            }
            return boardArray;
        }

        public int PlayerPieceCount(PlayerEnum player) {
            int pieceCount = 0;
            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numColumns; col++)
                {
                    if (board[row,col] == (int)player) {
                        pieceCount++;
                    }
                }
            }
            return pieceCount;
        }

        public void SwapPiecePosition(Position oldPos, Position newPos) {
            //Debug.Log("OLDPIECE: " + oldPiece + " oldpos.col: " + oldPos.column + " oldpos row: " + oldPos.row);
            //Debug.Log("NEWPIECE: " + oldPiece + " newPos.col: " + newPos.column + " newPos row: " + newPos.row);
            int oldPiece = board[oldPos.row, oldPos.column];
            board[oldPos.row, oldPos.column] = 0;
            board[newPos.row, newPos.column] = oldPiece;
        }

        public void SetActivePieceAsComplete() {


            if (activeMovingPieces.Count > 0) {
                completedMovingPieces.Add(activeMovingPieces[0]);
                activeMovingPieces.RemoveAt(0);
            }
        }

        public void ProcessBoardUpdate(IToken token, bool swapPiece) {
            bool pieceInSquare = false;

            // process next moving piece at gameBoard.activeMovingPieces[0]
            if (!token.canEvaluateWithoutEntering) {
                if (activeMovingPieces.Count > 0) {
                    MovingGamePiece piece = activeMovingPieces[0];
                    Position nextPosition = piece.GetNextPosition();
                    Position currentPosition = piece.GetCurrentPosition();
                    //bool isPieceDestroyed = piece.isDestroyed;

                    if (token.changePieceDirection) {
                        activeMovingPieces[0].positions.Add(piece.GetNextPosition());
                    }

                    if (token.isMoveable) {
                        if (GetCell(nextPosition.column, nextPosition.row) != 0) {
                            pieceInSquare = true;
                            PlayerEnum player = (PlayerEnum)GetCell(nextPosition.column, nextPosition.row);
                            Move move = new Move(nextPosition, piece.currentDirection, player);

                            var newMomentum = 0;
                            if (token.setMomentum > 0) {
                                newMomentum = token.setMomentum;  
                            } else {
                                newMomentum = piece.momentum;
                            }

                            MovingGamePiece activeMovingPiece = new MovingGamePiece(move, newMomentum);
                            activeMovingPieces.Add(activeMovingPiece);
                        }
                    }

                    if (piece.player != PlayerEnum.NONE) {
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
                        if (token.useCurrentDirection) {
                            switch (piece.currentDirection)
                            {
                                case Direction.UP:
                                    if (token.newPieceDirection == Direction.RIGHT)
                                    {
                                        piece.currentDirection = Direction.RIGHT;
                                    }
                                    else if (token.newPieceDirection == Direction.LEFT)
                                    {
                                        piece.currentDirection = Direction.LEFT;
                                    }
                                    break;
                                case Direction.DOWN:
                                    if (token.newPieceDirection == Direction.RIGHT)
                                    {
                                        piece.currentDirection = Direction.LEFT;
                                    }
                                    else if (token.newPieceDirection == Direction.LEFT)
                                    {
                                        piece.currentDirection = Direction.RIGHT;
                                    }
                                    break;
                                case Direction.LEFT:
                                    if (token.newPieceDirection == Direction.RIGHT)
                                    {
                                        piece.currentDirection = Direction.UP;
                                    }
                                    else if (token.newPieceDirection == Direction.LEFT)
                                    {
                                        piece.currentDirection = Direction.DOWN;
                                    }
                                    break;
                                case Direction.RIGHT:
                                    if (token.newPieceDirection == Direction.RIGHT)
                                    {
                                        piece.currentDirection = Direction.DOWN;
                                    }
                                    else if (token.newPieceDirection == Direction.LEFT)
                                    {
                                        piece.currentDirection = Direction.UP;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        } else {
                            piece.currentDirection = token.newPieceDirection;    
                        }
                    }

                    piece.friction += token.addFriction;

                    piece.momentum -= 1;

                    bool isPieceDestroyed = false;

                    if (pieceInSquare || token.pieceMustStopOn || piece.isDestroyed || piece.friction >= 1.0f || piece.momentum <= 0) {

                        if (token.destroyTokenOnEnd)
                        {
                            isPieceDestroyed = CalculatePieceChanceOfDestruction(token.chanceDestroyPieceOnEnd);
                        }

                        if (isPieceDestroyed)
                        {
                            piece.isDestroyed = true;
                            piece.animationState = PieceAnimState.FALLING;

                            if (InBoardBounds(nextPosition))
                            {
                                SetCell(nextPosition.column, nextPosition.row, PlayerEnum.NONE);
                            }
                        }

                        if ((token.tokenType == Token.CIRCLE_BOMB) && isPieceDestroyed)
                        {
                            gameState.SetTokenBoardCell(nextPosition.row, nextPosition.column, new EmptyToken(nextPosition.row, nextPosition.column));

                            if (InBoardBounds(new Position(nextPosition.column - 1, nextPosition.row - 1)))
                            {
                                SetCell(nextPosition.column - 1, nextPosition.row - 1, PlayerEnum.NONE);
                            }
                            if (InBoardBounds(new Position(nextPosition.column, nextPosition.row - 1)))
                            {
                                SetCell(nextPosition.column, nextPosition.row - 1, PlayerEnum.NONE);
                            }
                            if (InBoardBounds(new Position(nextPosition.column + 1, nextPosition.row - 1)))
                            {
                                SetCell(nextPosition.column + 1, nextPosition.row - 1, PlayerEnum.NONE);
                            }
                            if (InBoardBounds(new Position(nextPosition.column - 1, nextPosition.row)))
                            {
                                SetCell(nextPosition.column - 1, nextPosition.row, PlayerEnum.NONE);
                            }
                            if (InBoardBounds(new Position(nextPosition.column + 1, nextPosition.row)))
                            {
                                SetCell(nextPosition.column + 1, nextPosition.row, PlayerEnum.NONE);
                            }
                            if (InBoardBounds(new Position(nextPosition.column - 1, nextPosition.row + 1)))
                            {
                                SetCell(nextPosition.column - 1, nextPosition.row + 1, PlayerEnum.NONE);
                            }
                            if (InBoardBounds(new Position(nextPosition.column, nextPosition.row + 1)))
                            {
                                SetCell(nextPosition.column, nextPosition.row + 1, PlayerEnum.NONE);
                            }
                            if (InBoardBounds(new Position(nextPosition.column + 1, nextPosition.row + 1)))
                            {
                                SetCell(nextPosition.column + 1, nextPosition.row + 1, PlayerEnum.NONE);
                            }
                        }

                        SetActivePieceAsComplete();
                    }

                    if (token.setMomentum > 0)
                    {
                        piece.momentum = token.setMomentum;
                    }

                    if (token.hasEffect) {
                        // Debug.Log("token.hasEffect: token: " + token.tokenType);
                        //if (OnUpdateTokenBoard != null) {
                        //    OnUpdateTokenBoard(nextPosition.row, nextPosition.column, token.replacedToken);    
                        //}
                        if (token.replacedToken != null)
                        {
                            gameState.SetTokenBoardCell(nextPosition.row, nextPosition.column, token.replacedToken);
                        }
                    }
                }
            } else if (token.canEvaluateWithoutEntering) {
                
                if (activeMovingPieces.Count > 0)
                {
                    MovingGamePiece piece = activeMovingPieces[0];

                    if (token.useCurrentDirection)
                    {
                        switch (piece.currentDirection)
                        {
                            case Direction.UP:
                                if (token.newPieceDirection == Direction.REVERSE)
                                {
                                    piece.currentDirection = Direction.DOWN;
                                }
                                break;
                            case Direction.DOWN:
                                if (token.newPieceDirection == Direction.REVERSE)
                                {
                                    piece.currentDirection = Direction.UP;
                                }
                                break;
                            case Direction.LEFT:
                                if (token.newPieceDirection == Direction.REVERSE)
                                {
                                    piece.currentDirection = Direction.RIGHT;
                                }
                                break;
                            case Direction.RIGHT:
                                if (token.newPieceDirection == Direction.REVERSE)
                                {
                                    piece.currentDirection = Direction.LEFT;
                                }
                                break;
                            default:
                                break;
                        }
                    }

                    //if (token.changePieceDirection && token.newPieceDirection == Direction.REVERSE)
                    //{
                    //    activeMovingPieces[0].positions.Add(piece.GetNextPosition());
                    //}

                    //if (piece.player != PlayerEnum.NONE)
                    //{
                    //    SetCell(nextPosition.column, nextPosition.row, piece.player);
                    //    piece.player = 0;
                    //}
                    //else
                    //{
                    //    if (swapPiece)
                    //    {
                    //        SwapPiecePosition(currentPosition, nextPosition);
                    //    }
                    //}
                }
            } 
        }

        public bool InBoardBounds(Position pos) {
            if (pos.column >= 0 && pos.column < numColumns && pos.row >= 0 && pos.row < numRows) {
                return true;
            }
            return false;
        }

        public bool CalculatePieceChanceOfDestruction(float chance) {
            //Debug.Log("Chance Destroyed: " + chance);
            if (chance > 0.0f) {
                return true;
            } else {
                return false;
            }
        }

        public PlayerEnum PlayerAtPosition(Position position) {
            return (PlayerEnum)board[position.row, position.column];
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
            if (board[row, col] != player) { return false; }
            if (board[row + rowShift, col + colShift] != player) { return false; }
            if (board[row + (rowShift * 2), col + (colShift * 2)] != player) { return false; }
            if (board[row + (rowShift * 3), col + (colShift * 3)] != player) { return false; }

            if (player == 1) {
                Debug.Log("add player1WinningPositions");
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
                Debug.Log("add player2WinningPositions");
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

        public string PrintBoard(string name) {
            string log = name + "\n";

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
