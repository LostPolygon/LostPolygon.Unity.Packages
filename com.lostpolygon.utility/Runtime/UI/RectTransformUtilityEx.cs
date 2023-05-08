using System.Buffers;
using UnityEngine;

namespace LostPolygon.Unity.Utility {
    public static class RectTransformUtilityEx {
        public static Rect ScreenSpaceRect(RectTransform rectTransform, Camera camera) {
            Vector3[] worldCorners = ArrayPool<Vector3>.Shared.Rent(4);
            try {
                rectTransform.GetWorldCorners(worldCorners);
                Vector3 bottomLeftCorner = camera.WorldToScreenPoint(worldCorners[0]);
                Vector3 topRightCorner = camera.WorldToScreenPoint(worldCorners[2]);

                return Rect.MinMaxRect(bottomLeftCorner.x, bottomLeftCorner.y, topRightCorner.x, topRightCorner.y);
            } finally {
                ArrayPool<Vector3>.Shared.Return(worldCorners);
            }
        }
    }
}
