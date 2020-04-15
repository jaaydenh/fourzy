//@vadym udod

using Sirenix.OdinInspector;
using UnityEngine;

namespace Fourzy._Updates.Mechanics.GameplayScene
{
    public class GameplayBG : MonoBehaviour
    {
        public SpriteRenderer background;
        public bool configureAtStart = true;

        [EnumToggleButtons]
        public MatchSide ipadMatch = MatchSide.HEIGHT;
        [EnumToggleButtons]
        public MatchSide iphoneMatch = MatchSide.HEIGHT;
        [EnumToggleButtons]
        public MatchSide iphoneXMatch = MatchSide.WIDTH;

        protected void Awake()
        {
            if (configureAtStart) Configure();
        }

        public void Configure()
        {
            Camera _camera = Camera.main;

            if (Screen.width > Screen.height)
            {
                //widescreen
                float scaleBy = _camera.orthographicSize / background.bounds.extents.y;
                background.transform.localScale = new Vector3(scaleBy, scaleBy, 1f);
            }
            else
            {
                //portrait
                if (_camera.aspect > .7f)
                {
                    //ipad
                    if (ipadMatch == MatchSide.HEIGHT) MatchHeight(_camera);
                    else MatchWidth(_camera);
                }
                else if (_camera.aspect >= .5f)
                {
                    //iphone
                    if (iphoneMatch == MatchSide.HEIGHT) MatchHeight(_camera);
                    else MatchWidth(_camera);
                }
                else
                {
                    //iphonex
                    if (iphoneXMatch == MatchSide.HEIGHT) MatchHeight(_camera);
                    else MatchWidth(_camera);
                }
            }
        }

        private void MatchWidth(Camera _camera) => _camera.orthographicSize = background.bounds.extents.x / _camera.aspect;

        private void MatchHeight(Camera _camera) => _camera.orthographicSize = background.bounds.extents.y;

        public enum MatchSide
        {
            WIDTH,
            HEIGHT,
        }
    }
}