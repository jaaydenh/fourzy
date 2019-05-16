using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SA.Foundation;

public static class SA_Extensions_Vector3  {
    

    public static Vector3 Reset(this Vector3 vec) {
        return Vector3.zero;
    }


    public static Vector3 ResetXCoord(this Vector3 vec) {
        Vector3 newVec = vec;
        newVec.x = 0f;

        return newVec;
    }

    public static Vector3 ResetYCoord(this Vector3 vec) {
        Vector3 newVec = vec;
        newVec.y = 0f;

        return newVec;
    }

    public static Vector3 ResetZCoord(this Vector3 vec) {
        Vector3 newVec = vec;
        newVec.z = 0f;

        return newVec;
    }

    public static Vector3 Average(this Vector3[] f) {
        var temp = Vector3.zero;
        var validValues = 0;

        for (int i = 0; i < f.Length - 1; i++) {
            // If a value is 0.0f it means we didn't collect that sample yet, therefore, we'll ignore it in our average
            if (f[i] != Vector3.zero) {
                temp += f[i];
                validValues++;
            }
        }

        // Prevent divisions by 0, and as such, the destruction of the world
        validValues = (validValues > 0) ? validValues : 1;

        temp.x = temp.x / validValues;
        temp.y = temp.y / validValues;
        temp.z = temp.z / validValues;

        return temp;

    }

    public static void Add(this Vector3[] v3, Vector3 value) {
        for (int i = v3.Length - 1; i > 0; i--)
            v3[i] = v3[i - 1];

        v3[0] = value;
    }
}
