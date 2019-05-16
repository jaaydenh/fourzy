using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.iOS.Contacts {

    /// <summary>
    /// An authorization status the user can grant for an app to access the specified entity type.
    /// </summary>
    public enum ISN_CNAuthorizationStatus 
    {
        /*! The user has not yet made a choice regarding whether the application may access contact data. */
        NotDetermined = 0,
        /*! The application is not authorized to access contact data.
         *  The user cannot change this application’s status, possibly due to active restrictions such as parental controls being in place. */
        Restricted,
        /*! The user explicitly denied access to contact data for the application. */
        Denied,
        /*! The application is authorized to access contact data. */
        Authorized
    }
}

