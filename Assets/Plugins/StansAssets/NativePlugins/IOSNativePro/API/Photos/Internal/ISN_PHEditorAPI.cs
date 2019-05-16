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

namespace SA.iOS.Photos.Internal
{

    internal class ISN_PHEditorAPI : ISN_iPHAPI
    {
        public ISN_PHAuthorizationStatus GetAuthorizationStatus() {
            return ISN_PHAuthorizationStatus.Authorized;
        }

        public void RequestAuthorization(Action<ISN_PHAuthorizationStatus> callback) {
            SA_Coroutine.WaitForSeconds(2f, () => {
                callback.Invoke(ISN_PHAuthorizationStatus.Authorized);
            });
        }
    }
}
