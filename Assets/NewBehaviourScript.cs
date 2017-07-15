using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour {

public float height;
 public RectTransform rt;

    // Use this for initialization
    void Start ()
    {
        height = Screen.height;
		
        Vector2 newSize = new Vector2(height / 5, height / 5);
		Debug.Log("newSize: " + newSize);
        this.gameObject.GetComponent<GridLayoutGroup>().cellSize = newSize;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
