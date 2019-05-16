using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA.iOS.UserNotifications
{
    /// <summary>
    /// Constants indicating whether the app is allowed to schedule notifications.
    /// </summary>
    public enum ISN_UNAuthorizationStatus 
    {
        // The user has not yet made a choice regarding whether the application may post user notifications.
        NotDetermined = 0,
    
        // The application is not authorized to post user notifications.
        Denied,
    
        // The application is authorized to post user notifications.
        Authorized
    }
}