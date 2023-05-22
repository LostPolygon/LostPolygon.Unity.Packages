using UnityEditor;
using UnityEditor.Build;

namespace LostPolygon.Unity.Utility.Editor {
    public static class EditorUserBuildSettingsUtilsInternals {
        private static ReflectionWrapper EditorUserBuildSettingsUtils { get; } =
            ReflectionWrapper.Wrap(
                typeof(EditorGUILayout).Assembly.GetType("UnityEditor.EditorUserBuildSettingsUtils")
            );

        public static NamedBuildTarget CalculateSelectedNamedBuildTarget() {
            return EditorUserBuildSettingsUtils.Method<NamedBuildTarget>("CalculateSelectedNamedBuildTarget").Invoke();
        }

        public static NamedBuildTarget CalculateActiveNamedBuildTarget() {
            return EditorUserBuildSettingsUtils.Method<NamedBuildTarget>("CalculateActiveNamedBuildTarget").Invoke();
        }

        public static bool IsSelected(this BuildPlatform buildPlatform) =>
            EditorUserBuildSettings.selectedBuildTargetGroup == BuildTargetGroup.Unknown ?
                buildPlatform.NamedBuildTarget == NamedBuildTarget.Unknown :
                buildPlatform.NamedBuildTarget == CalculateSelectedNamedBuildTarget();

        public static bool IsActive(this BuildPlatform buildPlatform) =>
            buildPlatform.NamedBuildTarget == CalculateActiveNamedBuildTarget();
    }
}
