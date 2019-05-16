////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native 2018 - New Generation
// @author Stan's Assets team 
// @support support@stansassets.com
// @website https://stansassets.com
//
//////////////////////////////////////////////////////////////////////////////// 
using System;
using SA.Foundation.Events;
using SA.Foundation.Templates;

namespace SA.iOS.StoreKit.Internal
{
	
    internal interface ISN_iSKAPI {
		
        void LoadStore (ISN_SKLib.SA_PluginSettingsWindowStylesitRequest request , Action<ISN_SKInitResult> callback);
        void AddPayment(string productIdentifier);
        void FinishTransaction (ISN_SKPaymentTransaction transaction);
        void RestoreCompletedTransactions ();
        void SetTransactionObserverState(bool enabled);

		
        bool CanMakePayments ();
        ISN_SKAppStoreReceipt RetrieveAppStoreReceipt();
        void RefreshRequest(ISN_SKReceiptDictionary dictionary, Action<SA_Result> callback);
        void StoreRequestReview();

        SA_iEvent<ISN_SKPaymentTransaction> TransactionUpdated { get; }
        SA_iEvent<ISN_SKPaymentTransaction> TransactionRemoved { get; }
        SA_iEvent<ISN_SKProduct> ShouldAddStorePayment { get; }
        SA_iEvent<SA_Result> RestoreTransactionsComplete { get; }






	}
}
