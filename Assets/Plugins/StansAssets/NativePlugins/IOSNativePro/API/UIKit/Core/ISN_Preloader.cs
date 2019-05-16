using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.iOS.UIKit.Internal;


namespace SA.iOS.UIKit {
    /// <summary>
    /// Calss allows to show preloaders and lock application screen
    /// </summary>
    public static class ISN_Preloader
    {

        /// <summary>
        /// Locks the screen and displayes a preloader spinner
        /// </summary>
        public static void LockScreen() {
            ISN_UILib.API.PreloaderLockScreen();
        }


        /// <summary>
        /// Unlocks the screen and hide a preloader spinner
        /// In case there is no preloader displayed, method does nothing
        /// </summary>
        public static void UnlockScreen() {
            ISN_UILib.API.PreloaderUnlockScreen();
        }

    }
}