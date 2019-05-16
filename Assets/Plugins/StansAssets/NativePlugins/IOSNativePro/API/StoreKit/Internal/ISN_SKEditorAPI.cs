////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native 2018 - New Generation
// @author Stan's Assets team 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;
using System;
using SA.Foundation.Async;
using SA.Foundation.Events;
using SA.Foundation.Templates;

namespace SA.iOS.StoreKit.Internal
{

    internal class ISN_SKEditorAPI : ISN_iSKAPI {
        
        private SA_Event<ISN_SKPaymentTransaction> m_transactionUpdated = new SA_Event<ISN_SKPaymentTransaction>();
        private SA_Event<ISN_SKPaymentTransaction> m_transactionRemoved = new SA_Event<ISN_SKPaymentTransaction>();
        private SA_Event<ISN_SKProduct> m_shouldAddStorePayment = new SA_Event<ISN_SKProduct>();
        private SA_Event<SA_Result> m_restoreTransactionsComplete = new SA_Event<SA_Result>();


        public void LoadStore(ISN_SKLib.SA_PluginSettingsWindowStylesitRequest request, Action<ISN_SKInitResult> callback) {
            SA_Coroutine.WaitForSeconds(DelayTime, () => {
                ISN_SKInitResult res = new ISN_SKInitResult(ISN_Settings.Instance.InAppProducts);
                callback.Invoke(res);
            });
        }

        public void AddPayment(string productIdentifier) {
            SA_Coroutine.WaitForSeconds(DelayTime, () => {
                var produdct = ISN_SKPaymentQueue.GetProductById(productIdentifier);
                var tranasaction = new ISN_SKPaymentTransaction(produdct, ISN_SKPaymentTransactionState.Purchased);

                m_transactionUpdated.Invoke(tranasaction);
            });
        }


        public void FinishTransaction(ISN_SKPaymentTransaction transaction) {
            SA_Coroutine.WaitForSeconds(DelayTime, () => {
                m_transactionRemoved.Invoke(transaction);
            });
        }


        public void RestoreCompletedTransactions() {
            SA_Coroutine.WaitForSeconds(DelayTime, () => {
                foreach(var product in ISN_SKPaymentQueue.Products) {
                    if(product.Type == ISN_SKProductType.NonConsumable) {
                        var tranasaction = new ISN_SKPaymentTransaction(product, ISN_SKPaymentTransactionState.Restored);
                        m_transactionUpdated.Invoke(tranasaction);
                    }
                   
                }
                m_restoreTransactionsComplete.Invoke(new SA_Result());
            });
        }

        public void SetTransactionObserverState(bool enabled) {
            //Just do nothgin
        }

        public ISN_SKAppStoreReceipt RetrieveAppStoreReceipt() {
            return new ISN_SKAppStoreReceipt(string.Empty);
        }

      

        public bool CanMakePayments() {
            return true;
        }

        public void StoreRequestReview() {
            //do nothing
            //probabl need to simulate show popup in an editor
        }

        public void RefreshRequest(ISN_SKReceiptDictionary dictionary, Action<SA_Result> callback) {
            SA_Coroutine.WaitForSeconds(DelayTime, () => {
                callback.Invoke(new SA_Result());
            });
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

        private float DelayTime {
            get {
                return UnityEngine.Random.Range(0.1f, 3f);
            }
        }
    }
}
