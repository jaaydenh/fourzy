using System;
using System.Collections.Generic;
using UnityEngine;

using SA.iOS.Foundation.Internal;

namespace SA.iOS.Foundation
{
    /// <summary>
    /// Information about standard time conventions associated with a specific geopolitical region.
    /// 
    /// Time zones represent the standard time policies for a geopolitical region. 
    /// Time zones have identifiers like “America/Los_Angeles” and can also be identified by abbreviations, 
    /// such as PST for Pacific Standard Time.
    /// </summary>
    [Serializable]
    public class ISN_NSTimeZone
    {

        [SerializeField] string m_name = null;
        [SerializeField] string m_description = null;
        [SerializeField] private int m_secondsFromGMT = 0;


        //--------------------------------------
        // Get / Set
        //--------------------------------------


        /// <summary>
        /// The geopolitical region ID that identifies the receiver.
        /// </summary>
        public string Name {
            get {
                return m_name;
            }
        }


        /// <summary>
        /// A textual description of the time zone including the name, abbreviation, 
        /// offset from GMT, and whether or not daylight saving time is currently in effect.
        /// </summary>
        public string Description {
            get {
                return m_description;
            }

        }


        /// <summary>
        /// The current difference in seconds between the receiver and Greenwich Mean Time.
        /// </summary>
        public int SecondsFromGMT {
            get {
                return m_secondsFromGMT;
            }
        }


        //--------------------------------------
        // Static Methods
        //--------------------------------------


        /// <summary>
        /// Clears any time zone value cached for the systemTimeZone property.
        /// 
        /// If the app has cached the system time zone by accessing the <see cref="SystemTimeZone"/> class property, 
        /// this method clears that cached value. 
        /// If you subsequently access the  <see cref="SystemTimeZone"/> class property, 
        /// a new time zone object is created and cached.
        /// </summary>
        public static void ResetSystemTimeZone() {
            ISN_NSLib.API.ResetSystemTimeZone();
        }


        /// <summary>
        /// Gets the local time zone.
        /// 
        /// If the current system time zone cannot be determined, the GMT time zone is used instead.
        ///
        /// If you access the  <see cref="SystemTimeZone"/> class property, 
        /// its value is cached by the app and doesn't update if the user subsequently changes the system time zone. 
        /// In order for the systemTimeZone property to reflect the new time zone, 
        /// you must first call the <see cref="ResetSystemTimeZone()"/> method to clear the cached value. 
        /// Then, the next time you access the <see cref="SystemTimeZone"/> property, 
        /// it returns the current system time zone, and caches that value.
        /// 
        /// If you access the  <see cref="SystemTimeZone"/>  class property, assign its value to a variable, 
        /// and clear the cached value for the property by calling the <see cref="ResetSystemTimeZone()"/> method, 
        /// the object stored in the variable doesn't update to reflect the new system time zone. 
        /// Contrast this behavior with that of the <see cref="LocalTimeZone"/> class property, 
        /// which returns a proxy object that always reflects the current system time zone.
        /// </summary>
        public static ISN_NSTimeZone LocalTimeZone {
            get {
                return ISN_NSLib.API.LocalTimeZone;
            }
        }



        /// <summary>
        /// Gets the local time zone.
        /// 
        /// If the current system time zone cannot be determined, the GMT time zone is used instead.
        ///
        /// If you access the  <see cref="SystemTimeZone"/> class property, 
        /// its value is cached by the app and doesn't update if the user subsequently changes the system time zone. 
        /// In order for the systemTimeZone property to reflect the new time zone, 
        /// you must first call the <see cref="ResetSystemTimeZone()"/> method to clear the cached value. 
        /// Then, the next time you access the <see cref="SystemTimeZone"/> property, 
        /// it returns the current system time zone, and caches that value.
        /// 
        /// If you access the  <see cref="SystemTimeZone"/>  class property, assign its value to a variable, 
        /// and clear the cached value for the property by calling the <see cref="ResetSystemTimeZone()"/> method, 
        /// the object stored in the variable doesn't update to reflect the new system time zone. 
        /// Contrast this behavior with that of the <see cref="LocalTimeZone"/> class property, 
        /// which returns a proxy object that always reflects the current system time zone.
        /// </summary>
        public static ISN_NSTimeZone SystemTimeZone {
            get {
                return ISN_NSLib.API.SystemTimeZone;
            }
        }


        /// <summary>
        /// The default time zone for the current app.
        /// 
        /// If no <see cref="DefaultTimeZone"/> time zone has been set, the current system time zone is used. 
        /// If the current system time zone cannot be determined, the GMT time zone is used instead.
        /// 
        /// The <see cref="DefaultTimeZone"/> time zone is used by the app for date and time operations.
        /// You can set it to cause the app to run as if it were in a different time zone. 
        /// Setting the <see cref="DefaultTimeZone"/> property clears any value that was previously set.
        /// 
        /// If you access the <see cref="DefaultTimeZone"/> class property, 
        /// assign its value to a variable, and set a new defaultTimeZone time zone, 
        /// the object stored in the variable doesn't update to reflect the new defaultTimeZone time zone. 
        /// Contrast this behavior with that of the localTimeZone class property, 
        /// which returns a proxy object that always reflects the current system time zone.
        /// </summary>
        public static ISN_NSTimeZone DefaultTimeZone {
            get {
                return ISN_NSLib.API.DefaultTimeZone;
            }
        }

    }
}