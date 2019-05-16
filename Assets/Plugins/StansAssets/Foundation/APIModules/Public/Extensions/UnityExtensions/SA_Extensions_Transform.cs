using UnityEngine;

public static class SA_Extensions_Transform  {



    public static void SetGlobalScale(this Transform transform, Vector3 globalScale) {
        transform.localScale = Vector3.one;
        var lossyScale = transform.lossyScale;
        transform.localScale = new Vector3(globalScale.x / lossyScale.x, globalScale.y / lossyScale.y, globalScale.z / lossyScale.z);
    }

    public static void Reset(this Transform t) {
        t.localScale = Vector3.one;
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
    }

    public static Transform Clear(this Transform transform) {

        if (transform.childCount == 0) {
            return transform;
        }

        Transform[] children = transform.GetComponentsInChildren<Transform>();

        foreach (Transform child in children) {
            if (child != transform && child != null) {
                Object.DestroyImmediate(child.gameObject);
            }
        }
        return transform;
    }

    public static Transform FindOrCreateChild(this Transform target, string name) {
        var part = target.Find(name);
        if (part == null) {
            part = new GameObject(name).transform;
            part.parent = target;
            part.Reset();
        }
        return part;
    }


    //--------------------------------------
    // Bounds
    //--------------------------------------
  

    public static Bounds GetRendererBounds(this Transform t) {
        return t.gameObject.GetRendererBounds();
    }


    public static Vector3 GetVertex(this Transform t, SA_VertexX x, SA_VertexY y, SA_VertexZ z) {
        return t.gameObject.GetVertex(x, y, z);
    }




}
