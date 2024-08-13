using UnityEngine;

namespace LostPolygon.Unity.Utility {
    public static class ColorExtensions {
        public static Color WithAlpha(this Color color, float alpha) {
            color.a = alpha;
            return color;
        }

        public static Color WithRgbColor(this Color color, in Color rgbColor) {
            color.r = rgbColor.r;
            color.g = rgbColor.g;
            color.b = rgbColor.b;
            return color;
        }
    }
}
