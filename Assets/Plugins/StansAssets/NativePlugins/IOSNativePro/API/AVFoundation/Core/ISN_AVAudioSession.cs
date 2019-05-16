using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Templates;
using SA.Foundation.Events;
using SA.iOS.AVFoundation.Internal;

namespace SA.iOS.AVFoundation {

    /// <summary>
    /// An intermediary object that communicates to the system how you intend to use audio in your app.
    /// 
    /// An audio session acts as an intermediary between your app and the operating system—and, 
    /// in turn, the underlying audio hardware. 
    /// You use an audio session to communicate to the operating system the nature of your app’s audio without detailing the specific behavior 
    /// or required interactions with the audio hardware. 
    /// This behavior delegates the management of those details to the audio session, 
    /// which ensures that the operating system can best manage the user’s audio experience.
    /// </summary>
    public static class ISN_AVAudioSession 
    {

        //--------------------------------------
        // Public Methods
        //--------------------------------------
       

        /// <summary>
        /// Sets the current audio session category.
        /// 
        /// The audio session's category defines how the app intends to use audio. 
        /// Typically, you set the category before activating the session. 
        /// You can also set the category while the session is active, but this results in an immediate route change.
        /// </summary>
        /// <returns>Returns operation result info</returns>
        /// <param name="category">The audio session category to apply to the audio session.</param>
        public static SA_Result SetCategory(ISN_AVAudioSessionCategory category) {
            return ISN_AVLib.API.AudioSessionSetCategory(category);
        }

        /// <summary>
        /// Activates or deactivates your app’s audio session.
        /// 
        /// If another active audio session has higher priority than yours (for example, a phone call), 
        /// and neither audio session allows mixing, attempting to activate your audio session fails. 
        /// Deactivating your session will fail if any associated audio objects (such as queues, converters, players, or recorders) are currently running.
        /// </summary>
        /// <returns>Returns operation result info<</returns>
        /// <param name="isActive">Use <c>true</c> to activate your app’s audio session, or <c>false</c> to deactivate it.</param>
        public static SA_Result SetActive(bool isActive)  {
            return ISN_AVLib.API.AudioSessionSetActive(isActive);
        }


        //--------------------------------------
        // Get / Set
        //--------------------------------------

        /// <summary>
        /// The current audio session category.
        /// An audio session category defines a set of audio behaviors for your app.
        /// The default category assigned to an audio session is <see cref="ISN_AVAudioSessionCategory.SoloAmbient"/>.
        /// </summary>
        public static ISN_AVAudioSessionCategory Category {
            get {
                return ISN_AVLib.API.AudioSessionCategory;
            }
        }


        /// <summary>
        /// The event is posted when the system’s audio route changes.
        /// </summary>
        /// <value>The on audio session route change.</value>
        public static SA_iEvent<ISN_AVAudioSessionRouteChangeReason> OnAudioSessionRouteChange {
            get {
                return ISN_AVLib.API.OnAudioSessionRouteChange;
            }
        }

    }
}