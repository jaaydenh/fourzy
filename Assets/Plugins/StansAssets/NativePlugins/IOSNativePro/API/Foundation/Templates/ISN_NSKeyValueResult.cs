////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Koretsky Konstantin (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using SA.Foundation.Templates;
using UnityEngine;

namespace SA.iOS.Foundation
{

    public class ISN_NSKeyValueResult : SA_Result
    {
        [SerializeField] private ISN_NSKeyValueObject m_keyValueObject;

        public ISN_NSKeyValueResult(ISN_NSKeyValueObject keyValueObject):base() {
            m_keyValueObject = keyValueObject;
        }

        public ISN_NSKeyValueResult(SA_Error error) : base(error) {}

        /// <summary>
        /// Returns the object associated with the specified key.
        /// </summary>
        public ISN_NSKeyValueObject KeyValueObject {
            get {
                return m_keyValueObject;
            }
        }
    }
}
