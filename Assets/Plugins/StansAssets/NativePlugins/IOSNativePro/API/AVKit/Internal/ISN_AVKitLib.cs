using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Utility;

namespace SA.iOS.AVKit.Internal
{
    /// <summary>
    /// This class is for plugin internal use only
    /// </summary>
    internal static class ISN_AVKitLib 
    {
        private static ISN_iAVKitAPI m_api = null;
        public static ISN_iAVKitAPI API {
            get {

                if (!ISN_Settings.Instance.AVKit) {
                    SA_Plugins.OnDisabledAPIUseAttempt(ISN_Settings.PLUGIN_NAME, "AV Kit");
                }

                if (m_api == null) {
                    if (Application.isEditor) {
                        m_api = new ISN_AVKitEditorAPI();
                    } else {
                        m_api = ISN_AVKitNativeAPI.Instance;
                    }
                }

                return m_api;
            }
        }
    }
}
