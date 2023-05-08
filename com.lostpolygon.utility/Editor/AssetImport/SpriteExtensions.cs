using UnityEditor;
using UnityEditor.Sprites;
using UnityEngine;

namespace LostPolygon.Unity.Utility.Editor {
    public static class SpriteExtensions {
        public static bool IsTextureTightMesh(this Sprite sprite) {
            return
                sprite.packingMode == SpritePackingMode.Tight ||
                SpriteUtility.GetSpriteTexture(sprite, false).IsTightSpriteMesh();
        }

        public static bool IsSpriteTextureInSpriteImportMode(this Sprite sprite) {
            if (sprite == null)
                return false;

            Texture2D texture = SpriteUtility.GetSpriteTexture(sprite, false);
            if (texture == null)
                return false;

            TextureImporter textureImporter = texture.GetTextureImporter();
            if (textureImporter.IsTextureImporterInSpriteMode())
                return true;

            return false;
        }

        public static bool AreSpritePropertiesEqual(this Sprite a, Sprite b) {
            float aRatio = a.rect.height == 0f ? 0f : a.rect.width / a.rect.height;
            float bRatio = b.rect.height == 0f ? 0f : b.rect.width / b.rect.height;
            Vector2 normalizedAPivot = a.pivot;
            normalizedAPivot.x /= a.texture.width;
            normalizedAPivot.y /= a.texture.height;
            Vector2 normalizedBPivot = b.pivot;
            normalizedBPivot.x /= b.texture.width;
            normalizedBPivot.y /= b.texture.height;

            return
                a.border == b.border &&
                normalizedAPivot == normalizedBPivot &&
                Mathf.Abs(aRatio - bRatio) < Vector3.kEpsilon &&
                a.pixelsPerUnit == a.pixelsPerUnit;
        }

        public static SpriteAssetLazyReference ToSpriteLazyReference(this Sprite sprite) {
            return SpriteAssetLazyReference.FromSprite(sprite);
        }

        public static SpriteAssetLazyReference ToSpriteLazyReference(this Sprite sprite, string spriteTextureGuid) {
            return SpriteAssetLazyReference.FromSprite(sprite, spriteTextureGuid);
        }
    }
}
