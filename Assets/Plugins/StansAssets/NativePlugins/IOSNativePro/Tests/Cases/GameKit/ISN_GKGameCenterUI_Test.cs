

using UnityEngine;
using System.Collections.Generic;
using SA.Foundation.Tests;
using SA.Foundation.Async;
using SA.Foundation.Templates;

using SA.iOS.GameKit;
using SA.iOS.Utilities;


namespace SA.iOS.Tests.GameKit
{
    public class ISN_GKGameCenterUI_Test : SA_BaseTest
    {

        public override void Test() {


            /*
            ISN_GKGameCenterViewController viewController = new ISN_GKGameCenterViewController();
            viewController.ViewState = ISN_GKGameCenterViewControllerState.Achievements;
            viewController.Show();

            SA_Coroutine.WaitForSeconds(2f, () => {
                ISN_Logger.Log("Are we here??");
            });
*/
            SetResult(SA_TestResult.OK);

        }
    } 
}