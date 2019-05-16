using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation;

public static class SA_Extensions_Material  {

    public static void SetAlpha(this Material material, float value) {
        if (material.HasProperty("_Color")) {
            Color color = material.color;
            color.a = value;
            material.color = color;
        }
    }

}
