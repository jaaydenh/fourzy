using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy {
    public class AiPlayer {

        string profile = "default";

        public AiPlayer(string profile) {
            this.profile = profile;
    	}

        // gameboard, tokenboard, player 1-2 turn, num rows, num cols
        public Move GetMove(GameObject[,] gamePieces, IToken[,] tokenBoard, int player)  {
            

            int numRows = Constants.numRows;
            int numCols = Constants.numColumns;
            int movePosition;
            Move move;
            Direction direction;
            bool canMove = true;
            do
            {
                movePosition = Random.Range(0, 7);
                //int dir = Random.Range(0,3);
                move = new Move(movePosition, Direction.DOWN);
                MovingGamePiece piece = new MovingGamePiece(move.position, Direction.DOWN);
                if (GameManager.instance.CanMoveInPosition(new Position(0,0), piece.GetNextPosition(), Direction.DOWN)) {
                    canMove = true;
                }
            } while (!canMove);

            return move;
    	}
    }
}