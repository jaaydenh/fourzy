using UnityEngine;
using System.Collections;

public class UserInputHandler : MonoBehaviour {

    #region EVENTS
    // Gesture Events
    public delegate void TapAction(Vector3 pos);
    public static event TapAction OnTap;
    #endregion

    #region PUBLIC VARIABLES
    // Maximum pixels a tap can move.
    //public float tapMaxMovement = 50f;
    public static bool inputEnabled = true;
    #endregion

    #region PRIVATE VARIABLES
    //private Vector2 movement;

    //private bool tapGestureFailed = false; 
    bool mouseButtonPressed = false;

    #endregion

    #region MONOBEHAVIOUR METHODS
    void Update () {

        // if (Input.touchCount > 0 && !mouseButtonPressed && inputEnabled) {
        //     mouseButtonPressed = true;
        //     Debug.Log("touch: " + Input.touches[0].position.ToString());
        //     if (OnTap != null)
                
        //         OnTap(Input.touches[0].position);
        // } else {
        //     mouseButtonPressed = false;
        // }

        if (Input.GetMouseButtonDown(0) && !mouseButtonPressed && inputEnabled) {
            mouseButtonPressed = true;
            if (OnTap != null)
                OnTap(Input.mousePosition);
        } else {
            mouseButtonPressed = false;
        }

//        if (Input.touchCount > 0)
//        {
//            Touch touch = Input.touches[0];
//
//            if (touch.phase == TouchPhase.Began)
//            {
//                movement = Vector2.zero;
//            }
//            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
//            {
//                movement += touch.deltaPosition;
//
//                if (movement.magnitude > tapMaxMovement)
//                    tapGestureFailed = true;
//            }
//            else
//            {
//                if (!tapGestureFailed)
//                {
//                    if (OnTap != null)
//                        OnTap(touch);
//                }
//
//                tapGestureFailed = false;
//            }
//        }
    }
    #endregion
}
