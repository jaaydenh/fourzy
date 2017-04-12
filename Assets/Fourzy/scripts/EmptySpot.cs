using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptySpot : MonoBehaviour {

    public bool isEdgeSpot;
    public bool hasPiece;
    public Animator anim;

    public IEnumerator AnimateSpot(bool animate) {
        yield return new WaitUntil(() => anim.isInitialized);
        //Debug.Log("AnimateEmptyEdgeSpots before");
        if (isEdgeSpot && !hasPiece) {
          //  Debug.Log("AnimateEmptyEdgeSpots after");
            anim.SetBool("isPlayerTurn", animate);    
        }
    }
}
