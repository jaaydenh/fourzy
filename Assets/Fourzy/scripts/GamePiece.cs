using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy {
    
    public class GamePiece : MonoBehaviour {

        public Player player;
        public Position position;
        public bool isMoveable;

    	void Start () {
            isMoveable = false;
    	}

        public Position GetNextPosition(Direction direction) {
            Position nextPosition = new Position(0,0);

            switch (direction)
            {
                case Direction.UP:
                    nextPosition.column = position.column;
                    nextPosition.row = position.row - 1;
                    break;
                case Direction.DOWN:
                    nextPosition.column = position.column;
                    nextPosition.row = position.row + 1;
                    break;
                case Direction.LEFT:
                    nextPosition.column = position.column - 1;
                    nextPosition.row = position.row;
                    break;
                case Direction.RIGHT:
                    nextPosition.column = position.column + 1;
                    nextPosition.row = position.row;
                    break;
                default:
                    break;
            }

            return nextPosition;
        }
    }
}
