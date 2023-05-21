using UnityEditor;
using UnityEngine;

namespace LostPolygon.Unity.Utility.Editor {
    /// <summary>
    /// Exposes non-public members of the <see cref="UnityEditor.EditorGUILayout"/> via reflection.
    /// </summary>
    public static class EditorGUILayoutInternals {
        private static readonly ReflectionWrapper Wrapped =
            ReflectionWrapper.Wrap(typeof(EditorGUILayout));

        public static int BeginPlatformGrouping(GUIContent defaultTab) {
            return Wrapped.Method<int>(
                "BeginPlatformGrouping",
                new[] {
                    BuildPlatformsUtility.ValidBuildPlatformsRaw.GetType(),
                    typeof(GUIContent)
                }
            ).Invoke(BuildPlatformsUtility.ValidBuildPlatformsRaw, defaultTab);
        }

        public static int BeginPlatformGrouping(int currentValue, GUIContent defaultTab) {
            bool initialSelectedDefaultValue = Wrapped.Field("s_SelectedDefault").Property<bool>("value");
            BuildTargetGroup initialSelectedBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            try {
                EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.Unknown;
                for (int i = 0; i < BuildPlatformsUtility.ValidBuildPlatforms.Length; i++) {
                    BuildPlatform buildPlatform = BuildPlatformsUtility.ValidBuildPlatforms[i];
                    if (i == currentValue) {
                        EditorUserBuildSettings.selectedBuildTargetGroup = buildPlatform.BuildTargetGroup;
                        Wrapped.Field("s_SelectedDefault").Property<bool>("value").Set(false);
                        break;
                    }
                }

                return BeginPlatformGrouping(defaultTab);
            } finally {
                EditorUserBuildSettings.selectedBuildTargetGroup = initialSelectedBuildTargetGroup;
                Wrapped.Field("s_SelectedDefault").Property<bool>("value").Set(initialSelectedDefaultValue);
            }
        }

        public static void EndPlatformGrouping() {
            EditorGUILayout.EndVertical();
        }
    }
}
