//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.Mechanics._Camera
{
    [RequireComponent(typeof(Camera))]
    public class CameraSize : MonoBehaviour
    {
        [Tooltip("Target camera width")]
        public float width;
        public float thresholdValue = .5625f;

        private Camera _camera;

        protected void Awake()
        {
            _camera = GetComponent<Camera>();

            //adjust camera to match specified width
            if (_camera && _camera.aspect > thresholdValue)
                _camera.orthographicSize = width / _camera.aspect;
        }
    }
}