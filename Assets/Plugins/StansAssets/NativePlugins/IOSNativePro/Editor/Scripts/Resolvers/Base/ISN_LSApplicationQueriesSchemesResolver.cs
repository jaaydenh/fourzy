using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SA.iOS.XCode;


namespace SA.iOS
{
    public abstract class ISN_LSApplicationQueriesSchemesResolver : ISN_APIResolver {


        protected override void RemoveXcodePlistKey(ISD_PlistKey key) {
            if (key.Name.Equals("LSApplicationQueriesSchemes")) {
                var existingKey = ISD_API.GetInfoPlistKey("LSApplicationQueriesSchemes");
                if (existingKey == null) {
                    return;
                }

                List<ISD_PlistKey> keysToRemove = new List<ISD_PlistKey>();
                foreach (var testeChild in key.Children) {
                    var existingChild = existingKey.GetChildByStringValue(testeChild.StringValue);
                    if (existingChild != null) {
                        keysToRemove.Add(existingChild);
                    }
                }

                if (keysToRemove.Count == existingKey.Children.Count) {
                    ISD_API.RemoveInfoPlistKey(existingKey);
                } else {
                    foreach (var removeKey in keysToRemove) {
                        existingKey.RemoveChild(removeKey);
                    }
                }
            } else {
                base.RemoveXcodePlistKey(key);
            }
        }


        protected override void AddXcodePlistKey(ISD_PlistKey key) {
            if (key.Name.Equals("LSApplicationQueriesSchemes")) {
                var existingKey = ISD_API.GetInfoPlistKey("LSApplicationQueriesSchemes");
                if (existingKey == null) {
                    ISD_API.SetInfoPlistKey(key);
                } else {
                    List<ISD_PlistKey> missingKeys = new List<ISD_PlistKey>();

                    foreach (var testeChild in key.Children) {
                        bool contains = false;
                        foreach (var child in existingKey.Children) {
                            if (child.StringValue.Equals(testeChild.StringValue)) {
                                contains = true;
                            }
                        }
                        if (!contains) {
                            missingKeys.Add(testeChild);
                        }
                    }

                    foreach (var child in missingKeys) {
                        existingKey.AddChild(child);
                    }
                }
            } else {
                base.AddXcodePlistKey(key);
            }
        }

             
                
    	
    }
}