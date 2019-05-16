using UnityEngine;
using System.Collections.Generic;
using SA.Foundation.Tests;
using SA.Foundation.Async;

using SA.iOS.Foundation;


namespace SA.iOS.Tests.Foundation
{
    public class ISN_NSFileManagerTest : SA_BaseTest
    {

        public override void Test() {


           string token =  ISN_NSFileManager.UbiquityIdentityToken;
           Debug.Log(token);

            SetResult(SA_TestResult.OK);
        }
    }
}