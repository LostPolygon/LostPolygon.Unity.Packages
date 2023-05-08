using UnityEngine;

namespace LostPolygon.Unity.Utility {
    public static class SpriteRendererExtensions {
        public static void CopyMainPropertiesFrom(this SpriteRenderer target, SpriteRenderer source) {
            target.sortingLayerID = source.sortingLayerID;
            target.sortingOrder = source.sortingOrder;
            target.color = source.color;
            target.drawMode = target.drawMode;
            target.adaptiveModeThreshold = source.adaptiveModeThreshold;
            target.spriteSortPoint = source.spriteSortPoint;
            target.size = source.size;
            target.flipX = source.flipX;
            target.flipY = source.flipY;
            target.maskInteraction = source.maskInteraction;
            target.tileMode = source.tileMode;
        }
    }
}
