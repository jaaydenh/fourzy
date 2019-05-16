using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SA.iOS.AVFoundation
{

    /// <summary>
    /// Constants values indicating the reason for an audio route change.
    /// </summary>
    public enum ISN_AVAudioSessionRouteChangeReason
    {
        Unknown = 0,
        NewDeviceAvailable = 1,
        OldDeviceUnavailable = 2,
        CategoryChange = 3,
        Override = 4,
        WakeFromSleep = 6,
        NoSuitableRouteForCategory = 7,
        RouteConfigurationChange = 8
    }
}