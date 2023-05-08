using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LostPolygon.Unity.Utility;
using LostPolygon.Unity.Utility.Editor;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace LostPolygon.Unity.BuildMetadata.Editor {
    /// <summary>
    /// Updates build metadata using data from external source and Cloud Build manifest.
    /// </summary>
    public abstract class StandardBuildMetaDataGenerator<T> : IPreprocessBuildWithReport
        where T : StandardBuildMetaData<T> {

        protected virtual IReadOnlyList<string> ReleaseGitBranchNames { get; } = new[] {
            "qa",
            "staging",
            "release"
        };

        protected virtual IReadOnlyList<string> ReleaseGitBranchPrefixes { get; } = new[] {
            "release/",
        };

        public virtual int callbackOrder { get; } = 0;

        public virtual void OnPreprocessBuild(BuildReport report) {
            UpdateBuildMetaData();
        }

        public virtual BasicBuildMetaData CreateBasicBuildMetaData() {
            string gitBranchName = CommandExecutor.ExecuteCommandWithOutput("git", "rev-parse --abbrev-ref HEAD");
            string gitCommitHash = CommandExecutor.ExecuteCommandWithOutput("git", "log --pretty=format:%h -n 1");
            int gitRevisionCount =
                Convert.ToInt32(CommandExecutor.ExecuteCommandWithOutput("git", "rev-list --no-merges --count HEAD"));
            long gitLastCommitUnixTimestamp =
                Convert.ToInt64(CommandExecutor.ExecuteCommandWithOutput("git", "show -s --format=%ct"));
            DateTimeOffset gitLastCommitDateTime = DateTimeOffset.FromUnixTimeSeconds(gitLastCommitUnixTimestamp);
            bool isReleaseGitBranch =
                ReleaseGitBranchNames.Contains(gitBranchName) ||
                ReleaseGitBranchPrefixes.Any(prefix => gitBranchName.StartsWith(prefix));

            // Only use YYYY.MM versioning on release builds to differentiate from dev builds
            string majorMinorVersion = isReleaseGitBranch ? $"{gitLastCommitDateTime.Year}.{gitLastCommitDateTime.Month}" : "0.0";

            // Use git commit count as monotonic counter to avoid
            // having different versions for same code when built at different times
            int patchVersion = gitRevisionCount;
            string fullVersion = $"{majorMinorVersion}.{patchVersion}";

            return new BasicBuildMetaData(
                fullVersion,
                DateTime.UtcNow,
                gitBranchName,
                gitCommitHash,
                isReleaseGitBranch
            );
        }

        public virtual void UpdateBuildMetaData() {
            BasicBuildMetaData basicBuildMetadata = CreateBasicBuildMetaData();

            if (!basicBuildMetadata.IsReleaseBuild) {
                Debug.LogWarning($"[BuildMetaDataGenerator] Building from non-release branch '{basicBuildMetadata.GitBranchName}', version would be set to {basicBuildMetadata.Version}");
            } else {
                Debug.Log($"[BuildMetaDataGenerator] Build version set to {basicBuildMetadata.Version}");
            }

            // Save new data
            T buildMetaData = GetOrCreateBuildMetaData();
            buildMetaData.SetBasicBuildMetaData(basicBuildMetadata);

            EditorUtility.SetDirty(buildMetaData);
        }

        // ReSharper disable once RedundantNameQualifier
        public virtual void PreCloudBuildExport(UnityEngine.CloudBuild.BuildManifestObject manifest) {
#if UNITY_CLOUD_BUILD
            Debug.Log("Cloud Build manifest:\r\n" + manifest.ToJson());
#endif

            T buildMetaData = GetOrCreateBuildMetaData();

            const int gitShortHashLength = 8;
            buildMetaData.SetBasicBuildMetaData(buildMetaData.ToBasicBuildMetaData() with {
                GitBranchName = manifest.GetValue<string>("scmBranch"),
                GitCommitHash = manifest.GetValue<string>("scmCommitId")[..gitShortHashLength]
            });
            buildMetaData.SetCloudBuildMetadata(
                manifest.GetValue<string>("cloudBuildTargetName"),
                Convert.ToInt32(manifest.GetValue<string>("buildNumber"))
            );

            UnityEditor.EditorUtility.SetDirty(buildMetaData);
        }

        protected static T GetOrCreateBuildMetaData() {
            T instance = StandardBuildMetaData<T>.GetInstance(false);
            if (instance != null)
                return instance;

            instance = ScriptableObject.CreateInstance<T>();
            AssetPathAttribute assetPathAttribute = typeof(T).GetCustomAttribute<AssetPathAttribute>();
            if (assetPathAttribute == null || String.IsNullOrWhiteSpace(assetPathAttribute.EditorPath))
                throw new Exception($"{nameof(AssetPathAttribute)} on type {typeof(T).Name} is missing or invalid");

            AssetDatabase.CreateAsset(instance, assetPathAttribute.EditorPath);
            return instance;
        }
    }
}
