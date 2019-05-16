using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SA.iOS.Examples {
    [Serializable]
    public class SA_ExampleSceneConfig : MonoBehaviour {
        
        public List<SA_ExampleServicesConfig> Services = new List<SA_ExampleServicesConfig>();

        public Sprite Logo;
    }
}
