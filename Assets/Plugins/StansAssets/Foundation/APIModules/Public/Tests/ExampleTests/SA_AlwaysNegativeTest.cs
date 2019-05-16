using UnityEngine;
using System.Collections;
using SA.Foundation.Tests;
namespace SA.Foundation.Tests {
    public class SA_AlwaysNegativeTest : SA_BaseTest {


        public override string Title {
            get {
                return "Always Negative Test Example";
            }
        }

        public override void Test() {
            SetResult(SA_TestResult.FromSAResult(new SA.Foundation.Templates.SA_Result(new SA.Foundation.Templates.SA_Error(100, "error message"))));
        }
    }
}