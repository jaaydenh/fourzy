using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.iOS.StoreKit
{
    /// <summary>
    /// Values representing the payment modes for a product discount
    /// </summary>
    public enum ISN_SKPaymentMode
    {
        SKProductDiscountPaymentModePayAsYouGo = 1, //A constant indicating that the payment mode of a product discount is billed over a single or multiple billing periods.
        SKProductDiscountPaymentModePayUpFront = 2, //A constant indicating that the payment mode of a product discount is paid up front.
        SKProductDiscountPaymentModeFreeTrial = 3   //A constant that indicates that the payment mode is a free trial.
    }
}