using System;
using System.IO;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LostPolygon.Unity.Utility.Editor {
    /// <summary>
    /// Project-wise reference to a persistent UnityEngine.Object asset,
    /// identified by asset GUID and subasset LocalIdentifier.
    /// </summary>
    [Serializable]
    public struct SpriteAssetReference :
        IEquatable<SpriteAssetReference>,
        IComparable<SpriteAssetReference> {
        [SerializeField]
        private string _guid;

        [SerializeField]
        private string _spriteId;

        [NonSerialized]
        private GUID? _spriteIdUnity;

        public string Guid => _guid;

        public GUID SpriteId =>
            _spriteIdUnity ??=
                GUID.TryParse(_spriteId, out GUID parsed) ?
                    parsed :
                    throw new InvalidDataException($"'{_guid}' is not a valid GUID");

        public string SpriteIdString => _spriteId;

        public SpriteAssetReference(string guid, string spriteId) {
            _guid = guid;
            _spriteId = spriteId;
            _spriteIdUnity = null;
        }

        public SpriteAssetReference(string guid, GUID spriteId) {
            _guid = guid;
            _spriteId = spriteId.ToString();
            _spriteIdUnity = spriteId;
        }

        public Sprite Load() {
            string assetPath = AssetDatabase.GUIDToAssetPath(_guid);
            if (String.IsNullOrEmpty(assetPath))
                return null;

            Object[] objects = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            foreach (Object obj in objects) {
                if (obj is not Sprite sprite)
                    continue;

                if (sprite.GetSpriteID() == SpriteId)
                    return sprite;
            }

            return null;
        }

        public override string ToString() {
            return $"{_guid}:{_spriteId}";
        }

        public bool Equals(SpriteAssetReference other) {
            return
                StringComparer.OrdinalIgnoreCase.Equals(_spriteId, other._spriteId) &&
                StringComparer.OrdinalIgnoreCase.Equals(_guid, other._guid);
        }

        public override bool Equals(object obj) {
            return obj is SpriteAssetReference other && Equals(other);
        }

        public override int GetHashCode() {
            return HashCode.Combine(
                _spriteId != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(_spriteId) : 0,
                _guid != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(_guid) : 0
            );
        }

        public static bool operator ==(SpriteAssetReference left, SpriteAssetReference right) {
            return left.Equals(right);
        }

        public static bool operator !=(SpriteAssetReference left, SpriteAssetReference right) {
            return !left.Equals(right);
        }

        public int CompareTo(SpriteAssetReference other) {
            int guidComparison = String.Compare(_guid, other._guid, StringComparison.OrdinalIgnoreCase);
            if (guidComparison != 0)
                return guidComparison;

            return String.Compare(_spriteId, other._spriteId, StringComparison.OrdinalIgnoreCase);
        }
    }
}
