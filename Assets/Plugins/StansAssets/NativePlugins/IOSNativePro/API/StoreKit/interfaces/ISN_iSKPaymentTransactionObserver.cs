using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Templates;

namespace SA.iOS.StoreKit 
{

    public interface ISN_iSKPaymentTransactionObserver
    {

        /// <summary>
        /// Tells an observer that one or more transactions have been updated.
        /// </summary>
        void OnTransactionUpdated(ISN_SKPaymentTransaction result);


        /// <summary>
        /// Tells an observer that one or more transactions have been removed from the queue.
        /// 
        /// Your application does not typically need to anything on this event,  
        /// but it may be used to update user interface to reflect that a transaction has been completed.
        /// </summary>
        void OnTransactionRemoved(ISN_SKPaymentTransaction result);


        /// <summary>
        /// Tells the observer that a user initiated an in-app purchase from the App Store.
        /// 
        /// Return true to continue the transaction in your app.
        /// Return false to defer or cancel the transaction.
        /// If you return false, you can continue the transaction later using requetsed <see cref="ISN_SKProduct"/>
        /// </summary>
        bool OnShouldAddStorePayment(ISN_SKProduct product);

        /// <summary>
        /// Tells the observer that the payment queue has finished sending restored transactions.
        /// 
        /// This method is called after all restorable transactions have been processed by the payment queue. 
        /// Your application is not required to do anything in this method.
        /// </summary>
        void OnRestoreTransactionsComplete(SA_Result result);
    }
}


