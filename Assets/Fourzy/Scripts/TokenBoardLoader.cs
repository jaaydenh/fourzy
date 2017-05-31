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
                    tokenBoard = TokenBoard1();
                    break;
                case 1:
                    tokenBoard = TokenBoard2();
                    break;
                case 2:
                    tokenBoard = TokenBoard3();
                    break;
                case 3:
                    tokenBoard = TokenBoard4();
                    break;
                case 4:
                    tokenBoard = TokenBoard5();
                    break;
                case 5:
                    tokenBoard = TokenBoard6();
                    break;
                case 6:
                    tokenBoard = TokenBoard7();
                    break;
                case 7:
                    tokenBoard = TokenBoard8();
                    break;
                case 8:
                    tokenBoard = TokenBoard9();
                    break;
                case 9:
                    tokenBoard = TokenBoard10();
                    break;
                case 10:
                    tokenBoard = TokenBoard11();
                    break;
                case 11:
                    tokenBoard = TokenBoard12();
                    break;
                case 12:
                    tokenBoard = TokenBoard13();
                    break;
                case 13:
                    tokenBoard = TokenBoard14();
                    break;
                case 14:
                    tokenBoard = TokenBoard15();
                    break;
                case 15:
                    tokenBoard = TokenBoard16();
                    break;
                case 16:
                    tokenBoard = TokenBoard17();
                    break;
                case 17:
                    tokenBoard = TokenBoard18();
                    break;
                case 18:
                    tokenBoard = TokenBoard19();
                    break;
                case 19:
                    tokenBoard = TokenBoard20();
                    break;
                case 20:
                    tokenBoard = TokenBoard21();
                    break;
                case 21:
                    tokenBoard = TokenBoard22();
                    break;
                case 22:
                    tokenBoard = TokenBoard23();
                    break;
                case 23:
                    tokenBoard = TokenBoard24();
                    break;
                default:
                    tokenBoard = TokenBoard5();
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
                    tokenBoard = TokenBoard1();
                    break;
                case 1:
                    tokenBoard = TokenBoard2();
                    break;
                case 2:
                    tokenBoard = TokenBoard3();
                    break;
                case 3:
                    tokenBoard = TokenBoard4();
                    break;
                case 4:
                    tokenBoard = TokenBoard5();
                    break;
                case 5:
                    tokenBoard = TokenBoard6();
                    break;
                case 6:
                    tokenBoard = TokenBoard7();
                    break;
                case 7:
                    tokenBoard = TokenBoard8();
                    break;
                case 8:
                    tokenBoard = TokenBoard9();
                    break;
                case 9:
                    tokenBoard = TokenBoard10();
                    break;
                case 10:
                    tokenBoard = TokenBoard11();
                    break;
                case 11:
                    tokenBoard = TokenBoard12();
                    break;
                case 12:
                    tokenBoard = TokenBoard13();
                    break;
                case 13:
                    tokenBoard = TokenBoard14();
                    break;
                case 14:
                    tokenBoard = TokenBoard15();
                    break;
                case 15:
                    tokenBoard = TokenBoard16();
                    break;
                default:
                    tokenBoard = TokenBoard5();
                    break;
            }

            return tokenBoard;
        }

        public TokenBoard TokenBoard1() {
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
            TokenBoard tokenboard = new TokenBoard(tokens, id, name);
            return tokenboard;
        }
            
        public TokenBoard TokenBoard2() {
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
            TokenBoard tokenboard = new TokenBoard(tokens, id, name);
            return tokenboard;
        }

        public TokenBoard TokenBoard3() {
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
            TokenBoard tokenboard = new TokenBoard(tokens, id, name);
            return tokenboard;
        }

        public TokenBoard TokenBoard4() {
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
            TokenBoard tokenboard = new TokenBoard(tokens, id, name);
            return tokenboard;
        }

        public TokenBoard TokenBoard5() {
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
            TokenBoard tokenboard = new TokenBoard(tokens, id, name);
            return tokenboard;
        }

        public TokenBoard TokenBoard6() {
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
            TokenBoard tokenboard = new TokenBoard(tokens, id, name);
            return tokenboard;
        }

        public TokenBoard TokenBoard7() {
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
            TokenBoard tokenboard = new TokenBoard(tokens, id, name);
            return tokenboard;
        }

        public TokenBoard TokenBoard8() {
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
            TokenBoard tokenboard = new TokenBoard(tokens, id, name);
            return tokenboard;
        }

        public TokenBoard TokenBoard9() {
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
            TokenBoard tokenboard = new TokenBoard(tokens, id, name);
            return tokenboard;
        }

        public TokenBoard TokenBoard10() {
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
            TokenBoard tokenboard = new TokenBoard(tokens, id, name);
            return tokenboard;
        }

        public TokenBoard TokenBoard11() {
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
            TokenBoard tokenboard = new TokenBoard(tokens, id, name);
            return tokenboard;
        }

        public TokenBoard TokenBoard12() {
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
            TokenBoard tokenboard = new TokenBoard(tokens, id, name);
            return tokenboard;
        }

        public TokenBoard TokenBoard13() {
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
            TokenBoard tokenboard = new TokenBoard(tokens, id, name);
            return tokenboard;
        }

        public TokenBoard TokenBoard14() {
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
            TokenBoard tokenboard = new TokenBoard(tokens, id, name);
            return tokenboard;
        }

        public TokenBoard TokenBoard15() {
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
            TokenBoard tokenboard = new TokenBoard(tokens, id, name);
            return tokenboard;
        }

        public TokenBoard TokenBoard16() {
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
            TokenBoard tokenboard = new TokenBoard(tokens, id, name);
            return tokenboard;
        }

        public TokenBoard TokenBoard17() {
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
            TokenBoard tokenboard = new TokenBoard(tokens, id, name);
            return tokenboard;
        }

        public TokenBoard TokenBoard18() {
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
            TokenBoard tokenboard = new TokenBoard(tokens, id, name);
            return tokenboard;
        }

        public TokenBoard TokenBoard19() {
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
            TokenBoard tokenboard = new TokenBoard(tokens, id, name);
            return tokenboard;
        }

        public TokenBoard TokenBoard20() {
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
            TokenBoard tokenboard = new TokenBoard(tokens, id, name);
            return tokenboard;
        }

        public TokenBoard TokenBoard21() {
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
            TokenBoard tokenboard = new TokenBoard(tokens, id, name);
            return tokenboard;
        }

        public TokenBoard TokenBoard22() {
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
            TokenBoard tokenboard = new TokenBoard(tokens, id, name);
            return tokenboard;
        }

        public TokenBoard TokenBoard23() {
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
            TokenBoard tokenboard = new TokenBoard(tokens, id, name);
            return tokenboard;
        }

        public TokenBoard TokenBoard24() {
            string id = "1023";
            string name = "Scattered Ghost Ring";
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
            TokenBoard tokenboard = new TokenBoard(tokens, id, name);
            return tokenboard;
        }
    }
}
