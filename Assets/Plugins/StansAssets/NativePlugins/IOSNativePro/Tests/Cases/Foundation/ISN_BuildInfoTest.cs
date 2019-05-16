using UnityEngine;
using System.Collections.Generic;
using SA.Foundation.Tests;
using SA.Foundation.Async;

using SA.iOS.Utilities;
using SA.iOS.Foundation;


namespace SA.iOS.Tests.Foundation
{
    public class ISN_BuildInfoTest : SA_BaseTest
    {

        public override void Test() {


            ISN_NSBuildInfo buildInfo = ISN_NSBundle.BuildInfo;
            ISN_Logger.Log("AppVersion: " + buildInfo.AppVersion);
            ISN_Logger.Log("BuildNumber: " + buildInfo.BuildNumber);


            SetResult(SA_TestResult.OK);
        }
    }
}