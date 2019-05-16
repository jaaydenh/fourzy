////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native 2018 - New Generation
// @author Stan's Assets team 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections.Generic;

using SA.iOS.StoreKit.Internal;
        
namespace SA.iOS.StoreKit
{
    /// <summary>
    /// A queue of payment transactions to be processed by the App Store.
    /// 
    /// The payment queue communicates with the App Store 
    /// and presents a user interface so that the user can authorize payment. 
    /// The contents of the queue are persistent between launches of your app.
    /// </summary>
    public static class ISN_SKPaymentQueue 
    {	
        
		//Actions
        private static event Action<ISN_SKInitResult> m_onStoreKitInitComplete = delegate{};
        private static List<ISN_iSKPaymentTransactionObserver> m_observers = new List<ISN_iSKPaymentTransactionObserver>();

        private static bool m_isInitializationInProgress = false;
        private static ISN_SKInitResult m_successInitResultCache = null;

        private static Dictionary<string, ISN_SKProduct> m_products = new Dictionary<string, ISN_SKProduct>();


        //--------------------------------------
        // Initialization
        //--------------------------------------

        static ISN_SKPaymentQueue() {
            SubscribeToNativeEvents();
        }

        //--------------------------------------
        //  Public Methods
        //--------------------------------------


        /// <summary>
        /// Initializes the Store Kit with the set of previously defined product
        /// Products can be defined under the editor plugin settings: Stan's Assets->IOS Native->Edit Settings
        /// Or you can add product's via code using <see cref="RegisterProduct"/>
        /// </summary>
        /// <param name="callback">Callback with the initialization result</param>
        public static void Init(Action<ISN_SKInitResult> callback) {


            if (m_successInitResultCache != null) {
                callback.Invoke(m_successInitResultCache);
                return;
            }

            m_onStoreKitInitComplete += callback;
            if (m_isInitializationInProgress) { return; }


            m_isInitializationInProgress = true;

            var request = new ISN_SKLib.SA_PluginSettingsWindowStylesitRequest();
            foreach (var product in ISN_Settings.Instance.InAppProducts) {
                request.ProductIdentifiers.Add(product.ProductIdentifier);
            }


            ISN_SKLib.API.LoadStore(request, (ISN_SKInitResult result) => {
                m_isInitializationInProgress = false;
                if(result.IsSucceeded) {
                    CacheAppStoreProducts(result);
					m_successInitResultCache = result;
                }
                m_onStoreKitInitComplete.Invoke(result);
                m_onStoreKitInitComplete = delegate {};

            });

		}


        /// <summary>
        /// Adds an observer to the payment queue.
        /// 
        /// Your application should add an observer to the payment queue during application initialization. 
        /// If there are no observers attached to the queue, the payment queue does not synchronize its list 
        /// of pending transactions with the Apple App Store, 
        /// because there is no observer to respond to updated transactions.
        /// 
        /// If an application quits when transactions are still being processed, 
        /// those transactions are not lost. The next time the application launches, 
        /// the payment queue will resume processing the transactions. 
        /// Your application should always expect to be notified of completed transactions.
        /// 
        /// If more than one transaction observer is attached to the payment queue, 
        /// no guarantees are made as to the order they will be called in. 
        /// It is safe for multiple observers to call <see cref="FinishTransaction"/>, but not recommended. 
        /// It is recommended that you use a single observer to process and finish the transaction.
        /// </summary>
        /// <param name="observer">The observer to add to the queue.</param>
        public static void AddTransactionObserver(ISN_iSKPaymentTransactionObserver observer) {
            m_observers.Add(observer);
            if(m_observers.Count == 1) {
                //we have atleas one observer atm, so let's enable observation on a native side
                ISN_SKLib.API.SetTransactionObserverState(true);
            }
        }

        /// <summary>
        /// Removes an observer from the payment queue.
        /// 
        /// If there are no observers attached to the queue, 
        /// the payment queue does not synchronize its list of pending transactions with the Apple App Store, 
        /// because there is no observer to respond to updated transactions.
        /// </summary>
        /// <param name="observer">The observer to remove.</param>
        public static void RemoveTransactionObserver(ISN_iSKPaymentTransactionObserver observer) {
            m_observers.Remove(observer);
            if (m_observers.Count == 0) {
                //we have no observer's atm, have to disable observation on a native side
                ISN_SKLib.API.SetTransactionObserverState(false);
            }
        }
            

			
        /// <summary>
        /// Adds a payment request to the queue.
        /// 
        /// An application should always have at least one observer of the payment queue before adding payment requests.
        /// The payment request must have a product identifier registered with the Apple App Store.
        /// 
        /// When a payment request is added to the queue, 
        /// the payment queue processes that request with the Apple App Store 
        /// and arranges for payment from the user. When that transaction is complete or if a failure occurs, 
        /// the payment queue sends the <see cref="ISN_SKPaymentTransaction"/> object that encapsulates the request 
        /// to all transaction observers.
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        public static void AddPayment(string productId) {    
            Init((result) => {
                ISN_SKLib.API.AddPayment(productId);
            });

		}

        /// <summary>
        /// Completes a pending transaction.
        /// 
        /// Your application should call this method from a transaction observer 
        /// that received a notification from the payment queue. 
        /// Calling <see cref="FinishTransaction"/> on a transaction removes it from the queue. 
        /// Your application should call <see cref="FinishTransaction"/> only after 
        /// it has successfully processed the transaction and unlocked the functionality purchased by the user.
        ///
        /// Calling <see cref="FinishTransaction"/> on a transaction that is in the Purchasing state throws an exception.
        /// </summary>
        /// <param name="transaction">transaction to finish</param>
        public static void FinishTransaction(ISN_SKPaymentTransaction transaction) {
            Init((result) => {
                ISN_SKLib.API.FinishTransaction(transaction);
            });
		}


