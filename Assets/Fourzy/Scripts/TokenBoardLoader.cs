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

            int boardIndex = Random.Range(0, 35);

            switch (boardIndex)
            {
                case 0:
                    tokenBoard = TokenBoard1(true);
                    break;
                case 1:
                    tokenBoard = TokenBoard25(true);
                    break;
                case 2:
                    tokenBoard = TokenBoard26(true);
                    break;
                case 3:
                    tokenBoard = TokenBoard27(true);
                    break;
                case 4:
                    tokenBoard = TokenBoard28(true);
                    break;
                case 5:
                    tokenBoard = TokenBoard29(true);
                    break;
                case 6:
                    tokenBoard = TokenBoard30(true);
                    break;
                case 7:
                    tokenBoard = TokenBoard31(true);
                    break;
                case 8:
                    tokenBoard = TokenBoard32(true);
                    break;
                case 9:
                    tokenBoard = TokenBoard33(true);
                    break;
                case 10:
                    tokenBoard = TokenBoard34(true);
                    break;
                case 11:
                    tokenBoard = TokenBoard35(true);
                    break;
                case 12:
                    tokenBoard = TokenBoard36(true);
                    break;
                case 13:
                    tokenBoard = TokenBoard37(true);
                    break;
                case 14:
                    tokenBoard = TokenBoard38(true);
                    break;
                case 15:
                    tokenBoard = TokenBoard39(true);
                    break;
                case 16:
                    tokenBoard = TokenBoard40(true);
                    break;
                case 17:
                    tokenBoard = TokenBoard41(true);
                    break;
                case 18:
                    tokenBoard = TokenBoard42(true);
                    break;
                case 19:
                    tokenBoard = TokenBoard43(true);
                    break;
                case 20:
                    tokenBoard = TokenBoard44(true);
                    break;
                case 21:
                    tokenBoard = TokenBoard45(true);
                    break;
                case 22:
                    tokenBoard = TokenBoard46(true);
                    break;
                case 23:
                    tokenBoard = TokenBoard47(true);
                    break;
                case 24:
                    tokenBoard = TokenBoard48(true);
                    break;
                case 25:
                    tokenBoard = TokenBoard49(true);
                    break;
                case 26:
                    tokenBoard = TokenBoard50(true);
                    break;
                case 27:
                    tokenBoard = TokenBoard51(true);
                    break;
                case 28:
                    tokenBoard = TokenBoard52(true);
                    break;
                case 29:
                    tokenBoard = TokenBoard53(true);
                    break;
                case 30:
                    tokenBoard = TokenBoard54(true);
                    break;
                case 31:
                    tokenBoard = TokenBoard55(true);
                    break;
                case 32:
                    tokenBoard = TokenBoard56(true);
                    break;
                case 33:
                    tokenBoard = TokenBoard57(true);
                    break;
                case 34:
                    tokenBoard = TokenBoard58(true);
                    break;
                default:
                    tokenBoard = TokenBoard1(true);
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
            TokenBoard[] tokenBoards = new TokenBoard[35];

            TokenBoard tokenBoard1 = TokenBoard1(false);
            tokenBoards[0] = tokenBoard1;

            TokenBoard tokenBoard2 = TokenBoard25(false);
            tokenBoards[1] = tokenBoard2;

            TokenBoard tokenBoard3 = TokenBoard26(false);
            tokenBoards[2] = tokenBoard3;

            TokenBoard tokenBoard4 = TokenBoard27(false);
            tokenBoards[3] = tokenBoard4;

            TokenBoard tokenBoard5 = TokenBoard28(false);
            tokenBoards[4] = tokenBoard5;

            TokenBoard tokenBoard6 = TokenBoard29(false);
            tokenBoards[5] = tokenBoard6;

            TokenBoard tokenBoard7 = TokenBoard30(false);
            tokenBoards[6] = tokenBoard7;

            TokenBoard tokenBoard8 = TokenBoard31(false);
            tokenBoards[7] = tokenBoard8;
            
            TokenBoard tokenBoard9 = TokenBoard32(false);
            tokenBoards[8] = tokenBoard9;

            TokenBoard tokenBoard10 = TokenBoard33(false);
            tokenBoards[9] = tokenBoard10;

            TokenBoard tokenBoard11 = TokenBoard34(false);
            tokenBoards[10] = tokenBoard11;

            TokenBoard tokenBoard12 = TokenBoard35(false);
            tokenBoards[11] = tokenBoard12;

            TokenBoard tokenBoard13 = TokenBoard36(false);
            tokenBoards[12] = tokenBoard13;

            TokenBoard tokenBoard14 = TokenBoard37(false);
            tokenBoards[13] = tokenBoard14;

            TokenBoard tokenBoard15 = TokenBoard38(false);
            tokenBoards[14] = tokenBoard15;

            TokenBoard tokenBoard16 = TokenBoard39(false);
            tokenBoards[15] = tokenBoard16;

            TokenBoard tokenBoard17 = TokenBoard40(false);
            tokenBoards[16] = tokenBoard17;

            TokenBoard tokenBoard18 = TokenBoard41(false);
            tokenBoards[17] = tokenBoard18;

            TokenBoard tokenBoard19 = TokenBoard42(false);
            tokenBoards[18] = tokenBoard19;

            TokenBoard tokenBoard20 = TokenBoard43(false);
            tokenBoards[19] = tokenBoard20;

            TokenBoard tokenBoard21 = TokenBoard44(false);
            tokenBoards[20] = tokenBoard21;

            TokenBoard tokenBoard22 = TokenBoard45(false);
            tokenBoards[21] = tokenBoard22;

            TokenBoard tokenBoard23 = TokenBoard46(false);
            tokenBoards[22] = tokenBoard23;

            TokenBoard tokenBoard24 = TokenBoard47(false);
            tokenBoards[23] = tokenBoard24;

            TokenBoard tokenBoard25 = TokenBoard48(false);
            tokenBoards[24] = tokenBoard25;

            TokenBoard tokenBoard26 = TokenBoard49(false);
            tokenBoards[25] = tokenBoard26;

            TokenBoard tokenBoard27 = TokenBoard50(false);
            tokenBoards[26] = tokenBoard27;

            TokenBoard tokenBoard28 = TokenBoard51(false);
            tokenBoards[27] = tokenBoard28;

            TokenBoard tokenBoard29 = TokenBoard52(false);
            tokenBoards[28] = tokenBoard29;

            TokenBoard tokenBoard30 = TokenBoard53(false);
            tokenBoards[29] = tokenBoard30;

            TokenBoard tokenBoard31 = TokenBoard54(false);
            tokenBoards[30] = tokenBoard31;

            TokenBoard tokenBoard32 = TokenBoard55(false);
            tokenBoards[31] = tokenBoard32;

            TokenBoard tokenBoard33 = TokenBoard56(false);
            tokenBoards[32] = tokenBoard33;

            TokenBoard tokenBoard34 = TokenBoard57(false);
            tokenBoards[33] = tokenBoard34;

            TokenBoard tokenBoard35 = TokenBoard58(false);
            tokenBoards[34] = tokenBoard35;

            return tokenBoards;
        }

        public TokenBoard TokenBoard1(bool instantiateTokenBoard) {
            string id = "1000";
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

        //-------------------------------

        public TokenBoard TokenBoard25(bool instantiateTokenBoard) {
            string id = "1024";
            string name = "GhostSticky Large Four";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,7,0,0,0,0,0 },
                { 0,0,0,0,0,0,7,0 },
                { 0,0,0,5,0,0,0,0 },
                { 0,0,0,0,0,5,0,0 },
                { 0,0,7,0,0,0,0,0 },
                { 0,0,0,0,0,0,7,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard26(bool instantiateTokenBoard) {
            string id = "1025";
            string name = "Ghosts Diagonal";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,7,0,0,0,0 },
                { 0,0,0,0,0,7,0,0 },
                { 0,0,7,0,0,0,0,0 },
                { 0,0,0,0,7,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard27(bool instantiateTokenBoard) {
            string id = "1026";
            string name = "GhostSticky Diagonal Cross";
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

        public TokenBoard TokenBoard28(bool instantiateTokenBoard) {
            string id = "1027";
            string name = "GhostSticky Hanging Light";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,7,7,0,0,6 },
                { 0,0,0,7,7,0,0,0 },
                { 0,0,0,7,7,0,0,0 },
                { 0,0,0,5,5,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard29(bool instantiateTokenBoard) {
            string id = "1028";
            string name = "GhostSticky Up Down";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,7,0,0,0,6 },
                { 0,0,0,7,0,0,0,0 },
                { 0,0,0,7,0,0,0,0 },
                { 0,0,0,5,0,0,0,0 },
                { 0,0,0,0,5,0,0,0 },
                { 0,0,0,0,7,0,0,0 },
                { 0,0,0,0,7,0,0,0 },
                { 6,0,0,0,7,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard30(bool instantiateTokenBoard) {
            string id = "1029";
            string name = "GhostSticky in the Middle";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,7,5,7,0,0 },
                { 0,0,0,0,5,0,0,0 },
                { 0,0,0,7,5,7,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard31(bool instantiateTokenBoard) {
            string id = "1030";
            string name = "Ghost Circle";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,7,0,7,0,0,0 },
                { 0,0,0,0,0,0,7,0 },
                { 0,7,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,7,0 },
                { 0,7,0,0,0,0,0,0 },
                { 0,0,0,7,0,7,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard32(bool instantiateTokenBoard) {
            string id = "1031";
            string name = "Ghost Circle With Center";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,7,0,7,0,0,0 },
                { 0,0,0,0,0,0,7,0 },
                { 0,7,0,5,5,0,0,0 },
                { 0,0,0,5,5,0,7,0 },
                { 0,7,0,0,0,0,0,0 },
                { 0,0,0,7,0,7,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard33(bool instantiateTokenBoard) {
            string id = "1032";
            string name = "SkyScraper";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,5,5,0,0,0 },
                { 7,7,5,5,5,5,7,7 },
                { 7,7,5,5,5,5,7,7 },
                { 0,0,5,5,5,5,0,0 },
                { 0,0,5,5,5,5,0,0 },
                { 0,0,5,5,5,5,0,0 },
                { 6,0,5,5,5,5,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard34(bool instantiateTokenBoard) {
            string id = "1033";
            string name = "Center Three";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,7,5,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,5,7,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,7,5,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard35(bool instantiateTokenBoard) {
            string id = "1034";
            string name = "Sticky Double Diagonal";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,5,0,0,0,0 },
                { 0,0,0,0,5,0,0,0 },
                { 0,5,0,0,0,5,0,0 },
                { 0,0,5,0,0,0,5,0 },
                { 0,0,0,5,0,0,0,0 },
                { 0,0,0,0,5,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard36(bool instantiateTokenBoard) {
            string id = "1035";
            string name = "Center Star";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,5,0,0,0 },
                { 0,0,0,5,6,5,0,0 },
                { 0,0,0,0,5,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard37(bool instantiateTokenBoard) {
            string id = "1036";
            string name = "Arrow Sticky Down Up";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,0,0,0 },
                { 0,3,5,3,0,0,0,0 },
                { 0,0,3,5,0,0,0,0 },
                { 0,0,0,0,5,2,0,0 },
                { 0,0,0,0,2,5,2,0 },
                { 0,0,0,0,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard38(bool instantiateTokenBoard) {
            string id = "1037";
            string name = "Big Sticky Arrow";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,5,0,0,0 },
                { 0,0,0,0,5,5,0,0 },
                { 0,5,5,5,5,5,5,0 },
                { 0,5,5,5,5,5,5,0 },
                { 0,0,0,0,5,5,0,0 },
                { 0,0,0,0,5,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard39(bool instantiateTokenBoard) {
            string id = "1038";
            string name = "Sticky Four";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,5,0,0,5,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,5,0,0,5,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard40(bool instantiateTokenBoard) {
            string id = "1039";
            string name = "Sticky Four Askew";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,5,0,0 },
                { 0,0,0,5,0,0,0,0 },
                { 0,0,0,0,5,0,0,0 },
                { 0,0,5,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard41(bool instantiateTokenBoard) {
            string id = "1040";
            string name = "Smile";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,7,7,0,0,7,7,0 },
                { 0,0,1,0,0,1,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,5,0,0,0,0,5,0 },
                { 0,5,5,5,5,5,5,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard42(bool instantiateTokenBoard) {
            string id = "1041";
            string name = "Big Sticky Square";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,5,5,5,5,5,5,0 },
                { 0,5,5,5,5,5,5,0 },
                { 0,5,5,5,5,5,5,0 },
                { 0,5,5,5,5,5,5,0 },
                { 0,5,5,5,5,5,5,0 },
                { 0,5,5,5,5,5,5,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard43(bool instantiateTokenBoard) {
            string id = "1042";
            string name = "Sticky Wedge";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,0,5,0 },
                { 0,0,0,0,0,5,5,0 },
                { 0,0,0,0,5,5,5,0 },
                { 0,0,0,5,5,5,5,0 },
                { 0,0,5,5,5,5,0,0 },
                { 0,5,5,5,5,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard44(bool instantiateTokenBoard) {
            string id = "1043";
            string name = "Sticky Diamond";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,5,5,0,0,0 },
                { 0,0,5,5,5,5,0,0 },
                { 0,5,5,5,5,5,5,0 },
                { 0,5,5,5,5,5,5,0 },
                { 0,0,5,5,5,5,0,0 },
                { 0,0,0,5,5,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard45(bool instantiateTokenBoard) {
            string id = "1044";
            string name = "Arrows";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,2,0,0,0,0 },
                { 0,0,0,0,0,3,0,0 },
                { 0,0,4,0,0,0,0,0 },
                { 0,0,0,0,1,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard46(bool instantiateTokenBoard) {
            string id = "1045";
            string name = "Diagonal Arrow Channels";
            int [,] tokens = new int[8, 8] { 
                { 6,4,1,2,3,4,1,6 },
                { 4,1,2,3,4,1,2,3 },
                { 1,2,3,4,1,2,3,4 },
                { 2,3,4,1,2,3,4,1 },
                { 3,4,1,2,3,4,1,2 },
                { 4,1,2,3,4,1,2,3 },
                { 1,2,3,4,1,2,3,4 },
                { 6,3,4,1,2,3,4,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard47(bool instantiateTokenBoard) {
            string id = "1046";
            string name = "Three Sides";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 6,2,2,2,2,2,2,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard48(bool instantiateTokenBoard) {
            string id = "1047";
            string name = "Splash";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,5,0,0,0 },
                { 0,0,0,3,0,0,0,0 },
                { 5,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,5 },
                { 0,0,0,0,4,0,0,0 },
                { 0,0,0,5,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard49(bool instantiateTokenBoard) {
            string id = "1048";
            string name = "Test Board Alpha";
            int [,] tokens = new int[8, 8] { 
                { 6,0,1,0,0,2,0,6 },
                { 0,0,1,0,0,2,0,0 },
                { 0,0,1,0,0,2,0,0 },
                { 0,0,1,5,5,2,0,0 },
                { 0,0,1,5,5,2,0,0 },
                { 0,0,1,0,0,2,0,0 },
                { 0,0,1,0,0,2,0,0 },
                { 6,0,1,0,0,2,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard50(bool instantiateTokenBoard) {
            string id = "1049";
            string name = "Simple Stars";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,5,0,0 },
                { 0,0,0,0,5,6,5,0 },
                { 0,0,0,0,0,5,0,0 },
                { 0,0,5,0,0,0,0,0 },
                { 0,5,6,5,0,0,0,0 },
                { 0,0,5,0,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard51(bool instantiateTokenBoard) {
            string id = "1050";
            string name = "Bridge";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 7,7,7,5,5,7,7,7 },
                { 7,7,0,0,0,0,7,7 },
                { 7,0,0,0,0,0,0,7 },
                { 0,0,0,0,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard52(bool instantiateTokenBoard) {
            string id = "1051";
            string name = "Ghost";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,7,0,0 },
                { 0,0,0,0,0,7,7,0 },
                { 0,7,7,0,0,0,0,0 },
                { 0,0,7,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard53(bool instantiateTokenBoard) {
            string id = "1052";
            string name = "Ghost Edges";
            int [,] tokens = new int[8, 8] { 
                { 6,0,7,0,7,0,7,6 },
                { 7,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,7 },
                { 7,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,7 },
                { 7,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,7 },
                { 6,7,0,7,0,7,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard54(bool instantiateTokenBoard) {
            string id = "1053";
            string name = "Ghosts and Stuff";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,2,0,7,3,0,0 },
                { 0,0,0,0,0,7,0,0 },
                { 0,0,0,0,0,0,7,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,4,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,1,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard55(bool instantiateTokenBoard) {
            string id = "1054";
            string name = "Arrows In Diagonal";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,4,0,0,0,0 },
                { 0,0,0,0,4,0,0,0 },
                { 0,1,0,0,0,2,0,0 },
                { 0,0,1,0,0,0,2,0 },
                { 0,0,0,3,0,0,0,0 },
                { 0,0,0,0,3,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard56(bool instantiateTokenBoard) {
            string id = "1055";
            string name = "Push Out";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,5,5,0,0,0 },
                { 0,0,5,1,4,5,0,0 },
                { 0,0,5,3,2,5,0,0 },
                { 0,0,0,5,5,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard57(bool instantiateTokenBoard) {
            string id = "1056";
            string name = "Stone Edges";
            int [,] tokens = new int[8, 8] { 
                { 6,0,6,0,6,0,6,6 },
                { 6,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,6 },
                { 6,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,6 },
                { 6,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,6 },
                { 6,6,0,6,0,6,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard58(bool instantiateTokenBoard) {
            string id = "1057";
            string name = "Arrow Stick";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,2,0,0,0,0 },
                { 0,0,0,5,5,3,0,0 },
                { 0,0,4,5,5,0,0,0 },
                { 0,0,0,0,1,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }
    }
}
