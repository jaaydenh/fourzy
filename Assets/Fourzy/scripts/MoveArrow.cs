using UnityEngine;
using System.Collections;

namespace Fourzy
{
    public class MoveArrow : MonoBehaviour {

        public Fourzy.GameManager.Direction direction;
        bool mouseButtonPressed = false;

        void OnMouseDown() {
            if (!mouseButtonPressed && Fourzy.GameManager.instance.isCurrentPlayerTurn)
            {
                mouseButtonPressed = true;
                int row = gameObject.GetComponentInParent<CornerSpot>().row;
                int column = gameObject.GetComponentInParent<CornerSpot>().column;
                if (direction == Fourzy.GameManager.Direction.Down) {
                    StartCoroutine(GameManager.instance.MovePiece(column, direction, false));
                } else if (direction == Fourzy.GameManager.Direction.Up) {
                    StartCoroutine(GameManager.instance.MovePiece(column, direction, false));
                } else if (direction == Fourzy.GameManager.Direction.Right) {
                    StartCoroutine(GameManager.instance.MovePiece(row, direction, false));
                } else if (direction == Fourzy.GameManager.Direction.Left) {
                    StartCoroutine(GameManager.instance.MovePiece(row, direction, false));
                }
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
