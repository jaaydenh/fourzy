using System;
using System.Globalization;

namespace SA.Foundation.Time
{

    public static class SA_Rfc3339_Time
    {

        private static string[] _rfc3339Formats = new string[0];
        private const string Rfc3339Format = "yyyy-MM-dd'T'HH:mm:ssK";
        private const string MinRfc339Value = "0001-01-01T00:00:00Z";


        /// <summary>
        /// Converts DateTime to Rfc3339 formated string
        /// </summary>
        /// <param name="dateTime">Source time for convertation</param>
        public static string ToRfc3339(DateTime dateTime) {
            if (dateTime == DateTime.MinValue) {
                return MinRfc339Value;
            } else {
                return dateTime.ToString(Rfc3339Format, DateTimeFormatInfo.InvariantInfo);
            }
        }


        /// <summary>
        /// Tries to parce Rfc3339 formated string into a <see cref="DateTime"/> object
        /// </summary>
        /// <param name="Rfc3339Time">Rfc3339 Time formated string</param>
        /// <param name="result">Object for recording a parsing result</param>
        public static bool TryParseRfc3339(string Rfc3339Time, out DateTime result) {
            bool wasConverted = false;
            result = DateTime.Now;

            if (!string.IsNullOrEmpty(Rfc3339Time)) {
                DateTime parseResult;
                if (DateTime.TryParseExact(Rfc3339Time, DateTimePatterns, DateTimeFormatInfo.InvariantInfo,
                    DateTimeStyles.AdjustToUniversal, out parseResult)) {
                    result = DateTime.SpecifyKind(parseResult, DateTimeKind.Utc);
                    result = result.ToLocalTime();
                    wasConverted = true;
                }
            }
            return wasConverted;
        }





        private static string[] DateTimePatterns {
            get {
                if (_rfc3339Formats.Length > 0) {
                    return _rfc3339Formats;
                }
                _rfc3339Formats = new string[11];

                // Rfc3339DateTimePatterns
                _rfc3339Formats[0] = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffK";
                _rfc3339Formats[1] = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffffffK";
                _rfc3339Formats[2] = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffK";
                _rfc3339Formats[3] = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffffK";
                _rfc3339Formats[4] = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK";
                _rfc3339Formats[5] = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffK";
                _rfc3339Formats[6] = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fK";
                _rfc3339Formats[7] = "yyyy'-'MM'-'dd'T'HH':'mm':'ssK";

                // Fall back patterns
                _rfc3339Formats[8] = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffK"; // RoundtripDateTimePattern
                _rfc3339Formats[9] = DateTimeFormatInfo.InvariantInfo.UniversalSortableDateTimePattern;
                _rfc3339Formats[10] = DateTimeFormatInfo.InvariantInfo.SortableDateTimePattern;

                return _rfc3339Formats;
            }
        }

    }
}