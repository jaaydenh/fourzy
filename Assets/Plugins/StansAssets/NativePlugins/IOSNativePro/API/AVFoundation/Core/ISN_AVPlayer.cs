using System;
using System.Collections.Generic;
using UnityEngine;

using SA.iOS.Foundation;


namespace SA.iOS.AVFoundation
{

    /// <summary>
    /// An object that provides the interface to control the player’s transport behavior.
    /// </summary>
    [Serializable]
    public class ISN_AVPlayer 
    {

        [SerializeField] ISN_NSURL m_url;
        [SerializeField] float m_volume = 1f;

        /// <summary>
        /// Initializes a new player to play a single audiovisual resource referenced by a given URL.
        /// </summary>
        /// <param name="url">A URL that identifies an audiovisual resource.</param>
        public ISN_AVPlayer(ISN_NSURL url) {
            m_url = url;
        }


        /// <summary>
        /// The audio playback volume for the player, ranging from 0.0 through 1.0 on a linear scale.
        /// 
        /// A value of 0.0 indicates silence; a value of 1.0 (the default) 
        /// indicates full audio volume for the player instance.
        /// 
        /// This property is used to control the player audio volume relative to the system volume. 
        /// There is no programmatic way to control the system volume in iOS, 
        /// but you can use the MediaPlayer framework’s MPVolumeView class 
        /// to present a standard user interface for controlling system volume.
        /// MPVolumeView is not yet implemented with the IOS Native plugin
        /// </summary>
        public float Volume {
            get {
                return m_volume;
            }

            set {
                m_volume = value;
            }
        }

        /// <summary>
        /// A URL that identifies an audiovisual resource.
        /// </summary>
        public ISN_NSURL Url {
            get {
                return m_url;
            }
        }
    }
}