using System.Collections.Generic;

namespace Fourzy {
    public class TokenBoard {

        public string id;
        public string name;
        public IToken[,] tokens;
        public int[,] tokenBoard;
        public int[] initialGameBoard;
        public List<MoveInfo> initialMoves;

        public TokenBoard(int[] tokenData, string id, string name, List<MoveInfo> initialMoves, int[] initialGameBoardData, bool instantiateTokenBoard) {
            InitTokenBoard();
            this.id = id;
            this.name = name;
            this.initialMoves = initialMoves;

            //int[,] convertedTokenData = new int [8,8];
            tokenBoard = new int[Constants.numRows, Constants.numColumns];
            //initialGameBoard = new int[Constants.numRows, Constants.numColumns];
            this.initialGameBoard = initialGameBoardData;

            for(int row = 0; row < Constants.numRows; row++)
            {
                for(int col = 0; col < Constants.numColumns; col++)
                {
                    tokenBoard[row, col] = tokenData[row * Constants.numRows + col];
                    //initialGameBoard[row, col] = initialGameBoardData[row * Constants.numRows + col];
                }
            }

            if (instantiateTokenBoard) {
                SetTokenBoardFromData(tokenBoard);
            }
        }

        // Used by Random Board Generator and for setting the mini game boards
        public TokenBoard(int[,] tokenData, string id, string name, List<MoveInfo> initialMoves, int[] initialGameBoardData, bool instantiateTokenBoard) {
            InitTokenBoard();
            this.id = id;
            this.name = name;
            this.initialMoves = initialMoves;
            this.initialGameBoard = initialGameBoardData;

            //initialGameBoard = new int[Constants.numRows, Constants.numColumns];

            //for (int row = 0; row < Constants.numRows; row++)
            //{
            //    for (int col = 0; col < Constants.numColumns; col++)
            //    {
            //        initialGameBoard[row, col] = initialGameBoardData[row * Constants.numRows + col];
            //    }
            //}

            this.tokenBoard = tokenData;
            if (instantiateTokenBoard) {
                SetTokenBoardFromData(tokenData);
            } else {
                this.tokenBoard = tokenData;
            }
        }

        // Only used for Cloning the TokenBoard
        public TokenBoard(IToken[,] tokens, string id, string name, List<MoveInfo> initialMoves) {
            InitTokenBoard();
            this.id = id;
            this.name = name;
            this.initialMoves = initialMoves;

            for (int row = 0; row < Constants.numRows; row++)
            {
                for (int col = 0; col < Constants.numColumns; col++)
                {
                    this.tokens[row, col] = tokens[row, col];
                }
            }
        }

        public void InitTokenBoard() {
            tokens = new IToken[Constants.numColumns, Constants.numRows];

            for(int row = 0; row < Constants.numRows; row++)
            {
                for(int col = 0; col < Constants.numColumns; col++)
                {
                    tokens[row, col] = new EmptyToken(row, col);
                }
            }
        }

        public void RefreshTokenBoard()
        {
            SetTokenBoardFromData(tokenBoard);
        }

        public List<long> GetTokenBoardData() {
            List<long> tokenBoardList = new List<long>();
            for(int row = 0; row < Constants.numRows; row++)
            {
                for(int col = 0; col < Constants.numColumns; col++)
                {
                    tokenBoardList.Add((int)tokens[row, col].tokenType);
                }
            }
            return tokenBoardList;
        }

        public List<long> GetInitialGameBoardData()
        {
            List<long> gameBoardList = new List<long>();
            for (int row = 0; row < Constants.numRows; row++)
            {
                for (int col = 0; col < Constants.numColumns; col++)
                {
                    gameBoardList.Add(initialGameBoard[row * Constants.numRows + col]);
                }
            }
            return gameBoardList;
        }

        public void SetTokenBoardCell(int row, int col, IToken token) {
            tokens[row, col] = token;
        }

        public void SetTokenBoardFromData(int[,] tokenData) {
            
            for(int row = 0; row < Constants.numRows; row++)
            {
                for(int col = 0; col < Constants.numColumns; col++)
                {
                    int token = tokenData[row, col];

                    if (token == (int)Token.UP_ARROW)
                    {
                        tokens[row, col] = new UpArrowToken(row, col);
                    }
                    else if (token == (int)Token.DOWN_ARROW)
                    {
                        tokens[row, col] = new DownArrowToken(row, col);
                    }
                    else if (token == (int)Token.LEFT_ARROW)
                    {
                        tokens[row, col] = new LeftArrowToken(row, col);
                    }
                    else if (token == (int)Token.RIGHT_ARROW)
                    {
                        tokens[row, col] = new RightArrowToken(row, col);
                    }
                    else if (token == (int)Token.STICKY)
                    {
                        tokens[row, col] = new StickyToken(row, col);
                    }
                    else if (token == (int)Token.BLOCKER)
                    {
                        tokens[row, col] = new BlockerToken(row, col);
                    }
                    else if (token == (int)Token.GHOST)
                    {
                        tokens[row, col] = new GhostToken(row, col);
                    }
                    else if (token == (int)Token.ICE_SHEET)
                    {
                        tokens[row, col] = new IceSheetToken(row, col);
                    }
                    else if (token == (int)Token.PIT)
                    {
                        tokens[row, col] = new PitToken(row, col);
                    }
                    else if (token == (int)Token.NINETY_RIGHT_ARROW)
                    {
                        tokens[row, col] = new NinetyRightArrowToken(row, col);
                    }
                    else if (token == (int)Token.NINETY_LEFT_ARROW)
                    {
                        tokens[row, col] = new NinetyLeftArrowToken(row, col);
                    }
                    else if (token == (int)Token.BUMPER)
                    {
                        tokens[row, col] = new BumperToken(row, col);
                    }
                    else if (token == (int)Token.COIN)
                    {
                        tokens[row, col] = new CoinToken(row, col);
                    }
                    else if (token == (int)Token.FRUIT)
                    {
                        tokens[row, col] = new FruitToken(row, col);
                    }
                    else if (token == (int)Token.FRUIT_TREE)
                    {
                        tokens[row, col] = new FruitTreeToken(row, col);
                    }
                    else if (token == (int)Token.WEB)
                    {
                        tokens[row, col] = new WebToken(row, col);
                    }
                    else if (token == (int)Token.SPIDER)
                    {
                        tokens[row, col] = new SpiderToken(row, col);
                    }
                    else if (token == (int)Token.SAND)
                    {
                        tokens[row, col] = new SandToken(row, col);
                    }
                }
            }
        }

        public TokenBoard Clone()
        {
            return new TokenBoard(tokens, id, name, initialMoves);
        }

        public string PrintBoard(string name)
        {
            string log = name + "\n";

            for (int row = 0; row < Constants.numRows; row++)
            {
                for (int col = 0; col < Constants.numColumns; col++)
                {
                    log += tokens[row, col].tokenType;
                    log += " ";
                }
                log += "\n";
            }
            log += "\n";

            return log;
        }

        public string PrintTokenBoardData(string name)
        {
            string log = name + "\n";

            for (int row = 0; row < Constants.numRows; row++)
            {
                for (int col = 0; col < Constants.numColumns; col++)
                {
                    log += tokenBoard[row, col].ToString();
                    log += " ";
                }
                log += "\n";
            }
            log += "\n";

            return log;
        }
    }
}