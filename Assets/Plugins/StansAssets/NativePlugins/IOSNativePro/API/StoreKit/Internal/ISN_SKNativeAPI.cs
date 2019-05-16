////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native 2018 - New Generation
// @author Stan's Assets team 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;
using SA.iOS.Utilities;
using SA.Foundation.Templates;
using SA.Foundation.Events;


#if ((UNITY_IPHONE || UNITY_TVOS) && STORE_KIT_API_ENABLED) 
using System.Runtime.InteropServices;
#endif

namespace SA.iOS.StoreKit.Internal
{
	
    internal class ISN_SKNativeAPI : ISN_Singleton<ISN_SKNativeAPI>, ISN_iSKAPI {

#if ((UNITY_IPHONE || UNITY_TVOS) && STORE_KIT_API_ENABLED)
        [DllImport ("__Internal")] private static extern void _ISN_LoadStore(string data);
        [DllImport ("__Internal")] private static extern void _ISN_AddPayment(string productIdentifier);
        [DllImport ("__Internal")] private static extern void _ISN_FinishTransaction(string transactionIdentifier);
        [DllImport ("__Internal")] private static extern void _ISN_RestoreCompletedTransactions();
        [DllImport ("__Internal")] private static extern void _ISN_StoreRequestReview();
        [DllImport ("__Internal")]  private static extern void _ISN_SetTransactionObserverState(bool enabled);

        [DllImport("__Internal")] private static extern bool _ISN_CanMakePayments();
        [DllImport("__Internal")] private static extern string _ISN_RetrieveAppStoreReceipt();

        [DllImport("__Internal")] static extern void _ISN_SK_RefreshRequest(string data, IntPtr callback);


#endif

        private SA_Event<ISN_SKPaymentTransaction> m_transactionUpdated = new SA_Event<ISN_SKPaymentTransaction>();
        private SA_Event<ISN_SKPaymentTransaction> m_transactionRemoved = new SA_Event<ISN_SKPaymentTransaction>();
        private SA_Event<ISN_SKProduct> m_shouldAddStorePayment = new SA_Event<ISN_SKProduct>();
        private SA_Event<SA_Result> m_restoreTransactionsComplete = new SA_Event<SA_Result>();
        private Action<ISN_SKInitResult> m_LoadStoreCallback;



        public void LoadStore(ISN_SKLib.SA_PluginSettingsWindowStylesitRequest request, Action<ISN_SKInitResult> callback) {
            m_LoadStoreCallback  = callback;
			#if ((UNITY_IPHONE || UNITY_TVOS) && STORE_KIT_API_ENABLED)
            _ISN_LoadStore(JsonUtility.ToJson(request));
			#endif
		}


        void OnStoreKitDidReceiveResponse(string data) {
            var result = JsonUtility.FromJson<ISN_SKInitResult>(data);
            m_LoadStoreCallback.Invoke(result);
        }



        public void RefreshRequest(ISN_SKReceiptDictionary dictionary, Action<SA_Result> callback) {
            #if ((UNITY_IPHONE || UNITY_TVOS) && STORE_KIT_API_ENABLED)
            _ISN_SK_RefreshRequest(JsonUtility.ToJson(dictionary), ISN_MonoPCallback.ActionToIntPtr(callback));
            #endif
        }

        void OnSRefreshRequestResponse(string data) {
            var result = JsonUtility.FromJson<ISN_SKInitResult>(data);
            m_LoadStoreCallback.Invoke(result);
        }



        public void SetTransactionObserverState(bool enabled) {
            #if ((UNITY_IPHONE || UNITY_TVOS) && STORE_KIT_API_ENABLED)
            _ISN_SetTransactionObserverState(enabled);
            #endif
        }

        public void AddPayment(string productIdentifier) {
			#if ((UNITY_IPHONE || UNITY_TVOS) && STORE_KIT_API_ENABLED) 
            _ISN_AddPayment(productIdentifier);
			#endif
		}

        public void FinishTransaction(ISN_SKPaymentTransaction transaction) {
			#if ((UNITY_IPHONE || UNITY_TVOS) && STORE_KIT_API_ENABLED) 
            _ISN_FinishTransaction(transaction.TransactionIdentifier);
			#endif
		}

        public void RestoreCompletedTransactions() {
			#if ((UNITY_IPHONE || UNITY_TVOS) && STORE_KIT_API_ENABLED) 
                _ISN_RestoreCompletedTransactions();
			#endif
		}

		

        public bool CanMakePayments() {
			#if ((UNITY_IPHONE || UNITY_TVOS) && STORE_KIT_API_ENABLED) 
                return _ISN_CanMakePayments();
			#else
				return false;
			#endif
		}

        public ISN_SKAppStoreReceipt RetrieveAppStoreReceipt() {
            #if ((UNITY_IPHONE || UNITY_TVOS) && STORE_KIT_API_ENABLED)
            return new ISN_SKAppStoreReceipt(_ISN_RetrieveAppStoreReceipt()); 
            #else
            return new ISN_SKAppStoreReceipt(string.Empty); 
            #endif
        }

        public void StoreRequestReview() {
            #if ((UNITY_IPHONE || UNITY_TVOS) && STORE_KIT_API_ENABLED)
             _ISN_StoreRequestReview();
            #endif

        }



        public SA_iEvent<ISN_SKPaymentTransaction> TransactionUpdated { 
            get {
                return m_transactionUpdated;
            }
        }

        public SA_iEvent<ISN_SKPaymentTransaction> TransactionRemoved { 
            get {
                return m_transactionRemoved;
            }
        }

        public SA_iEvent<ISN_SKProduct> ShouldAddStorePayment { 
            get {
                return m_shouldAddStorePayment;
            }
        }

        public SA_iEvent<SA_Result> RestoreTransactionsComplete { 
            get {
                return m_restoreTransactionsComplete;
            }
        }



        //--------------------------------------
        //  ISN_TransactionObserver
        //--------------------------------------


        void OnTransactionUpdated(string data) {
            var result = JsonUtility.FromJson<ISN_SKPaymentTransaction>(data);
            m_transactionUpdated.Invoke(result);
        }


        void OnTransactionRemoved(string data) {
            var result = JsonUtility.FromJson<ISN_SKPaymentTransaction>(data);
            m_transactionRemoved.Invoke(result);
        }

        void OnShouldAddStorePayment(string data) {
            var result = JsonUtility.FromJson<ISN_SKProduct>(data);
            m_shouldAddStorePayment.Invoke(result);
        }


        void OnRestoreTransactionsComplete(string data) {
            var result = JsonUtility.FromJson<SA_Result>(data);
            m_restoreTransactionsComplete.Invoke(result);
        }

   
    }
}
