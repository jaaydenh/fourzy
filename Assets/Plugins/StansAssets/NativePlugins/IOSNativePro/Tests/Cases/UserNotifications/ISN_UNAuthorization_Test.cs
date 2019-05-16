using UnityEngine;
using System.Collections.Generic;
using SA.Foundation.Tests;
using SA.Foundation.Templates;

using SA.iOS.Foundation;
using SA.iOS.UserNotifications;

namespace SA.iOS.Tests.UserNotifications
{
    public class ISN_UNAuthorization_Test : SA_BaseTest
    {

        public override void Test() {
            ISN_UNUserNotificationCenter.GetNotificationSettings((setting) => {
               
              //  if (setting.AuthorizationStatus != ISN_UNAuthorizationStatus.Authorized) {
                    int options = ISN_UNAuthorizationOptions.Alert | ISN_UNAuthorizationOptions.Sound;
                    ISN_UNUserNotificationCenter.RequestAuthorization(options, (result) => {
                        SetAPIResult(result);
                    });
                //} 
            });
        }
    }
}