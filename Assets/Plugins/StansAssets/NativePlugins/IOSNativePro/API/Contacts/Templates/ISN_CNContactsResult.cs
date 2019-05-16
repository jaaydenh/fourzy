////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using SA.Foundation.Templates;

using UnityEngine;

namespace SA.iOS.Contacts {

    [Serializable]
	public class ISN_CNContactsResult : SA_Result {

        [SerializeField] List<ISN_CNContact> m_contacts = new List<ISN_CNContact>();


        public ISN_CNContactsResult(List<ISN_CNContact> contacts) {
            m_contacts = contacts;
        }

        /// <summary>
        /// Gets the array of loaded contacts. 
        /// </summary>
		public List<ISN_CNContact> Contacts {
			get {
				return m_contacts;
			}
		}
	}

}