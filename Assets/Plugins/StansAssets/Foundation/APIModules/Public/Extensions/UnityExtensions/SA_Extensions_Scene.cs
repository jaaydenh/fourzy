using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

using SA.Foundation;

public static class SA_Extensions_Scene
{
    
    public static T GetComponentInScene<T>(this Scene scene) {
        foreach(var gameObject in scene.GetRootGameObjects()) {
            T component = gameObject.GetComponentInChildren<T>();
            if(component != null) {
                return component;
            }
        }
        return default(T);
    }

}
