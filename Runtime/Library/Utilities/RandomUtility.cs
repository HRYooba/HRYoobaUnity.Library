using UnityEngine;

namespace HRYooba.Library
{
    public static class RandomUtility
    {
        public static Vector2 Vector2Range(Vector2 min, Vector2 max)
        {
            return new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
        }

        public static Vector3 Vector3Range(Vector3 min, Vector3 max)
        {
            return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
        }

        public static Vector3 Vector4Range(Vector4 min, Vector4 max)
        {
            return new Vector4(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z), Random.Range(min.w, max.w));
        }

        public static Color ColorRange(Color color1, Color color2)
        {
            return new Color(Random.Range(color1.r, color2.r), Random.Range(color1.g, color2.g), Random.Range(color1.b, color2.b), Random.Range(color1.a, color2.a));
        }
    }
}