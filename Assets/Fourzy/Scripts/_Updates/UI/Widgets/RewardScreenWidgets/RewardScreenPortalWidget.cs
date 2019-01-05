//@vadym udod

using Fourzy._Updates.Tween;
using System;
using UnityEngine;

namespace Fourzy._Updates.UI.Widgets
{
    /// <summary>
    /// Not sure if any other visuals will be needed for portal, so lets pack its functionality into separate class
    /// </summary>
    public class RewardScreenPortalWidget : MonoBehaviour
    {
        public PortalStyle defaultPortalStyle = PortalStyle.BLUE;
        public PositionTween positionTween;
        public PortalStyleObjectPair[] portals;

        protected void Start()
        {
            SetPortals(defaultPortalStyle);
        }

        public void OpenAnimation()
        {
            //play tween animation
            positionTween.PlayForward(true);
        }

        public void ResetAnimation()
        {
            positionTween.AtProgress(0f);
        }

        public void StopOpenAnimation()
        {
            positionTween.AtProgress(1f);
        }

        public void SetPortals(PortalStyle portalStyle)
        {
            foreach (PortalStyleObjectPair pair in portals)
                pair.portalObject.gameObject.SetActive(portalStyle.HasFlag(pair.portalStyle));
        }

        [Serializable]
        public struct PortalStyleObjectPair
        {
            public ParticleSystem portalObject;
            public PortalStyle portalStyle;
        }

        [Flags]
        public enum PortalStyle
        {
            BLUE,
            RED,
            GOLD,
            PURPLE,
        }
    }
}
