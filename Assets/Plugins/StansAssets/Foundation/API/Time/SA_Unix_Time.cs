using System;

namespace SA.Foundation.Time
{
    public static class SA_Unix_Time
    {

        /// <summary>
        /// Converts a UNIX time stamp into <see cref="DateTime"/> object
        /// </summary>
        /// <param name="timestamp">UNIX timestamp</param>
        public static DateTime ToDateTime(long timestamp) {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }


        /// <summary>
        /// Get's a UNIX timestamp from a <see cref="DateTime"/> object
        /// </summary>
        /// <param name="date">Source date for convertation</param>
        public static long ToUnixTime(DateTime date) {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return (long)diff.TotalSeconds;
        }

    }
}