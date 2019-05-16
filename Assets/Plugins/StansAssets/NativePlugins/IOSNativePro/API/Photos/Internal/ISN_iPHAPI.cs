////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native 2018 - New Generation
// @author Stan's Assets team 
// @support support@stansassets.com
// @website https://stansassets.com
//
//////////////////////////////////////////////////////////////////////////////// 
using System;
using SA.Foundation.Events;
using SA.Foundation.Templates;

namespace SA.iOS.Photos.Internal
{
    internal interface ISN_iPHAPI
    {
        ISN_PHAuthorizationStatus GetAuthorizationStatus();
        void RequestAuthorization(Action<ISN_PHAuthorizationStatus> callback);
    }
}
