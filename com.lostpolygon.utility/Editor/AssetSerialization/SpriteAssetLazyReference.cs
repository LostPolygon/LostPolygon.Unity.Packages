using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;

namespace LostPolygon.Unity.Utility.Editor {
    /// <summary>
    ///     <see cref=" AssetLazyReference{T}"/>
    /// </summary>
    [Serializable]
    public struct SpriteAssetLazyReference :
        IEquatable<SpriteAssetLazyReference>,
        IComparable<SpriteAssetLazyReference> {
        [SerializeField]
        private SpriteAssetReference _assetReference;

        [SerializeField]
        private int _cachedInstanceId;

        [NonSerialized]
        private Sprite _cachedInstance;

        public SpriteAssetReference AssetReference => _assetReference;

        public SpriteAssetLazyReference(SpriteAssetReference spriteAssetReference, int instanceId = 0) {
            _assetReference = spriteAssetReference;
            _cachedInstanceId = instanceId;
            _cachedInstance = null;
        }

        public Sprite GetInstance() {
            if (this.IsInvalid())
                return null;

            if (_cachedInstance == null) {
                SpriteAssetReference tmpAssetReference = _assetReference;
                _cachedInstance = AssetLazyReferenceUtility.GetInstance(
                    () => tmpAssetReference.Load(),
                    ref _cachedInstanceId
                );
            }

            return _cachedInstance;
        }

        public Dictionary<string, object> GetAssetPersistentReferenceJson() {
            return new Dictionary<string, object> {
                { "Guid", _assetReference.Guid },
                { "SpriteId", _assetReference.SpriteIdString }
            };
        }

        public override string ToString() {
            return this.IsInvalid() ? "[Invalid]" : _assetReference.ToString();
        }

        public static SpriteAssetLazyReference FromSprite(Sprite sprite) {
            string spriteTextureGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(sprite));
            if (String.IsNullOrEmpty(spriteTextureGuid))
                return default;

            return FromSprite(sprite, spriteTextureGuid);
        }

        public static SpriteAssetLazyReference FromSprite(Sprite sprite, string spriteTextureGuid) {
            return new SpriteAssetLazyReference(
                new SpriteAssetReference(spriteTextureGuid, sprite.GetSpriteID()),
                sprite.GetInstanceID()
            );
        }

        public static SpriteAssetLazyReference DeepClone(SpriteAssetLazyReference source) {
            if (source.IsInvalid())
                return default;

            SpriteAssetLazyReference spriteLazyReference = new(source._assetReference);
            return spriteLazyReference;
        }

        public bool Equals(SpriteAssetLazyReference other) {
            return _assetReference.Equals(other._assetReference);
        }

        public override bool Equals(object obj) {
            return obj is SpriteAssetLazyReference other && Equals(other);
        }

        public override int GetHashCode() {
            return _assetReference.GetHashCode();
        }

        public static bool operator ==(SpriteAssetLazyReference left, SpriteAssetLazyReference right) {
            return Equals(left, right);
        }

        public static bool operator !=(SpriteAssetLazyReference left, SpriteAssetLazyReference right) {
            return !Equals(left, right);
        }

        public int CompareTo(SpriteAssetLazyReference other) {
            return _assetReference.CompareTo(other._assetReference);
        }
    }
}
