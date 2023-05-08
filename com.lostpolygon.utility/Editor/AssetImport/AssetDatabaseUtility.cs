using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Object = UnityEngine.Object;

namespace LostPolygon.Unity.Utility.Editor {
    /// <summary>
    /// Helper utilities for working with Textures.
    /// </summary>
    public static class AssetDatabaseUtility {
        public static void ReimportDistinctAssets<T>(IEnumerable<T> assets) where T : UnityEngine.Object {
            ReimportAssets(assets.Distinct().ToArray());
        }

        public static void ReimportAssets<T>(IEnumerable<T> assets) where T : UnityEngine.Object {
            ReimportAssets(assets.Select(AssetDatabase.GetAssetPath));
        }

        public static void ReimportAssets(IEnumerable<string> paths) {
            if (!paths.Any())
                return;

            try {
                AssetDatabase.StartAssetEditing();
                foreach (string path in paths) {
                    AssetDatabase.ImportAsset(
                        path,
                        ImportAssetOptions.ForceUpdate | ImportAssetOptions.DontDownloadFromCacheServer
                    );
                }
            } finally {
                AssetDatabase.StopAssetEditing();
            }
        }

        public static bool IsPathInsideAssets(string path) {
            return path.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase);
        }

        public static T[] LoadAllAssetsAtPath<T>(string path) where T : Object {
            T[] assets =
                AssetDatabase
                    .LoadAllAssetsAtPath(path)
                    .OfType<T>()
                    .ToArray();

            return assets;
        }

        public static Object GetSubAssetByGuidAndLocalFileIdentifier(string guid, long localIdentifier) {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (String.IsNullOrEmpty(assetPath))
                return null;

            Object[] objects = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            if (objects == null || objects.Length == 0)
                return null;

            foreach (Object obj in objects) {
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(obj, out string _, out long objectLocalId);
                if (objectLocalId == localIdentifier)
                    return obj;
            }

            return null;
        }
    }
}
