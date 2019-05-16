using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Templates;
using SA.Foundation.Time;
using SA.Foundation.Utility;


namespace SA.iOS.StoreKit 
{

    [Serializable]
    public class ISN_SKPaymentTransaction : SA_Result
    {

       
        [SerializeField] string m_productIdentifier = null;
        [SerializeField] string m_transactionIdentifier = null;
        [SerializeField] private long m_unitxDate = 0;
        [SerializeField] ISN_SKPaymentTransactionState m_state = default(ISN_SKPaymentTransactionState);
        [SerializeField] ISN_SKPaymentTransaction m_originalTransaction = null;


        public ISN_SKPaymentTransaction(ISN_SKProduct product, ISN_SKPaymentTransactionState state) {
            m_productIdentifier = product.ProductIdentifier;
            m_transactionIdentifier = SA_IdFactory.RandomString;
            m_unitxDate = SA_Unix_Time.ToUnixTime(DateTime.UtcNow);

            m_state = state;
        }


        /// <summary>
        /// A string used to identify a product that can be purchased from within your application.
        /// </summary>
        public string ProductIdentifier {
            get {
                return m_productIdentifier;
            }
        }


        /// <summary>
        /// A string that uniquely identifies a successful payment transaction.
        /// 
        /// The contents of this property are undefined except when <see cref="State"/> 
        /// is set to <see cref="ISN_SKPaymentTransactionState.Purchased"/> or <see cref="ISN_SKPaymentTransactionState.Restored"/>. 
        /// The transactionIdentifier is a string that uniquely identifies the processed payment. 
        /// Your application may wish to record this string as part of an audit trail for App Store purchases. 
        /// See <see href="https://developer.apple.com/library/content/documentation/NetworkingInternet/Conceptual/StoreKitGuide/Introduction.html#//apple_ref/doc/uid/TP40008267"> In-App Purchase Programming Guide </see> for more information. 
        /// 
        ///The value of this property corresponds to the Transaction Identifier property in the <see href="https://developer.apple.com/library/content/releasenotes/General/ValidateAppStoreReceipt/Chapters/ReceiptFields.html#//apple_ref/doc/uid/TP40010573-CH106-SW13">receipt </see>.
        /// </summary>
        public string TransactionIdentifier {
            get {
                return m_transactionIdentifier;
            }
        }

        /// <summary>
        /// The date the transaction was added to the App Store’s payment queue.
        /// 
        /// The contents of this property are undefined except when <see cref="State"/> 
        /// is set to <see cref="ISN_SKPaymentTransactionState.Purchased"/> or <see cref="ISN_SKPaymentTransactionState.Restored"/>.
        /// </summary>
        public DateTime Date {
            get {
                return SA_Unix_Time.ToDateTime(m_unitxDate);
            }
        }


        /// <summary>
        /// An object describing the error that occurred while processing the transaction.
        /// </summary>
        public ISN_SKPaymentTransactionState State {
            get {
                return m_state;
            }
        }

        /// <summary>
        /// Gets the associated product with this transaction.
        /// </summary>
        public ISN_SKProduct Product {
            get {
                return ISN_SKPaymentQueue.GetProductById(m_productIdentifier);
            }
        }

        /// <summary>
        /// The transaction that was restored by the App Store.
        /// 
        /// The contents of this property are undefined except when <see cref="State"/> 
        /// is set to <see cref="ISN_SKPaymentTransactionState.Restored"/>. 
        /// When a transaction is restored, the current transaction holds a new transaction identifier, receipt, and so on. 
        /// Your application will read this property to retrieve the restored transaction.
        /// </summary>
        /// <value>The original transaction.</value>
        public ISN_SKPaymentTransaction OriginalTransaction {
            get {
                return m_originalTransaction;
            }

        }
    }
}