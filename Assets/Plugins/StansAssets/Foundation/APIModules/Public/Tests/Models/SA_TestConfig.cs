using UnityEngine;
using System.Collections;
using System;
using TypeReferences;
using SA.Foundation.Tests;
namespace SA.Foundation.Tests {
    [Serializable]
    public class SA_TestConfig {
        [ClassExtends(typeof(SA_BaseTest))]
        public ClassTypeReference TestReference;
        public bool StopsNextTestsIfFail;
    }
}