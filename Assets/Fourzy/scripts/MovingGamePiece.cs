using System.Collections.Generic;
using UnityEngine;

namespace Fourzy {
    public class MovingGamePiece {

        public Direction currentDirection;
        public List<Position> positions;
        public Position position;
        public Player player = Player.NONE;
        public GameObject gamePieceObject;
        public bool isDestroyed;
        public PieceAnimState animationState;

        public MovingGamePiece(Move move) {
            positions = new List<Position>();
            this.position = move.position;
            positions.Add(position);
            currentDirection = move.direction;
            this.player = move.player;
            this.isDestroyed = false;
            this.animationState = PieceAnimState.NONE;
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
    }
}