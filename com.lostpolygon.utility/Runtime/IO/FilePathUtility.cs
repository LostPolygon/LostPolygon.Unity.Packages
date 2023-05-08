using System;

namespace LostPolygon.Unity.Utility {
    /// <summary>
    /// Utilities for working with file paths.
    /// </summary>
    public static class FilePathUtility {
        public static string MakeRelativePath(string path, string referencePath) {
            Uri fileUri = new(path);
            Uri referenceUri = new(referencePath);
            return FixSlashes(Uri.UnescapeDataString(referenceUri.MakeRelativeUri(fileUri).ToString()));
        }

        public static string FixSlashes(string path) {
            return path.Replace('\\', '/');
        }
    }
}
