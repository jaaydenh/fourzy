using System;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation;

public static class SA_Extensions_Texture2D  {

    public static Texture2D Rotate(this Texture2D texture, float angle) {
        return SA.Foundation.Utility.SA_IconManager.Rotate(texture, angle);
    }


    public static Sprite ToSprite(this Texture2D texture) {
        return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }


    public static byte[] ToBytesArray(this Texture2D texture) {
        return texture.EncodeToPNG(); 
    }

    public static string ToBase64String(this Texture2D texture) {
        byte[] val = texture.ToBytesArray();
        return Convert.ToBase64String(val);
    }


    public static Texture2D LoadImageFromBase64(this Texture2D texture, string base64EncodedStrong) {
        byte[] decodedFromBase64 = base64EncodedStrong.BytesFromBase64String();
        texture.LoadImage(decodedFromBase64);

        return texture;
    }


}
