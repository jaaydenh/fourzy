//@vadym udod

using UnityEngine;

namespace Fourzy._Updates.Mechanics.GameplayScene
{
    public class GameplayBG : MonoBehaviour
    {
        public SpriteRenderer background;

        private bool configured = false;

        protected void Start()
        {
            FitCamera();
        }

        public void FitCamera()
        {
            if (configured)
                return;

            configured = true;

            Camera _camera = Camera.main;
            
            if (_camera.aspect < .51f)
                _camera.orthographicSize = background.bounds.extents.x / _camera.aspect;
            else if (_camera.aspect < .655f)
                _camera.orthographicSize = background.bounds.extents.x / _camera.aspect;
            else
                _camera.orthographicSize = background.bounds.extents.y;
        }
    }
}