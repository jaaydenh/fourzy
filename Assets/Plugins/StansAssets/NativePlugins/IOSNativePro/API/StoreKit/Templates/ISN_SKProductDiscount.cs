using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using SA.iOS.Foundation;

namespace SA.iOS.StoreKit
{
	[Serializable]
	public class ISN_SKProductDiscount 
	{

		//Getting Price and Payment Mode
		[SerializeField] float m_price = 0.99f;
        [SerializeField] ISN_NSLocale m_priceLocale = null;
		[SerializeField] ISN_SKPaymentMode m_paymentMode = ISN_SKPaymentMode.SKProductDiscountPaymentModeFreeTrial;

		//Getting the Discount Duration
		[SerializeField] int m_numberOfUnits = 0;
		[SerializeField] ISN_SKProductSubscriptionPeriod m_subscriptionPeriod = null;


		[SerializeField] string m_localizedPrice = string.Empty;

		/// <summary>
		/// The discount price of the product in the local currency.
		/// </summary>
		public float Price {
			get {
				return m_price;
			}
			set {
				m_price = value;
			}
		}

		/// <summary>
		/// The locale used to format the discount price of the product.
		/// </summary>
        public ISN_NSLocale PriceLocale {
            get {
                return m_priceLocale;
            }
        }

		/// <summary>
		/// The payment mode for this product discount.
		/// </summary>
		public ISN_SKPaymentMode PaymentMode {
			get {
				return m_paymentMode;
			}
			set {
				m_paymentMode = value;
			}
		}

		/// <summary>
		/// An integer that indicates the number of periods the product discount is available.
		/// </summary>
		public int NumberOfUnits {
			get {
				return m_numberOfUnits;
			}
			set {
				m_numberOfUnits = value;
			}
		}

		/// <summary>
		/// An object that defines the period for the product discount.
		/// </summary>
		public ISN_SKProductSubscriptionPeriod SubscriptionPeriod {
			get {
				return m_subscriptionPeriod;
			}
			set {
				m_subscriptionPeriod = value;
			}
		}


		/// <summary>
		/// The locale used to format the price of the product.
		/// </summary>
		public string LocalizedPrice {
			get {
				return m_localizedPrice;
			}
			set {
				m_localizedPrice = value;
			}
		}
	}
}
