using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace LostPolygon.Unity.Utility.Editor {
    public static class AssetLazyReferenceExtensions {
        public static bool IsNull<T>(this AssetLazyReference<T> assetLazyReference, bool checkInstance = false) where T : Object {
            return
                assetLazyReference == null ||
                String.IsNullOrEmpty(assetLazyReference.AssetReference.Guid) ||
                (checkInstance && assetLazyReference.GetInstance() == null);
        }

        public static bool IsInvalid(this SpriteAssetLazyReference spriteAssetLazyReference, bool checkInstance = false) {
            return
                spriteAssetLazyReference == default ||
                String.IsNullOrEmpty(spriteAssetLazyReference.AssetReference.Guid) ||
                (checkInstance && spriteAssetLazyReference.GetInstance() == null);
        }

        public static SpriteAssetLazyReference? AsNullable(this SpriteAssetLazyReference spriteAssetLazyReference, bool checkInstance = false) {
            return spriteAssetLazyReference.IsInvalid(checkInstance) ? null : spriteAssetLazyReference;
        }

        public static bool ExistsInAssetDatabase(this SpriteAssetLazyReference spriteAssetLazyReference) {
            if (spriteAssetLazyReference.IsInvalid())
                return false;

            if (String.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath(spriteAssetLazyReference.AssetReference.Guid)))
                return false;

            return true;
        }
    }
}
