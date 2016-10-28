using UnityEngine;
using System.Collections;

public class CornerSpot : MonoBehaviour {

    public int row;
    public int column;
    public bool rightArrowActive = false;
    public bool leftArrowActive = false;
    public bool upArrowActive = false;
    public bool downArrowActive = false;
    GameObject RightArrow;
    GameObject LeftArrow;
    GameObject UpArrow;
    GameObject DownArrow;

	void Start () {
        RightArrow = GameObject.Find("RightArrow");
        LeftArrow = GameObject.Find("LeftArrow");
        UpArrow = GameObject.Find("UpArrow");
        DownArrow = GameObject.Find("DownArrow");

        HideArrows();
	}
	
    public void HideArrows() {
        RightArrow.SetActive(false);
        LeftArrow.SetActive(false);
        UpArrow.SetActive(false);
        DownArrow.SetActive(false);
    }

    void OnMouseDown() {
        if (rightArrowActive)
        {
            RightArrow.SetActive(true);
        }
        if (leftArrowActive)
        {
            LeftArrow.SetActive(true);
        }
        if (upArrowActive)
        {
            UpArrow.SetActive(true);
        }
        if (downArrowActive)
        {
            DownArrow.SetActive(true);
        }
    }
}
