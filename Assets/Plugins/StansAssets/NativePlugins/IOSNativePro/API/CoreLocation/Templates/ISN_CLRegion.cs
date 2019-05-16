using System;
using UnityEngine;

namespace SA.iOS.CoreLocation
{
    /// <summary>
    /// An area that can be monitored.
    /// </summary>
    [Serializable]
    public abstract class ISN_CLRegion 
    {
        [SerializeField] string m_identifier;
        [SerializeField] bool m_notifyOnEntry = true;
        [SerializeField] bool m_notifyOnExit = true;


        public ISN_CLRegion(string identifier) {
            m_identifier = identifier;
        }

        /// <summary>
        /// The identifier for the region object.
        /// This is a value that you specify and can use to identify this region inside your application.
        /// </summary>
        public string Identifier {
            get {
                return m_identifier;
            }
        }


        /// <summary>
        /// A Boolean indicating that notifications are generated upon entry into the region.
        /// 
        /// When this property is <c>true</c>, a device crossing from outside the region 
        /// to inside the region triggers the delivery of a notification. 
        /// If the property is <c>false</c>, a notification is not generated. 
        /// The default value of this property is <c>true</c>.
        /// 
        /// If the app is not running when a boundary crossing occurs, 
        /// the system launches the app into the background to handle it. 
        /// Upon launch, your app must configure new location manager and delegate objects to receive the notification. 
        /// The notification is sent to your delegate’s locationManager:didEnterRegion: method.
        /// </summary>
        public bool NotifyOnEntry {
            get {
                return m_notifyOnEntry;
            }

            set {
                m_notifyOnEntry = value;
            }
        }

        /// <summary>
        /// A Boolean indicating that notifications are generated upon exit from the region.
        /// 
        /// When this property is <c>true</c>, a device crossing from inside the region to outside the region triggers 
        /// the delivery of a notification. If the property is <c>false</c>, 
        /// a notification is not generated. The default value of this property is <c>true</c>.
        /// 
        /// If the app is not running when a boundary crossing occurs, 
        /// the system launches the app into the background to handle it. 
        /// Upon launch, your app must configure new location manager and delegate objects to receive the notification. 
        /// The notification is sent to your delegate’s locationManager:didExitRegion: method.
        /// </summary>
        public bool NotifyOnExit {
            get {
                return m_notifyOnExit;
            }

            set {
                m_notifyOnExit = value;
            }
        }
    }
}