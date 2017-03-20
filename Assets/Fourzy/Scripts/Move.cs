using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy {
    public class Move {

        public int location;
        public Position position;
        public Direction direction;

        public Move(int location, Direction direction) {
            this.location = location;
            this.direction = direction;

            Position pos = new Position(0,0);
            switch (direction)
            {
                case Direction.UP:
                    pos.column = location;
                    pos.row = Constants.numRows;
                    break;
                case Direction.DOWN:
                    pos.column = location;
                    pos.row = -1;
                    break;
                case Direction.LEFT:
                    pos.column = Constants.numColumns;
                    pos.row = location;
                    break;
                case Direction.RIGHT:
                    pos.column = -1;
                    pos.row = location;
                    break;
                default:
                    break;
            }

            this.position = pos;
        }
    }
}
