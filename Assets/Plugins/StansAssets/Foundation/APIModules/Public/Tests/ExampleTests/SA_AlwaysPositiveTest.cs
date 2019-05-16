using UnityEngine;
using System.Collections;
using SA.Foundation.Tests;
namespace SA.Foundation.Tests {
    public class SA_AlwaysPositiveTest : SA_BaseTest {
        public override string Title {
            get {
                return "Always Positive Test Example";
            }
        }


        public override void Test() {
            SetResult(SA_TestResult.OK);
        }
    }
}