        /// <summary>
        /// Asks the payment queue to restore previously completed purchases.
        /// 
        /// our application calls this method to restore transactions that were previously finished 
        /// so that you can process them again. 
        /// For example, your application would use this to allow a user to unlock previously purchased content 
        /// onto a new device.
        /// </summary>
        public static void RestoreCompletedTransactions() {
            Init((result) => {
                ISN_SKLib.API.RestoreCompletedTransactions();
            });
        }
			
		
        /// <summary>
        /// Gets the product by identifier.
        /// </summary>
        /// <param name="productIdentifier">Prodcut identifier.</param>
        public static ISN_SKProduct GetProductById(string productIdentifier) {
            return m_products[productIdentifier];
		}


        /// <summary>
        /// Simplified product registration by the product identifier.
        /// You can also define products using editor plugin settings: Stan's Assets->IOS Native->Edit Settings
        /// </summary>
        /// <param name="productId">Product identifier.</param>
        public static void RegisterProductId(string productId) {

            ISN_SKProduct tpl = new ISN_SKProduct();
            tpl.ProductIdentifier = productId;
            RegisterProduct(tpl);
        }


        /// <summary>
        /// Registers the product.
        /// You can also define products using editor plugin settings: Stan's Assets->IOS Native->Edit Settings
        /// </summary>
        /// <param name="product">Product.</param>
        public static void RegisterProduct(ISN_SKProduct product) {

            bool IsProductAlreadyInList = false;
            int replaceIndex = 0;
            foreach (ISN_SKProduct p in ISN_Settings.Instance.InAppProducts) {
                if (p.ProductIdentifier.Equals(product.ProductIdentifier)) {
                    IsProductAlreadyInList = true;
                    replaceIndex = ISN_Settings.Instance.InAppProducts.IndexOf(p);
                    break;
                }
            }

            if (IsProductAlreadyInList) {
                ISN_Settings.Instance.InAppProducts[replaceIndex] = product;
            } else {
                ISN_Settings.Instance.InAppProducts.Add(product);
            }
        }


		//--------------------------------------
		//  Get / Set
		//--------------------------------------


		
        /// <summary>
        /// Gets a value indicating whether this <see cref="ISN_SKPaymentQueue"/> is ready.
        /// The ISN_SKPaymentQueue is ready once Init is completed successfully
        /// </summary>
        /// <value><c>true</c> if is ready; otherwise, <c>false</c>.</value>
        public static bool IsReady {
			get {
                return m_successInitResultCache != null;
			}
		}


        /// <summary>
        /// For an application purchased from the App Store, use this property a to get the receipt. 
        /// This property makes no guarantee about whether there is a file at the URL—only 
        /// that if a receipt is present, that is its location.
        /// </summary>
        /// <returns>The app store receipt.</returns>
        public static ISN_SKAppStoreReceipt AppStoreReceipt {
            get {
                return ISN_SKLib.API.RetrieveAppStoreReceipt();
            }
        }


        /// <summary>
        /// A list of products, one product for each valid product identifier provided in the original init request.
        /// only valid to use when <see cref="IsReady"/> is <c>true</c>
        /// </summary>
        public static List<ISN_SKProduct> Products {
            get {
                return new List<ISN_SKProduct>(m_products.Values);
            }
        }


        /// <summary>
        /// Indicates whether the user is allowed to make payments.
        /// 
        /// An iPhone can be restricted from accessing the Apple App Store. 
        /// For example, parents can restrict their children’s ability to purchase additional content. 
        /// Your application should confirm that the user is allowed to authorize payments 
        /// before adding a payment to the queue. 
        /// Your application may also want to alter its behavior or appearance 
        /// when the user is not allowed to authorize payments.
        /// </summary>
        /// <value><c>true</c> if can make payments; otherwise, <c>false</c>.</value>
        public static bool CanMakePayments {
			get {
                return ISN_SKLib.API.CanMakePayments();
			}
		}



		//--------------------------------------
		//  Private Methods
		//--------------------------------------


        private static void SubscribeToNativeEvents() {
            ISN_SKLib.API.TransactionUpdated.AddListener((result) => {
                foreach(var observer in m_observers) {
                    observer.OnTransactionUpdated(result);
                }
            });


            ISN_SKLib.API.TransactionRemoved.AddListener((result) => {
                foreach (var observer in m_observers) {
                    observer.OnTransactionRemoved(result);
                }
            });

            ISN_SKLib.API.RestoreTransactionsComplete.AddListener((result) => {
                foreach (var observer in m_observers) {
                    observer.OnRestoreTransactionsComplete(result);
                }
            });

            ISN_SKLib.API.ShouldAddStorePayment.AddListener((result) => {
                bool startTransaction = false;
                foreach (var observer in m_observers) {
                    startTransaction = observer.OnShouldAddStorePayment(result);
                }

                if(startTransaction) {
                    AddPayment(result.ProductIdentifier);
                }
            });
			
		}


        private static void CacheAppStoreProducts(ISN_SKInitResult result) {

            m_products.Clear();
            foreach(ISN_SKProduct product in result.Products) {
                ISN_SKProduct settingsProduct = GetProductFromSettings(product.ProductIdentifier);
                if(settingsProduct != null) {
                    product.EditorData = settingsProduct.EditorData;
                }

                m_products.Add(product.ProductIdentifier, product);
            }

        }


        private static ISN_SKProduct GetProductFromSettings(string productIdentifier) {
            foreach(ISN_SKProduct product in ISN_Settings.Instance.InAppProducts) {
                if(product.ProductIdentifier.Equals(productIdentifier)) {
                    return product;
                }
            }
            return null;
        }


	}
}



