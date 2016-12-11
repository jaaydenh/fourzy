using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	/// <summary>
	/// Represents an object that holds information about a product registered in Store.
	/// </summary>
	[System.Serializable]
	public class BillingProduct
	{
		#region Fields

		[SerializeField]
		[Tooltip("The name of the product. This information is used for simulating feature behaviour in Editor.")]
		private 	string		m_name;
		[SerializeField]
		[Tooltip("The description of the product. This information is used for simulating feature behaviour in Editor.")]
		private 	string		m_description;
		[SerializeField]
		[Tooltip("The flag determines the product type. If enabled, product is identified as consumable.")]
		private 	bool		m_isConsumable;
		[SerializeField]
		private 	float		m_price;
		[SerializeField]
		[Tooltip("Product's registration id in iOS App Store.")]
		private 	string		m_iosProductId;
		[SerializeField]
		[Tooltip("Product's registration id in Google Play Store.")]
		private 	string 		m_androidProductId;
		[SerializeField]
		[Tooltip("Optional extra data to let store to send back in purchase response. Used for security purposes and supported by Google's Android Platform only currently.")]
		private 	string 		m_developerPayload;

		#endregion

		#region Properties

		/// <summary>
		/// The name of the product. (read-only)
		/// </summary>
		public string Name 
		{ 
			get	
			{ 
				return  m_name; 
			}

			protected set	
			{ 
				m_name	= value; 
			}
		}

		/// <summary>
		/// A description of the product. (read-only)
		/// </summary>
		public string Description 
		{ 
			get	
			{ 
				return  m_description; 
			} 

			protected set	
			{ 
				m_description	= value; 
			} 
		}

		/// <summary>
		/// A bool value used to identify product type. Is it Consumable/Non-Consumable product? (read-only)
		/// </summary>
		public bool IsConsumable
		{ 
			get	
			{ 
				return  m_isConsumable; 
			}

			protected set 
			{ 
				m_isConsumable	= value; 
			} 
		}

		/// <summary>
		/// The cost of the product in the local currency. (read-only)
		/// </summary>
		public float Price 
		{ 
			get	
			{ 
				return  m_price; 
			}
			
			protected set 
			{ 
				m_price	= value; 
			}
		}

		/// <summary>
		/// The cost of the product prefixed with local currency symbol. (read-only)
		/// </summary>
		public string LocalizedPrice 
		{ 
			get; 
			protected set; 
		}

		/// <summary>
		/// The string used as a local currency code. (read-only)
		/// </summary>
		public string CurrencyCode
		{
			get;
			protected set;
		}

		/// <summary>
		/// The string used as a local currency symbol. (read-only)
		/// </summary>
		public string CurrencySymbol
		{
			get;
			protected set;
		}

		protected string IOSProductID
		{
			get
			{
				return m_iosProductId;
			}

			set
			{
				m_iosProductId = value;
			}
		}

		protected string AndroidProductID
		{
			get
			{
				return m_androidProductId;
			}

			set
			{
				m_androidProductId = value;
			}
		}

		/// <summary>
		/// The string that identifies the product to the Store. (read-only)
		/// </summary>
		public string ProductIdentifier
		{
			get 
			{
#if UNITY_ANDROID
				return m_androidProductId;
#else
				return m_iosProductId;
#endif
			}
		}

		/// <summary>
		/// This is used to specify any additional arguments that you want Store to send back along with the purchase information.
		/// Note : This is currently supported by Google's Android Platform only.
		/// </summary>
		public string DeveloperPayload
		{
			get
			{
				return m_developerPayload;
			}
			
			set
			{
				m_developerPayload = value;
			}
		}


		#endregion

		#region Constructors

		protected BillingProduct ()
		{}

		protected BillingProduct (BillingProduct _product)
		{
			this.Name				= _product.Name;
			this.Description		= _product.Description;
			this.IsConsumable		= _product.IsConsumable;
			this.Price				= _product.Price;
			this.LocalizedPrice		= _product.LocalizedPrice;
			this.CurrencyCode		= _product.CurrencyCode;
			this.CurrencySymbol		= _product.CurrencySymbol;
			this.IOSProductID		= _product.IOSProductID;
			this.AndroidProductID	= _product.AndroidProductID;
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// Creates a new billing product with given information.
		/// </summary>
		/// <param name="_name">The name of the product.</param>
		/// <param name="_isConsumable">The type of the billing product. Is it Consumable/Non-Consumable product?</param>
		/// <param name="_platformIDs">An array of platform specific product identifiers.</param>
		/// <example>
		/// The following code example shows how to dynamically create billing product.
		/// <code>
		/// BillingProduct	_newProduct	= BillingProduct.Create("name", true, Platform.Android("android-product-id"), Platform.IOS("ios-product-id"));
		/// </code>
		/// </example>
		public static BillingProduct Create (string _name, bool _isConsumable, params PlatformID[] _platformIDs)
		{
			BillingProduct	_newProduct	= new BillingProduct();
			_newProduct.Name			= _name;
			_newProduct.IsConsumable	= _isConsumable;

			// Set product identifiers
			if (_platformIDs != null)
			{
				foreach (PlatformID _curID in _platformIDs)
				{
					if (_curID == null)
						continue;

					if (_curID.Platform == PlatformID.ePlatform.IOS)
						_newProduct.IOSProductID		= _curID.Value;
					else if (_curID.Platform == PlatformID.ePlatform.ANDROID)
						_newProduct.AndroidProductID	= _curID.Value;
				}
			}

			return _newProduct;
		}

		#endregion

		#region Methods

		public override string ToString ()
		{
			return string.Format ("[BillingProduct: Name={0}, ProductIdentifier={1}, IsConsumable={2}, LocalizedPrice={3}, CurrencyCode={4}, CurrencySymbol={5}]", 
			                      Name, ProductIdentifier, IsConsumable, LocalizedPrice, CurrencyCode, CurrencySymbol);
		}

		#endregion

		#region Deprecated Methods

		[System.Obsolete("Instead use Create methods.")]
		public BillingProduct Copy ()
		{
			return null;
		}

		#endregion
	}
}