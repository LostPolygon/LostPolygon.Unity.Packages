using System;
using UnityEditor;
using UnityEngine;

namespace LostPolygon.Unity.Utility.Editor {
    /// <summary>
    /// Exposes non-public members of the <see cref="UnityEditor.EditorGUILayout"/> via reflection.
    /// </summary>
    public static class EditorGUILayoutInternals {
        private static ReflectionWrapper Wrapped { get; } =
            ReflectionWrapper.Wrap(typeof(EditorGUILayout)); 

        private static readonly GUIStyle EditorStylesFrameBox =
            ReflectionWrapper.Wrap(typeof(EditorStyles))
                .Property<GUIStyle>("frameBox");

        public static int BeginPlatformGrouping(
            GUIContent defaultTab,
            GUIStyle guiStyle = null,
            Func<BuildPlatformId, bool> showOverrideForPlatform = null
        ) {
            guiStyle ??= EditorStylesFrameBox;
            Func<int, bool> showOverrideForPlatformInternal =
                showOverrideForPlatform != null ?
                    index => {
                        BuildPlatformId buildPlatformId =
                            index != -1 ? 
                                BuildPlatformsUtility.ValidBuildPlatforms[index].BuildPlatformId : 
                                BuildPlatformId.Default;
                        
                        return showOverrideForPlatform(buildPlatformId);
                    } :
                    null;
            return Wrapped.Method<int>(
                "BeginPlatformGrouping",
                new[] {
                    BuildPlatformsUtility.ValidBuildPlatformsRaw.GetType(),
                    typeof(GUIContent),
                    typeof(GUIStyle),
                    typeof(Func<int, bool>) // Func<int, bool> showOverrideForPlatform
                }
            ).Invoke(BuildPlatformsUtility.ValidBuildPlatformsRaw, defaultTab, guiStyle, showOverrideForPlatformInternal);
        }

        public static int BeginPlatformGrouping(
            int currentValue,
            GUIContent defaultTab,
            GUIStyle guiStyle = null,
            Func<BuildPlatformId, bool> showOverrideForPlatform = null
        ) {
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

                return BeginPlatformGrouping(defaultTab, guiStyle, showOverrideForPlatform);
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
