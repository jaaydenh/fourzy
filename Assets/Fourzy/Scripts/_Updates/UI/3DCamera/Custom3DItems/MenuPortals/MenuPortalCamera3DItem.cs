//@vadym udod

using System;
using UnityEngine;

namespace Fourzy._Updates.UI.Camera3D
{
    public class MenuPortalCamera3DItem : Camera3DItem
    {
        public PortalStyle defaultPortalStyle = PortalStyle.BLUE;
        public PortalStyleObjectPair[] portals;

        /// <summary>
        /// Allows to enable different portals to be rendered
        /// </summary>
        /// <param name="style">Can set multiple e.g. PortalStyle.RED | PortalStyle.BLUE</param>
        public void ShowPortal(PortalStyle style)
        {
            foreach (PortalStyleObjectPair pair in portals)
                pair.portalObject.SetActive(style.HasFlag(pair.portalStyle));
        }

        public override void Init()
        {
            base.Init();

            ShowPortal(defaultPortalStyle);
        }

        [Serializable]
        public struct PortalStyleObjectPair
        {
            public GameObject portalObject;
            public PortalStyle portalStyle;
        }

        //public 
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
