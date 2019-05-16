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
	public class SA_Result  {

        [SerializeField] protected SA_Error m_error = null;
        [SerializeField] protected string m_requestId = String.Empty;

        //--------------------------------------
        // Initialization
        //--------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="SA_Result"/> class.
        /// </summary>
        public SA_Result() {

		}


        /// <summary>
        /// Initializes a new instance of the <see cref="SA_Result"/> class with predefined error
        /// </summary>
        /// <param name="error">A predefined result error object.</param>
        public SA_Result(SA_Error error) {
            SetError(error);
		}


        //--------------------------------------
        // Public Methods
        //--------------------------------------

        /// <summary>
        /// Sets the error to result.
        /// </summary>
        ///  /// <param name="error">A predefined result error object.</param>
        public void SetError(SA_Error error) {
            m_error = error;
        }


		//--------------------------------------
		// Get / Set
		//--------------------------------------


		/// <summary>
        /// Gets the result error object. If Error message is empty,
        /// result is succeeded.
        /// </summary>
        public SA_Error Error {
			get {
				return m_error;
			}
		}

		/// <summary>
        /// Gets a value indicating whether this <see cref="T:SA.Support.Templates.SA_Result"/> has error.
        /// </summary>
        /// <value><c>true</c> if has error; otherwise, <c>false</c>.</value>
		public bool HasError {
			get {
                if (m_error == null || (string.IsNullOrEmpty(m_error.Message) && Error.Code == default(int))) {
					return false;
				} else {
					return true;
				}
			}
		}

		/// <summary>
        /// Gets a value indicating whether this <see cref="T:SA.Support.Templates.SA_Result"/> is succeeded.
        /// </summary>
        /// <value><c>true</c> if is succeeded; otherwise, <c>false</c>.</value>
		public bool IsSucceeded {
			get {
				return !HasError;
			}
		}

		/// <summary>
        /// Gets a value indicating whether this <see cref="T:SA.Support.Templates.SA_Result"/> is failed.
        /// </summary>
        /// <value><c>true</c> if is failed; otherwise, <c>false</c>.</value>
		public bool IsFailed {
			get {
				return HasError;
			}
		}

        /// <summary>
        /// Returns the id of the player who made the request in order to create correct data references
        /// </summary>
        public string RequestId {
            get { return m_requestId; }
            set { m_requestId = value; }
        }


        public string ToJson() {
            return JsonUtility.ToJson(this);
        }
	}
}
