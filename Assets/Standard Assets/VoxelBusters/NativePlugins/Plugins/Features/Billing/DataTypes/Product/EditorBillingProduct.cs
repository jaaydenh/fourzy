using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins.Internal
{
	public sealed class EditorBillingProduct : MutableBillingProduct 
	{
		#region Constructors
		
		public EditorBillingProduct (BillingProduct _product) : base (_product)
		{}

		#endregion
	}
}