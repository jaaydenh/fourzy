using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyToken : MonoBehaviour {

    private AudioSource as1;

	void Start () {
        as1 = gameObject.GetComponent<AudioSource>();
	}

    void OnTriggerEnter2D(Collider2D coll) {
        //Debug.Log("play stick sound");
        if (coll.gameObject.tag == "GamePiece") {
            as1.Play();
        }
    }
}
