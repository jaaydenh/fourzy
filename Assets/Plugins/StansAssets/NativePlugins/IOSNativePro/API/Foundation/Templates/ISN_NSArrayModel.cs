using System;
using System.Collections.Generic;
using UnityEngine;

namespace SA.iOS.Foundation
{
    /// <summary>
    /// Array representation model. 
    /// Use to send data to the native part using JSONUtility.
    /// </summary>
    [Serializable]
    public class ISN_NSArrayModel 
    {
        [SerializeField] List<string> m_value = new List<string>();

        /// <summary>
        /// Add's list item to the model.
        /// </summary>
        /// <param name="item"></param>
        public void Add(string item) {
            m_value.Add(item);
        }

        /// <summary>
        /// List values
        /// </summary>
        public List<string> Value {
            get {
                return m_value;
            }
        }
    }
}