

using UnityEngine;
using System.Collections.Generic;
using SA.Foundation.Tests;
using SA.Foundation.Templates;

using SA.iOS.UIKit;
using SA.iOS.UserNotifications;

using SA.iOS.Utilities;

namespace SA.iOS.Tests.UserNotifications
{
    public class ISN_RemoteNotifications_Test : SA_BaseTest
    {

        private ISN_UNNotificationRequest m_request;

        public override void Test() {


            ISN_UIApplication.RegisterForRemoteNotifications();
            ISN_UIApplication.ApplicationDelegate.DidRegisterForRemoteNotifications.AddListener((result) => {
                if (result.IsSucceeded) {
                    var token = result.DeviceTokenUTF8;
                    if(string.IsNullOrEmpty(token)) {
                        SetResult(SA_TestResult.WithError("Token is empty"));
                        return;
                    }
                    ISN_Logger.Log("ANS token string:" + token);
                } else {
                    ISN_Logger.Log("Error: " + result.Error.Message);
                }

                SetAPIResult(result);
            });

        }
    }
}