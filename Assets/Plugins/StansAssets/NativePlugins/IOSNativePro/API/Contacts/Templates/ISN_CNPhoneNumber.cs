////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;

namespace SA.iOS.Contacts {

    [Serializable]
	public class ISN_CNPhoneNumber  {

        [SerializeField] string m_countryCode = string.Empty;
        [SerializeField] string m_digits = string.Empty;


        /// <summary>
        /// Gets the phone number country code.
        /// </summary>
        public string CountryCode {
            get {
                return m_countryCode;
            }
        }

        /// <summary>
        /// Gets the phone number without country code.
        /// </summary>
        public string Digits {
            get {
                return m_digits;
            }
        }


        /// <summary>
        /// Full phone number including Country Code and Digits
        /// </summary>
        public string FullNumber { 
            get {
                return CountryCode + Digits;
            }
        }
    }

}
