using System;
using System.Collections.Generic;
using UnityEngine;


namespace SA.iOS.UIKit.Internal
{
    /// <summary>
    /// This class is for plugin internal use only
    /// </summary>
    internal static class ISN_UILib 
    {
        
        private static ISN_iUIAPI m_api = null;
        public static ISN_iUIAPI API {
            get {
                if (m_api == null) {
                    m_api = ISN_UINativeAPI.Instance;
                        
                                           /*
                    if (Application.isEditor) {
                        m_api = new ISN_SKEditorAPI();
                    } else {
                        m_api = ISN_SKNativeAPI.Instance;
                    }*/
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
