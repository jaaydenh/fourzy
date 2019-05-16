using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.iOS.StoreKit
{
	[Serializable]
	public class ISN_SKProductSubscriptionPeriod {

		//Getting Subscription Period Details
		[SerializeField] private int m_numberOfUnits;
		[SerializeField] private ISN_SKProductPeriodUnit m_unit;

		/// <summary>
		/// The number of units per subscription period.
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
		/// The increment of time that a subscription period is specified in.
		/// </summary>
		public ISN_SKProductPeriodUnit Unit {
			get {
				return m_unit;
			}
			set {
				m_unit = value;
			}
		}
	}
}
