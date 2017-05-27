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

            int boardIndex = Random.Range(0, 21);

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
                    tokenBoard = TokenBoardSticky1();
                    break;
                case 9:
                    tokenBoard = TokenBoardSticky2();
                    break;
                case 10:
                    tokenBoard = TokenBoardSticky3();
                    break;
                case 11:
                    tokenBoard = TokenBoardSticky4();
                    break;
                case 12:
                    tokenBoard = TokenBoardSticky5();
                    break;
                case 13:
                    tokenBoard = TokenBoardSticky6();
                    break;
                case 14:
                    tokenBoard = TokenBoardSticky7();
                    break;
                case 15:
                    tokenBoard = TokenBoardSticky8();
                    break;
                case 16:
                    tokenBoard = TokenBoardGhost1();
                    break;
                case 17:
                    tokenBoard = TokenBoardGhost2();
                    break;
                case 18:
                    tokenBoard = TokenBoardGhost3();
                    break;
                case 19:
                    tokenBoard = TokenBoardGhost4();
                    break;
                case 20:
                    tokenBoard = TokenBoardGhost5();
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
                    tokenBoard = TokenBoardSticky1();
                    break;
                case 9:
                    tokenBoard = TokenBoardSticky2();
                    break;
                case 10:
                    tokenBoard = TokenBoardSticky3();
                    break;
                case 11:
                    tokenBoard = TokenBoardSticky4();
                    break;
                case 12:
                    tokenBoard = TokenBoardSticky5();
                    break;
                case 13:
                    tokenBoard = TokenBoardSticky6();
                    break;
                case 14:
                    tokenBoard = TokenBoardSticky7();
                    break;
                case 15:
                    tokenBoard = TokenBoardSticky8();
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
            string name = "Wierd Arrows";
            int [,] tokens = new int[8, 8] { 
                { 6,0,0,0,0,0,0,6 },
                { 0,0,0,0,0,1,0,0 },
                { 0,0,0,0,0,5,0,0 },
                { 0,0,3,5,3,1,0,0 },
                { 0,0,2,4,5,4,0,0 },
                { 0,0,5,0,0,0,0,0 },
                { 0,0,2,0,0,0,0,0 },
                { 6,0,0,0,0,0,0,6 }
            };
            TokenBoard tokenboard = new TokenBoard(tokens, id, name);
            return tokenboard;
        }

        public TokenBoard TokenBoardSticky1() {
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

        public TokenBoard TokenBoardSticky2() {
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

        public TokenBoard TokenBoardSticky3() {
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

        public TokenBoard TokenBoardSticky4() {
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

        public TokenBoard TokenBoardSticky5() {
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

        public TokenBoard TokenBoardSticky6() {
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

        public TokenBoard TokenBoardSticky7() {
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

        public TokenBoard TokenBoardSticky8() {
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

        public TokenBoard TokenBoardGhost1() {
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

        public TokenBoard TokenBoardGhost2() {
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

        public TokenBoard TokenBoardGhost3() {
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

        public TokenBoard TokenBoardGhost4() {
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

        public TokenBoard TokenBoardGhost5() {
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
    }
}
