using UnityEditor;

namespace LostPolygon.Unity.Utility.Editor {
    public static class AssetImportUtility {
        public static AssetImporter GetAssetImporterByGuid(string assetGuid) {
            return AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(assetGuid));
        }

        public static void ForceWriteImportSettings(AssetImporter assetImporter) {
            //Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(assetImporter.assetPath);
            //EditorUtility.SetDirty(texture);
            EditorUtility.SetDirty(assetImporter);
            AssetDatabase.WriteImportSettingsIfDirty(assetImporter.assetPath);
        }
    }
}
