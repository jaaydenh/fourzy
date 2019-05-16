using UnityEngine;
using System.Collections.Generic;
using SA.Foundation.Tests;
using SA.Foundation.Async;

using SA.iOS.Utilities;
using SA.iOS.Foundation;


namespace SA.iOS.Tests.Foundation
{
    public class ISN_TimeZoneTest : SA_BaseTest
    {

        public override void Test() {


            var zone = ISN_NSTimeZone.LocalTimeZone;
            ISN_Logger.Log("LocalTimeZone.Name: " + zone.Name);
            ISN_Logger.Log("LocalTimeZone.Description: " + zone.Description);
            ISN_Logger.Log("LocalTimeZone.SecondsFromGMT: " + zone.SecondsFromGMT);


            zone = ISN_NSTimeZone.DefaultTimeZone;
            ISN_Logger.Log("DefaultTimeZone.Name: " + zone.Name);
            ISN_Logger.Log("DefaultTimeZone.Description: " + zone.Description);
            ISN_Logger.Log("DefaultTimeZone.SecondsFromGMT: " + zone.SecondsFromGMT);


            zone = ISN_NSTimeZone.SystemTimeZone;
            ISN_Logger.Log("SystemTimeZone.Name: " + zone.Name);
            ISN_Logger.Log("SystemTimeZone.Description: " + zone.Description);
            ISN_Logger.Log("SystemTimeZone.SecondsFromGMT: " + zone.SecondsFromGMT);


            SetResult(SA_TestResult.OK);
        }
    }
}