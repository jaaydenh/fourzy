using UnityEngine;
using System.Collections;

namespace Fourzy
{
    public class MoveArrow : MonoBehaviour {

        public Direction direction;
        bool mouseButtonPressed = false;

        void OnMouseDown() {
            if (!mouseButtonPressed && Fourzy.GameManager.instance.isCurrentPlayerTurn)
            {
                mouseButtonPressed = true;
                int row = gameObject.GetComponentInParent<CornerSpot>().row;
                int column = gameObject.GetComponentInParent<CornerSpot>().column;
                Position pos = new Position(column, row);
                StartCoroutine(GameManager.instance.MovePiece(pos, direction, false));

//                if (direction == Direction.DOWN) {
//                    StartCoroutine(GameManager.instance.NewMovePiece(column, direction, false));
//                } else if (direction == Direction.UP) {
//                    StartCoroutine(GameManager.instance.NewMovePiece(column, direction, false));
//                } else if (direction == Direction.RIGHT) {
//                    StartCoroutine(GameManager.instance.NewMovePiece(row, direction, false));
//                } else if (direction == Direction.LEFT) {
//                    StartCoroutine(GameManager.instance.NewMovePiece(row, direction, false));
//                }
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
