using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraUtility
{
    public static float HorizontalToVerticalFOV(float horizontalFOV, float aspect)
    {
        return Mathf.Rad2Deg * 2 * Mathf.Atan(Mathf.Tan((horizontalFOV * Mathf.Deg2Rad) / 2f) / aspect);
    }
}
