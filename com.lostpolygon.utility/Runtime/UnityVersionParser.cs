using System;
using System.Text.RegularExpressions;

namespace LostPolygon.Unity.Utility {
    public class UnityVersionParser {
        private readonly int _versionMajor;
        private readonly int _versionMinor;
        private readonly int _versionPatch;
        private readonly UnityBuildType? _versionBuildType = UnityBuildType.Unknown;
        private readonly int? _versionReleaseNumber;
        private readonly Version _version;

        public UnityVersionParser(string unityVersion) {
            Match versionMatch = Regex.Match(unityVersion, @"(\d+)\.(\d+)\.(\d+)([abpf])?(\d+)?");
            _versionMajor = TryParseInt(versionMatch.Groups[1].Value, 0);
            _versionMinor = TryParseInt(versionMatch.Groups[2].Value, 0);
            _versionPatch = TryParseInt(versionMatch.Groups[3].Value, 0);
            if (versionMatch.Groups.Count > 4) {
                string versionBuildType = versionMatch.Groups[4].Value;
                _versionBuildType = versionBuildType switch {
                    "f" => UnityBuildType.Final,
                    "p" => UnityBuildType.Patch,
                    "a" => UnityBuildType.Alpha,
                    "b" => UnityBuildType.Beta,
                    _ => UnityBuildType.Unknown
                };

                _versionReleaseNumber = TryParseInt(versionMatch.Groups[5].Value);
            }

            _version = new Version(_versionMajor, _versionMinor, _versionPatch);
        }

        public Version Version => _version;

        public int VersionMajor => _versionMajor;

        public int VersionMinor => _versionMinor;

        public int VersionPatch => _versionPatch;

        public UnityBuildType? VersionBuildType => _versionBuildType;

        public int? VersionReleaseNumber => _versionReleaseNumber;

        public override string ToString() {
            return
                String.Format(
                    "{0}.{1}.{2}{3}{4}",
                    _versionMajor,
                    _versionMinor,
                    _versionPatch,
                    _versionBuildType != null ? GetTypeShortName(_versionBuildType.Value) : "",
                    _versionReleaseNumber != null ? _versionReleaseNumber.Value.ToString() : ""
                );
        }

        private static string GetTypeShortName(UnityBuildType buildType) =>
            buildType switch {
                UnityBuildType.Final => "f",
                UnityBuildType.Patch => "p",
                UnityBuildType.Alpha => "a",
                UnityBuildType.Beta => "b",
                UnityBuildType.Unknown => "",
                _ => throw new ArgumentOutOfRangeException(nameof(buildType), buildType, null)
            };

        private static int TryParseInt(string s, int defaultValue) =>
            Int32.TryParse(s, out int val) ? val : defaultValue;

        private static int? TryParseInt(string s) {
            if (!Int32.TryParse(s, out int val))
                return null;

            return val;
        }

        public enum UnityBuildType {
            Unknown,
            Final,
            Alpha,
            Beta,
            Patch
        }
    }
}
