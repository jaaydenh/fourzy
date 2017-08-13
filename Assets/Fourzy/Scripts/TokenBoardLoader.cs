using UnityEngine;

namespace Fourzy
{

    public class TokenBoardLoader : MonoBehaviour
    {

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

        public TokenBoard GetTokenBoard()
        {

            TokenBoard tokenBoard;

            int boardIndex = Random.Range(0, 59);

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
                    tokenBoard = TokenBoard59(true);
                    break;
                case 16:
                    tokenBoard = TokenBoard60(true);
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
                    tokenBoard = TokenBoard61(true);
                    break;
                case 22:
                    tokenBoard = TokenBoard62(true);
                    break;
                case 23:
                    tokenBoard = TokenBoard47(true);
                    break;
                case 24:
                    tokenBoard = TokenBoard63(true);
                    break;
                case 25:
                    tokenBoard = TokenBoard64(true);
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
                    tokenBoard = TokenBoard65(true);
                    break;
                case 34:
                    tokenBoard = TokenBoard58(true);
                    break;
                case 35:
                    tokenBoard = TokenBoard66(true);
                    break;
                case 36:
                    tokenBoard = TokenBoard67(true);
                    break;
                case 37:
                    tokenBoard = TokenBoard68(true);
                    break;
                case 38:
                    tokenBoard = TokenBoard69(true);
                    break;
                case 39:
                    tokenBoard = TokenBoard70(true);
                    break;
                case 40:
                    tokenBoard = TokenBoard71(true);
                    break;
                case 41:
                    tokenBoard = TokenBoard72(true);
                    break;
                case 42:
                    tokenBoard = TokenBoard73(true);
                    break;
                case 43:
                    tokenBoard = TokenBoard74(true);
                    break;
                case 44:
                    tokenBoard = TokenBoard75(true);
                    break;
                case 45:
                    tokenBoard = TokenBoard76(true);
                    break;
                case 46:
                    tokenBoard = TokenBoard77(true);
                    break;
                case 47:
                    tokenBoard = TokenBoard78(true);
                    break;
                case 48:
                    tokenBoard = TokenBoard79(true);
                    break;
                case 49:
                    tokenBoard = TokenBoard80(true);
                    break;
                case 50:
                    tokenBoard = TokenBoard81(true);
                    break;
                case 51:
                    tokenBoard = TokenBoard82(true);
                    break;
                case 52:
                    tokenBoard = TokenBoard83(true);
                    break;
                case 53:
                    tokenBoard = TokenBoard84(true);
                    break;
                case 54:
                    tokenBoard = TokenBoard85(true);
                    break;
                case 55:
                    tokenBoard = TokenBoard86(true);
                    break;
                case 56:
                    tokenBoard = TokenBoard87(true);
                    break;
                case 57:
                    tokenBoard = TokenBoard88(true);
                    break;
                case 58:
                    tokenBoard = TokenBoard89(true);
                    break;

                default:
                    tokenBoard = TokenBoard1(true);
                    break;
            }

            return tokenBoard;
        }

