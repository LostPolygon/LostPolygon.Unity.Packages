using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LostPolygon.Unity.Utility.Editor {
    /// <summary>
    /// Lazy reference to a persistent UnityEngine.Object asset.
    /// GUID and LocalIdentifier are stored,
    /// the actual object is only loaded lazily when needed.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public sealed class AssetLazyReference<T> : IEquatable<AssetLazyReference<T>> where T : Object {
        [SerializeField]
        private AssetReference _assetReference;

        [SerializeField]
        private int _cachedInstanceId;

        public AssetReference AssetReference => _assetReference;

        public AssetLazyReference(AssetReference assetReference, int instanceId = 0) {
            _assetReference = assetReference;
            _cachedInstanceId = instanceId;
        }

        public T GetInstance() {
            T instance = AssetLazyReferenceUtility.GetInstance(
                () => _assetReference.Load() as T,
                ref _cachedInstanceId
            );

            return instance;
        }

        public Dictionary<string, object> GetAssetPersistentReferenceJson() {
            return UnityAssetSerializationUtility.GetAssetPersistentReferenceJson(
                _assetReference.Guid,
                _assetReference.LocalIdentifier
            );
        }

        public bool Equals(AssetLazyReference<T> other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _assetReference.Equals(other._assetReference);
        }

        public override bool Equals(object obj) {
            return ReferenceEquals(this, obj) || (obj is AssetLazyReference<T> other && Equals(other));
        }

        public override int GetHashCode() {
            return _assetReference.GetHashCode();
        }

        public static bool operator ==(AssetLazyReference<T> left, AssetLazyReference<T> right) {
            return Equals(left, right);
        }

        public static bool operator !=(AssetLazyReference<T> left, AssetLazyReference<T> right) {
            return !Equals(left, right);
        }
    }
}
