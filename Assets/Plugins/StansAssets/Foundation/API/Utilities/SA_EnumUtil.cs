using System;
using UnityEngine;

namespace SA.Foundation.Utility
{

    public static class SA_EnumUtil
    {

        #pragma warning disable 168
        
        /// <summary>
        /// Check's if a given string can be parsed to a specified enum type
        /// </summary>
        /// <param name="value">String enum value</param>
        public static bool CanBeParsedToEnum<T>(string value) {
            try {
                Enum.Parse(typeof(T), value, true);
                return true;
            } catch (Exception ex) {
                return false;
            }
        }


        /// <summary>
        /// Tries to parse string value to a specified enum type
        /// </summary>
        /// <param name="value">String enum value</param>
        /// <param name="result">Enum result</param>
        public static bool TryParseEnum<T>(string value, out T result) {
            try {
                result = (T)Enum.Parse(typeof(T), value, true);
                return true;
            } catch (Exception ex) {
                result = default(T);
                return false;
            }
        }

        #pragma warning restore 168


        /// <summary>
        /// Tries to parse string value to a specified enum type.
        /// Will print a warning in case of falture, and returen default value for a given Enum type
        /// </summary>
        /// <param name="value">String enum value</param>
        public static T ParseEnum<T>(string value) {
            try {
                T val = (T)Enum.Parse(typeof(T), value, true);
                return val;
            } catch (Exception ex) {
                Debug.LogWarning("Enum Parsing failed: " + ex.Message);
                return default(T);
            }
        }


    }
}