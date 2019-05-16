////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Koretsky Konstantin (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Text;
using SA.Foundation.Templates;
using UnityEngine;

namespace SA.iOS.GameKit
{
    [Serializable]
    public class ISN_GKSavedGameLoadResult : SA_Result
    {
        [SerializeField] string m_data = String.Empty;

        public ISN_GKSavedGameLoadResult(string data) {
            m_data = data;
        }

        public ISN_GKSavedGameLoadResult(SA_Error error) : base(error) {

        }

        /// <summary>
        /// Returns string representation of the data
        /// </summary>
        public string StringData {
            get {
                return m_data;
            }
        }

        /// <summary>
        /// Returns bytes array representation of the data.
        /// </summary>
        public byte[] BytesArrayData {
            get {
                return Convert.FromBase64String(m_data);
            }
        }
    }
}