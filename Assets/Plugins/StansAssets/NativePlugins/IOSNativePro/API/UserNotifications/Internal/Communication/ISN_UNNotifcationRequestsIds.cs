using System;
using System.Collections.Generic;
using UnityEngine;

namespace SA.iOS.UserNotifications.Internal
{
    [Serializable]
    public class ISN_UNNotifcationRequestsIds 
    {

        [SerializeField] List<string> m_notificationIds;


        public ISN_UNNotifcationRequestsIds(List<string> notificationIds) {
            m_notificationIds = notificationIds;
        }

        public List<string> NotificationIds {
            get {
                return m_notificationIds;
            }
        }

    }
}