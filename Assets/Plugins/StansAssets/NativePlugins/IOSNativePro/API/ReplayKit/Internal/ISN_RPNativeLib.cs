using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Utility;
namespace SA.iOS.ReplayKit.Internal
{
    /// <summary>
    /// This class is for plugin internal use only
    /// </summary>
    internal static class ISN_RPNativeLib 
    {


        private static ISN_iRRAPI m_api = null;
        public static ISN_iRRAPI API {
            get {

                if (!ISN_Settings.Instance.ReplayKit) {
                    SA_Plugins.OnDisabledAPIUseAttempt(ISN_Settings.PLUGIN_NAME, "Replay Kit");
                }


                if (m_api == null) {
                    if (Application.isEditor) {
                        m_api = new ISN_RPEditorAPI();
                    } else {
                        m_api = ISN_RPNativeAPI.Instance;
                    }
                }

                return m_api;
            }
        }
      
    }
}
