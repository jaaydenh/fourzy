using System;
using System.Collections.Generic;
using UnityEngine;


namespace SA.iOS.UIKit
{
    [Serializable]
    public class ISN_UIUrlType 
    {


        public string Identifier = string.Empty;
        public List<string> Schemes = new List<string>();


        public ISN_UIUrlType(string identifier) {
            Identifier = identifier;
        }

        public void AddSchemes(string schemes) {
            Schemes.Add(schemes);
        }
    }
}