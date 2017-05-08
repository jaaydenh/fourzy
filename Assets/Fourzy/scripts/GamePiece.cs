using UnityEngine;

namespace Fourzy
{

    public class GamePiece : MonoBehaviour {

        public Player player;
        public int column;
        public int row;
        public bool isMoveableUp = false;
        public bool isMoveableDown = false;
        public bool isMoveableLeft = false;
        public bool isMoveableRight = false;

        public void MakeMoveable(bool moveable, Direction direction) {
            switch (direction)
            {
                case Direction.UP:
                    isMoveableUp = moveable;
                    break;
                case Direction.DOWN:
                    isMoveableDown = moveable;
                    break;
                case Direction.LEFT:
                    isMoveableLeft = moveable;
                    break;
                case Direction.RIGHT:
                    isMoveableRight = moveable;
                    break;
                default:
                    break;
            }
        }

        public Position GetNextPosition(Direction direction) {
            Position nextPosition = new Position(0,0);

            switch (direction)
            {
                case Direction.UP:
                    nextPosition.column = column;
                    nextPosition.row = row - 1;
                    break;
                case Direction.DOWN:
                    nextPosition.column = column;
                    nextPosition.row = row + 1;
                    break;
                case Direction.LEFT:
                    nextPosition.column = column - 1;
                    nextPosition.row = row;
                    break;
                case Direction.RIGHT:
                    nextPosition.column = column + 1;
                    nextPosition.row = row;
                    break;
                default:
                    break;
            }

            return nextPosition;
        }
    }
}
