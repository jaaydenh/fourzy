using System;
using System.Collections.Generic;
using UnityEngine;


namespace SA.iOS.StoreKit
{
    /// <summary>
    /// <see cref="ISN_SKReceiptRefreshRequest"/> properties dictionary
    /// </summary>
    [Serializable]
    public class ISN_SKReceiptDictionary
    {
        [SerializeField] List<ISN_SKReceiptProperty> m_keys = new List<ISN_SKReceiptProperty>();
        [SerializeField] List<int> m_values = new List<int>();

        /// <summary>
        /// Add the specified key and value.
        /// </summary>
        /// <returns>The add.</returns>
        /// <param name="key">Property Key.</param>
        /// <param name="value">Property Value.</param>
        public void Add(ISN_SKReceiptProperty key, int value) {
            m_keys.Add(key);
            m_values.Add(value);
        }
    }
}