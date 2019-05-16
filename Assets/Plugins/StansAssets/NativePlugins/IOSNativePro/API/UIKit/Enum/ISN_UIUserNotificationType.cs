using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace SA.iOS.UIKit
{
    /// <summary>
    /// Constants indicating how the app alerts the user when a local or push notification arrives.
    /// </summary>
    public enum ISN_UIUserNotificationType
    {
       None = 0,       // the application may not present any UI upon a notification being received
       Badge = 1 << 0, // the application may badge its icon upon a notification being received
       Sound = 1 << 1, // the application may play a sound upon a notification being received
       Alert = 1 << 2, // the application may display an alert upon a notification being received
    }
}