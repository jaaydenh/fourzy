using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyToken : MonoBehaviour {

    private AudioSource as1;

	void Start () {
        as1 = gameObject.GetComponent<AudioSource>();
	}

    void OnTriggerEnter2D(Collider2D coll) {
        if (coll.gameObject.tag == "GamePiece") {
            Debug.Log("STICKY PIECE COLLISION");
            as1.Play();
        }
    }
}
