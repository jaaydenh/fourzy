using UnityEngine;

namespace Fourzy
{

    public class TokenBoardLoader : Singleton<TokenBoardLoader> {

        public int[,] FindTokenBoardAll() {

            int boardIndex = Random.Range(0, 16);
            int[,] tokenBoard = new int[8, 8];

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
                    break;
            }

            return tokenBoard;
        }

        public int[,] FindTokenBoardNoSticky() {

            int boardIndex = Random.Range(0, 8);
            int[,] tokenBoard = new int[8, 8];

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
                default:
                    break;
            }

            return tokenBoard;
        }

        public int[,] TokenBoard1() {
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

            return tokens;
        }
            
        public int[,] TokenBoard2() {
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

            return tokens;
        }

        public int[,] TokenBoard3() {
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

            return tokens;
        }

        public int[,] TokenBoard4() {
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

            return tokens;
        }

        public int[,] TokenBoard5() {
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

            return tokens;
        }

        public int[,] TokenBoard6() {
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

            return tokens;
        }

        public int[,] TokenBoard7() {
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

            return tokens;
        }

        public int[,] TokenBoard8() {
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

            return tokens;
        }

        public int[,] TokenBoardSticky1() {
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

            return tokens;
        }

        public int[,] TokenBoardSticky2() {
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

            return tokens;
        }

        public int[,] TokenBoardSticky3() {
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

            return tokens;
        }

        public int[,] TokenBoardSticky4() {
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

            return tokens;
        }

        public int[,] TokenBoardSticky5() {
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

            return tokens;
        }

        public int[,] TokenBoardSticky6() {
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

            return tokens;
        }

        public int[,] TokenBoardSticky7() {
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

            return tokens;
        }

        public int[,] TokenBoardSticky8() {
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

            return tokens;
        }
    }
}
