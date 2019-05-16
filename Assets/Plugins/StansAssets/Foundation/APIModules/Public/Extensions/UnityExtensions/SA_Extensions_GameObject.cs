using UnityEngine;
using System;
using SA.Foundation.Animation;

public static class SA_Extensions_GameObject  {



    public static void Reset(this GameObject go) {
        go.transform.Reset();
    }

    public static void Clear(this GameObject go)
    {
        go.transform.Clear();
    }

    //--------------------------------------
    // Animation
    //--------------------------------------

    public static void RotateTo(this GameObject go, Vector3 eulerRotation, float time, SA_EaseType easeType = SA_EaseType.linear) {
        RotateGameObjectTo(go, go, eulerRotation, time, easeType, null);
    }

    public static void RotateTo(this GameObject go, object callbackTarget, Vector3 eulerRotation, float time, SA_EaseType easeType, Action OnCompleteAction) {
        RotateGameObjectTo(go, callbackTarget, eulerRotation, time, easeType, OnCompleteAction);
    }

    public static void RotateGameObjectTo(this GameObject go, object callbackTarget, Vector3 eulerRotation, float time, SA_EaseType easeType, Action OnCompleteAction) {
        SA_ValuesTween tw = go.AddComponent<SA_ValuesTween>();

        tw.DestoryGameObjectOnComplete = false;
        tw.RotateTo(go.transform.rotation.eulerAngles, eulerRotation, time, easeType);

        tw.OnComplete.AddSafeListener(callbackTarget, OnCompleteAction);
    }

    public static void MoveTo(this GameObject go,  Vector3 position, float time, SA_EaseType easeType = SA_EaseType.linear) {
        MoveGameObjectTo(go, go, position, time, easeType, null);
    }

    public static void MoveTo(this GameObject go, object callbackTarget, Vector3 position, float time, SA_EaseType easeType, Action OnCompleteAction) {
        MoveGameObjectTo(go, callbackTarget, position, time, easeType, OnCompleteAction);
    }

    public static void MoveGameObjectTo(GameObject go, object callbackTarget, Vector3 position, float time, SA_EaseType easeType, Action OnCompleteAction ) {
        SA_ValuesTween tw = go.AddComponent<SA_ValuesTween>();

        tw.DestoryGameObjectOnComplete = false;
        tw.VectorTo(go.transform.position, position, time, easeType);

        tw.OnComplete.AddSafeListener(callbackTarget, OnCompleteAction);
    }


    public static void ScaleTo(this GameObject go, Vector3 scale, float time, SA_EaseType easeType = SA_EaseType.linear) {
        ScaleGameObjectTo(go, go, scale, time, easeType, null);
    }

    public static void ScaleTo(this GameObject go, object callbackTarget,  Vector3 scale, float time, SA_EaseType easeType, Action OnCompleteAction) {
        ScaleGameObjectTo(go, callbackTarget, scale, time, easeType, OnCompleteAction);
    }

    public static void ScaleGameObjectTo(GameObject go, object callbackTarget, Vector3 scale, float time, SA_EaseType easeType, Action OnCompleteAction) {
        SA_ValuesTween tw = go.AddComponent<SA_ValuesTween>();

        tw.DestoryGameObjectOnComplete = false;
        tw.ScaleTo(go.transform.localScale, scale, time, easeType);

        tw.OnComplete.AddSafeListener(callbackTarget, OnCompleteAction);
    }




    //--------------------------------------
    // Bounds
    //--------------------------------------

    public static Bounds GetRendererBounds(this GameObject go) {
        return SA_Extensions_Bounds.CalculateBounds(go);
    }

    public static Vector3 GetVertex(this GameObject go, SA_VertexX x, SA_VertexY y, SA_VertexZ z) {
        Bounds bounds = go.GetRendererBounds();
        return bounds.GetVertex(x, y, z);
    }

    //--------------------------------------
    // Layers
    //--------------------------------------


    // public static void SetLayerRecursively(this GameObject go, int layerNumber) {
    //     foreach (Transform trans in go.GetComponentsInChildren<Transform>(true)) {
    //         trans.gameObject.layer = layerNumber;
    //     }
    // }



}
