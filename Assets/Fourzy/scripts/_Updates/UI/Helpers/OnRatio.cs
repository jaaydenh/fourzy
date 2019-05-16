//@vadym udod

using ByteSheep.Events;
using UnityEngine;

namespace Fourzy._Updates.UI.Helpers
{
    [ExecuteInEditMode]
    public class OnRatio : MonoBehaviour
    {
        public bool continious = false;

        public DeviceOrientation orientation = DeviceOrientation.Portrait;
        public AdvancedEvent onIphone;
        public AdvancedEvent onIphoneX;
        public AdvancedEvent onIpad;

        private DisplayRatioOption ratio = DisplayRatioOption.NONE;
        private DisplayRatioOption previousRatio = DisplayRatioOption.NONE;

        protected void Start()
        {
            if (!Application.isPlaying)
                return;

            CheckOrientation();
        }

        protected void Update()
        {
            if (continious || Application.isEditor)
                CheckOrientation();
        }

        private void CheckOrientation()
        {
            //ratio check
            switch (orientation)
            {
                case DeviceOrientation.LandscapeLeft:
                case DeviceOrientation.LandscapeRight:
                    if (Camera.main.aspect > 2f)
                        ratio = DisplayRatioOption.IPHONEX;
                    else if (Camera.main.aspect > 1.5f)
                        ratio = DisplayRatioOption.IPHONE;
                    else
                        ratio = DisplayRatioOption.IPAD;
                    break;

                case DeviceOrientation.Portrait:
                case DeviceOrientation.PortraitUpsideDown:
                    if (Camera.main.aspect > .7f)
                        ratio = DisplayRatioOption.IPAD;
                    else if (Camera.main.aspect > .5f)
                        ratio = DisplayRatioOption.IPHONE;
                    else
                        ratio = DisplayRatioOption.IPHONEX;
                    break;
            }

            if (ratio != previousRatio)
            {
                OnRationChanged(ratio);
                previousRatio = ratio;
            }
        }

        private void OnRationChanged(DisplayRatioOption option)
        {
            if (onIphone == null || onIphoneX == null || onIpad == null)
                return;

            switch (option)
            {
                case DisplayRatioOption.IPHONE:
                    onIphone.Invoke();
                    break;

                case DisplayRatioOption.IPHONEX:
                    onIphoneX.Invoke();
                    break;

                case DisplayRatioOption.IPAD:
                    onIpad.Invoke();
                    break;
            }
        }

        public enum DisplayRatioOption
        {
            NONE,

            IPHONE,
            IPHONEX,
            IPAD,
        }
    }
}