using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptySpot : MonoBehaviour {

    public bool isEdgeSpot;
    public bool hasPiece;
    public Animator anim;

    public IEnumerator AnimateSpot(bool animate) {
        yield return new WaitUntil(() => anim.isInitialized);
        if (isEdgeSpot && !hasPiece) {
            anim.SetBool("isPlayerTurn", animate);    
        }
    }
}
