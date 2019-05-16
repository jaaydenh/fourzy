using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA.iOS.Examples {
    [Serializable]
    public class SA_ExampleServicesConfig {
        public string Name;
        public Sprite Icon;
        public List<SA_ExampleSubsectionConfig> Subsections = new List<SA_ExampleSubsectionConfig>();
    }
}