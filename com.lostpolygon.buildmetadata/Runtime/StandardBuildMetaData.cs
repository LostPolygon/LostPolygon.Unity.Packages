using System;
using System.Globalization;
using System.Reflection;
using LostPolygon.Unity.Utility;
using UnityEngine;

namespace LostPolygon.Unity.BuildMetadata {
    public class StandardBuildMetaData<T> : ScriptableObject, IBasicBuildMetaData
        where T : ScriptableObject, IBasicBuildMetaData {
        private static T _instance;

        [SerializeField]
        private string _version = "0.0.0";

        [SerializeField]
        private string _buildDateTime = "";

        [SerializeField]
        private string _gitBranchName = "";

        [SerializeField]
        private string _gitCommitHash = "";

        [SerializeField]
        private bool _isReleaseBuild;

        [SerializeField]
        private string _cloudBuildTargetName = "";

        [SerializeField]
        private int _cloudBuildBuildNumber;

        public string FullVersionText {
            get {
                string text = Version;

                if (!String.IsNullOrEmpty(GitCommitHash)) {
                    text +=
                        CloudBuildBuildNumber > 0 ?
                            $" ({GitCommitHash} #{CloudBuildBuildNumber})" :
                            $" ({GitCommitHash}/{GitBranchName})";
                }

                return text;
            }
        }

        public DateTime BuildDateTime =>
            String.IsNullOrWhiteSpace(_buildDateTime) ?
                DateTime.UnixEpoch :
                DateTime.ParseExact(_buildDateTime, "s", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

        public string BuildDateTimeString => _buildDateTime;

        public string Version => _version;

        public string GitBranchName => _gitBranchName;

        public string GitCommitHash => _gitCommitHash;

        public bool IsReleaseBuild => _isReleaseBuild;

        public string CloudBuildTargetName => _cloudBuildTargetName;

        public int CloudBuildBuildNumber => _cloudBuildBuildNumber;

        public BasicBuildMetaData ToBasicBuildMetaData() =>
            new(
                Version,
                BuildDateTime,
                GitBranchName,
                GitCommitHash,
                IsReleaseBuild
            );

        public void SetBasicBuildMetaData(BasicBuildMetaData metaData) {
            _version = metaData.Version;
            _buildDateTime = metaData.BuildDateTime.ToString("s");
            _gitBranchName = metaData.GitBranchName;
            _gitCommitHash = metaData.GitCommitHash;
            _isReleaseBuild = metaData.IsReleaseBuild;
        }

        public void SetCloudBuildMetadata(
            string targetName,
            int buildNumber
        ) {
            _cloudBuildTargetName = targetName;
            _cloudBuildBuildNumber = buildNumber;
        }

        public static T GetInstance(bool throwIfNotFound) {
            if (!_instance) {
                string resourcesPath = "BuildMetaData";
                AssetPathAttribute assetPathAttribute = typeof(T).GetCustomAttribute<AssetPathAttribute>();
                if (assetPathAttribute != null) {
                    resourcesPath = assetPathAttribute.ResourcesPath;
                }

                _instance = Resources.Load<T>(resourcesPath);
                if (throwIfNotFound && _instance == null)
                    throw new Exception($"'{resourcesPath}' file not found in Resources");
            }

            return _instance;
        }

        public static T Instance => GetInstance(true);
    }
}
