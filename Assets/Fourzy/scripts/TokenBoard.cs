using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy {
    
    public class TokenBoard : Singleton<TokenBoard> {

        public int[] FindTokenBoard() {

            int boardIndex = Random.Range(0, 3);
            int[] tokenBoard = new int[64];

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
                default:
                    break;
            }

            return tokenBoard;
        }

        public int[] TokenBoard1() {
            int[] tokens = new int[] {
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 2, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 3, 0, 0,
                0, 0, 4, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 1, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0};

            return tokens;
        }
            
        public int[] TokenBoard2() {
            int[] tokens = new int[] {
                0, 0, 0, 0, 0, 0, 0, 4,
                0, 0, 0, 0, 0, 0, 4, 0,
                0, 0, 0, 0, 0, 4, 0, 0,
                0, 0, 0, 0, 4, 0, 0, 0,
                0, 0, 0, 4, 0, 0, 0, 0,
                0, 0, 4, 0, 0, 0, 0, 0,
                0, 4, 0, 0, 0, 0, 0, 0,
                4, 0, 0, 0, 0, 0, 0, 0};

            return tokens;
        }

        public int[] TokenBoard3() {
            int[] tokens = new int[] {
                0, 0, 0, 0, 1, 0, 0, 0,
                0, 0, 0, 0, 1, 0, 0, 0,
                0, 0, 0, 0, 1, 0, 0, 0,
                0, 0, 0, 0, 1, 0, 0, 0,
                0, 0, 0, 2, 0, 0, 0, 0,
                0, 0, 0, 2, 0, 0, 0, 0,
                0, 0, 0, 2, 0, 0, 0, 0,
                0, 0, 0, 2, 0, 0, 0, 0};

            return tokens;
        }
    }
}
