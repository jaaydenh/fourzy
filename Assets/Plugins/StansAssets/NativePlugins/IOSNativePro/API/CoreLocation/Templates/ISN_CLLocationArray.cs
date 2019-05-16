using System;
using System.Collections.Generic;

using UnityEngine;

namespace SA.iOS.CoreLocation
{
    /// <summary>
    /// Object that holds locations provided by the location service. 
    /// </summary>
    [Serializable]
    public class ISN_CLLocationArray
    {
        [SerializeField] List<ISN_CLLocation> m_locations = null;

        /// <summary>
        /// Locations Array.
        /// </summary>
        public List<ISN_CLLocation> Locations {
            get {
                return m_locations;
            }
        }
    }
}