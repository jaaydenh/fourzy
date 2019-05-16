using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.iOS.AVFoundation.Internal;

namespace SA.iOS.AVFoundation
{
    /// <summary>
    /// An object that provides thumbnail or preview images of assets independently of playback.
    /// </summary>
    public class ISN_AVAssetImageGenerator
    {


        /// <summary>
        /// Returns a CGImage for the asset at or near a specified time.
        /// </summary>
        public static Texture2D CopyCGImageAtTime(string movieUrl, double seconds) {
            return ISN_AVLib.API.CopyCGImageAtTime(movieUrl, seconds);
        }

    }
}