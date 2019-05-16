using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Utility;
using SA.iOS.XCode;

namespace SA.iOS.StoreKit.Internal
{
    /// <summary>
    /// This class is for plugin internal use only
    /// </summary>
    internal static class ISN_SKLib 
    {
        
        private static ISN_iSKAPI m_api = null;
        public static ISN_iSKAPI API {
            get {

                if (!ISD_API.Capability.InAppPurchase.Enabled) {
                    SA_Plugins.OnDisabledAPIUseAttempt(ISN_Settings.PLUGIN_NAME, "Store Kit");
                }


                if (m_api == null) {
                    if (Application.isEditor) {
                        m_api = new ISN_SKEditorAPI();
                    } else {
                        m_api = ISN_SKNativeAPI.Instance;
                    }
                }

                return m_api;
            }
        }



        [Serializable]
        public class SA_PluginSettingsWindowStylesitRequest {
            public List<string> ProductIdentifiers = new List<string>();
        }

      
    }
}
