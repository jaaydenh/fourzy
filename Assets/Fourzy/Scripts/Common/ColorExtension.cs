using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExtension {

    public static Color WithAlpha(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }
}
