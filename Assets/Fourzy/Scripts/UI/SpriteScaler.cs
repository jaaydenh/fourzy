using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteScaler : MonoBehaviour
{
    private void Awake()
    {
        float aspectRatio = (float)Screen.width / Screen.height;
        float standardAspectRatio = 750f / 1334f;
        if (aspectRatio > standardAspectRatio)
        {
            Vector3 scale = this.transform.localScale;
            scale = scale * (standardAspectRatio / aspectRatio);
            this.transform.localScale = scale;
        }
    }
}
