using System;
using UnityEngine;

namespace SA.iOS.CoreLocation
{
    /// <summary>
    /// The latitude, longitude, and course information reported by the system.
    /// </summary>
    [Serializable]
    public class ISN_CLLocation 
    {
        [SerializeField] double m_altitude = 0;
        [SerializeField] int m_floor = 0;
        [SerializeField] double m_speed = 0;
        [SerializeField] double m_course = 0;
        [SerializeField] long m_timestamp = 0;
        [SerializeField] ISN_CLLocationCoordinate2D m_coordinate = null;

        /// <summary>
        /// The altitude, measured in meters.
        /// Positive values indicate altitudes above sea level.
        /// Negative values indicate altitudes below sea level.
        /// </summary>
        public double Altitude {
            get {
                return m_altitude;
            }
        }

        /// <summary>
        /// The logical floor of the building in which the user is located.
        /// If floor information is not available for the current location, the value of this property is -1.
        /// </summary>
        public int Floor {
            get {
                return m_floor;
            }
        }
        
        /// <summary>
        /// The instantaneous speed of the device, measured in meters per second.
        ///
        /// This value reflects the instantaneous speed of the device as it moves in the direction of its current heading.
        /// A negative value indicates an invalid speed.
        /// Because the actual speed can change many times between the delivery of location events,
        /// use this property for informational purposes only.
        /// </summary>
        public double Speed {
            get {
                return m_speed;
            }
        }

        /// <summary>
        /// The direction in which the device is traveling, measured in degrees and relative to due north.
        /// Course values are measured in degrees starting at due north and continue clockwise around the compass.
        /// Thus, north is 0 degrees, east is 90 degrees, south is 180 degrees, and so on.
        /// Course values may not be available on all devices. A negative value indicates that the course information is invalid.
        /// </summary>
        public double Course {
            get {
                return m_course;
            }
        }

        /// <summary>
        /// The time at which this location was determined.
        /// </summary>
        public long Timestamp {
            get {
                return m_timestamp;
            }
        }

        /// <summary>
        /// The geographical coordinate information.
        /// When running in the simulator, Core Location uses the values provided to it by the simulator.
        /// You must run your application on an iOS-based device to get the actual location of that device.
        /// </summary>
        public ISN_CLLocationCoordinate2D Coordinate {
            get {
                return m_coordinate;
            }
        }
    }
}