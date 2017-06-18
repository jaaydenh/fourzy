using UnityEngine;

namespace Fourzy
{

    public class TokenBoardLoader : MonoBehaviour {

        //Singleton
        private static TokenBoardLoader _instance;
        public static TokenBoardLoader instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<TokenBoardLoader>();
                }
                return _instance;
            }
        }

        public TokenBoard GetTokenBoard() {

            TokenBoard tokenBoard;

            int boardIndex = Random.Range(0, 24);

            switch (boardIndex)
            {
                case 0:
                    tokenBoard = TokenBoard1(true);
                    break;
                case 1:
                    tokenBoard = TokenBoard2(true);
                    break;
                case 2:
                    tokenBoard = TokenBoard3(true);
                    break;
                case 3:
                    tokenBoard = TokenBoard4(true);
                    break;
                case 4:
                    tokenBoard = TokenBoard5(true);
                    break;
                case 5:
                    tokenBoard = TokenBoard6(true);
                    break;
                case 6:
                    tokenBoard = TokenBoard7(true);
                    break;
                case 7:
                    tokenBoard = TokenBoard8(true);
                    break;
                case 8:
                    tokenBoard = TokenBoard9(true);
                    break;
                case 9:
                    tokenBoard = TokenBoard10(true);
                    break;
                case 10:
                    tokenBoard = TokenBoard11(true);
                    break;
                case 11:
                    tokenBoard = TokenBoard12(true);
                    break;
                case 12:
                    tokenBoard = TokenBoard13(true);
                    break;
                case 13:
                    tokenBoard = TokenBoard14(true);
                    break;
                case 14:
                    tokenBoard = TokenBoard15(true);
                    break;
                case 15:
                    tokenBoard = TokenBoard16(true);
                    break;
                case 16:
                    tokenBoard = TokenBoard17(true);
                    break;
                case 17:
                    tokenBoard = TokenBoard18(true);
                    break;
                case 18:
                    tokenBoard = TokenBoard19(true);
                    break;
                case 19:
                    tokenBoard = TokenBoard20(true);
                    break;
                case 20:
                    tokenBoard = TokenBoard21(true);
                    break;
                case 21:
                    tokenBoard = TokenBoard22(true);
                    break;
                case 22:
                    tokenBoard = TokenBoard23(true);
                    break;
                case 23:
                    tokenBoard = TokenBoard24(true);
                    break;
                default:
                    tokenBoard = TokenBoard5(true);
                    break;
            }

            return tokenBoard;
        }

        public TokenBoard GetTokenBoardNoGhost() {

            TokenBoard tokenBoard;
            int boardIndex = Random.Range(0, 16);

            switch (boardIndex)
            {
                case 0:
                    tokenBoard = TokenBoard1(true);
                    break;
                case 1:
                    tokenBoard = TokenBoard2(true);
                    break;
                case 2:
                    tokenBoard = TokenBoard3(true);
                    break;
                case 3:
                    tokenBoard = TokenBoard4(true);
                    break;
                case 4:
                    tokenBoard = TokenBoard5(true);
                    break;
                case 5:
                    tokenBoard = TokenBoard6(true);
                    break;
                case 6:
                    tokenBoard = TokenBoard7(true);
                    break;
                case 7:
                    tokenBoard = TokenBoard8(true);
                    break;
                case 8:
                    tokenBoard = TokenBoard9(true);
                    break;
                case 9:
                    tokenBoard = TokenBoard10(true);
                    break;
                case 10:
                    tokenBoard = TokenBoard11(true);
                    break;
                case 11:
                    tokenBoard = TokenBoard12(true);
                    break;
                case 12:
                    tokenBoard = TokenBoard13(true);
                    break;
                case 13:
                    tokenBoard = TokenBoard14(true);
                    break;
                case 14:
                    tokenBoard = TokenBoard15(true);
                    break;
                case 15:
                    tokenBoard = TokenBoard16(true);
                    break;
                default:
                    tokenBoard = TokenBoard5(true);
                    break;
            }

            return tokenBoard;
        }

        public TokenBoard[] GetAllTokenBoards() {
            TokenBoard[] tokenBoards = new TokenBoard[24];

            TokenBoard tokenBoard1 = TokenBoard1(false);
            tokenBoards[0] = tokenBoard1;

            TokenBoard tokenBoard2 = TokenBoard2(false);
            tokenBoards[1] = tokenBoard2;

            TokenBoard tokenBoard3 = TokenBoard3(false);
            tokenBoards[2] = tokenBoard3;

            TokenBoard tokenBoard4 = TokenBoard4(false);
            tokenBoards[3] = tokenBoard4;

            TokenBoard tokenBoard5 = TokenBoard5(false);
            tokenBoards[4] = tokenBoard5;

            TokenBoard tokenBoard6 = TokenBoard6(false);
            tokenBoards[5] = tokenBoard6;

            TokenBoard tokenBoard7 = TokenBoard7(false);
            tokenBoards[6] = tokenBoard7;

            TokenBoard tokenBoard8 = TokenBoard8(false);
            tokenBoards[7] = tokenBoard8;
            
            TokenBoard tokenBoard9 = TokenBoard9(false);
            tokenBoards[8] = tokenBoard9;

            TokenBoard tokenBoard10 = TokenBoard10(false);
            tokenBoards[9] = tokenBoard10;

            TokenBoard tokenBoard11 = TokenBoard11(false);
            tokenBoards[10] = tokenBoard11;

            TokenBoard tokenBoard12 = TokenBoard12(false);
            tokenBoards[11] = tokenBoard12;

            TokenBoard tokenBoard13 = TokenBoard13(false);
            tokenBoards[12] = tokenBoard13;

            TokenBoard tokenBoard14 = TokenBoard14(false);
            tokenBoards[13] = tokenBoard14;

            TokenBoard tokenBoard15 = TokenBoard15(false);
            tokenBoards[14] = tokenBoard15;

            TokenBoard tokenBoard16 = TokenBoard16(false);
            tokenBoards[15] = tokenBoard16;

            TokenBoard tokenBoard17 = TokenBoard17(false);
            tokenBoards[16] = tokenBoard17;

            TokenBoard tokenBoard18 = TokenBoard18(false);
            tokenBoards[17] = tokenBoard18;

            TokenBoard tokenBoard19 = TokenBoard19(false);
            tokenBoards[18] = tokenBoard19;

            TokenBoard tokenBoard20 = TokenBoard20(false);
            tokenBoards[19] = tokenBoard20;

            TokenBoard tokenBoard21 = TokenBoard21(false);
            tokenBoards[20] = tokenBoard21;

            TokenBoard tokenBoard22 = TokenBoard22(false);
            tokenBoards[21] = tokenBoard22;

            TokenBoard tokenBoard23 = TokenBoard23(false);
            tokenBoards[22] = tokenBoard23;

            TokenBoard tokenBoard24 = TokenBoard24(false);
            tokenBoards[23] = tokenBoard24;

            return tokenBoards;
        }

        public TokenBoard TokenBoard1(bool instantiateTokenBoard) {
            string id = "1000";
            string name = "Simple Spiral";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,4,0,0,0,0 },
                { 0,0,0,0,0,2,0,0 },
                { 0,0,1,0,0,0,0,0 },
                { 0,0,0,0,3,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }
            
        public TokenBoard TokenBoard2(bool instantiateTokenBoard) {
            string id = "1001";
            string name = "Sticky to the Right";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,0,4,0 },
                { 0,0,0,0,0,4,0,0 },
                { 0,0,0,0,4,5,4,0 },
                { 0,0,0,4,5,4,0,0 },
                { 0,0,4,5,4,0,0,0 },
                { 0,4,5,4,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard3(bool instantiateTokenBoard) {
            string id = "1002";
            string name = "Up and Down";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,2,2,2,0 },
                { 0,1,1,1,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard4(bool instantiateTokenBoard) {
            string id = "1003";
            string name = "Empty Center";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,5,5,0,0,0 },
                { 0,0,6,2,2,6,0,0 },
                { 0,0,0,0,0,3,5,0 },
                { 0,0,0,0,0,3,5,0 },
                { 0,0,6,0,0,6,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard5(bool instantiateTokenBoard) {
            string id = "1004";
            string name = "The Basic Game";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard6(bool instantiateTokenBoard) {
            string id = "1005";
            string name = "Left or Right";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,3,0,0,0 },
                { 0,0,0,4,0,0,0,0 },
                { 0,0,0,0,3,0,0,0 },
                { 0,0,0,4,0,0,0,0 },
                { 0,0,0,0,3,0,0,0 },
                { 0,0,0,4,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard7(bool instantiateTokenBoard) {
            string id = "1006";
            string name = "A Sticky Escape";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,2,2,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,4,0,5,5,0,1,0 },
                { 0,4,0,5,5,0,2,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,3,4,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard8(bool instantiateTokenBoard) {
            string id = "1007";
            string name = "Ghost Coated Shuriken";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,7,5,0,0,0 },
                { 0,0,0,7,5,0,0,0 },
                { 0,5,5,5,5,7,7,0 },
                { 0,7,7,5,5,5,5,0 },
                { 0,0,0,5,7,0,0,0 },
                { 0,0,0,5,7,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard9(bool instantiateTokenBoard) {
            string id = "1008";
            string name = "Narrow Miss";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,5,2,0,5,0,0 },
                { 0,0,0,0,0,3,0,0 },
                { 0,0,4,0,0,0,0,0 },
                { 0,0,5,0,1,5,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard10(bool instantiateTokenBoard) {
            string id = "1009";
            string name = "Up or Stuck";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,1,0,0 },
                { 0,0,0,0,1,5,0,0 },
                { 0,0,0,1,5,0,0,0 },
                { 0,0,1,5,0,0,0,0 },
                { 0,0,5,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard11(bool instantiateTokenBoard) {
            string id = "1010";
            string name = "Sticky Donut";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,5,5,5,5,0,0 },
                { 0,0,5,0,0,5,0,0 },
                { 0,0,5,0,0,5,0,0 },
                { 0,0,5,5,5,5,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard12(bool instantiateTokenBoard) {
            string id = "1011";
            string name = "Reversed Sticky X";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,5,0,0,0,0,5,0 },
                { 0,0,5,0,0,5,0,0 },
                { 0,0,0,0,4,0,0,0 },
                { 0,0,0,3,0,0,0,0 },
                { 0,0,5,0,0,5,0,0 },
                { 0,5,0,0,0,0,5,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard13(bool instantiateTokenBoard) {
            string id = "1012";
            string name = "Sticky Corners";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,5,5,0,0,5,5,0 },
                { 0,5,5,0,0,5,5,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,5,5,0,0,5,5,0 },
                { 0,5,5,0,0,5,5,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard14(bool instantiateTokenBoard) {
            string id = "1013";
            string name = "Sticky Checkers";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,5,0,5,0,5,0,0 },
                { 0,0,5,0,5,0,5,0 },
                { 0,5,0,5,0,5,0,0 },
                { 0,0,5,0,5,0,5,0 },
                { 0,5,0,5,0,5,0,0 },
                { 0,0,5,0,5,0,5,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard15(bool instantiateTokenBoard) {
            string id = "1014";
            string name = "Big Sticky Zero";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,5,5,5,5,0,0 },
                { 0,5,0,0,0,0,5,0 },
                { 0,5,0,0,0,0,5,0 },
                { 0,5,0,0,0,0,5,0 },
                { 0,5,0,0,0,0,5,0 },
                { 0,0,5,5,5,5,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard16(bool instantiateTokenBoard) {
            string id = "1015";
            string name = "Big Sticky Block";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,5,5,5,5,0,0 },
                { 0,0,5,6,6,5,0,0 },
                { 0,0,5,6,6,5,0,0 },
                { 0,0,5,5,5,5,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard17(bool instantiateTokenBoard) {
            string id = "1016";
            string name = "Original Ghost";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,7,0,0,7,0,0 },
                { 0,0,0,0,7,0,0,0 },
                { 0,0,0,7,0,0,0,0 },
                { 0,0,7,0,0,7,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard18(bool instantiateTokenBoard) {
            string id = "1017";
            string name = "Ghost Push";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,7,3,4,7,0,0 },
                { 0,0,1,0,0,1,0,0 },
                { 0,0,2,0,0,2,0,0 },
                { 0,0,7,3,4,7,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard19(bool instantiateTokenBoard) {
            string id = "1018";
            string name = "Sticky Sided Ghost";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,7,5,5,7,0,0 },
                { 0,0,5,0,0,5,0,0 },
                { 0,0,5,0,0,5,0,0 },
                { 0,0,7,5,5,7,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard20(bool instantiateTokenBoard) {
            string id = "1019";
            string name = "Big Ghost Zero";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,7,7,0,0,0 },
                { 0,0,7,0,0,7,0,0 },
                { 0,0,7,0,0,7,0,0 },
                { 0,0,0,7,7,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard21(bool instantiateTokenBoard) {
            string id = "1020";
            string name = "Ghost Edge";
            int [,] tokens = new int[8, 8] { 
                { 6,7,7,7,7,7,7,6 },
                { 7,0,0,0,0,0,0,7 },
                { 7,0,0,0,0,0,0,7 },
                { 7,0,0,0,0,0,0,7 },
                { 7,0,0,0,0,0,0,7 },
                { 7,0,0,0,0,0,0,7 },
                { 7,0,0,0,0,0,0,7 },
                { 6,7,7,7,7,7,7,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard22(bool instantiateTokenBoard) {
            string id = "1021";
            string name = "Sticky Filled Ghost Checkers";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,7,0,7,0,7,0 },
                { 0,7,5,7,5,7,0,0 },
                { 0,0,7,5,7,5,7,0 },
                { 0,7,5,7,5,7,0,0 },
                { 0,0,7,5,7,5,7,0 },
                { 0,7,0,7,0,7,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard23(bool instantiateTokenBoard) {
            string id = "1022";
            string name = "Divide by Ghosts";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,0,7,0 },
                { 0,0,0,0,0,7,0,0 },
                { 0,0,0,0,7,0,0,0 },
                { 0,0,0,7,0,0,0,0 },
                { 0,0,7,0,0,0,0,0 },
                { 0,7,0,0,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard24(bool instantiateTokenBoard) {
            string id = "1023";
            string name = "Covered Sticky Stick";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,6,0,0,6,0,0 },
                { 0,6,0,3,4,0,6,0 },
                { 0,0,1,5,5,0,0,0 },
                { 0,0,2,5,5,0,0,0 },
                { 0,6,0,0,0,5,0,0 },
                { 0,0,6,0,0,0,5,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }
    }
}
