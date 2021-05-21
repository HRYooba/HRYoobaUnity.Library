using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HRYooba.Library
{
    public static class RandomUtility
    {
        public static Vector3 Vector3Range(Vector3 min, Vector3 max)
        {
            return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
        }
    }
}