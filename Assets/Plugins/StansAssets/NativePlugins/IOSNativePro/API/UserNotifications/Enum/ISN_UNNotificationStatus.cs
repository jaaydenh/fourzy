using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA.iOS.UserNotifications
{
    /// <summary>
    /// Constants indicating the current status of a notification setting.
    /// </summary>
    public enum ISN_UNNotificationStatus 
    {
        // The application does not support this notification type
        NotSupported = 0,

        // The notification setting is turned off.
        Disabled,

        // The notification setting is turned on.
        Enabled,
    }
}