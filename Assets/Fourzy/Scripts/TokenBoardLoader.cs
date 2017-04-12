using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy {
    
    public class TokenBoardLoader : Singleton<TokenBoardLoader> {

        public int[] FindTokenBoardAll() {

            int boardIndex = Random.Range(0, 8);
            int[] tokenBoard = new int[64];

            switch (boardIndex)
            {
                case 0:
                    tokenBoard = TokenBoard5();
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
                    tokenBoard = TokenBoardSticky1();
                    break;
                case 5:
                    tokenBoard = TokenBoardSticky2();
                    break;
                case 6:
                    tokenBoard = TokenBoardSticky3();
                    break;
                case 7:
                    tokenBoard = TokenBoardSticky4();
                    break;
                default:
                    break;
            }

            return tokenBoard;
        }

        public int[] FindTokenBoardNoSticky() {

            int boardIndex = Random.Range(0, 4);
            int[] tokenBoard = new int[64];

            switch (boardIndex)
            {
                case 0:
                    tokenBoard = TokenBoard5();
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
                default:
                    break;
            }

            return tokenBoard;
        }

        public int[] TokenBoard1() {
            int[] tokens = new int[] {
                6, 0, 0, 0, 0, 0, 0, 6,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 2, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 3, 0, 0,
                0, 0, 4, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 1, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                6, 0, 0, 0, 0, 0, 0, 6};

            return tokens;
        }
            
        public int[] TokenBoard2() {
            int[] tokens = new int[] {
                6, 0, 0, 0, 0, 0, 0, 6,
                0, 0, 0, 0, 0, 0, 4, 0,
                0, 0, 0, 0, 0, 4, 0, 0,
                0, 0, 0, 0, 4, 0, 0, 0,
                0, 0, 0, 4, 0, 0, 0, 0,
                0, 0, 4, 0, 0, 0, 0, 0,
                0, 4, 0, 0, 0, 0, 0, 0,
                6, 0, 0, 0, 0, 0, 0, 6};

            return tokens;
        }

        public int[] TokenBoard3() {
            int[] tokens = new int[] {
                6, 0, 0, 0, 0, 0, 0, 6,
                0, 0, 0, 0, 1, 0, 0, 0,
                0, 0, 0, 0, 1, 0, 0, 0,
                0, 0, 0, 0, 1, 0, 0, 0,
                0, 0, 0, 2, 0, 0, 0, 0,
                0, 0, 0, 2, 0, 0, 0, 0,
                0, 0, 0, 2, 0, 0, 0, 0,
                6, 0, 0, 0, 0, 0, 0, 6};

            return tokens;
        }

        public int[] TokenBoard4() {
            int[] tokens = new int[] {
                6, 0, 0, 0, 0, 0, 0, 6,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 6, 0, 0, 6, 0, 0,
                0, 0, 2, 0, 0, 0, 0, 0,
                0, 0, 2, 0, 0, 0, 0, 0,
                0, 0, 6, 3, 3, 6, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                6, 0, 0, 0, 0, 0, 0, 6};

            return tokens;
        }

        public int[] TokenBoard5() {
            int[] tokens = new int[] {
                6, 0, 0, 0, 0, 0, 0, 6,
                0, 0, 0, 2, 0, 0, 0, 0,
                0, 0, 0, 0, 4, 0, 0, 0,
                0, 0, 2, 0, 0, 0, 4, 0,
                0, 3, 0, 0, 0, 1, 0, 0,
                0, 0, 0, 3, 0, 0, 0, 0,
                0, 0, 0, 0, 1, 0, 0, 0,
                6, 0, 0, 0, 0, 0, 0, 6};

            return tokens;
        }

        public int[] TokenBoardSticky1() {
            int[] tokens = new int[] {
                6, 0, 0, 0, 0, 0, 0, 6,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 5, 0, 0, 5, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 5, 0, 0, 5, 0, 0,
                0 ,0, 0, 0, 0, 0, 0, 0,
                6, 0, 0, 0, 0, 0, 0, 6};

            return tokens;
        }

        public int[] TokenBoardSticky2() {
            int[] tokens = new int[] {
                6, 0, 0, 0, 0, 0, 0, 6,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 1, 5, 0, 0,
                0, 0, 0, 1, 5, 0, 0, 0,
                0, 0, 1, 5, 0, 0, 0, 0,
                0, 1, 5, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                6, 0, 0, 0, 0, 0, 0, 6};

            return tokens;
        }

        public int[] TokenBoardSticky3() {
            int[] tokens = new int[] {
                6, 0, 0, 0, 0, 0, 0, 6,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 5, 5, 5, 5, 0, 0,
                0, 0, 5, 5, 5, 5, 0, 0,
                0, 0, 5, 5, 5, 5, 0, 0,
                0, 0, 5, 5, 5, 5, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                6, 0, 0, 0, 0, 0, 0, 6};

            return tokens;
        }

        public int[] TokenBoardSticky4() {
            int[] tokens = new int[] {
                6, 0, 0, 0, 0, 0, 0, 6,
                0, 0, 0, 0, 0, 0, 5, 0,
                0, 0, 0, 0, 0, 5, 0, 0,
                0, 0, 0, 4, 0, 0, 0, 0,
                0, 0, 0, 0, 3, 0, 0, 0,
                0, 0, 5, 0, 0, 0, 0, 0,
                0, 5, 0, 0, 0, 0, 0, 0,
                6, 0, 0, 0, 0, 0, 0, 6};

            return tokens;
        }
    }
}
