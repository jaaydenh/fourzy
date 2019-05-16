using UnityEngine;

public static class SA_Extensions_Renderer  {
    
    public static bool IsVisibleFrom(this Renderer renderer, Camera camera) {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }

}
