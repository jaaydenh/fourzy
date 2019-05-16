using System;
using UnityEngine;

namespace SA.iOS.CoreLocation
{
    /// <summary>
    /// A circular geographic region, specified as a center point and radius.
    /// </summary>
    [Serializable]
    public class ISN_CLCircularRegion : ISN_CLRegion
    {

        [SerializeField] float m_radius;
        [SerializeField] ISN_CLLocationCoordinate2D m_center;


        /// <summary>
        /// Initializes and returns a region object defining a circular geographic area.
        /// </summary>
        /// <param name="center">The center point of the geographic region to monitor.</param>
        /// <param name="radius">The distance (measured in meters) from the center point of the geographic region to the edge of the circular boundary.</param>
        /// <param name="identifier">
        /// A unique identifier to associate with the region object. You use this identifier to differentiate regions within your application. 
        /// This value must not be <c>null</c>.
        /// </param>
        public ISN_CLCircularRegion(ISN_CLLocationCoordinate2D center, float radius, string identifier):base(identifier) {
            m_center = center;
            m_radius = radius;
        }


        /// <summary>
        /// The radius (measured in meters) that defines the geographic area’s outer boundary.
        /// </summary>
        public float Radius {
            get {
                return m_radius;
            }
        }


        /// <summary>
        /// The center point of the geographic area.
        /// </summary>
        public ISN_CLLocationCoordinate2D Center {
            get {
                return m_center;
            }
        }
    }
}
