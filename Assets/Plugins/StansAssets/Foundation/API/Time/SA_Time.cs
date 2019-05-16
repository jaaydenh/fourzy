using System;
using System.Collections.Generic;

namespace SA.Foundation.Time
{

    public static class SA_Time
    {


        private static Dictionary<string, long> s_timers = new Dictionary<string, long>();


        /// <summary>
        /// Start new timer, with given name.
        /// You can check timer second with GetTime metod
        /// </summary>
        /// <param name="name">Timer name</param>
        public static void StartTimer(string name) {
            s_timers[name] = DateTime.Now.Ticks;
        }



        /// <summary> 
        /// Get timer value in seconds by name. 
        /// If timer with spesifayed name doesn't exist 0 value will be returned
        /// </summary>
        /// <param name="name">Timer name</param>
        public static float GetTime(string name) {

            if (s_timers.ContainsKey(name)) {
                long ticks = DateTime.Now.Ticks - s_timers[name];
                return (float)ticks / TimeSpan.TicksPerSecond;
            } else {
                return 0f;
            }
        }


        /// <summary>
        /// Current time UTC timestamp format
        /// </summary>
        public static Int32 CurrentTimeUTC {
            get {
                return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            }
        }
    }
}
