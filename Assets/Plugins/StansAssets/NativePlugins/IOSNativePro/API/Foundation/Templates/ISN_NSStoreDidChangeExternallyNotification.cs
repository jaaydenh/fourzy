////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Koretsky Konstantin (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using UnityEngine;

namespace SA.iOS.Foundation
{

    [Serializable]
    public class ISN_NSStoreDidChangeExternallyNotification
    {

        [SerializeField] private ISN_NSUbiquitousKeyValueStoreChangeReasons m_reason = ISN_NSUbiquitousKeyValueStoreChangeReasons.None;
        [SerializeField] private List<ISN_NSKeyValueObject> m_updatedData = new List<ISN_NSKeyValueObject>();

        /// <summary>
        /// Return possible values associated with the NSUbiquitousKeyValueStoreChangeReasonKey key.
        /// </summary>
        public ISN_NSUbiquitousKeyValueStoreChangeReasons Reason {
            get { return m_reason; }
            set { m_reason = value; }
        }

        /// <summary>
        /// Returns an array of ISN_NSKeyValueObject objects, that changed in the key-value store.
        /// </summary>
        public List<ISN_NSKeyValueObject> UpdatedData {
            get { return m_updatedData; }
            set { m_updatedData = value; }
        }
    }
}