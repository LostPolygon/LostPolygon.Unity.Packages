#nullable enable

using UnityEditor;

namespace LostPolygon.Unity.Utility.Editor {
    /// <summary>
    /// Helper utilities for working with Textures.
    /// </summary>
    public static class TextureImportUtility {
        public static TextureImporter? GetTextureImporterByPath(string texturePath) {
            return AssetImporter.GetAtPath(texturePath) as TextureImporter;
        }

        public static TextureImporter? GetTextureImporterByGuid(string textureAssetGuid) {
            return AssetImportUtility.GetAssetImporterByGuid(textureAssetGuid) as TextureImporter;
        }
    }
}
