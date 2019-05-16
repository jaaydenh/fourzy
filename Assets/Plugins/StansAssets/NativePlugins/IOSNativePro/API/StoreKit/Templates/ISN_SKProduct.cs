////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using System;
using UnityEngine;

using SA.iOS.Foundation;
using SA.iOS.StoreKit.Internal;

namespace SA.iOS.StoreKit
{

    [Serializable]
	public class ISN_SKProduct  {

		//Getting the Product Identifier
		[SerializeField] string m_productIdentifier = string.Empty;

		//Getting Product Attributes
		[SerializeField] string m_localizedDescription = string.Empty;
		[SerializeField] string m_localizedTitle =  "New Product";

		//Getting Pricing Information
		[SerializeField] float m_price = 0.99f;
        [SerializeField] ISN_NSLocale m_priceLocale  = null;
		[SerializeField] ISN_SKProductDiscount m_introductoryPrice  = null;

		//Getting the Subscription Period and Duration
		[SerializeField] ISN_SKProductSubscriptionPeriod m_subscriptionPeriod = null;

		//Additional data
		[SerializeField] string m_localizedPrice = string.Empty;
        [SerializeField] ISN_SKProductEditorData m_editorData = new  ISN_SKProductEditorData();
		
       
        /// <summary>
        /// The string that identifies the product to the Apple App Store.
        /// </summary>
		public string ProductIdentifier {
			get {
				return m_productIdentifier;
			}
			set {
				m_productIdentifier = value;
			}
		}

		/// <summary>
		/// A description of the product.
		/// </summary>
		public string LocalizedDescription {
			get {
				return m_localizedDescription;
			}
			set {
				m_localizedDescription = value;
			}
		}

		/// <summary>
		/// The name of the product.
		/// </summary>
		public string LocalizedTitle {
			get {
				return m_localizedTitle;
			}			
			set {
				m_localizedTitle = value;
			}
		}

		/// <summary>
		/// The cost of the product in the local currency.
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
		/// The locale used to format the price of the product.
		/// </summary>
        public ISN_NSLocale PriceLocale {
			get {
				return m_priceLocale;
			}
		}

		/// <summary>
		/// The object containing introductory price information for the product.
		/// </summary>
		public ISN_SKProductDiscount IntroductoryPrice {
			get {
				return m_introductoryPrice;
			}
		}

		/// <summary>
		/// The period details for products that are subscriptions.
		/// </summary>
		public ISN_SKProductSubscriptionPeriod SubscriptionPeriod {
			get {
				return m_subscriptionPeriod;
			}
		}

		

		/// <summary>
		/// Gets the price in micros.
		/// </summary>
		public long PriceInMicros {
			get {
				return Convert.ToInt64(m_price * 1000000f);
			}
		}

		/// <summary>
		/// The locale used to format the price of the product.
		/// </summary>
		public string LocalizedPrice {
			get {
				if (string.IsNullOrEmpty (m_localizedPrice)) {
					return Price.ToString () + " " + "$";
				} else {
					return m_localizedPrice;
				}
			}
		}

	

        //--------------------------------------
        // ISN_SKProductEditorData
        //--------------------------------------


        /// <summary>
        /// Type of the product 
        /// </summary>
        public ISN_SKProductType Type {
            get { return m_editorData.ProductType; }
            set { m_editorData.ProductType = value; }
        }


        /// <summary>
        /// Gets icon of the product
        /// </summary>
        public Texture2D Icon {
            get { return m_editorData.Texture; }
            set { m_editorData.Texture = value;}
        }


        /// <summary>
        /// Gets and updates Price Tier
        /// </summary>
        public ISN_SKPriceTier PriceTier {
            get {return m_editorData.PriceTier; }
            set {
                if (m_editorData.PriceTier != value) {
                    m_editorData.PriceTier = value;
                    m_price = ISN_SKUtil.GetPriceByTier(m_editorData.PriceTier);
                }
            }
        }


        /// <summary>
        /// Contains data that is only can be set using the Edito Plugin Settings
        /// </summary>
        public ISN_SKProductEditorData EditorData {
            get { return m_editorData; }
            set { m_editorData = value; }
        }
	}
}
