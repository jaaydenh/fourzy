using UnityEngine;

public class tokenSticky : MonoBehaviour {

    private AudioSource as1;

    void Start () {
        as1 = gameObject.GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D coll) {
        if (coll.gameObject.tag == "GamePiece") {
            as1.Play();
        }
    }
}
