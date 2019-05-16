using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA.iOS.UIKit
{
    /// <summary>
    /// Styles to apply to action buttons in an alert.
    /// </summary>
    public enum ISN_UIAlertActionStyle 
    {
        Default, //Apply the default style to the action’s button.
        Cancel,  //Apply a style that indicates the action cancels the operation and leaves things unchanged.
        Destructive //Apply a style that indicates the action might change or delete data.
    }
}