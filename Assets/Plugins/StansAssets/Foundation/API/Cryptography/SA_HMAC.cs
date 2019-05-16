////////////////////////////////////////////////////////////////////////////////
//  
// @module Assets Common Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


using System.Text;
using System.Security.Cryptography;

#if NETFX_CORE
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
#endif

namespace SA.Foundation.Cryptography {

	public static class SA_HMAC {



#if NETFX_CORE

        /// <summary>
        /// Generate HMAC SHA256 hex key 
        /// </summary>
        public static string Hash(string key, string data) {
            byte[] secretKey = Encoding.ASCII.GetBytes(key);
            var objMacProv = MacAlgorithmProvider.OpenAlgorithm(MacAlgorithmNames.HmacSha256);
            var hash = objMacProv.CreateHash(secretKey.AsBuffer());
            hash.Append(CryptographicBuffer.ConvertStringToBinary(data, BinaryStringEncoding.Utf8));
            return CryptographicBuffer.EncodeToBase64String(hash.GetValueAndReset());
        }
#else
        /// <summary>
        /// Generate HMAC SHA256 hex key 
        /// </summary>
        public static string Hash(string key, string data) {
            var keyByte = ASCIIEncoding.UTF8.GetBytes(key);
            using (var hmacsha256 = new HMACSHA256(keyByte)) {
                hmacsha256.ComputeHash(ASCIIEncoding.UTF8.GetBytes(data));

                byte[] buff = hmacsha256.Hash;
                string sbinary = "";

                for (int i = 0; i < buff.Length; i++)
                    sbinary += buff[i].ToString("X2"); /* hex format */
                return sbinary.ToLower();
            }
        }
#endif

	}

}
