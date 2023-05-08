using System;
using UnityEditor;
using UnityEngine;

namespace LostPolygon.Unity.Utility.Editor {
    [Serializable]
    public struct BuildPlatformId :
        IEquatable<BuildPlatformId>,
        IComparable<BuildPlatformId> {
        public static BuildPlatformId Default { get; } = new((BuildTargetGroup) (-1), 0);

        public BuildTargetGroup BuildTargetGroup => _buildTargetGroup;

        public int Subtarget => _subtarget;

        [SerializeField]
        private BuildTargetGroup _buildTargetGroup;

        [SerializeField]
        private int _subtarget;

        public BuildPlatformId(BuildTargetGroup buildTargetGroup, int subtarget) {
            _buildTargetGroup = buildTargetGroup;
            _subtarget = subtarget;
        }

        public bool Equals(BuildPlatformId other) {
            return BuildTargetGroup == other.BuildTargetGroup && Subtarget == other.Subtarget;
        }

        public override bool Equals(object obj) {
            return obj is BuildPlatformId other && Equals(other);
        }

        public override int GetHashCode() {
            return HashCode.Combine((int) BuildTargetGroup, Subtarget);
        }

        public static bool operator ==(BuildPlatformId left, BuildPlatformId right) {
            return left.Equals(right);
        }

        public int CompareTo(BuildPlatformId other) {
            int buildTargetGroupComparison = _buildTargetGroup.CompareTo(other._buildTargetGroup);
            if (buildTargetGroupComparison != 0) return buildTargetGroupComparison;
            return _subtarget.CompareTo(other._subtarget);
        }

        public static bool operator !=(BuildPlatformId left, BuildPlatformId right) {
            return !left.Equals(right);
        }

        public override string ToString() {
            return $"{nameof(BuildTargetGroup)}: {BuildTargetGroup}, {nameof(Subtarget)}: {Subtarget}";
        }
    }
}
