using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy {
    
    public class MovingGamePiece {

        public Direction currentDirection;
        public bool isActive;
        public List<Position> positions;
        public Position position;

        public MovingGamePiece(Position position, Direction direction) {
            positions = new List<Position>();
            this.position = position;
            positions.Add(position);
            currentDirection = direction;
        }
    	
        public Position GetCurrentPosition() {
            return positions[positions.Count - 1];
        }

        // position is a row and column
        public Position GetNextPosition() {
            Position nextPosition = new Position(0,0);
            Position currentPosition = positions[positions.Count - 1];

            switch (currentDirection)
            {
                case Direction.UP:
                    nextPosition.column = currentPosition.column;
                    nextPosition.row = currentPosition.row - 1;
                    break;
                case Direction.DOWN:
                    nextPosition.column = currentPosition.column;
                    nextPosition.row = currentPosition.row + 1;
                    break;
                case Direction.LEFT:
                    nextPosition.column = currentPosition.column - 1;
                    nextPosition.row = currentPosition.row;
                    break;
                case Direction.RIGHT:
                    nextPosition.column = currentPosition.column + 1;
                    nextPosition.row = currentPosition.row;
                    break;
                default:
                    break;
            }

            return nextPosition;
        }
    }
}