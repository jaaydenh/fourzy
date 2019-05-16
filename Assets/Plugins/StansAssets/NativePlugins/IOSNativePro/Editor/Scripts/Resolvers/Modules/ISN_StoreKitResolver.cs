using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.iOS.XCode;

namespace SA.iOS 
{
    public class ISN_StoreKitResolver : ISN_APIResolver
    {

        protected override ISN_XcodeRequirements GenerateRequirements() {
            var requirements = new ISN_XcodeRequirements();
            requirements.AddFramework(new ISD_Framework(ISD_iOSFramework.StoreKit));
            requirements.Capabilities.Add("In-App Purchase");
            return requirements;
        }

        protected override void RemoveXcodeRequirements() {
            base.RemoveXcodeRequirements();
        }

        protected override void AddXcodeRequirements() {
            base.AddXcodeRequirements();
        }

  
        protected override string LibFolder { get { return "StoreKit/"; } }
        public override bool IsSettingsEnabled {
            get { return ISD_API.Capability.InAppPurchase.Enabled; }
            set {
                ISD_API.Capability.InAppPurchase.Enabled = value;
                ISD_Settings.Save();
            }
        }

        public override string DefineName { get { return "STORE_KIT_API_ENABLED"; } }
    }
}

