using System;
using System.Collections.Generic;
using UnityEngine;


namespace SA.iOS.StoreKit
{

    [Serializable]
    public class ISN_SKProductEditorData 
    {

        public Texture2D Texture;
        public ISN_SKPriceTier PriceTier = ISN_SKPriceTier.Tier1;
        public ISN_SKProductType ProductType = ISN_SKProductType.Consumable;
    }
}