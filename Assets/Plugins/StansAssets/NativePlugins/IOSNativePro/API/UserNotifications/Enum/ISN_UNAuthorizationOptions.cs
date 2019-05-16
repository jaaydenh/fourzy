using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA.iOS.UserNotifications
{
    /// <summary>
    /// Constants for requesting authorization to interact with the user.
    /// </summary>
    public static class ISN_UNAuthorizationOptions 
    {
        //The ability to update the app’s badge.
        public const int Badge = (1 << 0);

        //The ability to play sounds.
        public const int Sound = (1 << 1);

        //The ability to display alerts.
        public const int Alert = (1 << 2);

        //The ability to display notifications in a CarPlay environment.
        public const int CarPlay = (1 << 3);

    }
}