using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LostPolygon.Unity.Utility.Editor {
    /// <summary>
    /// Project-wise reference to a persistent UnityEngine.Object asset,
    /// identified by asset GUID and subasset LocalIdentifier.
    /// </summary>
    [Serializable]
    public struct AssetReference : IEquatable<AssetReference> {
        [SerializeField]
        private string _guid;

        [SerializeField]
        private long _localIdentifier;

        public string Guid => _guid;

        public long LocalIdentifier => _localIdentifier;

        public bool Valid => !String.IsNullOrWhiteSpace(_guid);

        public AssetReference(string guid, long localIdentifier) {
            _guid = guid ?? throw new ArgumentNullException(nameof(guid));
            _localIdentifier = localIdentifier;
        }

        public Object Load() {
            return AssetDatabaseUtility.GetSubAssetByGuidAndLocalFileIdentifier(_guid, _localIdentifier);
        }

        public bool Equals(AssetReference other) {
            return
                String.Equals(_guid, other._guid, StringComparison.OrdinalIgnoreCase) &&
                _localIdentifier == other._localIdentifier;
        }

        public override bool Equals(object obj) {
            return obj is AssetReference other && Equals(other);
        }

        public override int GetHashCode() {
            HashCode hashCode = new();
            hashCode.Add(_guid, StringComparer.OrdinalIgnoreCase);
            hashCode.Add(_localIdentifier);
            return hashCode.ToHashCode();
        }

        public static bool operator ==(AssetReference left, AssetReference right) {
            return left.Equals(right);
        }

        public static bool operator !=(AssetReference left, AssetReference right) {
            return !left.Equals(right);
        }
    }
}
