using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy {
    
    public class MovingGamePiece {

        public Direction currentDirection;
        public bool isActive;
        public List<Position> positions;
        public Position position;
        public Player player;
        public bool swapPiece = true;
        public GameObject gamePieceObject;

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
            return GetNextPositionWithDirection(currentDirection);
        }

        // position is a row and column
        public Position GetNextPositionWithDirection(Direction direction) {
            Position nextPosition = new Position(0,0);
            Position currentPosition = positions[positions.Count - 1];

            switch (direction)
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


        // o > o o o o
        // o ^ < o o o 
        // o o ^ < o o
        // o o o o o o

//        public void SimplifyMovePositions() {
//            if (positions.Count > 2) {
//                int x = positions.Count - 1;
//                int column = -1;
//                int row = -1;
//                
//                while (x > 1)
//                {
//                    if (positions[x - 1].column == positions[x - 2].column) {
//                        column = positions[x - 1].column;
//                        if (positions[x].column == positions[x - 1].column) {
//                            
//                            positions.RemoveAt(x - 1);
//                            //x--;
//                        }
//                    }
//                    if (positions[x - 1].row == positions[x - 2].row) {
//                        row = positions[x - 1].row;
//                        if (positions[x].row == positions[x-1].row) {
//                            positions.RemoveAt(x - 1);
//                            //x--;
//                        }
//                    }
//                    x--;
//                }
//                //for (int x = 0; x < positions.Count; x++) {
//                  //  if (positions[x])
//                //}
//            }
//        }
    }
}