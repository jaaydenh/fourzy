////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin
// @author Koretsky Konstantin (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

namespace SA.iOS.Foundation.Internal
{

    internal static class ISN_NSLib
    {
        private static ISN_NSAPI m_api = null;

        public static ISN_NSAPI API {
            get {
                if (m_api == null) {
                    if (Application.isEditor) {
                        m_api = new ISN_NSEditorAPI();
                    }
                    else {
                        m_api = ISN_NSNativeAPI.Instance;
                    }
                }

                return m_api;
            }
        }
    }
}