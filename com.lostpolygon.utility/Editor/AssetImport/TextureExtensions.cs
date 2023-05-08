using UnityEditor;
using UnityEngine;

namespace LostPolygon.Unity.Utility.Editor {
    /// <summary>
    /// <see cref="Texture"/> helper extensions.
    /// </summary>
    public static class TextureExtensions {
        public static TextureImporter GetTextureImporter(this Texture2D texture) {
            string assetPath = AssetDatabase.GetAssetPath(texture);
            TextureImporter textureImporter = TextureImportUtility.GetTextureImporterByPath(assetPath);

            return textureImporter;
        }

        public static TextureImporterSettings GetTextureImporterSettings(this Texture2D texture) {
            TextureImporter textureImporter = GetTextureImporter(texture);
            if (textureImporter == null)
                return null;

            TextureImporterSettings textureImporterSettings = new();
            textureImporter.ReadTextureSettings(textureImporterSettings);
            return textureImporterSettings;
        }

        public static bool IsTightSpriteMesh(this Texture2D texture) {
            TextureImporterSettings settings = texture.GetTextureImporterSettings();
            if (settings == null)
                return false;

            return settings.spriteMeshType == SpriteMeshType.Tight;
        }
    }
}
