using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA.iOS.UserNotifications
{
    /// <summary>
    /// Constants indicating the types of alerts that can be displayed to the user.
    /// </summary>
    public enum ISN_UNAlertStyle 
    {
        //No alerts. Alerts are not displayed for the app.
        None = 0,

        //Banner alerts. Alerts are displayed as a slide-down banner. 
        //Banners appear for a short time and then disappear automatically if the user does nothing.
        Banner,

        //Modal alerts. Alerts are displayed in a modal window that must be dismissed explicitly by the user.
        Alert,
    }
}