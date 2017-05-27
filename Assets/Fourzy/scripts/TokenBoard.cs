using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy {
    public class TokenBoard {

        public string id;
        public string name;
        public IToken[,] tokens;

        public TokenBoard(int[] tokenData, string id, string name) {
            InitTokenBoard();
            this.id = id;
            this.name = name;

            int[,] convertedTokenData = new int [8,8];

            for(int row = 0; row < Constants.numRows; row++)
            {
                for(int col = 0; col < Constants.numColumns; col++)
                {
                    convertedTokenData[row, col] = tokenData[row * Constants.numRows + col];
                }
            }

            SetTokenBoardFromData(convertedTokenData);
        }

        public TokenBoard(int[,] tokenData, string id, string name) {
            InitTokenBoard();
            this.id = id;
            this.name = name;
            SetTokenBoardFromData(tokenData);
        }
    	
        public void InitTokenBoard() {
            tokens = new IToken[Constants.numColumns, Constants.numRows];

            for(int row = 0; row < Constants.numRows; row++)
            {
                for(int col = 0; col < Constants.numColumns; col++)
                {
                    tokens[row, col] = new EmptyToken();
                }
            }
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

        public void SetTokenBoardFromData(int[,] tokenData) {
            
            for(int row = 0; row < Constants.numRows; row++)
            {
                for(int col = 0; col < Constants.numColumns; col++)
                {
                    int token = tokenData[row, col];

                    if (token == (int)Token.UP_ARROW)
                    {
                        tokens[row, col] = new UpArrowToken();
                    }
                    else if (token == (int)Token.DOWN_ARROW)
                    {
                        tokens[row, col] = new DownArrowToken();
                    }
                    else if (token == (int)Token.LEFT_ARROW)
                    {
                        tokens[row, col] = new LeftArrowToken();
                    }
                    else if (token == (int)Token.RIGHT_ARROW)
                    {
                        tokens[row, col] = new RightArrowToken();
                    }
                    else if (token == (int)Token.STICKY)
                    {
                        tokens[row, col] = new StickyToken();
                    }
                    else if (token == (int)Token.BLOCKER)
                    {
                        tokens[row, col] = new BlockerToken();
                    }
                    else if (token == (int)Token.GHOST)
                    {
                        tokens[row, col] = new GhostToken();
                    }
                }
            }
        }
    }
}