using System;
using UnityEngine;


namespace SA.iOS.CoreLocation
{

    /// <summary>
    /// The latitude and longitude associated with a location, specified using the WGS 84 reference frame.
    /// </summary>
    [Serializable]
    public class ISN_CLLocationCoordinate2D 
    {

        [SerializeField] float m_latitude;
        [SerializeField] float m_longitude;


        /// <summary>
        /// Initializes a new instance of the <see cref="ISN_CLLocationCoordinate2D"/> class.
        /// </summary>
        /// <param name="latitude">he latitude in degrees.</param>
        /// <param name="longitude">The longitude in degrees.</param>
        public ISN_CLLocationCoordinate2D(float latitude, float longitude) {
            m_latitude = latitude;
            m_longitude = longitude;
        }


        /// <summary>
        /// The latitude in degrees.
        /// 
        /// Positive values indicate latitudes north of the equator. 
        /// Negative values indicate latitudes south of the equator.
        /// </summary>
        /// <value>The latitude.</value>
        public float Latitude {
            get {
                return m_latitude;
            }

            set {
                m_latitude = value;
            }
        }

        /// <summary>
        /// The longitude in degrees.
        /// 
        /// Measurements are relative to the zero meridian, 
        /// with positive values extending east of the meridian and negative values extending west of the meridian.
        /// </summary>
        /// <value>The longitude.</value>
        public float Longitude {
            get {
                return m_longitude;
            }

            set {
                m_longitude = value;
            }
        }
    }
}