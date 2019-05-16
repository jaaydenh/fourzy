using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA.iOS.UserNotifications
{
    /// <summary>
    /// Constants indicating the types of alerts that can be displayed to the user.
    /// </summary>
    public enum ISN_UNShowPreviewsSetting 
    {
        // Notification previews are always shown.
        Always,

        // Notifications previews are only shown when authenticated.
       WhenAuthenticated,

        // Notifications previews are never shown.
        Never
    }
}