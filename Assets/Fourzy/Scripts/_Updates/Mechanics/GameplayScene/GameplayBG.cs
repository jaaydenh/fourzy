//@vadym udod

using Sirenix.OdinInspector;
using UnityEngine;

namespace Fourzy._Updates.Mechanics.GameplayScene
{
    public class GameplayBG : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer background;
        [SerializeField]
        private bool configureAtStart = true;

        [EnumToggleButtons]
        public MatchSide ipadMatch = MatchSide.HEIGHT;
        [EnumToggleButtons]
        public MatchSide iphoneMatch = MatchSide.HEIGHT;
        [EnumToggleButtons]
        public MatchSide iphoneXMatch = MatchSide.WIDTH;

        protected void Awake()
        {
            if (configureAtStart)
            {
                Configure();
            }
        }

        public void Configure()
        {
            Camera _camera = Camera.main;

            bool matchHeight;
            if (Screen.width > Screen.height) //landscape
            {
                if (_camera.aspect > 1.78f)
                {
                    //iphonex
                    matchHeight = iphoneXMatch == MatchSide.HEIGHT;
                }
                else if (_camera.aspect >= 1.34f)
                {
                    //iphone
                    matchHeight = iphoneMatch == MatchSide.HEIGHT;
                }
                else
                {
                    //ipad
                    matchHeight = ipadMatch == MatchSide.HEIGHT;
                }
            }
            else //portrait
            {
                if (_camera.aspect > .74f)
                {
                    //ipad
                    matchHeight = ipadMatch == MatchSide.HEIGHT;
                }
                else if (_camera.aspect > .55f)
                {
                    //iphone
                    matchHeight = iphoneMatch == MatchSide.HEIGHT;
                }
                else
                {
                    //iphonex
                    matchHeight = iphoneXMatch == MatchSide.HEIGHT;
                }
            }

            if (matchHeight)
            {
                MatchHeight(_camera);
            }
            else
            {
                MatchWidth(_camera);
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