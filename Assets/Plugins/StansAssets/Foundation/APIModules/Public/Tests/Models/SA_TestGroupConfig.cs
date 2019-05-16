using System;
using System.Collections.Generic;
using UnityEngine;

namespace SA.Foundation.Tests { 
    [Serializable]
    public class SA_TestGroupConfig {
        public List<SA_TestConfig> Tests = new List<SA_TestConfig>();
        public string Name;
        public bool Enabled = true;
        public Texture2D Texture;


    }
}