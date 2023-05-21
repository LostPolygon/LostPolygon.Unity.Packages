using UnityEditor;
using UnityEngine;
#if LP_UNITY_2D_SPRITE_ENABLED
using UnityEditor.U2D.Sprites;
#endif

namespace LostPolygon.Unity.Utility.Editor {
    /// <summary>
    /// <see cref="TextureImporter"/> helper extensions.
    /// </summary>
    public static class TextureImporterExtensions {
#if LP_UNITY_2D_SPRITE_ENABLED
        private static readonly SpriteDataProviderFactories SpriteDataProviderFactories;
#endif

        static TextureImporterExtensions() {
#if LP_UNITY_2D_SPRITE_ENABLED
            SpriteDataProviderFactories = new SpriteDataProviderFactories();
            SpriteDataProviderFactories.Init();
#endif
        }

        public static bool DoesTextureHaveAlpha(this TextureImporter textureImporter) =>
            textureImporter.DoesSourceTextureHaveAlpha() ||
            textureImporter.alphaSource != TextureImporterAlphaSource.None;

        public static Vector2Int GetSourceTextureWidthAndHeight(this TextureImporter textureImporter) {
            textureImporter.GetSourceTextureWidthAndHeight(out int width, out int height);

            return new Vector2Int(width, height);
        }

        public static bool IsTextureImporterInSpriteMode(this TextureImporter textureImporter) =>
            textureImporter.textureType == TextureImporterType.Sprite;

        public static bool IsTextureImporterWithEditableSpriteMode(this TextureImporter textureImporter) {
            if (textureImporter == null)
                return false;

            return
                textureImporter.textureType == TextureImporterType.Sprite &&
                textureImporter.spriteImportMode != SpriteImportMode.Polygon;
        }

#if LP_UNITY_2D_SPRITE_ENABLED
        public static ISpriteEditorDataProvider GetSpriteEditorDataProvider(this TextureImporter textureImporter) {
            return SpriteDataProviderFactories.GetSpriteEditorDataProviderFromObject(textureImporter);
        }
#endif
    }
}
