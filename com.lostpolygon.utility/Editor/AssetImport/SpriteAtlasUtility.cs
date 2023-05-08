using System.IO;

namespace LostPolygon.Unity.Utility.Editor {
    public static class SpriteAtlasUtility {
        public static void ClearSpritePackerAtlasCache() {
            string[] directories = GetSpritePackerAtlasCacheDirectories();
            if (directories == null)
                return;

            foreach (string directory in directories) {
                Directory.Delete(directory, true);
            }
        }

        private static string[] GetSpritePackerAtlasCacheDirectories() {
            string atlasCachePath = GetAtlasCachePath();

            if (!Directory.Exists(atlasCachePath))
                return null;

            string[] directories = Directory.GetDirectories(atlasCachePath);
            return directories;
        }

        private static string GetAtlasCachePath() {
            string atlasCachePath = Path.Combine("Library", "AtlasCache");
            atlasCachePath = Path.GetFullPath(atlasCachePath);
            return atlasCachePath;
        }
    }
}
