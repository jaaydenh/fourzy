//@vadym udod

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fourzy._Updates.Tools
{
    /// <summary>
    /// Utilieis class
    /// </summary>
    public static class Utils
    {
        private static System.Random rng = new System.Random();

        public static int ElementIndex<T>(this T[] array, T element)
        {
            for (int i = 0; i < array.Length; i++)
                if (array[i].Equals(element))
                    return i;

            return -1;
        }

        /// <summary>
        /// String to enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        //https://stackoverflow.com/questions/16100/how-should-i-convert-a-string-to-an-enum-in-c
        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        /// <summary>
        /// Angle to vector
        /// </summary>
        /// <param name="vec1"></param>
        /// <param name="vec2"></param>
        /// <returns></returns>
        public static float AngleTo(this Vector2 vec1, Vector2 vec2)
        {
            return Mathf.DeltaAngle(Mathf.Atan2(vec1.y, vec1.x) * Mathf.Rad2Deg, Mathf.Atan2(vec2.y, vec2.x) * Mathf.Rad2Deg);
        }

        /// <summary>
        /// Add element to array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T[] AddElement<T>(this T[] values, T value)
        {
            T[] result = new T[values.Length + 1];

            Array.Copy(values, result, values.Length);
            result[values.Length] = value;

            return result;
        }

        /// <summary>
        /// Removes last element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static T[] RemoveElement<T>(this T[] values)
        {
            T[] result = new T[values.Length - 1];

            Array.Copy(values, result, result.Length);

            return result;
        }

        /// <summary>
        /// List shuffle
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static float Vector3ArrayLength(this Vector3[] set)
        {
            float length = 0;

            if (set == null)
                return length;

            for (int i = 1; i < set.Length; i++)
                length += (set[i] - set[i - 1]).magnitude;

            return length;
        }

        public static float Vector2ArrayLength(this Vector2[] set)
        {
            float length = 0;

            if (set == null)
                return length;

            for (int i = 1; i < set.Length; i++)
                length += (set[i] - set[i - 1]).magnitude;

            return length;
        }

        /// <summary>
        /// Get poin on array of Vector3
        /// </summary>
        /// <param name="set">Array</param>
        /// <param name="delta">Delta (0 - 1)</param>
        /// <returns></returns>
        public static Vector3 PointOnVector3Array(this Vector3[] set, float delta)
        {
            if (set == null)
                return Vector3.zero;

            float totalLength = set.Vector3ArrayLength();
            float lengthLeft = totalLength * delta;
            float deltaLeft = Mathf.Clamp01(delta);

            Vector3 result = set[0];

            for (int i = 1; i < set.Length; i++)
            {
                float pieceLength = (set[i] - set[i - 1]).magnitude;

                if (lengthLeft - pieceLength > 0f)
                {
                    lengthLeft -= pieceLength;
                    deltaLeft -= pieceLength / totalLength;
                }
                else
                    return Vector3.Lerp(set[i - 1], set[i], deltaLeft / (pieceLength / totalLength));
            }

            return result;
        }
    }

}