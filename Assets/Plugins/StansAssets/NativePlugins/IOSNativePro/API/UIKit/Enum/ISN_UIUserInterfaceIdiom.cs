using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA.iOS.UIKit
{

    /// <summary>
    /// The type of interface that should be used on the current device
    /// </summary>
    public enum ISN_UIUserInterfaceIdiom 
    {
        Unspecified = -1,
        Phone = 0, // iPhone and iPod touch style UI
        Pad = 1, // iPad style UI
        TV = 2, // Apple TV style UI
        CarPlay = 3 // CarPlay style UI
    }
}