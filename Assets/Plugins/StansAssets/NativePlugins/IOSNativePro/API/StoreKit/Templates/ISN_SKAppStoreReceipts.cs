////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using SA.iOS.Utilities;
namespace SA.iOS.StoreKit
{

    /// <summary>
    /// App Store receipt.
    /// </summary>
    public class ISN_SKAppStoreReceipt
    {

        private byte[] m_data = new byte[] {};
        private string m_receiptString = string.Empty;

        public ISN_SKAppStoreReceipt(string data)  {
            if (data.Length > 0) {
                try {
                    m_data = System.Convert.FromBase64String(data);
                    m_receiptString = data;
                } catch(System.Exception ex) {
                    ISN_Logger.LogError("Can't parce the receipt: " + ex.Message);
                }
               
            }
        }


        /// <summary>
        /// The receipt data
        /// </summary>
        public byte[] Data {
            get {
                return m_data;
            }
        }

        /// <summary>
        /// The receipt data represented as Base64String
        /// </summary>
        public string AsBase64String {
            get {
                return m_receiptString;
            }
        }
    }
}
