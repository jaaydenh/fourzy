using System;
using System.Collections.Generic;
using UnityEngine;


namespace SA.iOS.UIKit.Internal
{
    [Serializable]
    internal class ISN_UIAlertActionId
    {
        [SerializeField] private int m_alertId = 0;
        [SerializeField] private int m_actionId = 0;

        public int AlertId {
            get {
                return m_alertId;
            }
        }

        public int ActionId {
            get {
                return m_actionId;
            }
        }
    }
}