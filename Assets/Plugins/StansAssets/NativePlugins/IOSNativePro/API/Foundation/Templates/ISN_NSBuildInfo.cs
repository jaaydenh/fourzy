using System;
using UnityEngine;

namespace SA.iOS.Foundation
{

    /// <summary>
    /// Contains current build information
    /// </summary>
    [Serializable]
    public class ISN_NSBuildInfo
    {
        [SerializeField] string m_appVersion = null;
        [SerializeField] string m_buildNumber = null;

        /// <summary>
        /// Current App Version
        /// </summary>
        public string AppVersion {
            get {
                return m_appVersion;
            }
        }

        /// <summary>
        /// Current Build Number
        /// </summary>
        public string BuildNumber {
            get {
                return m_buildNumber;
            }
        }
    }
}
