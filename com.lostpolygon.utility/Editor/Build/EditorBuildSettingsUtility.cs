using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace LostPolygon.Unity.Utility.Editor {
    public static class EditorBuildSettingsUtility {
        public static string[] GetEnabledScenes() {
            return
                EditorBuildSettings.scenes
                    .Where(scene => scene.enabled)
                    .Select(s => s.path)
                    .ToArray();
        }

        public static void ApplyScenes(EditorBuildSettingsScene[] overrideScenes) {
            List<EditorBuildSettingsScene> newScenes = new(EditorBuildSettings.scenes);
            foreach (EditorBuildSettingsScene overrideScene in overrideScenes) {
                bool matched = false;
                foreach (EditorBuildSettingsScene newScene in newScenes) {
                    if (overrideScene.CompareTo(newScene) == 0) {
                        newScene.enabled = overrideScene.enabled;
                        matched = true;
                    }
                }

                if (!matched) {
                    newScenes.Add(overrideScene);
                }
            }

            EditorBuildSettings.scenes = newScenes.ToArray();
        }
    }
}
