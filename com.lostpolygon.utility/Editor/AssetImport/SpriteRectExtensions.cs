#if LP_UNITY_2D_SPRITE_ENABLED

using UnityEditor;

namespace LostPolygon.Unity.Utility.Editor {
    public static class SpriteRectExtensions {
        public static SpriteRect Clone(this SpriteRect spriteRect, bool generateNewSpriteId = true) {
            return new SpriteRect {
                alignment = spriteRect.alignment,
                border = spriteRect.border,
                name = spriteRect.name,
                pivot = spriteRect.pivot,
                rect = spriteRect.rect,
                spriteID = generateNewSpriteId ? GUID.Generate() : spriteRect.spriteID
            };
        }
    }
}

#endif
