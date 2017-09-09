using UnityEngine;
using System.Collections;

namespace Fourzy
{
    public class MoveArrow : MonoBehaviour {

        public Direction direction;
        bool mouseButtonPressed = false;

        void OnMouseDown() {
            if (!mouseButtonPressed && Fourzy.GameManager.instance.gameState.isCurrentPlayerTurn)
            {
                mouseButtonPressed = true;
                int row = gameObject.GetComponentInParent<CornerSpot>().row;
                int column = gameObject.GetComponentInParent<CornerSpot>().column;
                Position pos = new Position(column, row);
                if (direction == Direction.DOWN) {
                    pos.row = -1;
                } else if (direction == Direction.UP) {
                    pos.row = Constants.numRows;
                } else if (direction == Direction.RIGHT) {
                    pos.column = -1;
                } else if (direction == Direction.LEFT) {
                    pos.column = Constants.numColumns;
                }

                Player player = GameManager.instance.gameState.isPlayerOneTurn ? Player.ONE : Player.TWO;
                Move move = new Move(pos, direction, player);

                StartCoroutine(GameManager.instance.ProcessMove(move, true));
            }
        }
            
        void Start () {
            GameManager.OnMoved += setMouseButtonPressed;
        }

        void setMouseButtonPressed() {
            mouseButtonPressed = false;
        }
    }
}
