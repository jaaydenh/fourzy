using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins.Internal
{
	public class MutableBillingProduct : BillingProduct 
	{
		#region Constructors

		protected MutableBillingProduct ()
		{}

		protected MutableBillingProduct (BillingProduct _product) : base (_product)
		{}

		#endregion

		#region Methods
		
		public void SetIsConsumable (bool _isConsumable)
		{
			this.IsConsumable	= _isConsumable;
		}
		
		public void SetLocalizePrice (string _locPrice)
		{
			this.LocalizedPrice	= _locPrice;
		}
		
		public void SetCurrencyCode (string _code)
		{
			this.CurrencyCode	= _code;
		}
		
		public void SetCurrencySymbol (string _symbol)
		{
			this.CurrencySymbol	= _symbol;
		}
		
		#endregion
	}
}
