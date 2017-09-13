using UnityEngine;

namespace Fourzy
{
    public class CameraSize : MonoBehaviour 
    {
        Camera cam;
        public float widthToBeSeen = 8.2f;

        void Awake () 
        {
            cam = GetComponent<Camera>();
        }

        void LateUpdate()
        {
            cam.orthographicSize = widthToBeSeen * Screen.height / Screen.width * 0.5f;
        }
    }
}
