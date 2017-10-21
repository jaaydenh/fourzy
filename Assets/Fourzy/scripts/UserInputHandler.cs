using UnityEngine;
using System.Collections;

public class UserInputHandler : MonoBehaviour {

    public delegate void TapAction(Vector3 pos);
    public static event TapAction OnTap;
    public static bool inputEnabled = true;
    bool mouseButtonPressed = false;
    public MoveHintTouchArea moveHintTouchArea;
    public GameObject moveHintAreas;
    public GameObject topHintArea;
    public GameObject bottomHintArea;
    public GameObject leftHintArea;
    public GameObject rightHintArea;
    Vector3 touchPosWorld;

    void Update () {
        // TODO: use touch events
        if (Input.GetMouseButtonDown(0) && !mouseButtonPressed && inputEnabled) {
            mouseButtonPressed = true;
            if (OnTap != null)
                OnTap(Input.mousePosition);
        } else {
            mouseButtonPressed = false;
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
            //We transform the touch position into word space from screen space and store it.
            touchPosWorld = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            //Debug.Log("Input.touchCount: " + Input.touchCount);
             Vector2 touchPosWorld2D = new Vector2(touchPosWorld.x, touchPosWorld.y);
 
             //We now raycast with this information. If we have hit something we can process it.
             RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld2D, Camera.main.transform.forward);
 
             if (hitInformation.collider != null) {
                 //We should have hit something with a 2D Physics collider!
                 GameObject touchedObject = hitInformation.transform.gameObject;
                 //touchedObject should be the object someone touched.
                 //Debug.Log("Touched " + touchedObject.transform.name);
                 if (touchedObject.transform.name == "MoveHintTouchArea") {
                    moveHintTouchArea.FadeInAndOutSprite();
                 }
             }
         }
    }
}
