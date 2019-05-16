using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.iOS.StoreKit
{
    /// <summary>
    /// Values representing the state of a transaction.
    /// </summary>
    public enum ISN_SKPaymentTransactionState
    {
        Purchasing = 0,    // Transaction is being added to the server queue.
        Purchased = 1,     // Transaction is in queue, user has been charged.  Client should complete the transaction.
        Failed = 2,        // Transaction was cancelled or failed before being added to the server queue.
        Restored = 3,      // Transaction was restored from user's purchase history.  Client should complete the transaction.
        Deferred = 4
    }
}