        public TokenBoard[] GetAllTokenBoards()
        {
            TokenBoard[] tokenBoards = new TokenBoard[59];

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

            TokenBoard tokenBoard16 = TokenBoard59(false);
            tokenBoards[15] = tokenBoard16;

            TokenBoard tokenBoard17 = TokenBoard60(false);
            tokenBoards[16] = tokenBoard17;

            TokenBoard tokenBoard18 = TokenBoard41(false);
            tokenBoards[17] = tokenBoard18;

            TokenBoard tokenBoard19 = TokenBoard42(false);
            tokenBoards[18] = tokenBoard19;

            TokenBoard tokenBoard20 = TokenBoard43(false);
            tokenBoards[19] = tokenBoard20;

            TokenBoard tokenBoard21 = TokenBoard44(false);
            tokenBoards[20] = tokenBoard21;

            TokenBoard tokenBoard22 = TokenBoard61(false);
            tokenBoards[21] = tokenBoard22;

            TokenBoard tokenBoard23 = TokenBoard62(false);
            tokenBoards[22] = tokenBoard23;

            TokenBoard tokenBoard24 = TokenBoard47(false);
            tokenBoards[23] = tokenBoard24;

            TokenBoard tokenBoard25 = TokenBoard63(false);
            tokenBoards[24] = tokenBoard25;

            TokenBoard tokenBoard26 = TokenBoard64(false);
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

            TokenBoard tokenBoard34 = TokenBoard65(false);
            tokenBoards[33] = tokenBoard34;

            TokenBoard tokenBoard35 = TokenBoard58(false);
            tokenBoards[34] = tokenBoard35;

            TokenBoard tokenBoard36 = TokenBoard66(false);
            tokenBoards[35] = tokenBoard36;

            TokenBoard tokenBoard37 = TokenBoard67(false);
            tokenBoards[36] = tokenBoard37;

            TokenBoard tokenBoard38 = TokenBoard68(false);
            tokenBoards[37] = tokenBoard38;

            TokenBoard tokenBoard39 = TokenBoard69(false);
            tokenBoards[38] = tokenBoard39;

            TokenBoard tokenBoard40 = TokenBoard70(false);
            tokenBoards[39] = tokenBoard40;

            TokenBoard tokenBoard41 = TokenBoard71(false);
            tokenBoards[40] = tokenBoard41;

            TokenBoard tokenBoard42 = TokenBoard72(false);
            tokenBoards[41] = tokenBoard42;

            TokenBoard tokenBoard43 = TokenBoard73(false);
            tokenBoards[42] = tokenBoard43;

            TokenBoard tokenBoard44 = TokenBoard74(false);
            tokenBoards[43] = tokenBoard44;

            TokenBoard tokenBoard45 = TokenBoard75(false);
            tokenBoards[44] = tokenBoard45;

            TokenBoard tokenBoard46 = TokenBoard76(false);
            tokenBoards[45] = tokenBoard46;

            TokenBoard tokenBoard47 = TokenBoard77(false);
            tokenBoards[46] = tokenBoard47;

            TokenBoard tokenBoard48 = TokenBoard78(false);
            tokenBoards[47] = tokenBoard48;

            TokenBoard tokenBoard49 = TokenBoard79(false);
            tokenBoards[48] = tokenBoard49;

            TokenBoard tokenBoard50 = TokenBoard80(false);
            tokenBoards[49] = tokenBoard50;

            TokenBoard tokenBoard51 = TokenBoard81(false);
            tokenBoards[50] = tokenBoard51;

            TokenBoard tokenBoard52 = TokenBoard82(false);
            tokenBoards[51] = tokenBoard52;

            TokenBoard tokenBoard53 = TokenBoard83(false);
            tokenBoards[52] = tokenBoard53;

            TokenBoard tokenBoard54 = TokenBoard84(false);
            tokenBoards[53] = tokenBoard54;

            TokenBoard tokenBoard55 = TokenBoard85(false);
            tokenBoards[54] = tokenBoard55;

            TokenBoard tokenBoard56 = TokenBoard86(false);
            tokenBoards[55] = tokenBoard56;

            TokenBoard tokenBoard57 = TokenBoard87(false);
            tokenBoards[56] = tokenBoard57;

            TokenBoard tokenBoard58 = TokenBoard88(false);
            tokenBoards[57] = tokenBoard58;

            TokenBoard tokenBoard59 = TokenBoard89(false);
            tokenBoards[58] = tokenBoard59;

            return tokenBoards;
        }

