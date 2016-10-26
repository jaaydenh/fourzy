using UnityEngine;
using System.Collections;

namespace ConnectFour
{
    public class MoveArrow : MonoBehaviour {

        public ConnectFour.GameManager.Direction direction;
    	// Use this for initialization
    	void Start () {
            
    	}
    	
        IEnumerator OnMouseDown() {
            Debug.Log("movearrow");
            int row = gameObject.GetComponentInParent<CornerSpot>().row;
            int column = gameObject.GetComponentInParent<CornerSpot>().column;
            yield return StartCoroutine(GameManager.instance.movePiece(column, row, direction));

            //gameObject.GetComponentInParent<CornerSpot>().HideArrows();
        }

    	// Update is called once per frame
    	void Update () {
    	
    	}
    }
}
