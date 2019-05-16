////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Koretsky Konstantin (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation.Utility;
using SA.iOS.XCode;

namespace SA.iOS.GameKit.Internal
{
    /// <summary>
    /// This class is for plugin internal use only
    /// </summary>
    internal static class ISN_GKLib
    {

        private static ISN_iGKAPI m_api = null;
        public static ISN_iGKAPI API {
            get {

                if (!ISD_API.Capability.GameCenter.Enabled) {
                    SA_Plugins.OnDisabledAPIUseAttempt(ISN_Settings.PLUGIN_NAME, "Game Kit");
                }

                if (m_api == null) {
                    if (Application.isEditor) {
                        m_api = new ISN_GKEditorAPI();
                    }
                    else {
                        m_api = ISN_GKNativeAPI.Instance;
                    }
                }

                return m_api;
            }
        }
    }
}
