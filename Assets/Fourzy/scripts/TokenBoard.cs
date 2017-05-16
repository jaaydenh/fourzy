using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy {
    public class TokenBoard {

        public IToken[,] tokens;

        public TokenBoard(int[] tokenData) {
            InitTokenBoard();
            int[,] convertedTokenData = new int [8,8];

            for(int row = 0; row < Constants.numRows; row++)
            {
                for(int col = 0; col < Constants.numColumns; col++)
                {
                    convertedTokenData[row, col] = tokenData[row * Constants.numRows + col];
                }
            }

            SetTokenBoard(convertedTokenData);
        }

        public TokenBoard (string type) {
            InitTokenBoard();
            //Debug.Log("TOKEN BOARD TYPE: " + type);
            if (type == "ALL") {
                SetTokenBoard(TokenBoardLoader.Instance.FindTokenBoardAll());
            } else if (type == "NOSTICKY") {
                SetTokenBoard(TokenBoardLoader.Instance.FindTokenBoardAll());
            } else if (type == "EMPTY") {
                SetTokenBoard(new int[8,8]);
            }
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

        public void SetTokenBoard(int[,] tokenData) {
            
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
                }
            }
        }
    }
}