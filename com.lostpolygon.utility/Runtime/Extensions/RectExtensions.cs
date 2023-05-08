using UnityEngine;

namespace LostPolygon.Unity.Utility {
    /// <summary>
    /// <see cref="Rect"/> helper extensions.
    /// </summary>
    public static class RectExtensions {
        public static RectInt MinMaxRectInt(int left, int top, int right, int bottom) {
            return new RectInt(left, top, right - left, bottom - top);
        }

        public static Rect ToRect(this in RectInt rectInt) {
            return new Rect(rectInt.position, rectInt.size);
        }

        public static RectInt ToIntRectWithPositiveBias(this in Rect rect) {
            return new RectInt(
                Mathf.FloorToInt(rect.xMin),
                Mathf.FloorToInt(rect.yMin),
                Mathf.CeilToInt(rect.width),
                Mathf.CeilToInt(rect.height)
            );
        }
    }
}
