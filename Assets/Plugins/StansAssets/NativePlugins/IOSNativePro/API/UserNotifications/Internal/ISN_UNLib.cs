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
namespace SA.iOS.UserNotifications.Internal
{
    /// <summary>
    /// This class is for plugin internal use only
    /// </summary>
    internal static class ISN_UNLib
    {

        private static ISN_iUNAPI m_api = null;
        public static ISN_iUNAPI API {
            get {

                if (!ISN_Settings.Instance.UserNotifications) {
                    SA_Plugins.OnDisabledAPIUseAttempt(ISN_Settings.PLUGIN_NAME, "User Notifications");
                }


                if (m_api == null) {
                    if (Application.isEditor) {
                        m_api = new ISN_UNEditorAPI();
                    } else {
                        m_api = ISN_UNNativeAPI.Instance;
                    }
                }

                return m_api;
            }
        }
    }
}
