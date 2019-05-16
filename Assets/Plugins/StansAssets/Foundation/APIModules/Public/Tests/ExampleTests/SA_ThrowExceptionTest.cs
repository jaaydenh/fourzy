using UnityEngine;
using System.Collections;

using SA.Foundation.Tests;
namespace SA.Foundation.Tests {
    public class SA_ThrowExceptionTest : SA_BaseTest {

        public override string Title {
            get {
                return "Throw Exception";
            }
        }

        public override void Test() {
            throw new System.NotImplementedException();
        }
    }
}
