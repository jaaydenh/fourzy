using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Utility;

namespace SA.iOS.Photos.Internal
{
    /// <summary>
    /// This class is for plugin internal use only
    /// </summary>
    internal static class ISN_PHitLib
    {
        private static ISN_iPHAPI m_api = null;
        public static ISN_iPHAPI API {
            get {

                if (!ISN_Settings.Instance.Photos) {
                    SA_Plugins.OnDisabledAPIUseAttempt(ISN_Settings.PLUGIN_NAME, "Photos");
                }

                if (m_api == null) {
                    if (Application.isEditor) {
                        m_api = new ISN_PHEditorAPI();
                    } else {
                        m_api = ISN_PHNativeAPI.Instance;
                    }
                }

                return m_api;
            }
        }
    }
}
