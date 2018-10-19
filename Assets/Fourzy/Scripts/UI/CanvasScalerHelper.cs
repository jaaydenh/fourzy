using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasScalerHelper : MonoBehaviour
{
    private void Awake()
    {
        CanvasScaler canvasScaler = this.GetComponent<CanvasScaler>();
        
        float aspectRatio = (float)Screen.width / Screen.height;
        float standardAspectRatio = canvasScaler.referenceResolution.x / canvasScaler.referenceResolution.y;
        if (aspectRatio > standardAspectRatio)
        {
            canvasScaler.matchWidthOrHeight = 1.0f;
        }
        else
        {
            canvasScaler.matchWidthOrHeight = 0.0f;
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        this.Awake();
    }
#endif
}
