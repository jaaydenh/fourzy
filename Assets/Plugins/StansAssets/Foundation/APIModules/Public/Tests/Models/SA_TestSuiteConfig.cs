using System.Collections.Generic; 
using UnityEngine;

namespace SA.Foundation.Tests {
    public class SA_TestSuiteConfig : ScriptableObject {

        public bool SkipInteractableTests = false;
        public bool PauseOnError = false;
        public List<SA_TestGroupConfig> TestGroups = new List<SA_TestGroupConfig>();

    }
}