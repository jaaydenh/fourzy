////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native 2018 - New Generation
// @author Stan's Assets team 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;
using System;
using SA.Foundation.Async;
using SA.Foundation.Events;
using SA.Foundation.Templates;

namespace SA.iOS.AVKit.Internal
{

    internal class ISN_AVKitEditorAPI : ISN_iAVKitAPI
    {
        public void ShowPlayerViewController(ISN_AVPlayerViewController controller) {
            Application.OpenURL(controller.Player.Url.Url);
        }
    }
}
