using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Utility;
namespace SA.iOS.Contacts.Internal
{
    /// <summary>
    /// This class is for plugin internal use only
    /// </summary>
    internal static class ISN_CNLib 
    {
        private static ISN_iCNAPI m_api = null;
        public static ISN_iCNAPI API {
            get {

                if (!ISN_Settings.Instance.Contacts) {
                    SA_Plugins.OnDisabledAPIUseAttempt(ISN_Settings.PLUGIN_NAME, "Contacts");
                }

                if (m_api == null) {
                    if (Application.isEditor) {
                        m_api = new ISN_CNEditorAPI();
                    } else {
                        m_api = ISN_CNNativeAPI.Instance;
                    }
                }

                return m_api;
            }
        }
    }
}
