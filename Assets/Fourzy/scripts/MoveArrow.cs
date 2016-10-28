using UnityEngine;
using System.Collections;

namespace Fourzy
{
    public class MoveArrow : MonoBehaviour {

        public Fourzy.GameManager.Direction direction;
        bool mouseButtonPressed = false;

        void OnMouseDown() {
            Debug.Log("mouseButtonPressed: " + mouseButtonPressed);

            if (!mouseButtonPressed)
            {
                mouseButtonPressed = true;
                int row = gameObject.GetComponentInParent<CornerSpot>().row;
                int column = gameObject.GetComponentInParent<CornerSpot>().column;
                StartCoroutine(GameManager.instance.movePiece(column, row, direction));
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
