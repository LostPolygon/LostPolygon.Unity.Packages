using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace LostPolygon.Unity.Utility.Editor {
    /// <summary>
    /// Contains information about <see cref="BuildTargetGroup"/> for use in GUI.
    /// </summary>
    public class BuildPlatform : IEquatable<BuildPlatform> {
        public readonly BuildTargetGroup BuildTargetGroup;
        public readonly int Subtarget;
        public readonly NamedBuildTarget NamedBuildTarget;
        public readonly string Name;
        public readonly string Tooltip;
        public readonly Texture SmallIcon;

        public BuildPlatform(
            BuildTargetGroup buildTargetGroup,
            int subtarget,
            NamedBuildTarget namedBuildTarget,
            string name,
            string tooltip,
            Texture smallIcon
        ) {
            BuildTargetGroup = buildTargetGroup;
            Name = name;
            Tooltip = tooltip;
            SmallIcon = smallIcon;
            Subtarget = subtarget;
            NamedBuildTarget = namedBuildTarget;
        }

        public BuildPlatformId BuildPlatformId => new(BuildTargetGroup, Subtarget);

        public bool Equals(BuildPlatform other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return BuildPlatformId == other.BuildPlatformId;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((BuildPlatform) obj);
        }

        public override int GetHashCode() {
            return BuildPlatformId.GetHashCode();
        }

        public static bool operator ==(BuildPlatform left, BuildPlatform right) {
            return Equals(left, right);
        }

        public static bool operator !=(BuildPlatform left, BuildPlatform right) {
            return !Equals(left, right);
        }

        public override string ToString() {
            return NamedBuildTarget.TargetName;
        }
    }
}
