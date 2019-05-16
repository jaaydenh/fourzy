using System;
using UnityEngine;
using SA.Foundation.Templates;

namespace SA.iOS.GameKit
{

    /// <summary>
    /// Game Kit signature generation result
    /// </summary>
    [Serializable]
    public class ISN_GKIdentityVerificationSignatureResult : SA_Result
    {
#pragma warning disable 649
        [SerializeField] string m_publicKeyUrl;
        [SerializeField] string m_signature;
        [SerializeField] string m_salt;
        [SerializeField] long m_timestamp;
#pragma warning restore 649

        public ISN_GKIdentityVerificationSignatureResult(SA_Error error) : base(error) {

        }

        /// <summary>
        /// The URL for the public encryption key.
        /// </summary>
        public string PublicKeyUrl {
            get {
                return m_publicKeyUrl;
            }
        }


        /// <summary>
        /// The date and time that the signature was created.
        /// </summary>
        public long Timestamp {
            get {
                return m_timestamp;
            }
        }


        /// <summary>
        /// The verification signature data generated.
        /// </summary>
        public byte[] Signature {
            get {
                return m_signature.BytesFromBase64String();
            }
        }

        /// <summary>
        /// The verification signature data generated.
        /// </summary>
        public string SignatureAsBse64String {
            get {
                return m_signature;
            }
        }


        /// <summary>
        /// A random NSString used to compute the hash and keep it randomized.
        /// </summary>
        public byte[] Salt {
            get {
                return m_salt.BytesFromBase64String();
            }
        }

        /// <summary>
        /// A random NSString used to compute the hash and keep it randomized.
        /// </summary>
        public string SaltAsBse64String {
            get {
                return m_salt;
            }
        }
    }
}