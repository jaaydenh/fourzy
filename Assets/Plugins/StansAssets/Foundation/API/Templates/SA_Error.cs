////////////////////////////////////////////////////////////////////////////////
//  
// @module Assets Common Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;

namespace SA.Foundation.Templates {

    [Serializable]
	public class SA_Error  {

        [SerializeField] int m_code;
        [SerializeField] string m_message = string.Empty;



        //--------------------------------------
        // Initialization
        //--------------------------------------



        /// <summary>
        /// Initializes a new instance of the <see cref="SA_Error"/> class,
        /// with predefined <see cref="Code"/> and <see cref="Message"/> s
        /// </summary>
        /// <param name="code">instance error <see cref="Code"/>.</param>
        /// <param name="message">instance error <see cref="Message"/>.</param>
        public SA_Error(int code, string message = "") {
			m_code = code;
			m_message = message;
		}


		//--------------------------------------
		// Get / Set
		//--------------------------------------


		/// <summary>
		/// Error Code
		/// </summary>
		public int Code {
			get {
				return m_code;
			}
		}


		/// <summary>
		/// Error Describtion Message
		/// </summary>
		public string Message {
			get {
				return m_message;
			}
		}


        /// <summary>
        /// Fromated message that combines <see cref="Code"/> and <see cref="Message"/>
        /// </summary>
        public string FullMessage {
            get {
                if(Message.StartsWith(Code.ToString())) {
                    return Message;
                } else {
                    return Code + "::" + Message;
                }
            }
        }
	}


}