using UnityEngine;
using System.Collections.Generic;
using SA.Foundation.Tests;
using SA.Foundation.Async;

using SA.iOS.Foundation;


namespace SA.iOS.Tests.Foundation
{
    public class ISN_iCloudKeyValueStorageTest : SA_BaseTest
    {

        public override void Test() {

            
            ISN_NSUbiquitousKeyValueStore.Synchronize();


            //Probably need to add more data types to test later.
            //but we only save string on iOS part so this should be enouph
            string testString = "testString";
            ISN_NSUbiquitousKeyValueStore.SetString("test_key_string", testString);
            ISN_NSKeyValueObject kvObject = ISN_NSUbiquitousKeyValueStore.KeyValueStoreObjectForKey("test_key_string");
            if(!kvObject.StringValue.Equals(testString)) {
                SetResult(SA_TestResult.WithError("String value is wrong"));
                return;
            }


            ISN_NSUbiquitousKeyValueStore.Reset();





            SetResult(SA_TestResult.OK);
        }
    }
}