        public TokenBoard TokenBoard1(bool instantiateTokenBoard)
        {
            string id = "1000";
            string name = "The Basic Game";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard2(bool instantiateTokenBoard)
        {
            string id = "1001";
            string name = "Sticky to the Right";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard3(bool instantiateTokenBoard)
        {
            string id = "1002";
            string name = "Up and Down";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard4(bool instantiateTokenBoard)
        {
            string id = "1003";
            string name = "Empty Center";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard5(bool instantiateTokenBoard)
        {
            string id = "1004";
            string name = "Simple Spiral";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard6(bool instantiateTokenBoard)
        {
            string id = "1005";
            string name = "Left or Right";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard7(bool instantiateTokenBoard)
        {
            string id = "1006";
            string name = "A Sticky Escape";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard8(bool instantiateTokenBoard)
        {
            string id = "1007";
            string name = "Ghost Coated Shuriken";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard9(bool instantiateTokenBoard)
        {
            string id = "1008";
            string name = "Narrow Miss";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard10(bool instantiateTokenBoard)
        {
            string id = "1009";
            string name = "Up or Stuck";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard11(bool instantiateTokenBoard)
        {
            string id = "1010";
            string name = "Sticky Donut";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard12(bool instantiateTokenBoard)
        {
            string id = "1011";
            string name = "Reversed Sticky X";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard13(bool instantiateTokenBoard)
        {
            string id = "1012";
            string name = "Sticky Corners";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard14(bool instantiateTokenBoard)
        {
            string id = "1013";
            string name = "Sticky Checkers";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard15(bool instantiateTokenBoard)
        {
            string id = "1014";
            string name = "Big Sticky Zero";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard16(bool instantiateTokenBoard)
        {
            string id = "1015";
            string name = "Big Sticky Block";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard17(bool instantiateTokenBoard)
        {
            string id = "1016";
            string name = "Original Ghost";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard18(bool instantiateTokenBoard)
        {
            string id = "1017";
            string name = "Ghost Push";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard19(bool instantiateTokenBoard)
        {
            string id = "1018";
            string name = "Sticky Sided Ghost";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard20(bool instantiateTokenBoard)
        {
            string id = "1019";
            string name = "Big Ghost Zero";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard21(bool instantiateTokenBoard)
        {
            string id = "1020";
            string name = "Ghost Edge";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard22(bool instantiateTokenBoard)
        {
            string id = "1021";
            string name = "Sticky Filled Ghost Checkers";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard23(bool instantiateTokenBoard)
        {
            string id = "1022";
            string name = "Divide by Ghosts";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard24(bool instantiateTokenBoard)
        {
            string id = "1023";
            string name = "Covered Sticky Stick";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard25(bool instantiateTokenBoard)
        {
            string id = "1024";
            string name = "GhostSticky Large Four";
            int[,] tokens = new int[8, 8] {
                { 6,0,0,0,0,0,0,6 },
                { 0,0,7,8,0,0,8,0 },
                { 0,0,8,0,0,8,7,0 },
                { 0,0,0,5,0,0,0,0 },
                { 0,0,8,0,0,5,0,0 },
                { 0,0,7,8,0,0,8,0 },
                { 0,0,8,0,0,8,7,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard26(bool instantiateTokenBoard)
        {
            string id = "1025";
            string name = "Ghosts Diagonal";
            int[,] tokens = new int[8, 8] {
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,0,0,0 },
                { 5,0,0,0,0,0,0,0 },
                { 5,0,0,7,0,0,0,0 },
                { 5,0,0,0,0,7,0,0 },
                { 5,0,7,0,0,0,0,0 },
                { 0,0,0,0,7,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard27(bool instantiateTokenBoard)
        {
            string id = "1026";
            string name = "GhostSticky Diagonal Cross";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard28(bool instantiateTokenBoard)
        {
            string id = "1027";
            string name = "GhostSticky Hanging Light";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard29(bool instantiateTokenBoard)
        {
            string id = "1028";
            string name = "GhostSticky Up Down";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard30(bool instantiateTokenBoard)
        {
            string id = "1029";
            string name = "GhostSticky in the Middle";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard31(bool instantiateTokenBoard)
        {
            string id = "1030";
            string name = "Ghost Circle";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard32(bool instantiateTokenBoard)
        {
            string id = "1031";
            string name = "Ghost Circle With Center";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard33(bool instantiateTokenBoard)
        {
            string id = "1032";
            string name = "SkyScraper";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard34(bool instantiateTokenBoard)
        {
            string id = "1033";
            string name = "Center Three";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard35(bool instantiateTokenBoard)
        {
            string id = "1034";
            string name = "Sticky Double Diagonal";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard36(bool instantiateTokenBoard)
        {
            string id = "1035";
            string name = "Center Star";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard37(bool instantiateTokenBoard)
        {
            string id = "1036";
            string name = "Arrow Sticky Down Up";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard38(bool instantiateTokenBoard)
        {
            string id = "1037";
            string name = "Big Sticky Arrow";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard39(bool instantiateTokenBoard)
        {
            string id = "1038";
            string name = "Sticky Four";
            bool enabled = false;
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard40(bool instantiateTokenBoard)
        {
            string id = "1039";
            string name = "Sticky Four Askew";
            bool enabled = false;
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard41(bool instantiateTokenBoard)
        {
            string id = "1040";
            string name = "Smile";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard42(bool instantiateTokenBoard)
        {
            string id = "1041";
            string name = "Big Sticky Square";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard43(bool instantiateTokenBoard)
        {
            string id = "1042";
            string name = "Sticky Wedge";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard44(bool instantiateTokenBoard)
        {
            string id = "1043";
            string name = "Sticky Diamond";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard45(bool instantiateTokenBoard)
        {
            string id = "1044";
            string name = "Arrows";
            bool enabled = false;
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard46(bool instantiateTokenBoard)
        {
            string id = "1045";
            string name = "Diagonal Arrow Channels";
            bool enabled = false;
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard47(bool instantiateTokenBoard)
        {
            string id = "1046";
            string name = "Three Sides";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard48(bool instantiateTokenBoard)
        {
            string id = "1047";
            string name = "Splash";
            bool enabled = false;
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard49(bool instantiateTokenBoard)
        {
            string id = "1048";
            string name = "Test Board Alpha";
            bool enabled = false;
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard50(bool instantiateTokenBoard)
        {
            string id = "1049";
            string name = "Simple Stars";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard51(bool instantiateTokenBoard)
        {
            string id = "1050";
            string name = "Bridge";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard52(bool instantiateTokenBoard)
        {
            string id = "1051";
            string name = "Ghost";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard53(bool instantiateTokenBoard)
        {
            string id = "1052";
            string name = "Ghost Edges";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard54(bool instantiateTokenBoard)
        {
            string id = "1053";
            string name = "Ghosts and Stuff";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard55(bool instantiateTokenBoard)
        {
            string id = "1054";
            string name = "Arrows In Diagonal";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard56(bool instantiateTokenBoard)
        {
            string id = "1055";
            string name = "Push Out";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard57(bool instantiateTokenBoard)
        {
            string id = "1056";
            string name = "Stone Edges";
            bool enabled = false;
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard58(bool instantiateTokenBoard)
        {
            string id = "1057";
            string name = "Arrow Stick";
            int[,] tokens = new int[8, 8] {
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

        public TokenBoard TokenBoard59(bool instantiateTokenBoard)
        {
            string id = "1058";
            string name = "Arrow Path 1";
            int[,] tokens = new int[8, 8] {
                { 6,2,2,2,2,2,2,6 },
                { 3,3,2,2,2,2,4,4 },
                { 3,3,3,2,2,4,4,4 },
                { 3,3,3,3,4,4,4,4 },
                { 3,3,3,4,4,4,4,4 },
                { 3,3,1,1,4,4,4,4 },
                { 3,1,1,1,1,4,4,4 },
                { 6,1,1,1,1,1,2,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard60(bool instantiateTokenBoard)
        {
            string id = "1059";
            string name = "Arrow Path 2";
            int[,] tokens = new int[8, 8] {
                { 6,2,2,4,4,4,4,6 },
                { 2,2,4,4,4,4,4,4 },
                { 2,4,4,4,4,4,4,4 },
                { 4,4,4,4,4,4,4,4 },
                { 4,4,4,4,4,4,4,4 },
                { 1,4,4,4,4,4,4,4 },
                { 1,1,4,4,4,4,4,4 },
                { 6,1,1,4,4,4,4,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard61(bool instantiateTokenBoard)
        {
            string id = "1060";
            string name = "Arrow Path 3";
            int[,] tokens = new int[8, 8] {
                { 6,1,2,2,1,1,1,6 },
                { 4,1,2,2,1,1,1,3 },
                { 3,3,2,4,1,1,4,4 },
                { 4,1,4,4,4,1,1,3 },
                { 4,2,4,4,4,2,2,3 },
                { 3,3,1,4,2,2,4,4 },
                { 4,2,1,1,2,2,2,3 },
                { 6,2,1,1,2,2,2,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard62(bool instantiateTokenBoard)
        {
            string id = "1061";
            string name = "Arrow Path 4";
            int[,] tokens = new int[8, 8] {
                { 6,4,4,4,2,1,2,6 },
                { 4,4,4,2,2,1,2,2 },
                { 3,3,3,3,2,1,2,2 },
                { 3,3,3,3,3,1,3,4 },
                { 3,4,4,4,4,4,4,4 },
                { 1,1,2,1,4,4,4,4 },
                { 1,1,2,1,1,3,3,3 },
                { 6,1,2,1,3,3,3,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard63(bool instantiateTokenBoard)
        {
            string id = "1062";
            string name = "Block 1";
            int[,] tokens = new int[8, 8] {
                { 6,0,6,0,0,6,0,6 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 6,6,0,6,6,0,6,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard64(bool instantiateTokenBoard)
        {
            string id = "1063";
            string name = "Block 2";
            int[,] tokens = new int[8, 8] {
                { 6,0,0,0,0,6,6,6 },
                { 6,0,0,0,0,0,0,0 },
                { 6,0,0,5,5,0,0,0 },
                { 0,0,5,5,5,5,0,0 },
                { 0,0,5,5,5,5,0,0 },
                { 0,0,0,5,5,0,0,6 },
                { 0,0,0,0,0,0,0,6 },
                { 6,6,6,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard65(bool instantiateTokenBoard)
        {
            string id = "1064";
            string name = "Protected Sides";
            int[,] tokens = new int[8, 8] {
                { 6,3,3,3,3,3,3,6 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,5,5,5,0,0 },
                { 0,0,0,5,5,5,0,0 },
                { 0,0,0,5,5,5,0,0 },
                { 0,0,0,5,5,5,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 6,3,3,3,3,3,3,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard66(bool instantiateTokenBoard)
        {
            string id = "1065";
            string name = "Sticky Group 1";
            int[,] tokens = new int[8, 8] {
                { 6,0,0,0,0,0,0,6 },
                { 0,5,5,0,5,5,5,0 },
                { 0,5,5,0,5,5,5,0 },
                { 0,5,5,0,0,0,0,0 },
                { 0,0,0,0,0,5,5,0 },
                { 0,5,5,5,0,5,5,0 },
                { 0,5,5,5,0,5,5,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }


        public TokenBoard TokenBoard67(bool instantiateTokenBoard)
        {
            string id = "1066";
            string name = "Sticky Group 2";
            int[,] tokens = new int[8, 8] {
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,5,5,5,0,0,0,0 },
                { 0,5,5,5,0,0,0,0 },
                { 0,5,5,5,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard68(bool instantiateTokenBoard)
        {
            string id = "1067";
            string name = "Sticky Group 3";
            int[,] tokens = new int[8, 8] {
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,5,5,0,0 },
                { 0,0,0,0,5,5,0,0 },
                { 0,0,0,0,5,5,0,0 },
                { 0,5,5,0,5,5,0,0 },
                { 0,5,5,5,5,5,0,0 },
                { 0,5,5,5,5,5,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard69(bool instantiateTokenBoard)
        {
            string id = "1068";
            string name = "Sticky Group 4";
            int[,] tokens = new int[8, 8] {
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,5,0,0 },
                { 0,0,0,0,0,5,0,0 },
                { 0,0,0,0,0,5,0,0 },
                { 0,0,0,0,0,5,0,0 },
                { 0,5,5,5,5,5,5,0 },
                { 0,0,0,0,0,5,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard70(bool instantiateTokenBoard)
        {
            string id = "1069";
            string name = "Ice Flow 1";
            int[,] tokens = new int[8, 8] {
                { 6,8,8,8,8,8,8,6 },
                { 8,8,8,8,8,8,8,8 },
                { 8,8,8,8,8,8,8,8 },
                { 8,8,8,8,8,8,8,8 },
                { 8,8,8,8,8,8,8,8 },
                { 8,8,8,8,8,8,8,8 },
                { 8,8,8,8,8,8,8,8 },
                { 6,8,8,8,8,8,8,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard71(bool instantiateTokenBoard)
        {
            string id = "1070";
            string name = "Ice Flow 2";
            int[,] tokens = new int[8, 8] {
                { 6,8,8,8,8,8,8,6 },
                { 8,8,8,8,8,8,8,8 },
                { 8,8,6,8,8,8,8,8 },
                { 8,8,8,8,8,8,8,8 },
                { 8,8,8,8,8,8,8,8 },
                { 8,8,8,8,8,6,8,8 },
                { 8,8,8,8,8,8,8,8 },
                { 6,8,8,8,8,8,8,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard72(bool instantiateTokenBoard)
        {
            string id = "1071";
            string name = "Ice Flow 3";
            int[,] tokens = new int[8, 8] {
                { 6,8,8,8,8,8,8,6 },
                { 8,8,8,8,8,8,8,8 },
                { 8,8,8,6,8,8,8,8 },
                { 8,8,8,8,8,8,8,8 },
                { 8,8,8,8,8,8,8,8 },
                { 8,8,8,8,6,8,8,8 },
                { 8,8,8,8,8,8,8,8 },
                { 6,8,8,8,8,8,8,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard73(bool instantiateTokenBoard)
        {
            string id = "1072";
            string name = "Ice Flow 4";
            int[,] tokens = new int[8, 8] {
                { 6,8,8,8,8,8,8,6 },
                { 8,8,8,8,6,8,8,8 },
                { 8,8,6,8,8,8,8,8 },
                { 8,8,8,8,8,6,8,8 },
                { 8,8,8,6,8,8,8,8 },
                { 8,8,8,8,8,8,8,8 },
                { 8,8,8,8,8,8,8,8 },
                { 6,8,8,8,8,8,8,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard74(bool instantiateTokenBoard)
        {
            string id = "1073";
            string name = "Ice Flow 5";
            int[,] tokens = new int[8, 8] {
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,8,8,8,8,0,0 },
                { 0,0,8,8,8,8,0,0 },
                { 0,0,8,8,8,8,0,0 },
                { 0,0,8,8,8,8,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }


        public TokenBoard TokenBoard75(bool instantiateTokenBoard)
        {
            string id = "1074";
            string name = "Ice Flow 6";
            int[,] tokens = new int[8, 8] {
                { 6,8,8,8,8,8,8,6 },
                { 8,8,8,8,8,8,8,8 },
                { 8,8,8,8,8,8,8,8 },
                { 8,8,8,0,0,8,8,8 },
                { 8,8,8,0,0,8,8,8 },
                { 8,8,8,8,8,8,8,8 },
                { 8,8,8,8,8,8,8,8 },
                { 6,8,8,8,8,8,8,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard76(bool instantiateTokenBoard)
        {
            string id = "1075";
            string name = "Ice Flow 7";
            int[,] tokens = new int[8, 8] {
                { 6,8,8,8,8,8,8,6 },
                { 8,8,8,8,8,8,8,8 },
                { 8,8,8,5,5,8,8,8 },
                { 8,8,5,5,5,5,8,8 },
                { 8,8,5,5,5,5,8,8 },
                { 8,8,8,5,5,8,8,8 },
                { 8,8,8,8,8,8,8,8 },
                { 6,8,8,8,8,8,8,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard77(bool instantiateTokenBoard)
        {
            string id = "1076";
            string name = "Ice Flow 8";
            int[,] tokens = new int[8, 8] {
                { 6,0,0,8,8,0,0,6 },
                { 0,0,0,8,8,0,0,0 },
                { 0,0,0,8,8,0,0,0 },
                { 8,8,8,8,8,8,8,8 },
                { 8,8,8,8,8,8,8,8 },
                { 0,0,0,8,8,0,0,0 },
                { 0,0,0,8,8,0,0,0 },
                { 6,0,0,8,8,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard78(bool instantiateTokenBoard)
        {
            string id = "1077";
            string name = "Ice Flow 9";
            int[,] tokens = new int[8, 8] {
                { 6,0,0,0,8,8,8,6 },
                { 0,0,0,0,8,8,8,8 },
                { 0,0,0,0,8,8,8,8 },
                { 0,0,0,0,8,8,8,8 },
                { 8,8,8,8,0,0,0,0 },
                { 8,8,8,8,0,0,0,0 },
                { 8,8,8,8,0,0,0,0 },
                { 6,8,8,8,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard79(bool instantiateTokenBoard)
        {
            string id = "1078";
            string name = "Ice Flow 10";
            int[,] tokens = new int[8, 8] {
                { 6,8,8,8,8,8,8,6 },
                { 8,8,8,8,8,8,8,8 },
                { 8,8,8,2,8,8,8,8 },
                { 8,8,4,8,8,8,8,8 },
                { 8,8,8,8,8,3,8,8 },
                { 8,8,8,8,1,8,8,8 },
                { 8,8,8,8,8,8,8,8 },
                { 6,8,8,8,8,8,8,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard80(bool instantiateTokenBoard)
        {
            string id = "1079";
            string name = "Mixed 1";
            int[,] tokens = new int[8, 8] {
                { 6,2,2,2,2,2,2,6 },
                { 3,0,0,0,0,0,0,3 },
                { 4,0,0,0,0,0,0,4 },
                { 3,0,0,0,0,0,0,3 },
                { 4,0,0,0,0,0,0,4 },
                { 3,0,0,0,0,0,0,3 },
                { 4,0,0,0,0,0,0,4 },
                { 6,2,2,2,2,2,2,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard81(bool instantiateTokenBoard)
        {
            string id = "1080";
            string name = "Mixed 2";
            int[,] tokens = new int[8, 8] {
                { 6,0,0,0,0,0,0,6 },
                { 0,0,5,0,0,5,0,0 },
                { 0,5,5,0,0,5,5,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,5,5,0,0,5,5,0 },
                { 0,0,5,0,0,5,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard82(bool instantiateTokenBoard)
        {
            string id = "1081";
            string name = "Mixed 3";
            int[,] tokens = new int[8, 8] {
                { 6,2,2,2,2,2,2,6 },
                { 3,0,0,0,0,0,0,3 },
                { 4,0,0,0,0,0,0,4 },
                { 3,0,0,0,0,0,0,3 },
                { 4,0,0,0,0,0,0,4 },
                { 3,0,0,0,0,0,0,3 },
                { 4,0,0,0,0,0,0,4 },
                { 6,2,2,2,2,2,2,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard83(bool instantiateTokenBoard)
        {
            string id = "1082";
            string name = "Ghost Edge 1";
            int[,] tokens = new int[8, 8] {
                { 6,7,7,0,0,7,7,6 },
                { 7,7,0,0,0,0,7,7 },
                { 7,0,0,0,0,0,0,7 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 7,0,0,0,0,0,0,7 },
                { 7,7,0,0,0,0,7,7 },
                { 6,7,7,0,0,7,7,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }


        public TokenBoard TokenBoard84(bool instantiateTokenBoard)
        {
            string id = "1083";
            string name = "Sticky Group 10";
            int[,] tokens = new int[8, 8] {
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,5,5,5,5,5,5,0 },
                { 0,5,5,5,5,5,5,0 },
                { 0,0,0,0,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }


        public TokenBoard TokenBoard85(bool instantiateTokenBoard)
        {
            string id = "1084";
            string name = "Mixed 4";
            int[,] tokens = new int[8, 8] {
                { 6,0,0,0,0,0,0,6 },
                { 0,2,0,0,0,0,5,0 },
                { 0,0,2,0,0,5,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,5,0,0,1,0,0 },
                { 0,5,0,0,0,0,1,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }


        public TokenBoard TokenBoard86(bool instantiateTokenBoard)
        {
            string id = "1085";
            string name = "Sticky Group 11";
            int[,] tokens = new int[8, 8] {
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,0,0,0 },
                { 0,5,0,5,0,5,0,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,0,5,0,5,0,5,0 },
                { 0,0,0,0,0,0,0,0 },
                { 0,5,0,5,0,5,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }


        public TokenBoard TokenBoard87(bool instantiateTokenBoard)
        {
            string id = "1086";
            string name = "Ghost Scatter";
            int[,] tokens = new int[8, 8] {
                { 6,0,0,7,0,7,0,6 },
                { 7,0,7,0,7,0,7,0 },
                { 0,7,0,0,0,0,0,7 },
                { 7,0,0,0,0,0,7,0 },
                { 0,7,0,0,0,0,0,7 },
                { 7,0,0,0,0,0,7,0 },
                { 0,7,0,7,0,7,0,7 },
                { 6,0,7,0,7,0,7,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }

        public TokenBoard TokenBoard88(bool instantiateTokenBoard)
        {
            string id = "1087";
            string name = "Mixed 5";
            int[,] tokens = new int[8, 8] {
                { 6,0,0,7,0,0,0,6 },
                { 0,0,0,0,7,0,0,0 },
                { 0,0,0,0,0,7,0,0 },
                { 0,5,5,5,0,0,7,0 },
                { 0,0,0,0,1,0,0,7 },
                { 0,0,0,1,0,0,0,0 },
                { 0,0,1,0,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }


        public TokenBoard TokenBoard89(bool instantiateTokenBoard)
        {
            string id = "1088";
            string name = "Mixed 6";
            int[,] tokens = new int[8, 8] {
                { 6,0,0,0,0,0,0,6 },
                { 0,0,7,0,0,0,0,0 },
                { 0,0,0,0,0,7,0,0 },
                { 0,0,0,3,4,0,0,0 },
                { 0,0,0,3,4,0,0,0 },
                { 0,0,7,0,0,0,0,0 },
                { 0,0,0,0,0,7,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name, instantiateTokenBoard);
            return tokenboard;
        }
    }
}
